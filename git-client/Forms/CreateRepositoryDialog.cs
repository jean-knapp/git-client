using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitClient.Git;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Forms
{
    /// <summary>
    /// Creates the repository on GitHub and points the local one at it, so a first push does not
    /// need the website or the command line.
    /// </summary>
    public partial class CreateRepositoryDialog : ModernForm
    {
        private readonly GitRepository _repository;
        private readonly string _remoteName;
        private GitHubAuth.Token _token;
        private List<GitHubOwner> _owners = new List<GitHubOwner>();
        private GitHubOwner _owner;
        private bool _working;
        private readonly Dictionary<string, System.Drawing.Image> _avatars =
            new Dictionary<string, System.Drawing.Image>(StringComparer.Ordinal);
        private readonly string _suggestedName;

        public CreateRepositoryDialog(GitRepository repository, string suggestedName, string remoteName = "origin")
        {
            InitializeComponent();
            Theme.Apply(skin);
            _repository = repository;
            _remoteName = string.IsNullOrWhiteSpace(remoteName) ? "origin" : remoteName;
            _suggestedName = SanitizeName(suggestedName ?? repository?.Name);
        }

        /// <summary>The repository GitHub created, once the dialog returns OK.</summary>
        public GitHubRepository CreatedRepository { get; private set; }

        /// <summary>True when the user asked for the branch to be pushed afterwards.</summary>
        public bool PushRequested => pushSwitch.Checked;

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // The inner text box only exists once the form is up, so the suggestion is filled in
            // here rather than in the constructor.
            nameBox.Text = _suggestedName;
            nameBox.Focus();
            await SignInAsync(null);
        }

        // ------------------------------------------------------------------ sign-in

        private async Task SignInAsync(string manualToken)
        {
            SetWorking(true);
            try
            {
                var token = manualToken != null
                    ? new GitHubAuth.Token(manualToken, "the token you pasted", null)
                    : await GitHubAuth.FindAsync();

                if (token == null)
                {
                    _token = null;
                    accountLabel.Text = "No GitHub sign-in found. Use a token to continue.";
                    statusLabel.Text = string.Empty;
                    return;
                }

                _owners = await GitHubClient.GetOwnersAsync(token.Value);
                _token = token;
                _owner = _owners.FirstOrDefault();
                ownerButton.Text = _owner?.Login ?? "-";
                accountLabel.Text = "Signed in as " + _owner?.Login + " · " + token.Source;
                statusLabel.Text = string.Empty;
                if (manualToken != null) await GitHubAuth.StoreAsync(_owner?.Login, manualToken);

                await LoadAvatarsAsync();
                ownerButton.IconImage = AvatarFor(_owner);
                if (manualToken != null) await OfferToPinAccountAsync(_owner?.Login);
            }
            catch (GitHubException ex)
            {
                _token = null;
                accountLabel.Text = "GitHub did not accept the sign-in.";
                statusLabel.Text = ex.IsAuthenticationProblem
                    ? ex.Message + Environment.NewLine + "Use a token with the repo scope."
                    : ex.Message;
            }
            catch (Exception ex)
            {
                _token = null;
                accountLabel.Text = "Could not reach GitHub.";
                statusLabel.Text = ex.Message;
            }
            finally
            {
                SetWorking(false);
            }
        }

        private async void tokenButton_Click(object sender, EventArgs e)
        {
            var choice = Dialogs.Show(this, "GitHub token",
                "Sign in with a personal access token that has the repo scope (and read:org to list your organisations)."
                + Environment.NewLine + Environment.NewLine +
                "GitHub can create one for you with those scopes pre-selected.",
                "Open GitHub", "I have a token", "Cancel");
            if (choice == DialogResult.Cancel) return;
            if (choice == DialogResult.OK) Open(GitHubAuth.TokenPageUrl);

            using (var prompt = new TextInputDialog())
            {
                prompt.Caption = "GitHub token";
                prompt.Prompt = "Paste the token";
                if (prompt.ShowDialog(this) != DialogResult.OK) return;
                var token = prompt.Value;
                if (token.Length == 0) return;
                await SignInAsync(token);
            }
        }

        private static void Open(string url)
        {
            try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); }
            catch (Exception) { }
        }

        // ------------------------------------------------------------------ form state

        private void ownerButton_Click(object sender, EventArgs e)
        {
            if (_owners.Count == 0) return;
            ownerMenu.Items.Clear();
            foreach (var owner in _owners)
            {
                var captured = owner;
                var item = ownerMenu.Items.Add(owner.Login + (owner.IsOrganization ? "  (organisation)" : string.Empty));
                // The account's picture, so the list reads the way GitHub's own account switcher does.
                item.Icon = AvatarFor(owner);
                item.Checkable = item.Icon == null;
                item.Checked = item.Icon == null && _owner != null && string.Equals(_owner.Login, owner.Login, StringComparison.Ordinal);
                item.Click += (s, a) =>
                {
                    _owner = captured;
                    ownerButton.Text = captured.Login;
                    ownerButton.IconImage = AvatarFor(captured);
                    UpdateCreateButton();
                };
            }
            ownerMenu.Show(ownerButton, ownerButton.PointToScreen(new System.Drawing.Point(0, ownerButton.Height + 2)));
        }

        private System.Drawing.Image AvatarFor(GitHubOwner owner)
        {
            System.Drawing.Image image;
            return owner != null && _avatars.TryGetValue(owner.Login, out image) ? image : null;
        }

        private async Task LoadAvatarsAsync()
        {
            foreach (var owner in _owners)
            {
                if (_avatars.ContainsKey(owner.Login)) continue;
                // 16 px matches both the menu's icon column and the button's icon size.
                var image = await GitHubClient.GetAvatarAsync(owner.AvatarUrl, 16);
                if (image != null) _avatars[owner.Login] = image;
            }
        }

        /// <summary>
        /// Offers to name this account in the git config. Git Credential Manager shows its account
        /// picker on every push while more than one GitHub sign-in is stored and none is pinned.
        /// </summary>
        private async Task OfferToPinAccountAsync(string login)
        {
            if (string.IsNullOrEmpty(login)) return;
            var current = await GitHubAuth.GetPreferredAccountAsync();
            if (string.Equals(current, login, StringComparison.OrdinalIgnoreCase)) return;

            if (!Dialogs.Confirm(this, "GitHub account",
                "Always use " + login + " for github.com?"
                + Environment.NewLine + Environment.NewLine +
                "GitHub asks which account to use on every push while more than one sign-in is stored. "
                + "This answers it once by setting " + GitHubAuth.AccountConfigKey + " in your global git config.",
                "Use this account")) return;

            try
            {
                await GitHubAuth.SetPreferredAccountAsync(login);
                statusLabel.Text = "git will use " + login + " for github.com from now on.";
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Could not write the git config: " + ex.Message;
            }
        }

        private void field_TextChanged(object sender, EventArgs e) => UpdateCreateButton();

        private void SetWorking(bool working)
        {
            _working = working;
            Cursor = working ? Cursors.AppStarting : Cursors.Default;
            tokenButton.Enabled = !working;
            UpdateCreateButton();
        }

        private void UpdateCreateButton()
        {
            createButton.Enabled = !_working && _token != null && _owner != null && nameBox.Text.Trim().Length > 0;
            createButton.Invalidate();
        }

        /// <summary>GitHub only accepts letters, digits, dots, hyphens and underscores.</summary>
        private static string SanitizeName(string name)
        {
            var text = (name ?? string.Empty).Trim();
            var clean = new System.Text.StringBuilder();
            foreach (var c in text)
            {
                clean.Append(char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '_' ? c : '-');
            }
            return clean.ToString().Trim('-');
        }

        // ------------------------------------------------------------------ create

        private async void createButton_Click(object sender, EventArgs e)
        {
            if (!createButton.Enabled) return;
            var name = SanitizeName(nameBox.Text);
            nameBox.Text = name;

            SetWorking(true);
            statusLabel.Text = "Creating " + _owner.Login + "/" + name + " on GitHub...";
            try
            {
                var created = await GitHubClient.CreateRepositoryAsync(
                    _token.Value, _owner, name, descriptionBox.Text, privateSwitch.Checked);

                var remotes = await _repository.GetRemotesAsync();
                var result = remotes.Contains(_remoteName)
                    ? await _repository.SetRemoteUrlAsync(_remoteName, created.CloneUrl)
                    : await _repository.AddRemoteAsync(_remoteName, created.CloneUrl);
                if (!result.Succeeded)
                {
                    statusLabel.Text = "The repository was created, but the remote could not be set:" +
                        Environment.NewLine + result.Message.Trim();
                    SetWorking(false);
                    return;
                }

                CreatedRepository = created;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (GitHubException ex)
            {
                statusLabel.Text = ex.Message;
                SetWorking(false);
            }
            catch (Exception ex)
            {
                statusLabel.Text = ex.Message;
                SetWorking(false);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape && !_working)
            {
                cancelButton_Click(cancelButton, EventArgs.Empty);
                return true;
            }
            if (keyData == Keys.Enter && createButton.Enabled)
            {
                createButton_Click(createButton, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
