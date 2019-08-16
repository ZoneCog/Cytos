using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Spatial.Euclidean;

namespace Cytos_v2_Tests.Classes
{
    [TestClass]
    public class PlaneTest
    {
        // IMPORTANT
        // XJB: Method Plane.Project(Point3D p, UnitVector3D projectionDirection) does not work correctly if you use
        // second parameter and use arbitrary projectionDirection vector

        /*
         * Maple 2016 calculations:
         * - play with p1,p2, p3 => define plane
         * - projectedPoint => the point we are projecting
         * - resultPoint is projected projectPoint on the plane in the direction of normal vector of the plane 
         * 
        with(geom3d);
        point(p1, [0, 0, 0]); coordinates(p1);
        point(p2, [0, 0, 1]); coordinates(p2);
        point(p3, [1, 0, 0]); coordinates(p3);
        plane(myPlane, [p1, p2, p3], [x, y, z]); detail(myPlane);
        point(projectedPoint, [3, 0, 3]); coordinates(projectedPoint);
        line(myLine, [projectedPoint, myPlane], t); detail(myLine);
        intersection(resultPoint, myLine, myPlane); coordinates(resultPoint);
        */

        // Tolerance I get with Maple
        private double MapleTolerance = 1E-8;

        // Basic tests for Plane.Project(Point3D p, UnitVector3D)
        // The plane is x-z plane with y=0 and tested point is (3,3,3) in default and various other projections
        [TestMethod]
        public void TestProjectBasic()
        {
            // xz-plane
            Point3D p1 = new Point3D(0, 0, 0);
            Point3D p2 = new Point3D(0, 0, 1);
            Point3D p3 = new Point3D(1, 0, 0);
            Plane myPlane = new Plane(p1, p2, p3);

            // get projection in the direction of plane's normal vector
            Point3D projectedPoint = new Point3D(3, 3, 3);
            Point3D resultPoint = myPlane.Project(projectedPoint);
            Assert.AreEqual(new Point3D(3, 0, 3), resultPoint);

            // place point below the plane
            projectedPoint = new Point3D(3, -3, 3);
            resultPoint = myPlane.Project(projectedPoint);
            Assert.AreEqual(new Point3D(3, 0, 3), resultPoint);

            // now use point on plane and see what happens
            projectedPoint = new Point3D(3, 0, 3);
            resultPoint = myPlane.Project(projectedPoint);
            Assert.AreEqual(new Point3D(3, 0, 3), resultPoint);
        }

        // based on TestProjectBasic() but more obsucre coordinates are used
        [TestMethod]
        public void TestProjectComplex()
        {
            // case 1
            Point3D p1 = new Point3D(3, 15, -6);
            Point3D p2 = new Point3D(1, 3, 4);
            Point3D p3 = new Point3D(13, 9, 7);
            Plane myPlane = new Plane(p1, p2, p3);

            Point3D projectedPoint = new Point3D(4, -8, 12);
            Point3D resultPoint = myPlane.Project(projectedPoint);
            Point3D expectPoint = new Point3D(3076.0/1181, -7285.0/1181, 16438.0/1181);
            Assert.IsTrue(expectPoint.Equals(resultPoint, MapleTolerance));

            // case 2
            p1 = new Point3D(6.32, -.34, -4.99);
            p2 = new Point3D(3.14, -6.28, 2.71);
            p3 = new Point3D(12.89, -56.33, 98.34);
            myPlane = new Plane(p1, p2, p3);

            projectedPoint = new Point3D(15.34, 14.36, -56.76);
            resultPoint = myPlane.Project(projectedPoint);
            expectPoint = new Point3D(9.384840357, 26.72232629, -49.68275189);
            Assert.IsTrue(expectPoint.Equals(resultPoint, MapleTolerance));

            // case 3
            p1 = new Point3D(-45.786, -2.091, 67.233);
            p2 = new Point3D(93.143, -26.288, 28.751);
            p3 = new Point3D(-101.089, 56.393, 89.344);
            myPlane = new Plane(p1, p2, p3);

            projectedPoint = new Point3D(34.354, -34.346, -0.1e-2);
            resultPoint = myPlane.Project(projectedPoint);
            expectPoint = new Point3D(44.26955989, -39.80031099, 39.22608754);
            Assert.IsTrue(expectPoint.Equals(resultPoint, MapleTolerance));
        }
    }
}
