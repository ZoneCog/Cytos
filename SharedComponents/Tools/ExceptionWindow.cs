using SharedComponents.Forms;

namespace SharedComponents.Tools
{
    /// <summary>
    /// Used for showing exception windows with exception information.
    /// </summary>
    public class ExceptionWindow
    {
        #region Public methods

        /// <summary>
        /// Shows window with error message.
        /// </summary>
        /// <param name="exception">Exception to show.</param>
        public static void Show(System.Exception exception)
        {
            ExceptionNotifier notifier = new ExceptionNotifier(exception);
            notifier.ShowDialog();
        }

        #endregion
    }
}
