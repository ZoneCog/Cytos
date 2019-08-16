using System;
using System.Collections.Generic;
using System.Text;

namespace MSystemSimulationEngine.Classes
{
    /// <summary>
    /// Represents the glue relation class.
    /// </summary>
    public class GlueRelation : Dictionary<Tuple<Glue, Glue>, NamedMultiset>
    {

        #region Public Methods

        /// <summary>
        /// Override of ToString() method.
        /// </summary>
        /// <returns>String of object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var kvp in this)
            {
            builder.AppendFormat("Glue pair: ({0}, {1}), Released objects: {2}\n", 
                kvp.Key.Item1.Name, kvp.Key.Item2.Name, kvp.Value);
            }
            return builder.ToString();
        }


        /// <summary>
        /// Returns true if the two glues are related in any order
        /// </summary>
        public bool MatchAsymmetric(Glue glue1, Glue glue2)
        {
            return ContainsKey(Tuple.Create(glue1, glue2)) || ContainsKey(Tuple.Create(glue2, glue1));
        }


        #endregion
    }
}
