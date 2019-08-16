using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using MSystemSimulationEngine.Classes.Tools;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Represents connector class.
    /// </summary>
    public class Connector : SimulationObject
    {
        #region Public data

        /// <summary>
        /// Holds the list of positions locating the connector. TODO rename the field
        /// </summary>
        public readonly ReadOnlyCollection<Point3D> Positions;

        /// <summary>
        /// Glue of the connector.
        /// </summary>
        public readonly Glue Glue;

        /// <summary>
        /// Angle of the connector.
        /// </summary>
        public readonly Angle Angle;

        /// <summary>
        /// Resistance of the connector.
        /// </summary>
        public readonly double Resistance;

        #endregion

        #region Constructor

        /// <summary>
        /// Connector constructor
        /// </summary>
        /// <param name="name">Name of the connector.</param>
        /// <param name="positions">List of positions of the connector, can be empty.</param>
        /// <param name="glue">Glue of the connector.</param>
        /// <param name="angle">Angle of the connector.</param>
        /// <param name="resistance">Resistance of the connector.</param>
        /// <exception cref="ArgumentException">
        /// If name is null or empty string or if list of positions is null or if Glue is null.
        /// </exception>
        public Connector(string name, IList<Point3D> positions, Glue glue, Angle angle, double resistance = 0) : base(name)
        {
            if (positions == null)
            {
                throw new ArgumentException($"Positions of the connector {name} can't be null.");
            }
            if (glue == null)
            {
                throw new ArgumentException($"Glue of the connector {name} can't be null.");
            }
            Glue = glue;
            Angle = angle;
            Resistance = resistance;

            switch (positions.Count)
            {
                case 0:
                    throw new ArgumentException($"Position of the connector {name} is undefined.");
                case 1:
                    Positions = new ReadOnlyCollection<Point3D>(positions);
                    break;
                case 2:
                    Positions = new Segment3D(positions[0], positions[1], name); // Checks the distance of endpoints
                    break;
                default:
                    throw new ArgumentException($"Connector {name} can't have more than two vertices.");
            }
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
            builder.AppendFormat("Connector: Name = {0}, {2}, Angle = {3} \nPositions = {1}", Name,
                string.Join(", ", Positions.Select(pos => pos.ToString("F4", null))), Glue,
                Angle.ToString("G5", null, AngleUnit.Degrees));
            return builder.ToString();
        }

        #endregion
    }
}
