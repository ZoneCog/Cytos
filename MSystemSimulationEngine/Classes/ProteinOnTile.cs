using System;
using System.Text;
using MathNet.Spatial.Euclidean;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Represents protein on tile.
    /// </summary>
    public class ProteinOnTile : Protein
    {
        #region Public data

        /// <summary>
        /// Position on a tile relative to origin of the tile.
        /// TODO there must be a setter checking whether the position on the object is correct. Make private and override this field in the child class?
        /// </summary>
        public Point3D Position;

        #endregion

        #region Constructor

        /// <summary>
        /// Protein constructor.
        /// </summary>
        /// <param name="proteinName">Protein name.</param>
        /// <param name="position">Position of the protein.</param>
        /// <exception cref="ArgumentException">If protein name is null or empty string.</exception>
        public ProteinOnTile(string proteinName, Point3D position) : base(proteinName)
        {
            Position = position;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String of object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Protein: name = {0}, position = {1}", Name, Position);
            return builder.ToString();
        }

        #endregion
    }
}
