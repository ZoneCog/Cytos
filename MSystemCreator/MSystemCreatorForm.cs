using MSystemCreator.Classes;
using SharedComponents.Xml;
using SharedComponents.XML;
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MSystemCreator
{
    /// <summary>
    /// M System creator form.
    /// </summary>
    public partial class MSystemCreatorForm : Form
    {

        #region Public data

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        #endregion

        #region Private data

        /// <summary>
        /// Name of the program.
        /// </summary>
        private const string c_MSystemCreator = "M System Creator{0}{1}";

        /// <summary>
        /// Main object for XML serialization.
        /// </summary>
        private SerializeMSystem v_MSystem;

        /// <summary>
        /// XSD path loaded from config.
        /// </summary>
        private readonly string v_XSDPath;

        /// <summary>
        /// Flag which holds state of XML - saved or in memory.
        /// </summary>
        private bool v_IsXmlSaved;

        /// <summary>
        /// Flag for show/hide hints
        /// </summary>
        private bool v_HintsAreHidden;

        /// <summary>
        /// Flag for handling manual modifications.
        /// </summary>
        private bool v_ManuallyModified;

        /// <summary>
        /// Path of saved XML.
        /// </summary>
        private string v_FilePath;

        #endregion

        #region Constructor

        /// <summary>
        /// Main method.
        /// </summary>
        public MSystemCreatorForm()
        {
            InitializeComponent();
            InitializeTooltips();
            CreateHintsTexts();
            toolStripButtonAddManualModifications.Visible = false;
            CreateTitle(" - ");

            v_XSDPath = ConfigurationManager.AppSettings["XSDPath"];

            Closing += OnClosing; //OnClosing event action.

            v_MSystem = new SerializeMSystem();
            ResetXmlAndUi(false, string.Empty);
        }

        #endregion

        #region Private methods

        #region UI events

        /// <summary>
        /// Adds tile using polygon.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonTile_Click(object sender, EventArgs e)
        {
            v_MSystem.CreateTile();
        }

        /// <summary>
        /// Adds tile using vertices.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonTileVertices_Click(object sender, EventArgs e)
        {
            v_MSystem.CreateTileUsingVertices();
        }

        /// <summary>
        /// Adds connector for tile.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonConnector_Click(object sender, EventArgs e)
        {
            if (!v_MSystem.MSystemXml.Root.GetElement("tiling/tiles").ExistsElement("tile"))
            {
                MessageBox.Show("At least one tile must exist!");
                return;
            }

            if (!v_MSystem.CreateConnector(out string errorMessage))
            {
                MessageBox.Show(errorMessage);
            }
        }

        /// <summary>
        /// Adds glue.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonGlue_Click(object sender, EventArgs e)
        {
            v_MSystem.CreateGlue();
        }

        /// <summary>
        /// Adds glue relation.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonGlueRelation_Click(object sender, EventArgs e)
        {
            v_MSystem.CreateGlueRelation();
        }

        /// <summary>
        /// Adds initial object.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonInitialObject_Click(object sender, EventArgs e)
        {
            v_MSystem.CreateInitialObject();
        }

        /// <summary>
        /// Adds glue radius.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonGlueRadius_Click(object sender, EventArgs e)
        {
            if (!v_MSystem.CreateGlueRadius(out string errorMessage))
            {
                MessageBox.Show(errorMessage);
            }
        }

        /// <summary>s
        /// Adds battery voltage object.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonBatteryVoltage_Click(object sender, EventArgs e)
        {
            if (!v_MSystem.CreateBatteryVoltage(out string errorMessage))
            {
                MessageBox.Show(errorMessage);
            }
        }

        /// <summary>s
        /// Adds threshold potential object.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonThresholdPotential_Click(object sender, EventArgs e)
        {
            if (!v_MSystem.CreateThresholdPotential(out string errorMessage))
            {
                MessageBox.Show(errorMessage);
            }
        }

        /// <summary>s
        /// Adds pushing coefficient object.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonPushingCoef_Click(object sender, EventArgs e)
        {
            if (!v_MSystem.CreatePushingCoef(out string errorMessage))
            {
                MessageBox.Show(errorMessage);
            }
        }

        /// <summary>
        /// Adds floating object.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonFloatingObject_Click(object sender, EventArgs e)
        {
            v_MSystem.CreateFloatingObject();
        }

        /// <summary>
        /// Adds protein.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonProtein_Click(object sender, EventArgs e)
        {
            v_MSystem.CreateProtein();
        }

        /// <summary>
        /// Adds protein on tile.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonProteinOnTile_Click(object sender, EventArgs e)
        {
            v_MSystem.CreateProteinOnTile();
        }

        /// <summary>
        /// Adds evo rule.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonEvoRule_Click(object sender, EventArgs e)
        {

            v_MSystem.CreateEvoRule();
        }

        /// <summary>s
        /// Adds signal object.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonSignalObject_Click(object sender, EventArgs e)
        {
            v_MSystem.CreateSignalObject();
        }

        /// <summary>s
        /// Adds reaction radius object.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void buttonReactionRadius_Click(object sender, EventArgs e)
        {
            if (!v_MSystem.CreateReactionRadius(out string errorMessage))
            {
                MessageBox.Show(errorMessage);
            }
        }

        /// <summary>
        /// Hides/shows hints.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void toolStripButtonHideShowHints_Click(object sender, EventArgs e)
        {
            if (!v_HintsAreHidden)
            {
                richTextBoxHintsTiling.Visible = false;
                richTextBoxHintsMSystem.Visible = false;

                toolStripButtonHideShowHints.Text = "Show hints";
                v_HintsAreHidden = true;
            }
            else
            {
                richTextBoxHintsTiling.Visible = true;
                richTextBoxHintsMSystem.Visible = true;

                toolStripButtonHideShowHints.Text = "Hide hints";
                v_HintsAreHidden = false;
            }
        }

        /// <summary>
        /// Event when XML text is changed.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void richTextBoxXML_TextChanged(object sender, EventArgs e)
        {
            if (v_ManuallyModified && !toolStripButtonAddManualModifications.Visible)
            {
                toolStripButtonAddManualModifications.Visible = true;
            }
            else
            {
                //Reset flag. Code automatically set flag to false.
                v_ManuallyModified = true;
            }
        }

        /// <summary>
        /// Adds manually modifed XML into base object.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void toolStripButtonAddManualModifications_Click(object sender, EventArgs e)
        {
            toolStripButtonAddManualModifications.Visible = false;
            if (v_MSystem.UpdateXmlWithManualModifications(richTextBoxXML.Text, out string errorMessage))
            {
                v_MSystem.MSystemXml.Changed += XmlChanged;//Document changed event action must be set again
                CreateTitle(" - *");
                MessageBox.Show("Modifications were sucessfully added.");
            }
            else
            {
                MessageBox.Show($"An error occured: {errorMessage}");
            }
        }

        /// <summary>
        /// Creates new M System XML.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void newMSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!v_IsXmlSaved)
            {
                var confirmResult = MessageBox.Show("You are going to create new M System XML, your current XML will be deleted. Are you sure?",
                    "New M System XML?",
                    MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.No)
                {
                    return;
                }
            }

            v_MSystem = new SerializeMSystem();
            ResetXmlAndUi(false, string.Empty);
        }

        /// <summary>
        /// Creates XML file.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void saveMSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "XML files (*.xml)|*.xml";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    v_MSystem.MSystemXml.Save(saveFileDialog.FileName);
                    v_FilePath = saveFileDialog.FileName;
                    v_IsXmlSaved = true;
                    CreateTitle(" - ");
                }
            }
        }

        /// <summary>
        /// Loads existing M System description into editor.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void loadMSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!v_IsXmlSaved)
            {
                var confirmResult = MessageBox.Show("You are going to load new M System XML, your current XML will be deleted. Are you sure?",
                    "New M System XML?",
                    MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.No)
                {
                    return;
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "xml files (*.xml)|*.xml",
                Title = "Please select M System desription file.",
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;

                if (!string.IsNullOrEmpty(path))
                {
                    if (v_MSystem.LoadExistingXmlFromFile(path, out string errorMessage))
                    {
                        ResetXmlAndUi(true, path);
                    }
                    else
                    {
                        MessageBox.Show($"An error occured: {errorMessage}");
                    }
                }
            }
        }

        /// <summary>
        /// Validates created XML.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void toolStripButtonValidateXML_Click(object sender, EventArgs e)
        {
            MessageBox.Show(XmlValidator.Validate(v_MSystem.MSystemXml, v_XSDPath, out string errorsAndWarnings) ? "XML is valid." : errorsAndWarnings);
        }

        #endregion

        /// <summary>
        /// Sets save flag to false.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private void XmlChanged(object sender, XObjectChangeEventArgs e)
        {
            v_IsXmlSaved = false;
            CreateTitle(" - *");
            RefreshRichtTextBoxOutput();
        }

        /// <summary>
        /// Refreshes output of created XML.
        /// </summary>
        private void RefreshRichtTextBoxOutput()
        {
            v_ManuallyModified = false;
            richTextBoxXML.Text = v_MSystem.MSystemXml.ToString();
        }

        /// <summary>
        /// On close event.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="cancelEventArgs">Cancelation event arguments.</param>
        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (!v_IsXmlSaved)
            {
                string msg = "You have unsaved M System description.\n\nDo you still want to close this window?";
                DialogResult result = MessageBox.Show(msg, "Close Confirmation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:
                        //Close confirmed.
                        break;
                    case DialogResult.No:
                        //Close rejected.
                        cancelEventArgs.Cancel = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Sets XML examples into each button.
        /// </summary>
        private void InitializeTooltips()
        {
            ToolTip toolTip = new ToolTip();

            //Tile - using polygon
            toolTip.SetToolTip(buttonTilePolygon, "<tile name=\"q0\">\n    <polygon>\n      <sides value=\"5\"/>\n      " +
                                                  "<radius value=\"10\"/>\n    </polygon>\n  <positions>\n    <position " +
                                                  "name=\"center\">\n      <posX value=\"0\"/>\n      <posY value=\"0\"/>" +
                                                  "\n    </position>\n  </positions>\n  <connectingAngle value=" +
                                                  "\"2.034443935795703\" unit=\"rad\"/>\n  <connectors />\n  <surfaceGlue " +
                                                  "name=\"gx\"/>\n  <alphaRation value=\"0.1\"/>\n  <color name=\"DeepSkyBlue\" alpha=\"64\"/>" +
                                                  "\n  <thickness value=\"0.1\"/>\n</tile>\n");
            //Connector
            toolTip.SetToolTip(buttonConnector, "<connector name=\"c7\">\n  <positions>\n    <position name=\"center\"/>\n  " +
                                                "</positions>\n  <glue name=\"g0\"/>\n  <angle value=\"90\" unit=\"deg\"/>" +
                                                "\n  <resistance value=\"10\"/>\n</connector>\n");
            //Glue
            toolTip.SetToolTip(buttonGlue, "<glue name=\"g0\"/>");
            //Glue relation
            toolTip.SetToolTip(buttonGlueRelation, "<glueTuple glue1=\"g0\" glue2=\"g1\"/>");
            //Initial object
            toolTip.SetToolTip(buttonInitialObject, "(AT LEAST ONE OBJECT IS REQUIRED)\n<initialObject name=\"q0\">\n  <posX value=\"0\"/>\n  <posY value=\"0\"/>\n  " +
                                                    "<posZ value=\"0\"/>\n  <angleX value=\"0\"/>\n  <angleY value=\"0\"/>\n  " +
                                                    "<angleZ value=\"0\"/>\n</initialObject>\n");
            //Glue radius
            toolTip.SetToolTip(buttonGlueRadius, "(OPTIONAL)\n<glueRadius value=\"0.1\"/>");
            //Floating object
            toolTip.SetToolTip(buttonFloatingObject, "<floatingObject name=\"a\">\n  <shape value=\"sphere\"/>\n  <size value=\"0.05\"/>" +
                                                     "\n  <color name=\"Lime\" alpha =\"255\"/>\n  <mobility value=\"5\"/>\n  " +
                                                     "<concentration value=\"0.1\"/>\n</floatingObject>\n");
            //Protein
            toolTip.SetToolTip(buttonProtein, "<protein name=\"p0\"/>");
            //Protein on Tile
            toolTip.SetToolTip(buttonProteinOnTile, "<tile name=\"q2\">\n  <protein name=\"p0\">\n    <posX value=\"2\"/>\n    " +
                                                    "<posY value=\"0\"/>\n  </protein>\n</tile>\n");
            //Evo rule
            toolTip.SetToolTip(buttonEvoRule, "<evoRule type=\"Metabolic\">\n  <leftside value =\"a,p0\"/>\n  <rightside " +
                                              "value= \"p0,a\"/>\n</evoRule>\n");
            //Signal object
            toolTip.SetToolTip(buttonSignalObject, "<glueTuple glue1=\"g4\" glue2=\"gt\">\n  <objects value=\"x,x,x\"/>\n</glueTuple>\n");

            //Theshold potential 
            toolTip.SetToolTip(buttonThresholdPotential, "(OPTIONAL)\n<thresholdPotential value = \"1\"/>");

            //Battery voltage
            toolTip.SetToolTip(buttonBatteryVoltage, "(OPTIONAL)\n<batteryVoltage value = \"1\"/>");

            //Reaction radius 
            toolTip.SetToolTip(buttonReactionRadius, "(OPTIONAL)\n<reactionRadius value = \"14.5\"/>");

            //Pushing coefficient
            toolTip.SetToolTip(buttonPushingCoef, "(OPTIONAL)\n<pushingCoef value=\"0.1\"/>");

        }

        /// <summary>
        /// Creates user hints for creating M System description.
        /// </summary>
        private void CreateHintsTexts()
        {
            StringBuilder tilingHints = new StringBuilder();
            StringBuilder mSystemHints = new StringBuilder();

            tilingHints.AppendLine("TILING HINTS:");
            tilingHints.AppendLine(string.Empty);
            tilingHints.AppendLine("- Tile with specific name must exists before connector is created");
            tilingHints.AppendLine("- At least one initial object must exist");
            tilingHints.AppendLine("- Glue radius(1), Battery voltage(2), Threshold potential(3) and Pushing coef.(4) are optional parameters however their order is fixed. " +
                                   "Using only (1) and (3) is valid, order (3) and then (1) will lead into XSD validation error.");

            mSystemHints.AppendLine(" M SYSTEM HINTS:");
            mSystemHints.AppendLine(string.Empty);
            mSystemHints.AppendLine("- Evo rules types are: Metabolic, Create, Insert, Divide and Destroy");
            mSystemHints.AppendLine("- Reaction radius is optional parameter");
            richTextBoxHintsTiling.Text = tilingHints.ToString();
            richTextBoxHintsMSystem.Text = mSystemHints.ToString();
        }

        /// <summary>
        /// Creates title(text) of the program.
        /// </summary>
        /// <param name="separator">Separator between program name and file name.</param>
        private void CreateTitle(string separator)
        {
            string fileName = "Unsaved document";
            if (!string.IsNullOrEmpty(v_FilePath))
            {
                fileName = Path.GetFileNameWithoutExtension(v_FilePath);
            }

            Text = string.Format(c_MSystemCreator, separator, fileName);
        }

        /// <summary>
        /// Resets UI with new XML.
        /// </summary>
        /// <param name="isXmlSaved">Flag if file is saved</param>
        /// <param name="filePath">File path to XML, if no path, use empty string</param>
        /// <param name="separator">Separator, by default " - "</param>
        private void ResetXmlAndUi(bool isXmlSaved, string filePath, string separator = " - ")
        {
            v_MSystem.MSystemXml.Changed += XmlChanged; //Document changed event action must be set again
            RefreshRichtTextBoxOutput(); //XmlChanged event is not called when document is created, must be refreshed manualy.
            v_IsXmlSaved = isXmlSaved;
            v_FilePath = filePath;
            CreateTitle(separator);
        }

        #endregion

    }
}