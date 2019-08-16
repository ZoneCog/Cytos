using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MathNet.Spatial.Euclidean;
using Cytos_v2_Tests.Classes.Tools;
using MSystemSimulationEngine.Classes;
using MSystemSimulationEngine.Classes.Xml;

namespace Cytos_v2_Tests.Classes
{

    [TestClass]
    public class TilesWorldTest
    {
        // some MathSage colors
        private string[] m_colors = new string[] { 
            "blue", "brown", "cyan", "gray", "green", "grey", "magenta", "orange",
            "pink", "purple", "red", "silver", "violet", "yellow", 
        };

        private MSystem m_testMSystem;
        private TilesWorld m_tilesWorld;
        private FloatingObjectsWorld m_floatingObjWorld;
        private DeserializedObjects mSystemObjects;
        private static XDocument mSystemDescription;
        private static string _path;

        // Initialize class => load test XML documents
        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _path = @"../../Classes/Xml/TestXML/testMSystemDescription.xml";
            mSystemDescription = XDocument.Load(_path);
        }

        // Initialize object/s before each test is run. It will be called for each test.
        [TestInitialize]
        public void Initialize()
        {
            mSystemObjects = new DeserializedObjects(mSystemDescription, _path);
            m_testMSystem = new MSystem(mSystemObjects);
            m_tilesWorld = new TilesWorld(m_testMSystem);
            m_floatingObjWorld = new FloatingObjectsWorld(m_testMSystem, m_tilesWorld);
            m_tilesWorld.FltObjectsWorld = m_floatingObjWorld;
        }

