using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using MSystemSimulationEngine.Classes.Tools;
using MSystemSimulationEngine.Interfaces;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// A tile placed in space, positioned and rotated. 
    /// Its position and angle can change during the simulation (pushing by other objects)
    /// </summary>
    public class TileInSpace : Tile
    {
        #region Private data

        /// <summary>
        /// Instance counter to provide a unique instance ID.
        /// </summary>
        private static ulong v_Counter;

        #endregion

        #region Public data

        /// <summary>
        /// Debugging only
        /// </summary>
        public Vector3D xpushingVector { get; private set; }

        /// <summary>
        /// Unique ID of an instance.
        /// </summary>
        public readonly ulong ID;

        /// <summary>
        /// Position of the tile.
        /// </summary>
        public Point3D Position => Point3D.Centroid(Vertices);

        /// <summary>
        /// Holds vertices of the tile = readonly collection of immutable points.
        /// </summary>
        public new IPolytope Vertices { get; private set; }

        /// <summary> 
        /// Holds connectors of the tile in space.
        /// </summary>
        public new ReadOnlyCollection<ConnectorOnTileInSpace> Connectors { get; private set; }

        /// <summary> TODO low priority: join all protein classes, overload constructor
        /// Holds proteins of the tile.
        /// </summary>
        public new ReadOnlyCollection<ProteinOnTileInSpace> Proteins { get; private set; }

        /// <summary>
        /// Holds note used in case of mutation.
        /// </summary>
        public string Note { get; private set; }

        /// <summary>
        /// Holds surface connector of the tile.
        /// </summary>
        public readonly SurfaceConnector Surface;

        /// <summary>
        /// Quaternion representing rotation of the object.
        /// </summary>
        public Quaternion Quaternion { get; private set; } = Quaternion.One;

        /// <summary>
        /// Vector by which this object has to be pushed in one simulation step. Too small vectors are ignored - prevent runtiome erros.
        /// </summary>
        public Vector3D PushingVector {
            get { return _pushingVector; }
            private set { _pushingVector = value.Length <= float.Epsilon ? default : value; }
        }
        private Vector3D _pushingVector;

        /// <summary>
        /// State of the tile.
        /// </summary>
        public FState State = FState.Create;

        /// <summary>
        /// Holds the color of the tile in space. Can change in time.
        /// </summary>
        public new Color Color { get; private set; }

        /// <summary>
        /// Holds flag about color change.
        /// </summary>
        public bool ColorWasChanged;

        /// <summary>
        /// Defines whether tile is usable in rules or locked for changes.
        /// </summary>
        public bool IsLocked;

        /// <summary>
        /// Step in which the tile will be unlocked.
        /// </summary>
        public uint ReadyAtStep;

        /// <summary>
        /// Enum holding possible state of the object.
        /// </summary>
        public enum FState
        {
            Create, Move, Destroy, Unchanged
        }

        /// <summary>
        /// Connectors for cTAM pointing four world sides; some or all may be null.
        /// </summary>
        public readonly ConnectorOnTileInSpace NorthConnector, EastConnector, SouthConnector, WestConnector;

        #endregion

        #region Constructors

        /// <summary>
        /// Tile in space constructor. 
        /// </summary>
        /// <param name="baseObject">Base tile.</param>
        /// <param name="position">Position of the tile in space.</param>
        /// <param name="quaternion">Orientation of the tile.</param>
        public TileInSpace(Tile baseObject, Point3D position, Quaternion quaternion) : 
            base(baseObject.Name, baseObject.Vertices.ToList(), baseObject.Connectors.Select(conn => (Connector)conn),
            baseObject.SurfaceGlue, baseObject.Proteins, baseObject.Color, baseObject.Alpha, baseObject.AlphaRatio)
        {
            Vertices = baseObject.Vertices; // Must precede the connectors
            var connectors = baseObject.Connectors.Select(connector => new ConnectorOnTileInSpace(this, connector)).ToList();
            Connectors = new ReadOnlyCollection<ConnectorOnTileInSpace>(connectors);

            // Tile and the connectors are now in the basic position
            NorthConnector = Connectors.FirstOrDefault(c => c.Positions.All(p => p.Y > Position.Y));
            EastConnector = Connectors.FirstOrDefault(c => c.Positions.All(p => p.X > Position.X));
            SouthConnector = Connectors.FirstOrDefault(c => c.Positions.All(p => p.Y < Position.Y));
            WestConnector = Connectors.FirstOrDefault(c => c.Positions.All(p => p.X < Position.X));

            var proteins = base.Proteins.Select(protein => new ProteinOnTileInSpace(this, protein)).ToList();
            Proteins = new ReadOnlyCollection<ProteinOnTileInSpace>(proteins);

            Surface = new SurfaceConnector(this);
            Color = baseObject.Color;

            Color = baseObject.Color;

            RotateAndMove(quaternion, position.ToVector3D());
            ID = v_Counter++;
            Note = "";
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Moving or rotating a 3D point around Position of this tile.
        /// </summary>
        /// <param name="rotation">Rotation quaternion.</param>
        /// <param name="shift">Moving vector.</param>
        /// <param name="point">Point to be transformed.</param>
        private Point3D RotateAndMovePoint(Quaternion? rotation, Vector3D? shift, Point3D point)
        {
            if (rotation.HasValue)
                point = (point - Position.ToVector3D()).Rotate(rotation.Value) + Position.ToVector3D();

            if (shift.HasValue)
                point += shift.Value;

            return point;
        }

        /// <summary>
        /// Moving or rotating collection of points in space.
        /// </summary>
        /// <param name="rotation">Rotation quaternion.</param>
        /// <param name="vector">Moving vector.</param>
        /// <param name="points">Collection of points to be transformed.</param>
        private IList<Point3D> RotateAndMoveCollection(Quaternion? rotation, Vector3D? vector, IEnumerable<Point3D> points)
        {
            return points.Select(point => RotateAndMovePoint(rotation, vector, point)).ToList();
        }


        /// <summary>
        /// Moving and rotating the object including positions of vertices, connectors, proteins.
        /// </summary>
        /// <param name="rotation">Rotation quaternion.</param>
        /// <param name="vector">Moving vector.</param>
        private void RotateAndMove(Quaternion? rotation, Vector3D? vector)
        {
            Vertices = CreateVertices(RotateAndMoveCollection(rotation, vector, Vertices), Name);

            foreach (var connector in Connectors)
                connector.Positions = new ReadOnlyCollection<Point3D>(RotateAndMoveCollection(rotation, vector, connector.Positions));

            foreach (var protein in Proteins)
                protein.Position = RotateAndMovePoint(rotation, vector, protein.Position);

            Quaternion = rotation.HasValue ? (rotation.Value * Quaternion).Normalized : Quaternion;

            // If the object is newly created, its state stays "Create" even if it is moved
            if (State != FState.Create)
                State = FState.Move;
        }

        /// <summary>
        /// Rotates the object around 3 angles, including positions of vertices, connectors, proteins.
        /// </summary>
        /// <param name="rotation">Rotation quaternion.</param>
        private void Rotate(Quaternion rotation)
        {
            RotateAndMove(rotation, null);
        }

        /// <returns>
        /// The set of all tiles connected to this one, including self
        /// </returns>
        // XJB DEBUG
        //private HashSet<TileInSpace> Component()
        public HashSet<TileInSpace> Component()
        {
            var component = new HashSet<TileInSpace> { this };

            // we search all connected objects in the growing list until no new objects can be added
            for (int i = 0; i < component.Count; i++)
            {
                // for each active connection on the current object we add connected tiles 
                foreach (var connector in component.ElementAt(i).Connectors.Union(component.ElementAt(i).Surface)
                    .Where(connector => connector.ConnectedTo != null))
                    component.Add(connector.ConnectedTo.OnTile);
            }
            return component;
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Rotates the tile by a given angle, including positions of vertices, connectors, proteins.
        /// </summary>
        /// <param name="axis">Rotation axis.</param>
        /// <param name="angle">Rotation angle.</param>
        public void Rotate(UnitVector3D axis, Angle angle)
        {
            // Rotations += $"\n Q Before: {Quaternion.Print()}\n Axis: {axis}\n Angle: {angle.Degrees}\n"; // Debugging
            Rotate(Geometry.NewQuaternion(axis, angle));
            // Rotations += $"Q Rotate: {Geometry.NewQuaternion(axis, angle).Print()}\n Q After: {Quaternion.Print()}\n"; // Debugging
        }

        /// <summary>
        /// Moves the object by a vector, including positions of vertices, connectors, proteins.
        /// </summary>
        /// <param name="vector">Moving vector.</param>
        public void Move(Vector3D vector)
        {
            RotateAndMove(null, vector);
        }

        /// <summary>
        /// if this.PushingVector is smaller than "pushingVector" parameter, component of this object is returned 
        /// and all its members have set PushingVector = pushingVector as a SIDE EFFECT.
        /// Empty set is returned otherwise.
        /// </summary>
        public HashSet<TileInSpace> SetAndGetPushedComponent(Vector3D pushingVector)
        {
            if (pushingVector.Length <= Math.Max(this.PushingVector.Length, float.Epsilon))
             // Component is already pushed so no new pushing is needed
               return new HashSet<TileInSpace>();

            var component = Component();
            foreach (var obj in component)
                obj.PushingVector = pushingVector;
            return component;
        }

        /// <summary>
        /// Clears pushing vectors of this object.
        /// </summary>
        public void ClearPushing()
        {
            xpushingVector = PushingVector;
            PushingVector = default;
        }

        /// <summary>
        /// Calculates pushing of another object intersecting with this one.
        /// </summary>
        /// <param name="another">The tile to be pushed.</param>
        /// <param name="direction">Pushing direction.</param>
        /// <returns>Pushing vector of another object.</returns>
        public Vector3D PushingIntersected(TileInSpace another, UnitVector3D direction)
        {
            // Estimate the maximum length of both intersecting objects (lower bound)
            // Create vector of sum of their lengths in the pushing direction
            var box1 = new Box3D(another.Vertices);
            var box2 = new Box3D(this.Vertices);
            var falsePushingVector = ((box1.MaxCorner - box1.MinCorner).Length + (box2.MaxCorner - box2.MinCorner).Length) * direction;

            // Create false object moved by negation of the created vector as if it was to be pushed to its actual position
            // This false object surely does not intersects with the "another"
            IPolytope falseVertices = CreateVertices(RotateAndMoveCollection(null, - falsePushingVector, Vertices), Name + "-false");
            
            // The method "PushingOf" returns the actual pushing vector of the intersecting object
            return falseVertices.PushingOf(another.Vertices, falsePushingVector);
        }

        /// <summary>
        /// Calculates possible pushing of "another" object by this one, being itself 
        /// pushed by its "PushingVector" field which MAY cause its intersection with "another".
        /// If this object already intersects "another", its "PushingVector" is returned.
        /// </summary>
        /// <param name="another">The tile to be eventually pushed.</param>
        /// <returns>Pushing vector of another object.</returns>
        public Vector3D PushingNonIntersected(TileInSpace another)
        {
           return Vertices.PushingOf(another.Vertices, PushingVector);
        }

        
        /// <returns>
        /// True if an intersection of this and another tile exists.
        /// </returns>
        public bool IntersectsWith(TileInSpace another)
        {
           return Vertices.IntersectionsWith(another.Vertices).Any();
        }


        /// <returns>
        /// True if this and another tile are both polygons in the same plane and they overlap.
        /// </returns>
        public bool OverlapsWith(TileInSpace another)
        {
            var p1 = this.Vertices as Polygon3D;
            var p2 = another.Vertices as Polygon3D;
            return p1 != null && p2 != null && p1.OverlapsWith(p2);
        }

        /// <returns>
        /// True if this tile intersects or overlaps another tile. Optimized.
        /// </returns>
        public bool CollidesWith(TileInSpace another)
        {
            return Vertices.IntersectionsWith(another.Vertices).Any();
        }


        /// <summary>
        /// Gives the reference point close to a given position around which we search for / add / remove floating objects
        /// inside/outside this object. If the object is not 2D, the first argument is returned.
        /// </summary>
        /// <param name="position">Position on tiles (typically of connector or protein)</param>
        /// <param name="side">Side of the tile</param>
        /// <returns>Reference point corresponding to "inside"/"outside" parameter</returns>
        public Point3D SidePoint(Point3D position, Tile.SideType side)
        {
            Polygon3D polygon = Vertices as Polygon3D;
            return polygon?.SidePoint(position, side) ?? position;
        }


        /// <summary>
        /// Shortens a 2D object to a given length, starting at a given connector.
        /// </summary>
        /// <param name="length">New length of the object.</param>
        /// <param name="connector">Connector from which the new length is measured.</param>
        /// <exception cref="InvalidOperationException">If the object is not new 
        /// or its vertices is not Segment3D or the required length >= than the current one.</exception>
        public void ShortenTo(double length, ConnectorOnTileInSpace connector)
        {
            var segment = Vertices as Segment3D;
            if (State != FState.Create || segment == null)
                throw new InvalidOperationException($"{Name} is not a new 2D object and so it cannot be shortened.");
            if (length >= segment.Vector.Length)
                throw new ArgumentException($"Shortened length of object {Name} must be smaller than the current one.");

            Note = "mutated";
            int start = connector.Positions[0].MyEquals(Vertices[0]) ? 0 : 1;
            var newStartPoint = Vertices[start];
            var newEndPoint = newStartPoint + length *
                              (Vertices[1-start] - newStartPoint).Normalize();
            Vertices = new Segment3D(newStartPoint, newEndPoint, Name);

            // Move connectors at the cut-off end to the new end
            foreach (var conn in Connectors.Where(conn => conn.Positions[0].DistanceTo(newStartPoint) > length))
                conn.Positions = new ReadOnlyCollection<Point3D>(new[] {newEndPoint});

            // Move proteins at the cut-off end to the new end
            foreach (var prot in Proteins.Where(prot => prot.Position.DistanceTo(newStartPoint) > length))
                prot.Position = newEndPoint;
        }

        /// <summary>
        /// DO NOT USE OUTSIDE OF UNIT TESTS!!!
        /// Resets tiles in space counter.
        /// </summary>
        /// <remarks>ONLY FOR UNIT TESTS PURPOSE</remarks>
        public static void ResetCounter()
        {
            v_Counter = 0;
        }

        /// <summary>
        /// Sets new color of tile in space
        /// </summary>
        /// <param name="color">Color</param>
        public void SetNewColor(Color color)
        {
            ColorWasChanged = Color != color;
            Color = color;
        }

        /// <summary>
        /// Sets new color of tile in space
        /// </summary>
        /// <param name="colorName">Name of the color</param>
        public void SetNewColor(string colorName)
        {
            SetNewColor(Color.FromName(colorName));
        }

        /// <summary>
        /// Sets new color of tile in space
        /// </summary>
        /// <param name="alpha">Aplha</param>
        /// <param name="red">Red</param>
        /// <param name="green">Green</param>
        /// <param name="blue">Blue</param>
        public void SetNewColor(int alpha, int red, int green, int blue)
        {
            SetNewColor(Color.FromArgb(alpha, red, green, blue));
        }

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String of object.</returns>
        public override string ToString()
        {
            return $"TileInSpace: name = {Name}, ID = {ID}, position = {Position.Round()}, angle = {Quaternion.ToEulerAngles()}";
        }
        #endregion
    }
}
