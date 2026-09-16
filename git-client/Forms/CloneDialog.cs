using System;
using System.IO;
using System.Linq;
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

        // The folder name follows the repository until it is typed over.
        private bool _nameEdited;
        private bool _fillingName;

        public CloneDialog()
        {
            InitializeComponent();
            Theme.Apply(skin);
            folderBox.OverrideSkinFont = true;
            folderBox.Font = Fonts.Code(13f);
            nameBox.OverrideSkinFont = true;
            nameBox.Font = Fonts.Code(13f);
            sparseBox.OverrideSkinFont = true;
            sparseBox.Font = Fonts.Code(12.5f);
            folderBox.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // The inner text boxes only exist once the form is up.
            FillNameFromUrl();
            UpdateTarget();
            urlBox.Focus();
        }

        /// <summary>Path of the cloned repository once the dialog returns OK.</summary>
        public string ClonedPath { get; private set; }

        private void urlBox_TextChanged(object sender, EventArgs e)
        {
            FillNameFromUrl();
            UpdateTarget();
        }

        private void folderBox_TextChanged(object sender, EventArgs e) => UpdateTarget();

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            // Clearing the box hands the name back to the repository.
            if (!_fillingName) _nameEdited = nameBox.Text.Trim().Length > 0;
            UpdateTarget();
        }

        private void sparseSwitch_CheckedChanged(object sender, EventArgs e)
        {
            sparseBox.Enabled = sparseSwitch.Checked;
            if (sparseSwitch.Checked) sparseBox.Focus();
            UpdateTarget();
        }

        /// <summary>A download option changed; the summary under the boxes says what it means.</summary>
        private void option_CheckedChanged(object sender, EventArgs e) => UpdateTarget();

        /// <summary>The folders typed for a sparse checkout, with empty entries dropped.</summary>
        private string[] SparseFolders()
        {
            return (sparseBox.Text ?? string.Empty)
                .Split(new[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(f => f.Trim().Trim('/', '\\'))
                .Where(f => f.Length > 0)
                .ToArray();
        }

        private void resetNameButton_Click(object sender, EventArgs e)
        {
            _nameEdited = false;
            FillNameFromUrl();
            UpdateTarget();
            nameBox.Focus();
        }

        /// <summary>Puts the repository's own name in the box, unless the user typed one.</summary>
        private void FillNameFromUrl()
        {
            if (_nameEdited || !nameBox.IsHandleCreated) return;
            _fillingName = true;
            try
            {
                nameBox.Text = RepositoryNameFromUrl(urlBox.Text.Trim()) ?? string.Empty;
            }
            finally
            {
                _fillingName = false;
            }
        }

        private void UpdateTarget()
        {
            var target = BuildTargetPath(out var problem);
            if (problem == null && sparseSwitch.Checked && SparseFolders().Length == 0) problem = "Type the folders to check out, or turn that option off.";
            if (problem != null) target = null;
            targetLabel.Role = problem == null ? TextRole.Tertiary : TextRole.Warning;
            targetLabel.Text = problem ?? (target == null ? string.Empty : "→ " + target);
            targetLabel.Invalidate();
            cloneButton.Enabled = target != null && !_cloning;
            var suggestion = RepositoryNameFromUrl(urlBox.Text.Trim());
            resetNameButton.Enabled = !_cloning && suggestion != null && !string.Equals(suggestion, nameBox.Text.Trim(), StringComparison.Ordinal);
        }

        private string BuildTargetPath() => BuildTargetPath(out _);

        /// <summary>Where the clone goes, or null - with the reason, once there is enough to judge.</summary>
        private string BuildTargetPath(out string problem)
        {
            problem = null;
            var url = urlBox.Text.Trim();
            var folder = folderBox.Text.Trim();
            var name = nameBox.Text.Trim();
            if (url.Length == 0 || folder.Length == 0) return null;
            if (name.Length == 0)
            {
                problem = RepositoryNameFromUrl(url) == null ? null : "Give the folder a name.";
                return null;
            }
            if (name == "." || name == "..")
            {
                problem = "That folder name is reserved.";
                return null;
            }
            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                problem = "A folder name cannot contain \\ / : * ? \" < > |";
                return null;
            }
            try
            {
                return Path.Combine(folder, name);
            }
            catch (ArgumentException)
            {
                problem = "Windows will not take that folder name.";
                return null;
            }
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

        /// <summary>Picks a repository from the signed-in GitHub account and fills the URL in.</summary>
        private void githubButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new GitHubRepositoryDialog())
            {
                if (dialog.ShowDialog(this) != DialogResult.OK || dialog.SelectedRepository == null) return;
                urlBox.Text = dialog.SelectedRepository.CloneUrl;
                UpdateTarget();
                cloneButton.Focus();
            }
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

            var sparseFolders = sparseSwitch.Checked ? SparseFolders() : new string[0];
            _cloning = true;
            _cancellation = new CancellationTokenSource();
            cloneButton.Enabled = false;
            urlBox.Enabled = false;
            folderBox.Enabled = false;
            nameBox.Enabled = false;
            browseButton.Enabled = false;
            githubButton.Enabled = false;
            resetNameButton.Enabled = false;
            optionsCard.Enabled = false;
            progressBar.Visible = true;
            progressLabel.Text = "Cloning...";

            try
            {
                var progress = new Progress<string>(line => progressLabel.Text = line);
                var result = await GitRepository.CloneAsync(url, target, submodulesSwitch.Checked, progress, _cancellation.Token,
                    bloblessSwitch.Checked, sparseFolders.Length > 0);
                if (result.Succeeded)
                {
                    if (sparseFolders.Length > 0)
                    {
                        progressLabel.Text = "Checking out " + string.Join(", ", sparseFolders) + "...";
                        var sparse = await GitRepository.SetSparseFoldersAsync(target, sparseFolders, _cancellation.Token);
                        // The clone is good either way; a folder that is not there is worth saying out loud.
                        if (!sparse.Succeeded)
                        {
                            Dialogs.Warning(this, "Clone", "The repository was cloned, but those folders could not be checked out:" + Environment.NewLine + Environment.NewLine + sparse.Message);
                        }
                        else
                        {
                            var missing = await GitRepository.MissingFoldersAsync(target, sparseFolders, _cancellation.Token);
                            if (missing.Count > 0)
                            {
                                Dialogs.Warning(this, "Clone",
                                    "The repository was cloned, but it has no folder called " + string.Join(", ", missing) + "." + Environment.NewLine + Environment.NewLine +
                                    "Nothing was checked out for that name. Clone again with the right folder name to fix it.");
                            }
                        }
                    }
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
                nameBox.Enabled = true;
                browseButton.Enabled = true;
                githubButton.Enabled = true;
                optionsCard.Enabled = true;
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
