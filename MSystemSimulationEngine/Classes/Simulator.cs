using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
                var message = $"cTAM description: Nu_0={SimulationMSystem.Nu0}, Tau={SimulationMSystem.Tau}";
                foreach (var tile in SimulationMSystem.Tiles.Values)
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

            var metabolicRules = ApplyMetabolicRules();
            var destructionRules = ApplyDestructionRules();
            var divisionRules = ApplyDivisionRules();
            var insertionRules = ApplyInsertionRules();
            var creationRules = ApplyCreationRules();

            FinalizeStep();
            var result = metabolicRules.Any() || destructionRules.Any() || divisionRules.Any() || insertionRules.Any() || creationRules.Any();

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
                                                 $"{FltObjectsWorld}");
            }
            return result;
        }


        /// <summary>
        /// Logs one step of a cTAM simulation.
        /// </summary>
        private void LogOneCtamStep()
        {
            var dump = $"Step: {v_Step}\r\n";
            foreach (var seedTile in SimulationMSystem.SeedTiles)
            {
                var ladder = "Ladder:";
                var voltages = "Voltages:";
                var tile = seedTile;
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
            var appliedRules = new NamedMultiset();

            // List of free proteins on all existing tiles
            var freeProteins = TilesWorld.SelectMany(tile => tile.Proteins.Where(protein => !protein.IsUsed)).ToArray();
            freeProteins.Shuffle();

            foreach (var freeProtein in freeProteins)
            {
                TileInSpace tile = freeProtein.m_tile;

                // List of available metabolic rules for this protein is constructed
                // For protein on a 1D object only catalyzed rules are applicable.
                var availableRules = SimulationMSystem.MetabolicRules[freeProtein.Name].Where(
                         rule => tile.Vertices is Polygon3D || rule.SubType == EvoMetabolicRule.MetabolicRuleType.Catalyzed).ToArray();
                availableRules.Shuffle();

                // Optimization - calculate the minimum multisets of objects to apply any rule in the list
                var unionOfLeftInNames = NamedMultiset.Union(availableRules.Select(rule => rule.MLeftInNames));
                var unionOfLeftOutNames = NamedMultiset.Union(availableRules.Select(rule => rule.MLeftOutNames));

                var innerSet = FltObjectsWorld.GetNearObjects(freeProtein, Tile.SideType.inside, unionOfLeftInNames);
                var outerSet = FltObjectsWorld.GetNearObjects(freeProtein, Tile.SideType.outside, unionOfLeftOutNames);

                // TODO change to FirstOrDefault
                foreach (var rule in availableRules)
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
            var appliedRules = new NamedMultiset();
            var tiles = TilesWorld.ToArray();
            tiles.Shuffle();

            foreach (var tile in tiles)
            {
                // List of destruction rules which can be applied to this object
                // The multisets of floating objects on the left-hand side of the rule must be present in reaction distance from the object
                var availableRules = SimulationMSystem.DestructionRules[tile.Name].ToArray();

                // Optimization - calculate the minimum multiset of objects to apply any rule in the list
                var unionOfLeftNames = NamedMultiset.Union(availableRules.Select(r => r.MLeftSideFloatingNames));

                var nearObjects = FltObjectsWorld.GetNearObjects(tile, unionOfLeftNames);
                var mNearObjects = nearObjects.ToMultiset();

                availableRules.Shuffle();
                var rule = availableRules.FirstOrDefault(r => r.MLeftSideFloatingNames.IsSubsetOf(mNearObjects));

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
            var appliedRules = new NamedMultiset();

            // List of used (interconnected) connectors on all existing objects
            // Each connector pair has only one its member in the list
            var availableConnectors = TilesWorld.SelectMany(tile => tile.Connectors.Where(connector =>
                    connector.ConnectedTo != null &&
                    tile.ID < connector.ConnectedTo.OnTile.ID))
                .ToArray();
            availableConnectors.Shuffle();

            foreach (var connector in availableConnectors)
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
                var unionOfLeftNames = NamedMultiset.Union(availableRules.Select(r => r.MLeftSideFloatingNames));

                FloatingObjectsSet nearObjects = FltObjectsWorld.GetNearObjects(connector, unionOfLeftNames);
                var mNearObjects = nearObjects.ToMultiset();

                availableRules.Shuffle();
                var rule = availableRules.FirstOrDefault(r => r.MLeftSideFloatingNames.IsSubsetOf(mNearObjects));

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
            var appliedRules = new NamedMultiset();

            // List of used (interconnected) connectors on all existing objects
            // Each connector pair has only one its member in the list
            var insertableConnectors = TilesWorld.SelectMany(tile => tile.Connectors.Where(connector =>
                    tile.State == TileInSpace.FState.Unchanged &&
                    ! connector.SetDisconnect &&
                    connector.ConnectedTo != null &&
                    tile.ID < connector.ConnectedTo.OnTile.ID &&
                    SimulationMSystem.InsertionRules.ContainsKey(connector.Glue) &&
                    SimulationMSystem.InsertionRules[connector.Glue].ContainsKey(connector.ConnectedTo.Glue)))
                .ToList();

            insertableConnectors.Shuffle();

            while (insertableConnectors.Any())
            {
                var connector = insertableConnectors[0];

                foreach(var rule in InsertableRules(connector))
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
            var availableRules = SimulationMSystem.InsertionRules[connector.Glue][connector.ConnectedTo.Glue]
                .Where(rule => MatchingConectors(rule.RightSideObjects.OfType<Tile>().Single(), connector).Item1 != null)
                .ToArray();

            // Optimization - calculate the minimum multiset of objects to apply any rule in the list
            var unionOfLeftNames = NamedMultiset.Union(availableRules.Select(r => r.MLeftSideFloatingNames));

            FloatingObjectsSet nearObjects = FltObjectsWorld.GetNearObjects(connector, unionOfLeftNames);
            var mNearObjects = nearObjects.ToMultiset();

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
            var pair = SimulationMSystem.MatchingConectors(tile, connector.Glue, connector.ConnectedTo.Glue);
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
            var appliedRules = new NamedMultiset();

            // List of connection sites on all existing non-destroyed objects to which a new object can attach.
            var availableConnectors = TilesWorld.Where(tile => tile.State != TileInSpace.FState.Destroy)
                    .SelectMany(tile => tile.Connectors).ToArray();
            availableConnectors.Shuffle();

            foreach (var priority in SimulationMSystem.CreationRulesPriorities)
            {
                foreach (var connector in availableConnectors)
                {
                    if (connector.ConnectedTo != null)   // must be tested here since connectors may auto-connect in each iteration
                        continue;

                    // If the connector on segment touches a tile, then it cannot be used
                    // TODO correct the case when such a tile can be pushed
                    if (connector.OnTile.Vertices is Segment3D &&
                        TilesWorld.Any(tile => tile.Vertices is Polygon3D && tile.Vertices.ContainsPoint(connector.Positions[0])))
                        continue;

                    // List of creation rules which can be applied to this connection site
                    var availableRules = SimulationMSystem.CreationRules[connector.Glue].Where(r => r.Priority == priority).ToArray();

                    // Optimization - calculate the minimum multiset of objects to apply any rule in the list
                    var unionOfLeftNames = NamedMultiset.Union(availableRules.Select(r => r.MLeftSideFloatingNames));

                    FloatingObjectsSet nearObjects = FltObjectsWorld.GetNearObjects(connector, unionOfLeftNames);
                    var mNearObjects = nearObjects.ToMultiset();

                    // Method ADD tries to attach the tile in the rule to the "connector" 
                    // Returns false if there are surrounding tiles in its way which cannot be pushed
                    availableRules.Shuffle();
                    var rule = availableRules.FirstOrDefault(r => r.MLeftSideFloatingNames.IsSubsetOf(mNearObjects)
                        && TilesWorld.Add(r.RightSideObjects.OfType<Tile>().Single(), connector));

                    if (rule != null)
                    {
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
            foreach (var tile in TilesWorld)
                foreach (var protein in tile.Proteins)
                    protein.IsUsed = false;
        }

        /// <summary>
        /// Complete simulation data in the M system at the end of each step.
        /// </summary>
        /// <remarks>Protected accessibility for testing only</remarks>
        protected void FinalizeStep()
        {
            FltObjectsWorld.FinalizeStep();
            var tileArray = TilesWorld.ToArray();

            if (v_SerializeFloatingObjects)
            {

                Xmlizer.AddSnapshot(v_Step, FltObjectsWorld.ToSet(), TilesWorld, v_SnapshotXmlDoc);
            }

            foreach (var tile in tileArray)
                foreach (var connector in tile.Connectors)
                {
                    // Disconnect marked connectors - must be done before snapshot serialization
                    if (connector.SetDisconnect)
                        connector.Disconnect();
                }

            // If the M system is a cTAM, calculate electric charges and re-color tiles accordingly
            if (SimulationMSystem.Nu0 > 0)
                Electric.CalculateCharges(SimulationMSystem);

            Xmlizer.AddSnapshot(v_Step, new FloatingObjectsSet(), TilesWorld, v_SnapshotXmlDoc);

            foreach (var tile in tileArray)
            {
                tile.ColorWasChanged = false; //Reset color changed flag.

                // Remove tile marked for destruction, do not change its state
                if (tile.State == TileInSpace.FState.Destroy)
                    TilesWorld.Remove(tile);
                else
                    tile.State = TileInSpace.FState.Unchanged;

                //PerformHealthCheck();
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
                foreach (var checkedTile in tilesList)
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
        /// Hurts system by removing randomly numberOfKills of type tileName. If tileName is empty string then we do not care about the type of the object
        /// </summary>
        /// <param name="tileName">Name of the tile.</param>
        /// <param name="numberOfKills">Number of object to be hurt.</param>
        private void HurtSystem(string tileName, int numberOfKills)
        {
            Random rnd = new Random();

            for (int i = 0; i < numberOfKills; i++)
            {
                int randPosition = rnd.Next(TilesWorld.Count());

                // we do not care about type of the object
                if (tileName == "")
                {
                    TilesWorld.Remove(TilesWorld.ElementAt(randPosition));
                    continue;
                }

                bool removed = false;
                // try to find object from random position going forward
                for (int j = randPosition; j < TilesWorld.Count(); j++)
                {
                    TileInSpace tile = TilesWorld.ElementAt(j);
                    if (tile.Name == tileName)
                    {
                        TilesWorld.Remove(tile);
                        removed = true;
                        break;
                    }
                }
                // we did not find object for removal, go backwards
                if (!removed)
                {
                    for (int j = randPosition - 1; j >= 0; j--)
                    {
                        TileInSpace tile = TilesWorld.ElementAt(j);
                        if (tile.Name == tileName)
                        {
                            TilesWorld.Remove(tile);
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

        #endregion

        #region Public Methods

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
            Random rand = new Random();

            int stepsElapsed = 0;
            while (stepsElapsed < numberOfSteps && RunOneSimulationStep())
            {
                stepsElapsed++;
                // we do not want to hurt system in step 1, because it would kill the whole system completely
                if (stepsElapsed > 1)
                {
                    // do we want to hurt system in this step
                    double selection = rand.NextDouble();
                    if (probabilityOfTheKill > selection)
                    {
                        HurtSystem(tileName, 1);
                    }
                }
            }
            SaveSnapshots(false);
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
