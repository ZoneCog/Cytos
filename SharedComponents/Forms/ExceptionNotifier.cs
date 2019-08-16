using System;
using System.Windows.Forms;
using SharedComponents.Tools;

namespace SharedComponents.Forms
{
    /// <summary>
    /// This form shows, when unhandled exception occures.
    /// </summary>
    public partial class ExceptionNotifier : Form
    {
        /// <summary>
        /// Initialization of the window.
        /// </summary>
        /// <param name="exception">Exception, which will be shown.</param>
        public ExceptionNotifier(Exception exception)
        {
            InitializeComponent();
            Logging.LogMessage(exception.Message);
            richTextBoxExceptionMessage.Text = exception.Message;
            richTextBoxExceptionStackTrace.Text = exception.StackTrace;
            if (exception.InnerException != null)
            {
                richTextBoxExceptionStackTrace.Text = string.Format("{0}\n{1}", exception.InnerException.StackTrace, richTextBoxExceptionStackTrace.Text);
            }
        }
    }
}
