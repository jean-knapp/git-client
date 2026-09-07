using System;
using System.Windows.Forms;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Forms
{
    /// <summary>Single-line prompt used for branch names, tags and stash messages.</summary>
    public partial class TextInputDialog : ModernForm
    {
        public TextInputDialog()
        {
            InitializeComponent();
            Theme.Apply(skin);
        }

        public string Caption
        {
            get => Text;
            set => Text = value;
        }

        public string Prompt
        {
            get => promptLabel.Text;
            set => promptLabel.Text = value;
        }

        public string Value
        {
            get => valueBox.Text.Trim();
            set { valueBox.Text = value ?? string.Empty; UpdateOkButton(); }
        }

        /// <summary>When true the OK button stays enabled for an empty value.</summary>
        public bool AllowEmpty { get; set; }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateOkButton();
            valueBox.Focus();
        }

        private void valueBox_TextChanged(object sender, EventArgs e) => UpdateOkButton();

        private void UpdateOkButton() => okButton.Enabled = AllowEmpty || valueBox.Text.Trim().Length > 0;

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!okButton.Enabled) return;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return true;
            }
            if (keyData == Keys.Enter)
            {
                okButton_Click(okButton, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
