using System.Collections.Generic;
using System.Text;
using MathNet.Spatial.Units;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// A collection (set) of floating objects. 
    /// Possible implementations:
    /// - hashset (recent)
    /// - named multiset (maybe later, more complicated)
    /// </summary>
    public class FloatingObjectsSet : HashSet<FloatingObjectInSpace>
    {
        #region Constructor

        /// <summary>
        /// An empty set 
        /// </summary>
        public FloatingObjectsSet()
        { }

        /// <summary>
        /// A set initialized by collection
        /// </summary>
        public FloatingObjectsSet(IEnumerable<FloatingObjectInSpace> fltObjectList) : base(fltObjectList)
        {  }

        #endregion

        #region Public methods

        /// <summary>
        /// Converts the set of objects to named multiset.
        /// </summary>
        /// <returns>Named multiset.</returns>
        public NamedMultiset ToMultiset()
        {
            return new NamedMultiset(this);
        }

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String of object.</returns>
        public override string ToString()
        {
            List<string> floatingObjectsNames = new List<string>();
            foreach (FloatingObjectInSpace floatingObjectInSpace in this)
            {
                floatingObjectsNames.Add(floatingObjectInSpace.ToString());
            }

            return string.Join(",", floatingObjectsNames);
        }

        #endregion

    }
}
