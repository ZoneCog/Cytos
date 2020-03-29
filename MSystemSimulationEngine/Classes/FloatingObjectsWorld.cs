using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using MathNet.Spatial.Euclidean;
using MSystemSimulationEngine.Classes.Tools;
using SharedComponents.Tools;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Contains methods splitting 3D space into a grid of keyed cubes containing floating objects.
    /// Provides enumeration of internal keys to the part of data grid covered by the underlying box.
    /// TODO LOW PRIORITY: make this a public class in a separate file not implementing IEnumebrable. 
    /// TODO include an internal class implementing IEnumebrable. Publish it as a result of a method whose argument will be scanbox.
    /// </summary>
    internal class FloatingObjectsWorldKeys : IEnumerable<ulong>
    {
        private readonly double v_CubeSize;
        private Box3D v_ScanBox;

        internal void SetBox(Box3D newBox) => v_ScanBox = newBox;

        #region Constructor

        ///  <summary>
        ///  Constructor of the floating objects grid keys collection.
        ///  </summary>
        ///  <param name="cubeSize">Grid cube size</param>
        internal FloatingObjectsWorldKeys(double cubeSize)
        {
            v_CubeSize = cubeSize;
            v_ScanBox = new Box3D(null);
        }

        #endregion

        /// <summary>
        /// Gets the grid key component in one dimension.
        /// </summary>
        /// <returns>Grid key component.</returns>
        private short CoordinateToKey(double coordinate)
        {
            return Convert.ToInt16(coordinate / v_CubeSize);
        }

        /// <summary>
        /// Calculates the grid key from its three components.
        /// </summary>
        /// <param name="keyX">X-component of the key.</param>
        /// <param name="keyY">Y-component of the key.</param>
        /// <param name="keyZ">Z-component of the key.</param>
        /// <returns>Grid key.</returns>
        private ulong ComponentsToKey(short keyX, short keyY, short keyZ)
        {
            unchecked
            {
                return ((((ulong)(ushort)keyZ << 16) | (ushort)keyY) << 16) | (ushort)keyX;
            }
        }

        /// <summary>
        /// Calculates coordinates of the center of a grid cube specified by its compoment keys.
        /// </summary>
        /// <param name="keyX">X-component of the key.</param>
        /// <param name="keyY">Y-component of the key.</param>
        /// <param name="keyZ">Z-component of the key.</param>
        /// <returns>Center point of a grid cube specified by its compoment keys.</returns>
        private Point3D KeysToPosition(short keyX, short keyY, short keyZ)
        {
            return new Point3D(keyX * v_CubeSize, keyY * v_CubeSize, keyZ * v_CubeSize);
        }

        /// <summary>
        /// Calculates coordinates of the grid cube corresponding to the given key.
        /// </summary>
        /// <param name="key">Grid key.</param>
        /// <returns>Grid cube specified by its key.</returns>
        internal Box3D KeyToBox(ulong key)
        {
            Point3D center;
            unchecked
            {
                center = KeysToPosition((short)(key & 0xffff),
                    (short)((key >> 16) & 0xffff),
                    (short)((key >> 32) & 0xffff));

            }
            Vector3D diagonal = new Vector3D(v_CubeSize, v_CubeSize, v_CubeSize);
            return new Box3D(center - diagonal / 2, center + diagonal / 2);
        }

        /// <summary>
        /// Gets the grid key for a given position.
        /// </summary>
        /// <param name="position">A position for which the key is calculated.</param>
        /// <returns>Grid key.</returns>
        internal ulong PositionToKey(Point3D position)
        {
            return ComponentsToKey(CoordinateToKey(position.X), CoordinateToKey(position.Y), CoordinateToKey(position.Z));
        }


        /// <summary>
        /// Enumerator of the part of grid given by this Box3D.
        /// </summary>
        /// <returns>Next grid key.</returns>
        public IEnumerator<ulong> GetEnumerator()
        {
            for (short x = CoordinateToKey(v_ScanBox.MinCorner.X); x <= CoordinateToKey(v_ScanBox.MaxCorner.X); x++)
                for (short y = CoordinateToKey(v_ScanBox.MinCorner.Y); y <= CoordinateToKey(v_ScanBox.MaxCorner.Y); y++)
                    for (short z = CoordinateToKey(v_ScanBox.MinCorner.Z); z <= CoordinateToKey(v_ScanBox.MaxCorner.Z); z++)
                        yield return ComponentsToKey(x, y, z);
        }

        /// <summary>
        /// Enumerator of (part of) the grid.
        /// </summary>
        /// <returns>Enumerator of the grid.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    /// <summary>
    /// World of all floating objects of a P system in 3D space.
    /// Split into cubic grid for performance purposes.
    /// </summary>
    public class FloatingObjectsWorld
    {
        // Wrapper class: positions of floating objects in the world can be set only internally, NOT externally!
        private class FloatingObjectInWorld : FloatingObjectInSpace
        {
            internal FloatingObjectInWorld(FloatingObject type, Point3D position) : base(type, position) { }

            internal new Point3D Position
            {
                get { return base.Position; }
                set { base.Position = value; }
            }
        }

        /// <summary>
        /// Collection of floating objects in one bar of a spatial grid for simulation. 
        /// Distinguishes old and newly created objects in a simulation step.
        /// New objects cannot be re-used in the same simulation step.
        /// </summary>
        private class FloatingObjectsBar
        {
            internal readonly ConcurrentHashSet<FloatingObjectInWorld> OldSet = new ConcurrentHashSet<FloatingObjectInWorld>();
            internal readonly ConcurrentHashSet<FloatingObjectInWorld> NewSet = new ConcurrentHashSet<FloatingObjectInWorld>();
        }

        #region Private data

        /// <summary>
        /// A frequently used constant to optimize the performance.
        /// </summary>
        private static double b = Math.Sqrt(8 / Math.PI);

        /// <summary>
        /// Vector extending the bounding box of the collection of all tiles in the world 
        /// to the overall box of the floating objects world.
        /// </summary>
        private readonly Vector3D v_BoundaryVector;

        /// <summary>
        /// Basic data structure - spatial grid with floating objects
        /// </summary>
        private readonly Dictionary<ulong, FloatingObjectsBar> v_Grid;

        /// <summary>
        /// A box holding the actual size of the world - must be entirely covered with the v_Grid
        /// </summary>
        private Box3D v_WorldBox;

        /// <summary>
        /// Copy of the v_WorldBox for unit testing only
        /// </summary>
        public Box3D GetBox => new Box3D(v_WorldBox.MinCorner, v_WorldBox.MaxCorner);

        /// <summary>
        /// Maps grid keys to spatial coordinates and back.
        /// Never use directly to enumerate keys. Use KeysInBox instead.
        /// </summary>
        private readonly FloatingObjectsWorldKeys v_GridKeys;

        /// <summary>
        /// P System backing this floating objects world
        /// </summary>
        private readonly MSystem v_MSystem;

        /// <summary>
        /// Holds the graph of tiles of the P system during simulation.
        /// </summary>
        private readonly TilesWorld v_tilesWorld;

        #endregion

        #region Constructor

        ///  <summary>
        ///  Constructor of the floating objects world.
        /// DO NOT USE outside the class SimulationWorld.
        ///  </summary>
        ///  <param name="mSystem">Simulated M system.</param>
        /// <param name="tilesWorld">World of tiles present initially in the P system</param>ra
        public FloatingObjectsWorld(MSystem mSystem, TilesWorld tilesWorld)
        {
            v_MSystem = mSystem;
            v_tilesWorld = tilesWorld;

            // Degree of granularity of the simulation space - objects are collected in these cubes
            // The constant 2.02 guarantees that each "floating object mobility" ball in space fits in
            // at most 2x2x2 grid cubes, plus 1% reserve.
            var cubeSize = mSystem.Mobility * 2.02;
            v_BoundaryVector = new Vector3D(cubeSize, cubeSize, cubeSize);

            v_Grid = new Dictionary<ulong, FloatingObjectsBar>();

            // Calculate a bounding box around the seed tiles
            // THESE THREE COMMANDS MUST BE IN THIS ORDER!!!
            v_WorldBox = new Box3D(v_tilesWorld.SelectMany(tile => tile.Vertices));
            v_GridKeys = new FloatingObjectsWorldKeys(cubeSize);
            ExpandWith(v_WorldBox);
        }

        #endregion

        #region Private methods

        /// <summary> 
        /// Returns IEnumerable of grid keys within given box.
        /// </summary>
        /// <param name="box"></param>
        private FloatingObjectsWorldKeys KeysInBox(Box3D box)
        {
            var minCorner = new Point3D(
                Math.Max(box.MinCorner.X, v_WorldBox.MinCorner.X),
                Math.Max(box.MinCorner.Y, v_WorldBox.MinCorner.Y),
                Math.Max(box.MinCorner.Z, v_WorldBox.MinCorner.Z));
            var maxCorner = new Point3D(
                Math.Min(box.MaxCorner.X, v_WorldBox.MaxCorner.X),
                Math.Min(box.MaxCorner.Y, v_WorldBox.MaxCorner.Y),
                Math.Min(box.MaxCorner.Z, v_WorldBox.MaxCorner.Z));

            v_GridKeys.SetBox(minCorner.IsLeq(maxCorner) ? new Box3D(minCorner, maxCorner) : new Box3D(null));
            return v_GridKeys;
        }

        /// <summary>
        /// Fills the given box with a random distribution of environmental objects.
        /// </summary>
        /// <param name="box">A box in space.</param>
        private void InitialFill(Box3D box)
        {
            foreach (var envObject in v_MSystem.FloatingObjects.Values)
            {
                var amount = box.Volume * envObject.Concentration - Randomizer.Rng.NextDouble();
                for (int i = 0; i < amount; i++)
                    Add(new FloatingObjectInWorld(envObject, box.RandomPoint()), false);
            }
        }

        /// <summary>
        /// Refills the box (OldSet only!) with floating objects up to their pre-defined concentrations in the environment.
        /// If the concentration of some objects is higher, no objects are removed.
        /// </summary>
        private void RefillBox(Box3D box)
        {
            var population = v_MSystem.FloatingObjects.Values.ToDictionary(obj => obj, obj => 0);

            foreach (var key in KeysInBox(box))
                foreach (var obj in v_Grid[key].OldSet.GetHashSet().Where(fltObject => box.Contains(fltObject.Position)))
                    population[obj.Type]++;

            foreach (var member in population)
                for (int i = 0; i < box.Volume * member.Key.Concentration - member.Value - Randomizer.Rng.NextDouble(); i++)
                    Add(new FloatingObjectInWorld(member.Key, box.RandomPoint()), false);
        }

        /// <summary>
        /// Add the new floating object to the world. MUST BE THREAD SAFE.
        /// If the object coordinates are outside the world, object is not added and false is returned.
        /// </summary>
        /// <returns>True if the object was added.</returns>
        private bool Add(FloatingObjectInWorld newObject, bool toNewSet)
        {
            ulong key = v_GridKeys.PositionToKey(newObject.Position);
            bool result = false;

            if (v_WorldBox.Contains(newObject.Position))
            {
                result = (toNewSet ? v_Grid[key].NewSet : v_Grid[key].OldSet).Add(newObject);
            }
            return result;
        }

        /// <summary>
        /// Removes a floating object in space from the world.
        /// </summary>
        private void Remove(FloatingObjectInWorld fltObject, bool mayBeInNewSet = false)
        {
            if (fltObject == null)
                throw new ArgumentNullException($"Removed floating object cannot be null");
            var key = v_GridKeys.PositionToKey(fltObject.Position);
            if (v_Grid.ContainsKey(key) && (v_Grid[key].OldSet.Remove(fltObject) ||
                          mayBeInNewSet && v_Grid[key].NewSet.Remove(fltObject))) return;

            throw new ArgumentException($"{fltObject} could not be removed from the world");
        }

        /// <summary>
        /// Random movement of floating objects in NewSet in all bars, avoiding colisions with tiles.
        /// Movement length due to Maxwell–Boltzmann probability distribution.
        /// Moved obejcts are placed to OldSet (in possibly other bars).
        /// All NewSets are cleared.
        /// </summary>
        private void RandomMove()
        {
            var polygons = v_tilesWorld.PolygonTiles.Select(tile => (Polygon3D)tile.Vertices).ToList();
            Parallel.ForEach(v_Grid.Values, (bar) =>
            {
                //Create own random for each thread.
                Random rng = new Random();

                foreach (var fltObject in bar.NewSet)
                {
                    // Optimization - environmental objects are many, they are moved, on average, every second step
                    if (!(fltObject.Type.Concentration > 0 && rng.NextBoolean()))
                    {
                        // A random vector in 3D Maxwell–Boltzmann distribution
                        var randomVector = new Vector3D(
                           Normal.Sample(rng, 0.0, 1.0),
                           Normal.Sample(rng, 0.0, 1.0),
                           Normal.Sample(rng, 0.0, 1.0));

                        Point3D newPosition = fltObject.Position + randomVector.ScaleBy(fltObject.Type.Mobility / b);

                        // Trajectory to the newPosition must mot intersect any polygon and the newPosition must not be too close to a polygon
                        // If the environment is not refilled ("test tube"), then objects cannot escape from the environment

                        if ((v_MSystem.RefillEnvironment || v_WorldBox.Contains(newPosition))
                            && !polygons.Any(polygon =>
                               polygon.Plane.MyIntersectsWith(fltObject.Position, newPosition) &&
                               (!polygon.IntersectionWith(fltObject.Position, newPosition, 0, false).IsNaN() ||
                               polygon.ContainsPoint(newPosition, 0, MSystem.SideDist))))
                            fltObject.Position = newPosition;
                    }
                    Add(fltObject, false); // Adding to OldSet, MUST BE THREAD-SAFE
                }
                bar.NewSet.Clear();
            }
            );
        }

        /// <summary>
        /// Adds to boundary boxes environmental floating objects coming from outer (unrepresented) world.
        /// The objects are calculated due to their concentration and added randomly.
        /// </summary>
        private void RefillBoundary()
        {
            var minCorner = v_WorldBox.MinCorner;
            var maxCorner = v_WorldBox.MaxCorner;

            // Refill -X boundary
            RefillBox(new Box3D(
                minCorner,
                new Point3D(minCorner.X + v_BoundaryVector.X, maxCorner.Y, maxCorner.Z)));
            // Refill +X boundary
            RefillBox(new Box3D(
                new Point3D(maxCorner.X - v_BoundaryVector.X, minCorner.Y, minCorner.Z),
                maxCorner));
            // Move corners so that already refilled boxes are not included
            minCorner += new Vector3D(v_BoundaryVector.X, 0, 0);
            maxCorner -= new Vector3D(v_BoundaryVector.X, 0, 0);

            // Refill -Y boundary
            RefillBox(new Box3D(
                minCorner,
                new Point3D(maxCorner.X, minCorner.Y + v_BoundaryVector.Y, maxCorner.Z)));
            // Refill +Y boundary
            RefillBox(new Box3D(
                new Point3D(minCorner.X, maxCorner.Y - v_BoundaryVector.Y, minCorner.Z),
                maxCorner));
            // Move corners so that already refilled boxes are not included
            minCorner += new Vector3D(0, v_BoundaryVector.Y, 0);
            maxCorner -= new Vector3D(0, v_BoundaryVector.Y, 0);

            // Refill -Z boundary
            RefillBox(new Box3D(
                minCorner,
                new Point3D(maxCorner.X, maxCorner.Y, minCorner.Z + v_BoundaryVector.Z)));
            // Refill +Z boundary
            RefillBox(new Box3D(
                new Point3D(minCorner.X, minCorner.Y, maxCorner.Z - v_BoundaryVector.Z),
                maxCorner));
        }

        /// <summary>
        /// Gets the set of floating objects within a reaction radius from a given position.
        /// Only objects unused in this simulation step are considered.
        /// Only objects with no obstacle between them and the given position are considered.
        /// </summary>
        /// <param name="position">Position whose neighborhood is searched</param>
        /// <param name="targetMultiset">OPTIMIZATION: Objects are collected only until this multiset is reached</param>
        private FloatingObjectsSet GetNearObjects(Point3D position, NamedMultiset targetMultiset)
        {
            var missingObjects = new NamedMultiset(targetMultiset.ToDictionary());
            var nearObjectsSet = new FloatingObjectsSet();
            if (!missingObjects.Any())
                return nearObjectsSet;

            Vector3D radiusVector = new Vector3D(v_MSystem.Mobility, v_MSystem.Mobility, v_MSystem.Mobility);

            var gridKeys = KeysInBox(new Box3D(position - radiusVector, position + radiusVector)).ToList();
            gridKeys.Shuffle();     // Random order of grid boxes where objects are sought for

            foreach (var key in gridKeys)
            {
                foreach (var fltObject in v_Grid[key].OldSet)
                {
                    if (missingObjects.Contains(fltObject.Name) &&
                        fltObject.Position.DistanceTo(position) < fltObject.Type.Mobility &&
                        !v_tilesWorld.IntersectsWith(fltObject.Position, position, false))
                    {
                        nearObjectsSet.Add(fltObject);
                        missingObjects.Remove(fltObject.Name);
                        if (!missingObjects.Any())
                            return nearObjectsSet;
                    }
                }
            }
            return nearObjectsSet;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Expands the world so that it includes the box given as a parameter plus the boundary.
        /// </summary>
        /// <param name="newBox">A box which must fit into the expanded world.</param>
        /// <param name="fillFloatingObjects">Populate the newly added space with environmental floating objects?</param>
        public void ExpandWith(Box3D newBox, bool fillFloatingObjects = true)
        {
            // Add a boundaryx
            newBox = new Box3D(newBox.MinCorner - v_BoundaryVector, newBox.MaxCorner + v_BoundaryVector);

            if (!(v_WorldBox.Contains(newBox.MinCorner) && v_WorldBox.Contains(newBox.MaxCorner)))
            {
                // The world box needs an extension.
                v_WorldBox = v_WorldBox.ExpandWith(newBox);

                foreach (var key in KeysInBox(v_WorldBox))
                    if (!v_Grid.ContainsKey(key))
                    {
                        v_Grid[key] = new FloatingObjectsBar();
                        if (fillFloatingObjects)
                            InitialFill(v_GridKeys.KeyToBox(key));
                    }
            }
        }


        /// <summary>
        /// Gets the set of floating objects within a reaction distance from the center of a tile.
        /// Only objects unused in this simulation step are considered.
        /// TODO low priority: return floating objects close to the whole shape of the tile except border
        /// </summary>
        public FloatingObjectsSet GetNearObjects(TileInSpace tile, NamedMultiset targetMultiset)
        {
            return GetNearObjects(tile.Position, targetMultiset);
        }


        /// <summary>
        /// Gets the set of floating objects within a reaction distance from a given position on a tile.
        /// Only objects unused in this simulation step are considered.
        /// If the tile is 2D, only floating objects on the inner/outer side are returned as specified by the parameter "isInside"
        /// Otherwise the parameter "isInside" is ignored.
        /// </summary>
        public FloatingObjectsSet GetNearObjects(ProteinOnTileInSpace protein, Tile.SideType side, NamedMultiset targetMultiset)
        {
            return GetNearObjects(protein.m_tile.SidePoint(protein.Position, side), targetMultiset);
        }

        /// <summary>
        /// Gets the set of floating objects within the reaction distance from any position of a given connector.
        /// Only objects unused in this simulation step are considered.
        /// TODO low priority: return floating objects close to the whole shape of the connector except endpoints
        /// </summary>
        public FloatingObjectsSet GetNearObjects(ConnectorOnTileInSpace connector, NamedMultiset targetMultiset)
        {
            FloatingObjectsSet objectsSet = new FloatingObjectsSet();

            foreach (var position in connector.Positions)
                objectsSet.UnionWith(GetNearObjects(connector.SidePoint(position), targetMultiset));
            return objectsSet;
        }

        /// <summary>
        /// Gets the set of floating objects within a given prism.
        /// Both objects used and unused in this simulation step are considered.
        /// </summary>
        public FloatingObjectsSet GetObjectsInPrism(Polygon3D basePolygon, Vector3D vector)
        {
            FloatingObjectsSet insideObjects = new FloatingObjectsSet();
            var topPolygon = basePolygon.Select(vertex => vertex + vector);

            foreach (var key in KeysInBox(new Box3D(basePolygon.Union(topPolygon))))
            {
                insideObjects.UnionWith(v_Grid[key].OldSet.GetHashSet().Where(fltObject =>
                    basePolygon.IntersectsWith(fltObject.Position, fltObject.Position - vector)));
                insideObjects.UnionWith(v_Grid[key].NewSet.GetHashSet().Where(fltObject =>
                    basePolygon.IntersectsWith(fltObject.Position, fltObject.Position - vector)));
            }
            return insideObjects;
        }

        /// <summary>
        /// Adds the multiset of floating objects at a given position.
        /// </summary>
        public void AddAt(NamedMultiset multiset, Point3D position)
        {
            foreach (var name in multiset)
                Add(new FloatingObjectInWorld(v_MSystem.FloatingObjects[name], position), true);
        }

        /// <summary>
        /// Removes the multiset of names of floating objects from a given set of floating objects in space
        /// </summary>
        /// <param name="multiset">Multiset of floating object names to be removed</param>
        /// <param name="targetSet">Set of floating objects from which the multiset is removed - all must be of type "FloatingObjectsInWorld"</param>
        /// <exception cref="ArgumentNullException">If the set contains object which is not "FloatingObjectsInWorld"</exception>
        /// <exception cref="ArgumentException">If the multiset could not be removed</exception>
        public void RemoveFrom(NamedMultiset multiset, FloatingObjectsSet targetSet)
        {
            // Deep copy of the original multiset
            NamedMultiset myMultiset = new NamedMultiset(multiset.ToDictionary());
            foreach (var fltObject in targetSet)
                if (myMultiset.Remove(fltObject.Name))
                    Remove(fltObject as FloatingObjectInWorld);

            if (myMultiset.Count > 0)
                throw new ArgumentException($"{myMultiset} could not be removed from the world!");
        }

        /// <summary>
        /// Moves a given floating object by a vector TODO add unit test
        /// </summary>
        /// <param name="fltObject">Floating object - must be of type "FloatingObjectsInWorld"</param>
        /// <param name="vector">Moving vector</param>
        /// <exception cref="ArgumentNullException">If the floating object is not "FloatingObjectsInWorld"</exception>
        public void Move(FloatingObjectInSpace fltObject, Vector3D vector)
        {
            var obj = fltObject as FloatingObjectInWorld;   // TODO refactor so that fltObject does not need retyping
            Remove(obj, true);    // If obj is null, then "Remove" throws an exception
            obj.Position += vector;
            Add(obj, false);
        }


        /// <summary>
        /// In all the bars, adds all floating objects in NewSet to OldSet and clears the NewSet.
        /// Randomly moves all objects and refills boundary of the world with objects coming from outside.
        /// </summary>
        /// <param name="freeze">If true, no random movements of objects, for testing only.</param>
        public void FinalizeStep(bool freeze = false)
        {
            foreach (var bar in v_Grid.Values)
                if (freeze)
                {
                    bar.OldSet.UnionWith(bar.NewSet);
                    bar.NewSet.Clear();
                }
                else
                {
                    bar.NewSet.UnionWith(bar.OldSet);
                    bar.OldSet.Clear();
                }
            if (!freeze)
                RandomMove();       // Moves all objects to OldSet, clears newSet
            if (v_MSystem.RefillEnvironment)
                RefillBoundary();
        }


        /// <summary>
        /// Provides the set of all floating objects in the world.
        /// </summary>
        /// <returns>Set of all floating objects in the world.</returns>
        public FloatingObjectsSet ToSet()
        {
            FloatingObjectsSet allFltObjects = new FloatingObjectsSet();
            foreach (var bar in v_Grid.Values)
            {
                allFltObjects.UnionWith(bar.OldSet.GetHashSet());
                allFltObjects.UnionWith(bar.NewSet.GetHashSet());
            }
            return allFltObjects;
        }


        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String of objects.</returns>
        public override string ToString()
        {
            return $"Floating objects in the world: {ToSet().ToMultiset()}";
        }

        #endregion
    }
}
