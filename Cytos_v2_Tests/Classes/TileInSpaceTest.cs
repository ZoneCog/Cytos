using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSystemSimulationEngine.Classes;
using MSystemSimulationEngine.Classes.Tools;

namespace Cytos_v2_Tests.Classes
{
    [TestClass]
    public class TileInSpaceTest
    {
        // Glues
        private static readonly Glue GlueA = new Glue("GlueA");
        private static Tile _square, _tubule;
        private static TileInSpace baseTile, eastTile, southTile, topTile, tubule;

        /// <summary>
        /// Provides a test tile - a square tile.
        /// </summary>
        /// <param name="name">Name of the tile.</param>
        private static Tile CreateSquare(string name)
        {
            Point3D v1 = new Point3D(5, 5, 0);
            Point3D v2 = new Point3D(5, -5, 0);
            Point3D v3 = new Point3D(-5, -5, 0);
            Point3D v4 = new Point3D(-5, 5, 0);

            return new Tile(name,
                    new [] { v1, v2, v3, v4 },
                    new []
                    {
                    new Connector("c1", new [] {v1, v2}, GlueA, Angle.FromDegrees(90), 0),
                    new Connector("c2", new [] {v2, v3}, GlueA, Angle.FromDegrees(90), 0),
                    new Connector("c-in1", new [] { new Point3D(3, 0, 0) }, GlueA, Angle.FromDegrees(90), 0),
                    new Connector("c-in2", new [] { default(Point3D) }, GlueA, Angle.FromDegrees(90), 0)
                    },
                null, null, Color.Aqua);
        }


        /// <summary>
        /// Provides a test tile - a vertical segment with two connectors.
        /// </summary>
        /// <param name="name">Name of the segment</param>
        /// <param name="v1">Vertex1</param>
        /// <param name="v2">Vertex2</param>
        private static Tile CreateSegment(string name, Point3D v1, Point3D v2)
        {
            var vertices = new List<Point3D>() { v1, v2 };
            var connectors = vertices.Select((t, i) => new Connector($"cs{i}",
                new [] { t }, GlueA, Angle.FromDegrees(30), 0)).ToList();
            return new Tile(name, vertices, connectors, null, null, Color.Red);
        }


        [TestInitialize]
        public void Initialize()
        {
            _square = CreateSquare("Square");
            baseTile = new TileInSpace(_square, default(Point3D), Quaternion.One);
            eastTile = baseTile.Connectors[0].ConnectObject(_square.Connectors[1]);
            southTile= eastTile.Connectors[0].ConnectObject(_square.Connectors[0]);
            topTile  = southTile.Connectors[1].ConnectObject(_square.Connectors[0]);

            _tubule = CreateSegment("tubule", new Point3D(0, 0, 0), new Point3D(2, 0, 0));
            tubule = baseTile.Connectors[2].ConnectObject(_tubule.Connectors[0]);
            tubule.Connectors[0].Disconnect();
            baseTile.Surface.ConnectTo(tubule.Connectors[0]);   // We need to test also component connected by surface glue
        }

        // Methods Rotate and Move are tested implicitly already in the Initialize() above
        // They are called from the method ConnectObject


        [TestMethod]
        public void TestPushedComponent_ClearPushing()
        {
            var expectedSet = new Collection<TileInSpace> { baseTile, eastTile, southTile, topTile, tubule };
            var emptySet = new Collection<TileInSpace>();
            CollectionAssert.AreEquivalent(expectedSet, baseTile.PushedComponent(new Vector3D(1,1,1)).ToList());
            CollectionAssert.AreEquivalent(emptySet, baseTile.PushedComponent(new Vector3D(1, 1, 1)).ToList());
            baseTile.ClearPushing();
            CollectionAssert.AreEquivalent(expectedSet, baseTile.PushedComponent(new Vector3D(1, 1, 1)).ToList());
            CollectionAssert.AreEquivalent(expectedSet, tubule.PushedComponent(new Vector3D(2, 2, 2)).ToList());

            tubule.Connectors[0].Disconnect();
            expectedSet.Remove(tubule);
            CollectionAssert.AreEquivalent(expectedSet, baseTile.PushedComponent(new Vector3D(2, 2, 3)).ToList());
        }


        [TestMethod]
        // TODO move to the TilesWorldTest.cs (low priority)
        public void TestPushingDirection()
        {
            Assert.AreEqual(new Point3D(0, 0, 1), TilesWorld.PushingDirection(tubule.Connectors[0]).ToPoint3D().Round());
            Assert.AreEqual(new Point3D(-1, 0, 0), TilesWorld.PushingDirection(baseTile.Connectors[0]).ToPoint3D().Round());
            Assert.AreEqual(new Point3D(0, 1, 0), TilesWorld.PushingDirection(eastTile.Connectors[0]).ToPoint3D().Round());
            Assert.AreEqual(new Point3D(0, 0, -1), TilesWorld.PushingDirection(southTile.Connectors[1]).ToPoint3D().Round());

            baseTile.Connectors[3].ConnectObject(_tubule.Connectors[0]);
            Assert.IsTrue(new Vector3D(0, 0, -1).Equals(TilesWorld.PushingDirection(baseTile.Connectors[3]), 1e-14));
        }

