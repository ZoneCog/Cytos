using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MathNet.Spatial.Euclidean;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSystemSimulationEngine.Classes;
using MSystemSimulationEngine.Classes.Tools;
using MSystemSimulationEngine.Classes.Xml;

namespace Cytos_v2_Tests.Classes
{
    /// <summary>
    /// SOME OF THESE TESTS ARE BASED ON AVERAGE VALUES OF RANDOM VARIABLES.
    /// IF ANY TEST FAILS, PLEASE TRY AGAIN.
    /// </summary>
    [TestClass]
    public class FloatingObjectsWorldTest
    {
        private static MSystem _testMSystem;
        private static TilesWorld _testTilesWorld;
        private static FloatingObjectsWorld _testFltObjectsWorld;
        private static readonly NamedMultiset v_InfiniteMultiset = new NamedMultiset( new Dictionary<string, int>
        {
            {"a", NamedMultiset.Infinity},
            {"b", NamedMultiset.Infinity},
            {"c", NamedMultiset.Infinity}
        });

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            string path = @"../../Classes/Xml/TestXML/testMSystemDescription.xml";
            XDocument mSystemDescription = XDocument.Load(path);
            var mSystemObjects = new DeserializedObjects(mSystemDescription, path);
            _testMSystem = new MSystem(mSystemObjects);
        }

        [TestInitialize]
        //Initialize object/s before each test is run. It will be called for each test.
        public void Initialize()
        {
            _testTilesWorld = new TilesWorld(_testMSystem);
            // Leaving only tile q1 in the TilesWorld
            _testTilesWorld.Remove(_testTilesWorld.ToArray()[1]);
            _testTilesWorld.Remove(_testTilesWorld.ToArray()[1]);
            _testFltObjectsWorld = new FloatingObjectsWorld(_testMSystem, _testTilesWorld);
            _testTilesWorld.FltObjectsWorld = _testFltObjectsWorld;
        }

        [TestCleanup]
        public void CleanUp()
        {
            //Clean up object/s after each test is run. It will be called for each test. 
            //(Use eg. for IDisposable objects or objects which state is changes during test run)
        }


        [TestMethod]
        public void ConstructorTest()
        {
            Assert.AreEqual(new Point3D(-15.57056516295, -14.150169943749, -6.06).Round(10), 
                _testFltObjectsWorld.GetBox.MinCorner.Round(10));
            Assert.AreEqual(new Point3D(15.5705651629515, 16.06, 6.06).Round(10), 
                _testFltObjectsWorld.GetBox.MaxCorner.Round(10));
        }

        [TestMethod]
        public void ExpandTest()
        {
            _testFltObjectsWorld.ExpandWith(new Box3D(new Point3D(-15,-15,0), new Point3D(15,15,1)));
            Assert.IsTrue(new Point3D(-21.06, -21.06, -6.06).Equals(_testFltObjectsWorld.GetBox.MinCorner, 1e-13));
            Assert.IsTrue(new Point3D(21.06, 21.06, 7.06).Equals(_testFltObjectsWorld.GetBox.MaxCorner, 1e-13));
        }

        [TestMethod]
        public void ToSetTest()
        {
            var testSet = _testFltObjectsWorld.ToSet(); 
            Assert.AreEqual(_testFltObjectsWorld.GetBox.Volume * 2, testSet.Count, 100);
        }

