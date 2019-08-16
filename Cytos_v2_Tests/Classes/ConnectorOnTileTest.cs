using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSystemSimulationEngine.Classes;
using static System.Math;

namespace Cytos_v2_Tests.Classes
{
    [TestClass]
    public class ConnectorOnTileTest
    {
        // Glues
        private static readonly Glue GlueA = new Glue("GlueA");
        private static readonly Glue GlueB = new Glue("GlueB");

        private static Tile Tile;
        private static Tile Tubule;

        [TestInitialize]
        public void Initialize()
        {
            Tile = CreateSquareTile("Tile1");
            Tubule = CreateSegment("Tubule1");
        }


        /// <summary>
        /// Provides a test tile - a square tile.
        /// </summary>
        /// <param name="name">Name of the tile.</param>
        private static Tile CreateSquareTile(string name)
        {
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
                    new Connector("c-in", new List<Point3D> { Point3D.Origin }, GlueA, Angle.FromDegrees(90), 0),
                    new Connector("c-out", new List<Point3D> { Point3D.Origin }, GlueA, Angle.FromDegrees(-90), 0)
                    },
                    null, null, Color.Aqua);
        }

        /// <summary>
        /// Provides a test tile - a segment with two connectors.
        /// </summary>
        /// <param name="name">Name of the segment.</param>
        private static Tile CreateSegment(string name)
        {
            var vertices = new List<Point3D>() {new Point3D(-1, 0, 0), new Point3D(1, 0, 0)};
            var connectors = vertices.Select((t, i) => new Connector($"cs{i}",
                new List<Point3D> {t},
                GlueB, Angle.FromDegrees(30), 0)).ToList();
            return new Tile(name, vertices, connectors, null, null, Color.Red);
        }

        [TestMethod]
        public void TestConstructor()
        {
            TileInSpace baseTile = new TileInSpace(Tile, Point3D.Origin, Quaternion.One);

            Assert.AreEqual(-90, baseTile.Connectors[3].Angle.Degrees);
            Assert.AreEqual(Tile.SideType.outside, baseTile.Connectors[3].Side);

            Assert.AreEqual(90, baseTile.Connectors[2].Angle.Degrees);
            Assert.AreEqual(Tile.SideType.inside, baseTile.Connectors[2].Side);
        }


        [TestMethod]
        public void TestConnectObject()
        {
            TileInSpace baseTile = new TileInSpace(Tile, Point3D.Origin, Quaternion.One);
            TileInSpace eastTile = baseTile.Connectors[0].ConnectObject(Tile.Connectors[1]);
            TileInSpace southTile = eastTile.Connectors[0].ConnectObject(Tile.Connectors[0]);
            TileInSpace topTile = southTile.Connectors[1].ConnectObject(Tile.Connectors[0]);

            // Assert.AreEqual(v_ObjInSpace3.Rotations, null);  //Debugging
            // Assert.AreEqual(v_ObjInSpace3.Quaternion.Print(), null));    // Debugging
            var vertices = topTile.Vertices.Select(v => new Point3D(Round(v.X,10), Round(v.Y,10), Round(v.Z,10)));
            Assert.AreEqual("(-5, -5, 10),(5, -5, 10),(5, 5, 10),(-5, 5, 10)", string.Join(",", vertices));

            TileInSpace verticalTubule = baseTile.Connectors[3].ConnectObject(Tubule.Connectors[0]);
            vertices = verticalTubule.Vertices.Select(v => new Point3D(Round(v.X, 10), Round(v.Y, 10), Round(v.Z, 10)));
            Assert.AreEqual("(0, 0, 0),(0, 0, -2)", string.Join(",", vertices));

            verticalTubule = baseTile.Connectors[2].ConnectObject(Tubule.Connectors[0]);
            vertices = verticalTubule.Vertices.Select(v => new Point3D(Round(v.X, 10), Round(v.Y, 10), Round(v.Z, 10)));
            Assert.AreEqual("(0, 0, 0),(0, 0, 2)", string.Join(",", vertices));

            // TODO add test of connection of a tile to a tubule

            TileInSpace slantTubule = verticalTubule.Connectors[1].ConnectObject(Tubule.Connectors[0]);
            Assert.AreEqual(2 + 2 * Cos(PI/6), slantTubule.Vertices[1].Z, MSystem.Tolerance);

        }

        [TestMethod]
        public void TestConnectToAndDisconnect()
        {
            Tile testTile1 = CreateSquareTile("test1");
            Tile testTile2 = CreateSquareTile("test2");

            TileInSpace testTileInSpace1 = new TileInSpace(testTile1, Point3D.Origin, Quaternion.One);
            TileInSpace testTileInSpace2 = new TileInSpace(testTile2, Point3D.Origin, Quaternion.One);

            ConnectorOnTileInSpace testConnectorOnTile1 = new ConnectorOnTileInSpace(testTileInSpace1, testTileInSpace1.Connectors[0]);
            ConnectorOnTileInSpace testConnectorOnTile2 = new ConnectorOnTileInSpace(testTileInSpace2, testTileInSpace2.Connectors[0]);
            testConnectorOnTile1.ConnectTo(testConnectorOnTile2);
            Assert.IsTrue(testConnectorOnTile1.ConnectedTo == testConnectorOnTile2);
            Assert.IsTrue(testConnectorOnTile2.ConnectedTo == testConnectorOnTile1);

            testConnectorOnTile1.Disconnect();
            Assert.IsNull(testConnectorOnTile1.ConnectedTo);
            Assert.IsNull(testConnectorOnTile2.ConnectedTo);
            Assert.IsFalse(testConnectorOnTile1.SetDisconnect);
            Assert.IsFalse(testConnectorOnTile2.SetDisconnect);
        }
    }
}
