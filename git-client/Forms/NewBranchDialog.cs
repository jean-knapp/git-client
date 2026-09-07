using System;
using System.Windows.Forms;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Forms
{
    public partial class NewBranchDialog : ModernForm
    {
        public NewBranchDialog()
        {
            InitializeComponent();
            Theme.Apply(skin);
        }

        public string BranchName => nameBox.Text.Trim();

        public bool CheckoutAfterCreate => checkoutSwitch.Checked;

        public string StartPointText
        {
            get => startPointValueLabel.Text;
            set => startPointValueLabel.Text = value ?? "HEAD";
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            nameBox.Focus();
        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            createButton.Enabled = IsValidBranchName(nameBox.Text.Trim());
        }

        /// <summary>Rejects the names git itself would refuse, so the error shows up before the command runs.</summary>
        public static bool IsValidBranchName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            if (name.StartsWith("-") || name.StartsWith("/") || name.EndsWith("/") || name.EndsWith(".")) return false;
            if (name.Contains("..") || name.Contains("//") || name.Contains("@{")) return false;
            foreach (var c in name)
            {
                if (char.IsWhiteSpace(c) || c == '~' || c == '^' || c == ':' || c == '?' || c == '*' || c == '[' || c == '\\' || c < 32 || c == 127) return false;
            }
            return true;
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            if (!createButton.Enabled) return;
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
            if (keyData == Keys.Enter && createButton.Enabled)
            {
                DialogResult = DialogResult.OK;
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
