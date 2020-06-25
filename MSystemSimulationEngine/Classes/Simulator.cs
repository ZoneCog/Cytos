using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MathNet.Numerics.Distributions;
using MathNet.Spatial.Euclidean;
using MSystemSimulationEngine.Classes.Tools;
using MSystemSimulationEngine.Classes.Xml;
using SharedComponents.Tools;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Provides central place of simulation process.
    /// </summary>
    public class Simulator : SimulationWorld
    {
        #region Private data

        /// <summary>
        /// Simulation step counter.
        /// </summary>
        private uint v_Step;

        /// <summary>
        /// Serialized snapshots XML document.
        /// </summary>
        private readonly SerializeSnapshot v_SnapshotXmlDoc;

        /// <summary>
        /// Main notification message channel
        /// </summary>
        private NotificationMessage v_Notification = new NotificationMessage();

        /// <summary>
        /// Flag whether serialize floating object to snapshot or not.
        /// </summary>
        private readonly bool v_SerializeFloatingObjects;

        #endregion

        #region Public data

        /// <summary>
        /// Enum used for controling simulation engine.
        /// </summary>
        public enum SimulationControlFlags
        {
            Run, Pause, Stop
        }

        /// <summary>
        /// Static parameters which handles current simulation engine state.
        /// </summary>
        public static SimulationControlFlags SimulationCurrentState = SimulationControlFlags.Stop;

        #endregion

        #region Constructor

        /// <summary>
        /// Simulation constructor.
        /// </summary>
        /// <param name="mSystemObjects">Deserialized M System objects.</param>
        /// <param name="serializeFloatingObjects">Flag whether serialize floating object to snapshot or not.</param>
        public Simulator(DeserializedObjects mSystemObjects, bool serializeFloatingObjects) :
            base(mSystemObjects)
        {
            v_SnapshotXmlDoc = new SerializeSnapshot(mSystemObjects.MSystemFilePath);
            v_SerializeFloatingObjects = serializeFloatingObjects;
            FinalizeStep();

            // For cTAM we log an initial description of the system
            if (SimulationMSystem.Nu0 > 0)
            {
                string message = $"cTAM description: Nu_0={SimulationMSystem.Nu0}, Tau={SimulationMSystem.Tau}";
                foreach (Tile tile in SimulationMSystem.Tiles.Values)
                {
                    message += "\r\nTile " + tile.Name + ", Alpha =" + tile.AlphaRatio;
                }
                Logging.LogSimulationMessage(message);
                LogOneCtamStep();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Run one step of the simulation.
        /// </summary>
        /// <returns>
        /// Return true if the P system did not halt - in this case the next step can be run.
        /// </returns>
        private bool RunOneSimulationStep()
        {
            // Order of rules is important:
            // Destruction and division rules mark some connectors as disconnected so that they establish no new connection in the same step.
            // Creation rules must be the last as they move (push) floating objects by newly created tiles

            InitializeStep();
            v_Step++;

            NamedMultiset metabolicRules = ApplyMetabolicRules();
            NamedMultiset destructionRules = ApplyDestructionRules();
            NamedMultiset divisionRules = ApplyDivisionRules();
            NamedMultiset insertionRules = ApplyInsertionRules();
            NamedMultiset creationRules = ApplyCreationRules();

            //Locked tiles must be counted before FinalStep as this method unlock tiles. 
            int numberOfLockedTiles = TilesWorld.Count(x => x.IsLocked);

            FinalizeStep();


            bool result = metabolicRules.Any() || destructionRules.Any() || divisionRules.Any() || insertionRules.Any() || creationRules.Any() || numberOfLockedTiles != 0;

            // Provide a dump of the step in the log file
            if (result)
            {
                if (SimulationMSystem.Nu0 > 0)
                    LogOneCtamStep();
                else
                    Logging.LogSimulationMessage($"Step: {v_Step}\r\n" +
                                                 "Applied rules:\r\n" +
                                                 metabolicRules + (metabolicRules.Count > 0 ? "\r\n" : "") +
                                                 destructionRules + (destructionRules.Count > 0 ? "\r\n" : "") +
                                                 divisionRules + (divisionRules.Count > 0 ? "\r\n" : "") +
                                                 insertionRules + (insertionRules.Count > 0 ? "\r\n" : "") +
                                                 creationRules + (creationRules.Count > 0 ? "\r\n" : "") +
                                                 $"Tiles: {TilesWorld.Count()} \r\n" +
                                                 $"{FltObjectsWorld} \r\n" +
                                                 $"Number of locked tiles: {TilesWorld.Count(x => x.IsLocked)}");
            }
            return result;
        }


        /// <summary>
        /// Logs one step of a cTAM simulation.
        /// </summary>
        private void LogOneCtamStep()
        {
            string dump = $"Step: {v_Step}\r\n";
            foreach (TileInSpace seedTile in SimulationMSystem.SeedTiles)
            {
                string ladder = "Ladder:";
                string voltages = "Voltages:";
                TileInSpace tile = seedTile;
                while (tile != null)
                {
                    ladder += tile.Name.PadLeft(7);
                    voltages += (tile.EastConnector?.Voltage ?? -1.0).ToString("F3").PadLeft(7);
                    tile = tile.EastConnector?.ConnectedTo?.OnTile;
                }

                dump += "\r\n" + ladder + "\r\n" + voltages + "\r\n";
            }
            Logging.LogSimulationMessage(dump);
        }


        /// <summary>
        /// Apply metabolic rules in the P system.
        /// Rules are chosen non-deterministically.
        /// Maximally parallel mode (simulated) is used.
        /// </summary>
        /// <returns>
        /// Multiset of applied rules.
        /// </returns>
        /// <remarks>Protected accessibility for testing only</remarks>
        protected NamedMultiset ApplyMetabolicRules()
        {
            NamedMultiset appliedRules = new NamedMultiset();

            // List of free proteins on all existing tiles
            ProteinOnTileInSpace[] freeProteins = TilesWorld.Where(tile => !tile.IsLocked).SelectMany(tile => tile.Proteins.Where(protein => !protein.IsUsed)).ToArray();
            freeProteins.Shuffle();

            foreach (ProteinOnTileInSpace freeProtein in freeProteins)
            {
                TileInSpace tile = freeProtein.m_tile;

                // List of available metabolic rules for this protein is constructed
                // For protein on a 1D object only catalyzed rules are applicable.
                EvoMetabolicRule[] availableRules = SimulationMSystem.MetabolicRules[freeProtein.Name].Where(
                         rule => tile.Vertices is Polygon3D || rule.SubType == EvoMetabolicRule.MetabolicRuleType.Catalyzed).ToArray();
                availableRules.Shuffle();

                // Optimization - calculate the minimum multisets of objects to apply any rule in the list
                NamedMultiset unionOfLeftInNames = NamedMultiset.Union(availableRules.Select(rule => rule.MLeftInNames));
                NamedMultiset unionOfLeftOutNames = NamedMultiset.Union(availableRules.Select(rule => rule.MLeftOutNames));

                FloatingObjectsSet innerSet = FltObjectsWorld.GetNearObjects(freeProtein, Tile.SideType.inside, unionOfLeftInNames);
                FloatingObjectsSet outerSet = FltObjectsWorld.GetNearObjects(freeProtein, Tile.SideType.outside, unionOfLeftOutNames);

                // TODO change to FirstOrDefault
                foreach (EvoMetabolicRule rule in availableRules)
                {
                    // Floating objects cannot be added to narrow spaces where two tiles are face-to-face TODO move outside the cycle
                    if ((rule.SubType == EvoMetabolicRule.MetabolicRuleType.Catalyzed &&
                                   rule.MLeftInNames.Count < rule.MRightInNames.Count &&
                                   TilesWorld.InNarrowSpace(freeProtein.Position, tile, true))
                               ||
                                  (rule.SubType == EvoMetabolicRule.MetabolicRuleType.Symport &&
                                  TilesWorld.InNarrowSpace(freeProtein.Position, tile, rule.MRightInNames.Any())))
                        continue;

                    // Both multisets of floating objects on the left-hand side of the rule must be present in the cells
                    if (rule.MLeftInNames.IsSubsetOf(innerSet.ToMultiset()) &&
                        rule.MLeftOutNames.IsSubsetOf(outerSet.ToMultiset()))

                    {   // Apply the rule.
                        appliedRules.Add(rule.Name);
                        // Each protein can be in one step used in max. one rule.
                        freeProtein.IsUsed = true;

                        FltObjectsWorld.RemoveFrom(rule.MLeftInNames, innerSet);
                        FltObjectsWorld.AddAt(rule.MRightInNames, tile.SidePoint(freeProtein.Position, Tile.SideType.inside));
                        FltObjectsWorld.RemoveFrom(rule.MLeftOutNames, outerSet);
                        FltObjectsWorld.AddAt(rule.MRightOutNames, tile.SidePoint(freeProtein.Position, Tile.SideType.outside));

                        break;
                    }
                }
            }
            return appliedRules;
        }

        /// <summary>
        /// Apply destruction rules in the P system.
        /// Rules are chosen non-deterministically.
        /// Maximally parallel mode (simulated) is used.
        /// </summary>
        /// <returns>
        /// Multiset of applied rules.
        /// </returns>
        /// <remarks>Protected accessibility for testing only</remarks>
        protected NamedMultiset ApplyDestructionRules()
        {
            NamedMultiset appliedRules = new NamedMultiset();
            TileInSpace[] tiles = TilesWorld.ToArray();
            tiles.Shuffle();

            foreach (TileInSpace tile in tiles)
            {
                // List of destruction rules which can be applied to this object
                // The multisets of floating objects on the left-hand side of the rule must be present in reaction distance from the object
                EvoNonMetabolicRule[] availableRules = SimulationMSystem.DestructionRules[tile.Name].ToArray();

                // Optimization - calculate the minimum multiset of objects to apply any rule in the list
                NamedMultiset unionOfLeftNames = NamedMultiset.Union(availableRules.Select(r => r.MLeftSideFloatingNames));

                FloatingObjectsSet nearObjects = FltObjectsWorld.GetNearObjects(tile, unionOfLeftNames);
                NamedMultiset mNearObjects = nearObjects.ToMultiset();

                availableRules.Shuffle();
                EvoNonMetabolicRule rule = availableRules.FirstOrDefault(r => r.MLeftSideFloatingNames.IsSubsetOf(mNearObjects));

                if (rule != null)
                {
                    // Mark the object (it is physically removed at the final phase of the current step)
                    tile.State = TileInSpace.FState.Destroy;
                    FltObjectsWorld.RemoveFrom(rule.MLeftSideFloatingNames, nearObjects);
                    FltObjectsWorld.AddAt(rule.MRightSideFloatingNames, tile.Position);
                    appliedRules.Add(rule.Name);
                }
            }
            return appliedRules;
        }

        /// <summary>
        /// Apply division rules in the P system.
        /// Rules are chosen non-deterministically.
        /// Maximally parallel mode (simulated) is used.
        /// </summary>
        /// <returns>
        /// Multiset of applied rules.
        /// </returns>
        /// <remarks>Protected accessibility for testing only</remarks>
        protected NamedMultiset ApplyDivisionRules()
        {
            NamedMultiset appliedRules = new NamedMultiset();

            // List of used (interconnected) connectors on all existing objects
            // Each connector pair has only one its member in the list
            ConnectorOnTileInSpace[] availableConnectors = TilesWorld.SelectMany(tile => tile.Connectors.Where(connector =>
                    connector.ConnectedTo != null &&
                    tile.ID < connector.ConnectedTo.OnTile.ID))
                .ToArray();
            availableConnectors.Shuffle();

            foreach (ConnectorOnTileInSpace connector in availableConnectors)
            {
                EvoNonMetabolicRule[] availableRules = { };
                try
                {
                    availableRules = SimulationMSystem.DivisionRules[connector.Glue][connector.ConnectedTo.Glue].ToArray();
                }
                catch (KeyNotFoundException)
                {
                    // No action needed - the array availableRules is left empty
                }

                // TODO in this and the next two methods move this part into a separate method returning shuffled list of available rules 
                // Optimization - calculate the minimum multiset of objects to apply any rule in the list
                NamedMultiset unionOfLeftNames = NamedMultiset.Union(availableRules.Select(r => r.MLeftSideFloatingNames));

                FloatingObjectsSet nearObjects = FltObjectsWorld.GetNearObjects(connector, unionOfLeftNames);
                NamedMultiset mNearObjects = nearObjects.ToMultiset();

                availableRules.Shuffle();
                EvoNonMetabolicRule rule = availableRules.FirstOrDefault(r => r.MLeftSideFloatingNames.IsSubsetOf(mNearObjects));

                if (rule != null)
                {
                    // Apply the rule and mark the connectors for disconnection 
                    FltObjectsWorld.RemoveFrom(rule.MLeftSideFloatingNames, nearObjects);
                    connector.ConnectedTo.SetDisconnect = true;
                    connector.SetDisconnect = true;
                    appliedRules.Add(rule.Name);
                }
            }
            return appliedRules;
        }

        /// <summary>
        /// Apply insertion rules in the P system.
        /// Rules are chosen non-deterministically.
        /// Maximally parallel mode (simulated) is used.
        /// TODO at the moment allows only insertion of rods, generalize also for 2D tiles
        /// </summary>
        /// <returns>
        /// Multiset of applied rules.
        /// </returns>
        /// <remarks>Protected accessibility for testing only</remarks>
        protected NamedMultiset ApplyInsertionRules()
        {
            /* 0. Take  connectors to which, due to glues, an insertion rule can be applied, shuffle them to an array <insertableConnectors>
             * 1. While <insertableConnectors> is nonempty, repeat steps 2 to 8
             * 2. Choose a connector c in <insertableConnectors>, check whether there are necessary floating objects nearby
             * 3. If yes, get the component K of fixed objects containing this connector
             * 4. take all connectors in <insertableConnectors> which connect tiles in K, find a minimal cut C_min for the two tiles connected by c by removing connectors not in the cut
             * 5. check whether insertion rules can be applied to all connectors in C_min = whether there are enough floating objects 
             *      (disconnect connectors and remove floating objects on the fly); 
             *      in the affirmative case, try to apply insertion rule to c
             * 6. if YES (the rule was applied), then apply all insertion rules to C_min-{c}; 
             *      if some would not work (due to pushing), throw an exception INCONSISTENT INSERTION ATTEMPT
             * 7. if NO, the rollback - reconnect connectors in C_min and return back floating objects
             * 8. Remove all connectors in C_min from <insertableConnectors>
             */
            NamedMultiset appliedRules = new NamedMultiset();

            // List of used (interconnected) connectors on all existing objects
            // Each connector pair has only one its member in the list
            List<ConnectorOnTileInSpace> insertableConnectors = TilesWorld.SelectMany(tile => tile.Connectors.Where(connector =>
                    tile.State == TileInSpace.FState.Unchanged &&
                    !connector.SetDisconnect &&
                    connector.ConnectedTo != null &&
                    tile.ID < connector.ConnectedTo.OnTile.ID &&
                    SimulationMSystem.InsertionRules.ContainsKey(connector.Glue) &&
                    SimulationMSystem.InsertionRules[connector.Glue].ContainsKey(connector.ConnectedTo.Glue)))
                .ToList();

            insertableConnectors.Shuffle();

            while (insertableConnectors.Any())
            {
                ConnectorOnTileInSpace connector = insertableConnectors[0];

                foreach (EvoNonMetabolicRule rule in InsertableRules(connector))
                {
                    //     && TilesWorld.Insert(r.RightSideObjects.OfType<Tile>().Single(), connector));
                    // TODO continue here
                    if (rule != null)
                    {
                        // Remove the floating objects consumed in creating the new tiles and exit
                        //FltObjectsWorld.RemoveFrom(rule.MLeftSideFloatingNames, nearObjects);
                        //appliedRules.Add(rule.Name);

                    }
                }

            }
            return appliedRules;
        }


        /// <summary>
        /// Returns shuffled set of rules applicable to insert a tile in a given connector.
        /// Each rule must match glues, connector sizes and nearby floating objects.
        /// Pushing not checked.
        /// </summary>
        /// <param name="connector">A connected connector (exception otherwise!)</param>
        /// <returns>
        /// Multiset of applicable rules.
        /// </returns>
        private IEnumerable<EvoNonMetabolicRule> InsertableRules(ConnectorOnTileInSpace connector)
        {
            // Both keys must exist - possible exception!
            EvoNonMetabolicRule[] availableRules = SimulationMSystem.InsertionRules[connector.Glue][connector.ConnectedTo.Glue]
                .Where(rule => MatchingConectors(rule.RightSideObjects.OfType<Tile>().Single(), connector).Item1 != null)
                .ToArray();

            // Optimization - calculate the minimum multiset of objects to apply any rule in the list
            NamedMultiset unionOfLeftNames = NamedMultiset.Union(availableRules.Select(r => r.MLeftSideFloatingNames));

            FloatingObjectsSet nearObjects = FltObjectsWorld.GetNearObjects(connector, unionOfLeftNames);
            NamedMultiset mNearObjects = nearObjects.ToMultiset();

            availableRules.Shuffle();
            return availableRules.Where(r => r.MLeftSideFloatingNames.IsSubsetOf(mNearObjects));
        }


        /// <summary>
        /// Returns a pair of facet connectors at distinct positions on "tile" which can be inserted in "connector" 
        /// due to GlueRelation and shape compatibility.
        /// If no such pair exist, then both returned connectors are null.
        /// </summary>
        private Tuple<ConnectorOnTile, ConnectorOnTile> MatchingConectors(Tile tile, ConnectorOnTileInSpace connector)
        {
            Tuple<ConnectorOnTile, ConnectorOnTile> pair = SimulationMSystem.MatchingConectors(tile, connector.Glue, connector.ConnectedTo.Glue);
            if (pair.Item1.IsSizeComppatibleWith(connector) && pair.Item2.IsSizeComppatibleWith(connector))
                return pair;
            ConnectorOnTile nope = null;
            return Tuple.Create(nope, nope);
            // TODO very low priority: return all possible pairs instead of a single pair
        }

        /// <summary>
        /// Apply creation rules in the P system.
        /// Rules are chosen non-deterministically.
        /// Maximally PARALLEL mode should be used but is complicated due to pushing and interacions of fixed and floating objects.
        /// Hence, new objects are created SEQUENTIALLY, one by one.
        /// </summary>
        /// <returns>
        /// Multiset of applied rules.
        /// </returns>
        /// <remarks>Protected accessibility for testing only</remarks>
        protected NamedMultiset ApplyCreationRules()
        {
            NamedMultiset appliedRules = new NamedMultiset();

            // List of connection sites on all existing non-destroyed objects to which a new object can attach.
            ConnectorOnTileInSpace[] availableConnectors = TilesWorld.Where(tile => tile.State != TileInSpace.FState.Destroy && !tile.IsLocked)
                    .SelectMany(tile => tile.Connectors).ToArray();
            if (SimulationMSystem.RandomizeConnectors)
                availableConnectors.Shuffle();

            foreach (int priority in SimulationMSystem.CreationRulesPriorities)
            {
                foreach (ConnectorOnTileInSpace connector in availableConnectors)
                {
                    if (connector.ConnectedTo != null)   // must be tested here since connectors may auto-connect in each iteration
                        continue;

                    // If the connector on segment touches a tile, then it cannot be used
                    // TODO correct the case when such a tile can be pushed
                    if (connector.OnTile.Vertices is Segment3D &&
                        TilesWorld.Any(tile => tile.Vertices is Polygon3D && tile.Vertices.ContainsPoint(connector.Positions[0])))
                        continue;

                    // List of creation rules which can be applied to this connection site
                    EvoNonMetabolicRule[] availableRules = SimulationMSystem.CreationRules[connector.Glue].Where(r => r.Priority == priority).ToArray();

                    // Optimization - calculate the minimum multiset of objects to apply any rule in the list
                    NamedMultiset unionOfLeftNames = NamedMultiset.Union(availableRules.Select(r => r.MLeftSideFloatingNames));

                    FloatingObjectsSet nearObjects = FltObjectsWorld.GetNearObjects(connector, unionOfLeftNames);
                    NamedMultiset mNearObjects = nearObjects.ToMultiset();

                    // Method ADD tries to attach the tile in the rule to the "connector" 
                    // Returns false if there are surrounding tiles in its way which cannot be pushed
                    availableRules.Shuffle();

                    TileInSpace newlyCreatedTile = null;
                    EvoNonMetabolicRule rule = availableRules.FirstOrDefault(r => r.MLeftSideFloatingNames.IsSubsetOf(mNearObjects)
                        && TilesWorld.Add(r.RightSideObjects.OfType<Tile>().Single(), connector, out newlyCreatedTile));

                    if (rule != null)
                    {
                        if (rule.Delay > 0 && newlyCreatedTile != null)
                        {
                            newlyCreatedTile.IsLocked = true;
                            // make distribution flatter but keep getting value until you get one within the bounds
                            double normalDistValue = Normal.Sample(Randomizer.Rng, rule.Delay, (double)rule.Delay / 2);
                            while (normalDistValue < 0 || normalDistValue > 2 * rule.Delay)
                            {
                                normalDistValue = Normal.Sample(Randomizer.Rng, rule.Delay, (double)rule.Delay / 2);
                            }
                            uint readyAtStep = (uint)Math.Round(v_Step + normalDistValue);
                            newlyCreatedTile.ReadyAtStep = readyAtStep;
                            Logging.LogSimulationMessage($"Tile name: {newlyCreatedTile.Name} | current step: {v_Step} | ready at: {readyAtStep} | normal distribution: {normalDistValue}.");
                        }

                        // Remove the floating objects consumed in creating the new tiles and exit
                        FltObjectsWorld.RemoveFrom(rule.MLeftSideFloatingNames, nearObjects);
                        appliedRules.Add(rule.Name);

                        // cTAM works in sequential mode - add one tile per step
                        if (SimulationMSystem.Nu0 > 0)
                            return appliedRules;
                    }
                }
            }
            return appliedRules;
        }

        /// <summary>
        /// Complete simulation data in the P system at the end of each step.
        /// </summary>
        private void InitializeStep()
        {
            foreach (TileInSpace tile in TilesWorld)
                foreach (ProteinOnTileInSpace protein in tile.Proteins)
                    protein.IsUsed = false;
        }

        /// <summary>
        /// Complete simulation data in the M system at the end of each step.
        /// </summary>
        /// <remarks>Protected accessibility for testing only</remarks>
        protected void FinalizeStep()
        {
            FltObjectsWorld.FinalizeStep();
            TileInSpace[] tileArray = TilesWorld.ToArray();

            foreach (TileInSpace tile in tileArray)
                foreach (ConnectorOnTileInSpace connector in tile.Connectors)
                {
                    // Disconnect marked connectors - must be done before snapshot serialization
                    if (connector.SetDisconnect)
                        connector.Disconnect();
                }

            // If the M system is a cTAM, calculate electric charges and re-color tiles accordingly
            if (SimulationMSystem.Nu0 > 0)
                Electric.CalculateCharges(SimulationMSystem);

            //Unlock tile and make it available for simulation.
            List<TileInSpace> tilesInSpaceToBeUnlocked = tileArray.Where(tile => tile.IsLocked && v_Step >= tile.ReadyAtStep).ToList();
            foreach (TileInSpace tileInSpace in tilesInSpaceToBeUnlocked)
            {
                tileInSpace.IsLocked = false;
                tileInSpace.ReadyAtStep = 0;
            }

            Xmlizer.AddSnapshot(v_Step,
                v_SerializeFloatingObjects ? FltObjectsWorld.ToSet() : new FloatingObjectsSet(),
                TilesWorld,
                v_SnapshotXmlDoc);

            foreach (TileInSpace tile in tileArray)
            {
                tile.ColorWasChanged = false; //Reset color changed flag.

                // Remove tile marked for destruction, do not change its state
                if (tile.State == TileInSpace.FState.Destroy)
                    TilesWorld.Remove(tile);
                else
                    tile.State = TileInSpace.FState.Unchanged;

                //PerformHealthCheck();
            }

            if (SimulationMSystem.TilingRandomMovement > 0)
            {
                TilesWorld.RandomlyMoveTiles(SimulationMSystem.TilingRandomMovement);
            }
        }

        /// <summary>
        /// Debugging purposes only
        /// </summary>
        private void PerformHealthCheck()
        {
            // perform health check for every tile in collection
            List<TileInSpace> tilesList = TilesWorld.ToList();
            tilesList.ForEach(tile =>
            {
                foreach (TileInSpace checkedTile in tilesList)
                {
                    PerformHealthCheck(tile, checkedTile);
                }
            });
        }

        /// <summary>
        /// Debugging purposes only
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="checkedTile"></param>
        private void PerformHealthCheck(TileInSpace tile, TileInSpace checkedTile)
        {
            // do not health check the same object against itself
            if (tile.Equals(checkedTile)) return;

            if (tile.OverlapsWith(checkedTile))
            { }
            if (tile.IntersectsWith(checkedTile))
            { }
        }

        /// <summary>
        /// Try to return tile at a specific position. Tile name and type match is performed before removal.
        /// </summary>
        /// <param name="position">Position of the tile in TilesWorld</param>
        /// <param name="tileName">Tile name to be matched, empty string always generates match.</param>
        /// <param name="onlyPologon3DTiles">Check whether tile is of Pologon3D type.</param>
        /// <returns></returns>
        private bool HurtSystemTryToRemoveTile(int position, string tileName, bool onlyPologon3DTiles)
        {
            // get the tile
            TileInSpace tile = TilesWorld.ElementAt(position);
            // empty name always generates name match
            if (tileName == "" || tile.Name == tileName)
            {
                // do we want to remove only Pologon3D tiles?
                if (onlyPologon3DTiles)
                {
                    if (tile.Vertices is Polygon3D)
                    {
                        TilesWorld.Remove(tile);
                        return true;
                    }
                }
                else
                {
                    TilesWorld.Remove(tile);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Hurts system by removing randomly numberOfKills of type tileName. If tileName is empty string then we do not care about the type of the object
        /// </summary>
        /// <param name="tileName">Name of the tile.</param>
        /// <param name="numberOfKills">Number of object to be hurt.</param>
        /// <param name="onlyPologon3DTiles">If TRUE then only Polygon3D tiles will be removed, otherwise any type including rods.</param>
        private void HurtSystem(string tileName, int numberOfKills, bool onlyPologon3DTiles = false)
        {
            for (int i = 0; i < numberOfKills; i++)
            {
                // always use global randomizer
                int randPosition = Randomizer.Next(TilesWorld.Count());

                // iterate over the collection try to find first available tile for removal
                bool removed = false;
                // try to find object from random position going forward
                for (int j = randPosition; j < TilesWorld.Count(); j++)
                {
                    if (HurtSystemTryToRemoveTile(j, tileName, onlyPologon3DTiles))
                    {
                        removed = true;
                        break;
                    }
                }
                // we did not find object for removal, go backwards
                if (!removed)
                {
                    for (int j = randPosition - 1; j >= 0; j--)
                    {
                        if (HurtSystemTryToRemoveTile(j, tileName, onlyPologon3DTiles))
                        {
                            removed = true;
                            break;
                        }
                    }
                }
                // there is no more object of this type in TilesWorld => do not waste 
                // more time continuing in the loop
                if (!removed) break;
            }
        }

        /// <summary>
        /// Pause simulation loop until SimulationCurrentState changes.
        /// </summary>
        private void PauseCurrentThread()
        {
            while (SimulationCurrentState == SimulationControlFlags.Pause)
            {
                Thread.Sleep(1000);
            }

            v_Notification.Message = "Simulation continue";
        }

        /// <summary>
        /// Count number of Polygon3D tiles in a component
        /// </summary>
        /// <param name="component">Component to be evaluated.</param>
        /// <returns>Number of 3D tiles in a component.</returns>
        private int Count3DTilesInComponent(HashSet<TileInSpace> component)
        {
            int numberOfTiles = 0;
            foreach (TileInSpace element in component)
            {
                // only polygon tiles matter
                if (element.Vertices is Polygon3D)
                {
                    numberOfTiles++;
                }
            }

            return numberOfTiles;
        }

        /// <summary>
        /// Count how many componenets consists of 0, 1, 2,...40 tiles. This gives the overview how complete is the system
        /// </summary>
        /// <param name="startString">Start of the resulting string. This way, method can append new information to existing string</param>
        private string CountComponents(string startString)
        {
            string result = ""; // result being returned

            // initiate component stats
            Dictionary<int, int> componentStats = new Dictionary<int, int>();
            for (int i = 0; i < 41; i++)
            {
                componentStats.Add(i, 0);
            }

            // get copy of all tiles
            HashSet<TileInSpace> polygonTiles = new HashSet<TileInSpace>(TilesWorld.PolygonTiles);

            while (polygonTiles.Count != 0)
            {
                // get first tile in the collection and get its component, e.g. all tiles connected to it
                TileInSpace tile = polygonTiles.First();
                HashSet<TileInSpace> component = tile.Component();
                // get number of 3D tiles in a component
                int numberOfTiles = Count3DTilesInComponent(component);
                // update stats
                componentStats[numberOfTiles]++;
                // remove all tiles of the component form the list
                foreach (TileInSpace element in component)
                {
                    polygonTiles.Remove(element);
                }
            }

            // update string
            result += startString;
            for (int i = 0; i < componentStats.Count(); ++i)
            {
                result += string.Format(",{0}", componentStats[i]);
            }
            result += "\n";

            return result;
        }

        #endregion

        #region Public Methods

        //TODO We have currenty to much "RunSimulation" method. There Should be only one entry point with configuration instead of so many heparated functionality.

        /// <summary>
        /// Run simulation loop. Can be called repeatedly.
        /// </summary>
        /// <param name="numberOfSteps">
        /// Number of steps of the simulation.
        /// If unlimited run (no steps defined, run till the end) => use numberOfSteps = 0;
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If exception occured during simulation run
        /// </exception>
        public void RunSimulation(object numberOfSteps)
        {
            try
            {
                ulong stepsNumber = TypeUtil.Cast<ulong>(numberOfSteps);
                // stepsNumber = 0 => unlimited number of simulation steps.
                ulong stepsElapsed = 0;
                bool runNextStep = true;
                while ((stepsNumber == 0 || stepsElapsed < stepsNumber) && runNextStep && SimulationCurrentState != SimulationControlFlags.Stop)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    runNextStep = RunOneSimulationStep();
                    sw.Stop();
                    stepsElapsed++;


                    v_Notification.Message = string.Format("--- Step {0} done in {1}", v_Step, sw.Elapsed);

                    //if (SimulationCurrentState == SimulationControlFlags.Pause)
                    //{
                    //    Logging.LogSimulationMessageAndVisualize("Simulation paused");
                    //    PauseCurrentThread();
                    //}
                }

                v_Notification.Message = string.Format("Simulation completed {0} steps", stepsElapsed);
            }
            catch (Exception ex)
            {
                SaveSnapshots();
                throw new InvalidOperationException(string.Format("Exception occured during simulation run: {0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// Run simulation with speficied number of steps and try to hurt the system by removing random object of type specified by tileName
        /// </summary>
        /// <param name="numberOfSteps">Number of simulation steps.</param>
        /// <param name="tileName">Name of the tile.</param>
        /// <param name="numberOfKills">Number of object to be hurt.</param>
        /// <param name="probabilistic">Flag for using random mechanism of choosing step.</param>
        public void RunSimulation(int numberOfSteps, string tileName, long numberOfKills, bool probabilistic)
        {
            // if numberOfKills is greater than number of steps then we will be killing in each (but not in the first one)
            int sureKills = (int)(numberOfKills / ((long)numberOfSteps - 1));
            int remainingKills = (sureKills == 0) ? (int)numberOfKills : ((int)numberOfKills - (sureKills * (numberOfSteps - 1)));

            // build hurt table
            List<int> hurtTable = new List<int>(numberOfSteps);
            for (int i = 0; i < numberOfSteps; i++)
            {
                if (i == 0)
                    hurtTable.Add(0);
                else
                    hurtTable.Add(sureKills);
            }

            if (probabilistic)
            {
                Random rand = new Random();

                long kills = 0;
                while (kills < remainingKills)
                {
                    int step = rand.Next(numberOfSteps);
                    // do not accept step zero or already selected step => accepted step greater than sureKills
                    if (step == 0 || hurtTable[step] > sureKills)
                        continue;
                    hurtTable[step]++;
                    kills++;
                }
            }
            else
            {
                // this may look a bit tricky but it does exactly what we want
                // kill steps are marked from the back and we mustn't mark step zero
                if (remainingKills > 0)
                {
                    int killEveryXStep = (int)((long)numberOfSteps / remainingKills);
                    for (int i = hurtTable.Count - 1; i > 0; i -= killEveryXStep)
                        hurtTable[i]++;
                }
            }

            // run required number of simulation steps
            int stepsElapsed = 0;
            while (stepsElapsed < numberOfSteps && RunOneSimulationStep())
            {
                // do we want to hurt system in this step
                if (hurtTable[stepsElapsed] != 0)
                    HurtSystem(tileName, hurtTable[stepsElapsed]);
                stepsElapsed++;
            }
            SaveSnapshots(false);
        }

        /// <summary>
        /// Run simulation with speficied number of steps and try to hurt the system by removing random object of type specified by tileName
        /// </summary>
        /// <param name="numberOfSteps">Number of simulation steps.</param>
        /// <param name="tileName">Name of the tile.</param>
        /// <param name="probabilityOfTheKill">TODO</param>
        public void RunSimulation(int numberOfSteps, string tileName, double probabilityOfTheKill)
        {
            int stepsElapsed = 0;
            while (stepsElapsed < numberOfSteps && RunOneSimulationStep())
            {
                stepsElapsed++;
                // we do not want to hurt system in step 1, because it would kill the whole system completely
                if (stepsElapsed > 1)
                {
                    // do we want to hurt system in this step
                    double selection = Randomizer.NextDoubleBetween(0, 1);
                    if (probabilityOfTheKill > selection)
                    {
                        HurtSystem(tileName, 1);
                    }
                }
            }
            SaveSnapshots(false);
        }

        /// <summary>
        /// Run simulation with speficied number of steps and try to hurt the system by removing random object of type specified by tileName. 
        /// This is Version2 procedure, previous version is deprecated
        /// </summary>
        /// <param name="numberOfSteps">Number of simulation steps.</param>
        /// <param name="countStep">At what steps shall we count cells.</param>
        /// <param name="runNo">Number of the run. It is only passed down the line for reporting purposes.</param>
        /// <param name="tileName">Name of the tile.</param>
        /// <param name="numberOfKills">Number of object to be hurt.</param>
        /// <param name="probabilistic">Flag for using random mechanism of choosing step.</param>
        public string RunSimulationV2(int numberOfSteps, int countStep, ulong runNo, string tileName, long numberOfKills, bool probabilistic)
        {
            string result = "";  // result string to be returned

            // if numberOfKills is greater than number of steps then we will be killing in each (but not in the first one)
            int sureKills = (int)(numberOfKills / ((long)numberOfSteps - 1));
            int remainingKills = (sureKills == 0) ? (int)numberOfKills : ((int)numberOfKills - (sureKills * (numberOfSteps - 1)));

            // build hurt table
            List<int> hurtTable = new List<int>(numberOfSteps);
            for (int i = 0; i < numberOfSteps; i++)
            {
                if (i == 0)
                    hurtTable.Add(0);
                else
                    hurtTable.Add(sureKills);
            }

            // if we require probabilistic distribution of kills, we choose steps randomly
            if (probabilistic)
            {
                long kills = 0;
                while (kills < remainingKills)
                {
                    int step = Randomizer.Next(numberOfSteps);
                    // do not accept step zero or already selected step => accepted step greater than sureKills
                    if (step == 0 || hurtTable[step] > sureKills)
                        continue;
                    hurtTable[step]++;
                    kills++;
                }
            }
            else
            {
                // this may look a bit tricky but it does exactly what we want
                // kill steps are marked from the back and we mustn't mark step zero
                if (remainingKills > 0)
                {
                    int killEveryXStep = (int)((long)numberOfSteps / remainingKills);
                    for (int i = hurtTable.Count - 1; i > 0; i -= killEveryXStep)
                        hurtTable[i]++;
                }
            }

            // run required number of simulation steps
            int stepsElapsed = 0;
            while (stepsElapsed < numberOfSteps)
            {
                // run one simulation step
                bool success = RunOneSimulationStep();

                // do we want to hurt system in this step
                if (hurtTable[stepsElapsed] != 0)
                    HurtSystem(tileName, hurtTable[stepsElapsed]);
                stepsElapsed++;

                // count components if condition is met
                if (stepsElapsed % countStep == 0)
                {
                    string startString = string.Format("STAT>> {0},{1},{2}", runNo, stepsElapsed, success ? 0 : 1);
                    result += CountComponents(startString);
                }
            }
            SaveSnapshots(false);

            return result;
        }

        /// <summary>
        /// Run simulation (version 2) with specified number of steps and count complete cells every countStep. Previous version
        /// of the simulation is deprecated.
        /// </summary>
        /// <param name="numberOfSteps">Number of steps for the simulation.</param>
        /// <param name="countStep">when do we count complete cells</param>
        /// <param name="runNo">Number of the run. It is only passed down the line for reporting purposes.</param>
        /// <param name="tileName">Name of the tile.</param>
        /// <param name="probabilityOfTheKill">What probability shall we assign to each step in simulation.</param>
        public string RunSimulationV2(int numberOfSteps, int countStep, ulong runNo, string tileName, double probabilityOfTheKill)
        {
            string result = "";          // build result string

            int stepsElapsed = 0;
            while (stepsElapsed++ < numberOfSteps)
            {
                // run one simulation step
                bool success = RunOneSimulationStep();

                // we do not want to hurt system in step 1, because it would kill the whole system completely
                if (stepsElapsed > 1)
                {
                    // do we want to hurt system in this step
                    double selection = Randomizer.NextDoubleBetween(0, 1);
                    if (selection < probabilityOfTheKill)
                    {
                        HurtSystem(tileName, 1);
                    }
                }

                // count components if condition is met
                if (stepsElapsed % countStep == 0)
                {
                    string startString = string.Format("STAT>> {0},{1},{2}", runNo, stepsElapsed, success ? 0 : 1);
                    result += CountComponents(startString);
                }
            }
            SaveSnapshots(false);

            return result;
        }

        /// <summary>
        /// This is helper procedure which shall be made more general but I need this quick for now.
        /// </summary>
        /// <returns></returns>
        private string ReportTileNumbers()
        {
            // I may want to put this into generic procedure to count components
            // and make it flexible by counting tiles in generic way without hardcoding names
            int nRectangle = 0;
            int nTrapezoid = 0;
            int nOct_small = 0;
            int nInner_rod1 = 0;
            int nInner_rod2 = 0;
            int nOuter_rod = 0;
            int nUnknown = 0;
            foreach (TileInSpace tile in TilesWorld)
            {
                switch (tile.Name)
                {
                    case "rectangle":
                        nRectangle++;
                        break;
                    case "trapezoid":
                        nTrapezoid++;
                        break;
                    case "oct_small":
                        nOct_small++;
                        break;
                    case "inner_rod1":
                        nInner_rod1++;
                        break;
                    case "inner_rod2":
                        nInner_rod2++;
                        break;
                    case "outer_rod":
                        nOuter_rod++;
                        break;
                    default:
                        nUnknown++;
                        break;
                }
            }
            return string.Format("Message: rectangle = {0}, trapezoid = {1}, oct_small = {2}, inner_rod1 = {3}, inner_rod2 = {4}, outer_rod = {5}, unknown = {6}\n",
                nRectangle, nTrapezoid, nOct_small, nInner_rod1, nInner_rod2, nOuter_rod, nUnknown);
        }

        /// <summary>
        /// Run one simulation for One Off damage test.
        /// </summary>
        /// <param name="runNo">Number of the current run to be used for reporting purposes.</param>
        /// <param name="tileName">Name of the targeted tile, empty string means any randomly chosen tile.</param>
        /// <param name="numberOfTiles">Number of tiles to remove.</param>
        /// <param name="onlyPologon3DTiles">If TRUE then only tiles of type Pologon3D type are being removed</param>
        /// <param name="numberOfRecoverySteps">Number of system iterations given the system to recover.</param>
        /// <returns></returns>
        public string RunSimulationOneOffDamage(int runNo, string tileName, int numberOfTiles, bool onlyPologon3DTiles, int numberOfRecoverySteps)
        {
            string startString = "";
            string res = "";

            int initialNumberOfSteps = 0;
            // first, we want to let the system grow until there are two comlete cells
            bool twoCells = false;
            while (!twoCells)
            {
                // run one simulation step
                RunOneSimulationStep();
                initialNumberOfSteps++;

                // report state of the system after initial number of steps
                startString = string.Format("STAT>> {0},{1},{2}", runNo, initialNumberOfSteps, 0);
                res += CountComponents(startString);

                int firstComponentCount = 0;
                int secondComponentCount = 0;

                // get copy of all tiles
                HashSet<TileInSpace> polygonTiles = new HashSet<TileInSpace>(TilesWorld.PolygonTiles);

                // get first tile in the collection and get its component, e.g. all tiles connected to it
                TileInSpace tile = polygonTiles.First();
                HashSet<TileInSpace> component = tile.Component();

                // how many tiles is in this component
                //firstComponentCount = component.Count;
                firstComponentCount = Count3DTilesInComponent(component);

                // remove all tiles of the component form the list
                foreach (TileInSpace element in component)
                {
                    polygonTiles.Remove(element);
                }

                // do it the same for the second component if it exists
                if (polygonTiles.Count > 0)
                {
                    tile = polygonTiles.First();
                    component = tile.Component();
                    secondComponentCount = Count3DTilesInComponent(component);
                }

                twoCells = (firstComponentCount >= 26) && (secondComponentCount >= 26);
            }

            // now, there are two cells and we have to remove the one with oct_base tile, because this
            // one will always continue to grow, 
            foreach (TileInSpace tile in TilesWorld.PolygonTiles)
            {
                if (tile.Name == "oct_base")
                {
                    HashSet<TileInSpace> component = tile.Component();
                    // remove component
                    foreach (TileInSpace element in component)
                    {
                        TilesWorld.Remove(element);
                    }
                    // and stop iterating
                    break;
                }
            }

            // XJB DEBUG BEGIN - I want to make sure I removed required number of tiles
            res += ReportTileNumbers();
            int countBeforeHurting = onlyPologon3DTiles ? TilesWorld.PolygonTiles.Count() : TilesWorld.Count();
            // now we can make the damage and we know all tiles belong to one component so 
            // no need to bother with the components
            HurtSystem(tileName, numberOfTiles, onlyPologon3DTiles);
            res += ReportTileNumbers();
            int countAfterHurting = onlyPologon3DTiles ? TilesWorld.PolygonTiles.Count() : TilesWorld.Count();
            if (countBeforeHurting != countAfterHurting + numberOfTiles)
            {
                res += string.Format("Warning: countBeforeHurting != countAfterHurting + numberOfTiles ({0} != {1} + {2})\n",
                    countBeforeHurting, countAfterHurting, numberOfTiles);
            }
            else
            {
                res += string.Format("Message: countBeforeHurting == countAfterHurting + numberOfTiles ({0} == {1} + {2})\n",
                    countBeforeHurting, countAfterHurting, numberOfTiles);
            }
            // XJB DEBUG END

            // see how the systems evolves over twice as many steps as it took to initial
            for (int i = 0; i < numberOfRecoverySteps; i++)
            {
                // perform one simulation step
                RunOneSimulationStep();
                // report state for every further stpe
                startString = string.Format("STAT>> {0},{1},{2}", runNo, 0, i + 1);
                res += CountComponents(startString);
            }

            return res;
        }

        /// <summary>
        /// Saves the snapshot file with all up-to-date simulation steps.
        /// </summary>
        public void SaveSnapshots(bool toTempFile = true)
        {
            Xmlizer.SaveSnapshotsDoc(v_SnapshotXmlDoc, toTempFile);
        }

        /// <summary>
        /// Saves the snapshot file with all up-to-date simulation steps.
        /// </summary>
        /// <returns>Path of snapshot</returns>
        public string SaveSnapshotsAndReturnPath()
        {
            return Xmlizer.SaveSnapshotsDoc(v_SnapshotXmlDoc);
        }

        /// <summary>
        /// Saves the snapshot file with all up-to-date simulation steps to specific location.
        /// </summary>
        /// <param name="path">Save file path.</param>
        public void SaveSnapshotsWithSpecificLocation(string path)
        {
            Xmlizer.SaveSnapshotsDocWithSpecificLocation(v_SnapshotXmlDoc, path);
        }

        /// <summary>
        /// Get statistics for current simulation.
        /// </summary>
        /// <returns>Statistics container</returns>
        public MSystemStats GetMSystemStats()
        {
            MSystemStats systemStats = new MSystemStats();

            systemStats.CalculateTileStats(TilesWorld);
            systemStats.CalculateFloatingObectsStats(FltObjectsWorld);
            systemStats.CalculateMSystemStats(SimulationMSystem);
            return systemStats;
        }

        /// <summary>
        /// Getter for notification reciver.
        /// </summary>
        /// <returns>Revicer object.</returns>
        public NotificationMessage GetNotificationReciever()
        {
            return v_Notification;
        }

        /// <summary>
        /// Returns string representation of simulated M system.
        /// </summary>
        public string MSystemToString() => SimulationMSystem.ToString();

        #endregion
    }
}
