using System;
using System.Windows.Forms;

namespace SharedComponents.Tools
{
    /// <summary>
    /// Shows/Hides wait cursor.
    /// </summary>
    public class WaitCursor : IDisposable
    {
        public WaitCursor()
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        /// <summary>
        /// Shows/Hides wait cursor.
        /// </summary>
        /// <param name="show">Show or hide.</param>
        public static void Show(bool show)
        {
            Cursor.Current = show ? Cursors.WaitCursor : Cursors.Default;
        }

        /// <summary>
        /// Implementation of IDisposable which resets cursor to default value.
        /// </summary>
        public void Dispose()
        {
            Cursor.Current = Cursors.Default;
        }
    }
}
