using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitClient.Controls;
using GitClient.Forms;
using GitClient.Git;
using GitClient.Graph;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Views
{
    /// <summary>The Fluent repository window: command bar, history and detail cards, changes and composer.</summary>
    public partial class RepositoryView : ModernUserControl
    {
        private GitRepository _repository;
        private RepositoryStatus _status = new RepositoryStatus();
        private List<RefInfo> _refs = new List<RefInfo>();
        private List<CommitInfo> _commits = new List<CommitInfo>();
        private List<StashInfo> _stashes = new List<StashInfo>();
        private RepositoryState _state = RepositoryState.None;
        private GraphLayout _layout;
        private Dictionary<string, LineDelta> _stagedStats = new Dictionary<string, LineDelta>(StringComparer.Ordinal);
        private Dictionary<string, LineDelta> _unstagedStats = new Dictionary<string, LineDelta>(StringComparer.Ordinal);

        private bool _busy;
        private bool _refreshing;
        private bool _refreshQueued;
        private DateTime _lastRefresh;
        private DateTime? _lastFetch;
        private bool _amendMessageLoaded;
        private bool _allBranches = true;
        private ModernContextMenu _dynamicMenu;
        private CancellationTokenSource _aiCancellation;
        private readonly StringBuilder _outputBuffer = new StringBuilder();

        private const int WatchDebounceMs = 400;
        private readonly System.Windows.Forms.Timer _watchTimer = new System.Windows.Forms.Timer();
        private FileSystemWatcher _worktreeWatcher;
        private FileSystemWatcher _gitWatcher;
        private string _gitDirectory;

        public event EventHandler TitleChanged;

        public RepositoryView()
        {
            InitializeComponent();
            _watchTimer.Interval = WatchDebounceMs;
            _watchTimer.Tick += WatchTimer_Tick;
            Disposed += (s, e) => StopWatching();
            Theme.Changed += OnThemeChanged;
            ApplySettings(AppSettings.Current);
        }

        // ------------------------------------------------------------------ identity

        public GitRepository Repository => _repository;
        public string RepositoryPath => _repository?.WorkingDirectory;
        public string TabTitle => _repository?.Name ?? "Repository";

        public string TabBranch
        {
            get
            {
                if (_status == null) return string.Empty;
                if (_status.IsDetached) return "detached";
                return _status.Branch ?? string.Empty;
            }
        }

        public string TabToolTip => _repository == null ? string.Empty : _repository.WorkingDirectory;

        // ------------------------------------------------------------------ theme and settings

        private ThemePalette P => Theme.Palette;

        private void OnThemeChanged(object sender, EventArgs e) => ApplyThemeToInputs();

        /// <summary>
        /// The composer's text boxes sit inside a card and must not paint their own border, so they
        /// opt out of the skin and take their colours from the palette directly.
        /// </summary>
        private void ApplyThemeToInputs()
        {
            var p = P;
            var fieldFill = p.FillOn(p.Layer);
            StyleFlatTextBox(summaryBox, fieldFill, 13.5f);
            StyleFlatTextBox(descriptionBox, fieldFill, 13.5f);
            StyleFlatTextBox(outputBox, p.Layer, 12f, monospace: true);
            conflictBanner.CustomFill = Color.FromArgb(26, p.Warning);
            conflictBanner.CustomBorder = Color.FromArgb(71, p.Warning);
            conflictBanner.Invalidate();
        }

        private static void StyleFlatTextBox(ModernTextBox box, Color fill, float sizePx, bool monospace = false)
        {
            var p = Theme.Palette;
            box.UseParentSkin = false;
            box.CornerStyle = ModernWinForms.Enums.CornerStyle.Square;
            box.Colors.BorderColor = Color.Transparent;
            box.Colors.Normal.BackColor = fill;
            box.Colors.Normal.ForeColor = p.Foreground;
            box.Colors.Hover.BackColor = fill;
            box.Colors.Hover.BorderColor = Color.Transparent;
            box.Colors.Active.BackColor = fill;
            box.Colors.Active.BorderColor = Color.Transparent;
            box.Colors.Disabled.BackColor = fill;
            box.Colors.Disabled.ForeColor = p.Foreground3;
            box.Colors.Disabled.BorderColor = Color.Transparent;
            box.ScrollBarColors.ThumbColor = p.Fill2On(fill);
            box.ScrollBarColors.ThumbHoverColor = p.Foreground3;
            box.OverrideSkinFont = true;
            box.Font = monospace ? Fonts.Code(sizePx) : Fonts.Ui(sizePx);
        }

        /// <summary>Applies the user's appearance and history preferences.</summary>
        public void ApplySettings(AppSettings settings)
        {
            historyList.RowHeight = settings.HistoryRowHeight;
            historyList.RelativeDates = settings.RelativeDates;
            diffView.ViewLayout = settings.DiffLayout;
            diffLayoutToggle.SelectedIndex = settings.DiffLayout == DiffLayout.Split ? 1 : 0;
            ApplyThemeToInputs();
        }

        // ------------------------------------------------------------------ load and watch

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            GitRunner.CommandExecuted += OnGitCommandExecuted;
            ApplyThemeToInputs();
            AddFilterGlyph();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            GitRunner.CommandExecuted -= OnGitCommandExecuted;
            base.OnHandleDestroyed(e);
        }

        private void AddFilterGlyph()
        {
            if (filterBox.Buttons.Count > 0) return;
            var glyph = new ModernTextBoxButton
            {
                SvgIcon = Icons.Search,
                SvgIconSize = 14,
                ToolTipText = "Filter the history",
            };
            filterBox.ButtonAlignment = ModernTextBoxButtonAlignment.Left;
            filterBox.Buttons.Add(glyph);
        }

        public async Task LoadAsync(string path)
        {
            _repository = new GitRepository(path);
            statusMessageLabel.Text = "Loading " + _repository.Name + "...";
            try { _gitDirectory = await _repository.GetGitDirectoryAsync(); }
            catch (GitException) { _gitDirectory = null; }
            await RefreshAsync();
            StartWatching();
        }

        private void StartWatching()
        {
            StopWatching();
            if (_repository == null) return;
            try
            {
                _worktreeWatcher = CreateWatcher(_repository.WorkingDirectory, OnWorktreeChanged);
                if (_gitDirectory != null && Directory.Exists(_gitDirectory))
                {
                    _gitWatcher = CreateWatcher(_gitDirectory, OnGitDirectoryChanged);
                }
            }
            catch (Exception)
            {
                StopWatching();
            }
        }

        private FileSystemWatcher CreateWatcher(string path, FileSystemEventHandler handler)
        {
            var watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.Size,
                InternalBufferSize = 64 * 1024,
            };
            watcher.Changed += handler;
            watcher.Created += handler;
            watcher.Deleted += handler;
            watcher.Renamed += (s, e) => handler(s, e);
            watcher.Error += (s, e) => ScheduleWatchRefresh();
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        private void StopWatching()
        {
            _watchTimer.Stop();
            DisposeWatcher(ref _worktreeWatcher);
            DisposeWatcher(ref _gitWatcher);
        }

        private static void DisposeWatcher(ref FileSystemWatcher watcher)
        {
            if (watcher == null) return;
            try
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            catch { }
            watcher = null;
        }

        private void OnWorktreeChanged(object sender, FileSystemEventArgs e)
        {
            if (IsInsideGitDirectory(e.FullPath)) return;
            ScheduleWatchRefresh();
        }

        private void OnGitDirectoryChanged(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            if (path.IndexOf("\\objects\\", StringComparison.OrdinalIgnoreCase) >= 0) return;
            if (path.IndexOf("\\lfs\\", StringComparison.OrdinalIgnoreCase) >= 0) return;
            if (path.EndsWith(".lock", StringComparison.OrdinalIgnoreCase)) return;
            ScheduleWatchRefresh();
        }

        private bool IsInsideGitDirectory(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return false;
            if (_gitDirectory != null && fullPath.StartsWith(_gitDirectory, StringComparison.OrdinalIgnoreCase)) return true;
            return fullPath.IndexOf("\\.git\\", StringComparison.OrdinalIgnoreCase) >= 0
                || fullPath.EndsWith("\\.git", StringComparison.OrdinalIgnoreCase);
        }

        private void ScheduleWatchRefresh()
        {
            if (IsDisposed) return;
            try
            {
                if (InvokeRequired) BeginInvoke(new Action(RestartWatchTimer));
                else RestartWatchTimer();
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        private void RestartWatchTimer()
        {
            if (IsDisposed) return;
            _watchTimer.Stop();
            _watchTimer.Start();
        }

        private async void WatchTimer_Tick(object sender, EventArgs e)
        {
            _watchTimer.Stop();
            if (_repository == null || IsDisposed) return;
            if (_busy)
            {
                _watchTimer.Start();
                return;
            }
            await RefreshAsync();
        }

        // ------------------------------------------------------------------ refresh

        public async Task RefreshAsync()
        {
            if (_repository == null) return;
            if (_refreshing) { _refreshQueued = true; return; }
            _refreshing = true;
            progressBar.Visible = true;
            try
            {
                do
                {
                    _refreshQueued = false;
                    await ReloadAsync();
                }
                while (_refreshQueued);
            }
            finally
            {
                _refreshing = false;
                _lastRefresh = DateTime.UtcNow;
                progressBar.Visible = _busy;
            }
        }

        public async Task RefreshIfIdleAsync()
        {
            if (_repository == null || _busy || _refreshing) return;
            if ((DateTime.UtcNow - _lastRefresh).TotalMilliseconds < 600) return;
            await RefreshAsync();
        }

        private async Task ReloadAsync()
        {
            try
            {
                var statusTask = _repository.GetStatusAsync();
                var refsTask = _repository.GetRefsAsync();
                var logTask = _repository.GetLogAsync(AppSettings.Current.MaxCommits, _allBranches);
                var stashTask = _repository.GetStashesAsync();
                var stateTask = _repository.GetStateAsync();
                var stagedTask = _repository.GetWorkingNumstatAsync(true);
                var unstagedTask = _repository.GetWorkingNumstatAsync(false);
                await Task.WhenAll(statusTask, refsTask, logTask, stashTask, stateTask, stagedTask, unstagedTask);

                _status = statusTask.Result;
                _refs = refsTask.Result;
                _commits = logTask.Result;
                _stashes = stashTask.Result;
                _state = stateTask.Result;
                _stagedStats = stagedTask.Result;
                _unstagedStats = unstagedTask.Result;

                AttachRefsToCommits();
                _layout = GraphLayout.Build(_commits, _status.HeadSha, _status.HasChanges);
                historyList.SetData(_layout, _status);
                changesList.SetStatus(_status, _stagedStats, _unstagedStats);

                UpdateCommandBar();
                UpdateChangesHeader();
                UpdateConflictBanner();
                UpdateComposer();
                UpdateStatusBar();
                TitleChanged?.Invoke(this, EventArgs.Empty);

                if (historyList.SelectedRow == null) historyList.SelectWorkInProgress();
                historyCountLabel.Text = FormatHistoryCount();
                if (statusMessageLabel.Text.EndsWith("...", StringComparison.Ordinal)) statusMessageLabel.Text = string.Empty;
                await UpdateDetailAsync();
            }
            catch (Exception ex)
            {
                statusMessageLabel.Text = ex.Message;
            }
        }

        private void AttachRefsToCommits()
        {
            var bySha = new Dictionary<string, CommitInfo>(StringComparer.Ordinal);
            foreach (var c in _commits) bySha[c.Sha] = c;
            foreach (var r in _refs)
            {
                CommitInfo commit;
                if (r.Sha != null && bySha.TryGetValue(r.Sha, out commit)) commit.Refs.Add(r);
            }
        }

        // ------------------------------------------------------------------ chrome updates

        private void UpdateCommandBar()
        {
            branchButton.Text = _status.IsDetached ? "detached at " + Short(_status.HeadSha) : (_status.Branch ?? "no branch");
            pushButton.BadgeText = _status.Ahead > 0 ? _status.Ahead.ToString() : null;
            pullButton.BadgeText = _status.Behind > 0 ? _status.Behind.ToString() : null;
            pushButton.Enabled = !_status.IsDetached;
            stashPopItem.Enabled = _stashes.Count > 0;
            stashApplyItem.Enabled = _stashes.Count > 0;
            stashDropItem.Enabled = _stashes.Count > 0;
            scopeButton.Text = _allBranches ? "All branches" : "This branch";
            scopeButton.Width = scopeButton.PreferredWidth;
            scopeButton.Left = historyHeader.ClientSize.Width - 12 - scopeButton.Width;
            LayoutCommandBar();
        }

        /// <summary>Packs the command bar left to right, since every button sizes to its label.</summary>
        private void LayoutCommandBar()
        {
            const int gap = 4;
            int x = 12;
            branchButton.Width = Math.Max(212, branchButton.PreferredWidth);
            branchButton.Left = x;
            x += branchButton.Width + gap + 8;

            commandSeparator1.Left = x;
            x += 1 + 8 + gap;

            foreach (var button in new[] { fetchButton, pullButton, pushButton })
            {
                button.Width = button.PreferredWidth;
                button.Left = x;
                x += button.Width + gap;
            }

            x += 8;
            commandSeparator2.Left = x;
            x += 1 + 8 + gap;

            foreach (var button in new[] { newBranchButton, stashButton })
            {
                button.Width = button.PreferredWidth;
                button.Left = x;
                x += button.Width + gap;
            }

            int right = commandBar.ClientSize.Width - 12;
            settingsButton.Left = right - 32; right -= 32 + gap;
            logButton.Left = right - 32; right -= 32 + gap;
            terminalButton.Left = right - 32; right -= 32 + gap;
            filterBox.Width = Math.Min(240, Math.Max(120, right - x - 8));
            filterBox.Left = right - filterBox.Width;
        }

        private void UpdateChangesHeader()
        {
            int total = _status.TotalChanges;
            changesCountChip.Text = total.ToString();
            changesCountChip.Visible = total > 0;

            // Both header actions size to their labels, then pack against the right edge.
            int right = changesHeader.ClientSize.Width - 8;
            overflowButton.SetBounds(right - 28, 8, 28, 28);
            stageAllButton.Width = stageAllButton.PreferredWidth;
            stageAllButton.Left = right - 28 - 4 - stageAllButton.Width;

            stageAllButton.Enabled = _status.Unstaged.Count > 0;
            unstageAllItem.Enabled = _status.Staged.Count > 0;
            discardAllItem.Enabled = _status.HasChanges;
            stashSelectedItem.Enabled = _status.HasChanges;
        }

        private void UpdateConflictBanner()
        {
            bool visible = _state != RepositoryState.None;
            conflictBanner.Visible = visible;
            if (!visible) return;

            string what;
            switch (_state)
            {
                case RepositoryState.Merging: what = "Merge"; break;
                case RepositoryState.Rebasing: what = "Rebase"; break;
                case RepositoryState.CherryPicking: what = "Cherry-pick"; break;
                case RepositoryState.Reverting: what = "Revert"; break;
                default: what = "Bisect"; break;
            }

            int conflicts = _status.ConflictCount;
            int totalFiles = _status.TotalChanges;
            conflictTitleLabel.Text = conflicts > 0
                ? what + " in progress — " + conflicts + " of " + totalFiles + " files have conflicts"
                : what + " in progress";
            conflictBodyLabel.Text = conflicts > 0
                ? "Resolve every conflict, stage the files, then continue."
                : "Stage your resolution, then continue.";
            continueButton.Enabled = conflicts == 0 && _state != RepositoryState.Bisecting;
            continueButton.Text = "Continue " + what.ToLowerInvariant();
        }

        private void UpdateComposer()
        {
            bool hasMessage = summaryBox.Text.Trim().Length > 0;
            bool amend = amendSwitch.Checked;
            int staged = _status.Staged.Count;

            branchChip.Text = _status.IsDetached ? Short(_status.HeadSha) : (_status.Branch ?? "HEAD");
            branchChip.Visible = true;

            commitButton.Enabled = hasMessage && (staged > 0 || amend) && !_busy;
            commitPushButton.Enabled = commitButton.Enabled && !_status.IsDetached;
            commitButton.Text = amend
                ? "Amend commit"
                : (staged > 0 ? "Commit " + staged + (staged == 1 ? " file" : " files") : "Commit");
            commitButton.Invalidate();
        }

        private void UpdateStatusBar()
        {
            statusBranchLabel.Text = _status.IsDetached ? "detached at " + Short(_status.HeadSha) : (_status.Branch ?? "no branch");
            statusBranchLabel.Width = Math.Max(60, statusBranchLabel.PreferredWidth + 4);

            var upstream = new StringBuilder();
            if (_status.Upstream != null)
            {
                upstream.Append(_status.Upstream);
                if (_status.Ahead > 0) upstream.Append(" · ahead ").Append(_status.Ahead);
                if (_status.Behind > 0) upstream.Append(" · behind ").Append(_status.Behind);
            }
            else if (!_status.IsDetached && _status.Branch != null) upstream.Append("no upstream");
            statusUpstreamLabel.Text = upstream.ToString();
            statusUpstreamLabel.Left = statusBranchLabel.Right + 16;
            statusUpstreamLabel.Width = Math.Max(40, statusUpstreamLabel.PreferredWidth + 4);

            var counts = historyList.LoadedCount + " commits loaded";
            if (_stashes.Count > 0) counts += " · " + _stashes.Count + " stash" + (_stashes.Count == 1 ? string.Empty : "es");
            statusCountLabel.Text = counts;
            statusCountLabel.Left = statusUpstreamLabel.Right + 16;
            statusCountLabel.Width = Math.Max(40, statusCountLabel.PreferredWidth + 4);

            if (_lastFetch.HasValue && !_busy)
            {
                statusMessageLabel.Text = "Fetched " + HistoryListControl.Relative(_lastFetch.Value);
            }
        }

        private static string Short(string sha) => string.IsNullOrEmpty(sha) ? string.Empty : (sha.Length > 7 ? sha.Substring(0, 7) : sha);

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutCommandBar();
        }

        // ------------------------------------------------------------------ detail card

        private async Task UpdateDetailAsync()
        {
            var row = historyList.SelectedRow;
            if (row == null)
            {
                detailAvatar.PersonName = string.Empty;
                detailSubjectLabel.Text = "Select a commit to see its details.";
                detailSubjectLabel.Role = TextRole.Tertiary;
                detailMetaLabel.Text = string.Empty;
                detailShaChip.Visible = false;
                detailParentLabel.Text = string.Empty;
                copyShaButton.Enabled = false;
                revertButton.Enabled = false;
                commitFilesList.Clear();
                diffView.Clear();
                diffPathLabel.Text = string.Empty;
                return;
            }

            if (row.IsWorkInProgress)
            {
                detailAvatar.PersonName = string.Empty;
                detailSubjectLabel.Text = "Uncommitted changes";
                detailSubjectLabel.Role = TextRole.Primary;
                detailMetaLabel.Text = "working tree · " + _status.TotalChanges + (_status.TotalChanges == 1 ? " file" : " files");
                detailShaChip.Visible = false;
                detailParentLabel.Text = string.Empty;
                copyShaButton.Enabled = false;
                revertButton.Enabled = false;

                var working = _status.Staged.Concat(_status.Unstaged).ToList();
                var stats = new Dictionary<string, LineDelta>(StringComparer.Ordinal);
                foreach (var pair in _unstagedStats) stats[pair.Key] = pair.Value;
                foreach (var pair in _stagedStats) stats[pair.Key] = pair.Value;
                commitFilesList.SetFiles(working, stats,
                    working.Count + (working.Count == 1 ? " file changed" : " files changed"));
                if (commitFilesList.SelectedChange == null) commitFilesList.SelectFirst();
                await ShowDiffForSelectionAsync();
                return;
            }

            var commit = row.Commit;
            detailAvatar.PersonName = commit.AuthorName;
            detailSubjectLabel.Text = commit.Subject;
            detailSubjectLabel.Role = TextRole.Primary;
            detailMetaLabel.Text = commit.AuthorName + "  ·  " + historyList.FormatDate(commit.AuthorDate) + "  ·  ";
            detailMetaLabel.Width = detailMetaLabel.PreferredWidth + 2;
            detailShaChip.Visible = true;
            detailShaChip.Text = commit.ShortSha;
            detailShaChip.Left = detailMetaLabel.Right;
            detailParentLabel.Text = commit.Parents.Count > 0 ? "·  parent " + Short(commit.Parents[0]) : "·  root commit";
            detailParentLabel.Left = detailShaChip.Right + 8;
            copyShaButton.Enabled = true;
            revertButton.Enabled = true;

            try
            {
                var filesTask = _repository.GetCommitFilesAsync(commit);
                var statsTask = _repository.GetCommitNumstatAsync(commit);
                await Task.WhenAll(filesTask, statsTask);
                if (historyList.SelectedRow != row) return;
                commitFilesList.SetFiles(filesTask.Result, statsTask.Result);
                commitFilesList.SelectFirst();
                await ShowDiffForSelectionAsync();
            }
            catch (Exception ex)
            {
                statusMessageLabel.Text = ex.Message;
            }
        }

        private async Task ShowDiffForSelectionAsync()
        {
            var change = commitFilesList.SelectedChange;
            if (change == null)
            {
                diffView.Clear();
                diffPathLabel.Text = string.Empty;
                return;
            }
            diffPathLabel.Text = change.Path;
            var row = historyList.SelectedRow;
            try
            {
                DiffDocument document;
                if (row == null || row.IsWorkInProgress) document = await _repository.GetWorkingDiffAsync(change);
                else document = await _repository.GetCommitFileDiffAsync(row.Commit, change);
                diffView.SetDocument(document);
            }
            catch (Exception ex)
            {
                statusMessageLabel.Text = ex.Message;
            }
        }

        // ------------------------------------------------------------------ command helpers

        private void SetBusy(bool busy, string text)
        {
            _busy = busy;
            progressBar.Visible = busy || _refreshing;
            commandBar.Enabled = !busy;
            if (text != null) statusMessageLabel.Text = text;
            UpdateComposer();
            Cursor = busy ? Cursors.AppStarting : Cursors.Default;
        }

        private async Task ExecuteAsync(string busyText, Func<Task> work)
        {
            if (_busy || _repository == null) return;
            SetBusy(true, busyText);
            try
            {
                await work();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                SetBusy(false, null);
                Dialogs.Error(this, "Git", ex.Message);
                await RefreshAsync();
                return;
            }
            SetBusy(false, null);
            await RefreshAsync();
        }

        private void ReportResult(GitResult result, string title, string successMessage)
        {
            if (result.Succeeded)
            {
                if (successMessage != null) statusMessageLabel.Text = successMessage;
                return;
            }
            if (result.HasConflicts)
            {
                Dialogs.Warning(this, title, "Stopped because of conflicts.\n\n" + result.Message +
                    "\n\nResolve the conflicts in the Changes pane, stage them, then press Continue.");
                return;
            }
            Dialogs.Error(this, title, result.Message);
        }

        private async Task<GitResult> WithCredentialRetryAsync(string title, Func<Task<GitResult>> operation)
        {
            var result = await operation();
            if (!result.NeedsCredentials) return result;
            if (!await TryEnableCredentialHelperAsync(title, result)) return result;
            return await operation();
        }

        private async Task<bool> TryEnableCredentialHelperAsync(string title, GitResult failure)
        {
            var configured = await CredentialHelper.GetConfiguredAsync();
            if (!string.IsNullOrEmpty(configured))
            {
                Dialogs.Error(this, title,
                    "Git could not sign in to the remote.\n\n" + failure.Message +
                    "\n\nThe credential helper in use is \"" + configured + "\". " +
                    "If your saved sign-in has expired, clear it in Windows Credential Manager and try again.");
                return false;
            }

            var manager = CredentialHelper.FindManager();
            if (manager == null)
            {
                Dialogs.Error(this, title,
                    "Git has no credential helper configured, so it cannot sign in to the remote.\n\n" + failure.Message +
                    "\n\nInstall Git Credential Manager, or configure a helper yourself with:\n\n" +
                    "git config --global credential.helper manager");
                return false;
            }

            if (!Dialogs.Confirm(this, title,
                "Git has no credential helper configured, so it cannot sign in to the remote.\n\n" +
                "Turn on Git Credential Manager? It opens a browser window the first time and then stores " +
                "your sign-in in Windows Credential Manager.\n\n" +
                "This sets credential.helper=manager in your global git config.", "Turn on and retry"))
            {
                return false;
            }

            try
            {
                await CredentialHelper.EnableManagerAsync();
                statusMessageLabel.Text = "Git Credential Manager enabled. Retrying...";
                return true;
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, title, "Could not write the git config:\n\n" + ex.Message);
                return false;
            }
        }

        private IProgress<string> CreateProgress()
        {
            return new Progress<string>(line =>
            {
                if (!IsDisposed) statusMessageLabel.Text = line;
            });
        }

        private void OnGitCommandExecuted(object sender, GitCommandEventArgs e)
        {
            if (_repository == null || e.WorkingDirectory == null) return;
            if (!string.Equals(Path.GetFullPath(e.WorkingDirectory).TrimEnd('\\'), _repository.WorkingDirectory, StringComparison.OrdinalIgnoreCase)) return;
            var line = "> " + e.Result.CommandLine + "  [" + e.Result.ExitCode + ", " + (int)e.Result.Duration.TotalMilliseconds + " ms]";
            var detail = e.Result.StandardError.Trim();
            if (detail.Length > 0) line += Environment.NewLine + detail;
            AppendOutput(line);
        }

        private void AppendOutput(string text)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                try { BeginInvoke(new Action<string>(AppendOutput), text); } catch { }
                return;
            }
            _outputBuffer.AppendLine(text);
            if (_outputBuffer.Length > 200000) _outputBuffer.Remove(0, _outputBuffer.Length - 120000);
            if (outputPanel.Visible) outputBox.Text = _outputBuffer.ToString();
        }

        // ------------------------------------------------------------------ command bar handlers

        private void branchButton_Click(object sender, EventArgs e)
        {
            _dynamicMenu?.Dispose();
            var menu = new ModernContextMenu { EnableSearch = true };
            _dynamicMenu = menu;

            foreach (var branch in _refs.Where(r => r.Kind == RefKind.LocalBranch).OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase))
            {
                var captured = branch;
                var item = menu.Items.Add(branch.Name + Track(branch));
                item.SvgIcon = Icons.Laptop;
                item.Checkable = true;
                item.Checked = branch.Name == _status.Branch;
                item.Click += async (s, a) =>
                {
                    if (captured.Name == _status.Branch) return;
                    await ExecuteAsync("Switching to " + captured.Name + "...", () => _repository.CheckoutAsync(captured.Name));
                };
            }

            bool first = true;
            foreach (var branch in _refs.Where(r => r.Kind == RefKind.RemoteBranch).OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase))
            {
                var captured = branch;
                var item = menu.Items.Add(branch.Name);
                item.SvgIcon = Icons.Cloud;
                if (first) { item.BeginGroup = true; first = false; }
                item.Click += async (s, a) =>
                    await ExecuteAsync("Switching to " + captured.ShortName + "...", () => _repository.CheckoutRemoteBranchAsync(captured));
            }

            var create = menu.Items.Add("New branch...");
            create.SvgIcon = Icons.Plus;
            create.BeginGroup = true;
            create.Click += (s, a) => CreateBranchCommand();

            menu.Show(branchButton, branchButton.PointToScreen(new Point(0, branchButton.Height + 2)));
        }

        private static string Track(RefInfo branch)
        {
            if (branch.Ahead == 0 && branch.Behind == 0) return string.Empty;
            var text = "   ";
            if (branch.Ahead > 0) text += "↑" + branch.Ahead;
            if (branch.Behind > 0) text += "↓" + branch.Behind;
            return text;
        }

        private async void fetchButton_Click(object sender, EventArgs e) => await FetchCommandAsync();

        public async Task FetchCommandAsync() => await ExecuteAsync("Fetching...", async () =>
        {
            var result = await WithCredentialRetryAsync("Fetch",
                () => _repository.FetchAsync(CreateProgress(), CancellationToken.None));
            ReportResult(result, "Fetch", "Fetch finished.");
            if (result.Succeeded) _lastFetch = DateTime.Now;
        });

        private async void pullButton_Click(object sender, EventArgs e) => await PullAsync(PullMode.Default);
        private async void pullDefaultItem_Click(object sender, EventArgs e) => await PullAsync(PullMode.Default);
        private async void pullFastForwardItem_Click(object sender, EventArgs e) => await PullAsync(PullMode.FastForwardOnly);
        private async void pullRebaseItem_Click(object sender, EventArgs e) => await PullAsync(PullMode.Rebase);

        public async Task PullCommandAsync() => await PullAsync(PullMode.Default);

        private async Task PullAsync(PullMode mode)
        {
            if (!_status.IsDetached && _status.Upstream == null && !await TrySetUpstreamAsync("Pull")) return;
            await ExecuteAsync("Pulling...", async () =>
            {
                var result = await WithCredentialRetryAsync("Pull",
                    () => _repository.PullAsync(mode, CreateProgress(), CancellationToken.None));
                ReportResult(result, "Pull", "Pull finished.");
                if (result.Succeeded) _lastFetch = DateTime.Now;
            });
        }

        private async void pushButton_Click(object sender, EventArgs e) => await PushAsync(false, false);
        private async void pushDefaultItem_Click(object sender, EventArgs e) => await PushAsync(false, false);
        private async void pushUpstreamItem_Click(object sender, EventArgs e) => await PushAsync(true, false);

        public async Task PushCommandAsync() => await PushAsync(false, false);

        private async void pushForceItem_Click(object sender, EventArgs e)
        {
            if (!Dialogs.Confirm(this, "Force push",
                "Force push (with lease) to " + (_status.Upstream ?? "the upstream branch") + "?\n\n" +
                "This rewrites the remote branch. It fails if someone else pushed in the meantime.", "Force push")) return;
            await PushAsync(false, true);
        }

        private async Task PushAsync(bool setUpstream, bool force)
        {
            if (_status.IsDetached)
            {
                Dialogs.Warning(this, "Push", "HEAD is detached. Check out a branch before pushing.");
                return;
            }
            bool needsUpstream = setUpstream || _status.Upstream == null;
            string remote = null;
            if (needsUpstream)
            {
                var remotes = await _repository.GetRemotesAsync();
                if (remotes.Count == 0)
                {
                    var choice = Dialogs.Show(this, "Push",
                        "This repository has no remote yet, so there is nowhere to push."
                        + Environment.NewLine + Environment.NewLine +
                        "Create it on GitHub now, or point it at a repository that already exists.",
                        "Create on GitHub...", "Add a remote...", "Cancel");
                    if (choice == DialogResult.Cancel) return;
                    remotes = choice == DialogResult.OK
                        ? await ShowCreateOnGitHubAsync("origin", false)
                        : await ShowRemoteDialogAsync();
                    if (remotes.Count == 0) return;
                }
                remote = remotes.Contains("origin") ? "origin" : remotes[0];
            }
            bool missingOnRemote = false;
            await ExecuteAsync("Pushing...", async () =>
            {
                var result = await WithCredentialRetryAsync("Push",
                    () => _repository.PushAsync(remote, _status.Branch, needsUpstream, force, CreateProgress(), CancellationToken.None));
                missingOnRemote = !result.Succeeded && LooksLikeMissingRepository(result.Message);
                if (!missingOnRemote) ReportResult(result, "Push", "Push finished.");
            });

            if (missingOnRemote) await OfferToCreateMissingRepositoryAsync(remote);
        }

        /// <summary>
        /// A branch with no upstream cannot pull. Offers to point it at the matching remote branch,
        /// or to publish it, instead of passing git's "set-upstream" advice on to the user.
        /// </summary>
        private async Task<bool> TrySetUpstreamAsync(string title)
        {
            var branch = _status.Branch;
            if (branch == null) return true;

            var remotes = await _repository.GetRemotesAsync();
            if (remotes.Count == 0)
            {
                if (!Dialogs.Confirm(this, title,
                    "This repository has no remote yet, so there is nothing to pull from." + Environment.NewLine + Environment.NewLine + "Add one now?",
                    "Add a remote...")) return false;
                remotes = await ShowRemoteDialogAsync();
                if (remotes.Count == 0) return false;
            }

            var remote = remotes.Contains("origin") ? "origin" : remotes[0];
            var tracking = remote + "/" + branch;
            bool onRemote = _refs.Any(r => r.Kind == RefKind.RemoteBranch && string.Equals(r.Name, tracking, StringComparison.Ordinal));

            if (!onRemote)
            {
                if (!Dialogs.Confirm(this, title,
                    "\"" + branch + "\" is not on " + remote + " yet, so there is nothing to pull." + Environment.NewLine + Environment.NewLine + "Publish it now? That pushes the branch and sets it to track " + tracking + ".",
                    "Publish branch")) return false;
                await PushAsync(true, false);
                return false;
            }

            if (!Dialogs.Confirm(this, title,
                "\"" + branch + "\" does not track a remote branch yet." + Environment.NewLine + Environment.NewLine + "Track " + tracking + " and continue?",
                "Track and continue")) return false;

            var result = await _repository.SetUpstreamAsync(branch, remote, branch);
            if (!result.Succeeded)
            {
                Dialogs.Error(this, title, result.Message);
                return false;
            }
            await RefreshAsync();
            return true;
        }

        /// <summary>git says this when the remote URL points at a repository that is not there.</summary>
        private static bool LooksLikeMissingRepository(string message)
        {
            if (string.IsNullOrEmpty(message)) return false;
            return message.IndexOf("Repository not found", StringComparison.OrdinalIgnoreCase) >= 0
                || message.IndexOf("does not appear to be a git repository", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private async Task OfferToCreateMissingRepositoryAsync(string remote)
        {
            var url = await _repository.GetRemoteUrlAsync(remote ?? "origin");
            bool github = (url ?? string.Empty).IndexOf("github.com", StringComparison.OrdinalIgnoreCase) >= 0;

            var choice = Dialogs.Show(this, "Push",
                (url ?? "The remote") + " does not exist, or the account signed in cannot see it."
                + Environment.NewLine + Environment.NewLine +
                (github
                    ? "Create it on GitHub now, or point the remote somewhere else."
                    : "Create a repository on GitHub for this project, or point the remote somewhere else."),
                "Create on GitHub...", "Edit the remote...", "Cancel");
            if (choice == DialogResult.Cancel) return;

            if (choice == DialogResult.OK) await ShowCreateOnGitHubAsync(remote ?? "origin", true);
            else await ShowRemoteDialogAsync();
        }

        private void createOnGitHubItem_Click(object sender, EventArgs e) => CreateOnGitHubCommand();

        private async void CreateOnGitHubCommand() => await ShowCreateOnGitHubAsync("origin", true);

        /// <summary>
        /// Creates the repository on GitHub, points a remote at it and (when asked) publishes the
        /// current branch. Returns the remotes that exist afterwards.
        /// </summary>
        private async Task<List<string>> ShowCreateOnGitHubAsync(string remote, bool pushWhenAsked)
        {
            if (_repository == null) return new List<string>();
            bool push = false;
            GitHubRepository created = null;
            using (var dialog = new CreateRepositoryDialog(_repository, _repository.Name, remote))
            {
                if (dialog.ShowDialog(FindForm()) == DialogResult.OK)
                {
                    created = dialog.CreatedRepository;
                    push = dialog.PushRequested;
                }
            }
            if (created == null) return await _repository.GetRemotesAsync();

            statusMessageLabel.Text = "Created " + created.FullName + ".";
            await RefreshAsync();
            if (push && pushWhenAsked && !_status.IsDetached) await PushAsync(true, false);
            return await _repository.GetRemotesAsync();
        }

        private void remotesItem_Click(object sender, EventArgs e) => ShowRemotesCommand();

        private async void ShowRemotesCommand() => await ShowRemoteDialogAsync();

        /// <summary>Opens the remote editor and reports the remotes that exist afterwards.</summary>
        private async Task<List<string>> ShowRemoteDialogAsync()
        {
            if (_repository == null) return new List<string>();
            bool changed;
            using (var dialog = new RemoteDialog(_repository))
            {
                dialog.ShowDialog(FindForm());
                changed = dialog.Changed;
            }
            if (changed) await RefreshAsync();
            return await _repository.GetRemotesAsync();
        }

        private void newBranchButton_Click(object sender, EventArgs e) => CreateBranchCommand();

        public async void CreateBranchCommand()
        {
            var startPoint = historyList.SelectedRow?.Commit?.Sha;
            var startLabel = startPoint != null
                ? Short(startPoint) + "  " + historyList.SelectedRow.Commit.Subject
                : (_status.Branch ?? "HEAD");
            using (var dialog = new NewBranchDialog())
            {
                dialog.StartPointText = startLabel;
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                var name = dialog.BranchName;
                bool checkout = dialog.CheckoutAfterCreate;
                await ExecuteAsync("Creating branch " + name + "...", () => _repository.CreateBranchAsync(name, startPoint, checkout));
            }
        }

        private async void stashButton_Click(object sender, EventArgs e) => await StashAsync(false);
        private async void stashPushItem_Click(object sender, EventArgs e) => await StashAsync(false);
        private async void stashPushUntrackedItem_Click(object sender, EventArgs e) => await StashAsync(true);

        private async Task StashAsync(bool includeUntracked)
        {
            if (!_status.HasChanges)
            {
                Dialogs.Information(this, "Stash", "There is nothing to stash.");
                return;
            }
            using (var dialog = new TextInputDialog())
            {
                dialog.Caption = "Stash changes";
                dialog.Prompt = "Message (optional)";
                dialog.AllowEmpty = true;
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                var message = dialog.Value;
                await ExecuteAsync("Stashing...", async () =>
                {
                    var result = await _repository.StashAsync(message, includeUntracked);
                    ReportResult(result, "Stash", "Changes stashed.");
                });
            }
        }

        private async void stashPopItem_Click(object sender, EventArgs e)
        {
            if (_stashes.Count == 0) return;
            await ExecuteAsync("Popping stash...", async () =>
            {
                var result = await _repository.StashPopAsync(0);
                ReportResult(result, "Pop stash", "Stash popped.");
            });
        }

        private async void stashApplyItem_Click(object sender, EventArgs e)
        {
            if (_stashes.Count == 0) return;
            await ExecuteAsync("Applying stash...", async () =>
            {
                var result = await _repository.StashApplyAsync(0);
                ReportResult(result, "Apply stash", "Stash applied.");
            });
        }

        private async void stashDropItem_Click(object sender, EventArgs e)
        {
            if (_stashes.Count == 0) return;
            if (!Dialogs.Confirm(this, "Drop stash", "Drop " + _stashes[0].Selector + " (" + _stashes[0].Message + ")?", "Drop")) return;
            await ExecuteAsync("Dropping stash...", async () =>
            {
                var result = await _repository.StashDropAsync(0);
                ReportResult(result, "Drop stash", "Stash dropped.");
            });
        }

        private void filterBox_TextChanged(object sender, EventArgs e)
        {
            historyList.Filter = filterBox.Text.Trim();
            historyCountLabel.Text = FormatHistoryCount();
        }

        private string FormatHistoryCount()
        {
            int loaded = historyList.LoadedCount;
            var text = loaded + (loaded == 1 ? " commit" : " commits");
            if (filterBox.Text.Trim().Length > 0) text += " · filtered";
            return text;
        }

        private void terminalButton_Click(object sender, EventArgs e) => OpenTerminal();

        private void logButton_Click(object sender, EventArgs e) => ToggleOutputPanel();

        private void settingsButton_Click(object sender, EventArgs e) => SettingsRequested?.Invoke(this, EventArgs.Empty);

        /// <summary>Raised when the command bar's settings button is pressed.</summary>
        public event EventHandler SettingsRequested;

        private void scopeButton_Click(object sender, EventArgs e)
        {
            _dynamicMenu?.Dispose();
            var menu = new ModernContextMenu();
            _dynamicMenu = menu;
            var all = menu.Items.Add("All branches");
            all.Checkable = true;
            all.Checked = _allBranches;
            all.Click += async (s, a) => { _allBranches = true; await RefreshAsync(); };
            var current = menu.Items.Add("This branch only");
            current.Checkable = true;
            current.Checked = !_allBranches;
            current.Click += async (s, a) => { _allBranches = false; await RefreshAsync(); };
            menu.Show(scopeButton, scopeButton.PointToScreen(new Point(0, scopeButton.Height + 2)));
        }

        public void ToggleOutputPanel()
        {
            outputPanel.Visible = !outputPanel.Visible;
            if (outputPanel.Visible) outputBox.Text = _outputBuffer.ToString();
            AppSettings.Current.ShowOutputPanel = outputPanel.Visible;
        }

        public void OpenTerminal()
        {
            if (_repository == null) return;
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Environment.GetEnvironmentVariable("ComSpec") ?? "cmd.exe",
                    WorkingDirectory = _repository.WorkingDirectory,
                    UseShellExecute = true,
                });
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, "Terminal", ex.Message);
            }
        }

        public void OpenInExplorer()
        {
            if (_repository == null) return;
            try { Process.Start("explorer.exe", "\"" + _repository.WorkingDirectory + "\""); }
            catch (Exception ex) { Dialogs.Error(this, "Show in Explorer", ex.Message); }
        }

        // ------------------------------------------------------------------ history

        private async void historyList_SelectionChanged(object sender, EventArgs e) => await UpdateDetailAsync();

        private async void commitFilesList_SelectionChanged(object sender, EventArgs e) => await ShowDiffForSelectionAsync();

        private void diffLayoutToggle_SelectedIndexChanged(object sender, EventArgs e)
        {
            diffView.ViewLayout = diffLayoutToggle.SelectedIndex == 1 ? DiffLayout.Split : DiffLayout.Unified;
            AppSettings.Current.DiffLayout = diffView.ViewLayout;
        }

        private void copyShaButton_Click(object sender, EventArgs e)
        {
            var commit = historyList.SelectedRow?.Commit;
            if (commit != null) CopyToClipboard(commit.Sha);
        }

        private async void revertButton_Click(object sender, EventArgs e)
        {
            var commit = historyList.SelectedRow?.Commit;
            if (commit == null) return;
            await RevertAsync(commit);
        }

        private async void historyList_RowActivated(object sender, HistoryRowEventArgs e)
        {
            if (e.Row?.Commit == null) return;
            var commit = e.Row.Commit;
            if (!Dialogs.Confirm(this, "Check out commit",
                "Check out " + commit.ShortSha + " (" + commit.Subject + ")?\n\nThis leaves HEAD detached.", "Check out")) return;
            await ExecuteAsync("Checking out " + commit.ShortSha + "...", () => _repository.CheckoutDetachedAsync(commit.Sha));
        }

        private async void historyList_RefActivated(object sender, HistoryRowEventArgs e)
        {
            var reference = e.Reference;
            if (reference == null) return;
            if (reference.Kind == RefKind.LocalBranch)
            {
                if (reference.Name == _status.Branch) return;
                await ExecuteAsync("Switching to " + reference.Name + "...", () => _repository.CheckoutAsync(reference.Name));
            }
            else if (reference.Kind == RefKind.RemoteBranch)
            {
                await ExecuteAsync("Switching to " + reference.ShortName + "...", () => _repository.CheckoutRemoteBranchAsync(reference));
            }
        }

        private void historyList_RowContextMenuRequested(object sender, HistoryRowEventArgs e)
        {
            if (e.Row == null || _repository == null) return;
            ShowHistoryMenu(e.Row, e.Reference, e.ScreenLocation);
        }

        private void ShowHistoryMenu(GraphRow row, RefInfo reference, Point screenLocation)
        {
            _dynamicMenu?.Dispose();
            var menu = new ModernContextMenu();
            _dynamicMenu = menu;

            var current = _status.Branch;
            var selectedCommits = historyList.SelectedCommits.ToList();

            if (row.IsWorkInProgress)
            {
                var stash = menu.Items.Add("Stash all changes...");
                stash.SvgIcon = Icons.Stash;
                stash.Enabled = _status.HasChanges;
                stash.Click += async (s, e) => await StashAsync(true);

                var discardAll = menu.Items.Add("Discard all changes...");
                discardAll.SvgIcon = Icons.Discard;
                discardAll.Enabled = _status.HasChanges;
                discardAll.Click += async (s, e) => await DiscardAsync(_status.Unstaged.ToList());
                menu.Show(historyList, screenLocation);
                return;
            }

            var commit = row.Commit;
            if (reference != null) AddRefItems(menu, reference);

            var checkoutItem = menu.Items.Add("Check out " + commit.ShortSha + " (detached)");
            checkoutItem.SvgIcon = Icons.Checkout;
            checkoutItem.BeginGroup = reference != null;
            checkoutItem.Click += async (s, e) => await ExecuteAsync("Checking out...", () => _repository.CheckoutDetachedAsync(commit.Sha));

            var branchHere = menu.Items.Add("Create branch here...");
            branchHere.SvgIcon = Icons.Branch;
            branchHere.Click += async (s, e) => await CreateBranchAtAsync(commit);

            var tagHere = menu.Items.Add("Create tag here...");
            tagHere.SvgIcon = Icons.Tag;
            tagHere.Click += async (s, e) => await CreateTagAtAsync(commit);

            var cherryPick = menu.Items.Add(selectedCommits.Count > 1
                ? "Cherry-pick " + selectedCommits.Count + " commits into " + (current ?? "HEAD")
                : "Cherry-pick into " + (current ?? "HEAD"));
            cherryPick.SvgIcon = Icons.CherryPick;
            cherryPick.BeginGroup = true;
            cherryPick.Enabled = !_status.IsDetached;
            cherryPick.Click += async (s, e) => await CherryPickAsync(selectedCommits.Count > 1 ? selectedCommits : new List<CommitInfo> { commit });

            var revert = menu.Items.Add("Revert commit");
            revert.SvgIcon = Icons.Revert;
            revert.Click += async (s, e) => await RevertAsync(commit);

            foreach (var branchRef in commit.Refs.Where(r => r.Kind == RefKind.LocalBranch || r.Kind == RefKind.RemoteBranch))
            {
                if (current != null && branchRef.Kind == RefKind.LocalBranch && branchRef.Name == current) continue;
                var captured = branchRef;
                var merge = menu.Items.Add("Merge " + captured.Name + " into " + (current ?? "HEAD"));
                merge.SvgIcon = Icons.Merge;
                merge.Enabled = !_status.IsDetached;
                merge.Click += async (s, e) => await MergeAsync(captured.Name);

                var rebase = menu.Items.Add("Rebase " + (current ?? "HEAD") + " onto " + captured.Name);
                rebase.SvgIcon = Icons.Branch;
                rebase.Enabled = !_status.IsDetached;
                rebase.Click += async (s, e) => await RebaseAsync(captured.Name);
            }

            var reset = menu.Items.Add("Reset " + (current ?? "HEAD") + " to here");
            reset.SvgIcon = Icons.Reset;
            reset.BeginGroup = true;
            reset.SubItems.Add("Soft (keep index and working tree)").Click += async (s, e) => await ResetAsync(commit, ResetMode.Soft);
            reset.SubItems.Add("Mixed (keep working tree)").Click += async (s, e) => await ResetAsync(commit, ResetMode.Mixed);
            reset.SubItems.Add("Hard (discard all changes)").Click += async (s, e) => await ResetAsync(commit, ResetMode.Hard);

            var drop = menu.Items.Add(selectedCommits.Count > 1 ? "Delete " + selectedCommits.Count + " commits..." : "Delete commit...");
            drop.SvgIcon = Icons.Trash;
            drop.Enabled = !commit.IsRoot;
            drop.Click += async (s, e) => await DropCommitsAsync(selectedCommits.Count > 1 ? selectedCommits : new List<CommitInfo> { commit });

            var copySha = menu.Items.Add("Copy sha");
            copySha.SvgIcon = Icons.Copy;
            copySha.BeginGroup = true;
            copySha.Click += (s, e) => CopyToClipboard(commit.Sha);

            var copyMessage = menu.Items.Add("Copy message");
            copyMessage.SvgIcon = Icons.Copy;
            copyMessage.Click += (s, e) => CopyToClipboard(commit.FullMessage);

            var openWeb = menu.Items.Add("Open commit on the web");
            openWeb.SvgIcon = Icons.Github;
            openWeb.Click += async (s, e) => await OpenCommitOnWebAsync(commit);

            menu.Show(historyList, screenLocation);
        }

        private void AddRefItems(ModernContextMenu menu, RefInfo reference)
        {
            if (reference.Kind == RefKind.LocalBranch)
            {
                if (reference.Name != _status.Branch)
                {
                    var checkout = menu.Items.Add("Check out " + reference.Name);
                    checkout.SvgIcon = Icons.Checkout;
                    checkout.Click += async (s, e) => await ExecuteAsync("Switching...", () => _repository.CheckoutAsync(reference.Name));
                }
                var rename = menu.Items.Add("Rename " + reference.Name + "...");
                rename.SvgIcon = Icons.Edit;
                rename.Click += async (s, e) => await RenameBranchAsync(reference);

                var push = menu.Items.Add("Push " + reference.Name);
                push.SvgIcon = Icons.Push;
                push.Click += async (s, e) => await PushSpecificBranchAsync(reference);

                var delete = menu.Items.Add("Delete " + reference.Name + "...");
                delete.SvgIcon = Icons.Trash;
                delete.Enabled = reference.Name != _status.Branch;
                delete.Click += async (s, e) => await DeleteBranchAsync(reference);
            }
            else if (reference.Kind == RefKind.RemoteBranch)
            {
                var checkout = menu.Items.Add("Check out " + reference.ShortName);
                checkout.SvgIcon = Icons.Checkout;
                checkout.Click += async (s, e) => await ExecuteAsync("Switching...", () => _repository.CheckoutRemoteBranchAsync(reference));

                var delete = menu.Items.Add("Delete " + reference.Name + " on the remote...");
                delete.SvgIcon = Icons.Trash;
                delete.Click += async (s, e) => await DeleteRemoteBranchAsync(reference);
            }
            else if (reference.Kind == RefKind.Tag)
            {
                var delete = menu.Items.Add("Delete tag " + reference.Name + "...");
                delete.SvgIcon = Icons.Trash;
                delete.Click += async (s, e) => await DeleteTagAsync(reference);
            }
        }

        // ------------------------------------------------------------------ history operations

        private async Task CreateBranchAtAsync(CommitInfo commit)
        {
            using (var dialog = new NewBranchDialog())
            {
                dialog.StartPointText = commit.ShortSha + "  " + commit.Subject;
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                var name = dialog.BranchName;
                bool checkout = dialog.CheckoutAfterCreate;
                await ExecuteAsync("Creating branch...", () => _repository.CreateBranchAsync(name, commit.Sha, checkout));
            }
        }

        private async Task CreateTagAtAsync(CommitInfo commit)
        {
            using (var dialog = new TextInputDialog())
            {
                dialog.Caption = "Create tag";
                dialog.Prompt = "Tag name for " + commit.ShortSha;
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                var name = dialog.Value;
                await ExecuteAsync("Creating tag...", () => _repository.CreateTagAsync(name, commit.Sha, null));
            }
        }

        private async Task CherryPickAsync(List<CommitInfo> commits)
        {
            var ordered = Enumerable.Reverse(commits).ToList();
            await ExecuteAsync("Cherry-picking...", async () =>
            {
                foreach (var commit in ordered)
                {
                    var result = await _repository.CherryPickAsync(commit);
                    if (!result.Succeeded)
                    {
                        ReportResult(result, "Cherry-pick", null);
                        return;
                    }
                }
                statusMessageLabel.Text = "Cherry-picked " + ordered.Count + " commit(s).";
            });
        }

        private async Task RevertAsync(CommitInfo commit)
        {
            await ExecuteAsync("Reverting...", async () =>
            {
                var result = await _repository.RevertAsync(commit);
                ReportResult(result, "Revert", "Revert committed.");
            });
        }

        private async Task MergeAsync(string branch)
        {
            await ExecuteAsync("Merging " + branch + "...", async () =>
            {
                var result = await _repository.MergeAsync(branch);
                ReportResult(result, "Merge", "Merged " + branch + ".");
            });
        }

        private async Task RebaseAsync(string onto)
        {
            if (!Dialogs.Confirm(this, "Rebase",
                "Rebase " + (_status.Branch ?? "HEAD") + " onto " + onto + "?\n\nThis rewrites the commits of the current branch.", "Rebase")) return;
            await ExecuteAsync("Rebasing...", async () =>
            {
                var result = await _repository.RebaseAsync(onto);
                ReportResult(result, "Rebase", "Rebase finished.");
            });
        }

        private async Task ResetAsync(CommitInfo commit, ResetMode mode)
        {
            if (mode == ResetMode.Hard && !Dialogs.Confirm(this, "Hard reset",
                "Reset to " + commit.ShortSha + " and discard every change in the working tree?", "Discard and reset")) return;
            await ExecuteAsync("Resetting...", () => _repository.ResetAsync(commit.Sha, mode));
        }

        private async Task DropCommitsAsync(List<CommitInfo> commits)
        {
            var ordered = commits.OrderBy(c => _layout.RowsBySha[c.Sha].Index).ToList();
            var message = ordered.Count == 1
                ? "Delete commit " + ordered[0].ShortSha + " (" + ordered[0].Subject + ")?"
                : "Delete these " + ordered.Count + " commits?\n\n" + string.Join("\n", ordered.Take(8).Select(c => c.ShortSha + "  " + c.Subject));
            message += "\n\nCommits after it are replayed onto its parent, which rewrites their hashes.";
            if (!Dialogs.Confirm(this, "Delete commit", message, "Delete")) return;

            await ExecuteAsync("Deleting commit(s)...", async () =>
            {
                foreach (var commit in ordered)
                {
                    var result = await _repository.DropCommitAsync(commit, false);
                    if (!result.Succeeded)
                    {
                        ReportResult(result, "Delete commit", null);
                        return;
                    }
                }
                statusMessageLabel.Text = "Deleted " + ordered.Count + " commit(s).";
            });
        }

        private async Task RenameBranchAsync(RefInfo branch)
        {
            using (var dialog = new TextInputDialog())
            {
                dialog.Caption = "Rename branch";
                dialog.Prompt = "New name for " + branch.Name;
                dialog.Value = branch.Name;
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                var name = dialog.Value;
                await ExecuteAsync("Renaming branch...", () => _repository.RenameBranchAsync(branch.Name, name));
            }
        }

        private async Task DeleteBranchAsync(RefInfo branch)
        {
            if (!Dialogs.Confirm(this, "Delete branch", "Delete local branch " + branch.Name + "?", "Delete")) return;
            await ExecuteAsync("Deleting branch...", async () =>
            {
                try
                {
                    await _repository.DeleteBranchAsync(branch.Name, false);
                }
                catch (GitException ex)
                {
                    if (ex.Result != null && ex.Result.Message.IndexOf("not fully merged", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (Dialogs.Confirm(this, "Delete branch",
                            branch.Name + " is not fully merged. Delete it anyway and lose its commits?", "Force delete"))
                        {
                            await _repository.DeleteBranchAsync(branch.Name, true);
                        }
                        return;
                    }
                    throw;
                }
            });
        }

        private async Task DeleteRemoteBranchAsync(RefInfo branch)
        {
            if (branch.Remote == null) return;
            if (!Dialogs.Confirm(this, "Delete remote branch",
                "Delete " + branch.ShortName + " on " + branch.Remote + "?\n\nThis affects everyone using that remote.", "Delete on remote")) return;
            await ExecuteAsync("Deleting remote branch...", async () =>
            {
                var result = await WithCredentialRetryAsync("Delete remote branch",
                    () => _repository.DeleteRemoteBranchAsync(branch.Remote, branch.ShortName));
                ReportResult(result, "Delete remote branch", "Remote branch deleted.");
            });
        }

        private async Task DeleteTagAsync(RefInfo tag)
        {
            if (!Dialogs.Confirm(this, "Delete tag", "Delete tag " + tag.Name + "?", "Delete")) return;
            await ExecuteAsync("Deleting tag...", () => _repository.DeleteTagAsync(tag.Name));
        }

        private async Task PushSpecificBranchAsync(RefInfo branch)
        {
            var remotes = await _repository.GetRemotesAsync();
            if (remotes.Count == 0)
            {
                Dialogs.Warning(this, "Push", "This repository has no remote.");
                return;
            }
            var remote = remotes.Contains("origin") ? "origin" : remotes[0];
            await ExecuteAsync("Pushing " + branch.Name + "...", async () =>
            {
                var result = await WithCredentialRetryAsync("Push",
                    () => _repository.PushBranchAsync(remote, branch.Name, CreateProgress(), CancellationToken.None));
                ReportResult(result, "Push", "Pushed " + branch.Name + ".");
            });
        }

        private async Task OpenCommitOnWebAsync(CommitInfo commit)
        {
            try
            {
                var remotes = await _repository.GetRemotesAsync();
                if (remotes.Count == 0) { Dialogs.Information(this, "Open on the web", "This repository has no remote."); return; }
                var url = GitRepository.ToWebUrl(await _repository.GetRemoteUrlAsync(remotes.Contains("origin") ? "origin" : remotes[0]));
                if (url == null) { Dialogs.Information(this, "Open on the web", "The remote URL is not a web address."); return; }
                Process.Start(url.TrimEnd('/') + "/commit/" + commit.Sha);
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, "Open on the web", ex.Message);
            }
        }

        private static void CopyToClipboard(string text)
        {
            try { if (!string.IsNullOrEmpty(text)) Clipboard.SetText(text); } catch { }
        }

        // ------------------------------------------------------------------ changes pane

        private async void changesList_SelectionChanged(object sender, EventArgs e)
        {
            var change = changesList.SelectedChange;
            if (change == null) return;
            historyList.SelectWorkInProgress();
            await UpdateDetailAsync();
            if (!commitFilesList.SelectPath(change.Path)) await ShowDiffForSelectionAsync();
        }

        private async void changesList_CheckedChanged(object sender, FileChangeEventArgs e)
        {
            if (e.Staged) await UnstagePathsAsync(new[] { e.Change.Path });
            else await StagePathsAsync(new[] { e.Change.Path });
        }

        private async void changesList_FileActivated(object sender, FileChangeEventArgs e)
        {
            if (e.Staged) await UnstagePathsAsync(new[] { e.Change.Path });
            else await StagePathsAsync(new[] { e.Change.Path });
        }

        private void changesList_RowRightClick(object sender, RowMouseEventArgs e)
        {
            var change = changesList.SelectedChange;
            if (change == null || _repository == null) return;
            var selected = changesList.SelectedChanges.ToList();
            if (selected.Count == 0) selected.Add(change);

            _dynamicMenu?.Dispose();
            var menu = new ModernContextMenu();
            _dynamicMenu = menu;

            if (change.Kind == FileChangeKind.Conflicted)
            {
                var ours = menu.Items.Add("Take ours");
                ours.Click += async (s, a) => await ResolveConflictAsync(change, true);
                var theirs = menu.Items.Add("Take theirs");
                theirs.Click += async (s, a) => await ResolveConflictAsync(change, false);
                var open = menu.Items.Add("Open in editor");
                open.BeginGroup = true;
                open.Click += (s, a) => OpenPath(FullPath(change));
                menu.Show(changesList, changesList.PointToScreen(e.Location));
                return;
            }

            if (selected.Any(c => !c.Staged))
            {
                var stage = menu.Items.Add(selected.Count > 1 ? "Stage " + selected.Count + " files" : "Stage file");
                stage.SvgIcon = Icons.Plus;
                stage.Click += async (s, a) => await StagePathsAsync(selected.Where(c => !c.Staged).Select(c => c.Path));
            }
            if (selected.Any(c => c.Staged))
            {
                var unstage = menu.Items.Add(selected.Count > 1 ? "Unstage " + selected.Count + " files" : "Unstage file");
                unstage.SvgIcon = Icons.Back;
                unstage.Click += async (s, a) => await UnstagePathsAsync(selected.Where(c => c.Staged).Select(c => c.Path));
            }

            var discard = menu.Items.Add(selected.Count > 1 ? "Discard " + selected.Count + " files..." : "Discard changes...");
            discard.SvgIcon = Icons.Discard;
            discard.BeginGroup = true;
            discard.Click += async (s, a) => await DiscardAsync(selected.Where(c => !c.Staged).DefaultIfEmpty(change).ToList());

            var openFile = menu.Items.Add("Open file");
            openFile.BeginGroup = true;
            openFile.Click += (s, a) => OpenPath(FullPath(change));

            var reveal = menu.Items.Add("Show in Explorer");
            reveal.SvgIcon = Icons.Folder;
            reveal.Click += (s, a) => RevealPath(FullPath(change));

            var copyPath = menu.Items.Add("Copy path");
            copyPath.SvgIcon = Icons.Copy;
            copyPath.Click += (s, a) => CopyToClipboard(change.Path);

            AddIgnoreMenu(menu, change, selected);

            menu.Show(changesList, changesList.PointToScreen(e.Location));
        }

        /// <summary>
        /// The "Add to .gitignore" submenu: the file itself, every file sharing its extension, and
        /// each of its folders walking up to the repository root.
        /// </summary>
        private void AddIgnoreMenu(ModernContextMenu menu, FileChange change, List<FileChange> selected)
        {
            var ignore = menu.Items.Add("Add to .gitignore");
            ignore.BeginGroup = true;

            // git only consults .gitignore for files it is not already tracking, so a rule for a
            // tracked file would quietly do nothing.
            var untracked = selected.Where(c => c.Kind == FileChangeKind.Untracked).ToList();
            if (untracked.Count > 1)
            {
                var all = ignore.SubItems.Add("These " + untracked.Count + " files");
                all.Click += async (s, a) => await AddToIgnoreAsync(untracked.Select(c => GitIgnoreFile.FileRule(c.Path)).ToList());
            }

            var file = ignore.SubItems.Add(GitIgnoreFile.FileRule(change.Path));
            file.Enabled = change.Kind == FileChangeKind.Untracked;
            file.Click += async (s, a) => await AddToIgnoreAsync(new[] { GitIgnoreFile.FileRule(change.Path) });

            var extensionRule = GitIgnoreFile.ExtensionRule(change.Path);
            if (extensionRule != null)
            {
                var extension = ignore.SubItems.Add(extensionRule);
                extension.Click += async (s, a) => await AddToIgnoreAsync(new[] { extensionRule });
            }

            bool first = true;
            foreach (var directory in GitIgnoreFile.AncestorDirectories(change.Path))
            {
                var rule = GitIgnoreFile.DirectoryRule(directory);
                var item = ignore.SubItems.Add(rule);
                item.BeginGroup = first;
                first = false;
                item.Click += async (s, a) => await AddToIgnoreAsync(new[] { rule });
            }

            var edit = ignore.SubItems.Add("Edit .gitignore...");
            edit.BeginGroup = true;
            edit.Click += (s, a) => ShowGitIgnoreDialog();
        }

        private async Task AddToIgnoreAsync(IEnumerable<string> rules)
        {
            if (_repository == null) return;
            var list = rules.ToList();
            try
            {
                int written = GitIgnoreFile.Append(_repository.WorkingDirectory, list);
                if (written == 0)
                {
                    statusMessageLabel.Text = list.Count == 1 ? list[0] + " is already in .gitignore" : "Already in .gitignore";
                    return;
                }
                statusMessageLabel.Text = written == 1 ? "Added " + list[0] + " to .gitignore" : "Added " + written + " rules to .gitignore";
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, "Add to .gitignore", ex.Message);
                return;
            }
            await RefreshAsync();
        }

        private void editIgnoreItem_Click(object sender, EventArgs e) => ShowGitIgnoreDialog();

        private async void ShowGitIgnoreDialog()
        {
            if (_repository == null) return;
            bool saved;
            using (var dialog = new GitIgnoreDialog(_repository.WorkingDirectory))
            {
                dialog.ShowDialog(FindForm());
                saved = dialog.Saved;
            }
            if (saved) await RefreshAsync();
        }

        private string FullPath(FileChange change) =>
            Path.Combine(_repository.WorkingDirectory, change.Path.Replace('/', Path.DirectorySeparatorChar));

        private async Task ResolveConflictAsync(FileChange change, bool ours)
        {
            await ExecuteAsync("Resolving " + change.FileName + "...", () => _repository.ResolveConflictAsync(change.Path, ours));
        }

        private async Task StagePathsAsync(IEnumerable<string> paths)
        {
            var list = paths.ToList();
            await ExecuteAsync("Staging...", () => _repository.StageAsync(list));
        }

        private async Task UnstagePathsAsync(IEnumerable<string> paths)
        {
            var list = paths.ToList();
            await ExecuteAsync("Unstaging...", () => _repository.UnstageAsync(list));
        }

        private async Task DiscardAsync(List<FileChange> changes)
        {
            if (changes.Count == 0) return;
            var message = changes.Count == 1
                ? "Discard changes to " + changes[0].Path + "?"
                : "Discard changes to these " + changes.Count + " files?";
            if (!Dialogs.Confirm(this, "Discard changes", message + "\n\nThis cannot be undone.", "Discard")) return;
            await ExecuteAsync("Discarding...", () => _repository.DiscardAsync(changes));
        }

        private async void stageAllButton_Click(object sender, EventArgs e) =>
            await ExecuteAsync("Staging all changes...", () => _repository.StageAllAsync());

        private async void unstageAllItem_Click(object sender, EventArgs e) =>
            await ExecuteAsync("Unstaging all changes...", () => _repository.UnstageAllAsync());

        private async void discardAllItem_Click(object sender, EventArgs e) =>
            await DiscardAsync(_status.Unstaged.ToList());

        private async void refreshItem_Click(object sender, EventArgs e) => await RefreshAsync();

        private void OpenPath(string path)
        {
            try
            {
                if (File.Exists(path)) Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, "Open file", ex.Message);
            }
        }

        private void RevealPath(string path)
        {
            try
            {
                if (File.Exists(path)) Process.Start("explorer.exe", "/select,\"" + path + "\"");
                else Process.Start("explorer.exe", "\"" + _repository.WorkingDirectory + "\"");
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, "Show in Explorer", ex.Message);
            }
        }

        // ------------------------------------------------------------------ conflict banner

        private async void continueButton_Click(object sender, EventArgs e)
        {
            var state = _state;
            await ExecuteAsync("Continuing...", async () =>
            {
                var result = await _repository.ContinueOperationAsync(state);
                ReportResult(result, "Continue", "Operation continued.");
            });
        }

        private async void abortButton_Click(object sender, EventArgs e)
        {
            var state = _state;
            if (!Dialogs.Confirm(this, "Abort", "Abort the operation in progress and return to the previous state?", "Abort")) return;
            await ExecuteAsync("Aborting...", async () =>
            {
                var result = await _repository.AbortOperationAsync(state);
                ReportResult(result, "Abort", "Operation aborted.");
            });
        }

        // ------------------------------------------------------------------ composer

        private void summaryBox_TextChanged(object sender, EventArgs e) => UpdateComposer();

        // A Win32 cue banner never shows on a multiline edit, so the description's placeholder is
        // a label parked over the empty box.
        private void descriptionBox_TextChanged(object sender, EventArgs e)
        {
            descriptionPlaceholder.Visible = descriptionBox.Text.Length == 0;
        }

        private void descriptionPlaceholder_Click(object sender, EventArgs e) => descriptionBox.Focus();

        private async void amendSwitch_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComposer();
            if (amendSwitch.Checked && !_amendMessageLoaded && summaryBox.Text.Trim().Length == 0 && _repository != null)
            {
                var message = await _repository.GetHeadMessageAsync();
                SetMessage(message);
                _amendMessageLoaded = true;
            }
            else if (!amendSwitch.Checked && _amendMessageLoaded)
            {
                SetMessage(string.Empty);
                _amendMessageLoaded = false;
            }
        }

        private void SetMessage(string message)
        {
            message = (message ?? string.Empty).Replace("\r\n", "\n").TrimEnd('\n');
            int split = message.IndexOf('\n');
            if (split < 0)
            {
                summaryBox.Text = message;
                descriptionBox.Text = string.Empty;
            }
            else
            {
                summaryBox.Text = message.Substring(0, split).Trim();
                descriptionBox.Text = message.Substring(split + 1).TrimStart('\n').Replace("\n", Environment.NewLine);
            }
            UpdateComposer();
        }

        private void commitBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (commitButton.Enabled) commitButton_Click(commitButton, EventArgs.Empty);
            }
        }

        private async void commitButton_Click(object sender, EventArgs e) => await CommitAsync(false);

        private async void commitPushButton_Click(object sender, EventArgs e) => await CommitAsync(true);

        private async Task CommitAsync(bool thenPush)
        {
            if (_repository == null) return;
            var summary = summaryBox.Text.Trim();
            if (summary.Length == 0) return;
            var description = descriptionBox.Text.Trim();
            var message = description.Length > 0 ? summary + "\n\n" + description.Replace("\r\n", "\n") : summary;
            bool amend = amendSwitch.Checked;

            if (_status.Staged.Count == 0 && !amend)
            {
                Dialogs.Information(this, "Commit", "Stage some changes first.");
                return;
            }

            await ExecuteAsync(amend ? "Amending..." : "Committing...", async () =>
            {
                await _repository.CommitAsync(message, amend);
                summaryBox.Text = string.Empty;
                descriptionBox.Text = string.Empty;
                amendSwitch.Checked = false;
                _amendMessageLoaded = false;
                statusMessageLabel.Text = amend ? "Commit amended." : "Commit created.";
            });

            if (thenPush) await PushAsync(false, false);
        }

        private async void aiButton_Click(object sender, EventArgs e)
        {
            if (_repository == null) return;
            if (_aiCancellation != null)
            {
                _aiCancellation.Cancel();
                return;
            }
            var executable = ClaudeCommitComposer.FindExecutable(AppSettings.Current.ClaudeExecutable);
            if (executable == null)
            {
                Dialogs.Warning(this, "Claude Code",
                    "The Claude Code CLI was not found.\n\nInstall it, or set the path to claude.exe in Settings.");
                return;
            }

            if (_status.Staged.Count == 0)
            {
                Dialogs.Information(this, "Claude Code", "Stage the changes you want described first.");
                return;
            }

            var patch = await _repository.GetStagedPatchAsync();
            if (string.IsNullOrWhiteSpace(patch))
            {
                Dialogs.Information(this, "Claude Code", "The staged diff is empty.");
                return;
            }

            var files = _status.Staged.Select(c => c.StatusLetter + " " + c.Path).ToList();
            var recent = await _repository.GetRecentSubjectsAsync(8);
            _aiCancellation = new CancellationTokenSource();
            var previousText = aiButton.Text;
            aiButton.Text = "Writing...";
            aiButton.Invalidate();
            progressBar.Visible = true;
            statusMessageLabel.Text = "Asking Claude for a commit message...";
            try
            {
                var suggestion = await ClaudeCommitComposer.ComposeAsync(
                    _repository.WorkingDirectory, patch, files, recent, executable, AppSettings.Current.ClaudeModel, _aiCancellation.Token);
                summaryBox.Text = suggestion.Subject;
                descriptionBox.Text = suggestion.Body.Replace("\n", Environment.NewLine);
                statusMessageLabel.Text = "Commit message written by Claude. Review it before committing.";
                UpdateComposer();
            }
            catch (OperationCanceledException)
            {
                statusMessageLabel.Text = "Composition cancelled.";
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, "Claude Code", ex.Message);
            }
            finally
            {
                _aiCancellation?.Dispose();
                _aiCancellation = null;
                aiButton.Text = previousText;
                aiButton.Invalidate();
                progressBar.Visible = _busy || _refreshing;
            }
        }

        // ------------------------------------------------------------------ session

        public void ApplyLayoutSettings(AppSettings settings)
        {
            if (settings.ShowOutputPanel && !outputPanel.Visible) ToggleOutputPanel();
        }

        public void StoreLayoutSettings(AppSettings settings)
        {
            settings.ShowOutputPanel = outputPanel.Visible;
            settings.DiffLayout = diffView.ViewLayout;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            historyCountLabel.Text = FormatHistoryCount();
            diffLayoutToggle.Width = diffLayoutToggle.PreferredWidth;
            diffLayoutToggle.Left = diffHeader.ClientSize.Width - 14 - diffLayoutToggle.Width;
            LayoutCommandBar();
        }
    }
}
