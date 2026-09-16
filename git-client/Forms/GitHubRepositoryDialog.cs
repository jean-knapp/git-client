using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitClient.Controls;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Forms
{
    /// <summary>
    /// Picks a repository from the signed-in GitHub account: its own, the ones it collaborates on
    /// and the ones its organisations hold, newest push first.
    /// </summary>
    public partial class GitHubRepositoryDialog : ModernForm
    {
        private readonly List<GitHubRepository> _all = new List<GitHubRepository>();
        private GitHubAuth.Token _token;
        private bool _working;

        public GitHubRepositoryDialog()
        {
            InitializeComponent();
            Theme.Apply(skin);
            filterBox.LeadingSvgIcon = Icons.Search;
            list.RowDoubleClick += list_RowDoubleClick;
            list.SelectionChanged += (s, e) => UpdateButtons();
        }

        /// <summary>The repository the user chose, once the dialog returns OK.</summary>
        public GitHubRepository SelectedRepository { get; private set; }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            filterBox.Focus();
            await SignInAndLoadAsync(null);
        }

        // ------------------------------------------------------------------ sign-in and loading

        private async Task SignInAndLoadAsync(string manualToken)
        {
            SetWorking(true);
            list.EmptyText = "Reading your repositories…";
            list.SetEntries(null);
            try
            {
                var token = manualToken != null
                    ? new GitHubAuth.Token(manualToken, "the token you pasted", null)
                    : await GitHubAuth.FindAsync();
                if (token == null)
                {
                    _token = null;
                    accountLabel.Text = "No GitHub sign-in found on this machine.";
                    list.EmptyText = "Sign in with a token to see your repositories.";
                    list.Invalidate();
                    return;
                }

                var user = await GitHubClient.GetUserAsync(token.Value);
                var repositories = await GitHubClient.GetRepositoriesAsync(token.Value);
                _token = token;
                accountLabel.Text = "Signed in as " + user.Login + " · " + token.Source;
                if (manualToken != null) await GitHubAuth.StoreAsync(user.Login, manualToken);

                _all.Clear();
                _all.AddRange(repositories.OrderByDescending(r => r.PushedUtc ?? DateTime.MinValue));
                list.EmptyText = _all.Count == 0 ? "This account has no repositories yet." : "No repository matches the filter.";
                ApplyFilter();
            }
            catch (GitHubException ex)
            {
                _token = null;
                accountLabel.Text = ex.IsAuthenticationProblem ? "GitHub did not accept the sign-in." : "GitHub could not be read.";
                list.EmptyText = ex.IsAuthenticationProblem
                    ? ex.Message + " Use a token with the repo scope."
                    : ex.Message;
                list.Invalidate();
            }
            catch (Exception ex)
            {
                _token = null;
                accountLabel.Text = "Could not reach GitHub.";
                list.EmptyText = ex.Message;
                list.Invalidate();
            }
            finally
            {
                SetWorking(false);
            }
        }

        private void SetWorking(bool working)
        {
            _working = working;
            // The button signs in when nothing was found, and swaps the account when one was.
            tokenButton.Text = _token == null ? "Sign in…" : "Switch…";
            progressBar.Visible = working;
            tokenButton.Enabled = !working;
            refreshButton.Enabled = !working;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            chooseButton.Enabled = !_working && list.SelectedEntry != null;
            chooseButton.Invalidate();
        }

        // ------------------------------------------------------------------ filtering and choosing

        private void filterBox_TextChanged(object sender, EventArgs e) => ApplyFilter();

        private void ApplyFilter()
        {
            var filter = (filterBox.Text ?? string.Empty).Trim();
            IEnumerable<GitHubRepository> shown = _all;
            if (filter.Length > 0)
            {
                shown = _all.Where(r =>
                    (r.FullName ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (r.Description ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            list.SetEntries(shown.ToList());
            UpdateButtons();
        }

        private void list_RowDoubleClick(object sender, RowMouseEventArgs e) => Choose();

        private void chooseButton_Click(object sender, EventArgs e) => Choose();

        private void Choose()
        {
            var entry = list.SelectedEntry;
            if (_working || entry == null) return;
            SelectedRepository = entry;
            DialogResult = DialogResult.OK;
            Close();
        }

        private async void refreshButton_Click(object sender, EventArgs e) => await SignInAndLoadAsync(null);

        private async void tokenButton_Click(object sender, EventArgs e)
        {
            var choice = Dialogs.Show(this, "GitHub token",
                "Sign in with a personal access token that has the repo scope, so private repositories are listed too."
                + Environment.NewLine + Environment.NewLine +
                "GitHub can create one for you with that scope pre-selected.",
                "Open GitHub", "I have a token", "Cancel");
            if (choice == DialogResult.Cancel) return;
            if (choice == DialogResult.OK)
            {
                try { Process.Start(GitHubAuth.TokenPageUrl); }
                catch (Exception) { }
            }

            using (var prompt = new TextInputDialog())
            {
                prompt.Caption = "GitHub token";
                prompt.Prompt = "Paste the token";
                if (prompt.ShowDialog(this) != DialogResult.OK) return;
                await SignInAndLoadAsync(prompt.Value.Trim());
            }
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
                cancelButton_Click(this, EventArgs.Empty);
                return true;
            }
            if (keyData == Keys.Enter && (filterBox.ContainsFocus || list.ContainsFocus))
            {
                Choose();
                return true;
            }
            // The filter keeps the caret while the arrows walk the list.
            if ((keyData == Keys.Down || keyData == Keys.Up) && filterBox.ContainsFocus && list.Count > 0)
            {
                list.Focus();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
