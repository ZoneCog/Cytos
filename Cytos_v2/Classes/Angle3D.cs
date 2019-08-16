namespace Cytos_v2.Classes
{
    /// <summary>
    /// Provides angle of the object in environment.
    /// </summary>
    public class Angle3D
    {
        #region Public data

        /// <summary>
        /// Holds the angle on axes X.
        /// </summary>
        public double X;

        /// <summary>
        /// Holds the angle on axes Y.
        /// </summary>
        public double Y;

        /// <summary>
        /// Holds the angle on axes Z.
        /// </summary>
        public double Z;

        #endregion

        #region Constructor

        /// <summary>
        /// Represents the angle of the object within the cell.
        /// </summary>
        /// <param name="x">Angle on axes X.</param>
        /// <param name="y">Angle on axes Y.</param>
        /// <param name="z">Angle on axes Z.</param>
        public Angle3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String of object.</returns>
        public override string ToString()
        {
            return string.Format("x = {0}; y = {1}; z = {2}", X, Y, Z);
        }

        #endregion
    }
}