        //Clean up object/s after each test is run. It will be called for each test. 
        //(Use eg. for IDisposable objects or objects which state is changes during test run)
        [TestCleanup]
        public void CleanUp()
        {
            ;
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Parameter \'mSystem\' cannot be null.", true)]
        public void TestConstructorMSystemNull()
        {
            TilesWorld fow = new TilesWorld(null);
        }

        [TestMethod]
        public void TestConstructorEqualTiles()
        {
            // test the the number of elements in our test torld
            Assert.AreEqual(m_testMSystem.SeedTiles.Count, m_tilesWorld.ToList().Count);

            // TODO - not sure we can test much more here
        }

        [TestMethod]
        public void TestIntersectsWith()
        {
            // TODO
        }

        [TestMethod]
        public void TestAutoConnect()
        {
            // TODO include the cases of (shortened) rod auto-connecting to polygon (both to connector and surface)
        }

        /// <summary>
        /// Tests whether signal objects have been released at correct positions after creating a new connection
        /// </summary>
        [TestMethod]
        public void TestSignalObjects()
        {
            // TODO include also the cases of (shortened) rod auto-connecting to polygon (both to connector and surface)
        }

        [TestMethod]
        public void TestIsNarrow()
        {
            // TODO PETR: DO NOT TEST YET - THE METHOD WILL BE PROBABLY COMPLETELY CHANGED
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Parameter \'tile\' cannot be null", true)]
        public void TestAddNullTile()
        {
            m_tilesWorld.Add(null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Parameter \'freeConnector\' cannot be null", true)]
        public void TestAddNullConnector()
        {
            m_tilesWorld.Add(m_testMSystem.Tiles["s1"], null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentException), "Parameter \'freeConnector\' is already connected", true)]
        public void TestAddConnectedConnector()
        {
            int count = m_tilesWorld.Count();

            // TODO - I am not using the first one (and I shall) because of bug in pussing off

            // connect new polygon to last edge connector on first object in the world
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[0].Connectors[4]));
            Assert.AreEqual(++count, m_tilesWorld.Count());

            // connect new polygon to last connector again => this time it shall fail
            m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[0].Connectors[4]);
        }


        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentNullException), "Parameter \'tile\' cannot be null", true)]
        public void TestRemoveNullObject()
        {
            m_tilesWorld.Remove(null);
        }

        [TestMethod]
        public void TestAddConnectorsCompatibilityOnPolygon()
        {
            int count = m_tilesWorld.Count();

            // try to add rod on polygon with incompatible connectors
            Assert.IsFalse(m_tilesWorld.Add(m_testMSystem.Tiles["s1"], m_tilesWorld.ToList()[0].Connectors[0]));
            Assert.AreEqual(count, m_tilesWorld.Count()); // number of elements did not change

            Assert.IsFalse(m_tilesWorld.Add(m_testMSystem.Tiles["s1"], m_tilesWorld.ToList()[0].Connectors[1]));
            Assert.AreEqual(count, m_tilesWorld.Count()); // number of elements did not change

            Assert.IsFalse(m_tilesWorld.Add(m_testMSystem.Tiles["s1"], m_tilesWorld.ToList()[0].Connectors[2]));
            Assert.AreEqual(count, m_tilesWorld.Count()); // number of elements did not change

            Assert.IsFalse(m_tilesWorld.Add(m_testMSystem.Tiles["s1"], m_tilesWorld.ToList()[0].Connectors[3]));
            Assert.AreEqual(count, m_tilesWorld.Count()); // number of elements did not change

            Assert.IsFalse(m_tilesWorld.Add(m_testMSystem.Tiles["s1"], m_tilesWorld.ToList()[0].Connectors[4]));
            Assert.AreEqual(count, m_tilesWorld.Count()); // number of elements did not change

            // this time, rod gets connected to polygon's compatible connector on surface
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["s1"], m_tilesWorld.ToList()[0].Connectors[5]));
            // there is one more object now attached in tiles world
            Assert.AreEqual(++count, m_tilesWorld.Count());
        }

        [TestMethod]
        public void TestAddConnectorsCompatibilityOnRod()
        {
            int count = m_tilesWorld.Count();

            // try to add polygon to rod with incompatible connectors
            Assert.AreEqual(count, m_tilesWorld.Count()); // number of elements did not change
            Assert.IsFalse(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[2].Connectors[0]));

            // try to add polygon to rod with incompatible connectors
            Assert.IsFalse(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[2].Connectors[1]));
            Assert.AreEqual(count, m_tilesWorld.Count()); // number of elements did not change

            // this time, polygon gets connected to rod's compatible connector 
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[2].Connectors[0]));
            Assert.AreEqual(++count, m_tilesWorld.Count());

            // try to add polygon to rod with incompatible connectors
            Assert.IsFalse(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[2].Connectors[1]));
            Assert.AreEqual(count, m_tilesWorld.Count());
        }

        // add new polygon to first one to its first connectors => result: move second polygon above the first one
        [TestMethod]
        public void TestVeryBasicPushingPolygonAByPolygon()
        {
            // save positions
            Point3D originalPosPoly1 = m_tilesWorld.ToList()[1].Position;
            Point3D originalPosLine1 = m_tilesWorld.ToList()[2].Position;

            // add object and perform health check
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[0].Connectors[0]));
            PerformHealthCheck();

            // get current position of the polygon, position must be different
            Point3D newPosPoly1 = m_tilesWorld.ToList()[1].Position;
            Assert.AreNotEqual(originalPosPoly1, newPosPoly1);
            Point3D expectedPosPoly1 = new Point3D(11.5219303466815, 14.7294308566871, 31.9967477524977);
            Assert.IsTrue(expectedPosPoly1.Equals(newPosPoly1, MSystem.Tolerance));

            // get current position of the line, positions must be the same
            Point3D newPosLine1 = m_tilesWorld.ToList()[2].Position;
            Assert.AreEqual(originalPosLine1, newPosLine1);
        }

        // add new line in the middle of first polygon on its 6th connectors => result: move second polygon above the first one
        [TestMethod]
        public void TestVeryBasicPushingPolygonByLine()
        {
            // save positions
            Point3D originalPosPoly1 = m_tilesWorld.ToList()[1].Position;
            Point3D originalPosLine1 = m_tilesWorld.ToList()[2].Position;

            // add object and perform health check
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["s1"], m_tilesWorld.ToList()[0].Connectors[5]));
            PerformHealthCheck();

            // get current position of the polygon, position must be different
            Point3D newPosPoly1 = m_tilesWorld.ToList()[1].Position;
            Assert.AreNotEqual(originalPosPoly1, newPosPoly1);
            Assert.AreEqual(7.07106781186548, newPosPoly1.Z, MSystem.Tolerance); // X & Y vary but Z is always fixed

            // get current position of the line, positions must be the same
            Point3D newPosLine1 = m_tilesWorld.ToList()[2].Position;
            Assert.AreEqual(originalPosLine1, newPosLine1);
        }

        // add new polygon to first one to its 3th connectors => result: move line above first polygon
        [TestMethod]
        public void TestVeryBasicPushingLineByPolygon()
        {
            // save positions
            Point3D originalPosPoly1 = m_tilesWorld.ToList()[1].Position;
            Point3D originalPosLine1 = m_tilesWorld.ToList()[2].Position;

            // add object and perform health check
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[0].Connectors[2]));
            PerformHealthCheck();

            // get current position of the line, position must be different
            Point3D newPosLine1 = m_tilesWorld.ToList()[2].Position;
            Assert.AreNotEqual(originalPosLine1, newPosLine1);
            Point3D expectedPosLine1 = new Point3D(3, -24.3539038811117, 27.7078077622233);
            Assert.IsTrue(expectedPosLine1.Equals(newPosLine1, MSystem.Tolerance));

            // get current position of the polygon, positions must be the same
            Point3D newPosPoly1 = m_tilesWorld.ToList()[1].Position;
            Assert.AreEqual(originalPosPoly1, newPosPoly1);
        }

        // add new polygon to first one on its 4th connector => result: no push
        [TestMethod]
        public void TestVeryBasicPushingNoPush()
        {
            // save positions
            Point3D originalPosPoly1 = m_tilesWorld.ToList()[1].Position;
            Point3D originalPosLine1 = m_tilesWorld.ToList()[2].Position;

            // add object and perform health check
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[0].Connectors[3]));
            PerformHealthCheck();

            // get current position of the polygon, position must be the same
            Point3D newPosPoly1 = m_tilesWorld.ToList()[1].Position;
            Point3D newPosLine1 = m_tilesWorld.ToList()[2].Position;
            Assert.AreEqual(originalPosPoly1, newPosPoly1);
            Assert.AreEqual(originalPosLine1, newPosLine1);
        }

        // add several connectors to second polygon (create structure), then add new polygon to first polygon on connector that
        // will move the sctructure
        [TestMethod]
        public void TestPushingComplexObject01()
        {
            // create structure on second polygon
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[0]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[1]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[2]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[3]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[4]));

            PerformHealthCheck();

            // save positions
            Point3D originalPosPoly0 = m_tilesWorld.ToList()[0].Position;
            Point3D originalPosPoly1 = m_tilesWorld.ToList()[1].Position;
            Point3D originalPosPoly2 = m_tilesWorld.ToList()[3].Position;
            Point3D originalPosPoly3 = m_tilesWorld.ToList()[4].Position;
            Point3D originalPosPoly4 = m_tilesWorld.ToList()[5].Position;
            Point3D originalPosPoly5 = m_tilesWorld.ToList()[6].Position;
            Point3D originalPosPoly6 = m_tilesWorld.ToList()[7].Position;
            Point3D originalPosLine1 = m_tilesWorld.ToList()[2].Position;

            // add object and perform health check
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[0].Connectors[0]));
            PerformHealthCheck();

            // polygon we were adding to must not move
            Point3D newPosPoly0 = m_tilesWorld.ToList()[0].Position;
            Assert.AreEqual(originalPosPoly0, newPosPoly0);
            // we check initial line => it shall not move
            Point3D newPosLine1 = m_tilesWorld.ToList()[2].Position;
            Assert.AreEqual(originalPosLine1, newPosLine1);

            // get current position of the polygon above base polygon, position must be different, check it out
            Point3D newPosPoly1 = m_tilesWorld.ToList()[1].Position;
            Assert.AreNotEqual(originalPosPoly1, newPosPoly1);
            Point3D expectedPosPoly1 = new Point3D(11.5219303466815, 14.7294308566871, 31.9967477524977);
            Assert.IsTrue(expectedPosPoly1.Equals(newPosPoly1, MSystem.Tolerance));

            Point3D newPosPoly2 = m_tilesWorld.ToList()[3].Position;
            Assert.AreNotEqual(originalPosPoly2, newPosPoly2);
            Point3D expectedPosPoly2 = new Point3D(18.4038399490374, 24.2015668116867, 39.2328157299975);
            Assert.IsTrue(expectedPosPoly2.Equals(newPosPoly2, MSystem.Tolerance));
            
            Point3D newPosPoly3 = m_tilesWorld.ToList()[4].Position;
            Assert.AreNotEqual(originalPosPoly3, newPosPoly3);
            Point3D expectedPosPoly3 = new Point3D(22.6570939907976, 11.1113968679372, 39.2328157299975);
            Assert.IsTrue(expectedPosPoly3.Equals(newPosPoly3, MSystem.Tolerance));

            Point3D newPosPoly4 = m_tilesWorld.ToList()[5].Position;
            Assert.AreNotEqual(originalPosPoly4, newPosPoly4);
            Point3D expectedPosPoly4 = new Point3D(11.5219303466816, 3.02122692418774, 39.2328157299975);
            Assert.IsTrue(expectedPosPoly4.Equals(newPosPoly4, MSystem.Tolerance));

            Point3D newPosPoly5 = m_tilesWorld.ToList()[6].Position;
            Assert.AreNotEqual(originalPosPoly5, newPosPoly5);
            Point3D expectedPosPoly5 = new Point3D(0.386766702565478, 11.1113968679372, 39.2328157299975);
            Assert.IsTrue(expectedPosPoly5.Equals(newPosPoly5, MSystem.Tolerance));

            Point3D newPosPoly6 = m_tilesWorld.ToList()[7].Position;
            Assert.AreNotEqual(originalPosPoly6, newPosPoly6);
            Point3D expectedPosPoly6 = new Point3D(4.64002074432568, 24.2015668116867, 39.2328157299975);
            Assert.IsTrue(expectedPosPoly6.Equals(newPosPoly6, MSystem.Tolerance));
        }

        // build the structure on base polygon in such a way that adding polygon to second one is impossible
        [TestMethod]
        public void TestPushingComplexObject02()
        {
            // create structure on second polygon
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[0].Connectors[3]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[3].Connectors[2]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[4].Connectors[3]));
            // build rest of the cell from top
            foreach (var con in m_tilesWorld.ToList()[5].Connectors)
            {
                if (con.ConnectedTo == null && con.Name != "c3")
                    Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], con));
            }
            PerformHealthCheck();

            // now we want to add another polygon to the base, this polygon will try to move our second base polygon or line
            // which cannot be moved because polygons above are connected to the base
            Assert.IsFalse(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[0].Connectors[0]));
            Assert.IsFalse(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[0].Connectors[1]));
            Assert.IsFalse(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[0].Connectors[1]));
            // this is the only edge on base polygon which is not conflicting anything
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[0].Connectors[4]));
            PerformHealthCheck();
        }

        // add several connectors to second polygon (create structure), then add new polygon to first polygon on connector that
        // will move the sctructure
        [TestMethod]
        public void TestPushingHalvesOfPolyhedron()
        {
            // create structure on second polygon
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[0]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[1]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[2]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[3]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[4]));

            // add second layer of tiles
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[3].Connectors.First(conn => conn.ConnectedTo == null)));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[4].Connectors.First(conn => conn.ConnectedTo == null)));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[5].Connectors.First(conn => conn.ConnectedTo == null)));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[6].Connectors.First(conn => conn.ConnectedTo == null)));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[7].Connectors.First(conn => conn.ConnectedTo == null)));

            // add final top tile - complete polyhedron
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[8].Connectors.First(conn => conn.ConnectedTo == null)));
            // check whether all adjacent edges of the top tile connected
            Assert.IsFalse(m_tilesWorld.ToList()[8].Connectors.Any(conn => conn.ConnectedTo == null));
            PerformHealthCheck();

            // save positions, bottom half
            Point3D originalPosPoly1 = m_tilesWorld.ToList()[1].Position;
            Point3D originalPosPoly2 = m_tilesWorld.ToList()[3].Position;
            Point3D originalPosPoly3 = m_tilesWorld.ToList()[4].Position;
            Point3D originalPosPoly4 = m_tilesWorld.ToList()[5].Position;
            Point3D originalPosPoly5 = m_tilesWorld.ToList()[6].Position;
            Point3D originalPosPoly6 = m_tilesWorld.ToList()[7].Position;

            // save positions, upper half
            Point3D originalPosPoly7 = m_tilesWorld.ToList()[8].Position;
            Point3D originalPosPoly8 = m_tilesWorld.ToList()[9].Position;
            Point3D originalPosPoly9 = m_tilesWorld.ToList()[10].Position;
            Point3D originalPosPoly10 = m_tilesWorld.ToList()[11].Position;
            Point3D originalPosPoly11 = m_tilesWorld.ToList()[12].Position;
            Point3D originalPosPoly12 = m_tilesWorld.ToList()[13].Position;

            // Divide the "equatorial" connectors on tiles => split polyhedron to two halves
            // basicly spliting the cell
            foreach (var tile in m_tilesWorld)
                foreach (var conn in tile.Connectors)
                    if (conn.Positions.All(pos => 12 < pos.Z && pos.Z < 20 ))
                        conn.Disconnect();

            // add object (tile No 14) and perform health check
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[3].Connectors.First(conn => conn.ConnectedTo == null)));
            PerformHealthCheck();

            // bottom half must not change position
            Point3D newPosPoly1 = m_tilesWorld.ToList()[1].Position;
            Point3D newPosPoly2 = m_tilesWorld.ToList()[3].Position;
            Point3D newPosPoly3 = m_tilesWorld.ToList()[4].Position;
            Point3D newPosPoly4 = m_tilesWorld.ToList()[5].Position;
            Point3D newPosPoly5 = m_tilesWorld.ToList()[6].Position;
            Point3D newPosPoly6 = m_tilesWorld.ToList()[7].Position;
            Assert.AreEqual(originalPosPoly1, newPosPoly1);
            Assert.AreEqual(originalPosPoly2, newPosPoly2);
            Assert.AreEqual(originalPosPoly3, newPosPoly3);
            Assert.AreEqual(originalPosPoly4, newPosPoly4);
            Assert.AreEqual(originalPosPoly5, newPosPoly5);
            Assert.AreEqual(originalPosPoly6, newPosPoly6);

            // upper half must change position
            Point3D newPosPoly7 = m_tilesWorld.ToList()[8].Position;
            Point3D newPosPoly8 = m_tilesWorld.ToList()[9].Position;
            Point3D newPosPoly9 = m_tilesWorld.ToList()[10].Position;
            Point3D newPosPoly10 = m_tilesWorld.ToList()[11].Position;
            Point3D newPosPoly11 = m_tilesWorld.ToList()[12].Position;
            Point3D newPosPoly12 = m_tilesWorld.ToList()[13].Position;
            Assert.AreNotEqual(originalPosPoly7, newPosPoly7);
            Assert.AreNotEqual(originalPosPoly8, newPosPoly8);
            Assert.AreNotEqual(originalPosPoly9, newPosPoly9);
            Assert.AreNotEqual(originalPosPoly10, newPosPoly10);
            Assert.AreNotEqual(originalPosPoly11, newPosPoly11);
            Assert.AreNotEqual(originalPosPoly12, newPosPoly12);

            // Now add more tiles - complete two new cells

            // add upper layer of tiles to the lower polyhedron
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[4].Connectors.First(conn => conn.ConnectedTo == null)));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[5].Connectors.First(conn => conn.ConnectedTo == null)));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[6].Connectors.First(conn => conn.ConnectedTo == null)));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[7].Connectors.First(conn => conn.ConnectedTo == null)));

            // An attempt to add this tile first FAILS!
            // Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[8].Connectors.First(conn => conn.ConnectedTo == null)));

            // add lower layer of tiles to the upper polyhedron
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[9].Connectors.First(conn => conn.ConnectedTo == null)));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[10].Connectors.First(conn => conn.ConnectedTo == null)));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[11].Connectors.First(conn => conn.ConnectedTo == null)));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[12].Connectors.First(conn => conn.ConnectedTo == null)));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[8].Connectors.First(conn => conn.ConnectedTo == null)));

            // add top tile to the lower polyhedron
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[14].Connectors.First(conn => conn.ConnectedTo == null)));

            // add botom tile to the upper polyhedron
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q2"], m_tilesWorld.ToList()[20].Connectors.First(conn => conn.ConnectedTo == null)));
        }

        // create two polygons on second polygon on two edges apart, then create polygon between them and check that
        // connector of the three new polygons are joined
        [TestMethod]
        public void TestAdjacentConnectorsConnected()
        {
            // create structure on second polygon
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[0]));
            // - skip Connector[1] - this is the intention
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[2]));
            PerformHealthCheck();

            // the following connectors shall be connected on initial polygon
            Assert.IsNotNull(m_tilesWorld.ToList()[1].Connectors[0].ConnectedTo);
            Assert.IsNotNull(m_tilesWorld.ToList()[1].Connectors[2].ConnectedTo);

            // the following connctors shall NOT be connected on initial polygon
            Assert.IsNull(m_tilesWorld.ToList()[1].Connectors[1].ConnectedTo);
            Assert.IsNull(m_tilesWorld.ToList()[1].Connectors[3].ConnectedTo);
            Assert.IsNull(m_tilesWorld.ToList()[1].Connectors[4].ConnectedTo);

            // check connectors on firstly added polygon
            TileInSpace foisA = m_tilesWorld.ToList()[3];
            Assert.IsNotNull(foisA.Connectors[0].ConnectedTo); // connected to starting polygon
            Assert.IsNull(foisA.Connectors[1].ConnectedTo);
            Assert.IsNull(foisA.Connectors[2].ConnectedTo);
            Assert.IsNull(foisA.Connectors[3].ConnectedTo);
            Assert.IsNull(foisA.Connectors[4].ConnectedTo);
            Assert.IsNull(foisA.Connectors[5].ConnectedTo);

            // check connectors on second added polygon
            TileInSpace foisB = m_tilesWorld.ToList()[4];
            Assert.IsNotNull(foisB.Connectors[0].ConnectedTo); // connected to starting polygon
            Assert.IsNull(foisB.Connectors[1].ConnectedTo);
            Assert.IsNull(foisB.Connectors[2].ConnectedTo);
            Assert.IsNull(foisB.Connectors[3].ConnectedTo);
            Assert.IsNull(foisB.Connectors[4].ConnectedTo);
            Assert.IsNull(foisB.Connectors[5].ConnectedTo);

            // now add polygon on Connector[1] => in between already added polygons
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[1]));
            PerformHealthCheck();

            // check connectors of all the polygons, adjecent polygons shall connect as well
            // the following connectors shall be connected on initial polygon
            Assert.IsNotNull(m_tilesWorld.ToList()[1].Connectors[0].ConnectedTo);
            Assert.IsNotNull(m_tilesWorld.ToList()[1].Connectors[1].ConnectedTo);
            Assert.IsNotNull(m_tilesWorld.ToList()[1].Connectors[2].ConnectedTo);

            // the following connctors shall NOT be connected on initial polygon
            Assert.IsNull(m_tilesWorld.ToList()[1].Connectors[3].ConnectedTo);
            Assert.IsNull(m_tilesWorld.ToList()[1].Connectors[4].ConnectedTo);

            // check connectors on firstly added polygon
            foisA = m_tilesWorld.ToList()[3];
            Assert.IsNotNull(foisA.Connectors[0].ConnectedTo); // connected to starting polygon
            Assert.IsNull(foisA.Connectors[1].ConnectedTo);
            Assert.IsNull(foisA.Connectors[2].ConnectedTo);
            Assert.IsNull(foisA.Connectors[3].ConnectedTo);
            Assert.IsNotNull(foisA.Connectors[4].ConnectedTo); // connected to middle polygon
            Assert.IsNull(foisA.Connectors[5].ConnectedTo);

            // check connectors on second added polygon
            foisB = m_tilesWorld.ToList()[4];
            Assert.IsNotNull(foisB.Connectors[0].ConnectedTo); // connected to starting polygon
            Assert.IsNotNull(foisB.Connectors[1].ConnectedTo); // connected to middle polygon
            Assert.IsNull(foisB.Connectors[2].ConnectedTo);
            Assert.IsNull(foisB.Connectors[3].ConnectedTo);
            Assert.IsNull(foisB.Connectors[4].ConnectedTo);
            Assert.IsNull(foisB.Connectors[5].ConnectedTo);

            // check connectors on middle polygon
            TileInSpace foisMiddle = m_tilesWorld.ToList()[5];
            Assert.IsNotNull(foisMiddle.Connectors[0].ConnectedTo); // connected to foisA 
            Assert.IsNotNull(foisMiddle.Connectors[1].ConnectedTo); // connected to starting polygon
            Assert.IsNotNull(foisMiddle.Connectors[2].ConnectedTo);  // connected to foisB
            Assert.IsNull(foisMiddle.Connectors[3].ConnectedTo);
            Assert.IsNull(foisMiddle.Connectors[4].ConnectedTo);
            Assert.IsNull(foisMiddle.Connectors[5].ConnectedTo);
        }


        // build polygons around base polygon and remove base polygon then
        [TestMethod]
        public void TestRemoveBase()
        {
            // create structure on second polygon
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[0]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[1]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[2]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[3]));
            Assert.IsTrue(m_tilesWorld.Add(m_testMSystem.Tiles["q1"], m_tilesWorld.ToList()[1].Connectors[4]));
            PerformHealthCheck();

            int count = m_tilesWorld.Count();
            m_tilesWorld.Remove(m_tilesWorld.ToList()[1]);
            PerformHealthCheck();

            // there must be one object less in the collection
            Assert.AreEqual(--count, m_tilesWorld.Count());
            // now test connectors on all previously connected objects
            for (int i = 2; i < count; i++)
            {
                for (int j = 0; j < m_tilesWorld.ToList()[i].Connectors.Count; j++)
                {
                    if (i % 2 == 0 && (j == 1 || j == 4) ||
                        i % 2 == 1 && (j == 0 || j == 2))
                    {   
                        // these connectors shall be connected
                        Assert.IsNotNull(m_tilesWorld.ToList()[i].Connectors[j].ConnectedTo);
                    }
                    else
                    {
                        // these connctors shall be free
                        Assert.IsNull(m_tilesWorld.ToList()[i].Connectors[j].ConnectedTo);
                    }
                }
            }
        }

        // XJB DEBUG BEGIN

        // MyRandom class is used for DEBUG purposes.
        class MyRandom
        {
            private Random m_rnd;
            private int[] m_sequence = {
                /* C1 begin sequence */
                0,2,1,  0,0,1, 1,0,2,  0,5,3,  0,1,0,  0,7,2,  0,8,2,  1,3,2,  1,1,4,  0,9,4,  1,7,4,  0,6,3,
                1,6,4,  1,6,1
                /* end sequence */

                /* C2 begin sequence 
                2,2,0,  0,1,4,  2,3,1,  2,2,1,  0,5,1,  0,0,4,  0,4,3,  0,0,3,  1,1,1,  0,11,3, 1,10,2, 1,9,4,
                1,10,1, 0,11,4, 0,8,2,  2,4,5,  1,17,3, 0,11,2, 0,4,1,  2,6,1,  2,20,5, 2,18,1, 1,19,4
                /* end sequence */

                /* C3 begin sequence - this one generates error by adding last polygon to the structure (p40) 
                1,1,3,  2,2,1,  0,0,2,  0,1,0,  1,5,3,  0,5,1,  0,6,3,  0,9,3,  0,10,4, 1,3,3,  1,10,2, 0,4,1,
                2,0,5,  1,7,4,  2,15,1, 1,10,1, 2,17,1, 0,14,1, 0,18,2, 2,19,1, 2,2,0,  0,14,3, 2,22,1, 0,5,2,
                2,23,1, 0,12,4, 1,20,4, 0,20,3, 0,29,2, 1,31,3, 0,31,2, 1,32,3, 0,25,1, 2,28,5, 0,7,3,  1,7,2,
                2,8,5,  0,0,0
                /* end sequence */

                /* C4 begin sequence 
                0,1,3,  0,1,2,  2,2,0,  0,3,3,  0,1,1,  2,5,1,  0,3,2,  1,1,4,  0,10,3, 0,6,2,  2,2,1,  2,12,5,
                0,0,0,  1,15,3, 0,10,4, 0,8,1,  0,0,1,  2,13,1, 1,18,2, 1,21,2, 0,22,4, 0,22,1, 2,3,5,  1,7,3,
                0,26,1, 2,4,5,  2,25,1, 2,6,5,  0,23,1
                /*  end sequence */

                /* C5 begin sequence 
                1,1,1,  0,2,0,  1,4,0,  1,0,2,  0,4,3,  1,4,4,  1,5,2,  1,5,3,  0,1,0,  1,0,0,  1,10,2, 1,3,4,
                1,14,3, 0,14,1, 1,6,1,  1,0,3,  1,15,1, 1,1,4,  0,12,1, 0,5,4,  1,12,3, 0,9,2,  0,21,4, 1,7,2,
                1,18,3, 0,2,1,  0,13,4, 1,16,2, 1,11,2, 0,30,4, 1,29,2, 1,19,4, 1,25,2, 2,24,5, 1,35,3, 2,29,5,
                2,0,5,  1,28,2, 1,6,3,  2,7,5,  2,36,1, 2,16,5, 2,43,1, 2,11,5, 2,39,1, 2,42,1, 0,40,1, 0,49,4, 
                2,48,1, 2,47,1
                /* end sequence */
            };
            private int m_sequencePos = 0;

            public MyRandom(Random rnd = null)
            {
                m_rnd = rnd;
            }

            public int Next(int maxValue)
            {
                if (m_rnd != null) return m_rnd.Next(maxValue);

                if (m_sequencePos == m_sequence.Count())
                    throw new IndexOutOfRangeException("Invalid position in the sequnce.");

                if (m_sequence[m_sequencePos] >= maxValue)
                    throw new ArgumentOutOfRangeException("Invalid argument");

                return m_sequence[m_sequencePos++];
            }
        };
        // XJB DEBUG BEGIN

        [TestMethod]
        public void TestBulkAddAndRemove()
        {
            // XJB DEBUG BEGIN
            string rndNumbers = "";
            // XJB DEBUG END

            // I am duplicating object names here but this way I can control what seed tiles I want to use in the test
            string[] objectNames = new string[] { "q1" , "q2", "s1" };

            // XJB DEBUG BEGIN
            //MyRandom rnd = new MyRandom();
            MyRandom rnd = new MyRandom(new Random());
            // XJB DEBUG END

            int count = m_tilesWorld.Count();

            // try to add one seed tile to random tile in TilesWorld and perform health check
            for (int i = 0; i < 1000; i++)
            {
                // select randomly TileName => object we will be addiding
                int rndSeedTilePos = rnd.Next(objectNames.Length);
                string rndSeedTileName = objectNames[rndSeedTilePos];

                // get randomly TileInSpace and also its random connector
                int rndTilePos = rnd.Next(count);
                TileInSpace rndTile = m_tilesWorld.ToList()[rndTilePos];
                int rndConnectorPos = rnd.Next(rndTile.Connectors.Count);
                var rndConnector = rndTile.Connectors[rndConnectorPos];
                // try Add() only if rndConnector is not used yet
                if (rndConnector.ConnectedTo == null)
                {
                    if (m_tilesWorld.Add(m_testMSystem.Tiles[rndSeedTileName], rndConnector))
                    {
                        // XJB DEBUG BEGIN
                        rndNumbers += rndSeedTilePos + "," + rndTilePos + "," + rndConnectorPos + "\n";
                        // XJB DEBUG END

                        Assert.AreEqual(++count, m_tilesWorld.Count());
                        PerformHealthCheck();
                    }
                    else
                        Assert.AreEqual(count, m_tilesWorld.Count());
                }
            }
            // XJB DEBUG - mathSage string is there to follow what happened
            string mathSage = DumpTilesWorldToSageMath();
        }

        [TestMethod]
        public void TestBulkAddAndRemoveHEAVY()
        {
            // XJB DEBUG - line below is commented because it serves for heavy test of Add() and Remove() methods and it takes time
            //for (int i = 0; i < 100; i++)
            {
                TestBulkAddAndRemove();
                Initialize();
            }
        }

        #region Private methods

        private void PerformHealthCheck()
        {
            // get m_tilesWorld into MatSage format for review should it fail
            string mathSage = DumpTilesWorldToSageMath();

            // perform health check for every tile in collection
            List<TileInSpace> tilesList = m_tilesWorld.ToList();
            tilesList.ForEach(tile =>
            {
                foreach (var checkedTile in tilesList)
                {
                    PerformHealthCheck(tile, checkedTile);
                }
            });
        }

        private void PerformHealthCheck(TileInSpace tile, TileInSpace checkedTile)
        {
            // do not health check the same object against itself
            if (tile.Equals(checkedTile)) return;

            // IDs must not be the same    
            Assert.AreNotEqual(tile.ID, checkedTile.ID);
            // objects must not overlap
            //Assert.IsFalse(tile.OverlapsWith(checkedTile));
            // XJB DEBUG
            if (tile.OverlapsWith(checkedTile))
            {
                // this shall never happen => get debug strings
                string polyI = DumpTileToMathSage(tile, m_colors[0]);
                string polyJ = DumpTileToMathSage(checkedTile, m_colors[1]);
                string sage = polyI + polyJ + "p" + tile.ID + "+p" + checkedTile.ID;
                Assert.IsFalse(true);
            }
            // objects must not intersect
            //Assert.IsFalse(tile.IntersectsWith(checkedTile));
            if (tile.IntersectsWith(checkedTile))
            {
                // this shall never happen => get debug strings
                string polyI = DumpTileToMathSage(tile, m_colors[0]);
                string polyJ = DumpTileToMathSage(checkedTile, m_colors[1]);
                string sage = polyI + polyJ + "p" + tile.ID + "+p" + checkedTile.ID;

                // Petr DEBUG
/*                tile.Move(-tile.xpushingVector);
                tile.PushingVector = tile.xpushingVector;
                checkedTile.Move(-checkedTile.xpushingVector);
                checkedTile.PushingVector = checkedTile.xpushingVector;

                UnitVector3D pushingDirection = TileInSpace.PushingDirection(xConnector.ConnectedTo);
                if (tile == m_tilesWorld.Last())
                    tile.PushingIntersected(checkedTile, pushingDirection);
                else if (checkedTile == m_tilesWorld.Last())
                    checkedTile.PushingIntersected(tile, pushingDirection);
                else if (tile.xpushingVector.Length > checkedTile.xpushingVector.Length)
                    tile.PushingNonIntersected(checkedTile);
                else
                    checkedTile.PushingNonIntersected(tile);
*/
                Assert.IsFalse(true);
            }
        }

        // dump all polygon and line objects into MathSage format
        private string DumpTilesWorldToSageMath()
        {
            int color = 0;
            string result = "";
            string additionLine = "";

            // walk through all objects
            List<TileInSpace> objectsList = m_tilesWorld.ToList();
            foreach (var tile in objectsList)
            {
                color = color == m_colors.Length ? 0 : color;
                result += DumpTileToMathSage(tile, m_colors[color++]);
                if (objectsList.First() != tile) additionLine += "+";
                additionLine += "p" + tile.ID;
            }
            result += additionLine;
            return result;
        }

        // dump one tile into MathSage format
        private string DumpTileToMathSage(TileInSpace tile, string color)
        {
            string result = "p" + tile.ID + (tile.Vertices.Count == 2 ? " = line([" : " = polygon([");

            foreach (var vertex in tile.Vertices)
            {
                if (tile.Vertices.First() != vertex) result += ",";
                result += "(" + vertex.X + "," + vertex.Y + "," + vertex.Z + ")";
            }
            result += "], color = '" + color + "');";
            return result;
        }

        #endregion
    }
}