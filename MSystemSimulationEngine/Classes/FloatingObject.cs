namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Represents a prototype of floating object in the definition of P system.
    /// </summary>
    public class FloatingObject : SimulationObject
    {
        #region Public data
        /// <summary>
        /// Mean mobility of the object in space.
        /// </summary>
        public readonly double Mobility;

        /// <summary>
        /// Concentration of the object in the environment.
        /// </summary>
        public readonly double Concentration;

        #endregion

        #region Constructor

        /// <summary>
        /// Floatring object constructor.
        /// </summary>
        /// <param name="name">Name of the floating object.</param>
        /// <param name="mobility">Mobility of the floating object.</param>
        /// <param name="concentration">Concentration of the floating object.</param>
        public FloatingObject(string name, double mobility, double concentration) : base(name)
        {
            Mobility = mobility;
            Concentration = concentration;
        }

        #endregion

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String representation of object.</returns>
        public override string ToString()
        {
            return $"Floating object: name = {Name}, Mobility = {Mobility}, Concentration = {Concentration}";
        }
    }
}