        [TestMethod]
        public void TestPushingIntersected()
        {
            var testSegment3 = new TileInSpace(CreateSegment("TestSegment3", new Point3D(0, 0, 0), new Point3D(0, 0, 5)), Point3D.Origin, Quaternion.One);
            var testSegment4 = new TileInSpace(CreateSegment("TestSegment4", new Point3D(0, -5, -3), new Point3D(0, 5, 3)), Point3D.Origin, Quaternion.One);
            var testSegment5 = new TileInSpace(CreateSegment("TestSegment5", new Point3D(0, -10, -3), new Point3D(0, 0, 1)), Point3D.Origin, Quaternion.One);

            Assert.AreEqual(default(Vector3D), testSegment3.PushingIntersected(testSegment4,  new UnitVector3D(0, 0, -1)));
            Assert.AreEqual(default(Vector3D), testSegment3.PushingIntersected(baseTile, new UnitVector3D(0, 0, -1)));
            Assert.IsTrue(new Vector3D(0, 0, -3).Equals(testSegment4.PushingIntersected(baseTile, new UnitVector3D(0, 0, -1)), 1e-14));
            Assert.IsTrue(new Vector3D(0, 0, -1).Equals(testSegment5.PushingIntersected(baseTile, new UnitVector3D(0, 0, -1)), 1e-14));
        }

        //TODO add PushingNonIntersected test, include the test case from TilesWorldTest.TestPushingComplexObject(), objects 1,3,4 should not mutually push

        [TestMethod]
        public void TestIntersection()
        {
            Assert.IsFalse(baseTile.IntersectsWith(eastTile));
            Assert.IsFalse(baseTile.IntersectsWith(southTile));
            Assert.IsFalse(eastTile.IntersectsWith(southTile));
            Assert.IsFalse(topTile.IntersectsWith(eastTile));
            Assert.IsFalse(topTile.IntersectsWith(southTile));

            baseTile.Move(new Vector3D(MSystem.Tolerance / 10, -MSystem.Tolerance / 10, MSystem.Tolerance / 10));
            Assert.IsFalse(baseTile.IntersectsWith(eastTile));
            Assert.IsFalse(baseTile.IntersectsWith(southTile));

            baseTile.Move(new Vector3D(MSystem.Tolerance * 10, -MSystem.Tolerance * 10, MSystem.Tolerance * 10));
            Assert.IsTrue(baseTile.IntersectsWith(eastTile));
            Assert.IsTrue(baseTile.IntersectsWith(southTile));
        }

        // NO TEST NEEDED FOR OverlapsWith, tested in Polygon3D

        // NO TEST NEEDED FOR SidePoint, tested in Polygon3D

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void TestShortenTo1()
        {
            tubule.ShortenTo(3, tubule.Connectors[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void TestShortenTo2()
        {
            baseTile.ShortenTo(3, baseTile.Connectors[0]);
        }

        [TestMethod]
        public void TestShortenTo3()
        {
            tubule.ShortenTo(1, tubule.Connectors[0]);
            Assert.IsTrue(new Point3D(3, 0, 1).Equals(tubule.Vertices[1], 1e-15));
            Assert.IsTrue(new Point3D(3, 0, 1).Equals(tubule.Connectors[1].Positions[0], 1e-15));
            Assert.AreEqual(GlueA, tubule.Connectors[1].Glue);
        }

        [TestMethod]
        public void ColorWasChangedIsTrueWhenTwoDifferentColorAreSet()
        {
            TileInSpace tile = new TileInSpace(CreateSquare("Square"), default(Point3D), Quaternion.One);
            Assert.IsFalse(tile.ColorWasChanged);
            Color tileColor = tile.Color;
            Color newTileColor = Color.Blue;
            Assert.AreNotEqual(tileColor, newTileColor);
            tile.SetNewColor(newTileColor);
            Assert.IsTrue(tile.ColorWasChanged);
        }

        [TestMethod]
        public void ColorWasChangedIsFalseWhenSameColorSet()
        {
            TileInSpace tile = new TileInSpace(CreateSquare("Square"), default(Point3D), Quaternion.One);
            Assert.IsFalse(tile.ColorWasChanged);
            tile.SetNewColor(tile.Color);
            Assert.IsFalse(tile.ColorWasChanged);
        }
    }
}
