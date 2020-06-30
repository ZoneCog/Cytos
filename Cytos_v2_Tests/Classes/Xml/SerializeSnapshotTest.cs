using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System.Xml.Linq;
using MSystemSimulationEngine.Classes;
using MSystemSimulationEngine.Classes.Xml;
using Quaternion = MathNet.Spatial.Euclidean.Quaternion;

namespace Cytos_v2_Tests.Classes.Xml
{
    [TestClass]
    public class SerializeSnapshotTest
    {
        // Test floating object for all tests, it is never changed.
        private static readonly FloatingObject v_TestFloatingObject = new FloatingObject("testName", 0, 0);
        private static readonly IEnumerable<Tile> v_EmptyTiles = new List<Tile>();

        private const string c_TestMSystemDescription = "../../Classes/Xml/TestXML/testEmptyMSystemDescription.xml";

        #region Unit Tests

        [TestMethod]
        public void TestConstructor()
        {
            SerializeSnapshot testSnapshot = new SerializeSnapshot(c_TestMSystemDescription, v_EmptyTiles);
            Assert.AreEqual("<root><MSystemDescription><tiling></tiling><Msystem></Msystem></MSystemDescription><snapshots/></root>", 
                Regex.Replace(testSnapshot.GetXmlDocAsAString(), @"\s+", ""));//Regex removes all whitespaces
        }

        [TestMethod]
        public void TestSaveXmlFile()
        {
            string xmlPath = "testFolder/testXML.xml";
            SerializeSnapshot testSnapshot = new SerializeSnapshot(c_TestMSystemDescription, v_EmptyTiles);
            testSnapshot.SaveXmlFile(xmlPath);
            Assert.IsTrue(File.Exists(xmlPath));
            File.Delete(xmlPath);
        }

        [TestMethod]
        public void SerializeTilesTest()
        {
            TileInSpace.ResetCounter();
            FloatingObjectInSpace.ResetCounter();

            //Set test Floating objects set
            FloatingObjectInSpace testFloatingObjectInSpace = new FloatingObjectInSpace(v_TestFloatingObject, new Point3D(0, 0, 0));
            FloatingObjectsSet testFloatingObjectsSet = new FloatingObjectsSet { testFloatingObjectInSpace };

            //Set test Tile set
            List<Point3D> testPointList = new List<Point3D>() { new Point3D(1, 1, 1), new Point3D(2, 2, 2) };
            Tile baseObject = new Tile("testName", testPointList, null, null, null, Color.Black);
            List<TileInSpace> testTilesList = new List<TileInSpace> { new TileInSpace(baseObject, Point3D.Origin, Quaternion.One) };

            SerializeSnapshot testSnapshot = new SerializeSnapshot(c_TestMSystemDescription, new List<Tile>{ baseObject });
            testSnapshot.Serialize(0, testFloatingObjectsSet, testTilesList);
            string serializedSnapShot = testSnapshot.GetXmlDocAsAString();
            XDocument expecteDocument = XDocument.Load("../../Classes/Xml/TestXML/serializedSnapShotTest.xml");

            //TODO Due to TileInSpace static counter this test fails if you use "run all tests".
            Assert.AreEqual(expecteDocument.ToString(), serializedSnapShot); //Compare both documents.
        }

        #endregion

        #region Integration tests

        //TODO It would be nice to have some kind of integration tests, that Deserialization -> Serialization works fine.

        /*

        //Set test Floating objects set
        FloatingObjectInSpace testFloatingObjectInSpace = new FloatingObjectInSpace("testName", new Point3D(0, 1, 2));
        FloatingObjectsSet testFloatingObjectsSet = new FloatingObjectsSet { testFloatingObjectInSpace };

        //Set tiles
        XmlDocument testInventoryDocument = new XmlDocument();
        testInventoryDocument.Load(@"..\..\..\Cytos_v2\Examples\InventoryFile.xml");

        XmlDocument testEvolutionDocument = new XmlDocument();
        testEvolutionDocument.Load(@"..\..\..\Cytos_v2\Examples\EvolutionFile.xml");

        DeserializedInventoryObjects testDeserializedInventoryObjects = new DeserializedInventoryObjects(testInventoryDocument);

        DeserializedEvolutionObjects testDeserializedEvolutionObjects = new DeserializedEvolutionObjects(testEvolutionDocument, testDeserializedInventoryObjects);

        MSystem testMSystem = new MSystem(testDeserializedInventoryObjects, testDeserializedEvolutionObjects);
        TilesWorld testTilesWorld = new TilesWorld(testMSystem);

        SerializeSnapshot testSnapshot = new SerializeSnapshot();
        testSnapshot.Serialize(0, testFloatingObjectsSet, testTilesWorld);
        string output = testSnapshot.GetXmlDocAsAString();

        Assert.AreEqual("expected string", output);

         */

