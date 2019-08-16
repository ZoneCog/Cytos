using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace MSystemSimulationEngine.Classes.Tools
{
    /// <summary>
    /// Point3D comparer with a given tolerance.
    /// </summary>
    internal class Point3DEqComparer : EqualityComparer<Point3D>
    {
        public override bool Equals(Point3D p1, Point3D p2) => p1.Equals(p2, MSystem.Tolerance);
        public override int GetHashCode(Point3D p) => base.GetHashCode();
    }


    /// <summary>
    /// Class that extends the functionality of Mathnet.Spatial.Euclidean.
    /// </summary>
    public static class Geometry
    {
        /// <summary>
        /// Point equality comparer based on Msystem.Tolerance
        /// </summary>
        public static readonly IEqualityComparer<Point3D> PointComparer = new Point3DEqComparer();

        /// <summary>
        /// Extension method for Plane class.
        /// Find intersection between a line defined by its endpoints and a Plane
        /// http://geomalgorithms.com/a05-_intersect-1.html
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="p1"> line startpoint</param>
        /// <param name="p2"> line endpoint</param>
        /// <returns>
        /// The intersection point if the line segment intersects the plane; NaN otherwise.
        /// </returns>
        public static Point3D MyIntersectionWith(this Plane plane, Point3D p1, Point3D p2)
        {
            var u = p1.VectorTo(p2);
            var product = u.DotProduct(plane.Normal);
            if (Math.Abs(product) < MSystem.Tolerance) //either parallel or lies in the plane
                return Point3D.NaN;

            var d = plane.SignedDistanceTo(p1);
            var t = -1 * d / product;
            if (t > 1 || t < 0) // They are not intersected
            {
                return Point3D.NaN;
            }
            return p1 + (t * u);
        }

        /// <summary>
        /// A normal vector orthogonal to this - prevent exception 'result too small'
        /// </summary>
        public static UnitVector3D MyOrthogonal(this Vector3D v)
        {
            var dx = (v.Z + v.Y) * (v.Z + v.Y) + 2 * v.X * v.X;
            var dy = (v.X + v.Z) * (v.X + v.Z) + 2 * v.Y * v.Y;
            var dz = (v.X + v.Y) * (v.X + v.Y) + 2 * v.Z * v.Z;

            // Choose the largest orthogonal vector of the three
            if (dx >= dy && dx >= dz)
                return new UnitVector3D(-v.Y - v.Z, v.X, v.X);
            if (dy >= dx && dy >= dz)
                return new UnitVector3D(v.Y, -v.X - v.Z, v.Y);
            else
                return new UnitVector3D(v.Z, v.Z, -v.X - v.Y);
        }

        /// <summary>
        /// Extension method. True if p1 equal to p2 within MSystem.Tolerance.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point.</param>
        public static bool MyEquals(this Point3D p1, Point3D p2) => PointComparer.Equals(p1, p2);


        /// <summary>
        /// Extension method. Rounds all coordinates of a point to d digits.
        /// </summary>
        /// <param name="p">Point.</param>
        /// <param name="d">Number of decimal digits of coordinates.</param>
        public static Point3D Round(this Point3D p, int d = 15) => 
            new Point3D(Math.Round(p.X, d), Math.Round(p.Y, d), Math.Round(p.Z, d));

                /// <summary>
        /// Extension method. True if p1 has all coordinates smaller than or equal to p2.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point.</param>
        public static bool IsLeq(this Point3D p1, Point3D p2) =>
            p1.X <= p2.X && p1.Y <= p2.Y && p1.Z <= p2.Z;


        /// <summary>
        /// Extension method. True if the point is NaN.
        /// </summary>
        public static bool IsNaN(this Point3D point) =>
            double.IsNaN(point.X) || double.IsNaN(point.Y) || double.IsNaN(point.Z);


        /// <summary>
        /// Extension method. Rotates the point by a quaternion.
        /// </summary>
        /// <param name="p1">Rotated point.</param>
        /// <param name="rotation">Rotation quaternion.</param>
        public static Point3D Rotate(this Point3D p1, Quaternion rotation)
        {
            if (!rotation.IsUnitQuaternion)
                throw new ArgumentException("The rotation quaternion is not a Unit Quaternion");
            Quaternion point = new Quaternion(0, p1.X, p1.Y, p1.Z);
            Quaternion result = (rotation*point)*rotation.Conjugate();
            return new Point3D(result.ImagX, result.ImagY, result.ImagZ);
        }


        /// <summary>
        /// Extension method for EulerAngles class. 
        /// Converts the rotation yaw-pitch roll (intrinsic Tait-Bryan angles) to quaternion.
        /// </summary>
        /// <param name="angles"></param>
        /// <returns>
        /// The quaternion corresponding to rotation by the angles.
        /// </returns>
        public static Quaternion ToQuaternion(this EulerAngles angles)
        {
            double alpha = angles.Alpha.Radians/2;
            double beta = angles.Beta.Radians/2;
            double gamma = angles.Gamma.Radians/2;
            return new Quaternion(
                Math.Cos(alpha) * Math.Cos(beta) * Math.Cos(gamma) + Math.Sin(alpha) * Math.Sin(beta) * Math.Sin(gamma),
                Math.Sin(alpha) * Math.Cos(beta) * Math.Cos(gamma) - Math.Cos(alpha) * Math.Sin(beta) * Math.Sin(gamma),
                Math.Cos(alpha) * Math.Sin(beta) * Math.Cos(gamma) + Math.Sin(alpha) * Math.Cos(beta) * Math.Sin(gamma),
                Math.Cos(alpha) * Math.Cos(beta) * Math.Sin(gamma) - Math.Sin(alpha) * Math.Sin(beta) * Math.Cos(gamma));
        }

        /// <summary>
        /// DEBUGGING, returns quaternion as real+ImagXi+ImagYj+ImagZk 
        /// </summary>
        /// <returns></returns>
        public static string Print(this Quaternion q)
        {
            return string.Format("{0}{1}{2}i{3}{4}j{5}{6}k",
                Math.Round(q.Real,3),
                (q.ImagX < 0) ? "" : "+",
                Math.Round(q.ImagX,3),
                (q.ImagY < 0) ? "" : "+",
                Math.Round(q.ImagY,3),
                (q.ImagZ < 0) ? "" : "+",
                Math.Round(q.ImagZ,3));
        }
        
        /// <summary>
        /// Creates a new quaternion representing the axis/angle rotation.
        /// </summary>
        /// <param name="axis">Rotation axis.</param>
        /// <param name="angle">Rotation angle.</param>
        /// <returns>
        /// The quaternion corresponding to rotation by the axis/angle.
        /// </returns>
        public static Quaternion NewQuaternion(UnitVector3D axis, Angle angle) =>
            new Quaternion(Math.Cos(angle.Radians / 2), 
                axis.X * Math.Sin(angle.Radians / 2),
                axis.Y * Math.Sin(angle.Radians / 2),
                axis.Z * Math.Sin(angle.Radians / 2));

        /// <summary>
        /// Creates a new quaternion representing the axis/angle rotation.
        /// </summary>
        /// <param name="axis">Rotation axis.</param>
        /// <param name="angle">Rotation angle.</param>
        /// <returns>
        /// The quaternion corresponding to rotation by the axis/angle.
        /// </returns>
        public static bool Overlap(IEnumerable<Point3D> list1, IEnumerable<Point3D> list2, double tolerance = MSystem.Tolerance) =>
            list1.All(pos1 => list2.Any(pos2 => pos1.DistanceTo(pos2) <= tolerance)) &&
            list2.All(pos2 => list1.Any(pos1 => pos1.DistanceTo(pos2) <= tolerance));


    }

}
