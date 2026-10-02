namespace GitClient.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.skin = new ModernWinForms.ModernSkin();
            this.tabMenu = new ModernWinForms.ModernContextMenu(this.components);
            this.tabCloseItem = new ModernWinForms.ModernContextMenuItem();
            this.tabCloseOthersItem = new ModernWinForms.ModernContextMenuItem();
            this.tabCopyPathItem = new ModernWinForms.ModernContextMenuItem();
            this.tabExplorerItem = new ModernWinForms.ModernContextMenuItem();
            this.tabTerminalItem = new ModernWinForms.ModernContextMenuItem();
            this.addMenu = new ModernWinForms.ModernContextMenu(this.components);
            this.openRecentItem = new ModernWinForms.ModernContextMenuItem();
            this.openRepositoryItem = new ModernWinForms.ModernContextMenuItem();
            this.cloneRepositoryItem = new ModernWinForms.ModernContextMenuItem();
            this.initRepositoryItem = new ModernWinForms.ModernContextMenuItem();
            this.settingsItem = new ModernWinForms.ModernContextMenuItem();
            this.tabStrip = new ModernWinForms.ModernTabStrip();
            this.hostPanel = new GitClient.Controls.SurfacePanel();
            this.welcomeView = new GitClient.Views.WelcomeView();
            this.hostPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // tabMenu
            //
            this.tabMenu.Items.Add(this.tabCloseItem);
            this.tabMenu.Items.Add(this.tabCloseOthersItem);
            this.tabMenu.Items.Add(this.tabCopyPathItem);
            this.tabMenu.Items.Add(this.tabExplorerItem);
            this.tabMenu.Items.Add(this.tabTerminalItem);
            //
            // tabCloseItem
            //
            this.tabCloseItem.Text = "Close";
            this.tabCloseItem.Click += new System.EventHandler(this.tabCloseItem_Click);
            //
            // tabCloseOthersItem
            //
            this.tabCloseOthersItem.Text = "Close others";
            this.tabCloseOthersItem.Click += new System.EventHandler(this.tabCloseOthersItem_Click);
            //
            // tabCopyPathItem
            //
            this.tabCopyPathItem.BeginGroup = true;
            this.tabCopyPathItem.Text = "Copy path";
            this.tabCopyPathItem.Click += new System.EventHandler(this.tabCopyPathItem_Click);
            //
            // tabExplorerItem
            //
            this.tabExplorerItem.Text = "Show in Explorer";
            this.tabExplorerItem.Click += new System.EventHandler(this.tabExplorerItem_Click);
            //
            // tabTerminalItem
            //
            this.tabTerminalItem.Text = "Open terminal";
            this.tabTerminalItem.Click += new System.EventHandler(this.tabTerminalItem_Click);
            //
            // addMenu
            //
            this.addMenu.Items.Add(this.openRepositoryItem);
            this.addMenu.Items.Add(this.openRecentItem);
            this.addMenu.Items.Add(this.cloneRepositoryItem);
            this.addMenu.Items.Add(this.initRepositoryItem);
            this.addMenu.Items.Add(this.settingsItem);
            //
            // openRepositoryItem
            //
            this.openRepositoryItem.Shortcut = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
            this.openRepositoryItem.SvgIcon = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M3 5h6l2 2h10v12H3z\"/></svg>";
            this.openRepositoryItem.Text = "Open a repository...";
            this.openRepositoryItem.Click += new System.EventHandler(this.openRepositoryItem_Click);
            //
            // openRecentItem
            //
            this.openRecentItem.Text = "Open recent";
            //
            // cloneRepositoryItem
            //
            this.cloneRepositoryItem.SvgIcon = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M11 3h2v9.2l3.5-3.5 1.4 1.4L12 16 6.1 10.1l1.4-1.4L11 12.2z\"/><path fill=\"none\" " +
                "stroke=\"currentColor\" stroke-width=\"2\" d=\"M4 15v5h16v-5\"/></svg>";
            this.cloneRepositoryItem.Text = "Clone from a URL...";
            this.cloneRepositoryItem.Click += new System.EventHandler(this.cloneRepositoryItem_Click);
            //
            // initRepositoryItem
            //
            this.initRepositoryItem.SvgIcon = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><circle fill=\"none\" stro" +
                "ke=\"currentColor\" stroke-width=\"2\" cx=\"12\" cy=\"12\" r=\"9\"/><path fill=\"currentCo" +
                "lor\" d=\"M11 7h2v4h4v2h-4v4h-2v-4H7v-2h4z\"/></svg>";
            this.initRepositoryItem.Text = "Create a repository...";
            this.initRepositoryItem.Click += new System.EventHandler(this.initRepositoryItem_Click);
            //
            // settingsItem
            //
            this.settingsItem.BeginGroup = true;
            this.settingsItem.SvgIcon = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><circle fill=\"none\" stro" +
                "ke=\"currentColor\" stroke-width=\"2\" cx=\"12\" cy=\"12\" r=\"3\"/><path fill=\"none\" st" +
                "roke=\"currentColor\" stroke-width=\"2\" d=\"M12 2v3M12 19v3M2 12h3M19 12h3M4.9 4.9l2.1 2." +
                "1M17 17l2.1 2.1M4.9 19.1L7 17M17 7l2.1-2.1\"/></svg>";
            this.settingsItem.Text = "Settings...";
            this.settingsItem.Click += new System.EventHandler(this.settingsItem_Click);
            //
            // tabStrip
            //
            this.tabStrip.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabStrip.Location = new System.Drawing.Point(0, 31);
            this.tabStrip.Name = "tabStrip";
            this.tabStrip.Size = new System.Drawing.Size(1400, 38);
            this.tabStrip.TabIndex = 0;
            this.tabStrip.SelectedIndexChanged += new System.EventHandler(this.tabStrip_SelectedIndexChanged);
            this.tabStrip.TabCloseRequested += new System.EventHandler<ModernWinForms.ModernTabStripEventArgs>(this.tabStrip_TabCloseRequested);
            this.tabStrip.TabContextMenuRequested += new System.EventHandler<ModernWinForms.ModernTabStripEventArgs>(this.tabStrip_TabContextMenuRequested);
            this.tabStrip.AddRequested += new System.EventHandler(this.tabStrip_AddRequested);
            this.tabStrip.TabMoved += new System.EventHandler<ModernWinForms.ModernTabMovedEventArgs>(this.tabStrip_TabMoved);
            //
            // hostPanel
            //
            this.hostPanel.Controls.Add(this.welcomeView);
            this.hostPanel.CornerRadius = 0;
            this.hostPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hostPanel.Location = new System.Drawing.Point(0, 69);
            this.hostPanel.Name = "hostPanel";
            this.hostPanel.Size = new System.Drawing.Size(1400, 831);
            this.hostPanel.Surface = GitClient.Controls.SurfaceKind.Base;
            this.hostPanel.TabIndex = 1;
            //
            // welcomeView
            //
            this.welcomeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.welcomeView.Location = new System.Drawing.Point(0, 0);
            this.welcomeView.Name = "welcomeView";
            this.welcomeView.Size = new System.Drawing.Size(1400, 831);
            this.welcomeView.TabIndex = 0;
            this.welcomeView.OpenRequested += new System.EventHandler(this.openRepositoryItem_Click);
            this.welcomeView.CloneRequested += new System.EventHandler(this.cloneRepositoryItem_Click);
            this.welcomeView.InitRequested += new System.EventHandler(this.initRepositoryItem_Click);
            this.welcomeView.RecentRequested += new System.EventHandler<GitClient.Views.RepositoryRequestedEventArgs>(this.welcomeView_RecentRequested);
            //
            // MainForm
            //
            this.ClientSize = new System.Drawing.Size(1400, 900);
            this.Controls.Add(this.hostPanel);
            this.Controls.Add(this.tabStrip);
            this.MinimumSize = new System.Drawing.Size(940, 620);
            this.Name = "MainForm";
            this.Skin = this.skin;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Git Client";
            this.TitleBar.ShowIcon = true;
            this.hostPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernSkin skin;
        private ModernWinForms.ModernContextMenu tabMenu;
        private ModernWinForms.ModernContextMenuItem tabCloseItem;
        private ModernWinForms.ModernContextMenuItem tabCloseOthersItem;
        private ModernWinForms.ModernContextMenuItem tabCopyPathItem;
        private ModernWinForms.ModernContextMenuItem tabExplorerItem;
        private ModernWinForms.ModernContextMenuItem tabTerminalItem;
        private ModernWinForms.ModernContextMenu addMenu;
        private ModernWinForms.ModernContextMenuItem openRepositoryItem;
        private ModernWinForms.ModernContextMenuItem openRecentItem;
        private ModernWinForms.ModernContextMenuItem cloneRepositoryItem;
        private ModernWinForms.ModernContextMenuItem initRepositoryItem;
        private ModernWinForms.ModernContextMenuItem settingsItem;
        private ModernWinForms.ModernTabStrip tabStrip;
        private GitClient.Controls.SurfacePanel hostPanel;
        private GitClient.Views.WelcomeView welcomeView;
    }
}
