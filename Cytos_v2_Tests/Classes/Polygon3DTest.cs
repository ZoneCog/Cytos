using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MathNet.Spatial.Euclidean;
using Cytos_v2_Tests.Classes.Tools;
using MSystemSimulationEngine.Classes;
using MSystemSimulationEngine.Classes.Tools;

namespace Cytos_v2_Tests.Classes
{
    [TestClass]
    public class Polygon3DTest
    {
        #region Private members

        // often used points to construct objects in space are defined here

        // triangle in xy-plane
        private static Point3D t1_1 = new Point3D(0, 0, 0);
        private static Point3D t1_2 = new Point3D(3, 0, 0);
        private static Point3D t1_3 = new Point3D(0, -3, 0);

        // square in xy-plane
        private static Point3D s1_1 = new Point3D(0, 0, 0);
        private static Point3D s1_2 = new Point3D(3, 0, 0);
        private static Point3D s1_3 = new Point3D(3, 3, 0);
        private static Point3D s1_4 = new Point3D(0, 3, 0);

        // unit vectors
        UnitVector3D uX = new UnitVector3D(1, 0, 0);
        UnitVector3D uY = new UnitVector3D(0, 1, 0);
        UnitVector3D uZ = new UnitVector3D(0, 0, 1);

        #endregion

