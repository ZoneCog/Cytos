using Cytos_v2.Classes.Tools;
using MSystemCreator.Classes;
using MSystemSimulationEngine.Classes;
using MSystemSimulationEngine.Classes.Tools;
using MSystemSimulationEngine.Classes.Xml;
using MSystemSimulationEngine.Interfaces;
using SharedComponents.Exceptions;
using SharedComponents.Forms;
using SharedComponents.Tools;
using SharedComponents.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Cytos_v2.Forms
{
    /// <summary>
    /// Main form with all available events and actions. 
    /// </summary>
    public partial class Main : Form
    {
        #region Private data


        /// <summary>
        /// Name of the project
        /// </summary>
        private const string c_ProjectName = "Cytos v2";

        /// <summary>
        /// Unity projectName CytosV.exe
        /// </summary>
        private const string c_UnityProjectName = "CytosV.exe";

        private const string c_SimulationEngineName = "MSystemSimulationEngine.dll";

        /// <summary>
        /// Path of selected M System declaration file.
        /// </summary>
        private string v_MSystemObjectsPath;

        /// <summary>
        /// Holds deserialized evolution objects.
        /// </summary>
        private DeserializedObjects v_MSystemObjects;

        /// <summary>
        /// Main simulator holder.
        /// </summary>
        private Simulator v_Simulator;

        /// <summary>
        /// Holds reciever state.
        /// </summary>
        private bool v_ReciverInitialized;

        /// <summary>
        /// XSD path loaded from config
        /// </summary>
        private readonly string v_XSDPath;

        /// <summary>
        /// Flag which defines if XSD validation should be used
        /// </summary>
        private readonly bool v_ValidateUsingXSD;

        /// <summary>
        /// Flag whether serialize floating object to snapshot or not.
        /// </summary>
        private readonly bool v_SerializeFloatingObjects;

        #endregion

        #region Private methods

        #region UI events

        /// <summary>
        /// --- New example - Click event ---
        /// Creates new example. Open new form with XML attributes required for specific example (Inventory/Evolution file).
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void newExampleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowLoader loader = new WindowLoader();
            loader.OpenMainWindow();
        }

        #region Load example menu events

        /// <summary>
        /// --- Load example -> M System description - Click event ---
        /// Opens OpenFileDialog, M System description file and deserialize input.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void mSystemDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "XML Files (.xml)|*.xml";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Check if selected file is really evolution file.
                    XDocument doc = XDocument.Load(openFileDialog.FileName);
                    if (doc.Element("root") == null)
                    {
                        throw new InvalidOperationException("Selected file is not an M System description file.");
                    }
                    v_MSystemObjectsPath = openFileDialog.FileName;

                    //XSD validation
                    if (v_ValidateUsingXSD)
                    {
                        string errorsAndWarnings;
                        if (!XmlValidator.Validate(v_MSystemObjectsPath, v_XSDPath, out errorsAndWarnings))
                        {
                            throw new ValidationFailed(errorsAndWarnings);
                        }
                    }

                    string errorMessage;
                    IDeserializedObjects deserializedObjects = Xmlizer.Deserialize(v_MSystemObjectsPath, out errorMessage);
                    if (deserializedObjects == null)
                    {
                        richTextBoxMSystem.Text = errorMessage;
                        return;
                    }
                    v_MSystemObjects = TypeUtil.Cast<DeserializedObjects>(deserializedObjects);
                    RestartSimulator();

                    richTextBoxMSystem.Text = v_Simulator.MSystemToString();
                    VisualizeLogging.LogMessageAndVisualize("Deserialization of M System description file was successful.");
                    VisualizeLogging.LogMessageAndVisualize(string.Format("File: {0}", openFileDialog.FileName));
                }
            }
            catch (Exception exception)
            {
                ExceptionWindow.Show(exception);
            }
        }

        #endregion

        /// <summary>
        /// --- Close program - Click event ---
        /// Closes program.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void closeProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// --- Reset simulator - Click event ---
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void restartSimulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestartSimulator();
        }

        #region Run menu events

        /// <summary>
        /// --- Run -> Run simulation - Click event ---
        /// Runs whole simulation with selected inventory and evolution files.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void runSimulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartSimulator(0);
        }

        /// <summary>
        /// --- Run -> Run 1 step - Click event ---
        /// Runs 1 simulation step with selected inventory and evolution files.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void run1StepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartSimulator(1);
        }

        /// <summary>
        /// --- Run -> Run specified number of steps - Click event ---
        /// Runs specified number of simulation steps with selected inventory and evolution files.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void runSpecifiedNumberOfStepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputBox inputBox = new InputBox("Number of steps", "OK", new List<string> { "Number of steps" });
            inputBox.ShowDialog();
            if (inputBox.OutputTexts.Count > 0)
            {
                ulong numberOfSteps;
                if (!ulong.TryParse(inputBox.OutputTexts[0], out numberOfSteps))
                {
                    MessageBox.Show("Entered value is not a number!");
                    return;
                }
                StartSimulator(numberOfSteps);
            }
        }

        /// <summary>
        /// --- Run -> Multiple runs (fixed kill) - Click event ---
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void multipleRunsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputBox inputBox = new InputBox("Multiple runs(fixed kills)", "OK",
                new List<string> { "Number of runs", "Number of steps for each run", "No of kills", "Probabilistic selection Y/N", "Tile name to remove (empty if it does not matter)" },
                new List<Regexp.Check> { Regexp.Check.NumberWithoutZero, Regexp.Check.NumberWithoutZero, Regexp.Check.NumberWithoutZero, Regexp.Check.String, Regexp.Check.String });
            inputBox.ShowDialog();
            if (inputBox.OutputTexts.Count == 5)
            {
                // get number of runs
                int numberOfRuns = int.Parse(inputBox.OutputTexts[0]);
                // get number of steps for each run
                int numberOfSteps = int.Parse(inputBox.OutputTexts[1]);
                // how often do we want to hurt?
                int noOfKills = int.Parse(inputBox.OutputTexts[2]);

                if (inputBox.OutputTexts[3] != "Y" && inputBox.OutputTexts[3] != "N")
                {
                    MessageBox.Show("Invalis selection for probabilistic selection. Allowed values are Y or N.");
                    return;
                }
                bool probabilistic = inputBox.OutputTexts[3] == "Y";

                // what object will be removed
                string tileName = "";
                if (noOfKills > 0)
                {
                    tileName = inputBox.OutputTexts[4];
                }
                RunMultipleSimulations(numberOfRuns, numberOfSteps, tileName, 0, noOfKills, probabilistic);
            }
        }

        /// <summary>
        /// --- Run septum simulation for number of steps and count number of complete cells
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void multipleRunsV2TestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get number of runs and number of steps at which I count complete cells
            InputBox inputBox = new InputBox("Septum count test", "OK",
                new List<string> { "Number of runs",                 // number of experiments
                                   "Number of steps for each run",   // number of steps in each experiment
                                   "Count cells every X step",       // count system and its cells every X step in each experiment
                                   "No of kills in each experiment", // how many kills do we want to do in each experiment
                                   "Probabilistic selection Y/N",    // N - no probability, kill every fixed number of steps, Y - choose randomly
                                   "Tile name to remove (empty if it does not matter)" },  // name of the tile to be targeted while hurting
                new List<Regexp.Check> { Regexp.Check.NumberWithoutZero,
                                         Regexp.Check.NumberWithoutZero,
                                         Regexp.Check.NumberWithoutZero,
                                         Regexp.Check.NumberWithoutZero,
                                         Regexp.Check.String,
                                         Regexp.Check.Skip}
                );
            inputBox.ShowDialog();
            if (inputBox.OutputTexts.Count == 6)
            {
                // get number of runs
                int numberOfRuns = int.Parse(inputBox.OutputTexts[0]);
                // get number of steps for each run
                int numberOfSteps = int.Parse(inputBox.OutputTexts[1]);
                // get number of steps at which to stop and count
                int countStep = int.Parse(inputBox.OutputTexts[2]);
                // how often do we want to hurt?
                int noOfKills = int.Parse(inputBox.OutputTexts[3]);
                // probbilistic selection ?
                if (inputBox.OutputTexts[4] != "Y" && inputBox.OutputTexts[4] != "N")
                {
                    MessageBox.Show("Invalis selection for probabilistic selection. Allowed values are Y or N.");
                    return;
                }
                bool probabilistic = inputBox.OutputTexts[4] == "Y";
                // what object will be removed
                string tileName = "";
                if (noOfKills > 0)
                {
                    tileName = inputBox.OutputTexts[5];
                }

                RunMultipleSimulationsV2(numberOfRuns, numberOfSteps, countStep, tileName, 0, noOfKills, probabilistic);
            }
        }

        /// <summary>
        /// --- Run -> Multiple runs (probabilistic kill) - Click event ---
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void multipleRunsprobabilisticKillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get number of runs
            InputBox inputBox = new InputBox("Multiple runs (probabilistic kill)", "OK",
                new List<string> { "Number of runs", "Number of steps for each run", "Probability of the kill (0.2 stands for 20%)", "Tile name to remove (empty if it does not matter)" },
                new List<Regexp.Check> { Regexp.Check.NumberWithoutZero, Regexp.Check.NumberWithoutZero, Regexp.Check.FloatingNumber, Regexp.Check.String }
                );
            inputBox.ShowDialog();
            if (inputBox.OutputTexts.Count == 4)
            {
                // get number of runs
                int numberOfRuns = int.Parse(inputBox.OutputTexts[0]);
                // get number of steps for each run
                int numberOfSteps = int.Parse(inputBox.OutputTexts[1]);
                // how often do we want to hurt?
                double probabilityOfTheKill = double.Parse(inputBox.OutputTexts[2]);
                // what object will be removed
                string tileName = inputBox.OutputTexts[3];
                RunMultipleSimulations(numberOfRuns, numberOfSteps, tileName, probabilityOfTheKill);
            }
        }

        /// <summary>
        /// --- Run -> Multiple runs (probabilistic kill) V2 - Click event ---
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void multipleRunsprobabilisticKillV2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get number of runs
            InputBox inputBox = new InputBox("Multiple runs (probabilistic kill)", "OK",
                new List<string> { "Number of runs",                 // number of experiments
                                   "Number of steps for each run",   // number of steps in each experiment
                                   "Count cells every X step",       // count system and its cells every X step in each experiment
                                   "Probability of the kill (0.2 stands for 20%)", // execute hurt with the the probability of
                                   "Tile name to remove (empty if it does not matter)" },  // name of the tile to be targeted
                new List<Regexp.Check> { Regexp.Check.NumberWithoutZero, 
                                         Regexp.Check.NumberWithoutZero,
                                         Regexp.Check.NumberWithoutZero,
                                         Regexp.Check.FloatingNumber, 
                                         Regexp.Check.Skip } 
                );
            inputBox.ShowDialog();
            if (inputBox.OutputTexts.Count == 5)
            {
                // get number of runs
                int numberOfRuns = int.Parse(inputBox.OutputTexts[0]);
                // get number of steps for each run
                int numberOfSteps = int.Parse(inputBox.OutputTexts[1]);
                // get number of steps at which to stop and count
                int countStep = int.Parse(inputBox.OutputTexts[2]);
                // how often do we want to hurt?
                double probabilityOfTheKill = double.Parse(inputBox.OutputTexts[3]);
                // what object will be removed
                string tileName = inputBox.OutputTexts[4];

                RunMultipleSimulationsV2(numberOfRuns, numberOfSteps, countStep, tileName, probabilityOfTheKill);
            }

        }
        #endregion

        #region Save Snapshot

        /// <summary>
        /// Saves Snapshot.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void saveSnapshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (v_Simulator == null)
            {
                ShowLoadSimulationMessage();
                return;
            }
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "XML files (*.xml)|*.xml";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string savePath = saveFileDialog.FileName;
                    v_Simulator.SaveSnapshotsWithSpecificLocation(savePath);
                    VisualizeLogging.LogMessageAndVisualize(string.Format("Snapshot save to {0}", savePath));
                }
            }
        }

        /// <summary>
        /// Saves Snapshot and Vizualize it.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void saveSnapshotAndVizualizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (v_Simulator == null)
            {
                ShowLoadSimulationMessage();
                return;
            }
            string snapshotPath = v_Simulator.SaveSnapshotsAndReturnPath();

            if (!File.Exists(c_UnityProjectName))
            {
                throw new InvalidOperationException(string.Format("Unity visualization project {0} was not found!", c_UnityProjectName));
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = c_UnityProjectName,
                Arguments = snapshotPath
            };

            Process.Start(startInfo);
        }

        #endregion

        /// <summary>
        /// --- About - Click event ---
        /// Opens form with Authors and Copyrights.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.LoadFrom(c_SimulationEngineName);

            MessageBox.Show($"Simulation of geometrical membrane systems\n\nUI version: {Assembly.GetExecutingAssembly().GetName().Version.ToString()}\n" +
                            $"Simulation engine version: {assembly.GetName().Version}\n" +
                            "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n\n" +
                            "Authors:\n\nPetr Sosík\nMax Garzon\nVladimír Smolka\nJan Drastík\nJaroslav Bradík");
        }

        /// <summary>
        /// --- Search in M System objects ---
        /// Search for words within M system description file.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void toolStripTextBoxSearchInMSystemObjects_TextChanged(object sender, EventArgs e)
        {
            string content = richTextBoxMSystem.Text;
            richTextBoxMSystem.Clear();
            richTextBoxMSystem.Text = content;
            int index = 0;
            if (!toolStripTextBoxSearchInMSystemObjects.Text.Equals(string.Empty))
            {
                while (index < richTextBoxMSystem.Text.LastIndexOf(toolStripTextBoxSearchInMSystemObjects.Text, StringComparison.Ordinal))
                {
                    richTextBoxMSystem.Find(toolStripTextBoxSearchInMSystemObjects.Text, index, richTextBoxMSystem.TextLength, RichTextBoxFinds.None);
                    richTextBoxMSystem.SelectionBackColor = Color.Yellow;
                    index = richTextBoxMSystem.Text.IndexOf(toolStripTextBoxSearchInMSystemObjects.Text, index, StringComparison.Ordinal) + 1;
                }
            }
        }

        /// <summary>
        /// --- Open Log file ---
        /// Opens log file.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void openLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenLogFile openLogFile = new OpenLogFile(Logging.GetLogFilePath());
            openLogFile.Show();
        }

        /// <summary>
        /// --- Open simulatuion log file ---
        /// Opens simulation log file.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void openSimulationLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenLogFile openLogFile = new OpenLogFile(Logging.GetSimulationLogFilePath());
            openLogFile.Show();
        }

        #region Simulation engine controls

        /// <summary>
        /// Stop simulation.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            VisualizeLogging.LogMessageAndVisualize("Simulation stopping...");
            Simulator.SimulationCurrentState = Simulator.SimulationControlFlags.Stop;
        }

        /// <summary>
        /// Run simulation.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonRun_Click(object sender, EventArgs e)
        {
            StartSimulator(0);
        }

        /// <summary>
        /// Restart simulation.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonRestart_Click(object sender, EventArgs e)
        {
            RestartSimulator();
        }

        #endregion

        /// <summary>
        /// On load event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void Main_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MainSize.Width == 0 || Properties.Settings.Default.MainSize.Height == 0)
            {
                // first start
                // optional: add default values
            }
            else
            {
                // we don't want a minimized window at startup
                if (WindowState == FormWindowState.Minimized) WindowState = FormWindowState.Normal;

                Location = Properties.Settings.Default.MainLocation;
                Size = Properties.Settings.Default.MainSize;
            }
        }

        /// <summary>
        /// On closing event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                // save location and size if the state is normal
                Properties.Settings.Default.MainLocation = Location;
                Properties.Settings.Default.MainSize = Size;
            }
            else
            {
                // save the RestoreBounds if the form is minimized or maximized!
                Properties.Settings.Default.MainLocation = RestoreBounds.Location;
                Properties.Settings.Default.MainSize = RestoreBounds.Size;
            }

            // don't forget to save the settings
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Key down event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            //Load M System description
            if (e.KeyCode == Keys.L && e.Control)
            {
                e.Handled = true;
                mSystemDescriptionToolStripMenuItem.PerformClick();
            }
            //Run simulator
            if (e.KeyCode == Keys.R && e.Control)
            {
                e.Handled = true;
                runSimulationToolStripMenuItem.PerformClick();
            }
            //Run simulator with specified number of steps
            if (e.KeyCode == Keys.N && e.Control)
            {
                e.Handled = true;
                runSpecifiedNumberOfStepsToolStripMenuItem.PerformClick();
            }
            //Visualize snapshot
            if (e.KeyCode == Keys.U && e.Control)
            {
                e.Handled = true;
                saveSnapshotAndVizualizeToolStripMenuItem.PerformClick();
            }
        }

        #endregion

        private void SetUItoRunningState()
        {
            buttonRun.Enabled = false;
            buttonStop.Enabled = true;
            buttonRestart.Enabled = false;

            runSimulationToolStripMenuItem.Enabled = false;
            run1StepToolStripMenuItem.Enabled = false;
            runSpecifiedNumberOfStepsToolStripMenuItem.Enabled = false;
            restartSimulationToolStripMenuItem.Enabled = false;
            loadExampleToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Resets UI components to default state.
        /// </summary>
        private void ResetUi()
        {
            if (InvokeRequired)
            {
                MethodInvoker mi = ResetUi;
                Invoke(mi);
            }
            else
            {
                buttonRun.Enabled = true;
                buttonStop.Enabled = false;
                buttonRestart.Enabled = true;

                runSimulationToolStripMenuItem.Enabled = true;
                run1StepToolStripMenuItem.Enabled = true;
                runSpecifiedNumberOfStepsToolStripMenuItem.Enabled = true;
                restartSimulationToolStripMenuItem.Enabled = true;
                loadExampleToolStripMenuItem.Enabled = true;

                // XJB - for statistical purposes
                //multipleRunsToolStripMenuItem.Enabled = true;                  // the test is obsolete and shall be revisited
                //multipleRunsprobabilisticKillToolStripMenuItem.Enabled = true; // the test is obsolete and shall be revisited
                multipleRunsV2ToolStripMenuItem.Enabled = true;               
                multipleRunsprobabilisticKillV2ToolStripMenuItem.Enabled = true;
                oneOffDamageTestToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Run simulation loop with specified number of steps
        /// </summary>
        /// <param name="numberOfSteps">Number of steps</param>
        /// <remarks>If number of steps is 0 it runs in unlimited loop until there is no applicable rule.</remarks>
        private void StartSimulator(ulong numberOfSteps)
        {
            if (v_Simulator == null)
            {
                ShowLoadSimulationMessage();
                return;
            }

            SetUItoRunningState();

            Simulator.SimulationCurrentState = Simulator.SimulationControlFlags.Run; //Set simulation flat to RUN.
            ParameterizedThreadStart starter = v_Simulator.RunSimulation;
            starter += delegate
            {
                ResetUi(); //Reset UI once thread is finished.
            };
            Thread simulationThread = new Thread(starter) { IsBackground = true };
            simulationThread.Start(numberOfSteps);

            VisualizeOutput.AddText("Simulation started");
        }

        /// <summary>
        /// Runs multiple simulations and count compete cells every countStep. This test is designed for
        /// Septum objects
        /// </summary>
        /// <param name="numberOfRuns">Number of simulation runs</param>
        /// <param name="numberOfSteps">Number of simulation steps within each run</param>
        /// <param name="countStep">Count complete septums every step</param>
        /// <param name="tileName">Name of the tile.</param>
        /// <param name="numberOfKills">Number of object to be hurt.</param>
        /// <param name="probabilityOfTheKill">TODO</param>
        /// <param name="probabilistic">Flag for using random mechanism of choosing step.</param>
        private void RunMultipleSimulationsV2(int numberOfRuns, 
                                                int numberOfSteps, 
                                                int countStep, 
                                                string tileName,  
                                                double probabilityOfTheKill, 
                                                long numberOfKills = -1,
                                                bool probabilistic = true)
        {
            // ReSharper disable once UnusedVariable
            using (WaitCursor cursor = new WaitCursor())
            {
                VisualizeLogging.LogMessageAndVisualize(string.Format("MSystemStats multiple runs V2 started (numberOfRuns={0}, numberOfSteps={1}, countStep={2}, tileName={3}, probabilityOfTheKill={4}, numberOfKills={5}, probabilistic={6})", 
                    numberOfRuns, numberOfSteps, countStep, tileName, probabilityOfTheKill, numberOfKills, probabilistic));

                // write down headline
                string headLine = "STAT>> RunNo,StepNo,Fail";
                for (int i = 0; i < 41; i++)
                    headLine += string.Format(",T_{0}", i);
                VisualizeLogging.LogMessageAndVisualize(headLine);

                ulong runNo = 1;
                for (int i = 0; i < numberOfRuns; i++, runNo++)
                {
                    // create simulator
                    Simulator simulator;
                    try
                    {
                        simulator = new Simulator(v_MSystemObjects, v_SerializeFloatingObjects);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(string.Format("Failed to create simulator object in run no. {0}. ExceptionMessage: {1}", runNo, e.Message));
                        return;
                    }

                    // do one run
                    try
                    {
                        string res = "";
                        // numberOfKills is -1 => we want to choose the kills purely probabilisticly
                        if (numberOfKills == -1)
                        {
                            res = simulator.RunSimulationV2(numberOfSteps, countStep, runNo, tileName, probabilityOfTheKill);
                        }
                        else
                        {
                            res = simulator.RunSimulationV2(numberOfSteps, countStep, runNo, tileName, numberOfKills, probabilistic);
                        }
                        VisualizeLogging.LogMessageAndVisualize(res);
                    }
                    catch (Exception e)
                    {
                        // this particular run has failed but we continue
                        VisualizeLogging.LogMessageAndVisualize(string.Format("MSystemStats run no.{0} failed. Exception:{1}", runNo, e));
                    }
                }
            }
        }

            /// <summary>
            /// Runs multiple simulations with specified 
            /// </summary>
            /// <param name="numberOfRuns">Number of simulation runs</param>
            /// <param name="numberOfSteps">Number of simulation steps within each run</param>
            /// <param name="tileName">Name of the tile.</param>
            /// <param name="numberOfKills">Number of object to be hurt.</param>
            /// <param name="probabilityOfTheKill">TODO</param>
            /// <param name="probabilistic">Flag for using random mechanism of choosing step.</param>
        private void RunMultipleSimulations(int numberOfRuns, int numberOfSteps, string tileName,
            double probabilityOfTheKill, long numberOfKills = -1, bool probabilistic = true)
        {
            // ReSharper disable once UnusedVariable
            using (WaitCursor cursor = new WaitCursor())
            {
                VisualizeLogging.LogSimulationMessageAndVisualize(string.Format("MSystemStats multiple runs started (numberOfRuns={0}, numberOfSteps={1}, " +
                                                 "tileName={2}, probabilityOfTheKill={3}, numberOfKills={4}, probabilistic={5}",
                    numberOfRuns, numberOfSteps, tileName, probabilityOfTheKill, numberOfKills, probabilistic));

                List<MSystemStats> simulationStats = new List<MSystemStats>();
                ulong runNo = 1;
                for (int i = 0; i < numberOfRuns; i++, runNo++)
                {
                    // create simulator
                    Simulator simulator;
                    try
                    {
                        simulator = new Simulator(v_MSystemObjects, v_SerializeFloatingObjects);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(string.Format("Failed to create simulator object in run no. {0}. ExceptionMessage: {1}", runNo, e.Message));
                        return;
                    }

                    // do one run
                    try
                    {
                        // numberOfKills is -1 => we want to choose the kills purely probabilisticly
                        if (numberOfKills == -1)
                            simulator.RunSimulation(numberOfSteps, tileName, probabilityOfTheKill);
                        else
                            simulator.RunSimulation(numberOfSteps, tileName, numberOfKills, probabilistic);
                        MSystemStats systemStats = simulator.GetMSystemStats();
                        VisualizeLogging.LogSimulationMessageAndVisualize(systemStats.ToString()); //Log to simulation log as it is a part of simulation.
                        simulationStats.Add(systemStats);
                    }
                    catch (Exception)
                    {
                        // this particular run has failed but we continue
                        VisualizeLogging.LogSimulationMessageAndVisualize(string.Format("MSystemStats run no.{0} failed.", runNo));
                    }
                }
                // process stats - get average
                double sum = 0;
                foreach (MSystemStats element in simulationStats)
                    sum += element.GetFullCellsCount();
                double average = sum / simulationStats.Count;

                // process stats - get standard deviation
                sum = 0;
                foreach (MSystemStats element in simulationStats)
                    sum += Math.Pow(average - element.GetFullCellsCount(), 2);
                double stdDev = Math.Pow(sum / simulationStats.Count, 0.5); // square root
                // Write result to the log
                VisualizeLogging.LogSimulationMessageAndVisualize(string.Format("MSystemStats result: Runs = {0} Average = {1} Std. deviation = {2}", simulationStats.Count, average, stdDev));
            }
        }

        /// <summary>
        /// Restarts simulation engine.
        /// </summary>
        private void RestartSimulator()
        {
            VisualizeLogging.LogMessageAndVisualize("Simulator restart.");
            ResetUi();
            Simulator.SimulationCurrentState = Simulator.SimulationControlFlags.Stop;
            v_Simulator = new Simulator(v_MSystemObjects, v_SerializeFloatingObjects);

            if (!v_ReciverInitialized)
            {
                //Reciever initializing and creating binding for method which rises on PropertyChanged event - aka new message arrived.
                NotificationMessage notificationReciever = v_Simulator.GetNotificationReciever();
                notificationReciever.PropertyChanged += VisualizeNotification;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Main method.
        /// </summary>
        public Main()
        {
            v_XSDPath = ConfigurationManager.AppSettings["XSDPath"];
            v_ValidateUsingXSD = ConfigurationManager.AppSettings["validateUsingXSD"].ToLower().Equals("true");
            v_SerializeFloatingObjects = ConfigurationManager.AppSettings["serializeFloatningObjects"].ToLower().Equals("true");
            InitializeComponent();
        }

        /// <summary>
        /// Used for added any text to output richTextBox.
        /// </summary>
        /// <param name="text">Input text which will be vizualized.</param>
        public void AddTextToOutput(string text)
        {
            if (InvokeRequired)
            {
                MethodInvoker mi = delegate { AddTextToOutput(text); };
                Invoke(mi);
            }
            else
            {
                richTextBoxOutput.AppendText(string.Format("{0}\n", text));
                richTextBoxOutput.ScrollToCaret();
            }
        }

        /// <summary>
        /// Vizualizes message from notification reciever.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void VisualizeNotification(object sender, PropertyChangedEventArgs e)
        {
            NotificationMessage notification = TypeUtil.TryCast<NotificationMessage>(sender);
            if (notification != null)
            {
                VisualizeLogging.LogMessageAndVisualize(notification.Message);
            }
        }

        /// <summary>
        /// Shows information message how to run simulation.
        /// </summary>
        private void ShowLoadSimulationMessage()
        {
            MessageBox.Show("First load M System description.\nGo to: File -> Load example -> M System description.");
        }

        #endregion

        /// <summary>
        /// --- One Off Damage Test - Click event ---
        /// Runs one off Damage Test
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void oneOffDamageTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get test parameters from user
            InputBox inputBox = new InputBox("One Off Damage Test", "OK",
                new List<string> { "Number of runs", 
                                   "Tile name to remove (empty if it does not matter)", 
                                   "Number of tiles to remove", 
                                   "Remove only Polygon3D tiles (Y/N) ?",
                                   "Number of recovery steps" },
                new List<Regexp.Check> { Regexp.Check.NumberWithoutZero, 
                                         Regexp.Check.Skip, 
                                         Regexp.Check.NumberWithoutZero, 
                                         Regexp.Check.String,
                                         Regexp.Check.NumberWithoutZero });
            inputBox.ShowDialog();
            if (inputBox.OutputTexts.Count == 5)
            {
                // get number of runs
                int numberOfRuns = int.Parse(inputBox.OutputTexts[0]);
                // what object will be removed
                string tileName = inputBox.OutputTexts[1];
                // number of tiles to remove
                int numberOfTiles = int.Parse(inputBox.OutputTexts[2]);
                // do we want to remove only Pologon3D tiles
                if (inputBox.OutputTexts[3] != "Y" && inputBox.OutputTexts[3] != "N")
                {
                    MessageBox.Show("Invalis selection for \"Remove only Polygon3D tiles(Y/N) ?\". Allowed values are Y or N.");
                    return;
                }
                bool onlyPologon3DTiles = inputBox.OutputTexts[3] == "Y";
                // number of further steps to follow after the demage
                int numberOfRecoverySteps = int.Parse(inputBox.OutputTexts[4]);
                // run the test
                RunOneOffDamageTest(numberOfRuns, tileName, numberOfTiles, onlyPologon3DTiles, numberOfRecoverySteps);
            }

        }

        /// <summary>
        /// Run One Off Damage test. For more information, see test description document.
        /// </summary>
        /// <param name="numberOfRuns">Number of runs (experiments) to perform.</param>
        /// <param name="tileName">Name of the targeted tile, empty string means any randomly chosen tile.</param>
        /// <param name="numberOfTiles">Number of tiles to remove.</param>
        /// <param name="onlyPologon3DTiles">If TRUE then only tiles of type Pologon3D type are being removed</param>
        /// <param name="numberOfRecoverySteps">Number of system iterations given the system to recover.</param>
        private void RunOneOffDamageTest(int numberOfRuns, string tileName, int numberOfTiles, bool onlyPologon3DTiles, int numberOfRecoverySteps)
        {
            // ReSharper disable once UnusedVariable
            using (WaitCursor cursor = new WaitCursor())
            {
                VisualizeLogging.LogMessageAndVisualize(string.Format("RunOneOffDamageTest started (numberOfRuns={0}, tileName={1}, numberOfTiles={2}, numberOfRecoverySteps = {3})",
                    numberOfRuns, tileName, numberOfTiles, numberOfRecoverySteps));

                // write down headline
                string headLine = "STAT>> RunNo,InitialStepNo,FurtherStepNo";
                for (int i = 0; i < 41; i++)
                    headLine += string.Format(",T_{0}", i);
                VisualizeLogging.LogMessageAndVisualize(headLine);

                for (int i = 0; i < numberOfRuns; i++)
                {
                    // create simulator
                    Simulator simulator;
                    try
                    {
                        simulator = new Simulator(v_MSystemObjects, v_SerializeFloatingObjects);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(string.Format("Failed to create simulator object in run no. {0}. ExceptionMessage: {1}", i+1, e.Message));
                        return;
                    }

                    // do one run
                    try
                    {
                        string res = simulator.RunSimulationOneOffDamage(i+1, tileName, numberOfTiles, onlyPologon3DTiles, numberOfRecoverySteps);
                        VisualizeLogging.LogMessageAndVisualize(res);
                    }
                    catch (Exception e)
                    {
                        // this particular run has failed but we continue
                        VisualizeLogging.LogMessageAndVisualize(string.Format("MSystemStats run no.{0} failed. Exception:{1}", i+1, e));
                    }
                }
            }
        }

    }
}
