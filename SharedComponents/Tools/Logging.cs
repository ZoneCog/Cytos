using System;
using System.IO;
using System.Threading;

namespace SharedComponents.Tools
{
    /// <summary>
    /// Provides a centralized place for logging operations.
    /// </summary>
    public static class Logging
    {
        #region Private data

        /// <summary>
        /// Default log file path.
        /// </summary>
        private static string v_LogFilePath = "cytos.log";

        /// <summary>
        /// Default log file path.
        /// </summary>
        private static string v_LogSimulationPath = "cytosSimulation.log";

        #endregion

        #region Private methods

        /// <summary>
        /// Logs message to specified file.
        /// </summary>
        /// <param name="message">Logging message.</param>
        /// <param name="logLocation">Path to the log file.</param>
        private static void LogMessage(string message, string logLocation)
        {
            if (!string.IsNullOrEmpty(message))
            {
                string logDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                //Check weather file is locked.
                while (FileAccessibility.IsFileLocked(new FileInfo(logLocation)))
                {
                    Thread.Sleep(5);
                }
                using (StreamWriter sw = new StreamWriter(logLocation, true))
                {
                    sw.WriteLine("- - - {0} - - -", logDateTime);
                    sw.WriteLine("{0}", message);
                    sw.WriteLine();
                }
            }
        }

        #endregion

        #region Public methods


        #region Basic log

        /// <summary>
        /// Logs message, date and time will be added automaticly.
        /// Default location is within 'bin/debug' folder.
        /// </summary>
        /// <param name="message">Logging message.</param>
        public static void LogMessage(string message)
        {
            LogMessage(message, v_LogFilePath);
        }

        #endregion

        #region Simulation log

        /// <summary>
        /// Logs simulation message, date and time will be added automaticly.
        /// Default location is within 'bin/debug' folder.
        /// </summary>
        /// <param name="message">Logging message.</param>
        public static void LogSimulationMessage(string message)
        {
            LogMessage(message, v_LogSimulationPath);
        }


        #endregion

        /// <summary>
        /// Changes default path for log file.
        /// </summary>
        /// <param name="path">Log file path.</param>
        public static void ChangeDefaultLogFilePath(string path)
        {
            v_LogFilePath = path;
        }

        /// <summary>
        /// Gets logging file path.
        /// </summary>
        /// <returns>Location of the log file.</returns>
        public static string GetLogFilePath()
        {
            return v_LogFilePath;
        }

        /// <summary>
        /// Changes default path for simulation log file.
        /// </summary>
        /// <param name="path">Log file path.</param>
        public static void ChangeDefaultSimulationLogFilePath(string path)
        {
            v_LogSimulationPath = path;
        }

        /// <summary>
        /// Gets simulation logging file path.
        /// </summary>
        /// <returns>Location of the log file.</returns>
        public static string GetSimulationLogFilePath()
        {
            return v_LogSimulationPath;
        }

        #endregion
    }
}
