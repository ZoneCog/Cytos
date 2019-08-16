namespace SharedComponents.Forms
{
    partial class ExceptionNotifier
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionNotifier));
            this.labelExceptionDescription = new System.Windows.Forms.Label();
            this.richTextBoxExceptionMessage = new System.Windows.Forms.RichTextBox();
            this.richTextBoxExceptionStackTrace = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // labelExceptionDescription
            // 
            this.labelExceptionDescription.AutoSize = true;
            this.labelExceptionDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelExceptionDescription.Location = new System.Drawing.Point(12, 9);
            this.labelExceptionDescription.Name = "labelExceptionDescription";
            this.labelExceptionDescription.Size = new System.Drawing.Size(342, 17);
            this.labelExceptionDescription.TabIndex = 0;
            this.labelExceptionDescription.Text = "During runtime exception occured. See details below:";
            // 
            // richTextBoxExceptionMessage
            // 
            this.richTextBoxExceptionMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxExceptionMessage.Location = new System.Drawing.Point(15, 29);
            this.richTextBoxExceptionMessage.Name = "richTextBoxExceptionMessage";
            this.richTextBoxExceptionMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxExceptionMessage.Size = new System.Drawing.Size(339, 98);
            this.richTextBoxExceptionMessage.TabIndex = 1;
            this.richTextBoxExceptionMessage.Text = "";
            // 
            // richTextBoxExceptionStackTrace
            // 
            this.richTextBoxExceptionStackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxExceptionStackTrace.Location = new System.Drawing.Point(15, 133);
            this.richTextBoxExceptionStackTrace.Name = "richTextBoxExceptionStackTrace";
            this.richTextBoxExceptionStackTrace.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxExceptionStackTrace.Size = new System.Drawing.Size(339, 214);
            this.richTextBoxExceptionStackTrace.TabIndex = 2;
            this.richTextBoxExceptionStackTrace.Text = "";
            // 
            // ExceptionNotifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 359);
            this.Controls.Add(this.richTextBoxExceptionStackTrace);
            this.Controls.Add(this.richTextBoxExceptionMessage);
            this.Controls.Add(this.labelExceptionDescription);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(382, 398);
            this.Name = "ExceptionNotifier";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExceptionNotifier";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelExceptionDescription;
        private System.Windows.Forms.RichTextBox richTextBoxExceptionMessage;
        private System.Windows.Forms.RichTextBox richTextBoxExceptionStackTrace;
    }
}