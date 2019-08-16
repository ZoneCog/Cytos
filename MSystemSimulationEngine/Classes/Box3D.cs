using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MathNet.Spatial.Euclidean;
using MSystemSimulationEngine.Classes.Tools;
using SharedComponents.Tools;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// 3D box with edges parallel to coordinate axes
    /// Not a struct as it sets its private properties in some impure methods
    ///  </summary>
    public struct Box3D
    {
        #region Public data

        /// <summary>
        /// Corner with all minimal coordinates.
        /// </summary>
        public readonly Point3D MinCorner;

        /// <summary>
        /// Corner with all minimal coordinates.
        /// </summary>
        public readonly Point3D MaxCorner;

        /// <summary>
        /// Volume of the box.
        /// </summary>
        public double Volume => (MaxCorner.X - MinCorner.X) * (MaxCorner.Y - MinCorner.Y) * (MaxCorner.Z - MinCorner.Z);
        
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the 3D box defined by the minimal and maximal corner.
        /// </summary>
        /// <param name="minCorner">Corner with all minimal coordinates.</param>
        /// <param name="maxCorner">Corner with all maximal coordinates.</param>
        /// <exception cref="ArgumentException">If the first corner has any coordinate greater than the second one</exception>
        ///
        public Box3D(Point3D minCorner, Point3D maxCorner)
        {
            if (minCorner.IsLeq(maxCorner))
            {
                MinCorner = minCorner;
                MaxCorner = maxCorner;
            }
            else
            {
                throw new ArgumentException($"The first box corner {minCorner} must not have any coordinate greater than the second corner {maxCorner}.");
            }
        }

        /// <summary>
        /// Constructor of the 3D box enclosing a given list of 3D points.
        /// </summary>
        /// <param name="points">List of 3D points.</param>
        ///
        public Box3D(IEnumerable<Point3D> points)
        {
           if (points?.Any() ?? false)
            {
                var myPoints = points as Point3D[] ?? points.ToArray();
                double xmin = myPoints.Min(point => point.X);
                double xmax = myPoints.Max(point => point.X);
                double ymin = myPoints.Min(point => point.Y);
                double ymax = myPoints.Max(point => point.Y);
                double zmin = myPoints.Min(point => point.Z);
                double zmax = myPoints.Max(point => point.Z);
                MinCorner = new Point3D(xmin, ymin, zmin);
                MaxCorner = new Point3D(xmax, ymax, zmax);
            }
            else
            {
                MinCorner = MaxCorner = Point3D.Origin;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Expands the box with another box so that the resulting box contains both.
        /// </summary>
        /// <param name="newBox">Expanding box.</param>
        public Box3D ExpandWith(Box3D newBox)
        {
            return new Box3D(new Point3D(
                Math.Min(MinCorner.X, newBox.MinCorner.X),
                Math.Min(MinCorner.Y, newBox.MinCorner.Y),
                Math.Min(MinCorner.Z, newBox.MinCorner.Z)),
                   new Point3D(
                Math.Max(MaxCorner.X, newBox.MaxCorner.X),
                Math.Max(MaxCorner.Y, newBox.MaxCorner.Y),
                Math.Max(MaxCorner.Z, newBox.MaxCorner.Z)));
        }


        /// <summary>
        /// Calculates random point within the box including borders.
        /// </summary>
        /// <returns>Random point within the box.</returns>
        [Pure]
        public Point3D RandomPoint()
        {
            return new Point3D(Randomizer.NextDoubleBetween(MinCorner.X, MaxCorner.X),
                Randomizer.NextDoubleBetween(MinCorner.Y, MaxCorner.Y),
                Randomizer.NextDoubleBetween(MinCorner.Z, MaxCorner.Z));
        }

        /// <summary>
        /// Checks whether the box contains a point.
        /// </summary>
        /// <param name="point">3D point.</param>
        /// <returns>True if the point lies within the box.</returns>
        [Pure]
        public bool Contains(Point3D point)
        {
            return MinCorner.IsLeq(point) && point.IsLeq(MaxCorner);
        }

        #endregion

    }
}
