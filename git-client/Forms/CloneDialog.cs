using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GitClient.Controls;
using GitClient.Git;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Forms
{
    public partial class CloneDialog : ModernForm
    {
        private CancellationTokenSource _cancellation;
        private bool _cloning;

        public CloneDialog()
        {
            InitializeComponent();
            Theme.Apply(skin);
            folderBox.OverrideSkinFont = true;
            folderBox.Font = Fonts.Code(13f);
            folderBox.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos");
        }

        /// <summary>Path of the cloned repository once the dialog returns OK.</summary>
        public string ClonedPath { get; private set; }

        private void urlBox_TextChanged(object sender, EventArgs e) => UpdateTarget();

        private void folderBox_TextChanged(object sender, EventArgs e) => UpdateTarget();

        private void UpdateTarget()
        {
            var target = BuildTargetPath();
            targetLabel.Text = target == null ? string.Empty : "→ " + target;
            cloneButton.Enabled = target != null && !_cloning;
        }

        private string BuildTargetPath()
        {
            var url = urlBox.Text.Trim();
            var folder = folderBox.Text.Trim();
            if (url.Length == 0 || folder.Length == 0) return null;
            var name = RepositoryNameFromUrl(url);
            if (name == null) return null;
            try { return Path.Combine(folder, name); }
            catch { return null; }
        }

        public static string RepositoryNameFromUrl(string url)
        {
            var text = url.Trim().TrimEnd('/');
            if (text.Length == 0) return null;
            if (text.EndsWith(".git", StringComparison.OrdinalIgnoreCase)) text = text.Substring(0, text.Length - 4);
            int slash = text.LastIndexOfAny(new[] { '/', ':', '\\' });
            var name = slash >= 0 ? text.Substring(slash + 1) : text;
            name = name.Trim();
            if (name.Length == 0) return null;
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                if (name.IndexOf(c) >= 0) return null;
            }
            return name;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            var path = FolderPicker.Show(this, "Choose where to clone the repository", folderBox.Text.Trim());
            if (path != null) folderBox.Text = path;
        }

        private async void cloneButton_Click(object sender, EventArgs e)
        {
            var url = urlBox.Text.Trim();
            var target = BuildTargetPath();
            if (target == null) return;
            if (Directory.Exists(target) && Directory.GetFileSystemEntries(target).Length > 0)
            {
                Dialogs.Warning(this, "Clone", "The folder already exists and is not empty:\n\n" + target);
                return;
            }

            _cloning = true;
            _cancellation = new CancellationTokenSource();
            cloneButton.Enabled = false;
            urlBox.Enabled = false;
            folderBox.Enabled = false;
            browseButton.Enabled = false;
            progressBar.Visible = true;
            progressLabel.Text = "Cloning...";

            try
            {
                var progress = new Progress<string>(line => progressLabel.Text = line);
                var result = await GitRepository.CloneAsync(url, target, submodulesCheck.Checked, progress, _cancellation.Token);
                if (result.Succeeded)
                {
                    ClonedPath = target;
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
                if (result.NeedsCredentials)
                {
                    Dialogs.Error(this, "Clone",
                        "Git could not sign in to the remote.\n\n" + result.Message +
                        "\n\nIf this is a private repository, configure a credential helper first:\n\n" +
                        "git config --global credential.helper manager");
                }
                else
                {
                    Dialogs.Error(this, "Clone", result.Message);
                }
            }
            catch (OperationCanceledException)
            {
                progressLabel.Text = "Cancelled.";
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, "Clone", ex.Message);
            }
            finally
            {
                _cloning = false;
                _cancellation?.Dispose();
                _cancellation = null;
                progressBar.Visible = false;
                urlBox.Enabled = true;
                folderBox.Enabled = true;
                browseButton.Enabled = true;
                UpdateTarget();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (_cloning)
            {
                _cancellation?.Cancel();
                return;
            }
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
            if (keyData == Keys.Enter && cloneButton.Enabled && !ModernShortcuts.YieldsToTextEntry(keyData, msg.HWnd))
            {
                cloneButton_Click(cloneButton, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
