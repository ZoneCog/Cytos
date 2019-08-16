using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MathNet.Spatial.Euclidean;
using MSystemSimulationEngine.Classes.Tools;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Represents list of named vertices of a tile forming either a rod, a polygon or a polyhedron
    /// Provides a method calculating vertices of a regular polygon
    /// </summary>
    public class NamedVertices : ReadOnlyCollection<NamedPosition>
    {
        #region Constructor

        /// <summary>
        /// Polygon constructor.
        /// </summary>
        /// <param name="vertices">List of vertices of tile.</param>
        public NamedVertices(IList<NamedPosition> vertices) : base(vertices)
        {
            if (Count < 2)
            {
                throw new InvalidOperationException($"The number of vertices of a tile is {Count}, must be >= 2.");
            }
        }

        /// <summary>
        /// Regular polygon or rod constructor.
        /// </summary>
        /// <param name="sides">Number of sides of the polygon</param>
        /// <param name="vertexDistance">Distance between vertices in polygon.</param>
        public NamedVertices(int sides, double vertexDistance) : this(GetVertices(sides, vertexDistance))
        { }

        #endregion

        #region Private methods

        /// <summary>
        /// Creates predefined number of vertices of a regular polygon or a rod.
        /// </summary>
        /// <param name="sides">Number of sides of the polygon</param>
        /// <param name="vertexDistance">Distance between vertices in polygon.</param>
        /// <returns>List of vertices</returns>
        private static List<NamedPosition> GetVertices(int sides, double vertexDistance)
        {
            var vertices = new List<NamedPosition>();
            double angle = sides==4 ? -Math.PI/4 : 0;   // We want squares to be placed in an orthogonal position, starting in top left corner
            for (int i = 0; i < sides; i++)
            {
                double posY = vertexDistance * Math.Cos(angle);
                double posX = vertexDistance * Math.Sin(angle);
                double posZ = 0;
                vertices.Add(new NamedPosition($"v{i + 1}", (new Point3D(posX, posY, posZ))));
                angle += 2*Math.PI/sides;   // Try to keep precision
            }
            return vertices;
        }

        #endregion

    }
}
