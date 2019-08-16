using System;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Cytos_v2_Tests.Classes.Tools;
using MSystemSimulationEngine.Classes;

namespace Cytos_v2_Tests.Classes
{
    [TestClass]
    public class TileTest
    {
        #region Public methods

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentException), "Name of the simulation object can't be null or empty string.")]
        public void TestTileConstructorNullName()
        {
            Tile fo = new Tile(null, null, null, null, null, Color.Black);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentException), "Name of the simulation object can't be null or empty string.")]
        public void TestTileConstructorEmptyName()
        {
            Tile fo = new Tile("", null, null, null, null, Color.Black);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentException), "Vertices of tile fo cannot be null.")]
        public void TestTileConstructorNullVertices()
        {
            Tile fo = new Tile("fo", null, null, null, null, Color.Black);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentException), "Polygon fo has 0 vertices, must have >= 3")]
        public void TestTileConstructorEmptyVertices()
        {
            List<Point3D> points = new List<Point3D>();
            Tile fo = new Tile("fo", points, null, null, null, Color.Black);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentException), "Polygon fo has 1 vertices, must have >= 3")]
        public void TestTileConstructorOneVertex()
        {
            List<Point3D> points = new List<Point3D> { new Point3D(0, 0, 0) };
            Tile fo = new Tile("fo", points, null, null, null, Color.Black);
        }

        [TestMethod]
        public void TestTileMinimalistic()
        {
            // test minimalistic Tile
  
            // 2 points list
            List<Point3D> points = new List<Point3D> { new Point3D(0, 0, 0), new Point3D(1, 0, 0) };
            Tile fo = new Tile("fo", points, null, null, null, Color.Black);

            Assert.IsTrue(fo.Name == "fo");
            Assert.IsTrue(fo.Color == Color.Black);
            Assert.IsTrue(fo.Vertices.Count == points.Count);
            foreach (Point3D pt in points)
            {   // testing fo.Vertices content here as we know Segment3D should be created
                Assert.IsTrue(((Segment3D)fo.Vertices).Contains(pt));
            }
            Assert.IsNotNull(fo.SurfaceGlue);
            Assert.IsTrue(fo.SurfaceGlue.Name == "EmptyGlue");
            Assert.IsNotNull(fo.Connectors);
            Assert.IsTrue(fo.Connectors.Count == 0);

            // 3 points list
            points = new List<Point3D> { new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(0, 1, 0) };
            fo = new Tile("fo", points, null, null, null, Color.Black);

            Assert.IsTrue(fo.Name == "fo");
            Assert.IsTrue(fo.Color == Color.Black);
            Assert.IsTrue(fo.Vertices.Count == points.Count);
            foreach (Point3D pt in points)
            {   // testing fo.Vertices content here as we know Polygon3D should be created
                Assert.IsTrue(((Polygon3D)fo.Vertices).Contains(pt));
            }
            Assert.IsNotNull(fo.SurfaceGlue);
            Assert.IsTrue(fo.SurfaceGlue.Name == "EmptyGlue");
            Assert.IsNotNull(fo.Connectors);
            Assert.IsTrue(fo.Connectors.Count == 0);
        }

        [TestMethod]
        public void TestTileConstructorGlue()
        {
            // test SurfaceGlue of Tile but not much to test

            // 2 points list
            List<Point3D> points = new List<Point3D> { new Point3D(0, 0, 0), new Point3D(1, 0, 0) };
            Tile fo = new Tile("fo", points, null, new Glue("g1"), null, Color.Black);

            Assert.IsTrue(fo.SurfaceGlue.Name == "g1");
        }

        [TestMethod]
        public void TestTileConnectorOnRod()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1 and v2 for tile (rod)
            Point3D v2 = new Point3D(2, 0, 0);
            List<Point3D> points = new List<Point3D> { v1, v2 };  // vertexes for our tile (rod)
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            // connector (point) at v1
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { v1 }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);

            // connector (point) at v2
            connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { v2 }, glue, angle, 0) };
            fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTileInvalidConnectorOnRod()
        {
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1 and v2 for tile (rod)
            Point3D v2 = new Point3D(2, 0, 0);
            List<Point3D> points = new List<Point3D> { v1, v2 };  // vertexes for our tile (rod)
            Point3D c1Pos = new Point3D(1, 0, 0);  // in between v1 and v2
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            // connector (point) in the middle of rod is incorrect
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { c1Pos }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTileInvalidConnectorOutsideRod()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1 and v2 for tile (rod)
            Point3D v2 = new Point3D(2, 0, 0);
            Point3D c2Pos = new Point3D(-1, 0, 0); // outside of rod
            List<Point3D> points = new List<Point3D> { v1, v2 };  // vertexes for our tile (rod)
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            // connector (point) outside of rod but on the line
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { c2Pos }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTileInvalidConnectorNotOnRod()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1 and v2 for tile (rod)
            Point3D v2 = new Point3D(2, 0, 0);
            Point3D c3Pos = new Point3D(3, 0, 0);  // outside of rod
            List<Point3D> points = new List<Point3D> { v1, v2 };  // vertexes for our tile (rod)
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            // connector (point) outside of rod but on the line
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { c3Pos }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        // No message tested: it can be in different languages due to system's localization
        public void TestTileInvalidConnectorPairOnRod()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1 and v2 for tile (rod)
            Point3D v2 = new Point3D(2, 0, 0);
            List<Point3D> points = new List<Point3D> { v1, v2 };  // vertexes for our tile (rod)
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            // test connector at v1 and v2
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { v1, v2 }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        public void TestTileValidConnectorOnPolygon()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1, v2 and v3 for tile (polygon -> triangle)
            Point3D v2 = new Point3D(2, 0, 0);
            Point3D v3 = new Point3D(0, 3, 0);
            Point3D c1Pos = new Point3D(1, 0, 0);       // on the edge between v1 and v2
            Point3D c3Pos = new Point3D(1, 1, 0);       // inside of polygon
            List<Point3D> points = new List<Point3D> { v1, v2, v3 };  // vertexes for our tile (polygon)
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            // connector (point) at v1
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { v1 }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
            // connector (point) on the edge between v1 and v2
            connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { c1Pos }, glue, angle, 0) };
            fo = new Tile("fo", points, connectors, glue, null, Color.Black);
            // connector (point) inside of polygon
            connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { c3Pos }, glue, Angle.FromDegrees(90), 0) };
            fo = new Tile("fo", points, connectors, glue, null, Color.Black);
            // connector (rod) at v1 - v2
            connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { v1, v2 }, glue, angle, 0) };
            fo = new Tile("fo", points, connectors, glue, null, Color.Black);
            // connector (rod) at v2 - v3
            connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { v2, v3 }, glue, angle, 0) };
            fo = new Tile("fo", points, connectors, glue, null, Color.Black);
            // connector (rod) at v3 - v1
            connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { v3, v1 }, glue, angle, 0) };
            fo = new Tile("fo", points, connectors, glue, null, Color.Black);

            // two connectors at the same place allowed
            Connector c1 = new Connector("cn1", new List<Point3D> { v1, v2 }, glue, angle, 0);
            Connector c2 = new Connector("cn2", new List<Point3D> { v1, v2 }, glue, angle, 0);

            connectors = new List<Connector> { c1, c2 };
            fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTileConnectorOutsidePolygon()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1, v2 and v3 for tile (polygon -> triangle)
            Point3D v2 = new Point3D(2, 0, 0);
            Point3D v3 = new Point3D(0, 3, 0);
            Point3D c2Pos = new Point3D(-1, 0, 0);      // outside of polygon
            List<Point3D> points = new List<Point3D> { v1, v2, v3 };  // vertexes for our tile (polygon)
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            // connector (point) outside of polygon
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { c2Pos }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTileConnectorOpositeDirection01()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1, v2 and v3 for tile (polygon -> triangle)
            Point3D v2 = new Point3D(2, 0, 0);
            Point3D v3 = new Point3D(0, 3, 0);
            List<Point3D> points = new List<Point3D> { v1, v2, v3 };  // vertexes for our Tile (polygon)
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            // connector (rod) at v2 - v1 is not allowed (oposite direction not allowed)
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { v2, v1 }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTileConnectorOpositeDirection02()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1, v2 and v3 for tile (polygon -> triangle)
            Point3D v2 = new Point3D(2, 0, 0);
            Point3D v3 = new Point3D(0, 3, 0);
            List<Point3D> points = new List<Point3D> { v1, v2, v3 };  // vertexes for our tile (polygon)
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            // connector (rod) at v3 - v2 is not allowed (oposite direction not allowed)
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { v3, v2 }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTileConnectorOpositeDirection03()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1, v2 and v3 for tile (polygon -> triangle)
            Point3D v2 = new Point3D(2, 0, 0);
            Point3D v3 = new Point3D(0, 3, 0);
            List<Point3D> points = new List<Point3D> { v1, v2, v3 };  // vertexes for our tile (polygon)
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            // connector (rod) at v1 - v3 is not allowed (oposite direction not allowed)
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { v1, v3 }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTileConnectorPairNotOnPolygon()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1, v2 and v3 for tile (polygon -> triangle)
            Point3D v2 = new Point3D(2, 0, 0);
            Point3D v3 = new Point3D(0, 3, 0);
            Point3D c2Pos = new Point3D(-1, 0, 0);      // outside of polygon
            List<Point3D> points = new List<Point3D> { v1, v2, v3 };  // vertexes for our tile (polygon)
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            //  connector with one point outside of edge
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { c2Pos, v2 }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTileConnectorPairInsidePolygon()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1, v2 and v3 for tile (polygon -> triangle)
            Point3D v2 = new Point3D(2, 0, 0);
            Point3D v3 = new Point3D(0, 3, 0);
            Point3D c3Pos = new Point3D(1, 1, 0);       // inside of polygon
            Point3D c4Pos = new Point3D(0.5, 0.5, 0);   // inside of polygon
            List<Point3D> points = new List<Point3D> { v1, v2, v3 };  // vertexes for our tile (polygon)
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            //  connector (rod) inside of polygon
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { c3Pos, c4Pos }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTileConnectorWrongPairOnPolygon()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1, v2 and v3 for tile (polygon -> triangle)
            Point3D v2 = new Point3D(2, 0, 0);
            Point3D v3 = new Point3D(0, 3, 0);
            Point3D c3Pos = new Point3D(1, 1, 0);       // inside of polygon
            List<Point3D> points = new List<Point3D> { v1, v2, v3 };  // vertexes for our tile (polygon)
            Glue glue = new Glue("g1");
            Angle angle = new Angle();

            //  connector (rod) - one point on vertex, another one inside of polygon
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { c3Pos, v1 }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTileConnectorDiagonalPairOnPolygon()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1, v2, v3 and v4 for tile (polygon -> rectangle)
            Point3D v2 = new Point3D(2, 0, 0); 
            Point3D v3 = new Point3D(2, 2, 0); 
            Point3D v4 = new Point3D(0, 2, 0);
            // create polygon and with connector which is on v1 and v3 (diagonal)
            List<Point3D> points = new List<Point3D> { v1, v2, v3, v4 };  
            Glue glue = new Glue("g1");
            Angle angle = new Angle();
             
            // connector (rod) at v1 - v3 (diagonal of the polygon - not allowed)
            List<Connector> connectors = new List<Connector> { new Connector("cn1", new List<Point3D> { v1, v3 }, glue, angle, 0) };
            Tile fo = new Tile("fo", points, connectors, glue, null, Color.Black);
        }

        [TestMethod]
        public void TestTileValidProteins()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1, v2, and v3 for tile (polygon -> triangle)
            Point3D v2 = new Point3D(2, 0, 0);
            Point3D v3 = new Point3D(0, 3, 0);
            Point3D c1Pos = new Point3D(1, 0, 0);  // between v1 and v2
            Point3D c3Pos = new Point3D(1, 1, 0);  // inside polygon

            List<Point3D> points = new List<Point3D> { v1, v2, v3 };  // vertexes for our Tile (polygon)
            Glue glue = new Glue("g1");

            // proteins at valid positions
            List<ProteinOnTile> proteins = new List<ProteinOnTile> {
                new ProteinOnTile("p1", v1), // at v1
                new ProteinOnTile("p2", v2), // at v2
                new ProteinOnTile("p3", v3), // at v3
                new ProteinOnTile("p4", c1Pos), // on edge v1 - v2
                new ProteinOnTile("p5", c3Pos)  // inside polygon
            };
            Tile fo = new Tile("fo", points, null, glue, proteins, Color.Black);
            CollectionAssert.AreEquivalent(proteins, fo.Proteins);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidOperationException), "Protein: name = p1, position = (3, 0, 0) must be on the object fo.")]
        public void TestTileNotOnEdgeProteins()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1, v2, and v3 for tile (polygon -> triangle)
            Point3D v2 = new Point3D(2, 0, 0);
            Point3D v3 = new Point3D(0, 3, 0);
            Point3D c2Pos = new Point3D(3, 0, 0);  // outside of polygon on line v1 - v2
            List<Point3D> points = new List<Point3D> { v1, v2, v3 };  // vertexes for our Tile (polygon)
            Glue glue = new Glue("g1");

            // proteins at invalid positions
            List<ProteinOnTile> proteins = new List<ProteinOnTile> { new ProteinOnTile("p1", c2Pos) };
            Tile fo = new Tile("fo", points, null, glue, proteins, Color.Black);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidOperationException), "Protein: name = p1, position = (3, 3, 0) must be on the object fo.")]
        public void TestTileNotOnPolygonProteins()
        {
            // create various points for tests below
            Point3D v1 = new Point3D(0, 0, 0);     // vertexes v1, v2, and v3 for tile (polygon -> triangle)
            Point3D v2 = new Point3D(2, 0, 0); 
            Point3D v3 = new Point3D(0, 3, 0); 
            Point3D c4Pos = new Point3D(3, 3, 0);  // outside of polygon
            List<Point3D> points = new List<Point3D> { v1, v2, v3 };  // vertexes for our tile (polygon)
            Glue glue = new Glue("g1");

            // proteins at invalid positions
            List<ProteinOnTile> proteins = new List<ProteinOnTile> { new ProteinOnTile("p1", c4Pos) };
            Tile fo = new Tile("fo", points, null, glue, proteins, Color.Black);
        }

        #endregion
    }
}
