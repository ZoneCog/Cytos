using System.Collections.Generic;
using Cytos_v2.Classes;
using MathNet.Spatial.Euclidean;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cytos_v2_Tests.Classes
{
    [TestClass]
    public class Angle3DTest
    {
        // Test box for all tests, it is never changed.
        private static Angle3D v_TestAngle;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            v_TestAngle = new Angle3D(0, 1, 2);

            //Initialize class before tests are run. It will be called only once.
            //(Use eg. for loading resources - files, pictures, etc. which are not changed during tests run)
        }

        [ClassCleanup]
        public static void CleanUpClass()
        {
            v_TestAngle = null;

            //Clean up class objects after all tests are run. It will be called only once. 
            //(Use eg. for destroying loaded resources - files, pictures, etc. which are not changed during tests run)
        }

        [TestMethod]
        public void TestConstructor()
        {
            Assert.AreEqual(v_TestAngle.X, 0);
            Assert.AreEqual(v_TestAngle.Y, 1);
            Assert.AreEqual(v_TestAngle.Z, 2);
        }
    }
}
