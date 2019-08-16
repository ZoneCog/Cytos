using System.Text;
using MathNet.Spatial.Euclidean;
using MSystemSimulationEngine.Interfaces;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// A copy of floating object identified by name and placed in 3D space
    /// </summary>
    public class FloatingObjectInSpace: ISimulationObject
    {
        #region Private data

        /// <summary>
        /// Instance counter to provide a unique instance ID. Probably not needed.
        /// </summary>
        private static ulong v_Counter;

        #endregion

        #region Public data
        /// <summary>
        /// Unique ID of an instance.
        /// </summary>
        public readonly ulong ID;

        /// <summary>
        /// Base type floating object.
        /// </summary>
        public readonly FloatingObject Type;

        /// <summary>
        /// Implements the interface ISimulationObject.
        /// </summary>
        public string Name => Type.Name;

        /// <summary>
        /// Position of the object.
        /// </summary>
        public Point3D Position { get; protected set; }

        #endregion


        #region Public methods

        public FloatingObjectInSpace(FloatingObject type, Point3D position)
        {
            Type = type;
            Position = position;
            ID = v_Counter++;
        }


        /// <summary>
        /// DO NOT USE OUTSIDE OF UNIT TESTS!!!
        /// Resets floating objects in space counter.
        /// </summary>
        /// <remarks>ONLY FOR UNIT TESTS PURPOSE</remarks>
        public static void ResetCounter()
        {
            v_Counter = 0;
        }

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String of object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Floating object: name = {0}, position = {1}", Name, Position);
            return builder.ToString();
        }
        #endregion

    }
}
