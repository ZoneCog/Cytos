using System;

namespace SharedComponents.Exceptions
{
    /// <summary>
    /// Validation failed exception.
    /// </summary>
    public class ValidationFailed:Exception
    {
        #region Constructor

        /// <summary>
        /// Error/warning appears during validation.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public ValidationFailed(string errorMessage) : base(errorMessage)
        { }

        #endregion
    }
}
