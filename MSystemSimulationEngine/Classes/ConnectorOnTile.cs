using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MathNet.Spatial.Euclidean;
using MSystemSimulationEngine.Classes.Tools;
using MSystemSimulationEngine.Interfaces;
using static MSystemSimulationEngine.Classes.Tools.Geometry;

namespace MSystemSimulationEngine.Classes
{
    public class ConnectorOnTile : Connector
    {
        #region Private data

        /// <summary>
        /// Internal representation of posotions
        /// </summary>
        private ReadOnlyCollection<Point3D> v_Positions;

        #endregion

        #region Public data

        /// <summary>
        /// The tile in space on which is the connector placed.
        /// </summary>
        public readonly Tile OnTile;

        /// <summary>
        /// Positions of the connector on the tile
        /// </summary>
        public new ReadOnlyCollection<Point3D> Positions {
            get
            {
                return v_Positions;
            }
            private set
            {
                CheckPositions(value, OnTile.Vertices);
                v_Positions = value;
            }
        }

        /// <summary>
        /// Indicates whether the connector is located on inner or outer face of the object.
        /// Relevant only for connectors on 2D.
        /// </summary>
        public readonly Tile.SideType Side;

        #endregion

        #region Constructor

        /// <summary>
        /// ConnectorOnTileInSpace constructor
        /// </summary>
        /// <param name="tile">Tile on which is this connector placed.</param>
        /// <param name="connector">Base connector from which this connector derives.</param>
        public ConnectorOnTile(Tile tile, Connector connector) 
            : base(connector.Name, connector.Positions, connector.Glue, connector.Angle, connector.Resistance)
        {
            OnTile = tile ?? throw new ArgumentException("Tile cannot be null.");
            // setter to Positions uses OnTile!!!
            Positions = connector.Positions;

            if (OnTile.Vertices is Polygon3D && Positions.Count == 1)
            {
                if (Angle.Radians > 0)
                    Side = Tile.SideType.inside;
                if (Angle.Radians < 0)
                    Side = Tile.SideType.outside;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Throws exception if the connector does not lie entirely on its tile or does not have smaller dimension
        /// or when a connector with Angle==0 on segment is not its endpoint
        /// or when a connector with Angle==0 on polygon is not at its border (if it is edge, must be clocwise)
        /// </summary>
        protected void CheckPositions(IReadOnlyList<Point3D> positions, IPolytope polytope)
        {
            var message = $"{this} of object {OnTile.Name}: ";

            if (! (positions.Count == 1 || polytope is Polygon3D && positions.Count == 2))
                throw new InvalidOperationException(message + "connector contains too many points.");
                
            if (! positions.All(point => polytope.ContainsPoint(point)))
                throw new InvalidOperationException(message + "all points must lie on the object.");

            if (Angle.Radians == 0)
                if (polytope is Polygon3D)
                {
                    var polygon = polytope as Polygon3D;
                    switch (positions.Count)
                    {
                        case 1:
                            // If Angle == 0 => connector must be on the polygon's border
                            if (polygon.ContainsPoint(positions[0], MSystem.Tolerance))
                                throw new InvalidOperationException(message + "connector with zero angle must lie on the border.");
                            break;

                        case 2:
                            int i = -1;
                            int j = -1;
                            for (int k = 0; k < polygon.Count; k++)
                            {
                                if (polygon[k].MyEquals(positions[0]))
                                    i = k;
                                if (polygon[k].MyEquals(positions[1]))
                                    j = k;
                            }
                            if (! (i >= 0 && (i + 1) % polygon.Count == j))
                                throw new InvalidOperationException(message + "connector with zero angle must be an edge of polygon, clockwise.");
                            break;
                    }

                }
                else if (polytope is Segment3D && ! polytope.Contains(positions[0], PointComparer))
                    throw new InvalidOperationException(message + "connector with zero angle must lie at an endpoint of segment.");
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns true if connectors are of the same size and they are not both point connectors on 2D tiles.
        /// </summary>
        public bool IsSizeComppatibleWith(ConnectorOnTile connector) => 
            Positions.Count == 1 && connector.Positions.Count == 1 &&
            (OnTile.Vertices is Segment3D || connector.OnTile.Vertices is Segment3D)
            ||
            Positions.Count == 2 && connector.Positions.Count == 2 &&
            Math.Abs(Positions[0].DistanceTo(Positions[1]) -
                     connector.Positions[0].DistanceTo(connector.Positions[1])) <= MSystem.Tolerance;

        #endregion
    }
}
