using System;
using System.Collections.Generic;
using System.Linq;
using MSystemSimulationEngine.Interfaces;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Metabolic type of evolution rule of P system used for simulation.
    /// </summary>
    public class EvoMetabolicRule : EvolutionRule
    {
        #region Public data

        /// <summary>
        /// Subtype of the metabolic rule.
        /// </summary>
        public readonly MetabolicRuleType SubType;

        /// <summary>
        /// Protein of the evolution rule.
        /// </summary>
        public readonly Protein RProtein;

        /// <summary>
        /// Multiset of left side floating objects (names) inside membrane.
        /// </summary>
        public readonly NamedMultiset MLeftInNames;

        /// <summary>
        /// Multiset of left side floating objects (names) outside membrane.
        /// </summary>
        public readonly NamedMultiset MLeftOutNames;

        /// <summary>
        /// Multiset of right side floating objects (names) inside membrane.
        /// </summary>
        public readonly NamedMultiset MRightInNames;

        /// <summary>
        /// Multiset of right side floating objects (names) outside membrane.
        /// </summary>
        public readonly NamedMultiset MRightOutNames;

        /// <summary>
        /// Enum holding all evolution rule types.
        /// </summary>
        public enum MetabolicRuleType
        {
            Undefined, Symport, Antiport, Catalyzed
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Metabolic evolution rule constructor.
        /// </summary>
        /// <param name="priority">Priority of the evolution rule.</param>
        /// <param name="leftSideObjects">List of left side objects.</param>
        /// <param name="rightSideObjects">List of right side objects.</param>
        /// <param name="delay">Number of steps which must be done before rule is applied to tiles.</param>
        /// <exception cref="ArgumentException">
        /// If format of the metabolic rule is invalid
        /// </exception>
        public EvoMetabolicRule(int priority, List<ISimulationObject> leftSideObjects, List<ISimulationObject> rightSideObjects, int delay)
             : base(RuleType.Metabolic, priority, leftSideObjects, rightSideObjects, delay)
        {
            string errorMessage = string.Format("{0}\n{1} ", "Invalid metabolic rule format:", this);

            // The rules must contain exactly one protein.
            try
            {
                RProtein = LeftSideObjects.OfType<Protein>().Single();
                if (RProtein != RightSideObjects.OfType<Protein>().Single())
                {
                    throw new ArgumentException(errorMessage);
                }
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException(errorMessage);
            }


            // The objects before protein are "out" (must be on the outer side of 2D object).
            // The objects after protein are "in" (must be on the inner side of 2D object).
            MLeftOutNames = new NamedMultiset(LeftSideObjects.TakeWhile(obj => obj is FloatingObject));
            MLeftInNames = new NamedMultiset(LeftSideObjects.SkipWhile(obj => obj is FloatingObject).OfType<FloatingObject>());
            MRightOutNames = new NamedMultiset(RightSideObjects.TakeWhile(obj => obj is FloatingObject));
            MRightInNames = new NamedMultiset(RightSideObjects.SkipWhile(obj => obj is FloatingObject).OfType<FloatingObject>());

            // Both left and right-hand side of the rule must contain at least one floating object
            if (MLeftInNames.Count + MLeftOutNames.Count > 0 && MRightInNames.Count + MRightOutNames.Count > 0)
            {
                if (MLeftOutNames.Count + MRightOutNames.Count == 0 || MLeftInNames.Count + MRightInNames.Count == 0)
                {
                    SubType = MetabolicRuleType.Catalyzed;
                }
                else if (MLeftInNames.Equals(MRightOutNames) && MLeftOutNames.Equals(MRightInNames))
                {
                    if (MLeftInNames.Count == 0 || MLeftOutNames.Count == 0)
                        SubType = MetabolicRuleType.Symport;
                    else
                        SubType = MetabolicRuleType.Antiport;
                }
            }


            if (SubType == MetabolicRuleType.Undefined)
            {
                throw new ArgumentException(errorMessage);
            }
        }

        #endregion

    }
}
