using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MSystemSimulationEngine.Classes.Tools;
using MSystemSimulationEngine.Classes.Xml;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// M system used for simulation process.
    /// TODO low priority: consider implementing as a singleton
    /// </summary>
    public class MSystem
    {
        #region Private data

        /// <summary>
        /// List of proteins.
        /// </summary>
        private readonly ReadOnlyDictionary<string, Protein> v_Proteins;

        /// <summary>
        /// List of evolution rules.
        /// </summary>
        private readonly List<EvolutionRule> v_EvolutionRules;

        #endregion

        #region Public data
        // Default values of some constants with optional XML definition can be found in DeserializedObjects.cs

        /// <summary>
        /// List of floating objects.
        /// </summary>
        public readonly ReadOnlyDictionary<string, FloatingObject> FloatingObjects;

        /// <summary>
        /// List of tiles.
        /// </summary>
        public readonly ReadOnlyDictionary<string, Tile> Tiles;

        /// <summary>
        /// Graph of seed tiles.
        /// </summary>
        public readonly IReadOnlyList<TileInSpace> SeedTiles;

        /// <summary>
        /// Radius of glues in P system - in this distance can glues stick together.
        /// </summary>
        public readonly double GlueRadius;

        /// <summary>
        /// Defines random movement of tiles during simulation.
        /// </summary>
        public readonly double TilingRandomMovement;

        /// <summary>
        /// Maximum mobility among floating objects in the M system.
        /// </summary>
        public readonly double Mobility = 1;
        
        /// <summary>
        /// Tolerance of the measurement of mutual positions of ibjects.
        /// If two objects are within the "Tolerance" distance, they are considered in touch.
        /// Experimentally observed errors in vertex positions are of the order 10^{-16} 
        /// when sizes of the objects are of the order of units.
        /// </summary>
        public const double Tolerance = 1E-9;

        /// <summary>
        /// If a protein or a connector is placed "inside" or "outside" of a 2D tile,
        /// its reference position is assumed at the "SideDist" distance from the object.
        /// No floating object can be placed closer than "SideDist" to a 2D/3D tile, 
        /// to distinguish clearly positions "in" and "out".
        /// </summary>
        public const double SideDist = 5*Tolerance;

        /// <summary>
        /// = 2 * SideDist
        /// If a 2D or 3D objects pushes another face-to-face, there must remain this distance between them.
        /// </summary>
        public const double MinFaceDist = 2*SideDist;

        /// <summary>
        /// Glue relation (dictionary).
        /// </summary>
        public readonly GlueRelation GlueRelation;

        /// <summary>
        /// List of metabolic rules keyed by the rule protein.
        /// </summary>
        public readonly Dictionary<string, IReadOnlyList<EvoMetabolicRule>> MetabolicRules;

        /// <summary>
        /// List of creation rules keyed by the glue to which the object created by the rule can attach.
        /// A rule can appear repeatedly for various glues.
        /// </summary>
        public readonly Dictionary<Glue, IReadOnlyList<EvoNonMetabolicRule>> CreationRules;

        /// <summary>
        /// TODO priorities: a quick solution to make dodecahedrons divide correctly: should be generalized or changed
        /// A rule can appear repeatedly for various glues.
        /// </summary>
        public readonly SortedSet<int> CreationRulesPriorities;

        /// <summary>
        /// List of division rules keyed symmetrically by two glues which the rule can separate.
        /// Keying by glue pairs would be impractical.
        /// </summary>
        public readonly Dictionary<Glue, Dictionary<Glue, IReadOnlyList<EvoNonMetabolicRule>>> DivisionRules;

        /// <summary>
        /// List of insertion rules keyed symmetrically by two glues which the rule can separate.
        /// Keying by glue pairs would be impractical.
        /// </summary>
        public readonly Dictionary<Glue, Dictionary<Glue, IReadOnlyList<EvoNonMetabolicRule>>> InsertionRules;

        /// <summary>
        /// List of destruction rules keyed by name of the tile which the rule destroys.
        /// </summary>
        public readonly Dictionary<string, IReadOnlyList<EvoNonMetabolicRule>> DestructionRules;

        /// <summary>
        /// Electric potential threshold >= 0.
        /// </summary>
        public readonly double Tau;

        /// <summary>
        /// Electric source voltage >= 0. If nonzero, the tile system is treated as Circuit Tile Assembly - cTAM
        /// </summary>
        public readonly double Nu0;

        /// <summary>
        /// Multiplicative coefficient of pushing distance
        /// </summary>
        public readonly double PushingCoef;

        /// <summary>
        /// Scan connectors for attachment of new tiles in random order? 
        /// If false, connectors are grouped by tiles in order "as created".
        /// </summary>
        public readonly bool RandomizeConnectors;

        /// <summary>
        /// Keep constant concentration of environmental objects as the environment grows?
        /// </summary>
        public readonly bool RefillEnvironment;

        #endregion

        #region Constructor

        /// <summary>
        /// P system constructor used during simulation.
        /// </summary>
        /// <param name="mSystemObjects">Deserialized M System objects.</param>
        /// <exception cref="ArgumentException">
        /// If M System objects objecst list is null.
        /// </exception>
        public MSystem(DeserializedObjects mSystemObjects)
        {
            if (mSystemObjects == null)
            {
                throw new ArgumentException("M System objects can't be null.");
            }
            // New copies of seed tiles must be created here!
            SeedTiles = mSystemObjects.SeedTiles.Select(tile => new TileInSpace((Tile)tile, tile.Position, tile.Quaternion)).ToList();
            GlueRadius = mSystemObjects.GlueRadius;
            TilingRandomMovement = mSystemObjects.TilingRandomMovement;
            Tau = mSystemObjects.Tau;
            Nu0 = mSystemObjects.Nu0;
            PushingCoef = mSystemObjects.PushingCoef;
            RandomizeConnectors = mSystemObjects.RandomizeConnectors;

            FloatingObjects = new ReadOnlyDictionary<string, FloatingObject>(mSystemObjects.FloatingObjects);
            if (FloatingObjects.Any())
                Mobility = FloatingObjects.Values.Max(obj => obj.Mobility);
            RefillEnvironment = mSystemObjects.RefillEnvironment;
            v_Proteins = new ReadOnlyDictionary<string, Protein>(mSystemObjects.Proteins);
            Tiles = new ReadOnlyDictionary<string, Tile>(mSystemObjects.Tiles);
            GlueRelation = mSystemObjects.GluePRelation;
            v_EvolutionRules = mSystemObjects.EvolutionRules;

            //======================================================================
            MetabolicRules = new Dictionary<string, IReadOnlyList<EvoMetabolicRule>>();
            foreach (string proteinName in v_Proteins.Keys)
            {
                MetabolicRules[proteinName] = v_EvolutionRules.OfType<EvoMetabolicRule>().Where(rule => proteinName == rule.RProtein.Name).ToList();
            }

            //======================================================================
            CreationRules = new Dictionary<Glue, IReadOnlyList<EvoNonMetabolicRule>>();
            foreach (Glue glue in mSystemObjects.Glues.Values)
            {
                var creationRules = v_EvolutionRules.OfType<EvoNonMetabolicRule>().Where(rule => rule.Type == EvolutionRule.RuleType.Create).ToList();
                CreationRulesPriorities = new SortedSet<int>(creationRules.Select(rule => rule.Priority));
                CreationRules[glue] = creationRules.Where(rule => rule.RightSideObjects.OfType<Tile>().Single()
                    .Connectors.Any(connector => GlueRelation.ContainsKey(Tuple.Create(glue, connector.Glue))))
                    .ToList();
            }

            //======================================================================
            InsertionRules = new Dictionary<Glue, Dictionary<Glue, IReadOnlyList<EvoNonMetabolicRule>>>();
            foreach (Glue glue1 in mSystemObjects.Glues.Values)
                foreach (Glue glue2 in mSystemObjects.Glues.Values)
                {
                    var insRules = v_EvolutionRules.OfType<EvoNonMetabolicRule>()
                            .Where(rule => (rule.Type == EvolutionRule.RuleType.Insert)
                                           && IsInsertable(rule.RightSideObjects.OfType<Tile>().Single(), glue1, glue2))
                            .ToList();
                    if (insRules.Any())
                    {
                        AddRulesToDictionary(InsertionRules, glue1, glue2, insRules);
                        AddRulesToDictionary(InsertionRules, glue2, glue1, insRules);
                    }
                }

            //======================================================================
            DestructionRules = new Dictionary<string, IReadOnlyList<EvoNonMetabolicRule>>();
            foreach (string tileName in Tiles.Keys)
            {
                DestructionRules[tileName] = v_EvolutionRules.OfType<EvoNonMetabolicRule>().Where(rule => (rule.Type == EvolutionRule.RuleType.Destroy)
                    && rule.LeftSideObjects.OfType<Tile>().Single().Name == tileName).ToList();
            }

            //======================================================================
            DivisionRules = new Dictionary<Glue, Dictionary<Glue, IReadOnlyList<EvoNonMetabolicRule>>>();
            foreach (Glue glue1 in mSystemObjects.Glues.Values)
                foreach (Glue glue2 in mSystemObjects.Glues.Values)
                {
                    var divRules = v_EvolutionRules.OfType<EvoNonMetabolicRule>()
                            .Where(rule => (rule.Type == EvolutionRule.RuleType.Divide)
                                           && glue1 == (Glue) rule.RightSideObjects[0] 
                                           && glue2 == (Glue) rule.RightSideObjects[1])
                            .ToList();
                    if (divRules.Any())
                    {
                        AddRulesToDictionary(DivisionRules, glue1, glue2, divRules);
                        AddRulesToDictionary(DivisionRules, glue2, glue1, divRules);
                    }
                }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes the key glue1 in the dictionary. 
        /// Then adds the element keyed glue1, glue2 to the dictionary.
        /// </summary>
        private static void AddRulesToDictionary<T>(IDictionary<Glue, Dictionary<Glue, T>> dict, Glue glue1, Glue glue2, T element)
        {
            if (!dict.ContainsKey(glue1))
                dict[glue1] = new Dictionary<Glue, T>();
            dict[glue1][glue2] = element;
        }


        /// <summary>
        /// Checks whether a tile can be eventually inserted between two connectors with glue1 and glue2. 
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="glue1"></param>
        /// <param name="glue2"></param>
        private bool IsInsertable(Tile tile, Glue glue1, Glue glue2)
        {
            return MatchingConectors(tile, glue1, glue2).Item2 != null;
        }


        #endregion

        #region Public methods

        /// <summary>
        /// Returns a pair of facet connectors at distinct positions on "tile" which match "glue1" and "glue2" in GlueRelation.
        /// If no such pair exist, then both returned connectors are null.
        /// TODO: check whether connectors are on opposite ends of the tile
        /// </summary>
        public Tuple<ConnectorOnTile, ConnectorOnTile> MatchingConectors(Tile tile, Glue glue1, Glue glue2)
        {
            ConnectorOnTile conn2 = null;
            var facetConnectors = tile.Connectors.Where(conn => conn.Positions.All(point => tile.Vertices.ContainsPoint(point)));

            var conn1 = facetConnectors.FirstOrDefault(
                connector1 => GlueRelation.MatchAsymmetric(glue1, connector1.Glue)
                              && (conn2 = facetConnectors.FirstOrDefault(
                                  connector2 => !Geometry.Overlap(connector1.Positions, connector2.Positions)
                                                && GlueRelation.MatchAsymmetric(glue2, connector2.Glue)))
                                != null);
            return Tuple.Create(conn1, conn2);
        }


        /// <summary>   TODO low priority: move to class Connector when MSystem is available as singleton
        /// Checks whether two connector are mutually compatible (dimension, size, glues).
        /// Order of connectors matters due to possibly asymmetric glue relation!
        /// </summary>
        /// <param name="connector1"></param>
        /// <param name="connector2"></param>
        public bool AreCompatible(ConnectorOnTileInSpace connector1, ConnectorOnTile connector2)
        {
            // compatible glues and  connector sizes
            bool result = GlueRelation.ContainsKey(Tuple.Create(connector1.Glue, connector2.Glue))
                && connector1.IsSizeComppatibleWith(connector2);

            if (!result || Nu0 <= 0) return result;

            //if Nu0 > 0, then this is a cTAM: check if connector voltage exceeds the threshold
            if (connector2.OnTile.Connectors.Count == 4)
            {
                // Assume that an inner cTAM tile can connect only to an east or south connector
                // It is attachable only if it connects to two connectors simultaneously and
                // the sum o heir voltage exceeds the threshold Tau
                ConnectorOnTileInSpace otherConnector = null;
                if (connector1 == connector1.OnTile.EastConnector)
                {
                    otherConnector = connector1.OnTile.NorthConnector?.ConnectedTo?.OnTile.EastConnector?.ConnectedTo
                        ?.OnTile.SouthConnector;
                }
                if (connector1 == connector1.OnTile.SouthConnector)
                {
                    otherConnector = connector1.OnTile.WestConnector?.ConnectedTo?.OnTile.SouthConnector?.ConnectedTo
                        ?.OnTile.EastConnector;
                }
                return otherConnector!= null &&  otherConnector.Voltage + connector1.Voltage >= Tau;
            }
            else
                // Border cTAM
                return connector1.Voltage >= Tau;

        }


        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String of object.</returns>
        public override string ToString()
        {
            //TODO add missing objects
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Tile System:");

            foreach (Tile tile in Tiles.Values)
                builder.AppendLine($"\n{tile}");

            builder.AppendLine("Seed Tiles:");
            foreach (var seedTile in SeedTiles)
                builder.AppendLine($"{seedTile}");

            builder.AppendLine(string.Format("\nGlue relation:\n{0}", GlueRelation));

            builder.AppendLine($"\nGlue radius: {GlueRadius}");
            builder.AppendLine($"\nTiling random movement: {TilingRandomMovement}");


            builder.AppendLine("\nM System:\n");

            builder.AppendLine("Floating objects:");
            string floatingObjects = string.Empty;
            foreach (FloatingObject floatingObject in FloatingObjects.Values)
            {
                floatingObjects = $"{floatingObjects}{floatingObject}\n";
            }
            builder.AppendLine(floatingObjects);

            builder.AppendLine("\nProteins:");
            string proteins = string.Empty;
            foreach (Protein protein in v_Proteins.Values)
            {
                proteins = $"{proteins}{protein}\n";
            }
            builder.AppendLine(proteins);

            builder.AppendLine("\nEvolution rules:");
            string evolutionRules = string.Empty;
            foreach (EvolutionRule evolutionRule in v_EvolutionRules)
            {
                evolutionRules = string.Format("{0}{1}\n", evolutionRules, evolutionRule);
            }
            builder.AppendLine(evolutionRules);

            builder.AppendLine($"Tolerance: {Tolerance}");
            builder.AppendLine($"Side distance: {SideDist}");
            builder.AppendLine($"Minimal face distance: {MinFaceDist}");
            builder.AppendLine($"Tau: {Tau}");
            builder.AppendLine($"Nu0: {Nu0}");
            builder.AppendLine($"Pushing coeficient: {PushingCoef}");
            return builder.ToString();
        }

        #endregion
    }
}
