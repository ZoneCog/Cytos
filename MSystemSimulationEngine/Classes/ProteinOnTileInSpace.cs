using System.Text;

namespace MSystemSimulationEngine.Classes
{
    public class ProteinOnTileInSpace : ProteinOnTile
    {
        #region Public data

        /// <summary>
        /// Tile in space on which is the protein placed. 
        /// </summary>
        public readonly TileInSpace m_tile;

        /// <summary>
        /// Indicates whether the protein is used or not.
        /// </summary>
        public bool IsUsed = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Protein constructor.
        /// </summary>
        /// <param name="tile">Tile on which is this protein placed.</param>
        /// <param name="protein">Base protein from which this protein derives.</param>
        public ProteinOnTileInSpace(TileInSpace tile, ProteinOnTile protein) : base(protein.Name, protein.Position)
        {
            m_tile = tile;
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
            builder.AppendFormat("Protein: name = {0}, position = {1}, used = {2}", Name, Position, IsUsed);
            return builder.ToString();
        }

        #endregion
    }
}