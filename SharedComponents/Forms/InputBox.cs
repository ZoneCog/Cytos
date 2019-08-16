using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SharedComponents.Tools;

namespace SharedComponents.Forms
{
    /// <summary>
    /// Empty input form
    /// </summary>
    public partial class InputBox : Form
    {
        #region Private data

        /// <summary>
        /// Width of textbox.
        /// </summary>
        private const int c_Width = 262;

        /// <summary>
        /// Heigh of textbox.
        /// </summary>
        private const int c_Heigh = 20;

        /// <summary>
        /// X position of textboxes.
        /// </summary>
        private const int c_StartPositionX = 12;

        /// <summary>
        /// List of all textboxes used for getting its values.
        /// </summary>
        private readonly List<TextBox> v_Textboxes = new List<TextBox>();

        private readonly List<Regexp.Check> v_Types;

        #endregion

        #region Public data

        /// <summary>
        /// Output values added to textfields.
        /// </summary>
        public List<string> OutputTexts { get; } = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="windowtText">Text shown in window header</param>
        /// <param name="buttonText">Text of confirmation button</param>
        /// <param name="contentTexts">List of texts shown in textbox as description.</param>
        public InputBox(string windowtText, string buttonText, List<string> contentTexts)
        {
            InitializeComponent();
            Text = windowtText;
            buttonConfirm.Text = buttonText;

            Height = Height + 26 * contentTexts.Count;

            for (var index = 0; index < contentTexts.Count; index++)
            {
                string contentText = contentTexts[index];
                CreateInputItem(contentText, index + 1);
            }

            MinimumSize = new Size(Width, Height);
            MaximumSize = new Size(Width, Height);
        }

        /// <summary>
        /// Constructor with checks.
        /// </summary>
        /// <param name="windowtText">Text shown in window header</param>
        /// <param name="buttonText">Text of confirmation button</param>
        /// <param name="contentTexts">List of texts shown in textbox as description.</param>
        /// <param name="types">Types for checks.</param>
        public InputBox(string windowtText, string buttonText, List<string> contentTexts, List<Regexp.Check> types) : this(windowtText, buttonText, contentTexts)
        {
            if (types.Count != contentTexts.Count)
            {
                throw new InvalidOperationException("Number of types must be the same as number of textboxes!");
            }
            v_Types = types;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// On click event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Arguments.</param>
        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < v_Textboxes.Count; i++)
            {
                //Only if we have created inputbox with check types constructor.
                if (v_Types != null)
                {
                    if (!InputChecks.CheckValue(v_Textboxes[i].Text, v_Types[i]))
                    {
                        OutputTexts.Clear();
                        return;
                    }
                }
                OutputTexts.Add(v_Textboxes[i].Text);
            }
            Close();
        }

        /// <summary>
        /// Creates new textbox on specific position computed based on index
        /// </summary>
        /// <param name="contentText">Text shown as description in textbox</param>
        /// <param name="index">Index of new textbox</param>
        private void CreateInputItem(string contentText, int index)
        {
            int startPositionY = 6 * index + 20 * (index - 1);
            TextBox textBox = new TextBox
            {
                Width = c_Width,
                Height = c_Heigh,
                Location = new Point(c_StartPositionX, startPositionY),
                Text = contentText
            };
            textBox.Click += ClearTextBox;
            v_Textboxes.Add(textBox);
            Controls.Add(textBox);
        }

        /// <summary>
        /// On click event for textbox - default behaviour is to clear description text.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Arguments.</param>
        private void ClearTextBox(object sender, EventArgs e)
        {
            ((TextBox)sender).Clear();
        }

        #endregion

        #region Public Methods

        public sealed override Size MaximumSize
        {
            get { return base.MaximumSize; }
            set { base.MaximumSize = value; }
        }

        public sealed override Size MinimumSize
        {
            get { return base.MinimumSize; }
            set { base.MinimumSize = value; }
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        #endregion

    }
}
