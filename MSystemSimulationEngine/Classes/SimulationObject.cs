using System;
using System.Text;
using MSystemSimulationEngine.Interfaces;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Represents a simulation object (floating, fixed, protein...) in the definition of P system.
    /// </summary>
    public class SimulationObject : ISimulationObject
    {
        #region Public data

        /// <summary>
        /// Holds the name of the simulation object.
        /// </summary>
        public string Name { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Simulation object constructor.
        /// </summary>
        /// <param name="name">Name of the simulation object.</param>
        protected SimulationObject(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name of the simulation object can't be null or empty string.");
            }
            Name = name;
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
            builder.AppendFormat("{0}: name = {1}", GetType().Name, Name);
            return builder.ToString();
        }

        #endregion
    }
}
