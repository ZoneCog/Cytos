namespace Cytos_v2.Forms
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.toolStripMainPanel = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.newExampleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadExampleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveSnapshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSnapshotAndVizualizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.openLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSimulationLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.closeProgramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownRun = new System.Windows.Forms.ToolStripDropDownButton();
            this.runSimulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.run1StepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runSpecifiedNumberOfStepsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.restartSimulationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.multipleRunsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multipleRunsprobabilisticKillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.multipleRunsV2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multipleRunsprobabilisticKillV2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.oneOffDamageTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonAbout = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxSearchInMSystemObjects = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.mSystemDescriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.richTextBoxMSystem = new System.Windows.Forms.RichTextBox();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonRun = new System.Windows.Forms.Button();
            this.buttonRestart = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonVizualizeSnapshot = new System.Windows.Forms.Button();
            this.buttonOpenSimLog = new System.Windows.Forms.Button();
            this.buttonLoadMSystem = new System.Windows.Forms.Button();
            this.toolStripMainPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMainPanel
            // 
            this.toolStripMainPanel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripDropDownRun,
            this.toolStripButtonAbout,
            this.toolStripLabel2,
            this.toolStripTextBoxSearchInMSystemObjects,
            this.toolStripLabel1});
            this.toolStripMainPanel.Location = new System.Drawing.Point(0, 0);
            this.toolStripMainPanel.Name = "toolStripMainPanel";
            this.toolStripMainPanel.Size = new System.Drawing.Size(1003, 25);
            this.toolStripMainPanel.TabIndex = 0;
            this.toolStripMainPanel.Text = "toolStrip";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newExampleToolStripMenuItem,
            this.loadExampleToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveSnapshotToolStripMenuItem,
            this.saveSnapshotAndVizualizeToolStripMenuItem,
            this.toolStripSeparator2,
            this.openLogFileToolStripMenuItem,
            this.openSimulationLogFileToolStripMenuItem,
            this.toolStripSeparator3,
            this.closeProgramToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton1.Text = "File";
            // 
            // newExampleToolStripMenuItem
            // 
            this.newExampleToolStripMenuItem.Name = "newExampleToolStripMenuItem";
            this.newExampleToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.newExampleToolStripMenuItem.Text = "New M System";
            this.newExampleToolStripMenuItem.Click += new System.EventHandler(this.newExampleToolStripMenuItem_Click);
            // 
            // loadExampleToolStripMenuItem
            // 
            this.loadExampleToolStripMenuItem.Name = "loadExampleToolStripMenuItem";
            this.loadExampleToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.loadExampleToolStripMenuItem.Text = "Load M System (ctrl+l)";
            this.loadExampleToolStripMenuItem.Click += new System.EventHandler(this.mSystemDescriptionToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(210, 6);
            // 
            // saveSnapshotToolStripMenuItem
            // 
            this.saveSnapshotToolStripMenuItem.Name = "saveSnapshotToolStripMenuItem";
            this.saveSnapshotToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.saveSnapshotToolStripMenuItem.Text = "Save snapshot";
            this.saveSnapshotToolStripMenuItem.Click += new System.EventHandler(this.saveSnapshotToolStripMenuItem_Click);
            // 
            // saveSnapshotAndVizualizeToolStripMenuItem
            // 
            this.saveSnapshotAndVizualizeToolStripMenuItem.Name = "saveSnapshotAndVizualizeToolStripMenuItem";
            this.saveSnapshotAndVizualizeToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.saveSnapshotAndVizualizeToolStripMenuItem.Text = "Visualize snapshot (ctrl+u)";
            this.saveSnapshotAndVizualizeToolStripMenuItem.Click += new System.EventHandler(this.saveSnapshotAndVizualizeToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(210, 6);
            // 
            // openLogFileToolStripMenuItem
            // 
            this.openLogFileToolStripMenuItem.Name = "openLogFileToolStripMenuItem";
            this.openLogFileToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.openLogFileToolStripMenuItem.Text = "Open log file";
            this.openLogFileToolStripMenuItem.Click += new System.EventHandler(this.openLogFileToolStripMenuItem_Click);
            // 
            // openSimulationLogFileToolStripMenuItem
            // 
            this.openSimulationLogFileToolStripMenuItem.Name = "openSimulationLogFileToolStripMenuItem";
            this.openSimulationLogFileToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.openSimulationLogFileToolStripMenuItem.Text = "Open simulation log file";
            this.openSimulationLogFileToolStripMenuItem.Click += new System.EventHandler(this.openSimulationLogFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(210, 6);
            // 
            // closeProgramToolStripMenuItem
            // 
            this.closeProgramToolStripMenuItem.Name = "closeProgramToolStripMenuItem";
            this.closeProgramToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.closeProgramToolStripMenuItem.Text = "Close program";
            this.closeProgramToolStripMenuItem.Click += new System.EventHandler(this.closeProgramToolStripMenuItem_Click);
            // 
            // toolStripDropDownRun
            // 
            this.toolStripDropDownRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownRun.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runSimulationToolStripMenuItem,
            this.run1StepToolStripMenuItem,
            this.runSpecifiedNumberOfStepsToolStripMenuItem,
            this.toolStripSeparator4,
            this.restartSimulationToolStripMenuItem,
            this.toolStripSeparator5,
            this.multipleRunsToolStripMenuItem,
            this.multipleRunsprobabilisticKillToolStripMenuItem,
            this.toolStripSeparator6,
            this.multipleRunsV2ToolStripMenuItem,
            this.multipleRunsprobabilisticKillV2ToolStripMenuItem,
            this.toolStripSeparator7,
            this.oneOffDamageTestToolStripMenuItem});
            this.toolStripDropDownRun.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownRun.Image")));
            this.toolStripDropDownRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownRun.Name = "toolStripDropDownRun";
            this.toolStripDropDownRun.Size = new System.Drawing.Size(41, 22);
            this.toolStripDropDownRun.Text = "Run";
            // 
            // runSimulationToolStripMenuItem
            // 
            this.runSimulationToolStripMenuItem.Enabled = false;
            this.runSimulationToolStripMenuItem.Name = "runSimulationToolStripMenuItem";
            this.runSimulationToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.runSimulationToolStripMenuItem.Text = "Run simulation (ctr+r)";
            this.runSimulationToolStripMenuItem.Click += new System.EventHandler(this.runSimulationToolStripMenuItem_Click);
            // 
            // run1StepToolStripMenuItem
            // 
            this.run1StepToolStripMenuItem.Enabled = false;
            this.run1StepToolStripMenuItem.Name = "run1StepToolStripMenuItem";
            this.run1StepToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.run1StepToolStripMenuItem.Text = "Run 1 step";
            this.run1StepToolStripMenuItem.Click += new System.EventHandler(this.run1StepToolStripMenuItem_Click);
            // 
            // runSpecifiedNumberOfStepsToolStripMenuItem
            // 
            this.runSpecifiedNumberOfStepsToolStripMenuItem.Enabled = false;
            this.runSpecifiedNumberOfStepsToolStripMenuItem.Name = "runSpecifiedNumberOfStepsToolStripMenuItem";
            this.runSpecifiedNumberOfStepsToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.runSpecifiedNumberOfStepsToolStripMenuItem.Text = "Run specified number of steps (ctrl+n)";
            this.runSpecifiedNumberOfStepsToolStripMenuItem.Click += new System.EventHandler(this.runSpecifiedNumberOfStepsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(274, 6);
            // 
            // restartSimulationToolStripMenuItem
            // 
            this.restartSimulationToolStripMenuItem.Enabled = false;
            this.restartSimulationToolStripMenuItem.Name = "restartSimulationToolStripMenuItem";
            this.restartSimulationToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.restartSimulationToolStripMenuItem.Text = "Restart simulation";
            this.restartSimulationToolStripMenuItem.Click += new System.EventHandler(this.restartSimulationToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(274, 6);
            // 
            // multipleRunsToolStripMenuItem
            // 
            this.multipleRunsToolStripMenuItem.Enabled = false;
            this.multipleRunsToolStripMenuItem.Name = "multipleRunsToolStripMenuItem";
            this.multipleRunsToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.multipleRunsToolStripMenuItem.Text = "Multiple runs (fixed kill)";
            this.multipleRunsToolStripMenuItem.Click += new System.EventHandler(this.multipleRunsToolStripMenuItem_Click);
            // 
            // multipleRunsprobabilisticKillToolStripMenuItem
            // 
            this.multipleRunsprobabilisticKillToolStripMenuItem.Enabled = false;
            this.multipleRunsprobabilisticKillToolStripMenuItem.Name = "multipleRunsprobabilisticKillToolStripMenuItem";
            this.multipleRunsprobabilisticKillToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.multipleRunsprobabilisticKillToolStripMenuItem.Text = "Multiple runs (probabilistic kill)";
            this.multipleRunsprobabilisticKillToolStripMenuItem.Click += new System.EventHandler(this.multipleRunsprobabilisticKillToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(274, 6);
            // 
            // multipleRunsV2ToolStripMenuItem
            // 
            this.multipleRunsV2ToolStripMenuItem.Enabled = false;
            this.multipleRunsV2ToolStripMenuItem.Name = "multipleRunsV2ToolStripMenuItem";
            this.multipleRunsV2ToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.multipleRunsV2ToolStripMenuItem.Text = "Multiple runs (fixed kills) V2";
            this.multipleRunsV2ToolStripMenuItem.Click += new System.EventHandler(this.multipleRunsV2TestToolStripMenuItem_Click);
            // 
            // multipleRunsprobabilisticKillV2ToolStripMenuItem
            // 
            this.multipleRunsprobabilisticKillV2ToolStripMenuItem.Enabled = false;
            this.multipleRunsprobabilisticKillV2ToolStripMenuItem.Name = "multipleRunsprobabilisticKillV2ToolStripMenuItem";
            this.multipleRunsprobabilisticKillV2ToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.multipleRunsprobabilisticKillV2ToolStripMenuItem.Text = "Multiple runs (probabilistic kill) V2";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(274, 6);
            // 
            // oneOffDamageTestToolStripMenuItem
            // 
            this.oneOffDamageTestToolStripMenuItem.Enabled = false;
            this.oneOffDamageTestToolStripMenuItem.Name = "oneOffDamageTestToolStripMenuItem";
            this.oneOffDamageTestToolStripMenuItem.Size = new System.Drawing.Size(277, 22);
            this.oneOffDamageTestToolStripMenuItem.Text = "One off damage Test";
            this.oneOffDamageTestToolStripMenuItem.Click += new System.EventHandler(this.oneOffDamageTestToolStripMenuItem_Click);
            // 
            // toolStripButtonAbout
            // 
            this.toolStripButtonAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonAbout.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAbout.Image")));
            this.toolStripButtonAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAbout.Name = "toolStripButtonAbout";
            this.toolStripButtonAbout.Size = new System.Drawing.Size(44, 22);
            this.toolStripButtonAbout.Text = "About";
            this.toolStripButtonAbout.Click += new System.EventHandler(this.toolStripButtonAbout_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel2.Font = new System.Drawing.Font("Segoe UI", 4F);
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(7, 22);
            this.toolStripLabel2.Text = " ";
            // 
            // toolStripTextBoxSearchInMSystemObjects
            // 
            this.toolStripTextBoxSearchInMSystemObjects.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripTextBoxSearchInMSystemObjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolStripTextBoxSearchInMSystemObjects.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBoxSearchInMSystemObjects.Name = "toolStripTextBoxSearchInMSystemObjects";
            this.toolStripTextBoxSearchInMSystemObjects.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripTextBoxSearchInMSystemObjects.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBoxSearchInMSystemObjects.TextChanged += new System.EventHandler(this.toolStripTextBoxSearchInMSystemObjects_TextChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripLabel1.Size = new System.Drawing.Size(154, 22);
            this.toolStripLabel1.Text = "Search in M System objects:";
            // 
            // mSystemDescriptionToolStripMenuItem
            // 
            this.mSystemDescriptionToolStripMenuItem.Name = "mSystemDescriptionToolStripMenuItem";
            this.mSystemDescriptionToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.mSystemDescriptionToolStripMenuItem.Text = "M System description";
            this.mSystemDescriptionToolStripMenuItem.Click += new System.EventHandler(this.mSystemDescriptionToolStripMenuItem_Click);
            // 
            // richTextBoxOutput
            // 
            this.richTextBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxOutput.Location = new System.Drawing.Point(18, 28);
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            this.richTextBoxOutput.Size = new System.Drawing.Size(338, 611);
            this.richTextBoxOutput.TabIndex = 1;
            this.richTextBoxOutput.Text = "";
            // 
            // richTextBoxMSystem
            // 
            this.richTextBoxMSystem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxMSystem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.richTextBoxMSystem.Location = new System.Drawing.Point(362, 28);
            this.richTextBoxMSystem.Name = "richTextBoxMSystem";
            this.richTextBoxMSystem.Size = new System.Drawing.Size(635, 715);
            this.richTextBoxMSystem.TabIndex = 2;
            this.richTextBoxMSystem.Text = "Load M System description file to see content.";
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(115, 16);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(103, 23);
            this.buttonStop.TabIndex = 8;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonRun
            // 
            this.buttonRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRun.Enabled = false;
            this.buttonRun.Location = new System.Drawing.Point(6, 16);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(103, 23);
            this.buttonRun.TabIndex = 7;
            this.buttonRun.Text = "Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // buttonRestart
            // 
            this.buttonRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRestart.Enabled = false;
            this.buttonRestart.Location = new System.Drawing.Point(224, 16);
            this.buttonRestart.Name = "buttonRestart";
            this.buttonRestart.Size = new System.Drawing.Size(103, 23);
            this.buttonRestart.TabIndex = 9;
            this.buttonRestart.Text = "Restart";
            this.buttonRestart.UseVisualStyleBackColor = true;
            this.buttonRestart.Click += new System.EventHandler(this.buttonRestart_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.buttonStop);
            this.groupBox1.Controls.Add(this.buttonRestart);
            this.groupBox1.Controls.Add(this.buttonRun);
            this.groupBox1.Location = new System.Drawing.Point(18, 697);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(338, 46);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Simulation loop controls";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.buttonVizualizeSnapshot);
            this.groupBox2.Controls.Add(this.buttonOpenSimLog);
            this.groupBox2.Controls.Add(this.buttonLoadMSystem);
            this.groupBox2.Location = new System.Drawing.Point(18, 645);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(338, 46);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Favourite controls";
            // 
            // buttonVizualizeSnapshot
            // 
            this.buttonVizualizeSnapshot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonVizualizeSnapshot.Location = new System.Drawing.Point(115, 16);
            this.buttonVizualizeSnapshot.Name = "buttonVizualizeSnapshot";
            this.buttonVizualizeSnapshot.Size = new System.Drawing.Size(103, 23);
            this.buttonVizualizeSnapshot.TabIndex = 8;
            this.buttonVizualizeSnapshot.Text = "Vizualize snapshot";
            this.buttonVizualizeSnapshot.UseVisualStyleBackColor = true;
            this.buttonVizualizeSnapshot.Click += new System.EventHandler(this.buttonVizualizeSnapshot_Click);
            // 
            // buttonOpenSimLog
            // 
            this.buttonOpenSimLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOpenSimLog.Location = new System.Drawing.Point(224, 16);
            this.buttonOpenSimLog.Name = "buttonOpenSimLog";
            this.buttonOpenSimLog.Size = new System.Drawing.Size(103, 23);
            this.buttonOpenSimLog.TabIndex = 9;
            this.buttonOpenSimLog.Text = "Open Sim log";
            this.buttonOpenSimLog.UseVisualStyleBackColor = true;
            this.buttonOpenSimLog.Click += new System.EventHandler(this.buttonOpenSimLog_Click);
            // 
            // buttonLoadMSystem
            // 
            this.buttonLoadMSystem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonLoadMSystem.Location = new System.Drawing.Point(6, 16);
            this.buttonLoadMSystem.Name = "buttonLoadMSystem";
            this.buttonLoadMSystem.Size = new System.Drawing.Size(103, 23);
            this.buttonLoadMSystem.TabIndex = 7;
            this.buttonLoadMSystem.Text = "Load M System";
            this.buttonLoadMSystem.UseVisualStyleBackColor = true;
            this.buttonLoadMSystem.Click += new System.EventHandler(this.buttonLoadMSystem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 754);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.richTextBoxMSystem);
            this.Controls.Add(this.richTextBoxOutput);
            this.Controls.Add(this.toolStripMainPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(939, 581);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cytos v2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_KeyDown);
            this.toolStripMainPanel.ResumeLayout(false);
            this.toolStripMainPanel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMainPanel;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem newExampleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeProgramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSystemDescriptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadExampleToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonAbout;
        private System.Windows.Forms.RichTextBox richTextBoxOutput;
        private System.Windows.Forms.RichTextBox richTextBoxMSystem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownRun;
        private System.Windows.Forms.ToolStripMenuItem runSimulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem run1StepToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runSpecifiedNumberOfStepsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multipleRunsToolStripMenuItem;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Button buttonRestart;
        private System.Windows.Forms.ToolStripMenuItem multipleRunsprobabilisticKillToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveSnapshotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSnapshotAndVizualizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem openSimulationLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxSearchInMSystemObjects;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripMenuItem restartSimulationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem multipleRunsV2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multipleRunsprobabilisticKillV2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem oneOffDamageTestToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonVizualizeSnapshot;
        private System.Windows.Forms.Button buttonOpenSimLog;
        private System.Windows.Forms.Button buttonLoadMSystem;
    }
}