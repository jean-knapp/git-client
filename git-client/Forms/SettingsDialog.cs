using System;
using System.IO;
using System.Windows.Forms;
using GitClient.Controls;
using GitClient.Git;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Forms
{
    public partial class SettingsDialog : ModernForm
    {
        private static readonly string[] PageTitles = { "Identity", "Tools & paths", "History", "Appearance" };

        private string _originalName;
        private string _originalEmail;
        private ThemeMode _originalTheme;

        public SettingsDialog()
        {
            InitializeComponent();
            Theme.Apply(skin);
            gitPathBox.OverrideSkinFont = true;
            gitPathBox.Font = Fonts.Code(13f);
            claudePathBox.OverrideSkinFont = true;
            claudePathBox.Font = Fonts.Code(13f);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var settings = AppSettings.Current;
            _originalTheme = Theme.Mode;

            gitPathBox.Text = settings.GitExecutable ?? string.Empty;
            claudePathBox.Text = settings.ClaudeExecutable ?? string.Empty;
            modelBox.Text = settings.ClaudeModel ?? string.Empty;
            maxCommitsBox.Value = Math.Max(100, Math.Min(100000, settings.MaxCommits));
            rowHeightBox.Value = Math.Max(30, Math.Min(48, settings.HistoryRowHeight));
            relativeDatesSwitch.Checked = settings.RelativeDates;
            themeToggle.SelectedIndex = Theme.Mode == ThemeMode.Light ? 1 : 0;

            // The segmented control sizes to its captions, so it never clips a label.
            themeToggle.Width = themeToggle.PreferredWidth;
            themeToggle.Left = themeRow.ClientSize.Width - 16 - themeToggle.Width;

            navRail.SetSelection(0, false);
            ShowPage(0);

            try
            {
                _originalName = await GitRepository.GetGlobalConfigAsync("user.name") ?? string.Empty;
                _originalEmail = await GitRepository.GetGlobalConfigAsync("user.email") ?? string.Empty;
                userNameBox.Text = _originalName;
                emailBox.Text = _originalEmail;
            }
            catch
            {
                // Leave the identity fields empty if git cannot be queried.
            }
        }

        private void navRail_SelectionChanged(object sender, EventArgs e) => ShowPage(navRail.SelectedIndex);

        private void ShowPage(int index)
        {
            if (index < 0 || index >= PageTitles.Length) return;
            pageTitleLabel.Text = PageTitles[index];
            identityPage.Visible = index == 0;
            toolsPage.Visible = index == 1;
            historyPage.Visible = index == 2;
            appearancePage.Visible = index == 3;
        }

        /// <summary>The theme previews live, so the choice can be judged before saving.</summary>
        private void themeToggle_SelectedIndexChanged(object sender, EventArgs e)
        {
            Theme.Mode = themeToggle.SelectedIndex == 1 ? ThemeMode.Light : ThemeMode.Dark;
            Theme.Apply(skin);
            Invalidate(true);
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {
            var settings = AppSettings.Current;
            var gitPath = gitPathBox.Text.Trim();
            if (gitPath.Length > 0 && !File.Exists(gitPath))
            {
                Dialogs.Warning(this, "Settings", "git.exe was not found at:\n\n" + gitPath);
                return;
            }
            var claudePath = claudePathBox.Text.Trim();
            if (claudePath.Length > 0 && !File.Exists(claudePath))
            {
                Dialogs.Warning(this, "Settings", "The Claude Code CLI was not found at:\n\n" + claudePath);
                return;
            }

            settings.GitExecutable = gitPath.Length > 0 ? gitPath : null;
            settings.ClaudeExecutable = claudePath.Length > 0 ? claudePath : null;
            settings.ClaudeModel = modelBox.Text.Trim().Length > 0 ? modelBox.Text.Trim() : null;
            settings.MaxCommits = (int)maxCommitsBox.Value;
            settings.HistoryRowHeight = (int)rowHeightBox.Value;
            settings.RelativeDates = relativeDatesSwitch.Checked;
            settings.Theme = Theme.Mode;
            settings.Save();
            GitRunner.GitExecutable = settings.GitExecutable ?? GitRunner.FindGit();

            try
            {
                if (userNameBox.Text.Trim() != _originalName) await GitRepository.SetGlobalConfigAsync("user.name", userNameBox.Text.Trim());
                if (emailBox.Text.Trim() != _originalEmail) await GitRepository.SetGlobalConfigAsync("user.email", emailBox.Text.Trim());
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, "Settings", "Could not write the git identity:\n\n" + ex.Message);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Undo the live theme preview when the dialog is dismissed.
            Theme.Mode = _originalTheme;
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
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
