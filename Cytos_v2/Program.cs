using System;
using System.Threading;
using System.Windows.Forms;
using Cytos_v2.Classes.Tools;
using Cytos_v2.Forms;
using SharedComponents.Tools;

namespace Cytos_v2
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
            Main mainWindow = new Main();
            new VisualizeOutput(mainWindow); //Hack which is used for vizualization of any output text anywhere in the application.
            Application.Run(mainWindow);
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
            ExceptionWindow.Show((Exception) e.ExceptionObject);
        }
    }
}
