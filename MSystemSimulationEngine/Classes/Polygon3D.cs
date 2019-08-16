using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MathNet.Spatial.Euclidean;
using MSystemSimulationEngine.Classes.Tools;
using MSystemSimulationEngine.Interfaces;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Represents 2D polygon with named vertices in 3D space
    /// TODO consider thick polygons, thickness = about 100 * Tolerance
    /// </summary>
    public class Polygon3D : ReadOnlyCollection<Point3D>, IPolytope
    {
        #region Private data

        /// <summary>
        /// Collection of normal vectors of edges of the polygon, placed in the polygon's plane
        /// </summary>
        private readonly ReadOnlyCollection<UnitVector3D> v_BorderNormals;

        /// <summary>
        /// Collection of offsets of border planes with normal vectors "v_BorderNormals"
        /// </summary>
        private readonly ReadOnlyCollection<double> v_D;

        #endregion


        #region Public data

        /// <summary>
        /// The unit normal vector to the polygon.
        /// </summary>
        public UnitVector3D Normal { get; }

        /// <summary>
        /// Collection of edges of the polygon.
        /// </summary>
        public ReadOnlyCollection<Line3D> Edges { get; }

        /// <summary>
        /// The plane in which the polygon lies.
        /// </summary>
        public readonly Plane Plane;

        /// <summary>
        /// The centroid of vertices of the polygon.
        /// </summary>
        public readonly Point3D Center;
        
        public string Name { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Polygon constructor.
        /// </summary>
        /// <param name="vertices">Vertices of the polygon</param>
        /// <param name="name">Name of the polygon.</param>
        public Polygon3D(IEnumerable<Point3D> vertices, string name) : base(Reorder(vertices, name))
        {
            // All exception tests are in the method Reorder
            Plane = new Plane(this[1], this[0], this[2]);
            Normal = Plane.Normal;
            Center = Point3D.Centroid(this);
            Name = name;

            var edges = this.Select((t, i) => new Line3D(t, this[++i % Count]));
            Edges = new ReadOnlyCollection<Line3D>(edges.ToList());

            var borderNormals = Edges.Select(edge => Normal.CrossProduct(edge.Direction).Negate());
            v_BorderNormals = new ReadOnlyCollection<UnitVector3D>(borderNormals.ToList());

            var offsets = this.Select((t, i) => -t.ToVector3D().DotProduct(v_BorderNormals[i]));
            v_D = new ReadOnlyCollection<double>(offsets.ToList());
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Checks the list of vertices, if counterclockwise then reorders them clockwise
        /// </summary>
        /// <param name="points">List of vertices</param>
        /// <param name="name">Polygon name</param>
        /// <returns>Reordered list of vertices.</returns>
        /// <throws>If vertices are null or less than 3 or nonconvex</throws>
        //private static IList<Point3D> Reorder (IList<Point3D> vertices, string name)
        private static IList<Point3D> Reorder(IEnumerable<Point3D> points, string name)
        {
            // test for name
            if (name == null)
                throw new NullReferenceException($"Polygon's name cannot be null");
            if (name == "")
                throw new ArgumentException($"Polygon's name cannot be empty");

            // test for non-null vertices
            if (points == null)
                throw new NullReferenceException($"Polygon {name}: list of vertices cannot be null");

            IList<Point3D> vertices = points.ToList();

            int count = vertices.Count;
            Plane plane;

            if (count < 3)
                throw new ArgumentException($"Polygon {name} has {count} vertices, must have >= 3");

            try
            {
                plane = new Plane(vertices[1], vertices[0], vertices[2]);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Polygon {name}: {e.Message}");
            }
            for (int i = 3; i < count; i++)
                if (plane.AbsoluteDistanceTo(vertices[i]) > MSystem.Tolerance)
                    throw new ArgumentException($"Polygon {name}: vertices do not lie in a plane");

            bool clockwise = true;
            bool counterclockwise = true;
            for (int i = 0; i < count; i++)
            {
                var v1 = vertices[i];
                var v2 = vertices[(i + 1)%count];
                var v3 = vertices[(i + 2)%count];
                if (v1.MyEquals(v2) || v2.MyEquals(v3))
                    throw new ArgumentException($"Polygon {name}: vertices are too close");

                var cpv = (v1 - v2).CrossProduct(v2 - v3);
                var sign = cpv.DotProduct(plane.Normal);
                clockwise &= sign < 0;
                counterclockwise &= sign > 0;
            }

            if (clockwise)
                return vertices.ToList();
            if (counterclockwise)
                return vertices.Reverse().ToList();

            throw new ArgumentException($"Polygon {name} must be convex.");
        }


        /// <summary>
        /// Returns all points where another polygon intersects this one. 
        /// Includes boundary/vertex touches.
        /// </summary>
        /// <param name="another">Polygon.</param>
        private HashSet<Point3D> OneWayIntersectionsWith(IReadOnlyList<Point3D> another)
        {
            var intersections = new HashSet<Point3D>(Geometry.PointComparer);

            for (int i = 0; i < another.Count; i++)
            {
                Point3D intersection = IntersectionWith(another[i], another[(i + 1) % another.Count]);
                if (!intersection.IsNaN())
                    intersections.Add(intersection);
            }
            return intersections;
        }

        /// <summary>
        /// Returns minimal distance of a vertex of the first polytope to an edge of the second one 
        /// in the direction of pushingVector. 
        /// The result is bounded from above by the length of pushingVector.
        /// </summary>
        /// <param name="vertices1">First polytope</param>
        /// <param name="vertices2">Second polytope</param>
        /// <param name="pushingVector">Pushing vector of the first polytope</param>
        private static double UnidirectionalDistance(IPolytope vertices1, IPolytope vertices2, Vector3D pushingVector)
        {
            var distance = pushingVector.Length;
            foreach (var point in vertices1)
                foreach (var edge in vertices2.Edges)
                {
                    var closePoints = new Line3D(point, point + pushingVector).ClosestPointsBetween(edge, true);
                    if (closePoints.Item1.DistanceTo(closePoints.Item2) <= MSystem.Tolerance)
                        // The projection of "point" by pushingVector intersects "edge"
                        distance = Math.Min(distance, point.DistanceTo(closePoints.Item1));
                }
            return distance;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns intersection point of this polygon with a line, or NaN if no intersection exists.
        /// </summary>
        /// <param name="p1">Line startpoint.</param>
        /// <param name="p2">Line endpoint.</param>
        /// <param name="borderWidth">Intersection within borderWidth not considered.</param>
        /// <param name="includeEndpoints">Consider also intersections in endpoints p1, p2 of the line?</param>
        public Point3D IntersectionWith(Point3D p1, Point3D p2, double borderWidth = 0, bool includeEndpoints = true)
        {
            Point3D intersection = Plane.MyIntersectionWith(p1, p2);

            if (!intersection.IsNaN() && ContainsPoint(intersection, borderWidth) &&
                (includeEndpoints || !(intersection.MyEquals(p1) || intersection.MyEquals(p2))))
                return intersection;
            return Point3D.NaN;
        }

        /// <summary>
        /// Gets all intersection of this polygon with a polytope (segment or polygon).
        /// Ignores boundary touches within Msystem.Tolerance.
        /// If the polytope is parallel to this polygon, nothing is returned even if they overlap.
        /// </summary>
        /// <param name="polytope">Polyline to intersect with.</param>
        /// <returns>List of intersection points.</returns>
        public HashSet<Point3D> IntersectionsWith(IPolytope polytope)
        {
            var polygon = polytope as Polygon3D;
            if (polygon != null)  
            {
                var intersections = OneWayIntersectionsWith(polygon);
                intersections.UnionWith(polygon.OneWayIntersectionsWith(this));
                if (intersections.Any())
                {
                    var center = Point3D.Centroid(intersections);
                    if (ContainsPoint(center, MSystem.Tolerance) && polygon.ContainsPoint(center, MSystem.Tolerance))
                        return intersections;
                }
            }
            var result = new HashSet<Point3D>(Geometry.PointComparer);
            if (polytope is Segment3D)
            {
                var intersection = IntersectionWith(polytope[0], polytope[1], MSystem.Tolerance, false);
                if (!intersection.IsNaN())
                    result.Add(intersection);
            }
            return result;
        }


        /// <summary>True if the polygon contains a given point (MSystem.Tolerance allowed).</summary>
        /// <param name="point"></param>
        public bool ContainsPoint(Point3D point) => ContainsPoint(point, 0);


        /// <summary>
        /// True if the given point lies inside the polygon
        /// </summary>
        /// <param name="point"></param>
        /// <param name="borderWidth">Width of the polygon border which is not considered inside</param>
        /// <param name="verticalTolerance">Tolerated distance of the point from the polygon's plane</param>
        public bool ContainsPoint(Point3D point, double borderWidth, double verticalTolerance = MSystem.Tolerance)
        {
            if (Plane.AbsoluteDistanceTo(point) > verticalTolerance)
                return false;

            var pointV = point.ToVector3D();

            for (int i = 0; i < Count; i++) 
            {
                // Signed distance of point to i-th edge of the polygon (+ inside, - outside)
                var signedDist = v_BorderNormals[i].DotProduct(pointV) + v_D[i];

                if (borderWidth > 0 && signedDist < borderWidth || signedDist < -MSystem.Tolerance)
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Returns true if this polygon intersects a given line.
        /// </summary>
        /// <param name="p1">Line startpoint.</param>
        /// <param name="p2">Line endpoint.</param>
        /// <param name="borderWidth">Intersection within borderWidth not considered.</param>
        /// <param name="includeEndpoints">Consider also intersections in endpoints p1, p2 of the line?</param>
        public bool IntersectsWith(Point3D p1, Point3D p2, double borderWidth = 0, bool includeEndpoints=true)
        {
            // Speed optimization - if both points lie on the same side of the polygon, then line does not intersect
            var prod1 = Normal.DotProduct(p1.ToVector3D()) + Plane.D;
            var prod2 = Normal.DotProduct(p2.ToVector3D()) + Plane.D;
            if (prod1*prod2 > MSystem.Tolerance*100)
                return false;

            return ! IntersectionWith(p1, p2, borderWidth, includeEndpoints).IsNaN();
        }


        /// <summary>
        /// True if (a) both polygons lie in the same plane (vertical distance up to Msystem.Tolerance), and
        /// (b) they overlap (the overlapping horizontal distance exceeds Msystem.Tolerance)
        /// </summary>
        public bool OverlapsWith(Polygon3D another)
        {
            if (!(Normal.IsParallelTo(another.Normal) && 
                Plane.AbsoluteDistanceTo(another[0]) < MSystem.Tolerance))
                return false;

            // If one polygon contains inside a vertex of another, then they overlap
            // Border tolerance is allowed for case that they just touch (do not overlap)
            if (another.Any(t => ContainsPoint(t, MSystem.Tolerance)) ||
                this.Any(t => another.ContainsPoint(t, MSystem.Tolerance)))
                return true;

            // Now check the case when no vertex of one polygon lies inside another
            var intersections = new HashSet<Point3D>();
            foreach (var edge1 in Edges)
                foreach (var edge2 in another.Edges)
                {
                    var intersection = edge1.ClosestPointsBetween(edge2, true);
                    if (intersection.Item1.DistanceTo(intersection.Item2) < MSystem.Tolerance)
                        intersections.Add(intersection.Item1);
                }
            if (!intersections.Any())
                return false;

            // Again exclude the case when they just touch and do not overlap
            var c = Point3D.Centroid(intersections);
            return ContainsPoint(c, MSystem.Tolerance) || another.ContainsPoint(c, MSystem.Tolerance);

        }


        /// <summary>
        /// Returns the inside/outside reference point used for search / adding / removal of floating objects
        /// </summary>
        /// <param name="position">Position on tile.</param>
        /// <param name="side">Side of the tile</param>
        /// <returns>Reference point corresponding to the "side" parameter.</returns>
        public Point3D SidePoint(Point3D position, Tile.SideType side)
        {
            return position + (side == Tile.SideType.inside ? 1 :
                side == Tile.SideType.outside ? -1 : 0) * MSystem.SideDist * Normal;
        }


        /// <summary>
        /// Calculates pushing of "another" polytope by this polygon, being itself 
        /// pushed by "pushingVector" which MAY cause its intersection with "another".
        /// If this polygon already intersects "another", "pushingVector" is returned.
        /// </summary>
        /// <param name="another">The polytope to be eventually pushed.</param>
        /// <param name="pushingVector">Pushing vector of this polytope</param>
        /// <returns>Pushing vector of another polytope which is not longer than "pushingVector".</returns>
        public Vector3D PushingOf(IPolytope another, Vector3D pushingVector)
        {
            if (pushingVector.Length < MSystem.Tolerance)   // must be tested, otherwise the Polygon3D ctor may throw an exception
                return default(Vector3D);

            if (another is Segment3D)
                return -another.PushingOf(this, -pushingVector);

            // If we got here, both objects are polygons
            var pushingDirection = pushingVector.Normalize();
            var distance = pushingVector.Length;

            // TODO if HealthCheck test for overlapping would not pass, then replace this by test for OverlapsWith
            if (Normal.IsPerpendicularTo(pushingDirection, 1E-12D))
                if (another.Normal.IsPerpendicularTo(pushingDirection, 1E-12D))    
                // Both polygons are parallel and pushing "edge to edge"
                {
                    if (Plane.AbsoluteDistanceTo(another[0]) >= MSystem.Tolerance)
                        return default(Vector3D);       // Polygons not in the same plane => not intersecting

                    distance = Math.Min(UnidirectionalDistance(this, another, pushingVector),
                        UnidirectionalDistance(another, this, -pushingVector));
                    return pushingDirection.ScaleBy(pushingVector.Length - distance);
                }
                else
                    // This polygon is parallel to the pushing vector, but another is not
                    return -another.PushingOf(this, -pushingVector);

            // If we got here, this polygon is not parallel to the pushing direction
            var intersections = new HashSet<Point3D>(Geometry.PointComparer);

            // Get intersections of edges of "another" with projection faces of "this"
            foreach (var edge in Edges)
            {
                var projectionFace = new Polygon3D(new List<Point3D> { edge.StartPoint, edge.EndPoint, edge.EndPoint + pushingVector, edge.StartPoint + pushingVector },
                    Name + "-pushing projection");
                intersections.UnionWith(projectionFace.IntersectionsWith(another));
            }

            // Add intersections of "this" with vectors -pushingVector starting in vertices of "another"
            intersections.UnionWith(another.Where(point => IntersectsWith(point, point - pushingVector, MSystem.Tolerance, false)));

            // Minimum distance of "this" to some of these intersection points
            foreach (var point in intersections)
            {
                var t = Math.Abs( Plane.SignedDistanceTo(point) / Normal.DotProduct(pushingDirection));
                distance = Math.Min(distance, t);
            }

            // Length of necessary pushing
            distance = pushingVector.Length - distance;

            if (Normal.IsParallelTo(another.Normal) && (distance > 0 || 
                ((Polygon3D)another).OverlapsWith(new Polygon3D(this.Select(v => v + pushingVector), Name+" pushed"))))
                //    
                // Both polygons are parallel, pushing "face to face" => minimal face distance must be ensured
                distance += MSystem.MinFaceDist / Math.Abs(Normal.DotProduct(pushingDirection));

            // Ensure that the resulting vector is NOT LONGER than "pushingVector"
            return pushingDirection.ScaleBy(Math.Min(distance, pushingVector.Length));
        }
        #endregion
    }

}
