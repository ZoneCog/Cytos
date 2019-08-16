using System.IO;

namespace SharedComponents.Tools
{
    /// <summary>
    /// Used for file accessibility check
    /// </summary>
    public static class FileAccessibility
    {
        /// <summary>
        /// Checks weather file is locked.
        /// </summary>
        /// <param name="file">File info.</param>
        /// <returns>True if file is locked otherwise false.</returns>
        public static bool IsFileLocked(FileInfo file)
        {
            if (!file.Exists)
            {
                return false;
            }

            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                stream?.Close();
            }

            //file is not locked
            return false;
        }
    }
}
