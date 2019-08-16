using SharedComponents.Tools;

namespace Cytos_v2.Classes.Tools
{
    public static class VisualizeLogging
    {
        /// <summary>
        /// Logs message and visualize it, date and time will be added automaticly.
        /// Default location is within 'bin/debug' folder.
        /// </summary>
        /// <param name="message">Logging message.</param>s
        public static void LogMessageAndVisualize(string message)
        {
            Logging.LogMessage(message);
            VisualizeOutput.AddText(message);
        }

        /// <summary>
        /// Logs simulation message and visualize it, date and time will be added automaticly.
        /// Default location is within 'bin/debug' folder.
        /// </summary>
        /// <param name="message">Logging message.</param>
        public static void LogSimulationMessageAndVisualize(string message)
        {
            Logging.LogSimulationMessage(message);
            VisualizeOutput.AddText(message);
        }
    }
}
