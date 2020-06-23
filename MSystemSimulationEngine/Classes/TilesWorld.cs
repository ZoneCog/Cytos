using MathNet.Spatial.Euclidean;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// The world of tiles during simulation.
    /// Tiles in space together with their connections form a graph structure.
    /// If any object is pushed, it carries the whole part of the graph interconnected with it.
    /// </summary>
    public class TilesWorld : IEnumerable<TileInSpace>
    {
        #region Private data

        private readonly HashSet<TileInSpace> v_TileSet = new HashSet<TileInSpace>();

        private readonly HashSet<TileInSpace> v_PolygonTiles = new HashSet<TileInSpace>();

        /// <summary>
        /// M System backing this tiles world
        /// </summary>
        private readonly MSystem v_MSystem;

        /// <summary>
        /// A reference to the world of all floating objects during simulation. Use FltObjectsWorld instead.
        /// </summary>
        private FloatingObjectsWorld v_FltObjectsWorld;

        #endregion

        #region Public data

        public IEnumerable<TileInSpace> PolygonTiles => v_PolygonTiles;

        /// <summary>
        /// DO NOT SET outside the class SimulationWorld.
        /// </summary>
        public FloatingObjectsWorld FltObjectsWorld
        {
            private get
            {
                if (v_FltObjectsWorld == null)
                    throw new InvalidOperationException("The reference to floating objects world was not set.");
                return v_FltObjectsWorld;
            }
            set
            {
                if (value == null)
                    throw new InvalidOperationException("The reference to floating objects world cannot be null.");
                v_FltObjectsWorld = value;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor initializing the world with a collection of seed tiles.
        /// DO NOT USE outside the class SimulationWorld.
        /// </summary>
        public TilesWorld(MSystem mSystem)
        {
            if (mSystem == null)
                throw new ArgumentNullException($"Parameter \'mSystem\' cannot be null.");

            v_MSystem = mSystem;

            foreach (var seedTile in mSystem.SeedTiles)
                _Add(seedTile);
        }
        #endregion

        #region Private methods

        /// <summary>
        /// The only safe method to add tiles to the world
        /// </summary>
        private void _Add(TileInSpace tile)
        {
            v_TileSet.Add(tile);
            if (tile.Vertices is Polygon3D)
                v_PolygonTiles.Add(tile);
        }

        /// <summary>
        /// The only safe method to remove tiles from the world
        /// </summary>
        private bool _Remove(TileInSpace tile)
        {
            v_PolygonTiles.Remove(tile);    // If tile is a segment, nothing happens
            return v_TileSet.Remove(tile);

        }

        /// <summary>
        /// Applies pushing to tiles in the passed list.
        /// Applies pushing to floating object pushed by these tiles.
        /// TODO for each component containing tiles: calculate pushing vectors by tiles growing from them.
        /// Calculate their common (average) pushing veector.
        /// If its scalar product with all particular pushing vectors is positive, then try to apply the average vector
        /// to all objects pushed by these interconnected growing tiles.
        /// </summary>
        private void ApplyPushing(IEnumerable<TileInSpace> pushedTiles)
        {
            var pushedFltObjects = new Dictionary<FloatingObjectInSpace, Vector3D>();
            foreach (var tile in pushedTiles)
            {
                var polygon = tile.Vertices as Polygon3D;
                // Only polygon pushed outside his own plane can push floating objects 
                if (polygon != null && !tile.PushingVector.IsPerpendicularTo(polygon.Normal))
                {
                    var checkedSet = FltObjectsWorld.GetObjectsInPrism(polygon, tile.PushingVector);
                    foreach (var fltObj in checkedSet)
                    {
                        // Vector between the current position of the floating object and the pushed position
                        var pushingVector = fltObj.Position
                            - polygon.Plane.IntersectionWith(new Ray3D(fltObj.Position, tile.PushingVector));

                        // Extra safe distance of floating objects from the closest tile
                        pushingVector += MSystem.SideDist * Math.Sign(polygon.Normal.DotProduct(tile.PushingVector)) * polygon.Normal;

                        if (!pushedFltObjects.ContainsKey(fltObj) || pushingVector.Length > pushedFltObjects[fltObj].Length)
                            pushedFltObjects[fltObj] = pushingVector;
                    }
                }
                tile.Move(tile.PushingVector);
                FltObjectsWorld.ExpandWith(new Box3D(tile.Vertices), v_MSystem.RefillEnvironment);
            }

            foreach (var fltObj in pushedFltObjects)
                v_FltObjectsWorld.Move(fltObj.Key, fltObj.Value);
        }

        /// <summary>
        /// Clears pushing vectors in all tiles in the world.
        /// </summary>
        private void ClearPushing()
        {
            foreach (var obj in v_TileSet)
                obj.ClearPushing();
        }

        /// <summary>
        /// Establishes all possible bi-directional connections of "newObject" with existing objects.
        /// </summary>
        /// <param name="newObject">A tile being newly added to the world.</param>
        private void AutoConnect(TileInSpace newObject)
        {
            var freeConnectors = v_TileSet.Where(obj => obj != newObject && obj.State != TileInSpace.FState.Destroy)
                .SelectMany(obj => obj.Connectors.Where(connector => connector.ConnectedTo == null));

            foreach (var conn2 in freeConnectors)
                foreach (var conn1 in newObject.Connectors)
                    if (conn1.ConnectedTo == null
                        && (v_MSystem.AreCompatible(conn1, conn2) || v_MSystem.AreCompatible(conn2, conn1))
                        && conn1.Overlaps(conn2, v_MSystem.GlueRadius))
                    {
                        conn2.ConnectTo(conn1);
                        ReleaseSignalObjects(conn1);
                        break;
                    }

            if (newObject.Vertices is Segment3D)
                foreach (var conn in newObject.Connectors.Where(conn => conn.ConnectedTo == null))
                    foreach (var obj in v_TileSet)
                    {
                        var point = conn.Positions[0];
                        if (obj.State != TileInSpace.FState.Destroy &&
                            v_MSystem.GlueRelation.ContainsKey(Tuple.Create(conn.Glue, obj.SurfaceGlue)))
                            if ((obj.Vertices as Polygon3D)?.ContainsPoint(point, 0, v_MSystem.GlueRadius) ??
                                (obj.Vertices as Segment3D)?.Edges[0].ClosestPointTo(point, true).DistanceTo(point) <= v_MSystem.GlueRadius)
                            {
                                obj.Surface.ConnectTo(conn);
                                ReleaseSignalObjects(conn);
                                break;
                            }
                    }
        }

        /// <summary>
        /// Tries to attach a tile in space to the world due to space constraints.
        /// Resolves pushing of fixed and floating objects.
        /// </summary>
        /// <param name="connector">Connector on the new tile by which it is attached to an existing one.</param>
        /// <returns>
        /// True if the given tile in space can be attached to the world.
        /// </returns>
        private bool Attach(ConnectorOnTileInSpace connector)
        {
            var pushingDirection = PushingDirection(connector);

            /* Randomize - up to phi/4 deviation - a parameter of randomization angle would have to be added to the input XML
            Vector3D randomVector = new Vector3D(Randomizer.Rng.NextDouble(), Randomizer.Rng.NextDouble(),
                Randomizer.Rng.NextDouble());
            var length = randomVector.Length;
            if (length > 0.001)
                randomVector = randomVector / (2 * length);     // Length is 0.5 now
            pushingDirection = (pushingDirection + randomVector).Normalize(); */

            Vector3D maxPushingVector = default;               // This will be the longest of all pushing vectors in the world
            var newTile = connector.OnTile;

            var objectsToBeChecked = new HashSet<TileInSpace>();  // Objects to be checked for pushing by newObject
            var objectsToBePushed = new HashSet<TileInSpace>();   // Objects which passed the check and will be pushed

            // Check whether the newObject intersects existing ones, and add those to pushingList 
            // together with their components
            foreach (var checkedObject in v_TileSet)       // TODO PARALLEL.FOREACH
                if (newTile.IntersectsWith(checkedObject) || newTile.OverlapsWith(checkedObject))
                {
                    var pushingVector = newTile.PushingIntersected(checkedObject, pushingDirection);
                    if (newTile.Vertices is Polygon3D)
                        pushingVector = v_MSystem.PushingCoef * pushingVector;

                    if (pushingVector.Length > float.Epsilon)
//                        lock(objectsToBeChecked)
                        {
                            objectsToBeChecked.UnionWith(checkedObject.SetAndGetPushedComponent(pushingVector));
                            if (pushingVector.Length > maxPushingVector.Length)
                                maxPushingVector = pushingVector;
                        }
                }

            var invalidPushing = newTile.PushingVector != default;

            // Check all objects in checkList whether they can be pushed, as they may in turn push other objects
            while (objectsToBeChecked.Any() && !invalidPushing)
            {
                var checkedObject = objectsToBeChecked.First();

                foreach (var existingObject in v_TileSet)       // TODO PARALLEL.FOREACH
                    if (existingObject != checkedObject)
                    // checked object is not checked against itself
                    {
                        var pushingVector = checkedObject.PushingNonIntersected(existingObject);  
                        // Secondary pushing
                        // Fact: pushingVector <= checkedObject.PushingVector; 
                        // hence any object with maximum pushing vector in checkList is never returned back to the list.
                        // This guarantees that the *while* cycle will eventually end

                        if (pushingVector.Length > float.Epsilon)
                        //                        lock(objectsToBeChecked)
                        {
                            objectsToBeChecked.UnionWith(existingObject.SetAndGetPushedComponent(pushingVector));
                        }
                    }
                objectsToBeChecked.Remove(checkedObject);
                objectsToBePushed.Add(checkedObject);
                invalidPushing |= newTile.PushingVector != default;
            }

            // If any object after pushing still intersects with newObjectm, then the pushing is unsuccessful.
            foreach (var pushedObject in v_TileSet.Where(obj => obj.PushingVector != default))
            {
                if (invalidPushing)     // No further cycling needed
                    break;

                // Push the object for intersection test
                var oldState = pushedObject.State;
                pushedObject.Move(pushedObject.PushingVector);
                invalidPushing |= newTile.IntersectsWith(pushedObject) || newTile.OverlapsWith(pushedObject);
                // Revert back
                pushedObject.Move(-pushedObject.PushingVector);
                pushedObject.State = oldState;
            }

            // If newObject == segment has not enough room, it can be shortened
            if (invalidPushing)
            {
                var segment = newTile.Vertices as Segment3D;
                var newLength = Math.Max(0D, segment?.Vector.Length - maxPushingVector.Length ?? 0D);
                if (newLength < MSystem.Tolerance)
                {
                    // newObject not attached = there is just a tiny room, shortening would not help
                    ClearPushing();
                    return false;
                }
                newTile.ShortenTo(newLength, connector);
            }
            else
                // Pushing possible => it is applied now
                ApplyPushing(objectsToBePushed);

            ClearPushing();
            return true;
        }


        /// <summary>
        /// Tries to insert a tile between two existing conected tiles due to space constraints.
        /// Resolves pushing of fixed and floating objects.
        /// TODO at the moment allows only insertion of rods, generalize also for 2D tiles
        /// </summary>
        /// <param name="connectorsOnTile">Connectors on the new tile by which it should be attached.</param>
        /// <param name="connector1">COnnector on an existing tile at which the new tile is inserted</param>
        /// <returns>
        /// True if the given tile in space can be inserted to the world.
        /// </returns>
        private bool _Insert(Tuple<ConnectorOnTile, ConnectorOnTile> connectorsOnTile, ConnectorOnTileInSpace connector1)
        {
            var newTile = connectorsOnTile.Item1.OnTile;    // TODO create new tile by "ConnectObject(olderConnector)"
            var connector2 = connector1.ConnectedTo;
            var olderConnector = (connector1.OnTile.ID < connector2.OnTile.ID ? connector1 : connector2);
            var pushingVector = newTile.Vertices[1] - newTile.Vertices[0];     // TODO correct

            // TODO FINISH
            return false;
        }



        /// <summary>
        /// Releases signal objects emited by a newly established connection to the floating objects world.
        /// </summary>
        ///  <param name="connector">Old connector to which a new connector was attached.</param>
        private void ReleaseSignalObjects(ConnectorOnTileInSpace connector)
        {
            NamedMultiset signalObjects;
            if (v_MSystem.GlueRelation.TryGetValue(Tuple.Create(connector.Glue,
                connector.ConnectedTo.Glue), out signalObjects))
            {
                v_FltObjectsWorld.AddAt(signalObjects, connector.SidePoint(connector.Positions.First()));
            }
        }


        #endregion

        #region Public methods

        /// <returns>
        /// Enumerator of the tiles world.
        /// </returns>
        public IEnumerator<TileInSpace> GetEnumerator() => v_TileSet.GetEnumerator();


        /// <returns>
        /// Enumerator of the tiles world.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <summary> 
        /// Calculates the direction in which this growing tile pushes others.
        /// </summary>
        /// <param name="connector">Connector from which this object grows.</param>
        /// <returns>Pushing direction.</returns>
        public static UnitVector3D PushingDirection(ConnectorOnTileInSpace connector)
        {
            if (connector.ConnectedTo == null)
            {
                throw new ArgumentException("Pushing direction for unconnected connector is undefined");
            }
            var tile = connector.OnTile;
            // Point-to-point connection of a segment - pushing in the segment direction
            if (tile.Vertices is Segment3D)
                return (tile.Position - connector.Positions[0]).Normalize();

            // Edge-to-edge connection of a polygon - pushing from connector to the object's center
            if (connector.Positions.Count == 2)
                return (new Line3D(connector.Positions[0], connector.Positions[1])).
                    LineTo(tile.Position, false).Direction;

            return (connector.Positions[0] - connector.ConnectedTo.OnTile.Position).Normalize();
        }


        /// <summary>
        /// Checks whether a given line intersects any 2D tile in the world.
        /// </summary>
        /// <param name="p1">Line startpoint.</param>
        /// <param name="p2">Line endpoint.</param>
        /// <param name="includeEndpoints">Consider also intersections in endpoints p1, p2 of the line?</param>
        /// <returns>True if an intersection exists.</returns>
        public bool IntersectsWith(Point3D p1, Point3D p2, bool includeEndpoints = true)
        {
            return v_PolygonTiles.Any(tile => ((Polygon3D)tile.Vertices).IntersectsWith(p1, p2, 0, includeEndpoints));
        }


        /// <summary>
        /// Checks whether the given 2D tile in the world is close to another 2D object at the given position and side
        /// so that the space between them is narrow (smaller than MinFaceDist).
        /// Returns false for a non-2D tile.
        /// TODO this will be probably changed as Max wants floating object to have a volume
        /// </summary>
        /// <param name="position">Position on tiles (typically of connector or protein).</param>
        /// <param name="tile">Tile which is tested.</param>
        /// <param name="inside">Consider inside (true) or outside (false) face of the tile.</param>
        public bool InNarrowSpace(Point3D position, TileInSpace tile, bool inside)
        {
            var polygon = tile.Vertices as Polygon3D;
            return polygon != null &&
                IntersectsWith(position, position + MSystem.MinFaceDist * (inside ? 1 : -1) * polygon.Normal, false);
        }


        /// <summary>
        /// Tries to add a new tile to a given connection on an existing tile.
        /// Tries possible various connectors on the new object => various orientations of the new object.
        /// Eventually releases the signal objects corresponding to the new connection.
        /// </summary>
        /// <param name="tile">New tile to be attached.</param>
        /// <param name="freeConnector">Connector to which the new tile should be attached.</param>
        /// <param name="newlyCreatedTile">Out parameter with newly created tile in space.</param>
        /// <returns>
        /// True if the tile was attached to the given connection.
        /// </returns>
        public bool Add(Tile tile, ConnectorOnTileInSpace freeConnector, out TileInSpace newlyCreatedTile)
        {
            if (tile == null)
                throw new ArgumentNullException($"Parameter \'tile\' cannot be null");
            if (freeConnector == null)
                throw new ArgumentNullException($"Parameter \'freeConnector\' cannot be null");

            // already connected connector cannot be used
            if (freeConnector.ConnectedTo != null)
                throw new ArgumentException($"Parameter \'freeConnector\' is already connected");

            foreach (var connector in tile.Connectors.Where(connector => v_MSystem.AreCompatible(freeConnector, connector)))
            {
                freeConnector.ConnectObject(connector);
                if (Attach(freeConnector.ConnectedTo))
                {
                    TileInSpace newTile = freeConnector.ConnectedTo.OnTile;
                    _Add(newTile);
                    AutoConnect(newTile);
                    FltObjectsWorld.ExpandWith(new Box3D(newTile.Vertices), v_MSystem.RefillEnvironment);
                    ReleaseSignalObjects(freeConnector);

                    newlyCreatedTile = newTile;
                    return true;
                }
                freeConnector.Disconnect();
            }

            newlyCreatedTile = null;
            return false;
        }


        /// <summary>
        /// Tries to insert a new tile between two existing conected tiles at a given connection.
        /// TODO at the moment allows only insertion of rods, generalize also for 2D tiles
        /// Eventually releases the signal objects corresponding to the new connection.
        /// </summary>
        /// <param name="tile">New tile to be inserted.</param>
        /// <param name="connector1">Connector at which the new tile should be inserted.</param>
        /// <returns>
        /// True if the tile was inserted at the given connection.
        /// </returns>
        public bool Insert(Tile tile, ConnectorOnTileInSpace connector1)
        {
            if (tile == null)
                throw new ArgumentNullException($"Parameter \'tile\' cannot be null");
            if (connector1 == null)
                throw new ArgumentNullException($"Parameter \'connector1\' cannot be null");

            if (!(tile.Vertices is Segment3D))
                throw new ArgumentException($"Parameter \'tile\' must be a 2D rod");
            var connector2 = connector1.ConnectedTo;
            if (connector2 == null)
                throw new ArgumentException($"Parameter \'connector1\' is not connected");

            var connectorsOnTile = v_MSystem.MatchingConectors(tile, connector1.Glue, connector2.Glue);
            if (_Insert(connectorsOnTile, connector1))
            {
                var newTile = connector1.ConnectedTo.OnTile;
                _Add(newTile);
                AutoConnect(newTile);
                ReleaseSignalObjects(connector1);
                ReleaseSignalObjects(connector2);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Removes the given tile from the world.
        /// Releases all connectors pointing to this tile.
        /// </summary>
        /// <param name="tile">Tile to be removed.</param>
        /// <exception cref="ArgumentException">If the object could not be removed.</exception>
        public void Remove(TileInSpace tile)
        {
            if (tile == null)
                throw new ArgumentNullException($"Parameter \'tile\' cannot be null");

            foreach (var connector in tile.Connectors)
                connector.Disconnect();
            tile.Surface.DisconnectAll();
            if (!_Remove(tile))
                throw new ArgumentException($"Tile {tile.Name} could not be removed from the world!");

        }

        /// <summary>
        /// Gets copy of tiles in space.
        /// </summary>
        /// <returns>
        /// Hashset of tiles in space.
        /// </returns>
        public HashSet<TileInSpace> GetCopyOfTilesInWorld()
        {
            TileInSpace[] copyOfTiles = new TileInSpace[v_TileSet.Count];
            v_TileSet.CopyTo(copyOfTiles);

            return new HashSet<TileInSpace>(copyOfTiles);
        }

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String of object.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Tiles in the world: \n{0}", String.Join("\n", v_TileSet));
            return builder.ToString();
        }

        #endregion
    }
}
