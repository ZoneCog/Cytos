using MathNet.Spatial.Euclidean;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSystemSimulationEngine.Classes;

namespace Cytos_v2_Tests.Classes
{
    [TestClass]
    public class Segment3DTest
    {
        
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void TestConstructor()
        {
            var testSegment = new Segment3D(new Point3D(0,0,0), new Point3D(1,1,1), "TestSegment1");
            Assert.AreEqual("TestSegment1", testSegment.Name);
            Assert.AreEqual(new Vector3D(1, 1, 1), testSegment.Vector);
            Assert.AreEqual(new Vector3D(1, 1, 1).Orthogonal, testSegment.Normal);

            const double tinyDiff = MSystem.Tolerance/10;
            new Segment3D(new Point3D(0, 0, 0), new Point3D(tinyDiff, 0, 0), "TestSegment2");
            // Exception should occur here
        }

        [TestMethod]
        public void TestPushingOf()
        {
            var testSegment3 = new Segment3D(new Point3D(0, 0, 5), new Point3D(0, 0, 10), "TestSegment3");
            var testSegment4 = new Segment3D(new Point3D(0, -5, 2), new Point3D(0, 5, 4), "TestSegment4");
            var testSegment5 = new Segment3D(new Point3D(0, -10, 2), new Point3D(0, 0, 4), "TestSegment5");
            var testPolygon1 = new Polygon3D(new[] 
                {new Point3D(-5,-5,0), new Point3D(-5, 5, 0), new Point3D(5, 5, 0), new Point3D(5, -5, 0) }, "TestPolygon1");
            Assert.AreEqual(default(Vector3D), testSegment3.PushingOf(testSegment4, new Vector3D(0,0,-5)));
            Assert.AreEqual(new Vector3D(0,0,-5), testSegment3.PushingOf(testPolygon1, new Vector3D(0, 0, -10)));
            Assert.AreEqual(new Vector3D(0,0,-8), testSegment4.PushingOf(testPolygon1, new Vector3D(0, 0, -10)));
            Assert.AreEqual(new Vector3D(0,0,-7), testSegment5.PushingOf(testPolygon1, new Vector3D(0, 0, -10)));
        }

    }
}