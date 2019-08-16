using System;
using System.Text;
using MathNet.Spatial.Euclidean;

namespace Cytos_v2.Classes
{
    /// <summary>
    /// Class of a named 3D point (named coordinate or vertex or floating object).
    /// Cannot inherit from Point3D as Point3D is sealed.
    /// </summary>
    public class NamedPoint3D 
    {
        #region Public data

        /// <summary>
        /// Name of the point.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Position of the point.
        /// </summary>
        public Point3D Position;

        #endregion

        #region Constructor

        /// <summary>
        /// Construictor of a named 3D point.
        /// </summary>
        /// <param name="name">Name of the point.</param>
        /// <param name="position">Position of the point.</param>
        /// <exception cref="ArgumentException">
        /// If name of the point is null or empty string or
        /// if position is null.
        /// </exception>
        public NamedPoint3D(string name, Point3D position)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name of the object can't be null or empty string.");
            }
            if (position == null)
            {
                throw new ArgumentException("Position of the object can't be null.");
            }
            Name = name;
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
            builder.AppendFormat("Point object in space: name = {0}, position = {1}", Name, Position);
            return builder.ToString();
        }

        #endregion
    }
}
