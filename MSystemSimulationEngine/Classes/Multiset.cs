using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Represents multiset class. Original version taken from 
    /// http://stackoverflow.com/questions/2597691/are-there-any-implementations-of-multiset-for-net
    /// Added support of elements with infinite multiplicity.
    /// </summary>
    public class Multiset<T> : IEnumerable<T>

    {
        #region Private data

        /// <summary>
        /// Holds the internal multiset representation.
        /// </summary>
        private readonly Dictionary<T, int> v_Data;
        
        /// <summary>
        /// Value to represent elements with infinite multiplicity.
        /// </summary>
        public const int Infinity = int.MaxValue;

        #endregion

        #region Constructor

        /// <summary>
        /// An empty multiset constructor
        /// </summary>
        protected Multiset()
        {
            v_Data = new Dictionary<T, int>();
        }

        /// <summary>
        /// Constructor of a multiset initialized from a dictionary.
        /// </summary>
        /// <param name="dictionary">multiset initializer (key = element, value = multiplicity.</param>
        protected Multiset(IDictionary<T, int> dictionary)
        {
            v_Data = dictionary.Where(kvp => kvp.Value > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Constructor of a multiset initialized from IEnumerable.
        /// </summary>
        /// <param name="elements"> of IEnumerable initializing the multiset, an element can be contained multiple times.</param>
        protected Multiset(IEnumerable<T> elements)
        {
            v_Data = new Dictionary<T, int>();
            foreach (T element in elements)
            {
                Add(element);
            }
        }

        #endregion 

        #region Public methods

        /// <summary>
        /// Add an element to the multiset.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            int count;
            v_Data.TryGetValue(item, out count);
            v_Data[item] = (count == Infinity ? Infinity : ++count);
        }

        /// <summary>
        /// Checks whether an element is in the multiset.
        /// </summary>
        /// <param name="item"></param>
        public bool Contains(T item)
        {
            int count;
            return v_Data.TryGetValue(item, out count) && count > 0;
        }

        /// <summary>
        /// Empty the multiset.
        /// </summary>
        public void Clear()
        {
            v_Data.Clear();
        }

        /// <summary>
        /// Computes union of this multiset with another: {1,1}\cup {1,2}={1,1,2}
        /// https://en.wikipedia.org/wiki/Multiset#Example_multiset_operations
        /// </summary>
        /// <param name="another"> Another multiset.</param>
        public void UnionWith(Multiset<T> another)
        {
            foreach (var key in another.v_Data.Keys)
            {
                int count;
                v_Data.TryGetValue(key, out count);
                v_Data[key] = Math.Max(count, another.v_Data[key]);
            }
        }

        /// <summary>
        /// Determine whether a Multiset<T> object is a subset of another multiset.
        /// </summary>
        /// <param name="another"> Another multiset.</param>
        public bool IsSubsetOf(Multiset<T> another)
        {
            int count;
            return v_Data.All(kvp => (another.v_Data.TryGetValue(kvp.Key, out count) 
                && kvp.Value <= count));
        }

        /// <summary>
        /// Determine whether a Multiset<T> object equals to another multiset.
        /// </summary>
        /// <param name="another"> Another multiset.</param>
        /// <returns>True if this multiset equals the other one.</returns>
        public bool Equals(Multiset<T> another)
        {
            // Determines whether a Multiset<T> object equals (componentwise) to another multiset.
            return IsSubsetOf(another) && another.IsSubsetOf(this);
        }

        /// <summary>
        /// Number of elements of the multiset, excluding infinite values.
        /// </summary>
        /// <returns>Number of elements of the multiset.</returns>
        public int Count
        {
            get { return v_Data.Values.Where(count => count != Infinity).Sum(); }
        }

        /// <summary>
        /// Remove an element the multiset.
        /// </summary>
        /// <param name="item"> Element to be removed.</param>
        /// <returns>True if the element was successfuly removed.</returns>
        public bool Remove(T item)
        {
            int count;
            if (!v_Data.TryGetValue(item, out count))
                return false;
            count = count == Infinity ? Infinity : --count;

            if (count == 0)
                v_Data.Remove(item);
            else
                v_Data[item] = count;

            return true;
        }

        /// <summary>
        /// Coonvert the multiset to a dictionary.
        /// </summary>
        /// <returns>New dictionary with the content of the multiset.</returns>
        public Dictionary<T, int> ToDictionary()
        {
            return new Dictionary<T, int>(v_Data);
        }

        /// <summary>
        /// Enumerator of the multiset.
        /// </summary>
        /// <returns>Next element of the multiset.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var kvp in v_Data)
                for (int i = 0; i < kvp.Value; i++)
                    yield return kvp.Key;
        }

        /// <summary>
        /// Enumerator of the multiset.
        /// </summary>
        /// <returns>Enumerator of the multiset.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String of multiset.</returns>
        public override string ToString()
        {
            return string.Join("\n", v_Data);
        }

        #endregion 
    }
}