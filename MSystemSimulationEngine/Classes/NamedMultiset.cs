using System.Collections.Generic;
using System.Linq;
using MSystemSimulationEngine.Interfaces;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Multiset of strings, typically names of objects (fixed or floating)
    /// </summary>
    public class NamedMultiset : Multiset<string>
    {
        #region Constructor

        /// <summary>
        /// Simple constructor for empty multiset
        /// </summary>
        public NamedMultiset()
        { }

        /// <summary>
        /// Constructor creating multiset of names of ISimulationObjects in a list
        /// </summary>
        public NamedMultiset(IEnumerable<ISimulationObject> simObjects) : base(simObjects.Select(simObject => simObject.Name))
        { }

        /// <summary>
        /// Constructor creating multiset from a dictionary with elements [item_name, item_multiplicity]
        /// </summary>
        public NamedMultiset(IDictionary<string, int> dictionary) : base(dictionary) { }

        #endregion

        /// <summary>
        /// Computes union of a sequence of multisets
        /// </summary>
        /// <param name="list"></param>
        public static NamedMultiset Union(IEnumerable<NamedMultiset> list)
        {
            var result = new NamedMultiset();
            foreach (var element in list)
                result.UnionWith(element);
            return result;
        }

        // ToString() method overriden in the base Multiset class.
    }
}
