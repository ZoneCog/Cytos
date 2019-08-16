using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SharedComponents.Tools;

namespace Cytos_v2.Forms
{
    /// <summary>
    /// Shows content of logging file.
    /// </summary>
    public partial class OpenLogFile : Form
    {
        /// <summary>
        /// Log file path.
        /// </summary>
        private readonly string v_LogPath;

        /// <summary>
        /// Number of lines in log file.
        /// </summary>
        private int v_NumberOfLines;


        /// <summary>
        /// Initialization of window.
        /// </summary>
        /// <param name="logPath">Path to log file.</param>
        public OpenLogFile(string logPath)
        {
            v_LogPath = logPath;

            InitializeComponent();
            ShowContent();
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Refreshes richtext box with possibly new data.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshContent();
        }

        /// <summary>
        /// Clears(remove) log file
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonClearLogFile_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Are you sure you want to clear log file?", "Clear log file", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                //Check weather file is locked.
                while (FileAccessibility.IsFileLocked(new FileInfo(v_LogPath)))
                {
                    Thread.Sleep(5);
                }
                File.Delete(v_LogPath);
            }
            ShowContent();
        }

        /// <summary>
        /// Shows content of log file
        /// </summary>
        private void ShowContent()
        {
            Text = string.Format("{0} | Last refrest at {1}", Path.GetFileName(v_LogPath), DateTime.Now);
            richTextBoxLogContent.Clear();
            if (File.Exists(v_LogPath))
            {
                using (StreamReader reader = new StreamReader(v_LogPath))
                {
                    string content;
                    int numberOfLines = 0;
                    while ((content = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(content))
                        {
                            richTextBoxLogContent.AppendText(Environment.NewLine);
                        }
                        else
                        {
                            richTextBoxLogContent.AppendText(content);
                            richTextBoxLogContent.AppendText(Environment.NewLine);
                        }
                        numberOfLines++;
                    }
                    v_NumberOfLines = numberOfLines;
                }
            }
            else
            {
                richTextBoxLogContent.AppendText("No log file found at default location.");
            }
            richTextBoxLogContent.ScrollToCaret();
        }

        /// <summary>
        /// Refresh only specific number of newly added lines.
        /// </summary>
        private void RefreshContent()
        {
            string[] newLines = File.ReadAllLines(v_LogPath).Skip(v_NumberOfLines).ToArray();
            foreach (string newLine in newLines)
            {
                if (string.IsNullOrEmpty(newLine))
                {
                    richTextBoxLogContent.AppendText(Environment.NewLine);
                }
                else
                {
                    richTextBoxLogContent.AppendText(newLine);
                    richTextBoxLogContent.AppendText(Environment.NewLine);
                }
            }
            v_NumberOfLines += newLines.Length;
            richTextBoxLogContent.ScrollToCaret();
        }

        private void OpenLogFile_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.LogSize.Width == 0 || Properties.Settings.Default.LogSize.Height == 0)
            {
                // first start
                // optional: add default values
            }
            else
            {
                // we don't want a minimized window at startup
                if (WindowState == FormWindowState.Minimized) WindowState = FormWindowState.Normal;

                Location = Properties.Settings.Default.LogLocation;
                Size = Properties.Settings.Default.LogSize;
            }
        }

        private void OpenLogFile_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                // save location and size if the state is normal
                Properties.Settings.Default.LogLocation = Location;
                Properties.Settings.Default.LogSize = Size;
            }
            else
            {
                // save the RestoreBounds if the form is minimized or maximized!
                Properties.Settings.Default.LogLocation = RestoreBounds.Location;
                Properties.Settings.Default.LogSize = RestoreBounds.Size;
            }

            // don't forget to save the settings
            Properties.Settings.Default.Save();
        }
    }
}