        [TestMethod]
        public void FinalizeStepTest()
        {
            var testSet1 = _testFltObjectsWorld.ToSet();
            var initPositions = new Dictionary<FloatingObjectInSpace, Point3D>();
            foreach (var obj in testSet1)
                initPositions[obj] = obj.Position;

            _testFltObjectsWorld.FinalizeStep();

            foreach (var objType in _testMSystem.FloatingObjects.Values)
            {
                var singleTypeSet = testSet1.Where(obj => obj.Type == objType);
                if (singleTypeSet.Any())
                {
                    double averageMove = singleTypeSet.Average(obj => obj.Position.DistanceTo(initPositions[obj]));
                    Assert.AreEqual(objType.Mobility / (objType.Concentration > 0 ? 2 : 1), averageMove,  0.1 * objType.Mobility);
                }
            }

            //int[] results = new int[10]; // DEBUGGING
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(_testFltObjectsWorld.GetBox.Volume * 2, _testFltObjectsWorld.ToSet().Count, 20);
                _testFltObjectsWorld.FinalizeStep();
            }
//            Assert.AreEqual("Result=", string.Join(";", results)); // DEBUGGING
        }

        [TestMethod]
        public void GetObjectstNearTileTest()
        {
            var testTile = _testTilesWorld.First();

            const int count = 20;
            int[] results = new int[count];
            for (var i = 0; i < count; i++)
            {
                results[i] = _testFltObjectsWorld.GetNearObjects(testTile, v_InfiniteMultiset).Count;
                _testFltObjectsWorld.FinalizeStep();
            }
            Assert.AreEqual(5300, results.Sum(), 1000);
            //Assert.AreEqual("Result=", string.Join(";", results)); // DEBUGGING
        }

        [TestMethod]
        public void GetObjectsNearProteinTest()
        {
            TileInSpace.ResetCounter();
            FloatingObjectInSpace.ResetCounter();

            var testTile = _testTilesWorld.First();
            var testProtein = testTile.Proteins.First();
            const int count = 20;
            int[] results = new int[count];
            for (var i = 0; i < count; i++)
            {
                results[i] =_testFltObjectsWorld.GetNearObjects(testProtein, Tile.SideType.outside, v_InfiniteMultiset).Count;
                _testFltObjectsWorld.FinalizeStep();
            }
            Assert.AreEqual(2500, results.Sum(), 500);

            for (var i = 0; i < count; i++)
            {
                results[i] = _testFltObjectsWorld.GetNearObjects(testProtein, Tile.SideType.inside, v_InfiniteMultiset).Count;
                _testFltObjectsWorld.FinalizeStep();
            }
            Assert.AreEqual(2400, results.Sum(), 500);
            //Assert.AreEqual("Result=", string.Join(";", results)); // DEBUGGING
        }

        [TestMethod]
        public void GetObjectsNearConnectorTest1()
        {
            var testTile = _testTilesWorld.First();
            const int count = 20;
            int[] results = new int[count];
            for (var i = 0; i < count; i++)
            {
                results[i] = _testFltObjectsWorld.GetNearObjects(testTile.Connectors.First(), v_InfiniteMultiset).Count;
                _testFltObjectsWorld.FinalizeStep();
            }

            Assert.AreEqual(12000, results.Sum(), 1000); 
            //Assert.AreEqual("Result=", string.Join(";", results)); // DEBUGGING
        }

        [TestMethod]
        public void GetObjectsNearConnectorTest2()
        {
            var testTile = _testTilesWorld.First();
            // Construct a lower part of a dodecahedron (1+5 tiles)
            Assert.IsTrue(_testTilesWorld.Add(_testMSystem.Tiles["q1"], testTile.Connectors[0]));
            Assert.IsTrue(_testTilesWorld.Add(_testMSystem.Tiles["q1"], testTile.Connectors[1]));
            Assert.IsTrue(_testTilesWorld.Add(_testMSystem.Tiles["q1"], testTile.Connectors[2]));
            Assert.IsTrue(_testTilesWorld.Add(_testMSystem.Tiles["q1"], testTile.Connectors[3]));
            Assert.IsTrue(_testTilesWorld.Add(_testMSystem.Tiles["q1"], testTile.Connectors[4]));
            //Assert.IsTrue(_testTilesWorld.Add(_testMSystem.Tiles["q1"], _testTilesWorld.ToArray()[1].Connectors[3]));

            var aBox = new Box3D(new Point3D(-5.87, -8.09, 0.01), new Point3D(-5.37, -7.59, 0.5));
            var aSet = new NamedMultiset(new[] { _testMSystem.FloatingObjects["a"] });
            for (int i = 0; i < 100; i++)
                _testFltObjectsWorld.AddAt(aSet, aBox.RandomPoint());

            var cBox = new Box3D(new Point3D(-5.87, -8.09, -0.5), new Point3D(-5.37, -7.59, -0.01));
            var cSet = new NamedMultiset(new[] { _testMSystem.FloatingObjects["c"] });
            for (int i = 0; i < 100; i++)
                _testFltObjectsWorld.AddAt(cSet, cBox.RandomPoint());

            _testFltObjectsWorld.FinalizeStep(true);

            for (int i = 2; i < 4; i++)
            {
                var result = _testFltObjectsWorld.GetNearObjects(testTile.Connectors[i], v_InfiniteMultiset);
                Assert.AreEqual(100, result.Count(obj => obj.Name == "a"));
                Assert.AreEqual(102, result.Count(obj => obj.Name == "c"));
                // TODO low priority t2 extra objects were created during ADD of tiles but generally they need not be there
                // add different floating objects to the test xml file and use them instead of "a","c"
            }
        }

        [TestMethod]
        public void GetObjectsInPrismTest()
        {
            var testPolygon = _testMSystem.Tiles.First().Value.Vertices as Polygon3D;
            const int count = 20;
            int[] results = new int[count];
            for (var i = 0; i < count; i++)
            {
                results[i] = _testFltObjectsWorld.GetObjectsInPrism(testPolygon, new Vector3D(0,0,5)).Count;
                _testFltObjectsWorld.FinalizeStep();
            }
            Assert.AreEqual(50195, results.Sum(), 500); //TODO how big should be delta?
            //Assert.AreEqual("Result=", string.Join(";", results)); // DEBUGGING
        }

        /// <summary>
        /// To Remove objects, we must first Add them to the world.
        /// Hence we test both methods simultaneously.
        /// </summary>
        [TestMethod]
        public void AddAndRemoveTest()
        {
            var aObject = _testMSystem.FloatingObjects["a"];
            _testFltObjectsWorld.AddAt(new NamedMultiset(new[] {aObject, aObject}), new Point3D(0, 0, 0.5));
            _testFltObjectsWorld.FinalizeStep(true);

            var detectedObjects = _testFltObjectsWorld.GetNearObjects(_testTilesWorld.First(), v_InfiniteMultiset);
            Assert.AreEqual(2, detectedObjects.Count(obj => obj.Name == "a"));

            _testFltObjectsWorld.RemoveFrom(new NamedMultiset(new[] {aObject}), detectedObjects);
            detectedObjects = _testFltObjectsWorld.GetNearObjects(_testTilesWorld.First(), v_InfiniteMultiset);
            Assert.AreEqual(1, detectedObjects.Count(obj => obj.Name == "a"));

            _testFltObjectsWorld.RemoveFrom(new NamedMultiset(new[] { aObject }), detectedObjects);
            detectedObjects = _testFltObjectsWorld.GetNearObjects(_testTilesWorld.First(), v_InfiniteMultiset);
            Assert.AreEqual(0, detectedObjects.Count(obj => obj.Name == "a"));
        }

        /// <summary>
        /// An attempt to remove an object which is not in the world must throw an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void RemoveExceptionTest()
        {
            var aObject = _testMSystem.FloatingObjects["a"];
            var detectedObjects = _testFltObjectsWorld.GetNearObjects(_testTilesWorld.First(), v_InfiniteMultiset);
            _testFltObjectsWorld.RemoveFrom(new NamedMultiset(new[] { aObject }), detectedObjects);
        }

    }
}
