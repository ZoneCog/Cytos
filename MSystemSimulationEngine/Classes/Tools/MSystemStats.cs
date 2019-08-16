using System;
using System.Collections.Generic;
using System.Linq;

namespace MSystemSimulationEngine.Classes.Tools
{
    /// <summary>
    /// Statistic class for M system simulatuion.
    /// </summary>
    public class MSystemStats
    {
        #region Private data

        private Dictionary<int, int> m_componentStats;

        #endregion

        #region Constructor
        public MSystemStats()
        {
            m_componentStats = new Dictionary<int, int>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// MSystemStats constructor.
        /// </summary>
        /// <param name="tilesWorld">Tiles in simulation world</param>
        public void CalculateTileStats(TilesWorld tilesWorld)
        {
            if (tilesWorld == null)
            {
                throw new ArgumentException("TilesWorld cannot be null.");
            }
            // get copy of all tiles
            HashSet<TileInSpace> polygonTiles = new HashSet<TileInSpace>(tilesWorld.PolygonTiles);

            while (polygonTiles.Count != 0)
            {
                // get first tile in the collection and get its component, e.g. all tiles connected to it
                TileInSpace tile = polygonTiles.First();
                HashSet<TileInSpace> component = tile.Component();

                // now walk through the whole component and count q1 (valued at 1) and q2 (valued at 10) tiles
                int completionValue = 0;
                foreach (TileInSpace element in component)
                {
                    // only polygon tiles matter
                    if (element.Vertices is Polygon3D)
                    {
                        switch (element.Name)
                        {
                            case "q0":
                            case "q1":
                                completionValue += 15;
                                break;
                            case "q2":
                                completionValue += 1;
                                break;
                        }
                    }
                }

                // increment number of components in stats dictionary
                if (!m_componentStats.ContainsKey(completionValue))
                    m_componentStats.Add(completionValue, 0);
                m_componentStats[completionValue]++;

                // remove all tiles of the component form the list
                foreach (TileInSpace element in component)
                {
                    polygonTiles.Remove(element);
                }
            }
        }

        /// <summary>
        /// No functionality.
        /// </summary>
        /// <param name="floatingObjectWorld">Floating object in simulation world.</param>
        public void CalculateFloatingObectsStats(FloatingObjectsWorld floatingObjectWorld)
        {

        }
        /// <summary>
        /// No functionality.
        /// </summary>
        /// <param name="mSystem">M system.</param>
        public void CalculateMSystemStats(MSystem mSystem)
        {

        }

        /// <summary>
        /// Get mumber of full cells of the system.
        /// </summary>
        /// <returns>Number of full cells.</returns>
        public int GetFullCellsCount()
        {
            if (m_componentStats.ContainsKey(40))
                return m_componentStats[40];
            return 0;
        }

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String representation of object.</returns>
        public override string ToString()
        {
            string msg1 = "";
            string msg2 = "";

            foreach (var item in m_componentStats)
            {
                msg1 += ";Completion value;" + item.Key + ";Count;" + item.Value;
                // full cells value
                if (item.Key == 40)
                    msg2 = ";Full cells counted;" + item.Value;
            }

            // we did not find any full cells
            if (msg2 == "")
                msg2 = ";Full cells counted;" + 0;

            return "MSystemStats" + msg2 + msg1;
        }

        #endregion
    }
}
