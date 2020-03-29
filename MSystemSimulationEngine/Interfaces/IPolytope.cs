using System.Collections.Generic;
using System.Collections.ObjectModel;
using MathNet.Spatial.Euclidean;

namespace MSystemSimulationEngine.Interfaces
{
    /// <summary>
    /// Polytope in 3D space usually defining shape of a tile
    /// </summary>
    public interface IPolytope : IReadOnlyList<Point3D>
    {
        /// <summary> 
        /// A unit normal vector to the polytope; for 1D polytope it has one degree of freedom
        /// </summary>
        UnitVector3D Normal { get; }

        /// <summary>
        /// Collection of edges of the polygtope
        /// </summary>
        ReadOnlyCollection<Line3D> Edges { get; }

        /// <summary>
        /// Central point (centroid)  of vertices 
        /// </summary>
        Point3D Center { get; }

        /// <summary>
        /// Distance from center to the furthest point
        /// </summary>
        double Radius { get; }

        /// <summary>
        /// Calculates pushing vector of "another" polytope NOT intersecting with this one.
        /// "This" is pushed by pushingVector which MAY cause intersection with "another"
        /// </summary>
        /// <param name="another">The polytope to be eventually pushed</param>
        /// <param name="pushingVector">Pushing vector of this polytope</param>
        Vector3D PushingOf(IPolytope another, Vector3D pushingVector);


        /// <summary>Returns all intersections of this polytope with another</summary>
        /// <param name="another">Vertices of another polytope</param>
        HashSet<Point3D> IntersectionsWith(IPolytope another);

        /// <summary>True if the polytope contains a given point (MSystem.Tolerance allowed)</summary>
        /// <param name="point"></param>
        bool ContainsPoint(Point3D point);

    }
}