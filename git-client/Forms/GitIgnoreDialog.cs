using System;
using System.IO;
using System.Windows.Forms;
using GitClient.Controls;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Forms
{
    /// <summary>
    /// Editor for a repository's root .gitignore, with the built-in rule templates and the user's
    /// own saved ones.
    /// </summary>
    public partial class GitIgnoreDialog : ModernForm
    {
        private readonly string _workingDirectory;
        private string _originalText = string.Empty;

        public GitIgnoreDialog(string workingDirectory)
        {
            InitializeComponent();
            Theme.Apply(skin);
            _workingDirectory = workingDirectory;

            var p = Theme.Palette;
            var fill = p.FillOn(p.Background);
            editorBox.UseParentSkin = false;
            editorBox.CornerStyle = ModernWinForms.Enums.CornerStyle.Square;
            editorBox.Colors.BorderColor = System.Drawing.Color.Transparent;
            editorBox.Colors.Normal.BackColor = fill;
            editorBox.Colors.Normal.ForeColor = p.Foreground;
            editorBox.Colors.Hover.BackColor = fill;
            editorBox.Colors.Hover.BorderColor = System.Drawing.Color.Transparent;
            editorBox.Colors.Active.BackColor = fill;
            editorBox.Colors.Active.BorderColor = System.Drawing.Color.Transparent;
            editorBox.ScrollBarColors.ThumbColor = p.Fill2On(fill);
            editorBox.ScrollBarColors.ThumbHoverColor = p.Foreground3;
            editorBox.OverrideSkinFont = true;
            editorBox.Font = Fonts.Code(13f);

            templateButton.DropDownMenu = templateMenu;
            pathLabel.Text = GitIgnoreFile.PathFor(_workingDirectory);
            LoadFile(GitIgnoreFile.Read(_workingDirectory));
        }

        /// <summary>True when the file was written, so the caller can refresh its status.</summary>
        public bool Saved { get; private set; }

        private void LoadFile(string text)
        {
            _originalText = text ?? string.Empty;
            editorBox.Text = _originalText.Replace("\n", Environment.NewLine).Replace("\r\r\n", "\r\n");
            UpdateStatus();
        }

        private string CurrentText => editorBox.Text ?? string.Empty;

        private bool IsDirty =>
            Normalize(CurrentText) != Normalize(_originalText);

        private static string Normalize(string text) =>
            (text ?? string.Empty).Replace("\r\n", "\n").Replace("\r", "\n");

        private void UpdateStatus()
        {
            bool exists = File.Exists(GitIgnoreFile.PathFor(_workingDirectory));
            statusLabel.Text = IsDirty
                ? "Unsaved changes"
                : (exists ? "No changes" : "This repository has no .gitignore yet — saving creates one.");
            saveButton.Enabled = IsDirty;
            saveButton.Invalidate();
        }

        private void editorBox_TextChanged(object sender, EventArgs e) => UpdateStatus();

        private void templateButton_Click(object sender, EventArgs e)
        {
            // Rebuilt on every drop, so templates saved during this session show up straight away.
            templateMenu.Items.Clear();
            bool separator = false;
            foreach (var template in GitIgnoreTemplates.All())
            {
                var captured = template;
                var item = templateMenu.Items.Add(template.Name);
                if (template.IsCustom && !separator)
                {
                    item.BeginGroup = true;
                    separator = true;
                }
                item.Click += (s, a) => InsertTemplate(captured);
            }
            if (templateMenu.Items.Count == 0)
            {
                var empty = templateMenu.Items.Add("No templates");
                empty.Enabled = false;
            }
        }

        private void InsertTemplate(GitIgnoreTemplate template)
        {
            var addition = (template.Content ?? string.Empty).Replace("\r\n", "\n").Replace("\n", Environment.NewLine).TrimEnd();
            if (addition.Length == 0) return;

            var text = CurrentText;
            if (text.Trim().Length == 0)
            {
                editorBox.Text = addition + Environment.NewLine;
            }
            else
            {
                if (!text.EndsWith(Environment.NewLine, StringComparison.Ordinal)) editorBox.AppendText(Environment.NewLine);
                editorBox.AppendText(Environment.NewLine + addition + Environment.NewLine);
            }

            editorBox.Focus();
        }

        private void saveTemplateButton_Click(object sender, EventArgs e)
        {
            if (CurrentText.Trim().Length == 0)
            {
                Dialogs.Information(this, "Save as template", "There is nothing to save yet.");
                return;
            }

            using (var prompt = new TextInputDialog())
            {
                prompt.Caption = "Save as template";
                prompt.Prompt = "Template name";
                prompt.Value = "My template";
                if (prompt.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    var saved = GitIgnoreTemplates.Save(prompt.Value, Normalize(CurrentText));
                    statusLabel.Text = "Saved template \"" + saved.Name + "\" in " + GitIgnoreTemplates.CustomDirectory;
                }
                catch (Exception ex)
                {
                    Dialogs.Error(this, "Save as template", ex.Message);
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!IsDirty)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }
            try
            {
                GitIgnoreFile.Write(_workingDirectory, CurrentText);
                Saved = true;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, "Edit .gitignore", "Could not write the file.\n\n" + ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (IsDirty && !Dialogs.Confirm(this, "Discard changes", "Close without saving your .gitignore changes?", "Discard")) return;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                cancelButton_Click(cancelButton, EventArgs.Empty);
                return true;
            }
            if (keyData == (Keys.Control | Keys.S))
            {
                saveButton_Click(saveButton, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
