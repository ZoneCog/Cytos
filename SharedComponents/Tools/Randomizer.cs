using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedComponents.Tools
{
    /// <summary>
    /// Provides some randomized methods.
    /// E.g. provides a mechanism for choosing random item from collection.
    /// </summary>
    public static class Randomizer
    {
        #region Public data

        /// <summary>
        /// Random generator - will be used MANY times,
        /// therefore we do not want to create new object each time.
        /// </summary>
        public static readonly Random Rng = new Random();

        #endregion

        #region Private methods

        /// <summary>
        /// Gets random item from given collection within given bounds.
        /// </summary>
        /// <param name="collection">Input collection.</param>
        /// <param name="lowerBound">Lower bound.</param>
        /// <param name="upperBound">Upper bound.</param>
        /// <returns>Random item form given collection.</returns>
        private static T GetRandomItemFromCollection<T>(ICollection<T> collection, int lowerBound, int upperBound)
        {
            return collection.ElementAt( Rng.Next(lowerBound, upperBound));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets a random item from an ICollection.
        /// </summary>
        /// <param name="collection">Input collection.</param>
        /// <returns>Random item form given collection.</returns>
        /// <exception cref="InvalidOperationException">
        /// If collection is null or empty.
        /// </exception>
        public static T GetRandomItem<T>(this ICollection<T> collection)
        {
            if (collection == null || collection.Count == 0)
                throw new InvalidOperationException("Collection can't be null or empty.");
            return GetRandomItemFromCollection(collection, 0, collection.Count);
        }

        /// <summary>
        /// Shuffles randomly an IList.
        /// </summary>
        /// <param name="list">Input IList.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int n = (list?.Count ?? 0)-1; n > 0; n--)
            {
                int k = Rng.Next(n + 1);
                var value = list[k];    // If list==null, the cycle would not iterate
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
         /// Gets random double within given bounds.
         /// </summary>
         /// <param name="lowerBound">Lower bound.</param>
         /// <param name="upperBound">Upper bound.</param>
         /// <returns>Random double within given bounds.</returns>
         /// <exception cref="InvalidOperationException">
         /// If lower bound exceed upper bound
         /// </exception>
        public static double NextDoubleBetween(double lowerBound, double upperBound)
        {
            if (lowerBound > upperBound)
            {
                throw new InvalidOperationException("Upper bound can't be lower than lower bound.");
            }
            return Rng.NextDouble() * (upperBound - lowerBound) + lowerBound;
        }

        #endregion
    }
}
