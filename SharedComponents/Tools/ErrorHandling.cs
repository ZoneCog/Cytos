namespace SharedComponents.Tools
{
    /// <summary>
    /// Provides a centralized place for non-critical error handling.
    /// </summary>
    class ErrorHandling
    {
        #region Public methods

        /// <summary>
        /// Holds non-critical errors during application run.
        /// These errors are only logged no exception thrown.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public static void LogErrorWithoutThrowingException(string errorMessage)
        {
            Logging.LogMessage(string.Format("---> Error occured: {0}", errorMessage));
        }

        #endregion
    }
}
