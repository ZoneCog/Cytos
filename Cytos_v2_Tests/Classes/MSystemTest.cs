using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSystemSimulationEngine.Classes;
using MSystemSimulationEngine.Classes.Tools;
using MSystemSimulationEngine.Classes.Xml;

namespace Cytos_v2_Tests.Classes
{
    [TestClass]
    public class MSystemTest
    {
        private static DeserializedObjects v_TestDeserializedMSystemObjects;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            string path = @"../../Classes/Xml/TestXML/testMSystemDescription.xml";
            XDocument inventoryFile = XDocument.Load(path);
            v_TestDeserializedMSystemObjects = new DeserializedObjects(inventoryFile, path);
        }

        [TestMethod]
        public void TestConstructor()
        {
            MSystem mSystem = new MSystem(v_TestDeserializedMSystemObjects);

            // Tiles
            ReadOnlyDictionary<string, Tile> tiles = mSystem.Tiles;
            Assert.AreEqual(3, tiles.Count);
            Tile tileQ1 = tiles["q1"];
            var connectors = tileQ1.Connectors;
            Assert.AreEqual(6, connectors.Count);
            var connectorC1 = connectors[0];
            Assert.AreEqual("c1", connectorC1.Name);
            Assert.AreEqual(Tile.SideType.undef, connectorC1.Side);
            Assert.AreEqual(v_TestDeserializedMSystemObjects.Glues["pa"], connectorC1.Glue);
            Assert.AreEqual(Angle.FromRadians(2.034443935795703), connectorC1.Angle);
            Assert.AreEqual(2, connectorC1.Positions.Count);
            Assert.AreEqual(new Point3D(0, 10, 0), connectorC1.Positions[0]);
            double posY = 10*Math.Cos((1)*2*Math.PI/5);
            double posX = 10*Math.Sin((1)*2*Math.PI/5);
            Assert.AreEqual(new Point3D(posX, posY, 0), connectorC1.Positions[1]);
            Glue surfaceGLuePx = tileQ1.SurfaceGlue;
            Assert.AreEqual("px", surfaceGLuePx.Name);
            Assert.AreEqual(4, tileQ1.Proteins.Count);
            Assert.AreEqual("p1", tileQ1.Proteins[0].Name);
            Assert.AreEqual("p1", tileQ1.Proteins[1].Name);
            Assert.AreEqual("p2", tileQ1.Proteins[2].Name);
            Assert.AreEqual("p2", tileQ1.Proteins[3].Name);
            foreach (var protein in tileQ1.Proteins)
            {
                Assert.AreEqual(Point3D.Origin, protein.Position);
            }

            Assert.AreEqual(Color.DeepSkyBlue, tileQ1.Color);
            Tile tileS1 = tiles["s1"];
            Assert.AreEqual(2, tileS1.Connectors.Count);
            Assert.AreEqual(1, tileS1.Proteins.Count);

            //Initial objects
            IReadOnlyList<TileInSpace> seedTiles = mSystem.SeedTiles;
            Assert.AreEqual(3, seedTiles.Count);
            TileInSpace seedTileQ1 = seedTiles[0];
            Assert.AreEqual("q1", seedTileQ1.Name);
            Assert.AreEqual(new Point3D(0, 0, 0), seedTileQ1.Position.Round(14));
            Assert.AreEqual(new EulerAngles(default(Angle), default(Angle), default(Angle)).ToQuaternion(), seedTileQ1.Quaternion);

            //Environmental objects
            var environmentalObjects = mSystem.FloatingObjects.Values.Where(obj => obj.Concentration > 0).ToList();
            Assert.AreEqual(1, environmentalObjects.Count);
            var environmentalObjectA = environmentalObjects[0];
            Assert.AreEqual("b", environmentalObjectA.Name);
            Assert.AreEqual(2, environmentalObjectA.Concentration);

            //GlueRadius
            Assert.AreEqual(0.1, mSystem.GlueRadius);

            //Mobility
            Assert.AreEqual(3, mSystem.Mobility);

            //Tolerance
            Assert.AreEqual(1E-10, MSystem.Tolerance);

            //SideDist
            Assert.AreEqual(5E-10, MSystem.SideDist);

            //GlueRelation
            GlueRelation glueTuples = mSystem.GlueRelation;
            Assert.AreEqual(3, glueTuples.Count);

            Tuple<Glue, Glue> glueTuplePaPb = glueTuples.Keys.ElementAt(0);
            Assert.AreEqual("pa", glueTuplePaPb.Item1.Name);
            Assert.AreEqual("pb", glueTuplePaPb.Item2.Name);
            Assert.AreEqual(1, glueTuples.Values.ElementAt(0).ToDictionary()["a"]);

            Tuple<Glue, Glue> glueTuplePaPa = glueTuples.Keys.ElementAt(1);
            Assert.AreEqual("pa", glueTuplePaPa.Item1.Name);
            Assert.AreEqual("pa", glueTuplePaPa.Item2.Name);
            Assert.AreEqual(1, glueTuples.Values.ElementAt(1).ToDictionary().Count);

            Tuple<Glue, Glue> glueTuplePbPb = glueTuples.Keys.ElementAt(2);
            Assert.AreEqual("pb", glueTuplePbPb.Item1.Name);
            Assert.AreEqual("pb", glueTuplePbPb.Item2.Name);
            Assert.AreEqual(0, glueTuples.Values.ElementAt(2).ToDictionary().Count);

            //MetabolicRules
            Dictionary<string, IReadOnlyList<EvoMetabolicRule>> metabolicRules = mSystem.MetabolicRules;
            Assert.AreEqual(2, metabolicRules.Count);
            Assert.IsNotNull(metabolicRules["p1"]);
            Assert.AreEqual(1, metabolicRules["p1"].Count);
            Assert.IsNotNull(metabolicRules["p2"]);
            Assert.AreEqual(0, metabolicRules["p2"].Count);

            EvoMetabolicRule metabolicRule = metabolicRules["p1"][0];
            Assert.AreEqual(0, metabolicRule.Priority);
            Assert.AreEqual(EvolutionRule.RuleType.Metabolic, metabolicRule.Type);
            Assert.AreEqual(EvoMetabolicRule.MetabolicRuleType.Symport, metabolicRule.SubType);
            Assert.AreEqual(2, metabolicRule.LeftSideObjects.Count);
            Assert.AreEqual("a", metabolicRule.LeftSideObjects[0].Name);
            Assert.AreEqual("p1", metabolicRule.LeftSideObjects[1].Name);
            Assert.AreEqual(2, metabolicRule.RightSideObjects.Count);
            Assert.AreEqual("p1", metabolicRule.RightSideObjects[0].Name);
            Assert.AreEqual("a", metabolicRule.RightSideObjects[1].Name);
            Assert.AreEqual(0, metabolicRule.MLeftInNames.Count);
            Assert.AreEqual(1, metabolicRule.MLeftOutNames.Count);
            Assert.AreEqual(1, metabolicRule.MLeftOutNames.ToDictionary()["a"]);
            Assert.AreEqual(1, metabolicRule.MRightInNames.Count);
            Assert.AreEqual(1, metabolicRule.MRightInNames.ToDictionary()["a"]);
            Assert.AreEqual(0, metabolicRule.MRightOutNames.Count);
            Assert.AreEqual("p1", metabolicRule.RProtein.Name);


            //CreationRules
            Dictionary<Glue, IReadOnlyList<EvoNonMetabolicRule>> createRules = mSystem.CreationRules;
            Assert.AreEqual(3, createRules.Count);

            Glue pa = v_TestDeserializedMSystemObjects.Glues["pa"];
            Glue pb = v_TestDeserializedMSystemObjects.Glues["pb"];
            Glue px = v_TestDeserializedMSystemObjects.Glues["px"];

            Assert.IsNotNull(createRules[pa]);
            Assert.AreEqual(1, createRules[pa].Count);
            Assert.IsNotNull(createRules[pb]);
            Assert.AreEqual(1, createRules[pb].Count);
            Assert.IsNotNull(createRules[px]);
            Assert.AreEqual(0, createRules[px].Count);

            // For different glues there is the same creation rule - this is OK.
            EvoNonMetabolicRule createRulePA = createRules[pa][0];
            Assert.AreEqual(1, createRulePA.Priority);
            Assert.AreEqual(EvolutionRule.RuleType.Create, createRulePA.Type);
            Assert.AreEqual(3, createRulePA.LeftSideObjects.Count);
            Assert.AreEqual("a", createRulePA.LeftSideObjects[0].Name);
            Assert.AreEqual("a", createRulePA.LeftSideObjects[1].Name);
            Assert.AreEqual("a", createRulePA.LeftSideObjects[2].Name);
            Assert.AreEqual(1, createRulePA.RightSideObjects.Count);
            Assert.AreEqual("q1", createRulePA.RightSideObjects[0].Name);
            Assert.AreEqual(3, createRulePA.MLeftSideFloatingNames.Count);
            Assert.AreEqual(3, createRulePA.MLeftSideFloatingNames.ToDictionary()["a"]);
            Assert.AreEqual(0, createRulePA.MRightSideFloatingNames.Count);

            // For different glues there is the same creation rule - this is OK.
            EvoNonMetabolicRule createRulePB = createRules[pb][0];
            Assert.AreEqual(1, createRulePB.Priority);
            Assert.AreEqual(EvolutionRule.RuleType.Create, createRulePB.Type);
            Assert.AreEqual(3, createRulePB.LeftSideObjects.Count);
            Assert.AreEqual("a", createRulePB.LeftSideObjects[0].Name);
            Assert.AreEqual("a", createRulePB.LeftSideObjects[1].Name);
            Assert.AreEqual("a", createRulePB.LeftSideObjects[2].Name);
            Assert.AreEqual(1, createRulePB.RightSideObjects.Count);
            Assert.AreEqual("q1", createRulePB.RightSideObjects[0].Name);
            Assert.AreEqual(3, createRulePB.MLeftSideFloatingNames.Count);
            Assert.AreEqual(3, createRulePB.MLeftSideFloatingNames.ToDictionary()["a"]);
            Assert.AreEqual(0, createRulePB.MRightSideFloatingNames.Count);

            //DestructionRules
            var destructionRules = mSystem.DestructionRules;
            Assert.AreEqual(3, destructionRules.Count);
            Assert.IsNotNull(destructionRules["q1"]);
            Assert.AreEqual(0, destructionRules["q1"].Count);
            Assert.IsNotNull(destructionRules["s1"]);
            Assert.AreEqual(1, destructionRules["s1"].Count);

            EvoNonMetabolicRule destroyRule = destructionRules["s1"][0];
            Assert.AreEqual(1, destroyRule.Priority);
            Assert.AreEqual(EvolutionRule.RuleType.Destroy, destroyRule.Type);
            Assert.AreEqual(3, destroyRule.LeftSideObjects.Count);
            Assert.AreEqual("a", destroyRule.LeftSideObjects[0].Name);
            Assert.AreEqual("a", destroyRule.LeftSideObjects[1].Name);
            Assert.AreEqual("s1", destroyRule.LeftSideObjects[2].Name);
            Assert.AreEqual(2, destroyRule.RightSideObjects.Count);
            Assert.AreEqual("c", destroyRule.RightSideObjects[0].Name);
            Assert.AreEqual("c", destroyRule.RightSideObjects[1].Name);
            Assert.AreEqual(2, destroyRule.MLeftSideFloatingNames.Count);
            Assert.AreEqual(2, destroyRule.MLeftSideFloatingNames.ToDictionary()["a"]);
            Assert.AreEqual(2, destroyRule.MRightSideFloatingNames.Count);
            Assert.AreEqual(2, destroyRule.MRightSideFloatingNames.ToDictionary()["c"]);

            //DivisionRules
            var divisionRules = mSystem.DivisionRules;

            Assert.AreEqual(1, divisionRules.Count);

            Assert.IsNotNull(divisionRules[pa]);
            Assert.AreEqual(1, divisionRules[pa].Count);

            Assert.IsNotNull(divisionRules[pa][pa]);
            Assert.AreEqual(1, divisionRules[pa][pa].Count);
           
            EvoNonMetabolicRule divideRule = divisionRules[pa][pa][0];
            Assert.AreEqual(1, divideRule.Priority);
            Assert.AreEqual(EvolutionRule.RuleType.Divide, divideRule.Type);
            Assert.AreEqual(4, divideRule.LeftSideObjects.Count);
            Assert.AreEqual("pa", divideRule.LeftSideObjects[0].Name);
            Assert.AreEqual("pa", divideRule.LeftSideObjects[1].Name);
            Assert.AreEqual("a", divideRule.LeftSideObjects[2].Name);
            Assert.AreEqual("a", divideRule.LeftSideObjects[3].Name);
            Assert.AreEqual(2, divideRule.RightSideObjects.Count);
            Assert.AreEqual("pa", divideRule.RightSideObjects[0].Name);
            Assert.AreEqual("pa", divideRule.RightSideObjects[1].Name);
            Assert.AreEqual(2, divideRule.MLeftSideFloatingNames.Count);
            Assert.AreEqual(2, divideRule.MLeftSideFloatingNames.ToDictionary()["a"]);
            Assert.AreEqual(0, divideRule.MRightSideFloatingNames.Count);

        }

        [TestMethod]
        public void TestAreCompatible()
        {
            MSystem mSystem = new MSystem(v_TestDeserializedMSystemObjects);

            var c1 = mSystem.SeedTiles.First(tile => tile.Name == "q1").Connectors[0];
            var c2 = v_TestDeserializedMSystemObjects.Tiles["q1"].Connectors[1];
            var c3 = v_TestDeserializedMSystemObjects.Tiles["s1"].Connectors[1];

            Assert.IsTrue(mSystem.AreCompatible(c1, c2));
            Assert.IsFalse(mSystem.AreCompatible(c1, c3));
        }

        [TestMethod]
        public void TestToString()
        {
            //It isn't really wise (and important) to test such extensive text structure.
        }
    }
}
