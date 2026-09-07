using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitClient.Controls;
using GitClient.Git;
using GitClient.Services;
using GitClient.Views;
using ModernWinForms;

namespace GitClient.Forms
{
    /// <summary>Shell window: title bar, repository tabs, and the welcome screen when nothing is open.</summary>
    public partial class MainForm : ModernForm
    {
        private readonly List<RepositoryView> _views = new List<RepositoryView>();
        private RepositoryView _activeView;
        private int _tabMenuIndex = -1;
        private Icon _titleIcon;

        public MainForm()
        {
            InitializeComponent();

            var settings = AppSettings.Current;
            Theme.Mode = settings.Theme;
            Theme.Apply(skin);
            Theme.Changed += OnThemeChanged;

            if (!string.IsNullOrEmpty(settings.GitExecutable) && File.Exists(settings.GitExecutable))
            {
                GitRunner.GitExecutable = settings.GitExecutable;
            }
            ApplyTitleIcon();
        }

        // ------------------------------------------------------------------ theme

        private void OnThemeChanged(object sender, EventArgs e)
        {
            Theme.Apply(skin);
            ApplyTitleIcon();
            Invalidate(true);
        }

        /// <summary>Puts the branch glyph in the title bar, in the accent lane colour.</summary>
        private void ApplyTitleIcon()
        {
            try
            {
                var image = IconCache.Get(Icons.Branch, 16, Theme.Palette.Lane);
                if (image == null) return;
                using (var bitmap = new Bitmap(image))
                {
                    var handle = bitmap.GetHicon();
                    var previous = _titleIcon;
                    _titleIcon = Icon.FromHandle(handle);
                    Icon = _titleIcon;
                    previous?.Dispose();
                }
            }
            catch
            {
                // The title bar simply keeps the default icon if the glyph cannot be rasterised.
            }
        }

        // ------------------------------------------------------------------ lifetime

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RestoreWindowPosition();
            RefreshWelcome();

            var settings = AppSettings.Current;
            foreach (var path in settings.OpenRepositories.ToList())
            {
                if (Directory.Exists(path)) await OpenRepositoryAsync(path, false);
            }
            if (_views.Count > 0)
            {
                int index = settings.ActiveRepository == null
                    ? 0
                    : Math.Max(0, _views.FindIndex(v => string.Equals(v.RepositoryPath, settings.ActiveRepository, StringComparison.OrdinalIgnoreCase)));
                tabStrip.SetSelectedIndexQuiet(index);
                ActivateView(index);
            }
            UpdateHostVisibility();
        }

