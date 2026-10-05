using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Forms
{
    /// <summary>Confirms running git init in a folder and picks a .gitignore template to start it with.</summary>
    public partial class NewRepositoryDialog : ModernForm
    {
        private readonly List<GitIgnoreTemplate> _templates = GitIgnoreTemplates.All();
        private readonly bool _hasIgnoreFile;

        public NewRepositoryDialog(string path, string message)
        {
            InitializeComponent();
            Theme.Apply(skin);

            if (message != null) messageLabel.Text = message;
            pathLabel.Text = path;
            _hasIgnoreFile = File.Exists(GitIgnoreFile.PathFor(path));

            ignoreCombo.Items.Add("None");
            foreach (var template in _templates) ignoreCombo.Items.Add(template.Name);

            // Start from the template picked last time, when it still exists.
            var last = AppSettings.Current.NewRepositoryIgnoreTemplate;
            int index = _templates.FindIndex(t => string.Equals(t.Name, last, StringComparison.OrdinalIgnoreCase));
            ignoreCombo.SelectedIndex = index + 1;
            UpdateHint();
        }

        /// <summary>The chosen template, or null to leave .gitignore alone.</summary>
        public GitIgnoreTemplate SelectedTemplate =>
            ignoreCombo.SelectedIndex > 0 ? _templates[ignoreCombo.SelectedIndex - 1] : null;

        private void ignoreCombo_SelectedIndexChanged(object sender, SelectedIndexChangedEventArgs e) => UpdateHint();

        private void UpdateHint()
        {
            var template = SelectedTemplate;
            if (template == null)
            {
                ignoreHint.Text = _hasIgnoreFile ? "The folder's existing .gitignore is left as it is" : "No .gitignore is created";
            }
            else
            {
                ignoreHint.Text = _hasIgnoreFile ? template.Description + " - added to the existing .gitignore" : template.Description;
            }
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            AppSettings.Current.NewRepositoryIgnoreTemplate = SelectedTemplate?.Name;
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
                createButton_Click(createButton, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
