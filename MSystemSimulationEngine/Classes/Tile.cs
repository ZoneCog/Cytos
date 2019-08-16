using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using MathNet.Spatial.Euclidean;
using MSystemSimulationEngine.Interfaces;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Represents tile class whose instances are types of tile of the P system.
    /// </summary>
    public class Tile : SimulationObject
    {
        #region Public data

        /// <summary>
        /// Holds vertices of the tile = readonly collection of immutable points.
        /// </summary>
        public readonly IPolytope Vertices;

        /// <summary>
        /// Holds connectors of the tile.
        /// </summary>
        public readonly ReadOnlyCollection<ConnectorOnTile> Connectors;

        /// <summary>
        /// Holds the surface glue of the tile. TODO use two glues for in/out sides
        /// </summary>
        public readonly Glue SurfaceGlue;

        /// <summary>
        /// Holds proteins of the tile.
        /// </summary>
        public readonly ReadOnlyCollection<ProteinOnTile> Proteins;

        /// <summary>
        /// Holds the color of the tile (named if possible).
        /// </summary>
        public readonly Color Color;

        /// <summary>
        /// Holds the alpha attribute of color of the tile.
        /// </summary>
        public readonly int Alpha;

        /// <summary>
        /// Enum holding possible position of object (connector, protein) on a tile.
        /// The "inside" and "outside" values are relevant only for polygons
        /// </summary>
        public enum SideType
        {
            undef, inside, outside
        }

        /// <summary>
        /// Tile resistor ratio in (0, infinity>, only for cTAM ladders, generalize for 2D cTAM.
        /// </summary>
        public readonly double AlphaRatio;

        #endregion

        #region Constructors

        /// <summary>
        /// Tile constructor. TODO low priority: separate constructor for vertices 1D, vertices 2D
        /// </summary>
        /// <param name="name">Name of the tile.</param>
        /// <param name="vertices">Vertices of the tile.</param>
        /// <param name="connectors">Connectors of the tile.</param>
        /// <param name="surfaceGlue">Surface glue of the tile.</param>
        /// <param name="proteins">Proteins of the tile.</param>
        /// <param name="color">Color of the tile (named if possible).</param>
        /// <param name="alpha">Alpha attribute of the color</param>
        /// <param name="alphaRatio">Tile resistor ratio, for cTAM tilings</param>
        public Tile(string name, IList<Point3D> vertices, IEnumerable<Connector> connectors,  Glue surfaceGlue, IList<ProteinOnTile> proteins, 
            Color color, int alpha = 255, double alphaRatio = 1) : base(name)
        {
            Vertices = CreateVertices(vertices, name);
            SurfaceGlue = surfaceGlue ?? new Glue("EmptyGlue");
            Color = color;
            Alpha = alpha;
            AlphaRatio = alphaRatio;

            Proteins = new ReadOnlyCollection<ProteinOnTile>(proteins ?? new List<ProteinOnTile>());
            foreach (var protein in Proteins)
                if (!Vertices.ContainsPoint(protein.Position))
                    throw new InvalidOperationException($"{protein} must be on the object {name}.");

            Connectors = new ReadOnlyCollection<ConnectorOnTile>(connectors?.Select(conn => new ConnectorOnTile(this, conn)).ToList() 
                ?? new List<ConnectorOnTile>());
        }



        #endregion

        #region Private methods

        /// <summary>
        /// Creates vetices of a tile.
        /// </summary>
        /// <param name="vertices">List of vertices.</param>
        /// <param name="name">Name of the object.</param>
        protected static IPolytope CreateVertices(IList<Point3D> vertices, string name)
        {
            if (vertices == null)
            {
                throw new ArgumentException($"Vertices of tile {name} cannot be null.");
            }
            if (vertices.Count == 2)
                return new Segment3D(vertices[0], vertices[1], name);

            return new Polygon3D(vertices, name);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String representation of object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                "Tile: name = {0}, \n{1} \nSurface {2}, \nProteins = {3}\n{4}", Name,
                String.Join("\n", Connectors), SurfaceGlue, String.Join(",", Proteins.Select(obj => obj.Name)), Color);

            return builder.ToString();
        }

        #endregion
    }
}
