using System;

namespace Cytos_v2.Exceptions
{
    /// <summary>
    /// Missing XML element exception.
    /// </summary>
    public class MissingXmlElement : Exception
    {
        #region Constructor

        /// <summary>
        /// MissingXmlElement constructor.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public MissingXmlElement(string errorMessage) : base(errorMessage)
        {}

        #endregion
    }
}
