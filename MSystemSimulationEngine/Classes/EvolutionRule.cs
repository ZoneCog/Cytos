using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSystemSimulationEngine.Interfaces;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Evolution rule of P system used for simulation.
    /// </summary>
    public class EvolutionRule: ISimulationObject
    {
        #region Public data

        /// <summary>
        /// Name of the rule = its short string representation
        /// </summary>
        public string Name =>
            string.Join(",", LeftSideObjects.Select(obj => obj.Name)) + " -> " +
            string.Join(",", RightSideObjects.Select(obj => obj.Name));

        /// <summary>
        /// Type of the evolution rule.
        /// </summary>
        public readonly RuleType Type;

        /// <summary>
        /// Priority of the evolution rule.
        /// </summary>
        public readonly int Priority;

        /// <summary>
        /// List of left side objects.
        /// </summary>
        public readonly IReadOnlyList<ISimulationObject> LeftSideObjects;

        /// <summary>
        /// List of right side objects.
        /// </summary>
        public readonly IReadOnlyList<ISimulationObject> RightSideObjects;

        /// <summary>
        /// Enum holding all evolution rule types.
        /// </summary>
        public enum RuleType
        {
            Metabolic, Create, Insert, Divide, Destroy 
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Evolution rule constructor.
        /// </summary>
        /// <param name="type">Type of the evolution rule.</param>
        /// <param name="priority">Priority of the evolution rule.</param>
        /// <param name="leftSideObjects">List of left side objects.</param>
        /// <param name="rightSideObjects">List of right side objects.</param>
        /// <exception cref="ArgumentException">
        /// list of left side objects is null or
        /// list of right side objects is null.
        /// </exception>
        protected EvolutionRule(RuleType type, int priority, IReadOnlyList<ISimulationObject> leftSideObjects, IReadOnlyList<ISimulationObject> rightSideObjects)
        {
            if (leftSideObjects == null)
            {
                throw new ArgumentException("Left side objects of the evolution rule can't be null.");
            }
            if (rightSideObjects == null)
            {
                throw new ArgumentException("Right side objects of the evolution rule can't be null.");
            }
            Type = type;
            Priority = priority;
            LeftSideObjects = leftSideObjects;
            RightSideObjects = rightSideObjects;
        }

        #endregion

        #region Private method

        /// <summary>
        /// Transforms input string type to enum.
        /// </summary>
        /// <param name="type">Type string.</param>
        /// <exception cref="ArgumentException">         
        /// If type is unknown
        /// </exception>
        /// <returns>Transformed type to enum.</returns>
        private static RuleType TransformEvolutionRuleTypeString(string type)
        {
            RuleType result;
            if (!Enum.TryParse(type, true, out result))
            {
                throw new ArgumentException(string.Format("Unknown evolution rule type {0}", type));
            }
            return result;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns a new evolution rule of a subclass depending on its type.
        /// </summary>
        /// <param name="type">Type of the evolution rule.</param>
        /// <param name="priority">Priority of the evolution rule.</param>
        /// <param name="leftSideObjects">List of left side objects.</param>
        /// <param name="rightSideObjects">List of right side objects.</param>
        /// <exception cref="ArgumentException">
        /// If rule type is null
        /// </exception>
        /// <returns>Evolution rule of a proper type.</returns>
        public static EvolutionRule NewRule(string type, int priority, List<ISimulationObject> leftSideObjects,
            List<ISimulationObject> rightSideObjects)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("Type of the evolution rule can't be null or empty.");
            }
            RuleType enumRuleType = TransformEvolutionRuleTypeString(type);

            if (enumRuleType == RuleType.Metabolic)
            {
                return new EvoMetabolicRule(priority, leftSideObjects, rightSideObjects);
            }
            else
            {
                return new EvoNonMetabolicRule(enumRuleType, priority, leftSideObjects, rightSideObjects);
            }
        }

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String of object.</returns>
        public override string ToString()
        {
            Array values = Enum.GetValues(typeof(RuleType));
            string type = values.GetValue(Convert.ToInt32(Type)).ToString();

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Rule: {2}, type = {0}, priority = {1}", type, Priority, Name);
            return builder.ToString();
        }
        #endregion
    }
}
