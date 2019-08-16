using System;
using System.Threading;
using System.Windows.Forms;
using SharedComponents.Tools;

namespace MSystemCreator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.ThreadException += UnhandledThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MSystemCreatorForm());
        }

        /// <summary>
        /// Handles untrapper thread exception.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        static void UnhandledThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ExceptionWindow.Show(e.Exception);
        }

        /// <summary>
        /// Handles unhandle application exception.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionWindow.Show((Exception)e.ExceptionObject);
        }
    }
}
