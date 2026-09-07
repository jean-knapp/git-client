using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitClient.Git;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Forms
{
    /// <summary>
    /// Adds, re-points and removes a repository's remotes, so connecting a repository to GitHub
    /// never needs the command line.
    /// </summary>
    public partial class RemoteDialog : ModernForm
    {
        private readonly GitRepository _repository;
        private List<RemoteInfo> _remotes = new List<RemoteInfo>();
        private RemoteInfo _selected;
        private bool _loading;

        public RemoteDialog(GitRepository repository)
        {
            InitializeComponent();
            Theme.Apply(skin);
            _repository = repository;
        }

        /// <summary>True when a remote was added, re-pointed or removed.</summary>
        public bool Changed { get; private set; }

        /// <summary>The remote to use afterwards - the one just saved, or the first one there is.</summary>
        public string CurrentRemoteName =>
            _selected?.Name ?? _remotes.Select(r => r.Name).FirstOrDefault();

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await LoadAsync(null);
            (_remotes.Count == 0 ? urlBox : (Control)urlBox).Focus();
        }

        private async Task LoadAsync(string select)
        {
            _remotes = await _repository.GetRemoteListAsync();
            var wanted = select
                ?? _selected?.Name
                ?? (_remotes.Any(r => r.Name == "origin") ? "origin" : _remotes.Select(r => r.Name).FirstOrDefault());
            _selected = _remotes.FirstOrDefault(r => string.Equals(r.Name, wanted, StringComparison.Ordinal));
            ShowSelection();
        }

        private void ShowSelection()
        {
            _loading = true;
            try
            {
                bool isNew = _selected == null;
                remoteButton.Text = isNew ? "New remote" : _selected.Name;
                nameBox.Text = isNew ? (_remotes.Any(r => r.Name == "origin") ? string.Empty : "origin") : _selected.Name;
                urlBox.Text = isNew ? string.Empty : (_selected.FetchUrl ?? string.Empty);
                removeButton.Enabled = !isNew;
                subLabel.Text = _remotes.Count == 0
                    ? "This repository has no remote yet. Add one to push it to GitHub."
                    : "Where this repository fetches from and pushes to.";
                statusLabel.Text = string.Empty;
            }
            finally
            {
                _loading = false;
            }
            UpdateSaveButton();
        }

        private void field_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            statusLabel.Text = string.Empty;
            UpdateSaveButton();
        }

        private void UpdateSaveButton()
        {
            saveButton.Enabled = nameBox.Text.Trim().Length > 0 && urlBox.Text.Trim().Length > 0;
            saveButton.Invalidate();
        }

        private void remoteButton_Click(object sender, EventArgs e)
        {
            remoteMenu.Items.Clear();
            foreach (var remote in _remotes)
            {
                var captured = remote;
                var item = remoteMenu.Items.Add(remote.Name);
                item.Checkable = true;
                item.Checked = _selected != null && string.Equals(_selected.Name, remote.Name, StringComparison.Ordinal);
                item.Click += (s, a) => { _selected = captured; ShowSelection(); };
            }
            var add = remoteMenu.Items.Add("Add a remote...");
            add.BeginGroup = _remotes.Count > 0;
            add.Click += (s, a) => { _selected = null; ShowSelection(); nameBox.Focus(); };
            remoteMenu.Show(remoteButton, remoteButton.PointToScreen(new System.Drawing.Point(0, remoteButton.Height + 2)));
        }

        private async void saveButton_Click(object sender, EventArgs e)
        {
            if (!saveButton.Enabled) return;
            var name = nameBox.Text.Trim();
            var url = NormalizeUrl(urlBox.Text);
            urlBox.Text = url;

            try
            {
                GitResult result;
                if (_selected == null)
                {
                    result = await _repository.AddRemoteAsync(name, url);
                }
                else
                {
                    if (!string.Equals(_selected.Name, name, StringComparison.Ordinal))
                    {
                        result = await _repository.RenameRemoteAsync(_selected.Name, name);
                        if (!result.Succeeded) { Fail(result); return; }
                    }
                    result = await _repository.SetRemoteUrlAsync(name, url);
                }
                if (!result.Succeeded) { Fail(result); return; }

                Changed = true;
                await LoadAsync(name);
                statusLabel.Text = "Saved " + name + " → " + url;
            }
            catch (Exception ex)
            {
                statusLabel.Text = ex.Message;
            }
        }

        private async void removeButton_Click(object sender, EventArgs e)
        {
            if (_selected == null) return;
            var name = _selected.Name;
            if (!Dialogs.Confirm(this, "Remove remote",
                "Remove the remote \"" + name + "\"?\n\nThe repository stays; only the link to " +
                (_selected.FetchUrl ?? "the remote") + " goes away.", "Remove")) return;

            var result = await _repository.RemoveRemoteAsync(name);
            if (!result.Succeeded) { Fail(result); return; }
            Changed = true;
            _selected = null;
            await LoadAsync(null);
            statusLabel.Text = "Removed " + name + ".";
        }

        private void Fail(GitResult result)
        {
            statusLabel.Text = string.IsNullOrWhiteSpace(result.Message) ? "git refused the change." : result.Message.Trim();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            DialogResult = Changed ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Accepts what people actually paste: a full URL, "github.com/owner/repo", or just
        /// "owner/repo". SSH and other schemes are left exactly as typed.
        /// </summary>
        public static string NormalizeUrl(string text)
        {
            var url = (text ?? string.Empty).Trim().Trim('"');
            if (url.Length == 0) return url;
            if (url.IndexOf("://", StringComparison.Ordinal) >= 0) return url;
            if (url.StartsWith("git@", StringComparison.OrdinalIgnoreCase)) return url;

            if (url.StartsWith("github.com/", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("www.github.com/", StringComparison.OrdinalIgnoreCase))
            {
                return "https://" + url.TrimStart('w', '.');
            }

            // owner/repository
            var parts = url.Split('/');
            if (parts.Length == 2 && parts.All(p => p.Length > 0 && p.IndexOf(' ') < 0))
            {
                var repository = parts[1].EndsWith(".git", StringComparison.OrdinalIgnoreCase) ? parts[1] : parts[1] + ".git";
                return "https://github.com/" + parts[0] + "/" + repository;
            }
            return url;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                closeButton_Click(closeButton, EventArgs.Empty);
                return true;
            }
            if (keyData == Keys.Enter && saveButton.Enabled)
            {
                saveButton_Click(saveButton, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