        [TestMethod]
        public void CreateComplexSnapshotFileTest()
        // Petr: Actually not a test - only temporary debugging method. Please do not remove.
        // Produces a test snapshot file for Honza - testing the visualization package in Unity.
        {
            //Set test Floating objects set
            var testFloatingObjectInSpace = new FloatingObjectInSpace(v_TestFloatingObject, new Point3D(0, 1, 2)); // Add more
            var testFloatingObjectsSet = new FloatingObjectsSet { testFloatingObjectInSpace };

            Tile baseTile = CreateTile("q2");
            Tile baseTubule = CreateSegment("s1");
            var v_baseTilesList = new List<Tile> { baseTile, baseTubule };

            var testTilesList = new List<TileInSpace>
                {new TileInSpace(baseTile, Point3D.Origin, Quaternion.One)};

            SerializeSnapshot testSnapshot = new SerializeSnapshot(c_TestMSystemDescription, v_baseTilesList);
            testSnapshot.Serialize(0, testFloatingObjectsSet, testTilesList);

            TileInSpace bottomTile = testTilesList.First();
            bottomTile.State = TileInSpace.FState.Unchanged;
            var level1Tiles = new List<TileInSpace>();

            for (int i = 0; i < 5; i++)
                level1Tiles.Add(bottomTile.Connectors[i].ConnectObject(baseTile.Connectors[0]));

            testTilesList.AddRange(level1Tiles);

            var tubule1 = bottomTile.Connectors.Last().ConnectObject(baseTubule.Connectors[0]);
            testTilesList.Add(tubule1);
            testSnapshot.Serialize(1, testFloatingObjectsSet, testTilesList);

            // Adding the second level of tiles of dodecahedron and tubules
            tubule1.State = TileInSpace.FState.Unchanged;
            var level2Tiles = new List<TileInSpace>();
            var level2Tubules = new List<TileInSpace>();

            foreach (var tile in level1Tiles)
            {
                tile.State = TileInSpace.FState.Unchanged;
                level2Tiles.Add(tile.Connectors[2].ConnectObject(baseTile.Connectors[0]));
            }
            for (int i=1; i<3; i++)
                level2Tubules.Add(tubule1.Connectors[i].ConnectObject(baseTubule.Connectors[0]));

            testTilesList.AddRange(level2Tiles);
            testTilesList.AddRange(level2Tubules);
            testSnapshot.Serialize(2, testFloatingObjectsSet, testTilesList);

            foreach (var tile in level2Tiles)
                tile.State = TileInSpace.FState.Unchanged;

            // Adding the last top tile of dodecahedron
            TileInSpace topTile = level2Tiles.Last().Connectors[3].ConnectObject(baseTile.Connectors[0]);
            testTilesList.Add(topTile);

            var level3Tubules = new List<TileInSpace>();
            foreach (var tubule in level2Tubules)
            {
                tubule.State = TileInSpace.FState.Unchanged;
                for (int i = 1; i < 3; i++)
                    level3Tubules.Add(tubule.Connectors[i].ConnectObject(baseTubule.Connectors[0]));
            }

            // Dodecahedron complete, three levels of tubule tree complete
            testTilesList.AddRange(level3Tubules);
            testSnapshot.Serialize(3, testFloatingObjectsSet, testTilesList);

            topTile.State = TileInSpace.FState.Unchanged;
            foreach (var tubule in level3Tubules)
                tubule.State = TileInSpace.FState.Unchanged;

            foreach (var tile in level2Tiles)
                tile.State = TileInSpace.FState.Destroy;

            // Level 2 tiles destructed
            testSnapshot.Serialize(4, testFloatingObjectsSet, testTilesList);

            testTilesList.RemoveAll(obj => level2Tiles.Contains(obj));
            topTile.Move(new Vector3D(0,0,-5));

            // Top tile moved 5 units down
            testSnapshot.Serialize(5, testFloatingObjectsSet, testTilesList);

            topTile.State = TileInSpace.FState.Unchanged;
            var Tile = CreateSquareTile("TestSquare");
            TileInSpace baseSquare = new TileInSpace(Tile, new Point3D(30,0,0), Quaternion.One);
            TileInSpace eastSquare = baseSquare.Connectors[0].ConnectObject(Tile.Connectors[1]);
            TileInSpace southSquare = eastSquare.Connectors[0].ConnectObject(Tile.Connectors[0]);
            TileInSpace topSquare = southSquare.Connectors[1].ConnectObject(Tile.Connectors[0]);

            // Four squares added, forming an incomplete cube
            testTilesList.AddRange(new List<TileInSpace>(){baseSquare, eastSquare, southSquare, topSquare});
            testSnapshot.Serialize(6, testFloatingObjectsSet, testTilesList);

            testSnapshot.SaveXmlFile(@"..\..\..\Cytos_v2\Examples\TestSnapshotFile.xml");
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Provides a test tile - a tile.
        /// </summary>
        /// <param name="name">Name of the tile.</param>
        private static Tile CreateTile(string name)
        {
            var glueA = new Glue("GlueA");
            var vertices = new NamedVertices(5, 10).Select(vertex => vertex.Position).ToList();
            var connectors = vertices.Select((t, i) => new Connector($"c{i}", 
                new List<Point3D> {t, vertices[(i + 1)%vertices.Count]}, 
                glueA, 
                Angle.FromRadians(Math.Acos(-1/Math.Sqrt(5))), 0))
                .ToList();
            
            // Point connector in the center of the tile
            connectors.Add(new Connector("cp",
                    new List<Point3D> { Point3D.Origin },
                    glueA,
                    Angle.FromDegrees(90), 0));

            return new Tile(name, vertices, connectors, null, null, Color.FromArgb(64, Color.DeepSkyBlue));
        }

        /// <summary>
        /// Provides a test tile - a square tile.
        /// </summary>
        /// <param name="name">Name of the tile.</param>
        private static Tile CreateSquareTile(string name)
        {
            var GlueA = new Glue("GlueA");
            Point3D v1 = new Point3D(5, 5, 0);
            Point3D v2 = new Point3D(5, -5, 0);
            Point3D v3 = new Point3D(-5, -5, 0);
            Point3D v4 = new Point3D(-5, 5, 0);

            return new Tile(name,
                    new List<Point3D> { v1, v2, v3, v4 },
                    new List<Connector>
                    {
                    new Connector("c1", new List<Point3D> {v1, v2}, GlueA, Angle.FromDegrees(90), 0),
                    new Connector("c2", new List<Point3D> {v2, v3}, GlueA, Angle.FromDegrees(90), 0),
                    new Connector("cp", new List<Point3D> { Point3D.Origin }, GlueA, Angle.FromDegrees(90), 0)
                    },
                    null, null, Color.FromArgb(64, Color.DarkBlue));
        }

        /// <summary>
        /// Provides a test tile - a segment with three connectors.
        /// </summary>
        /// <param name="name">Name of the segment.</param>
        private static Tile CreateSegment(string name)
        {
            var glueB = new Glue("GlueB");
            var vertices = new List<Point3D>() { new Point3D(-1, 0, 0), new Point3D(1, 0, 0) };
            var connectors = vertices.Select((t,i) => new Connector($"cs{i}",
                new List<Point3D> { t },
                glueB,
                Angle.FromDegrees(30), 0))
                .ToList();

            connectors.Add(new Connector($"cs2",
                new List<Point3D> { vertices[1] },
                glueB,
                Angle.FromDegrees(-30), 0));

            return new Tile(name, vertices, connectors, null, null, Color.Red);
        }

        #endregion
    }
}