        #region Public methods

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestConstructorNullVertices()
        {
            Polygon3D p = new Polygon3D(null, "p");
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentException), "Polygon p has 0 vertices, must have >= 3")]
        public void TestConstructorTooLittleVerticesNone()
        {   // empty list
            Polygon3D p = new Polygon3D(new List<Point3D> { }, "p");
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentException), "Polygon p has 1 vertices, must have >= 3")]
        public void TestConstructorTooLittleVerticesOne()
        {   // one point in the list
            Polygon3D p = new Polygon3D(new List<Point3D> { t1_1 }, "p");
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(ArgumentException), "Polygon p has 2 vertices, must have >= 3")]
        public void TestConstructorTooLittleVerticeTwo()
        {   // two points in the list
            Polygon3D p = new Polygon3D(new List<Point3D> { t1_1, t1_2 }, "p");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestConstructorObjectNameNull()
        {
            Polygon3D p1 = new Polygon3D(new List<Point3D> { t1_1, t1_2, t1_3 }, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorObjectNameEmpty()
        {
            Polygon3D p2 = new Polygon3D(new List<Point3D> { t1_1, t1_2, t1_3 }, "");
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorPolygonNotConvex()
        {
            // this polygon is not convex
            Point3D p1 = new Point3D(0, 0, 0);
            Point3D p2 = new Point3D(4, 0, 0);
            Point3D p3 = new Point3D(4, 4, 0);
            Point3D p4 = new Point3D(2, 2, 0); // this point is a problem
            Point3D p5 = new Point3D(0, 4, 0);

            Polygon3D p = new Polygon3D(new List<Point3D> { p1, p2, p3, p4, p5 }, "p");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructorSillyPolygons()
        {
            // build square but mix the order of the points
            Polygon3D p = new Polygon3D(new List<Point3D> { s1_1, s1_3, s1_2, s1_4 }, "p");
        }

        [TestMethod]
        public void TestIntersectsWithLine()
        {
            // polygons to intersect with
            Polygon3D t = new Polygon3D(new List<Point3D> { t1_1, t1_2, t1_3 }, "t");
            Polygon3D s = new Polygon3D(new List<Point3D> { s1_1, s1_2, s1_3, s1_4 }, "s");

            // -- 
            // line in the same plane as polygons
            // --

            // line going through polygons in the same plane
            Point3D start = new Point3D(1, 4, 0);
            Point3D end   = new Point3D(1, -3, 0);
            Assert.AreEqual(t.IntersectionWith(start, end).IsNaN(), true);
            Assert.AreEqual(s.IntersectionWith(start, end).IsNaN(), true);
            Assert.AreEqual(t.IntersectsWith(start, end, 0, true), false);
            Assert.AreEqual(t.IntersectsWith(start, end, 0, false), false);
            Assert.AreEqual(s.IntersectsWith(start, end, 0, true), false);
            Assert.AreEqual(s.IntersectsWith(start, end, 0, false), false);

            // line not going through the polygons in the same plane
            start = new Point3D(4, 4, 0);
            end   = new Point3D(4, -3, 0);
            Assert.AreEqual(t.IntersectionWith(start, end).IsNaN(), true);
            Assert.AreEqual(s.IntersectionWith(start, end).IsNaN(), true);
            Assert.AreEqual(t.IntersectsWith(start, end, 0, true), false);
            Assert.AreEqual(t.IntersectsWith(start, end, 0, false), false);
            Assert.AreEqual(s.IntersectsWith(start, end, 0, true), false);
            Assert.AreEqual(s.IntersectsWith(start, end, 0, false), false);

            // line starting in polygon going out in the same plane
            start = new Point3D(2,    2, 0);
            end =   new Point3D(2, -0.5, 0);
            Assert.AreEqual(t.IntersectionWith(start, end).IsNaN(), true);
            Assert.AreEqual(s.IntersectionWith(start, end).IsNaN(), true);
            Assert.AreEqual(t.IntersectsWith(start, end, 0, true), false);
            Assert.AreEqual(t.IntersectsWith(start, end, 0, false), false);
            Assert.AreEqual(s.IntersectsWith(start, end, 0, true), false);
            Assert.AreEqual(s.IntersectsWith(start, end, 0, false), false);

            // line inside square in the same plane
            start = new Point3D(0.5, 0.5, 0);
            end =   new Point3D(  2, 0.5, 0);
            Assert.AreEqual(s.IntersectionWith(start, end).IsNaN(), true);
            Assert.AreEqual(s.IntersectsWith(start, end, 0, true), false);
            Assert.AreEqual(s.IntersectsWith(start, end, 0, false), false);

            // line inside triangle in the same plane
            start = new Point3D(2, 2, 0);
            end =   new Point3D(2, -0.5, 0);
            Assert.AreEqual(t.IntersectionWith(start, end).IsNaN(), true);
            Assert.AreEqual(t.IntersectsWith(start, end, 0, true), false);
            Assert.AreEqual(t.IntersectsWith(start, end, 0, false), false);

            // line = border of square in the same plane
            start = s1_1;
            end = s1_2;
            Assert.AreEqual(s.IntersectionWith(start, end).IsNaN(), true);
            Assert.AreEqual(s.IntersectsWith(start, end, 0, true), false);
            Assert.AreEqual(s.IntersectsWith(start, end, 0, false), false);

            // line = border of triangle in the same plane
            start = t1_1;
            end = t1_2;
            Assert.AreEqual(t.IntersectionWith(start, end).IsNaN(), true);
            Assert.AreEqual(t.IntersectsWith(start, end, 0, true), false);
            Assert.AreEqual(t.IntersectsWith(start, end, 0, false), false);

            // --
            // now lines which are not in plane of the polygons in fact all lines in z-axis direction
            // --

            // three cases of line intersecting square at s1_4 (0, 3, 0)
            start = new Point3D(0, 3, -2);
            end   = new Point3D(0, 3,  2);
            Assert.AreEqual(s.IntersectionWith(start, end), s1_4);
            Assert.IsTrue(s.IntersectsWith(start, end, 0, true));
            Assert.IsTrue(s.IntersectsWith(start, end, 0, false));

            start = new Point3D(0, 3, -2);
            end   = new Point3D(0, 3,  0);
            Assert.AreEqual(s.IntersectionWith(start, end), s1_4);
            Assert.IsTrue(s.IntersectsWith(start, end, 0, true));
            Assert.IsFalse(s.IntersectsWith(start, end, 0, false));

            start = new Point3D(0, 3, -2);
            end   = new Point3D(0, 3, -1);
            Assert.IsTrue(s.IntersectionWith(start, end).IsNaN());
            Assert.IsFalse(s.IntersectsWith(start, end, 0, true));
            Assert.IsFalse(s.IntersectsWith(start, end, 0, false));

            // three cases of line intersecting square at (1, 4, 0) - no intersection
            start = new Point3D(1, 4, -2);
            end   = new Point3D(1, 4,  2);
            Assert.IsTrue(s.IntersectionWith(start, end).IsNaN());
            Assert.IsFalse(s.IntersectsWith(start, end, 0, true));
            Assert.IsFalse(s.IntersectsWith(start, end, 0, false));

            start = new Point3D(1, 4, -2);
            end   = new Point3D(1, 4,  0);
            Assert.IsTrue(s.IntersectionWith(start, end).IsNaN());
            Assert.IsFalse(s.IntersectsWith(start, end, 0, true));
            Assert.IsFalse(s.IntersectsWith(start, end, 0, false));

            start = new Point3D(1, 4, -2);
            end   = new Point3D(1, 4, -1);
            Assert.IsTrue(s.IntersectionWith(start, end).IsNaN());
            Assert.IsFalse(s.IntersectsWith(start, end, 0, true));
            Assert.IsFalse(s.IntersectsWith(start, end, 0, false));

            // three cases of line intersecting square on the edge (1, 3, 0)
            Point3D isec = new Point3D(1, 3, 0);
            start = new Point3D(1, 3, -2);
            end   = new Point3D(1, 3,  2);
            Assert.AreEqual(s.IntersectionWith(start, end), isec);
            Assert.IsTrue(s.IntersectsWith(start, end, 0, true));
            Assert.IsTrue(s.IntersectsWith(start, end, 0, false));

            start = new Point3D(1, 3, -2);
            end   = new Point3D(1, 3,  0);
            Assert.AreEqual(s.IntersectionWith(start, end), isec);
            Assert.IsTrue(s.IntersectsWith(start, end, 0, true));
            Assert.IsFalse(s.IntersectsWith(start, end, 0, false));

            start = new Point3D(1, 3, -2);
            end   = new Point3D(1, 3, -1);
            Assert.IsTrue(s.IntersectionWith(start, end).IsNaN());
            Assert.IsFalse(s.IntersectsWith(start, end, 0, true));
            Assert.IsFalse(s.IntersectsWith(start, end, 0, false));

            // three cases of line intersecting square insede (2, 2, 0)
            isec  = new Point3D(2, 2, 0);
            start = new Point3D(2, 2, -2);
            end   = new Point3D(2, 2,  2);
            Assert.AreEqual(s.IntersectionWith(start, end), isec);
            Assert.IsTrue(s.IntersectsWith(start, end, 0, true));
            Assert.IsTrue(s.IntersectsWith(start, end, 0, false));

            start = new Point3D(2, 2, -2);
            end   = new Point3D(2, 2,  0);
            Assert.AreEqual(s.IntersectionWith(start, end), isec);
            Assert.IsTrue(s.IntersectsWith(start, end, 0, true));
            Assert.IsFalse(s.IntersectsWith(start, end, 0, false));

            start = new Point3D(2, 2, -2);
            end   = new Point3D(2, 2, -1);
            Assert.IsTrue(s.IntersectionWith(start, end).IsNaN());
            Assert.IsFalse(s.IntersectsWith(start, end, 0, true));
            Assert.IsFalse(s.IntersectsWith(start, end, 0, false));

            // triangle not tested, assuming it will behave the same way

            // --
            // now lines paralel to the plane of the polygon
            // --
            start = new Point3D(1,  4, 1);
            end   = new Point3D(1, -3, 1);
            Assert.IsTrue(s.IntersectionWith(start, end).IsNaN());
            Assert.IsFalse(s.IntersectsWith(start, end, 0, true));
            Assert.IsFalse(s.IntersectsWith(start, end, 0, false));
        }

        [TestMethod]
        public void TestIntersectionWithPolyline()
        {
            // polygon can intersect in no more than two points with a polyline (if polyline is polygon)

            // polygon to intersect with
            Polygon3D s = new Polygon3D(new List<Point3D> { s1_1, s1_2, s1_3, s1_4 }, "s");

            // this is our base polyline which we will be moving left and up to test intersection with square
            Point3D p1 = new Point3D(1, -1, 1);
            Point3D p2 = new Point3D(1, -1, -1);
            Point3D p3 = new Point3D(2, -1, -1);
            Point3D p4 = new Point3D(2, -1, 1);
            List<Point3D> l = new List<Point3D> { p1, p2, p3, p4 };

            Polygon3D pl = new Polygon3D(l, "pl");

            // test base => no intersection
            HashSet<Point3D> res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 0);

            // test base shift left (x-1) => no intersection
            for (int i = 0; i < l.Count; i++)
                l[i] -= uX;
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 0);

            // test base shift left (x-1, 2 in total from base) => no intersection
            for (int i = 0; i < l.Count; i++)
                l[i] -= uX;
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 0);

            // test base shift left (x-1, 3 in total from base) => no intersection
            for (int i = 0; i < l.Count; i++)
                l[i] -= uX;
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 0);

            // move base back to original position and move up by one
            l[0] = p1 + uY;
            l[1] = p2 + uY;
            l[2] = p3 + uY;
            l[3] = p4 + uY;

            // test base position whereby base has moved Y+1
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 0); // intersects with two points on the edges of square but this is not counted

            // test base shift left (x-1) => two intersections on the edge of the square
            for (int i = 0; i < l.Count; i++)
                l[i] -= uX;
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 0);

            // test base shift left (x-1, 2 in total from base) => one intersection on the edge of the square
            for (int i = 0; i < l.Count; i++)
                l[i] -= uX;
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 0);

            // test base shift left (x-1, 3 in total from base) => no intersection
            for (int i = 0; i < l.Count; i++)
                l[i] -= uX;
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 0);

            // move base back to original position and move up by two
            l[0] = p1 + (2 * uY);
            l[1] = p2 + (2 * uY);
            l[2] = p3 + (2 * uY);
            l[3] = p4 + (2 * uY);

            // test base position whereby base has moved Y+2
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 2);
            // check the points
            Assert.IsTrue(res.Contains(new Point3D(2, 1, 0)));
            Assert.IsTrue(res.Contains(new Point3D(1, 1, 0)));

            // test base shift left (x-1) => two intersections (one on the edge [NOW counted] and one in the middle)
            for (int i = 0; i < l.Count; i++)
                l[i] -= uX;
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 2);
            // check the points
            Assert.IsTrue(res.Contains(new Point3D(0, 1, 0)));
            Assert.IsTrue(res.Contains(new Point3D(1, 1, 0)));
            
            // test base shift left (x-1 two in total) => one intersection one on the edge [not counted]
            for (int i = 0; i < l.Count; i++)
                l[i] -= uX;
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 0);

            // test base shift left (x-1 three in total) => no intersection
            for (int i = 0; i < l.Count; i++)
                l[i] -= uX;
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 0);
        }

        [TestMethod]
        public void TestIntersectionWithPolylineComplex()
        {
            // polygon can intersect in no more than two points with a polyline (if polyline is polygon)

            // polygon to intersect with
            Polygon3D s = new Polygon3D(new List<Point3D> { s1_1, s1_2, s1_3, s1_4 }, "s");

            // this is our base polyline which we will be moving left and up to test intersection with square
            Point3D p1 = new Point3D(-1.5,  0.5,  1);
            Point3D p2 = new Point3D(-1.5,  0.5, -1);
            Point3D p3 = new Point3D( 0.5, -1.5, -1);
            Point3D p4 = new Point3D( 0.5, -1.5,  1);
            List<Point3D> l = new List<Point3D> { p1, p2, p3, p4 };

            Polygon3D pl = new Polygon3D(l, "pl");

            // test base => no intersection
            HashSet<Point3D> res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 0);

            // test base shift left (x+0.5) and up (y+0.5) => on intersection at (0,0,0) which is not counted
            for (int i = 0; i < l.Count; i++)
            {
                l[i] += 0.5 * uX;
                l[i] += 0.5 * uY;
            }
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(0, res.Count);

            // test base shift left (x+0.5) and up (y+0.5) => intersection at (0,1,0) & (1,0,0) which is counted
            for (int i = 0; i < l.Count; i++)
            {
                l[i] += 0.5 * uX;
                l[i] += 0.5 * uY;
            }
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 2);
            // check the points
            Assert.IsTrue(res.Contains(new Point3D(0, 1, 0)));
            Assert.IsTrue(res.Contains(new Point3D(1, 0, 0)));

            // test base shift left (x+0.5) and up (y+0.5) => intersection at (0,2,0) & (2,0,0) which is counted
            for (int i = 0; i < l.Count; i++)
            {
                l[i] += 0.5 * uX;
                l[i] += 0.5 * uY;
            }
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 2); 
            // check the points
            Assert.IsTrue(res.Contains(new Point3D(0, 2, 0)));
            Assert.IsTrue(res.Contains(new Point3D(2, 0, 0)));

            // test base shift left (x+0.5) and up (y+0.5) => intersection at (0.5,2.5,0) & (2.5,0.5,0) which is counted
            for (int i = 0; i < l.Count; i++)
            {
                l[i] += 0.5 * uX;
                l[i] += 0.5 * uY;
            }
            pl = new Polygon3D(l, "pl");
            res = s.IntersectionsWith(pl);
            Assert.AreEqual(res.Count, 2);
            // check the points
            Assert.IsTrue(res.Contains(new Point3D(0.5, 2.5, 0)));
            Assert.IsTrue(res.Contains(new Point3D(2.5, 0.5, 0)));
        }

        [TestMethod]
        public void TestContainsInside()
        {
            // polygons to test with
            Polygon3D s = new Polygon3D(new List<Point3D> { s1_1, s1_2, s1_3, s1_4 }, "s");

            // parameters for the test
            double delta_1 = 3e-11; // under system tolerance
            double delta_2 = 1e-9;  // over system tolerance

            // ---
            // point on vertex of the square
            // ---
            Point3D p = new Point3D(0, 0, 0);
            Assert.IsTrue(s.ContainsPoint(p));

            // no borderTolerance
            Assert.IsTrue(s.ContainsPoint(p + (delta_1 * uX) + (delta_1 * uY)));   
            Assert.IsTrue(s.ContainsPoint(p + (delta_1 * uX) - (delta_1 * uY)));  
            Assert.IsTrue(s.ContainsPoint(p - (delta_1 * uX) - (delta_1 * uY)));
            Assert.IsTrue(s.ContainsPoint(p - (delta_1 * uX) + (delta_1 * uY)));

            // under border tolerance equal to MSystem.Tolerance (square gets smaller by MSystem.Tolerance)
            Assert.IsFalse(s.ContainsPoint(p + (delta_1 * uX) + (delta_1 * uY), MSystem.Tolerance));
            Assert.IsFalse(s.ContainsPoint(p + (delta_1 * uX) - (delta_1 * uY), MSystem.Tolerance)); 
            Assert.IsFalse(s.ContainsPoint(p - (delta_1 * uX) - (delta_1 * uY), MSystem.Tolerance));
            Assert.IsFalse(s.ContainsPoint(p - (delta_1 * uX) + (delta_1 * uY), MSystem.Tolerance));

            // over border tolerance equal to MSystem.Tolerance
            Assert.IsTrue(s.ContainsPoint(p + (delta_2 * uX) + (delta_2 * uY), MSystem.Tolerance));
            Assert.IsFalse(s.ContainsPoint(p + (delta_2 * uX) - (delta_2 * uY), MSystem.Tolerance)); 
            Assert.IsFalse(s.ContainsPoint(p - (delta_2 * uX) - (delta_2 * uY), MSystem.Tolerance));
            Assert.IsFalse(s.ContainsPoint(p - (delta_2 * uX) + (delta_2 * uY), MSystem.Tolerance));

            // ---
            // point on edge of the square
            // ---
            p = new Point3D(2, 0, 0);
            Assert.IsTrue(s.ContainsPoint(p));

            // no borderTolerance
            Assert.IsTrue(s.ContainsPoint(p + (delta_1 * uX) + (delta_1 * uY)));
            Assert.IsTrue(s.ContainsPoint(p + (delta_1 * uX) - (delta_1 * uY)));
            Assert.IsTrue(s.ContainsPoint(p - (delta_1 * uX) - (delta_1 * uY)));
            Assert.IsTrue(s.ContainsPoint(p - (delta_1 * uX) + (delta_1 * uY)));

            // under border tolerance equal to MSystem.Tolerance (square gets smaller by MSystem.Tolerance)
            Assert.IsFalse(s.ContainsPoint(p + (delta_1 * uX) + (delta_1 * uY), MSystem.Tolerance));
            Assert.IsFalse(s.ContainsPoint(p + (delta_1 * uX) - (delta_1 * uY), MSystem.Tolerance)); 
            Assert.IsFalse(s.ContainsPoint(p - (delta_1 * uX) - (delta_1 * uY), MSystem.Tolerance));
            Assert.IsFalse(s.ContainsPoint(p - (delta_1 * uX) + (delta_1 * uY), MSystem.Tolerance));

            // over border tolerance equal to MSystem.Tolerance
            Assert.IsTrue(s.ContainsPoint(p + (delta_2 * uX) + (delta_2 * uY), MSystem.Tolerance));
            Assert.IsFalse(s.ContainsPoint(p + (delta_2 * uX) - (delta_2 * uY), MSystem.Tolerance));
            Assert.IsFalse(s.ContainsPoint(p - (delta_2 * uX) - (delta_2 * uY), MSystem.Tolerance));
            Assert.IsTrue(s.ContainsPoint(p - (delta_2 * uX) + (delta_2 * uY), MSystem.Tolerance));

            // ---
            // point in the square
            // ---
            p = new Point3D(2, 2, 0);
            Assert.IsTrue(s.ContainsPoint(p));

            // no borderTolerance
            Assert.IsTrue(s.ContainsPoint(p + (delta_1 * uX) + (delta_1 * uY)));
            Assert.IsTrue(s.ContainsPoint(p + (delta_1 * uX) - (delta_1 * uY)));
            Assert.IsTrue(s.ContainsPoint(p - (delta_1 * uX) - (delta_1 * uY)));
            Assert.IsTrue(s.ContainsPoint(p - (delta_1 * uX) + (delta_1 * uY)));

            // under border tolerance equal to MSystem.Tolerance
            Assert.IsTrue(s.ContainsPoint(p + (delta_1 * uX) + (delta_1 * uY), MSystem.Tolerance));
            Assert.IsTrue(s.ContainsPoint(p + (delta_1 * uX) - (delta_1 * uY), MSystem.Tolerance)); 
            Assert.IsTrue(s.ContainsPoint(p - (delta_1 * uX) - (delta_1 * uY), MSystem.Tolerance));
            Assert.IsTrue(s.ContainsPoint(p - (delta_1 * uX) + (delta_1 * uY), MSystem.Tolerance));

            // over border tolerance equal to MSystem.Tolerance
            Assert.IsTrue(s.ContainsPoint(p + (delta_2 * uX) + (delta_2 * uY), MSystem.Tolerance));
            Assert.IsTrue(s.ContainsPoint(p + (delta_2 * uX) - (delta_2 * uY), MSystem.Tolerance));
            Assert.IsTrue(s.ContainsPoint(p - (delta_2 * uX) - (delta_2 * uY), MSystem.Tolerance));
            Assert.IsTrue(s.ContainsPoint(p - (delta_2 * uX) + (delta_2 * uY), MSystem.Tolerance));

            // ---
            // point "above" and below the square
            // ---
            p = new Point3D(0, 0, 0);
            // point above the edge or vertex is not on surfice
            Assert.IsTrue(s.ContainsPoint(p + (delta_1 * uZ)));
            Assert.IsTrue(s.ContainsPoint(p - (delta_1 * uZ)));
            Assert.IsFalse(s.ContainsPoint(p + (delta_2 * uZ)));
            Assert.IsFalse(s.ContainsPoint(p - (delta_2 * uZ)));

            p = new Point3D(2, 0, 0);
            // point above the edge or vertex is not on surfice
            Assert.IsTrue(s.ContainsPoint(p + (delta_1 * uZ)));
            Assert.IsTrue(s.ContainsPoint(p - (delta_1 * uZ)));
            Assert.IsFalse(s.ContainsPoint(p + (delta_2 * uZ)));
            Assert.IsFalse(s.ContainsPoint(p - (delta_2 * uZ)));

            p = new Point3D(2, 2, 0);
            Assert.IsTrue(s.ContainsPoint(p + (delta_1 * uZ)));
            Assert.IsTrue(s.ContainsPoint(p - (delta_1 * uZ)));
            Assert.IsFalse(s.ContainsPoint(p + (delta_2 * uZ)));
            Assert.IsFalse(s.ContainsPoint(p - (delta_2 * uZ)));
        }

        [TestMethod]
        public void TestOverlapsWith()
        {
            // polygon to intersect with
            Polygon3D s = new Polygon3D(new List<Point3D> { s1_1, s1_2, s1_3, s1_4 }, "s");

            // triangle in xy-plane which will be moving and testing overlapping
            Point3D p1 = new Point3D(0, -1, 0);
            Point3D p2 = new Point3D(3, -1, 0);
            Point3D p3 = new Point3D(0, -4, 0);

            List<Point3D> l = new List<Point3D> { p1, p2, p3 };
            Polygon3D t = new Polygon3D(l, "s");

            // polygon overlaps with itself
            Assert.IsTrue(t.OverlapsWith(t));
            Assert.IsTrue(s.OverlapsWith(s));

            // we must get the same result call the method the other way round => testing two calls
            Assert.IsFalse(t.OverlapsWith(s));
            Assert.IsFalse(s.OverlapsWith(t));

            // move up - borders touch on the edge but do not overlap
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsFalse(t.OverlapsWith(s));
            Assert.IsFalse(s.OverlapsWith(t));

            // move up 
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsTrue(t.OverlapsWith(s));
            Assert.IsTrue(s.OverlapsWith(t));

            // move up 
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsTrue(t.OverlapsWith(s));
            Assert.IsTrue(s.OverlapsWith(t));

            // move up 
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsTrue(t.OverlapsWith(s));
            Assert.IsTrue(s.OverlapsWith(t));

            // move up 
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsTrue(t.OverlapsWith(s));
            Assert.IsTrue(s.OverlapsWith(t));

            // move up 
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsTrue(t.OverlapsWith(s));
            Assert.IsTrue(s.OverlapsWith(t));

            // move up - touch on the bottom vertice of triangle => not counted
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsFalse(t.OverlapsWith(s)); 
            Assert.IsFalse(s.OverlapsWith(t));

            // move up 
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsFalse(t.OverlapsWith(s));
            Assert.IsFalse(s.OverlapsWith(t));

            // now we move triangle two units left and start moving upwards again
            l = new List<Point3D> { p1- ( 2 * uX), p2 - (2 * uX), p3 - (2 * uX) };

            Assert.IsFalse(t.OverlapsWith(s));
            Assert.IsFalse(s.OverlapsWith(t));

            // move up - borders touch on the edge but do not overlap
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsFalse(t.OverlapsWith(s));
            Assert.IsFalse(s.OverlapsWith(t));

            // move up 
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsTrue(t.OverlapsWith(s));
            Assert.IsTrue(s.OverlapsWith(t));

            // move up 
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsTrue(t.OverlapsWith(s));
            Assert.IsTrue(s.OverlapsWith(t));

            // move up 
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsTrue(t.OverlapsWith(s));
            Assert.IsTrue(s.OverlapsWith(t));

            // move up = touch in one point = false
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsFalse(t.OverlapsWith(s));
            Assert.IsFalse(s.OverlapsWith(t));

            // move up 
            for (int i = 0; i < l.Count; i++)
                l[i] += uY;
            t = new Polygon3D(l, "s");
            Assert.IsFalse(t.OverlapsWith(s));
            Assert.IsFalse(s.OverlapsWith(t));

            // now we put triangle over square but z axis equals 1 => no overlap
            l = new List<Point3D> { p1 + (2 * uY) + uZ, p2 + (2 * uY) + uZ, p3 + (2 * uY) + uZ };

            Assert.IsFalse(t.OverlapsWith(s));
            Assert.IsFalse(s.OverlapsWith(t));
        }

        [TestMethod]
        public void TestSidePoint()
        {
            // polygons to intersect with
            Polygon3D s1 = new Polygon3D(new List<Point3D> { s1_1, s1_2, s1_3, s1_4 }, "s2");
            Polygon3D s2 = new Polygon3D(new List<Point3D> { s1_1, s1_4, s1_3, s1_2 }, "s2"); // reverse order => swap surfaces

            // point "on surface" of the square
            Point3D p1 = new Point3D(1, 1, 0);
            double z = p1.Z + (MSystem.SideDist * s1.Normal.Z); // calculate Z for assert purposes

            Point3D res_s1_a = s1.SidePoint(p1, Tile.SideType.inside);
            Point3D res_s1_b = s1.SidePoint(p1, Tile.SideType.outside);
            Assert.AreEqual(res_s1_a.X, p1.X);
            Assert.AreEqual(res_s1_a.Y, p1.Y);
            Assert.AreEqual(res_s1_a.Z, z);
            Assert.AreEqual(res_s1_a.X, res_s1_b.X);
            Assert.AreEqual(res_s1_a.Y, res_s1_b.Y);
            Assert.AreEqual(res_s1_a.Z, -(res_s1_b.Z)); // a bit nasty as we started with Z=0 => test is OK, if Z <> 0 the this would fail

            Point3D res_s2_a = s2.SidePoint(p1, Tile.SideType.inside);
            Point3D res_s2_b = s2.SidePoint(p1, Tile.SideType.outside);
            Assert.AreEqual(res_s2_a, res_s1_b);
            Assert.AreEqual(res_s2_b, res_s1_a);

            // point "on edge" of the square
            p1 = new Point3D(1, 0, 0);
            z = p1.Z + (MSystem.SideDist * s1.Normal.Z); // calculate Z for assert purposes

            res_s1_a = s1.SidePoint(p1, Tile.SideType.inside);
            res_s1_b = s1.SidePoint(p1, Tile.SideType.outside);
            Assert.AreEqual(res_s1_a.X, p1.X);
            Assert.AreEqual(res_s1_a.Y, p1.Y);
            Assert.AreEqual(res_s1_a.Z, z);
            Assert.AreEqual(res_s1_a.X, res_s1_b.X);
            Assert.AreEqual(res_s1_a.Y, res_s1_b.Y);
            Assert.AreEqual(res_s1_a.Z, -(res_s1_b.Z));

            res_s2_a = s2.SidePoint(p1, Tile.SideType.inside);
            res_s2_b = s2.SidePoint(p1, Tile.SideType.outside);
            Assert.AreEqual(res_s2_a, res_s1_b);
            Assert.AreEqual(res_s2_b, res_s1_a);

            // point "not on the surface square" of the square but this is valid
            p1 = new Point3D(1, 1, 1);
            z = p1.Z + (MSystem.SideDist * s1.Normal.Z); // calculate Z for assert purposes

            res_s1_a = s1.SidePoint(p1, Tile.SideType.inside);
            res_s1_b = s1.SidePoint(p1, Tile.SideType.outside);
            Assert.AreEqual(res_s1_a.X, p1.X);
            Assert.AreEqual(res_s1_a.Y, p1.Y);
            Assert.AreEqual(res_s1_a.Z, z);
            Assert.AreEqual(res_s1_a.X, res_s1_b.X);
            Assert.AreEqual(res_s1_a.Y, res_s1_b.Y);
            // z-coordinates shall not be apart more than this calculation
            z = 2 * (MSystem.SideDist * Math.Abs(s1.Normal.Z));
            Assert.AreEqual(z, res_s1_b.Z - res_s1_a.Z, MSystem.Tolerance);

            res_s2_a = s2.SidePoint(p1, Tile.SideType.inside);
            res_s2_b = s2.SidePoint(p1, Tile.SideType.outside);
            Assert.AreEqual(res_s2_a, res_s1_b);
            Assert.AreEqual(res_s2_b, res_s1_a);
        }

        [TestMethod]
        public void TestPushingOfInPlaneInYDirection()
        {
            // polygon which we will be moving
            Polygon3D t1 = new Polygon3D(new List<Point3D> { t1_1, t1_2, t1_3 }, "t1");
            // create static polygon on the same plane, we will be testing whether there is any pushing or not for this polygone
            Vector3D shift = (-2 * uX) + (3 * uY);
            Polygon3D t2 = new Polygon3D(new List<Point3D> { t1_1+shift, t1_2+shift, t1_3+shift }, "t2");

            // we will be pushing one unit up (in Y coordinate) each time and testing the pusing of
            Vector3D pushingV = (0 * uX) + (1 * uY) + (0 * uZ);

            Vector3D expectedV = new Vector3D(0, 0, 0);
            Vector3D resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance));

            pushingV += uY;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance));

            pushingV += uY;
            expectedV += uY;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance));

            pushingV += uY;
            expectedV += uY;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance));

            pushingV += uY;
            expectedV += uY;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance));

            pushingV += uY;
            expectedV += uY;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance));

            pushingV += uY;
            expectedV += uY;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance));
        }

        [TestMethod]
        public void TestPushingOfInSpaceInZDirection()
        {
            // polygon which we will be moving
            Polygon3D t1 = new Polygon3D(new List<Point3D> { t1_1, t1_2, t1_3 }, "t1");
            // create static polygon, we will be testing whether there is any pushing or not for this polygone
            Vector3D shift = (-2*uX) + (-1*uY) + (+2*uZ);
            Polygon3D t2 = new Polygon3D(new List<Point3D> { t1_1 + shift, t1_2 + shift, t1_3 + shift }, "t2");

            // we will be pushing one unit up (in Z coordinate) each time and testing the pusing of
            Vector3D pushingV = (0 * uX) + (0 * uY) + (1 * uZ);

            Vector3D expectedV = new Vector3D(0, 0, 0);
            Vector3D resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance*100)); // XJB - pushes a bit more by design => increase tolerance

            pushingV += uZ;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance*100)); // XJB - pushes a bit more by design => increase tolerance

            pushingV += uZ;
            expectedV += uZ;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance*100)); // XJB - pushes a bit more by design => increase tolerance

            pushingV += uZ;
            expectedV += uZ;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance*100)); // XJB - pushes a bit more by design => increase tolerance
        }

        [TestMethod]
        public void TestPushingOfInSpaceInYDirection()
        {
            // polygon which we will be moving
            Polygon3D t1 = new Polygon3D(new List<Point3D> { t1_1, t1_2, t1_3 }, "t1");
            // create static polygon, we will be testing whether there is any pushing or not for this polygone
            Polygon3D t2 = new Polygon3D(new List<Point3D> { new Point3D(1, -1, -1), new Point3D(1, -1, -4), new Point3D(4, -4, -4) }, "t2");

            // we will be pushing one unit up (in Z coordinate) each time and testing the pusing of
            Vector3D pushingV = (0 * uX) + (0 * uY) + (-1 * uZ);

            Vector3D expectedV = new Vector3D(0, 0, 0);
            Vector3D resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance));

            pushingV -= uZ;
            expectedV -= uZ;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance));

            pushingV -= uZ;
            expectedV -= uZ;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance));

            pushingV -= uZ;
            expectedV -= uZ;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance));

            pushingV -= uZ;
            expectedV -= uZ;
            resultV = t1.PushingOf(t2, pushingV);
            Assert.IsTrue(expectedV.Equals(resultV, MSystem.Tolerance));
        }

        #endregion

    }
    }
