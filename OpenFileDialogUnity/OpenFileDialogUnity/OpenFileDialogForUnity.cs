using System.Windows.Forms;

namespace OpenFileDialogUnity
{
    /// <summary>
    /// Open file dialog class used by unity.
    /// </summary>
    public class OpenFileDialogForUnity
    {
        /// <summary>
        /// Opens open file dialog.
        /// </summary>
        /// <returns>Path of selected file.</returns>
        public string GetOpenFileDialogForUnity()
        {
            //TODO make it generic and send filter and title as input parameter.
            //TODO Recently used from regedit.
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "xml files (*.xml)|*.xml",
                FilterIndex = 1,
                Title = "Please select SnapShot file.",
                RestoreDirectory = true
                };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                return path;
            }
            return null;
        }
    }
}
