using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Spatial.Units;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Collection of surface connectors of a tile.
    /// Other non-surface connectors can connect to these connectors if the glues match.
    /// !!! Positions of these connectors have no meaning!!!
    /// </summary>
    public class SurfaceConnector : IEnumerable<ConnectorOnTileInSpace>
    {
        #region Private data

        private readonly List<ConnectorOnTileInSpace> v_Connectors = new List<ConnectorOnTileInSpace>();

        private readonly TileInSpace v_OnTile;

        #endregion

        #region Constructor

        /// <summary>
        /// Surface connector constructor
        /// </summary>
        /// <param name="tile">Tile on which is this connector placed.</param>
        public SurfaceConnector(TileInSpace tile)
        {
            v_OnTile = tile;
        }

        #endregion

        #region Public methods

        /// <returns>
        /// Enumerator of the tiles world.
        /// </returns>
        public IEnumerator<ConnectorOnTileInSpace> GetEnumerator() => v_Connectors.GetEnumerator();


        /// <returns>
        /// Enumerator of the tiles world.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <summary> 
        /// Connects this bi-directionally to a given connector.
        /// </summary>
        /// <param name="connector">Connector to which to connect.</param>
        public void ConnectTo(ConnectorOnTileInSpace connector)
        {
            if (v_OnTile == connector.OnTile)
                throw new ArgumentException($"Tile {v_OnTile.Name} cannot connect to itself.");

            // If v_OnTile is a polygon and product is +, then the object connects to the "in" side, otherwise to the "out" side
            var product = v_OnTile.Vertices.Normal.DotProduct(connector.OnTile.Position - v_OnTile.Position);
            var angle = Angle.FromRadians(Math.Sign(product));

            var freeConnector = v_Connectors.Find(conn => conn.ConnectedTo == null && conn.Angle == angle);
            if (freeConnector == null)
            {
                // As a position of the base connector we must pass an arbitrary position on Tile, not on TileInSpace
                // Hence cast (Tile) is needed
                var baseConnector = new Connector(v_OnTile.Name + " surface", new [] {((Tile)v_OnTile).Vertices[0]}, 
                    v_OnTile.SurfaceGlue, angle, connector.Resistance);
                freeConnector = new ConnectorOnTileInSpace(v_OnTile, baseConnector);
                v_Connectors.Add(freeConnector);
            }
            freeConnector.ConnectedTo = connector;
            connector.ConnectedTo = freeConnector;
        }

        /// <summary>
        /// Disonnects bi-directionally this connection from all connectors.
        /// </summary>
        public void DisconnectAll()
        {
            foreach (var connector in v_Connectors)
                connector.Disconnect();
        }

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String of object.</returns>
        public override string ToString()
        {
            return string.Join(";", v_Connectors);
        }

        #endregion
    }
}