        private void RestoreWindowPosition()
        {
            var settings = AppSettings.Current;
            if (settings.WindowWidth > 400 && settings.WindowHeight > 300)
            {
                Size = new Size(settings.WindowWidth, settings.WindowHeight);
            }
            if (settings.WindowX >= 0 && settings.WindowY >= 0)
            {
                var bounds = new Rectangle(settings.WindowX, settings.WindowY, Width, Height);
                if (Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(bounds)))
                {
                    StartPosition = FormStartPosition.Manual;
                    Location = new Point(settings.WindowX, settings.WindowY);
                }
            }
            if (settings.WindowMaximized) WindowState = FormWindowState.Maximized;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var settings = AppSettings.Current;
            settings.WindowMaximized = WindowState == FormWindowState.Maximized;
            if (WindowState == FormWindowState.Normal)
            {
                settings.WindowX = Location.X;
                settings.WindowY = Location.Y;
                settings.WindowWidth = Width;
                settings.WindowHeight = Height;
            }
            settings.OpenRepositories = _views.Select(v => v.RepositoryPath).Where(p => p != null).ToList();
            settings.ActiveRepository = _activeView?.RepositoryPath;
            _activeView?.StoreLayoutSettings(settings);
            settings.Theme = Theme.Mode;
            settings.Save();
            base.OnFormClosing(e);
        }

        protected override async void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (_activeView != null) await _activeView.RefreshIfIdleAsync();
        }

        // ------------------------------------------------------------------ repositories

        private async Task<bool> OpenRepositoryAsync(string path, bool activate)
        {
            string root;
            try
            {
                root = await GitRepository.DiscoverAsync(path);
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, "Open repository", ex.Message);
                return false;
            }
            if (root == null)
            {
                Dialogs.Warning(this, "Open repository", path + "\n\nis not inside a git repository.");
                return false;
            }

            var existing = _views.FindIndex(v => string.Equals(v.RepositoryPath, root, StringComparison.OrdinalIgnoreCase));
            if (existing >= 0)
            {
                tabStrip.SetSelectedIndexQuiet(existing);
                ActivateView(existing);
                return true;
            }

            var view = new RepositoryView { Dock = DockStyle.Fill, Visible = false };
            view.TitleChanged += View_TitleChanged;
            view.SettingsRequested += settingsItem_Click;
            hostPanel.Controls.Add(view);
            _views.Add(view);

            AppSettings.Current.AddRecent(root);
            RefreshTabs();

            try
            {
                await view.LoadAsync(root);
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, "Open repository", ex.Message);
            }

            view.ApplyLayoutSettings(AppSettings.Current);
            RefreshTabs();

            if (activate)
            {
                int index = _views.Count - 1;
                tabStrip.SetSelectedIndexQuiet(index);
                ActivateView(index);
            }
            UpdateHostVisibility();
            return true;
        }

        private void View_TitleChanged(object sender, EventArgs e)
        {
            RefreshTabs();
            if (sender == _activeView) Text = _activeView.TabTitle + " - Git Client";
        }

        private void RefreshTabs()
        {
            var tabs = _views.Select(v => new RepositoryTab
            {
                Title = v.TabTitle,
                Branch = v.TabBranch,
                ToolTip = v.TabToolTip,
                Tag = v,
            });
            tabStrip.Refresh(tabs, _activeView == null ? 0 : _views.IndexOf(_activeView));
        }

        private void ActivateView(int index)
        {
            if (index < 0 || index >= _views.Count)
            {
                _activeView = null;
                Text = "Git Client";
                return;
            }
            var view = _views[index];
            foreach (var v in _views) v.Visible = ReferenceEquals(v, view);
            view.BringToFront();
            _activeView = view;
            Text = view.TabTitle + " - Git Client";
            tabStrip.SetSelectedIndexQuiet(index);
        }

        private void CloseTab(int index)
        {
            if (index < 0 || index >= _views.Count) return;
            var view = _views[index];
            tabStrip.Refresh(Enumerable.Empty<RepositoryTab>(), -1);
            _views.RemoveAt(index);
            view.TitleChanged -= View_TitleChanged;
            view.SettingsRequested -= settingsItem_Click;
            hostPanel.Controls.Remove(view);
            view.Dispose();

            int next = Math.Min(index, _views.Count - 1);
            ActivateView(next);
            RefreshTabs();
            UpdateHostVisibility();
        }

        private void UpdateHostVisibility()
        {
            bool empty = _views.Count == 0;
            welcomeView.Visible = empty;
            if (empty)
            {
                RefreshWelcome();
                welcomeView.BringToFront();
                Text = "Git Client";
            }
        }

        private void RefreshWelcome()
        {
            welcomeView.SetRecent(AppSettings.Current.RecentRepositories, ReadBranchQuickly);
        }

        /// <summary>
        /// Reads a repository's current branch straight from .git/HEAD. The welcome list would
        /// otherwise need one git process per row before the window can be shown.
        /// </summary>
        private static string ReadBranchQuickly(string path)
        {
            try
            {
                var head = Path.Combine(path, ".git", "HEAD");
                if (!File.Exists(head)) return null;
                var text = File.ReadAllText(head).Trim();
                const string prefix = "ref: refs/heads/";
                if (text.StartsWith(prefix, StringComparison.Ordinal)) return text.Substring(prefix.Length);
                return text.Length >= 7 ? text.Substring(0, 7) : null;
            }
            catch
            {
                return null;
            }
        }

        // ------------------------------------------------------------------ tab strip

        private void tabStrip_SelectedIndexChanged(object sender, EventArgs e) => ActivateView(tabStrip.SelectedIndex);

        private void tabStrip_TabCloseRequested(object sender, TabEventArgs e) => CloseTab(e.Index);

        private void tabStrip_TabContextMenuRequested(object sender, TabEventArgs e)
        {
            _tabMenuIndex = e.Index;
            tabCloseOthersItem.Enabled = _views.Count > 1;
            tabMenu.Show(tabStrip, Cursor.Position);
        }

        private void tabStrip_AddRequested(object sender, EventArgs e)
        {
            addMenu.Show(tabStrip, Cursor.Position);
        }

        private void tabCloseItem_Click(object sender, EventArgs e) => CloseTab(_tabMenuIndex);

        private void tabCloseOthersItem_Click(object sender, EventArgs e)
        {
            if (_tabMenuIndex < 0 || _tabMenuIndex >= _views.Count) return;
            var keep = _views[_tabMenuIndex];
            for (int i = _views.Count - 1; i >= 0; i--)
            {
                if (!ReferenceEquals(_views[i], keep)) CloseTab(i);
            }
        }

        private void tabCopyPathItem_Click(object sender, EventArgs e)
        {
            if (_tabMenuIndex < 0 || _tabMenuIndex >= _views.Count) return;
            try { Clipboard.SetText(_views[_tabMenuIndex].RepositoryPath ?? string.Empty); } catch { }
        }

        private void tabExplorerItem_Click(object sender, EventArgs e)
        {
            if (_tabMenuIndex < 0 || _tabMenuIndex >= _views.Count) return;
            _views[_tabMenuIndex].OpenInExplorer();
        }

        private void tabTerminalItem_Click(object sender, EventArgs e)
        {
            if (_tabMenuIndex < 0 || _tabMenuIndex >= _views.Count) return;
            _views[_tabMenuIndex].OpenTerminal();
        }

        // ------------------------------------------------------------------ commands

        private async void openRepositoryItem_Click(object sender, EventArgs e)
        {
            var recent = AppSettings.Current.RecentRepositories.FirstOrDefault(Directory.Exists);
            var start = recent != null ? Path.GetDirectoryName(recent) : null;
            var path = FolderPicker.Show(this, "Choose a git repository", start);
            if (path == null) return;
            await OpenRepositoryAsync(path, true);
        }

        private async void cloneRepositoryItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new CloneDialog())
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                await OpenRepositoryAsync(dialog.ClonedPath, true);
            }
        }

        private async void initRepositoryItem_Click(object sender, EventArgs e)
        {
            var path = FolderPicker.Show(this, "Choose a folder for the new repository", null);
            if (path == null) return;
            if (Directory.Exists(Path.Combine(path, ".git")))
            {
                await OpenRepositoryAsync(path, true);
                return;
            }
            if (!Dialogs.Confirm(this, "Create repository", "Run git init in\n\n" + path + "?", "Create")) return;
            var result = await GitRepository.InitAsync(path);
            if (!result.Succeeded)
            {
                Dialogs.Error(this, "Create repository", result.Message);
                return;
            }
            await OpenRepositoryAsync(path, true);
        }

        private async void welcomeView_RecentRequested(object sender, RepositoryRequestedEventArgs e)
        {
            await OpenRepositoryAsync(e.Path, true);
        }

        private void settingsItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new SettingsDialog())
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                var settings = AppSettings.Current;
                Theme.Mode = settings.Theme;
                foreach (var view in _views) view.ApplySettings(settings);
                RefreshWelcome();
            }
        }

        // ------------------------------------------------------------------ shortcuts

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ModernShortcuts.YieldsToTextEntry(keyData, msg.HWnd)) return base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                case Keys.F5:
                    RefreshActive();
                    return true;
                case Keys.Control | Keys.O:
                    openRepositoryItem_Click(this, EventArgs.Empty);
                    return true;
                case Keys.Control | Keys.Shift | Keys.C:
                    cloneRepositoryItem_Click(this, EventArgs.Empty);
                    return true;
                case Keys.Control | Keys.W:
                    if (_views.Count > 0) CloseTab(_views.IndexOf(_activeView));
                    return true;
                case Keys.Control | Keys.Oemcomma:
                    settingsItem_Click(this, EventArgs.Empty);
                    return true;
                case Keys.Control | Keys.Tab:
                    if (_views.Count > 1) ActivateView((_views.IndexOf(_activeView) + 1) % _views.Count);
                    return true;
                case Keys.Control | Keys.Shift | Keys.Tab:
                    if (_views.Count > 1) ActivateView((_views.IndexOf(_activeView) - 1 + _views.Count) % _views.Count);
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async void RefreshActive()
        {
            if (_activeView != null) await _activeView.RefreshAsync();
        }
    }
}
