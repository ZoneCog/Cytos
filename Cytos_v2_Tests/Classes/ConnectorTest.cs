using System.Collections.Generic;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSystemSimulationEngine.Classes;

namespace Cytos_v2_Tests.Classes
{
    [TestClass]
    public class ConnectorTest
    {
        // Test box for all tests, it is never changed.
        private static Connector v_TestConnector;
        private static List<Point3D> v_Points;
        private static Glue v_TestGlue;

        [TestInitialize]
        public void Initialize()
        {
            v_Points = new List<Point3D> {new Point3D(1, 0, 0), new Point3D(0, 1, 0)};
            v_TestGlue = new Glue("TestGlue");
            v_TestConnector = new Connector("TestConnector", v_Points, v_TestGlue, default(Angle), 0);
        }

        [TestMethod]
        public void ConnectorConstructorTest()
        {
            Assert.AreEqual("TestConnector", v_TestConnector.Name);
            Assert.AreEqual(v_TestGlue, v_TestConnector.Glue);
            Assert.AreEqual(0, v_TestConnector.Angle.Degrees);
            Assert.AreEqual(v_Points.Count, v_TestConnector.Positions.Count);
            if (v_Points.Count == v_TestConnector.Positions.Count)
                for (int i=0; i < v_Points.Count; i++)
                    Assert.AreEqual(v_Points[i], v_TestConnector.Positions[i]);

        }

/*        [TestMethod] TODO create test class for Connector OnTileIn Space, move the test method there
        public void TestIsCloseTo()
        {
            Connector anotherConnector1 = new Connector("AnotherTestConnector", new List<Point3D> { new Point3D(1, 0, 0), new Point3D(0, 1, 0) }, v_TestGlue, default(Angle), 0);
            Connector anotherConnector2 = new Connector("AnotherTestConnector", new List<Point3D> { new Point3D(1, 0, 0), new Point3D(0, 2, 0) }, v_TestGlue, default(Angle), 0);
            Connector anotherConnector3 = new Connector("AnotherTestConnector", new List<Point3D> { new Point3D(1+1E-11, 0, 0), new Point3D(0, 1, 0) }, v_TestGlue, default(Angle), 0);
            Assert.IsTrue(v_TestConnector.Overlaps(anotherConnector1, MSystem.Tolerance));
            Assert.IsFalse(v_TestConnector.Overlaps(anotherConnector2, MSystem.Tolerance));
            Assert.IsTrue(v_TestConnector.Overlaps(anotherConnector3, MSystem.Tolerance));
        } */
    }
}
