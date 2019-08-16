using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using MSystemSimulationEngine.Classes.Tools;
using SharedComponents.Tools;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Represents connection class defining a connector on a tile in space.
    /// </summary>
    public class ConnectorOnTileInSpace : ConnectorOnTile
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
        public new readonly TileInSpace OnTile;

        /// <summary>
        /// Positions of the connector on the tile
        /// </summary>
        public new ReadOnlyCollection<Point3D> Positions
        {
            get
            {
                return v_Positions;
            }
            set
            {
                CheckPositions(value, OnTile.Vertices);
                v_Positions = value;
            }
        }

        /// <summary>
        /// Fake tile used to enhance color of the connector in Unity.
        /// </summary>
        public Color EmphColor { get
        {
//            var hue = OnTile.Color.GetHue();
//            var saturation = OnTile.Color.GetSaturation();
            var brigthness = OnTile.Color.GetBrightness();
            //saturation = saturation > 0.5 ? saturation/2 : saturation*2;
              
            return brigthness > 0.2 ? Color.Black : Color.White;  
            // return MyColor.FromAhsb(255, hue, saturation, brigthness);
        } }

        /// <summary>
        /// Flag indicating that the connector is marked to disconnect in this step.
        /// </summary>
        public bool SetDisconnect;

        /// <summary>
        /// To where it is connected?
        /// </summary>
        public ConnectorOnTileInSpace ConnectedTo;

        /// <summary>
        /// To where it was previously connected?
        /// </summary>
        public ConnectorOnTileInSpace WasConnectedTo { get; private set; }

        /// <summary>
        /// Was the connector displayed in emphasized color?
        /// </summary>
        public bool WasEmphasized = false;

        /// <summary>
        /// Voltage of the connector used in cTAM
        /// </summary>
        public double Voltage;

        #endregion

        #region Constructor

        /// <summary>
        /// ConnectorOnTileInSpace constructor
        /// </summary>
        /// <param name="tile">Tile on which is this connector placed.</param>
        /// <param name="connector">Base connector from which this connector derives.</param>
        public ConnectorOnTileInSpace(TileInSpace tile, Connector connector) 
            : base(tile, connector)
        {
            OnTile = tile;
            v_Positions = connector.Positions;  // assignment to Positions would cause double  check of positions on tile
        }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        /// <summary>
        /// True if (vertices of) both connectors overlap within "tolerance".
        /// </summary>
        public bool Overlaps(ConnectorOnTileInSpace another, double tolerance = MSystem.Tolerance)
        {
            return Geometry.Overlap(Positions, another.Positions, tolerance); 
        }


        /// <summary>
        /// Connects bi-directionally this connector to another.
        /// </summary>
        /// <param name="connector">Connector to which to connect.</param>
        public void ConnectTo(ConnectorOnTileInSpace connector)
        {
            if (this.OnTile == connector.OnTile)
            {
                throw new ArgumentException($"Tile {OnTile.Name} cannot connect to itself.");
            }
            ConnectedTo = connector;
            connector.ConnectedTo = this;
        }

        /// <summary>
        /// Disonnects bi-directionally this connection, resets the flag "SetDisconect".
        /// </summary>
        public void Disconnect()
        {
            if (ConnectedTo != null)
            {
                ConnectedTo.ConnectedTo = null;
                ConnectedTo.WasConnectedTo = this;
                ConnectedTo.SetDisconnect = false;
                WasConnectedTo = ConnectedTo;
                ConnectedTo = null;
            }
            SetDisconnect = false;
        }

        /// <summary>
        /// Returns the reference point around which we search for / add / remove floating objects close to this connector. 
        /// The point is determined by the "Side" field of the connector and by its eventual connection to another tile.
        /// </summary>
        public Point3D SidePoint(Point3D position)
        {
            if (Side == Tile.SideType.undef && ConnectedTo != null && ConnectedTo.Side != Tile.SideType.undef)
                    return ConnectedTo.SidePoint(position);
            
            return OnTile.SidePoint(position, Side);
        }

        /// <summary>
        /// Calculates position and angle of a new tile connecting to this connection and connects it.
        /// The connector given as parameter determines also the connecting object.
        /// </summary>
        /// <param name="connector">Connector on the new tile by which it connects</param>
        /// <returns>New tile in space connected to this connection.</returns>
        public TileInSpace ConnectObject(ConnectorOnTile connector)
        {
            // Place the old and the new object into the same plane or line
            TileInSpace newTile = new TileInSpace(connector.OnTile, Point3D.Origin, OnTile.Quaternion);
            int index = connector.OnTile.Connectors.IndexOf(connector);
            ConnectorOnTileInSpace newConnector = newTile.Connectors[index];

            UnitVector3D axis = OnTile.Vertices.Normal;   // rotation axis - TRY REVERSE IN CASE OF MALFUNCTION

            // Edge-to-edge
            if (newConnector.Positions.Count == 2 && Positions.Count == 2)
            {
                // Rotate the new object so that connector directions are opposite
                Vector3D vector1 = Positions[1] - Positions[0];
                Vector3D vector2 = newConnector.Positions[0] - newConnector.Positions[1];
                Angle angle = vector2.SignedAngleTo(vector1, axis);
                newTile.Rotate(axis, angle);

                // Rotate the new tile around the connector edge so that 
                // the angle between both tiles is the angle associated with the connector
                newTile.Rotate(vector1.Normalize(), Angle.FromDegrees(180)-Angle);    // TRY REVERSE IN CASE OF MALFUNCTION

                // Move the new object so that the connectors match
                newTile.Move(Positions[0] - newConnector.Positions[1]);
            }
            // Point-to-point
            else if (newConnector.Positions.Count == 1 && Positions.Count == 1)
            {
                // Rotate the new object around an axis perpendicular to it by the angle associated with the connector

                var newSegment = newTile.Vertices as Segment3D;
                var thisSegment = OnTile.Vertices as Segment3D;
                var thisAxis = thisSegment?.Vector.Normalize() ?? OnTile.Vertices.Normal;

                if (newSegment == null)
                {
                    // Tile connecting to a segment
                    if (thisSegment != null)
                        axis = newTile.Vertices.Normal.CrossProduct(-Math.Sign(connector.Angle.Radians)* thisSegment.Vector).Normalize();
                    // ELSE 2D tile connecting to a 2D tile by a single point - should never happen
                }
                else
                {  
                    if (OnTile.Vertices is Polygon3D) // Segment connecting to a tile
                        axis = axis.CrossProduct(- newSegment.Vector).Normalize();
                    // ELSE segment connecting to a segment - axis set above as a normal to this segment
                }
                newTile.Rotate(axis, Angle);    // TRY REVERSE AXIS OR ROTATION IN CASE OF MALFUNCTION
                                                
                // the newly connected object has one degree of freedom - rotate randomly
                newTile.Rotate(thisAxis, Angle.FromRadians(Randomizer.NextDoubleBetween(0, 2*Math.PI)));
                // Move the new object so that the connectors match
                newTile.Move(Positions[0] - newConnector.Positions[0]);
            }
            else
            {
                throw new ArgumentException("Connecting incompatible connectors:" + this +"\n and" + newConnector);
            }

            ConnectTo(newConnector);
            return newTile;
        }


        #endregion
    }
}
