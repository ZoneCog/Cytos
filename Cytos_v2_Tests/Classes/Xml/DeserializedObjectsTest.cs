using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSystemSimulationEngine.Classes;
using MSystemSimulationEngine.Classes.Xml;
using SharedComponents.Tools;
using MSystemSimulationEngine.Classes.Tools;

namespace Cytos_v2_Tests.Classes.Xml
{
    [TestClass]
    public class DeserializedObjectsTest
    {
        [TestMethod]
        public void TestThatInventoryFileIsDeserializable()
        {
            //Key of this test is to use oficial published inventory file.
            //DO NOT USE TEST XMLs.
            string path = @"..\..\..\Cytos_v2\Examples\MSystemDescription.xml";
            XDocument mSystemDescription = XDocument.Load(path);
            // ReSharper disable once UnusedVariable
            DeserializedObjects testDeserializedObjects = new DeserializedObjects(mSystemDescription, path);

            //No assert needed, tests only ability to deserialize invenoty file
        }

        [TestMethod]
        public void TestDeserializationCreatesObjectWithCorrectValues()
        {
            //NOTE: We cannot test equivalence of objects as objects contains readonly parameters, which are never equal.
            //Due to this limitation we test only values of parameters.

            string path = @"../../Classes/Xml/TestXML/testMSystemDescription.xml";
            XDocument mSystemDescription = XDocument.Load(path);
            DeserializedObjects testDeserializedObjects = new DeserializedObjects(mSystemDescription, path);

            //Floating objects
            Dictionary<string, FloatingObject> floatingObjects = testDeserializedObjects.FloatingObjects;
            Assert.AreEqual(3, floatingObjects.Count);
            Assert.AreEqual("a", floatingObjects["a"].Name);
            Assert.AreEqual(0, floatingObjects["a"].Concentration);
            Assert.AreEqual(2, floatingObjects["a"].Mobility);

            Assert.AreEqual("b", floatingObjects["b"].Name);
            Assert.AreEqual(2, floatingObjects["b"].Concentration);
            Assert.AreEqual(3, floatingObjects["b"].Mobility);

            //Proteins
            Dictionary<string, Protein> proteins = testDeserializedObjects.Proteins;
            Assert.AreEqual(2, proteins.Count);

            Assert.AreEqual("p1", proteins["p1"].Name);
            Assert.AreEqual("p2", proteins["p2"].Name);

            //Glues
            Dictionary<string, Glue> glues = testDeserializedObjects.Glues;
            Assert.AreEqual(3, glues.Count);

            Assert.AreEqual("pa", glues["pa"].Name);
            Assert.AreEqual("pb", glues["pb"].Name);
            Assert.AreEqual("px", glues["px"].Name);

            //Tiles
            Dictionary<string, Tile> tiles = testDeserializedObjects.Tiles;
            Assert.AreEqual(3, tiles.Count);

            Tile tileQ1 = tiles["q1"];

            //Connectors
            var connectors = tileQ1.Connectors;
            Assert.AreEqual(6, connectors.Count);

            var connectorC1 = connectors[0];
            Assert.AreEqual("c1", connectorC1.Name);
            Assert.AreEqual(Tile.SideType.undef, connectorC1.Side);
            Assert.AreEqual(glues["pa"], connectorC1.Glue);
            Assert.AreEqual(Angle.FromRadians(2.034443935795703), connectorC1.Angle);
            Assert.AreEqual(2, connectorC1.Positions.Count);
            Assert.AreEqual(new Point3D(0, 10, 0), connectorC1.Positions[0]);
            double posY = 10 * Math.Cos((1) * 2 * Math.PI / 5);
            double posX = 10 * Math.Sin((1) * 2 * Math.PI / 5);
            Assert.AreEqual(new Point3D(posX, posY, 0), connectorC1.Positions[1]);

            //Surface glue
            Glue surfaceGLuePx = tileQ1.SurfaceGlue;
            Assert.AreEqual("px", surfaceGLuePx.Name);

            //Proteins
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
            List<TileInSpace> seedTiles = testDeserializedObjects.SeedTiles;
            Assert.AreEqual(3, seedTiles.Count);
            var seedTileQ1 = seedTiles[0];
            Assert.AreEqual("q1", seedTileQ1.Name);
            Assert.AreEqual(new Point3D(0, 0, 0), seedTileQ1.Position.Round());
            Assert.AreEqual(new EulerAngles(default(Angle), default(Angle), default(Angle)), seedTileQ1.Quaternion.ToEulerAngles());

            //Others
            Assert.AreEqual(0.1, testDeserializedObjects.GlueRadius);

            //Glue tuples
            GlueRelation glueTuples = testDeserializedObjects.GluePRelation;
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

            //Evo rules
            List<EvolutionRule> evoRules = testDeserializedObjects.EvolutionRules;
            Assert.AreEqual(4, evoRules.Count);

            EvoMetabolicRule metabolicRule = TypeUtil.Cast<EvoMetabolicRule>(evoRules[0]);
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

            EvoNonMetabolicRule createRule = TypeUtil.Cast<EvoNonMetabolicRule>(evoRules[1]);
            Assert.AreEqual(1, createRule.Priority);
            Assert.AreEqual(EvolutionRule.RuleType.Create, createRule.Type);
            Assert.AreEqual(3, createRule.LeftSideObjects.Count);
            Assert.AreEqual("a", createRule.LeftSideObjects[0].Name);
            Assert.AreEqual("a", createRule.LeftSideObjects[1].Name);
            Assert.AreEqual("a", createRule.LeftSideObjects[2].Name);
            Assert.AreEqual(1, createRule.RightSideObjects.Count);
            Assert.AreEqual("q1", createRule.RightSideObjects[0].Name);
            Assert.AreEqual(3, createRule.MLeftSideFloatingNames.Count);
            Assert.AreEqual(3, createRule.MLeftSideFloatingNames.ToDictionary()["a"]);
            Assert.AreEqual(0, createRule.MRightSideFloatingNames.Count);

            EvoNonMetabolicRule divideRule = TypeUtil.Cast<EvoNonMetabolicRule>(evoRules[2]);
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

            EvoNonMetabolicRule destroyRule = TypeUtil.Cast<EvoNonMetabolicRule>(evoRules[3]);
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
        }
    }
}
