namespace Cytos_v2.Forms
{
    partial class OpenLogFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenLogFile));
            this.richTextBoxLogContent = new System.Windows.Forms.RichTextBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonClearLogFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBoxLogContent
            // 
            this.richTextBoxLogContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxLogContent.Location = new System.Drawing.Point(12, 12);
            this.richTextBoxLogContent.Name = "richTextBoxLogContent";
            this.richTextBoxLogContent.Size = new System.Drawing.Size(560, 708);
            this.richTextBoxLogContent.TabIndex = 0;
            this.richTextBoxLogContent.Text = "";
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRefresh.Location = new System.Drawing.Point(497, 726);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonRefresh.TabIndex = 1;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonClearLogFile
            // 
            this.buttonClearLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearLogFile.Location = new System.Drawing.Point(416, 727);
            this.buttonClearLogFile.Name = "buttonClearLogFile";
            this.buttonClearLogFile.Size = new System.Drawing.Size(75, 23);
            this.buttonClearLogFile.TabIndex = 2;
            this.buttonClearLogFile.Text = "Clear log file";
            this.buttonClearLogFile.UseVisualStyleBackColor = true;
            this.buttonClearLogFile.Click += new System.EventHandler(this.buttonClearLogFile_Click);
            // 
            // OpenLogFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 762);
            this.Controls.Add(this.buttonClearLogFile);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.richTextBoxLogContent);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(600, 800);
            this.Name = "OpenLogFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Log file";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OpenLogFile_FormClosing);
            this.Load += new System.EventHandler(this.OpenLogFile_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxLogContent;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonClearLogFile;
    }
}