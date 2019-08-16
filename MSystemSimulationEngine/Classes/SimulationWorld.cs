using System;
using MSystemSimulationEngine.Classes.Xml;

namespace MSystemSimulationEngine.Classes
{
    public class SimulationWorld
    {
        #region Protected data

        /// <summary>
        /// Holds defined P system during simulation.
        /// </summary>
        protected readonly MSystem SimulationMSystem;

        /// <summary>
        /// Holds the structure of all tiles of the P system created during simulation.
        /// </summary>
        protected readonly TilesWorld TilesWorld;

        /// <summary>
        /// Holds all floating objects during simulation.
        /// </summary>
        protected readonly FloatingObjectsWorld FltObjectsWorld;

        #endregion

        #region Constructor

        /// <summary>
        /// Simulation constructor.
        /// </summary>
        /// <param name="mSystemObjects">Deserialized M System objects.</param>
        /// <exception cref="ArgumentException">
        /// If M System objects objecst list is null.
        /// </exception>
        protected SimulationWorld(DeserializedObjects mSystemObjects)
        {
            if (mSystemObjects == null)
            {
                throw new ArgumentException("M System objects can't be null");
            }
            SimulationMSystem = new MSystem(mSystemObjects);
            TilesWorld = new TilesWorld(SimulationMSystem);
            FltObjectsWorld = new FloatingObjectsWorld(SimulationMSystem, TilesWorld);
            // The TilesWorld and FltObjectsWorld need to crossref.
            TilesWorld.FltObjectsWorld = FltObjectsWorld;
        }

        #endregion
    }
}
