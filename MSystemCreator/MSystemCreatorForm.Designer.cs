namespace MSystemCreator
{
    partial class MSystemCreatorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSystemCreatorForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.richTextBoxHintsTiling = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonPushingCoef = new System.Windows.Forms.Button();
            this.buttonBatteryVoltage = new System.Windows.Forms.Button();
            this.buttonThresholdPotential = new System.Windows.Forms.Button();
            this.buttonConnector = new System.Windows.Forms.Button();
            this.buttonTileVertices = new System.Windows.Forms.Button();
            this.buttonGlueRadius = new System.Windows.Forms.Button();
            this.buttonInitialObject = new System.Windows.Forms.Button();
            this.buttonGlueRelation = new System.Windows.Forms.Button();
            this.buttonGlue = new System.Windows.Forms.Button();
            this.buttonTilePolygon = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.richTextBoxHintsMSystem = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonReactionRadius = new System.Windows.Forms.Button();
            this.buttonEvoRule = new System.Windows.Forms.Button();
            this.buttonSignalObject = new System.Windows.Forms.Button();
            this.buttonProteinOnTile = new System.Windows.Forms.Button();
            this.buttonProtein = new System.Windows.Forms.Button();
            this.buttonFloatingObject = new System.Windows.Forms.Button();
            this.richTextBoxXML = new System.Windows.Forms.RichTextBox();
            this.toolStripMainPanel = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonHideShowHints = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddManualModifications = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.newMSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonValidateXML = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStripMainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 34);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(144, 609);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richTextBoxHintsTiling);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(136, 583);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tiling";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // richTextBoxHintsTiling
            // 
            this.richTextBoxHintsTiling.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxHintsTiling.Location = new System.Drawing.Point(6, 321);
            this.richTextBoxHintsTiling.Name = "richTextBoxHintsTiling";
            this.richTextBoxHintsTiling.Size = new System.Drawing.Size(121, 256);
            this.richTextBoxHintsTiling.TabIndex = 1;
            this.richTextBoxHintsTiling.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonPushingCoef);
            this.groupBox1.Controls.Add(this.buttonBatteryVoltage);
            this.groupBox1.Controls.Add(this.buttonThresholdPotential);
            this.groupBox1.Controls.Add(this.buttonConnector);
            this.groupBox1.Controls.Add(this.buttonTileVertices);
            this.groupBox1.Controls.Add(this.buttonGlueRadius);
            this.groupBox1.Controls.Add(this.buttonInitialObject);
            this.groupBox1.Controls.Add(this.buttonGlueRelation);
            this.groupBox1.Controls.Add(this.buttonGlue);
            this.groupBox1.Controls.Add(this.buttonTilePolygon);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(121, 309);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add components";
            // 
            // buttonPushingCoef
            // 
            this.buttonPushingCoef.Location = new System.Drawing.Point(8, 278);
            this.buttonPushingCoef.Name = "buttonPushingCoef";
            this.buttonPushingCoef.Size = new System.Drawing.Size(107, 23);
            this.buttonPushingCoef.TabIndex = 9;
            this.buttonPushingCoef.Text = "Pushing coefficient";
            this.buttonPushingCoef.UseVisualStyleBackColor = true;
            this.buttonPushingCoef.Click += new System.EventHandler(this.buttonPushingCoef_Click);
            // 
            // buttonBatteryVoltage
            // 
            this.buttonBatteryVoltage.Location = new System.Drawing.Point(6, 222);
            this.buttonBatteryVoltage.Name = "buttonBatteryVoltage";
            this.buttonBatteryVoltage.Size = new System.Drawing.Size(107, 23);
            this.buttonBatteryVoltage.TabIndex = 7;
            this.buttonBatteryVoltage.Text = "Battery voltage";
            this.buttonBatteryVoltage.UseVisualStyleBackColor = true;
            this.buttonBatteryVoltage.Click += new System.EventHandler(this.buttonBatteryVoltage_Click);
            // 
            // buttonThresholdPotential
            // 
            this.buttonThresholdPotential.Location = new System.Drawing.Point(8, 251);
            this.buttonThresholdPotential.Name = "buttonThresholdPotential";
            this.buttonThresholdPotential.Size = new System.Drawing.Size(107, 23);
            this.buttonThresholdPotential.TabIndex = 8;
            this.buttonThresholdPotential.Text = "Threshold potential";
            this.buttonThresholdPotential.UseVisualStyleBackColor = true;
            this.buttonThresholdPotential.Click += new System.EventHandler(this.buttonThresholdPotential_Click);
            // 
            // buttonConnector
            // 
            this.buttonConnector.Location = new System.Drawing.Point(6, 77);
            this.buttonConnector.Name = "buttonConnector";
            this.buttonConnector.Size = new System.Drawing.Size(107, 23);
            this.buttonConnector.TabIndex = 2;
            this.buttonConnector.Text = "Connector for tile";
            this.buttonConnector.UseVisualStyleBackColor = true;
            this.buttonConnector.Click += new System.EventHandler(this.buttonConnector_Click);
            // 
            // buttonTileVertices
            // 
            this.buttonTileVertices.Enabled = false;
            this.buttonTileVertices.Location = new System.Drawing.Point(6, 48);
            this.buttonTileVertices.Name = "buttonTileVertices";
            this.buttonTileVertices.Size = new System.Drawing.Size(107, 23);
            this.buttonTileVertices.TabIndex = 1;
            this.buttonTileVertices.Text = "Tile - using vertices";
            this.buttonTileVertices.UseVisualStyleBackColor = true;
            this.buttonTileVertices.Click += new System.EventHandler(this.buttonTileVertices_Click);
            // 
            // buttonGlueRadius
            // 
            this.buttonGlueRadius.Location = new System.Drawing.Point(6, 193);
            this.buttonGlueRadius.Name = "buttonGlueRadius";
            this.buttonGlueRadius.Size = new System.Drawing.Size(107, 23);
            this.buttonGlueRadius.TabIndex = 6;
            this.buttonGlueRadius.Text = "Glue radius";
            this.buttonGlueRadius.UseVisualStyleBackColor = true;
            this.buttonGlueRadius.Click += new System.EventHandler(this.buttonGlueRadius_Click);
            // 
            // buttonInitialObject
            // 
            this.buttonInitialObject.Location = new System.Drawing.Point(6, 164);
            this.buttonInitialObject.Name = "buttonInitialObject";
            this.buttonInitialObject.Size = new System.Drawing.Size(107, 23);
            this.buttonInitialObject.TabIndex = 5;
            this.buttonInitialObject.Text = "Initial object";
            this.buttonInitialObject.UseVisualStyleBackColor = true;
            this.buttonInitialObject.Click += new System.EventHandler(this.buttonInitialObject_Click);
            // 
            // buttonGlueRelation
            // 
            this.buttonGlueRelation.Location = new System.Drawing.Point(6, 135);
            this.buttonGlueRelation.Name = "buttonGlueRelation";
            this.buttonGlueRelation.Size = new System.Drawing.Size(107, 23);
            this.buttonGlueRelation.TabIndex = 4;
            this.buttonGlueRelation.Text = "Glue relation";
            this.buttonGlueRelation.UseVisualStyleBackColor = true;
            this.buttonGlueRelation.Click += new System.EventHandler(this.buttonGlueRelation_Click);
            // 
            // buttonGlue
            // 
            this.buttonGlue.Location = new System.Drawing.Point(6, 106);
            this.buttonGlue.Name = "buttonGlue";
            this.buttonGlue.Size = new System.Drawing.Size(107, 23);
            this.buttonGlue.TabIndex = 3;
            this.buttonGlue.Text = "Glue";
            this.buttonGlue.UseVisualStyleBackColor = true;
            this.buttonGlue.Click += new System.EventHandler(this.buttonGlue_Click);
            // 
            // buttonTilePolygon
            // 
            this.buttonTilePolygon.Location = new System.Drawing.Point(6, 19);
            this.buttonTilePolygon.Name = "buttonTilePolygon";
            this.buttonTilePolygon.Size = new System.Drawing.Size(107, 23);
            this.buttonTilePolygon.TabIndex = 0;
            this.buttonTilePolygon.Text = "Tile - using polygon";
            this.buttonTilePolygon.UseVisualStyleBackColor = true;
            this.buttonTilePolygon.Click += new System.EventHandler(this.buttonTile_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.richTextBoxHintsMSystem);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(136, 583);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "M System";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBoxHintsMSystem
            // 
            this.richTextBoxHintsMSystem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxHintsMSystem.Location = new System.Drawing.Point(6, 206);
            this.richTextBoxHintsMSystem.Name = "richTextBoxHintsMSystem";
            this.richTextBoxHintsMSystem.Size = new System.Drawing.Size(121, 371);
            this.richTextBoxHintsMSystem.TabIndex = 2;
            this.richTextBoxHintsMSystem.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonReactionRadius);
            this.groupBox2.Controls.Add(this.buttonEvoRule);
            this.groupBox2.Controls.Add(this.buttonSignalObject);
            this.groupBox2.Controls.Add(this.buttonProteinOnTile);
            this.groupBox2.Controls.Add(this.buttonProtein);
            this.groupBox2.Controls.Add(this.buttonFloatingObject);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(121, 194);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Add components";
            // 
            // buttonReactionRadius
            // 
            this.buttonReactionRadius.Location = new System.Drawing.Point(8, 164);
            this.buttonReactionRadius.Name = "buttonReactionRadius";
            this.buttonReactionRadius.Size = new System.Drawing.Size(107, 23);
            this.buttonReactionRadius.TabIndex = 6;
            this.buttonReactionRadius.Text = "Reaction radius";
            this.buttonReactionRadius.UseVisualStyleBackColor = true;
            this.buttonReactionRadius.Click += new System.EventHandler(this.buttonReactionRadius_Click);
            // 
            // buttonEvoRule
            // 
            this.buttonEvoRule.Location = new System.Drawing.Point(6, 106);
            this.buttonEvoRule.Name = "buttonEvoRule";
            this.buttonEvoRule.Size = new System.Drawing.Size(107, 23);
            this.buttonEvoRule.TabIndex = 3;
            this.buttonEvoRule.Text = "Evo rule";
            this.buttonEvoRule.UseVisualStyleBackColor = true;
            this.buttonEvoRule.Click += new System.EventHandler(this.buttonEvoRule_Click);
            // 
            // buttonSignalObject
            // 
            this.buttonSignalObject.Location = new System.Drawing.Point(8, 135);
            this.buttonSignalObject.Name = "buttonSignalObject";
            this.buttonSignalObject.Size = new System.Drawing.Size(107, 23);
            this.buttonSignalObject.TabIndex = 5;
            this.buttonSignalObject.Text = "Signal object";
            this.buttonSignalObject.UseVisualStyleBackColor = true;
            this.buttonSignalObject.Click += new System.EventHandler(this.buttonSignalObject_Click);
            // 
            // buttonProteinOnTile
            // 
            this.buttonProteinOnTile.Location = new System.Drawing.Point(6, 77);
            this.buttonProteinOnTile.Name = "buttonProteinOnTile";
            this.buttonProteinOnTile.Size = new System.Drawing.Size(107, 23);
            this.buttonProteinOnTile.TabIndex = 2;
            this.buttonProteinOnTile.Text = "Protein on tile";
            this.buttonProteinOnTile.UseVisualStyleBackColor = true;
            this.buttonProteinOnTile.Click += new System.EventHandler(this.buttonProteinOnTile_Click);
            // 
            // buttonProtein
            // 
            this.buttonProtein.Location = new System.Drawing.Point(6, 48);
            this.buttonProtein.Name = "buttonProtein";
            this.buttonProtein.Size = new System.Drawing.Size(107, 23);
            this.buttonProtein.TabIndex = 1;
            this.buttonProtein.Text = "Protein";
            this.buttonProtein.UseVisualStyleBackColor = true;
            this.buttonProtein.Click += new System.EventHandler(this.buttonProtein_Click);
            // 
            // buttonFloatingObject
            // 
            this.buttonFloatingObject.Location = new System.Drawing.Point(6, 19);
            this.buttonFloatingObject.Name = "buttonFloatingObject";
            this.buttonFloatingObject.Size = new System.Drawing.Size(107, 23);
            this.buttonFloatingObject.TabIndex = 0;
            this.buttonFloatingObject.Text = "Floating object";
            this.buttonFloatingObject.UseVisualStyleBackColor = true;
            this.buttonFloatingObject.Click += new System.EventHandler(this.buttonFloatingObject_Click);
            // 
            // richTextBoxXML
            // 
            this.richTextBoxXML.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxXML.Location = new System.Drawing.Point(162, 34);
            this.richTextBoxXML.Name = "richTextBoxXML";
            this.richTextBoxXML.Size = new System.Drawing.Size(404, 609);
            this.richTextBoxXML.TabIndex = 1;
            this.richTextBoxXML.Text = "";
            this.richTextBoxXML.TextChanged += new System.EventHandler(this.richTextBoxXML_TextChanged);
            // 
            // toolStripMainPanel
            // 
            this.toolStripMainPanel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddManualModifications,
            this.toolStripDropDownButton1,
            this.toolStripSeparator1,
            this.toolStripButtonHideShowHints,
            this.toolStripSeparator2,
            this.toolStripButtonValidateXML});
            this.toolStripMainPanel.Location = new System.Drawing.Point(0, 0);
            this.toolStripMainPanel.Name = "toolStripMainPanel";
            this.toolStripMainPanel.Size = new System.Drawing.Size(578, 25);
            this.toolStripMainPanel.TabIndex = 3;
            this.toolStripMainPanel.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonHideShowHints
            // 
            this.toolStripButtonHideShowHints.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonHideShowHints.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonHideShowHints.Image")));
            this.toolStripButtonHideShowHints.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonHideShowHints.Name = "toolStripButtonHideShowHints";
            this.toolStripButtonHideShowHints.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripButtonHideShowHints.Size = new System.Drawing.Size(65, 22);
            this.toolStripButtonHideShowHints.Text = "Hide hints";
            this.toolStripButtonHideShowHints.Click += new System.EventHandler(this.toolStripButtonHideShowHints_Click);
            // 
            // toolStripButtonAddManualModifications
            // 
            this.toolStripButtonAddManualModifications.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonAddManualModifications.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonAddManualModifications.Font = new System.Drawing.Font("Segoe UI", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripButtonAddManualModifications.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddManualModifications.Image")));
            this.toolStripButtonAddManualModifications.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddManualModifications.Name = "toolStripButtonAddManualModifications";
            this.toolStripButtonAddManualModifications.Size = new System.Drawing.Size(197, 22);
            this.toolStripButtonAddManualModifications.Text = "Add manual modifications to XML";
            this.toolStripButtonAddManualModifications.Click += new System.EventHandler(this.toolStripButtonAddManualModifications_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMSystemToolStripMenuItem,
            this.saveMSystemToolStripMenuItem,
            this.loadMSystemToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(72, 22);
            this.toolStripDropDownButton1.Text = "M System";
            // 
            // newMSystemToolStripMenuItem
            // 
            this.newMSystemToolStripMenuItem.Name = "newMSystemToolStripMenuItem";
            this.newMSystemToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.newMSystemToolStripMenuItem.Text = "Create empty";
            this.newMSystemToolStripMenuItem.Click += new System.EventHandler(this.newMSystemToolStripMenuItem_Click);
            // 
            // saveMSystemToolStripMenuItem
            // 
            this.saveMSystemToolStripMenuItem.Name = "saveMSystemToolStripMenuItem";
            this.saveMSystemToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.saveMSystemToolStripMenuItem.Text = "Save current";
            this.saveMSystemToolStripMenuItem.Click += new System.EventHandler(this.saveMSystemToolStripMenuItem_Click);
            // 
            // loadMSystemToolStripMenuItem
            // 
            this.loadMSystemToolStripMenuItem.Name = "loadMSystemToolStripMenuItem";
            this.loadMSystemToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.loadMSystemToolStripMenuItem.Text = "Load existing";
            this.loadMSystemToolStripMenuItem.Click += new System.EventHandler(this.loadMSystemToolStripMenuItem_Click);
            // 
            // toolStripButtonValidateXML
            // 
            this.toolStripButtonValidateXML.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonValidateXML.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonValidateXML.Image")));
            this.toolStripButtonValidateXML.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonValidateXML.Name = "toolStripButtonValidateXML";
            this.toolStripButtonValidateXML.Size = new System.Drawing.Size(79, 22);
            this.toolStripButtonValidateXML.Text = "Validate XML";
            this.toolStripButtonValidateXML.Click += new System.EventHandler(this.toolStripButtonValidateXML_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // MSystemCreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 655);
            this.Controls.Add(this.toolStripMainPanel);
            this.Controls.Add(this.richTextBoxXML);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(594, 694);
            this.Name = "MSystemCreatorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "M System Creator";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.toolStripMainPanel.ResumeLayout(false);
            this.toolStripMainPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonGlueRadius;
        private System.Windows.Forms.Button buttonInitialObject;
        private System.Windows.Forms.Button buttonGlueRelation;
        private System.Windows.Forms.Button buttonGlue;
        private System.Windows.Forms.Button buttonTilePolygon;
        private System.Windows.Forms.Button buttonSignalObject;
        private System.Windows.Forms.Button buttonEvoRule;
        private System.Windows.Forms.Button buttonProteinOnTile;
        private System.Windows.Forms.Button buttonProtein;
        private System.Windows.Forms.Button buttonFloatingObject;
        private System.Windows.Forms.RichTextBox richTextBoxXML;
        private System.Windows.Forms.ToolStrip toolStripMainPanel;
        private System.Windows.Forms.Button buttonTileVertices;
        private System.Windows.Forms.Button buttonConnector;
        private System.Windows.Forms.Button buttonThresholdPotential;
        private System.Windows.Forms.Button buttonBatteryVoltage;
        private System.Windows.Forms.Button buttonReactionRadius;
        private System.Windows.Forms.Button buttonPushingCoef;
        private System.Windows.Forms.RichTextBox richTextBoxHintsTiling;
        private System.Windows.Forms.RichTextBox richTextBoxHintsMSystem;
        private System.Windows.Forms.ToolStripButton toolStripButtonHideShowHints;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddManualModifications;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem newMSystemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMSystemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMSystemToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonValidateXML;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}