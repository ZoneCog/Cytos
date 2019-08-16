using System.Collections.Generic;
using MathNet.Spatial.Euclidean;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSystemSimulationEngine.Classes;
using MSystemSimulationEngine.Classes.Tools;

namespace Cytos_v2_Tests.Classes
{
    [TestClass]
    public class Box3DTest
    {
        // Test box for all tests, it is never changed.
        private static Box3D v_TestBox;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            //Initialize class before tests are run. It will be called only once.
            //(Use eg. for loading resources - files, pictures, etc. which are not changed during tests run)
        }

        [TestInitialize]
        public void Initialize()
        {
            v_TestBox = new Box3D(new List<Point3D> { new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1) });
            //Initialize object/s before each test is run. It will be called for each test.
            //(You can create test object within test body or use it here, if you use them in each test)
        }

        [TestCleanup]
        public void CleanUp()
        {
            //Clean up object/s after each test is run. It will be called for each test. 
            //(Use eg. for IDisposable objects or objects which state is changes during test run)
        }

        
        [TestMethod]
        public void TestConstructor()
        {
            Assert.AreEqual(new Point3D(0, 0, 0), v_TestBox.MinCorner);
            Assert.AreEqual(new Point3D(1, 1, 1), v_TestBox.MaxCorner);
        }

        [TestMethod]
        public void TestVolume()
        {
            Assert.AreEqual(1, v_TestBox.Volume);
        }

        [TestMethod]
        public void TestRandom()
        {
            for (int i = 0; i < 10; i++)
            {
                Point3D randomPoint = v_TestBox.RandomPoint();
                Assert.IsTrue(v_TestBox.MinCorner.IsLeq(randomPoint));
                Assert.IsTrue(randomPoint.IsLeq(v_TestBox.MaxCorner));
            }
        }

        [TestMethod]
        public void TestExpand()
        {
            Box3D box2 = new Box3D(new Point3D(2, 2, 2), new Point3D(3, 3, 3));
            v_TestBox = v_TestBox.ExpandWith(box2);
            Assert.AreEqual(new Point3D(0, 0, 0), v_TestBox.MinCorner);
            Assert.AreEqual(new Point3D(3, 3, 3), v_TestBox.MaxCorner);
        }

        [TestMethod]
        public void TestContains()
        {
            Assert.IsTrue(v_TestBox.Contains(new Point3D(0,0,0)));
            Assert.IsTrue(v_TestBox.Contains(new Point3D(0.5, 0.5, 0.5)));
            Assert.IsTrue(v_TestBox.Contains(new Point3D(1, 1, 1)));
            Assert.IsFalse(v_TestBox.Contains(new Point3D(0.5, 0.5, 1.5)));
        }
    }
}