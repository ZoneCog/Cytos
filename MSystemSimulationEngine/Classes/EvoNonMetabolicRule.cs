using System;
using System.Collections.Generic;
using System.Linq;
using MSystemSimulationEngine.Interfaces;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Creation, destruction or division rule of P system used for simulation.
    /// </summary>
    public class EvoNonMetabolicRule : EvolutionRule
    {
        #region Public data

        /// <summary>
        /// Multiset of left side floating objects (names).
        /// </summary>
        public readonly NamedMultiset MLeftSideFloatingNames;

        /// <summary>
        /// Multiset of right side floating objects (names).
        /// </summary>
        public readonly NamedMultiset MRightSideFloatingNames;

        #endregion

        #region Constructor

        /// <summary>
        /// Non-metabolic evolution rule constructor.
        /// </summary>
        /// <param name="type">Type of the evolution rule.</param>
        /// <param name="priority">Priority of the evolution rule.</param>
        /// <param name="leftSideObjects">List of left side objects.</param>
        /// <param name="rightSideObjects">List of right side objects.</param>
        /// <param name="delay">Number of steps which must be done before rule is applied to tiles.</param>
        /// <exception cref="ArgumentException">
        /// If format of the non-metabolic rule is invalid
        /// </exception>
        public EvoNonMetabolicRule(RuleType type, int priority, List<ISimulationObject> leftSideObjects, List<ISimulationObject> rightSideObjects, int delay) 
            : base(type, priority, leftSideObjects, rightSideObjects, delay)
        {
            string errorMessage = $"{"Invalid rule format:"}\n{this} ";
            bool correct = false;

            switch (type)
            {
                case RuleType.Create:
                case RuleType.Insert:
                    correct = //leftSideObjects.Count >= 1 &&
                              leftSideObjects.TrueForAll(obj => obj is FloatingObject) &&
                              rightSideObjects.SingleOrDefault() is Tile;
                    break;
                case RuleType.Destroy:
                    correct = leftSideObjects.Count >= 2 &&
                              leftSideObjects.OfType<Tile>().Count() == 1 &&
                              leftSideObjects.TrueForAll(obj => obj is FloatingObject || obj is Tile) &&
                              rightSideObjects.TrueForAll(obj => obj is FloatingObject);
                    break;
                case RuleType.Divide:
                    correct = leftSideObjects.Count >= 3 && 
                              leftSideObjects.OfType<Glue>().Count() == 2 &&
                              leftSideObjects.TrueForAll(obj => obj is FloatingObject || obj is Glue) &&
                              rightSideObjects.Count() == 2 &&
                              rightSideObjects[0] == leftSideObjects.OfType<Glue>().First() &&
                              rightSideObjects[1] == leftSideObjects.OfType<Glue>().Last();
                    break;
            }
            if (! correct)
                throw new ArgumentException(errorMessage);

            MLeftSideFloatingNames = new NamedMultiset(LeftSideObjects.OfType<FloatingObject>());
            MRightSideFloatingNames = new NamedMultiset(RightSideObjects.OfType<FloatingObject>());
        }

        #endregion

    }
}
