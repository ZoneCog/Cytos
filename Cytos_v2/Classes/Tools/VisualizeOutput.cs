using Cytos_v2.Forms;

namespace Cytos_v2.Classes.Tools
{
    public class VisualizeOutput
    {
        private static Main v_MainWindow;

        public VisualizeOutput(Main mainWindon)
        {
            v_MainWindow = mainWindon;
        }

        /// <summary>
        /// Adds text to output RichTextBox located in main window.
        /// </summary>
        /// <param name="text">Text which will be added to output.</param>
        public static void AddText(string text)
        {
            v_MainWindow.AddTextToOutput(text);
        }
    }
}
