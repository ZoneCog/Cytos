using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MathNet.Spatial.Euclidean;
using MSystemSimulationEngine.Classes.Tools;
using MSystemSimulationEngine.Interfaces;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Represents 1D line segment in 3D space represented as list of two points
    /// </summary>
    public class Segment3D : ReadOnlyCollection<Point3D>, IPolytope
    {
        #region Public data

        /// <summary>
        /// The vector of this segment.
        /// </summary>
        public Vector3D Vector => this[0].VectorTo(this[1]);

        /// <summary>
        /// Singleton collection of edges of the segment.
        /// </summary>
        public ReadOnlyCollection<Line3D> Edges { get; }

        /// <summary>
        /// A unit vector orthogonal to the segment.
        /// </summary>
        public UnitVector3D Normal => Vector.MyOrthogonal();

        /// <summary>
        /// The centroid of vertices of the polygon.
        /// </summary>
        public Point3D Center { get; }

        /// <summary>
        /// Distance from center to the furthest point
        /// </summary>
        public double Radius { get; }

        public readonly string Name;

        #endregion

        #region Constructor

        /// <summary>
        /// Line segment constructor.
        /// </summary>
        /// <param name="startPoint">First point of the segment</param>
        /// <param name="endPoint">Second point of the segment</param>
        /// <param name="name"></param>
        public Segment3D(Point3D startPoint, Point3D endPoint, string name) : base(new [] {startPoint, endPoint})
        {
            if (Geometry.PointComparer.Equals(startPoint, endPoint))
                throw new ArgumentException($"Segment {name}: endpoints are too close.");
            Name = name;
            Center = Point3D.Centroid(this);
            Radius = this[0].DistanceTo(Center);
            Edges = new ReadOnlyCollection<Line3D>(new [] {new Line3D(startPoint, endPoint) });
        }
        #endregion

        /// <summary>
        /// Calculates possible pushing of "another" polytope by this segment being itself 
        /// pushed by "pushingVector" which MAY cause its intersection with "another".
        /// If this segment already intersects "another", "pushingVector" is returned.
        /// </summary>
        /// <param name="another">The vertices of object to be eventually pushed.</param>
        /// <param name="pushingVector">Pushing vector of this object.</param>
        /// <returns>Pushing vector of another object.</returns>
        public Vector3D PushingOf(IPolytope another, Vector3D pushingVector)
        {
            Polygon3D polygon = another as Polygon3D;
            if (polygon == null || pushingVector.Length < MSystem.Tolerance)  // length must be tested, otherwise the Polygon3D ctor may throw an exception
                return default(Vector3D);

            if (this.Vector.IsParallelTo(pushingVector))
            {
                var intersection0 = polygon.IntersectionWith(this[0], this[0] + pushingVector);
                var intersection1 = polygon.IntersectionWith(this[1], this[1] + pushingVector);

                var vector0 = intersection0.IsNaN() ? pushingVector : intersection0 - this[0];
                var vector1 = intersection1.IsNaN() ? pushingVector : intersection1 - this[1];
                
                // Get minimal vector from this segment to the intersection point
                var minVector = vector0.Length > vector1.Length ? vector1 : vector0;
                return pushingVector - minVector;
            }

            var projectionFace = new Polygon3D(new List<Point3D> { this[0], this[1], this[1] + pushingVector, this[0] + pushingVector }, 
                Name + "-pushing projection");
            double pLength = pushingVector.Length, distance = pLength;
            
            foreach (var point in polygon.IntersectionsWith(projectionFace))
                distance = Math.Min(distance, point.DistanceTo(
                    Edges[0].ClosestPointsBetween(new Line3D(point, point-pushingVector), true).Item2));
            
            return pushingVector.ScaleBy((pLength - distance)/pLength);
        }


        /// <summary>
        /// Gets all intersection of this segment with a list of vertices.
        /// Ignores boundary touchs within Msystem.Tolerance.
        /// </summary>
        /// <param name="another">Vertices to intersect with.</param>
        /// <returns>List of intersection points.</returns>
        public HashSet<Point3D> IntersectionsWith(IPolytope another)
        {
            var polygon = another as Polygon3D;
            return polygon?.IntersectionsWith(this) ?? new HashSet<Point3D>();
        }

        /// <summary>True if the segment contains a given point (MSystem.Tolerance allowed).</summary>
        /// <param name="point"></param>
        public bool ContainsPoint(Point3D point) => Edges[0].ClosestPointTo(point, true).DistanceTo(point) < MSystem.Tolerance;

    }
}
