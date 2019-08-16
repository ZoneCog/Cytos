using System;

namespace Cytos_v2.Exceptions
{
    /// <summary>
    /// Missing XML attribute exception.
    /// </summary>
    public class MissingXmlAttribute : Exception
    {
        #region Constructor

        /// <summary>
        /// MissingXmlAttribute constructor.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public MissingXmlAttribute(string errorMessage) : base(errorMessage)
        {}

        #endregion
    }
}
