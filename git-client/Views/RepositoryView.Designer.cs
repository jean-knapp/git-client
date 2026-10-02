namespace GitClient.Views
{
    partial class RepositoryView
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pullMenu = new ModernWinForms.ModernContextMenu(this.components);
            this.pullDefaultItem = new ModernWinForms.ModernContextMenuItem();
            this.pullFastForwardItem = new ModernWinForms.ModernContextMenuItem();
            this.pullRebaseItem = new ModernWinForms.ModernContextMenuItem();
            this.pushMenu = new ModernWinForms.ModernContextMenu(this.components);
            this.pushDefaultItem = new ModernWinForms.ModernContextMenuItem();
            this.pushUpstreamItem = new ModernWinForms.ModernContextMenuItem();
            this.pushForceItem = new ModernWinForms.ModernContextMenuItem();
            this.stashMenu = new ModernWinForms.ModernContextMenu(this.components);
            this.stashPushItem = new ModernWinForms.ModernContextMenuItem();
            this.stashPushUntrackedItem = new ModernWinForms.ModernContextMenuItem();
            this.overflowMenu = new ModernWinForms.ModernContextMenu(this.components);
            this.unstageAllItem = new ModernWinForms.ModernContextMenuItem();
            this.discardAllItem = new ModernWinForms.ModernContextMenuItem();
            this.stashSelectedItem = new ModernWinForms.ModernContextMenuItem();
            this.claudeStageItem = new ModernWinForms.ModernContextMenuItem();
            this.claudeSplitItem = new ModernWinForms.ModernContextMenuItem();
            this.refreshItem = new ModernWinForms.ModernContextMenuItem();
            this.editIgnoreItem = new ModernWinForms.ModernContextMenuItem();
            this.remotesItem = new ModernWinForms.ModernContextMenuItem();
            this.createOnGitHubItem = new ModernWinForms.ModernContextMenuItem();
            this.githubAccountItem = new ModernWinForms.ModernContextMenuItem();
            this.commandBar = new GitClient.Controls.SurfacePanel();
            this.branchButton = new GitClient.Controls.CommandButton();
            this.commandSeparator1 = new GitClient.Controls.SurfacePanel();
            this.fetchButton = new GitClient.Controls.CommandButton();
            this.pullButton = new GitClient.Controls.CommandButton();
            this.pushButton = new GitClient.Controls.CommandButton();
            this.commandSeparator2 = new GitClient.Controls.SurfacePanel();
            this.newBranchButton = new GitClient.Controls.CommandButton();
            this.stashButton = new GitClient.Controls.CommandButton();
            this.filterBox = new ModernWinForms.ModernTextBox();
            this.terminalButton = new GitClient.Controls.CommandButton();
            this.logButton = new GitClient.Controls.CommandButton();
            this.settingsButton = new GitClient.Controls.CommandButton();
            this.statusBar = new GitClient.Controls.SurfacePanel();
            this.statusBranchLabel = new GitClient.Controls.TextLabel();
            this.statusUpstreamLabel = new GitClient.Controls.TextLabel();
            this.statusCountLabel = new GitClient.Controls.TextLabel();
            this.statusMessageLabel = new GitClient.Controls.TextLabel();
            this.progressBar = new ModernWinForms.ModernProgressBar();
            this.outputPanel = new GitClient.Controls.SurfacePanel();
            this.outputBox = new ModernWinForms.ModernTextBox();
            this.body = new GitClient.Controls.SurfacePanel();
            this.mainSplit = new ModernWinForms.ModernSplitContainer();
            this.historySplit = new ModernWinForms.ModernSplitContainer();
            this.detailSplit = new ModernWinForms.ModernSplitContainer();
            this.rightSplit = new ModernWinForms.ModernSplitContainer();
            this.historyCard = new GitClient.Controls.SurfacePanel();
            this.historyList = new GitClient.Controls.HistoryListControl();
            this.historyHeader = new GitClient.Controls.SurfacePanel();
            this.historyTitleLabel = new GitClient.Controls.TextLabel();
            this.historyCountLabel = new GitClient.Controls.TextLabel();
            this.scopeButton = new GitClient.Controls.CommandButton();
            this.detailCard = new GitClient.Controls.SurfacePanel();
            this.detailBody = new GitClient.Controls.SurfacePanel();
            this.diffColumn = new GitClient.Controls.SurfacePanel();
            this.diffView = new GitClient.Controls.DiffViewControl();
            this.diffHeader = new GitClient.Controls.SurfacePanel();
            this.diffPathLabel = new GitClient.Controls.TextLabel();
            this.diffLayoutToggle = new GitClient.Controls.SegmentedControl();
            this.commitFilesPanel = new GitClient.Controls.SurfacePanel();
            this.commitFilesList = new GitClient.Controls.FileListControl();
            this.detailHeader = new GitClient.Controls.SurfacePanel();
            this.detailAvatar = new GitClient.Controls.AvatarBox();
            this.detailSubjectLabel = new GitClient.Controls.TextLabel();
            this.detailMetaLabel = new GitClient.Controls.TextLabel();
            this.detailShaChip = new GitClient.Controls.Chip();
            this.detailParentLabel = new GitClient.Controls.TextLabel();
            this.copyShaButton = new GitClient.Controls.CommandButton();
            this.revertButton = new GitClient.Controls.CommandButton();
            this.rightColumn = new GitClient.Controls.SurfacePanel();
            this.changesCard = new GitClient.Controls.SurfacePanel();
            this.changesList = new GitClient.Controls.ChangesListControl();
            this.conflictBanner = new GitClient.Controls.SurfacePanel();
            this.conflictIcon = new GitClient.Controls.TextLabel();
            this.conflictTitleLabel = new GitClient.Controls.TextLabel();
            this.conflictBodyLabel = new GitClient.Controls.TextLabel();
            this.continueButton = new GitClient.Controls.CommandButton();
            this.abortButton = new GitClient.Controls.CommandButton();
            this.changesHeader = new GitClient.Controls.SurfacePanel();
            this.changesTitleLabel = new GitClient.Controls.TextLabel();
            this.changesCountChip = new GitClient.Controls.Chip();
            this.stageAllButton = new GitClient.Controls.CommandButton();
            this.claudeStageButton = new GitClient.Controls.CommandButton();
            this.overflowButton = new GitClient.Controls.CommandButton();
            this.composerCard = new GitClient.Controls.SurfacePanel();
            this.commitToLabel = new GitClient.Controls.TextLabel();
            this.branchChip = new GitClient.Controls.Chip();
            this.amendLabel = new GitClient.Controls.TextLabel();
            this.amendSwitch = new GitClient.Controls.ToggleSwitchControl();
            this.fieldGroup = new GitClient.Controls.SurfacePanel();
            this.descriptionBox = new ModernWinForms.ModernTextBox();
            this.descriptionPlaceholder = new GitClient.Controls.TextLabel();
            this.hintRow = new GitClient.Controls.SurfacePanel();
            this.hintLabel = new GitClient.Controls.TextLabel();
            this.summaryRow = new GitClient.Controls.SurfacePanel();
            this.summaryBox = new ModernWinForms.ModernTextBox();
            this.aiButton = new GitClient.Controls.CommandButton();
            this.commitButton = new GitClient.Controls.CommandButton();
            this.commitPushButton = new GitClient.Controls.CommandButton();
            this.commandBar.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.outputPanel.SuspendLayout();
            this.body.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.historySplit)).BeginInit();
            this.historySplit.Panel1.SuspendLayout();
            this.historySplit.Panel2.SuspendLayout();
            this.historySplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.detailSplit)).BeginInit();
            this.detailSplit.Panel1.SuspendLayout();
            this.detailSplit.Panel2.SuspendLayout();
            this.detailSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightSplit)).BeginInit();
            this.rightSplit.Panel1.SuspendLayout();
            this.rightSplit.Panel2.SuspendLayout();
            this.rightSplit.SuspendLayout();
            this.historyCard.SuspendLayout();
            this.historyHeader.SuspendLayout();
            this.detailCard.SuspendLayout();
            this.detailBody.SuspendLayout();
            this.diffColumn.SuspendLayout();
            this.diffHeader.SuspendLayout();
            this.commitFilesPanel.SuspendLayout();
            this.detailHeader.SuspendLayout();
            this.rightColumn.SuspendLayout();
            this.changesCard.SuspendLayout();
            this.conflictBanner.SuspendLayout();
            this.changesHeader.SuspendLayout();
            this.composerCard.SuspendLayout();
            this.fieldGroup.SuspendLayout();
            this.hintRow.SuspendLayout();
            this.summaryRow.SuspendLayout();
            this.SuspendLayout();
            //
            // pullMenu
            //
            this.pullMenu.Items.Add(this.pullDefaultItem);
            this.pullMenu.Items.Add(this.pullFastForwardItem);
            this.pullMenu.Items.Add(this.pullRebaseItem);
            //
            // pullDefaultItem
            //
            this.pullDefaultItem.Text = "Pull (merge)";
            this.pullDefaultItem.Click += new System.EventHandler(this.pullDefaultItem_Click);
            //
            // pullFastForwardItem
            //
            this.pullFastForwardItem.Text = "Pull (fast-forward only)";
            this.pullFastForwardItem.Click += new System.EventHandler(this.pullFastForwardItem_Click);
            //
            // pullRebaseItem
            //
            this.pullRebaseItem.Text = "Pull (rebase)";
            this.pullRebaseItem.Click += new System.EventHandler(this.pullRebaseItem_Click);
            //
            // pushMenu
            //
            this.pushMenu.Items.Add(this.pushDefaultItem);
            this.pushMenu.Items.Add(this.pushUpstreamItem);
            this.pushMenu.Items.Add(this.pushForceItem);
            //
            // pushDefaultItem
            //
            this.pushDefaultItem.Text = "Push";
            this.pushDefaultItem.Click += new System.EventHandler(this.pushDefaultItem_Click);
            //
            // pushUpstreamItem
            //
            this.pushUpstreamItem.Text = "Push and set upstream";
            this.pushUpstreamItem.Click += new System.EventHandler(this.pushUpstreamItem_Click);
            //
            // pushForceItem
            //
            this.pushForceItem.BeginGroup = true;
            this.pushForceItem.Text = "Force push (with lease)";
            this.pushForceItem.Click += new System.EventHandler(this.pushForceItem_Click);
            //
            // stashMenu
            //
            this.stashMenu.Items.Add(this.stashPushItem);
            this.stashMenu.Items.Add(this.stashPushUntrackedItem);
            //
            // stashPushItem
            //
            this.stashPushItem.Text = "Stash changes...";
            this.stashPushItem.Click += new System.EventHandler(this.stashPushItem_Click);
            //
            // stashPushUntrackedItem
            //
            this.stashPushUntrackedItem.Text = "Stash including untracked...";
            this.stashPushUntrackedItem.Click += new System.EventHandler(this.stashPushUntrackedItem_Click);
            //
            // overflowMenu
            //
            this.overflowMenu.Items.Add(this.claudeStageItem);
            this.overflowMenu.Items.Add(this.claudeSplitItem);
            this.overflowMenu.Items.Add(this.unstageAllItem);
            this.overflowMenu.Items.Add(this.discardAllItem);
            this.overflowMenu.Items.Add(this.stashSelectedItem);
            this.overflowMenu.Items.Add(this.editIgnoreItem);
            this.overflowMenu.Items.Add(this.remotesItem);
            this.overflowMenu.Items.Add(this.createOnGitHubItem);
            this.overflowMenu.Items.Add(this.githubAccountItem);
            this.overflowMenu.Items.Add(this.refreshItem);
            //
            // claudeStageItem
            //
            this.claudeStageItem.Text = "Stage with Claude...";
            this.claudeStageItem.Click += new System.EventHandler(this.claudeStageItem_Click);
            //
            // claudeSplitItem
            //
            this.claudeSplitItem.Text = "Split into commits with Claude...";
            this.claudeSplitItem.Click += new System.EventHandler(this.claudeSplitItem_Click);
            //
            // unstageAllItem
            //
            this.unstageAllItem.BeginGroup = true;
            this.unstageAllItem.Text = "Unstage all";
            this.unstageAllItem.Click += new System.EventHandler(this.unstageAllItem_Click);
            //
            // discardAllItem
            //
            this.discardAllItem.Text = "Discard all changes...";
            this.discardAllItem.Click += new System.EventHandler(this.discardAllItem_Click);
            //
            // stashSelectedItem
            //
            this.stashSelectedItem.Text = "Stash changes...";
            this.stashSelectedItem.Click += new System.EventHandler(this.stashPushItem_Click);
            //
            // editIgnoreItem
            //
            this.editIgnoreItem.BeginGroup = true;
            this.editIgnoreItem.Text = "Edit .gitignore...";
            this.editIgnoreItem.Click += new System.EventHandler(this.editIgnoreItem_Click);
            //
            // remotesItem
            //
            this.remotesItem.Text = "Remotes...";
            this.remotesItem.Click += new System.EventHandler(this.remotesItem_Click);
            //
            // createOnGitHubItem
            //
            this.createOnGitHubItem.Text = "Create on GitHub...";
            this.createOnGitHubItem.Click += new System.EventHandler(this.createOnGitHubItem_Click);
            //
            // githubAccountItem
            //
            this.githubAccountItem.Text = "GitHub account...";
            this.githubAccountItem.Click += new System.EventHandler(this.githubAccountItem_Click);
            //
            // refreshItem
            //
            this.refreshItem.BeginGroup = true;
            this.refreshItem.Text = "Refresh";
            this.refreshItem.Click += new System.EventHandler(this.refreshItem_Click);
            //
            // commandBar
            //
            this.commandBar.BottomDivider = true;
            this.commandBar.Controls.Add(this.branchButton);
            this.commandBar.Controls.Add(this.commandSeparator1);
            this.commandBar.Controls.Add(this.fetchButton);
            this.commandBar.Controls.Add(this.pullButton);
            this.commandBar.Controls.Add(this.pushButton);
            this.commandBar.Controls.Add(this.commandSeparator2);
            this.commandBar.Controls.Add(this.newBranchButton);
            this.commandBar.Controls.Add(this.stashButton);
            this.commandBar.Controls.Add(this.filterBox);
            this.commandBar.Controls.Add(this.terminalButton);
            this.commandBar.Controls.Add(this.logButton);
            this.commandBar.Controls.Add(this.settingsButton);
            this.commandBar.CornerRadius = 0;
            this.commandBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.commandBar.Location = new System.Drawing.Point(0, 0);
            this.commandBar.Name = "commandBar";
            this.commandBar.Size = new System.Drawing.Size(1400, 54);
            this.commandBar.Surface = GitClient.Controls.SurfaceKind.Layer;
            this.commandBar.TabIndex = 0;
            this.commandBar.TopCardStroke = true;
            this.commandBar.TopDivider = true;
            //
            // branchButton
            //
            this.branchButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.branchButton.Chevron = GitClient.Controls.ChevronMode.Trailing;
            this.branchButton.IconRole = GitClient.Controls.TextRole.Lane;
            this.branchButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><circle fill=\"currentColo" +
                "r\" cx=\"6\" cy=\"5\" r=\"2.6\"/><circle fill=\"currentColor\" cx=\"6\" cy=\"19\" r=\"2.6" +
                "\"/><circle fill=\"currentColor\" cx=\"18\" cy=\"8\" r=\"2.6\"/><path fill=\"none\" stroke" +
                "=\"currentColor\" stroke-width=\"2\" d=\"M6 7.5v9M18 10.5c0 5-12 2-12 6\"/></svg>";
            this.branchButton.Location = new System.Drawing.Point(12, 11);
            this.branchButton.Name = "branchButton";
            this.branchButton.Semibold = true;
            this.branchButton.Size = new System.Drawing.Size(212, 32);
            this.branchButton.TabIndex = 0;
            this.branchButton.Text = "master";
            this.branchButton.TextSizePx = 14F;
            this.branchButton.ToolTipText = "Switch branch";
            this.branchButton.Click += new System.EventHandler(this.branchButton_Click);
            //
            // commandSeparator1
            //
            this.commandSeparator1.CornerRadius = 0;
            this.commandSeparator1.Location = new System.Drawing.Point(236, 17);
            this.commandSeparator1.Name = "commandSeparator1";
            this.commandSeparator1.RightDivider = true;
            this.commandSeparator1.Size = new System.Drawing.Size(1, 20);
            this.commandSeparator1.Surface = GitClient.Controls.SurfaceKind.None;
            this.commandSeparator1.TabIndex = 1;
            //
            // fetchButton
            //
            this.fetchButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"none\" stroke" +
                "=\"currentColor\" stroke-width=\"2\" d=\"M19 12c0 3.9-3.1 7-7 7s-7-3.1-7-7 3.1-7 7-7c2.4 0" +
                " 4.5 1.2 5.8 3\"/><path fill=\"currentColor\" d=\"M20 3v6h-6z\"/></svg>";
            this.fetchButton.Location = new System.Drawing.Point(245, 11);
            this.fetchButton.Name = "fetchButton";
            this.fetchButton.Size = new System.Drawing.Size(76, 32);
            this.fetchButton.TabIndex = 2;
            this.fetchButton.Text = "Fetch";
            this.fetchButton.TextSizePx = 14F;
            this.fetchButton.Click += new System.EventHandler(this.fetchButton_Click);
            //
            // pullButton
            //
            this.pullButton.Chevron = GitClient.Controls.ChevronMode.Split;
            this.pullButton.DropDownMenu = this.pullMenu;
            this.pullButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M11 3h2v9.2l3.5-3.5 1.4 1.4L12 16 6.1 10.1l1.4-1.4L11 12.2z\"/><path fill=\"current" +
                "Color\" d=\"M4 18h16v2H4z\"/></svg>";
            this.pullButton.Location = new System.Drawing.Point(325, 11);
            this.pullButton.Name = "pullButton";
            this.pullButton.Size = new System.Drawing.Size(94, 32);
            this.pullButton.SplitWidth = 24;
            this.pullButton.TabIndex = 3;
            this.pullButton.Text = "Pull";
            this.pullButton.TextSizePx = 14F;
            this.pullButton.Click += new System.EventHandler(this.pullButton_Click);
            //
            // pushButton
            //
            this.pushButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.pushButton.Chevron = GitClient.Controls.ChevronMode.Split;
            this.pushButton.DropDownMenu = this.pushMenu;
            this.pushButton.IconRole = GitClient.Controls.TextRole.Accent;
            this.pushButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M12 3l5.9 5.9-1.4 1.4L13 6.8V16h-2V6.8L7.5 10.3 6.1 8.9z\"/><path fill=\"currentCol" +
                "or\" d=\"M4 18h16v2H4z\"/></svg>";
            this.pushButton.Location = new System.Drawing.Point(423, 11);
            this.pushButton.Name = "pushButton";
            this.pushButton.Size = new System.Drawing.Size(110, 32);
            this.pushButton.TabIndex = 4;
            this.pushButton.Text = "Push";
            this.pushButton.TextSizePx = 14F;
            this.pushButton.Click += new System.EventHandler(this.pushButton_Click);
            //
            // commandSeparator2
            //
            this.commandSeparator2.CornerRadius = 0;
            this.commandSeparator2.Location = new System.Drawing.Point(545, 17);
            this.commandSeparator2.Name = "commandSeparator2";
            this.commandSeparator2.RightDivider = true;
            this.commandSeparator2.Size = new System.Drawing.Size(1, 20);
            this.commandSeparator2.Surface = GitClient.Controls.SurfaceKind.None;
            this.commandSeparator2.TabIndex = 5;
            //
            // newBranchButton
            //
            this.newBranchButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><circle fill=\"currentColo" +
                "r\" cx=\"6\" cy=\"5\" r=\"2.6\"/><circle fill=\"currentColor\" cx=\"6\" cy=\"19\" r=\"2.6" +
                "\"/><circle fill=\"currentColor\" cx=\"18\" cy=\"8\" r=\"2.6\"/><path fill=\"none\" stroke" +
                "=\"currentColor\" stroke-width=\"2\" d=\"M6 7.5v9M18 10.5c0 5-12 2-12 6\"/></svg>";
            this.newBranchButton.Location = new System.Drawing.Point(554, 11);
            this.newBranchButton.Name = "newBranchButton";
            this.newBranchButton.Size = new System.Drawing.Size(116, 32);
            this.newBranchButton.TabIndex = 6;
            this.newBranchButton.Text = "New branch";
            this.newBranchButton.TextSizePx = 14F;
            this.newBranchButton.Click += new System.EventHandler(this.newBranchButton_Click);
            //
            // stashButton
            //
            this.stashButton.Chevron = GitClient.Controls.ChevronMode.Split;
            this.stashButton.DropDownMenu = this.stashMenu;
            this.stashButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M3 5h18v5h-2V7H5v3H3z\"/><path fill=\"currentColor\" d=\"M3 12h7v2h4v-2h7v8H3z\"/><" +
                "/svg>";
            this.stashButton.Location = new System.Drawing.Point(674, 11);
            this.stashButton.Name = "stashButton";
            this.stashButton.Size = new System.Drawing.Size(100, 32);
            this.stashButton.SplitWidth = 24;
            this.stashButton.TabIndex = 7;
            this.stashButton.Text = "Stash";
            this.stashButton.TextSizePx = 14F;
            this.stashButton.Click += new System.EventHandler(this.stashButton_Click);
            //
            // filterBox
            //
            this.filterBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.filterBox.Location = new System.Drawing.Point(1252, 11);
            this.filterBox.Name = "filterBox";
            this.filterBox.PlaceholderText = "Filter history, author, sha...";
            this.filterBox.Size = new System.Drawing.Size(240, 32);
            this.filterBox.TabIndex = 8;
            this.filterBox.TextChanged += new System.EventHandler(this.filterBox_TextChanged);
            //
            // terminalButton
            //
            this.terminalButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.terminalButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"none\" stroke" +
                "=\"currentColor\" stroke-width=\"2\" d=\"M4 5h16v14H4z\"/><path fill=\"none\" stroke=\"cur" +
                "rentColor\" stroke-width=\"2\" d=\"M7 9l3 3-3 3M12 15h5\"/></svg>";
            this.terminalButton.Location = new System.Drawing.Point(1496, 11);
            this.terminalButton.Name = "terminalButton";
            this.terminalButton.PaddingX = 8;
            this.terminalButton.Size = new System.Drawing.Size(32, 32);
            this.terminalButton.TabIndex = 9;
            this.terminalButton.ToolTipText = "Open a terminal here";
            this.terminalButton.Click += new System.EventHandler(this.terminalButton_Click);
            //
            // logButton
            //
            this.logButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.logButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><rect fill=\"currentColor" +
                "\" x=\"3\" y=\"4\" width=\"18\" height=\"3\"/><path fill=\"none\" stroke=\"currentColor\" " +
                "stroke-width=\"2\" d=\"M4 8v11h16V8\"/><path fill=\"none\" stroke=\"currentColor\" stroke-" +
                "width=\"2\" d=\"M7 12h6M7 15h10\"/></svg>";
            this.logButton.Location = new System.Drawing.Point(1532, 11);
            this.logButton.Name = "logButton";
            this.logButton.PaddingX = 8;
            this.logButton.Size = new System.Drawing.Size(32, 32);
            this.logButton.TabIndex = 10;
            this.logButton.ToolTipText = "Show the git command log";
            this.logButton.Click += new System.EventHandler(this.logButton_Click);
            //
            // settingsButton
            //
            this.settingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><circle fill=\"none\" stro" +
                "ke=\"currentColor\" stroke-width=\"2\" cx=\"12\" cy=\"12\" r=\"3\"/><path fill=\"none\" st" +
                "roke=\"currentColor\" stroke-width=\"2\" d=\"M12 2v3M12 19v3M2 12h3M19 12h3M4.9 4.9l2.1 2." +
                "1M17 17l2.1 2.1M4.9 19.1L7 17M17 7l2.1-2.1\"/></svg>";
            this.settingsButton.Location = new System.Drawing.Point(1568, 11);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.PaddingX = 8;
            this.settingsButton.Size = new System.Drawing.Size(32, 32);
            this.settingsButton.TabIndex = 11;
            this.settingsButton.ToolTipText = "Settings";
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            //
            // statusBar
            //
            this.statusBar.Controls.Add(this.statusBranchLabel);
            this.statusBar.Controls.Add(this.statusUpstreamLabel);
            this.statusBar.Controls.Add(this.statusCountLabel);
            this.statusBar.Controls.Add(this.statusMessageLabel);
            this.statusBar.Controls.Add(this.progressBar);
            this.statusBar.CornerRadius = 0;
            this.statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusBar.Location = new System.Drawing.Point(0, 872);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1400, 28);
            this.statusBar.Surface = GitClient.Controls.SurfaceKind.Base;
            this.statusBar.TabIndex = 3;
            this.statusBar.TopDivider = true;
            //
            // statusBranchLabel
            //
            this.statusBranchLabel.IconGap = 6;
            this.statusBranchLabel.IconRole = GitClient.Controls.TextRole.Secondary;
            this.statusBranchLabel.IconSize = 12;
            this.statusBranchLabel.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><circle fill=\"currentColo" +
                "r\" cx=\"6\" cy=\"5\" r=\"2.6\"/><circle fill=\"currentColor\" cx=\"6\" cy=\"19\" r=\"2.6" +
                "\"/><circle fill=\"currentColor\" cx=\"18\" cy=\"8\" r=\"2.6\"/><path fill=\"none\" stroke" +
                "=\"currentColor\" stroke-width=\"2\" d=\"M6 7.5v9M18 10.5c0 5-12 2-12 6\"/></svg>";
            this.statusBranchLabel.Location = new System.Drawing.Point(14, 0);
            this.statusBranchLabel.Name = "statusBranchLabel";
            this.statusBranchLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.statusBranchLabel.Size = new System.Drawing.Size(160, 28);
            this.statusBranchLabel.SizePx = 12F;
            this.statusBranchLabel.TabIndex = 0;
            this.statusBranchLabel.Text = "master";
            //
            // statusUpstreamLabel
            //
            this.statusUpstreamLabel.Location = new System.Drawing.Point(190, 0);
            this.statusUpstreamLabel.Name = "statusUpstreamLabel";
            this.statusUpstreamLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.statusUpstreamLabel.Size = new System.Drawing.Size(240, 28);
            this.statusUpstreamLabel.SizePx = 12F;
            this.statusUpstreamLabel.TabIndex = 1;
            //
            // statusCountLabel
            //
            this.statusCountLabel.Location = new System.Drawing.Point(446, 0);
            this.statusCountLabel.Name = "statusCountLabel";
            this.statusCountLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.statusCountLabel.Size = new System.Drawing.Size(220, 28);
            this.statusCountLabel.SizePx = 12F;
            this.statusCountLabel.TabIndex = 2;
            //
            // statusMessageLabel
            //
            this.statusMessageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.statusMessageLabel.Location = new System.Drawing.Point(986, 0);
            this.statusMessageLabel.Name = "statusMessageLabel";
            this.statusMessageLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.statusMessageLabel.Size = new System.Drawing.Size(240, 28);
            this.statusMessageLabel.SizePx = 12F;
            this.statusMessageLabel.TabIndex = 3;
            this.statusMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // progressBar
            //
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(1236, 10);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(150, 6);
            this.progressBar.Style = ModernWinForms.ModernProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 4;
            this.progressBar.Visible = false;
            //
            // outputPanel
            //
            this.outputPanel.Controls.Add(this.outputBox);
            this.outputPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.outputPanel.Location = new System.Drawing.Point(0, 712);
            this.outputPanel.Name = "outputPanel";
            this.outputPanel.Padding = new System.Windows.Forms.Padding(10);
            this.outputPanel.Size = new System.Drawing.Size(1400, 160);
            this.outputPanel.TabIndex = 2;
            this.outputPanel.Visible = false;
            //
            // outputBox
            //
            this.outputBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputBox.Font = new System.Drawing.Font("Consolas", 9F);
            this.outputBox.Location = new System.Drawing.Point(10, 10);
            this.outputBox.Multiline = true;
            this.outputBox.Name = "outputBox";
            this.outputBox.OverrideSkinFont = true;
            this.outputBox.ReadOnly = true;
            this.outputBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputBox.Size = new System.Drawing.Size(1380, 140);
            this.outputBox.TabIndex = 0;
            this.outputBox.WordWrap = false;
            //
            // body
            //
            this.body.Controls.Add(this.mainSplit);
            this.body.CornerRadius = 0;
            this.body.Dock = System.Windows.Forms.DockStyle.Fill;
            this.body.Location = new System.Drawing.Point(0, 54);
            this.body.Name = "body";
            this.body.Padding = new System.Windows.Forms.Padding(12);
            this.body.Size = new System.Drawing.Size(1400, 658);
            this.body.Surface = GitClient.Controls.SurfaceKind.Base;
            this.body.TabIndex = 1;
            //
            // mainSplit
            //
            this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.mainSplit.Location = new System.Drawing.Point(12, 12);
            this.mainSplit.Name = "mainSplit";
            this.mainSplit.Panel1.Controls.Add(this.historySplit);
            this.mainSplit.Panel1MinSize = 420;
            this.mainSplit.Panel2.Controls.Add(this.rightColumn);
            this.mainSplit.Panel2MinSize = 320;
            this.mainSplit.Size = new System.Drawing.Size(1376, 634);
            this.mainSplit.SplitterDistance = 928;
            this.mainSplit.SplitterWidth = 12;
            this.mainSplit.TabIndex = 0;
            this.mainSplit.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.split_SplitterMoved);
            //
            // historySplit
            //
            this.historySplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historySplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.historySplit.Location = new System.Drawing.Point(0, 0);
            this.historySplit.Name = "historySplit";
            this.historySplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.historySplit.Panel1.Controls.Add(this.historyCard);
            this.historySplit.Panel1MinSize = 120;
            this.historySplit.Panel2.Controls.Add(this.detailCard);
            this.historySplit.Panel2MinSize = 160;
            this.historySplit.Size = new System.Drawing.Size(928, 634);
            this.historySplit.SplitterDistance = 230;
            this.historySplit.SplitterWidth = 12;
            this.historySplit.TabIndex = 0;
            this.historySplit.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.split_SplitterMoved);
            //
            // detailSplit
            //
            this.detailSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.detailSplit.Location = new System.Drawing.Point(0, 0);
            this.detailSplit.Name = "detailSplit";
            this.detailSplit.Panel1.Controls.Add(this.commitFilesPanel);
            this.detailSplit.Panel1MinSize = 180;
            this.detailSplit.Panel2.Controls.Add(this.diffColumn);
            this.detailSplit.Panel2MinSize = 240;
            this.detailSplit.Size = new System.Drawing.Size(938, 312);
            this.detailSplit.SplitterDistance = 321;
            this.detailSplit.SplitterWidth = 12;
            this.detailSplit.TabIndex = 0;
            this.detailSplit.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.split_SplitterMoved);
            //
            // historyCard
            //
            this.historyCard.Controls.Add(this.historyList);
            this.historyCard.Controls.Add(this.historyHeader);
            this.historyCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyCard.Location = new System.Drawing.Point(0, 0);
            this.historyCard.Name = "historyCard";
            this.historyCard.Padding = new System.Windows.Forms.Padding(1);
            this.historyCard.Size = new System.Drawing.Size(940, 230);
            this.historyCard.TabIndex = 0;
            //
            // historyList
            //
            this.historyList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyList.Location = new System.Drawing.Point(1, 45);
            this.historyList.Name = "historyList";
            this.historyList.RowHeight = 36;
            this.historyList.Size = new System.Drawing.Size(938, 184);
            this.historyList.TabIndex = 1;
            this.historyList.SelectionChanged += new System.EventHandler(this.historyList_SelectionChanged);
            this.historyList.RowContextMenuRequested += new System.EventHandler<GitClient.Controls.HistoryRowEventArgs>(this.historyList_RowContextMenuRequested);
            this.historyList.RowActivated += new System.EventHandler<GitClient.Controls.HistoryRowEventArgs>(this.historyList_RowActivated);
            this.historyList.RefActivated += new System.EventHandler<GitClient.Controls.HistoryRowEventArgs>(this.historyList_RefActivated);
            //
            // historyHeader
            //
            this.historyHeader.Controls.Add(this.historyTitleLabel);
            this.historyHeader.Controls.Add(this.historyCountLabel);
            this.historyHeader.Controls.Add(this.scopeButton);
            this.historyHeader.CornerRadius = 0;
            this.historyHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.historyHeader.Location = new System.Drawing.Point(1, 1);
            this.historyHeader.Name = "historyHeader";
            this.historyHeader.Size = new System.Drawing.Size(938, 44);
            this.historyHeader.Surface = GitClient.Controls.SurfaceKind.None;
            this.historyHeader.TabIndex = 0;
            //
            // historyTitleLabel
            //
            this.historyTitleLabel.Location = new System.Drawing.Point(13, 0);
            this.historyTitleLabel.Name = "historyTitleLabel";
            this.historyTitleLabel.Semibold = true;
            this.historyTitleLabel.Size = new System.Drawing.Size(64, 44);
            this.historyTitleLabel.SizePx = 14F;
            this.historyTitleLabel.TabIndex = 0;
            this.historyTitleLabel.Text = "History";
            //
            // historyCountLabel
            //
            this.historyCountLabel.Location = new System.Drawing.Point(83, 0);
            this.historyCountLabel.Name = "historyCountLabel";
            this.historyCountLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.historyCountLabel.Size = new System.Drawing.Size(160, 44);
            this.historyCountLabel.SizePx = 12F;
            this.historyCountLabel.TabIndex = 1;
            //
            // scopeButton
            //
            this.scopeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.scopeButton.Chevron = GitClient.Controls.ChevronMode.Inline;
            this.scopeButton.Location = new System.Drawing.Point(806, 8);
            this.scopeButton.Name = "scopeButton";
            this.scopeButton.Size = new System.Drawing.Size(120, 28);
            this.scopeButton.TabIndex = 2;
            this.scopeButton.Text = "All branches";
            this.scopeButton.Click += new System.EventHandler(this.scopeButton_Click);
            //
            // detailCard
            //
            this.detailCard.Controls.Add(this.detailBody);
            this.detailCard.Controls.Add(this.detailHeader);
            this.detailCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailCard.Location = new System.Drawing.Point(0, 0);
            this.detailCard.Name = "detailCard";
            this.detailCard.Padding = new System.Windows.Forms.Padding(1);
            this.detailCard.Size = new System.Drawing.Size(928, 392);
            this.detailCard.TabIndex = 2;
            //
            // detailBody
            //
            this.detailBody.Controls.Add(this.detailSplit);
            this.detailBody.CornerRadius = 0;
            this.detailBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailBody.Location = new System.Drawing.Point(1, 79);
            this.detailBody.Name = "detailBody";
            this.detailBody.Size = new System.Drawing.Size(938, 312);
            this.detailBody.Surface = GitClient.Controls.SurfaceKind.None;
            this.detailBody.TabIndex = 1;
            //
            // diffColumn
            //
            this.diffColumn.Controls.Add(this.diffView);
            this.diffColumn.Controls.Add(this.diffHeader);
            this.diffColumn.CornerRadius = 0;
            this.diffColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diffColumn.Location = new System.Drawing.Point(0, 0);
            this.diffColumn.Name = "diffColumn";
            this.diffColumn.Size = new System.Drawing.Size(605, 312);
            this.diffColumn.Surface = GitClient.Controls.SurfaceKind.None;
            this.diffColumn.TabIndex = 1;
            //
            // diffView
            //
            this.diffView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diffView.Location = new System.Drawing.Point(0, 34);
            this.diffView.Name = "diffView";
            this.diffView.Size = new System.Drawing.Size(617, 278);
            this.diffView.TabIndex = 1;
            //
            // diffHeader
            //
            this.diffHeader.BottomDivider = true;
            this.diffHeader.Controls.Add(this.diffPathLabel);
            this.diffHeader.Controls.Add(this.diffLayoutToggle);
            this.diffHeader.CornerRadius = 0;
            this.diffHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.diffHeader.Location = new System.Drawing.Point(0, 0);
            this.diffHeader.Name = "diffHeader";
            this.diffHeader.Size = new System.Drawing.Size(617, 34);
            this.diffHeader.Surface = GitClient.Controls.SurfaceKind.None;
            this.diffHeader.TabIndex = 0;
            //
            // diffPathLabel
            //
            this.diffPathLabel.Location = new System.Drawing.Point(14, 0);
            this.diffPathLabel.Name = "diffPathLabel";
            this.diffPathLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.diffPathLabel.Size = new System.Drawing.Size(440, 33);
            this.diffPathLabel.SizePx = 12F;
            this.diffPathLabel.TabIndex = 0;
            //
            // diffLayoutToggle
            //
            this.diffLayoutToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.diffLayoutToggle.Items.Add("Unified");
            this.diffLayoutToggle.Items.Add("Split");
            this.diffLayoutToggle.Location = new System.Drawing.Point(497, 5);
            this.diffLayoutToggle.Name = "diffLayoutToggle";
            this.diffLayoutToggle.Size = new System.Drawing.Size(106, 24);
            this.diffLayoutToggle.TabIndex = 1;
            this.diffLayoutToggle.SelectedIndexChanged += new System.EventHandler(this.diffLayoutToggle_SelectedIndexChanged);
            //
            // commitFilesPanel
            //
            this.commitFilesPanel.Controls.Add(this.commitFilesList);
            this.commitFilesPanel.CornerRadius = 0;
            this.commitFilesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commitFilesPanel.Location = new System.Drawing.Point(0, 0);
            this.commitFilesPanel.Name = "commitFilesPanel";
            this.commitFilesPanel.Size = new System.Drawing.Size(321, 312);
            this.commitFilesPanel.Surface = GitClient.Controls.SurfaceKind.None;
            this.commitFilesPanel.TabIndex = 0;
            //
            // commitFilesList
            //
            this.commitFilesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commitFilesList.Location = new System.Drawing.Point(0, 0);
            this.commitFilesList.Name = "commitFilesList";
            this.commitFilesList.Size = new System.Drawing.Size(321, 312);
            this.commitFilesList.TabIndex = 0;
            this.commitFilesList.SelectionChanged += new System.EventHandler(this.commitFilesList_SelectionChanged);
            //
            // detailHeader
            //
            this.detailHeader.BottomDivider = true;
            this.detailHeader.Controls.Add(this.detailAvatar);
            this.detailHeader.Controls.Add(this.detailSubjectLabel);
            this.detailHeader.Controls.Add(this.detailMetaLabel);
            this.detailHeader.Controls.Add(this.detailShaChip);
            this.detailHeader.Controls.Add(this.detailParentLabel);
            this.detailHeader.Controls.Add(this.copyShaButton);
            this.detailHeader.Controls.Add(this.revertButton);
            this.detailHeader.CornerRadius = 0;
            this.detailHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.detailHeader.Location = new System.Drawing.Point(1, 1);
            this.detailHeader.Name = "detailHeader";
            this.detailHeader.Size = new System.Drawing.Size(938, 78);
            this.detailHeader.Surface = GitClient.Controls.SurfaceKind.None;
            this.detailHeader.TabIndex = 0;
            //
            // detailAvatar
            //
            this.detailAvatar.Location = new System.Drawing.Point(16, 20);
            this.detailAvatar.Name = "detailAvatar";
            this.detailAvatar.Size = new System.Drawing.Size(38, 38);
            this.detailAvatar.TabIndex = 0;
            //
            // detailSubjectLabel
            //
            this.detailSubjectLabel.Location = new System.Drawing.Point(68, 18);
            this.detailSubjectLabel.Name = "detailSubjectLabel";
            this.detailSubjectLabel.Semibold = true;
            this.detailSubjectLabel.Size = new System.Drawing.Size(560, 22);
            this.detailSubjectLabel.SizePx = 15F;
            this.detailSubjectLabel.TabIndex = 1;
            this.detailSubjectLabel.Text = "Select a commit to see its details.";
            //
            // detailMetaLabel
            //
            this.detailMetaLabel.Location = new System.Drawing.Point(68, 41);
            this.detailMetaLabel.Name = "detailMetaLabel";
            this.detailMetaLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.detailMetaLabel.Size = new System.Drawing.Size(300, 19);
            this.detailMetaLabel.SizePx = 12.5F;
            this.detailMetaLabel.TabIndex = 2;
            //
            // detailShaChip
            //
            this.detailShaChip.CornerRadius = 4;
            this.detailShaChip.Location = new System.Drawing.Point(374, 42);
            this.detailShaChip.Monospace = true;
            this.detailShaChip.Name = "detailShaChip";
            this.detailShaChip.Semibold = false;
            this.detailShaChip.Size = new System.Drawing.Size(62, 17);
            this.detailShaChip.Style = GitClient.Controls.ChipStyle.Subtle;
            this.detailShaChip.TabIndex = 3;
            this.detailShaChip.TextSizePx = 12F;
            this.detailShaChip.Visible = false;
            //
            // detailParentLabel
            //
            this.detailParentLabel.Location = new System.Drawing.Point(444, 41);
            this.detailParentLabel.Name = "detailParentLabel";
            this.detailParentLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.detailParentLabel.Size = new System.Drawing.Size(200, 19);
            this.detailParentLabel.SizePx = 12.5F;
            this.detailParentLabel.TabIndex = 4;
            //
            // copyShaButton
            //
            this.copyShaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.copyShaButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.copyShaButton.Enabled = false;
            this.copyShaButton.Gap = 7;
            this.copyShaButton.IconRole = GitClient.Controls.TextRole.Primary;
            this.copyShaButton.IconSize = 14;
            this.copyShaButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"none\" stroke" +
                "=\"currentColor\" stroke-width=\"2\" d=\"M8 8h12v12H8z\"/><path fill=\"none\" stroke=\"cur" +
                "rentColor\" stroke-width=\"2\" d=\"M16 8V4H4v12h4\"/></svg>";
            this.copyShaButton.Location = new System.Drawing.Point(736, 24);
            this.copyShaButton.Name = "copyShaButton";
            this.copyShaButton.PaddingX = 12;
            this.copyShaButton.Size = new System.Drawing.Size(102, 30);
            this.copyShaButton.TabIndex = 5;
            this.copyShaButton.Text = "Copy sha";
            this.copyShaButton.Click += new System.EventHandler(this.copyShaButton_Click);
            //
            // revertButton
            //
            this.revertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.revertButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.revertButton.Enabled = false;
            this.revertButton.Gap = 7;
            this.revertButton.IconRole = GitClient.Controls.TextRole.Primary;
            this.revertButton.IconSize = 14;
            this.revertButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M14 5l-6 6 6 6v-4h2a4 4 0 0 1 0 8h-3v2h3a6 6 0 0 0 0-12h-2z\"/></svg>";
            this.revertButton.Location = new System.Drawing.Point(842, 24);
            this.revertButton.Name = "revertButton";
            this.revertButton.PaddingX = 12;
            this.revertButton.Size = new System.Drawing.Size(84, 30);
            this.revertButton.TabIndex = 6;
            this.revertButton.Text = "Revert";
            this.revertButton.Click += new System.EventHandler(this.revertButton_Click);
            //
            // rightColumn
            //
            this.rightColumn.Controls.Add(this.rightSplit);
            this.rightColumn.CornerRadius = 0;
            this.rightColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightColumn.Location = new System.Drawing.Point(0, 0);
            this.rightColumn.Name = "rightColumn";
            this.rightColumn.Size = new System.Drawing.Size(436, 634);
            this.rightColumn.Surface = GitClient.Controls.SurfaceKind.None;
            this.rightColumn.TabIndex = 1;
            //
            // changesCard
            //
            this.changesCard.Controls.Add(this.changesList);
            this.changesCard.Controls.Add(this.conflictBanner);
            this.changesCard.Controls.Add(this.changesHeader);
            this.changesCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.changesCard.Location = new System.Drawing.Point(0, 0);
            this.changesCard.Name = "changesCard";
            this.changesCard.Padding = new System.Windows.Forms.Padding(1);
            this.changesCard.Size = new System.Drawing.Size(436, 354);
            this.changesCard.TabIndex = 0;
            //
            // changesList
            //
            this.changesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.changesList.Location = new System.Drawing.Point(1, 45);
            this.changesList.Name = "changesList";
            this.changesList.Size = new System.Drawing.Size(422, 308);
            this.changesList.TabIndex = 2;
            this.changesList.SelectionChanged += new System.EventHandler(this.changesList_SelectionChanged);
            this.changesList.CheckedChanged += new System.EventHandler<GitClient.Controls.FileChangeEventArgs>(this.changesList_CheckedChanged);
            this.changesList.FileActivated += new System.EventHandler<GitClient.Controls.FileChangeEventArgs>(this.changesList_FileActivated);
            this.changesList.RowRightClick += new System.EventHandler<GitClient.Controls.RowMouseEventArgs>(this.changesList_RowRightClick);
            //
            // conflictBanner
            //
            this.conflictBanner.Controls.Add(this.conflictIcon);
            this.conflictBanner.Controls.Add(this.conflictTitleLabel);
            this.conflictBanner.Controls.Add(this.conflictBodyLabel);
            this.conflictBanner.Controls.Add(this.continueButton);
            this.conflictBanner.Controls.Add(this.abortButton);
            this.conflictBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.conflictBanner.Location = new System.Drawing.Point(1, 45);
            this.conflictBanner.Name = "conflictBanner";
            this.conflictBanner.Size = new System.Drawing.Size(422, 96);
            this.conflictBanner.Surface = GitClient.Controls.SurfaceKind.Custom;
            this.conflictBanner.TabIndex = 1;
            this.conflictBanner.Visible = false;
            //
            // conflictIcon
            //
            this.conflictIcon.IconGap = 0;
            this.conflictIcon.IconRole = GitClient.Controls.TextRole.Warning;
            this.conflictIcon.IconSize = 20;
            this.conflictIcon.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M12 2.4l10.4 18H1.6z\"/><path fill=\"#000\" fill-opacity=\"0.55\" d=\"M11 9h2v6h-2z" +
                "M11 16.2h2v2.2h-2z\"/></svg>";
            this.conflictIcon.Location = new System.Drawing.Point(16, 14);
            this.conflictIcon.Name = "conflictIcon";
            this.conflictIcon.Size = new System.Drawing.Size(20, 22);
            this.conflictIcon.TabIndex = 0;
            //
            // conflictTitleLabel
            //
            this.conflictTitleLabel.Location = new System.Drawing.Point(50, 13);
            this.conflictTitleLabel.Name = "conflictTitleLabel";
            this.conflictTitleLabel.Semibold = true;
            this.conflictTitleLabel.Size = new System.Drawing.Size(250, 22);
            this.conflictTitleLabel.SizePx = 14F;
            this.conflictTitleLabel.TabIndex = 1;
            this.conflictTitleLabel.Text = "Merge in progress";
            //
            // conflictBodyLabel
            //
            this.conflictBodyLabel.Location = new System.Drawing.Point(50, 37);
            this.conflictBodyLabel.MultiLine = true;
            this.conflictBodyLabel.Name = "conflictBodyLabel";
            this.conflictBodyLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.conflictBodyLabel.Size = new System.Drawing.Size(250, 46);
            this.conflictBodyLabel.SizePx = 13F;
            this.conflictBodyLabel.TabIndex = 2;
            this.conflictBodyLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // continueButton
            //
            this.continueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.continueButton.Appearance = GitClient.Controls.ButtonAppearance.Accent;
            this.continueButton.CornerRadius = 5;
            this.continueButton.Location = new System.Drawing.Point(302, 14);
            this.continueButton.Name = "continueButton";
            this.continueButton.PaddingX = 14;
            this.continueButton.Size = new System.Drawing.Size(106, 32);
            this.continueButton.TabIndex = 3;
            this.continueButton.Text = "Continue";
            this.continueButton.TextSizePx = 13.5F;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            //
            // abortButton
            //
            this.abortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.abortButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.abortButton.CornerRadius = 5;
            this.abortButton.Location = new System.Drawing.Point(302, 52);
            this.abortButton.Name = "abortButton";
            this.abortButton.PaddingX = 14;
            this.abortButton.Size = new System.Drawing.Size(106, 32);
            this.abortButton.TabIndex = 4;
            this.abortButton.Text = "Abort";
            this.abortButton.TextSizePx = 13.5F;
            this.abortButton.Click += new System.EventHandler(this.abortButton_Click);
            //
            // changesHeader
            //
            this.changesHeader.Controls.Add(this.changesTitleLabel);
            this.changesHeader.Controls.Add(this.changesCountChip);
            this.changesHeader.Controls.Add(this.stageAllButton);
            this.changesHeader.Controls.Add(this.claudeStageButton);
            this.changesHeader.Controls.Add(this.overflowButton);
            this.changesHeader.CornerRadius = 0;
            this.changesHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.changesHeader.Location = new System.Drawing.Point(1, 1);
            this.changesHeader.Name = "changesHeader";
            this.changesHeader.Size = new System.Drawing.Size(422, 44);
            this.changesHeader.Surface = GitClient.Controls.SurfaceKind.None;
            this.changesHeader.TabIndex = 0;
            //
            // changesTitleLabel
            //
            this.changesTitleLabel.Location = new System.Drawing.Point(13, 0);
            this.changesTitleLabel.Name = "changesTitleLabel";
            this.changesTitleLabel.Semibold = true;
            this.changesTitleLabel.Size = new System.Drawing.Size(72, 44);
            this.changesTitleLabel.SizePx = 14F;
            this.changesTitleLabel.TabIndex = 0;
            this.changesTitleLabel.Text = "Changes";
            //
            // changesCountChip
            //
            this.changesCountChip.Location = new System.Drawing.Point(88, 13);
            this.changesCountChip.Name = "changesCountChip";
            this.changesCountChip.Size = new System.Drawing.Size(20, 18);
            this.changesCountChip.TabIndex = 1;
            this.changesCountChip.Text = "0";
            //
            // stageAllButton
            //
            this.stageAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stageAllButton.Gap = 7;
            this.stageAllButton.IconSize = 14;
            this.stageAllButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M9 16.2l-3.5-3.5-1.4 1.4L9 19 20 8l-1.4-1.4z\"/></svg>";
            this.stageAllButton.Location = new System.Drawing.Point(302, 8);
            this.stageAllButton.Name = "stageAllButton";
            this.stageAllButton.Size = new System.Drawing.Size(84, 28);
            this.stageAllButton.TabIndex = 2;
            this.stageAllButton.Text = "Stage all";
            this.stageAllButton.Click += new System.EventHandler(this.stageAllButton_Click);
            //
            // claudeStageButton
            //
            this.claudeStageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.claudeStageButton.IconRole = GitClient.Controls.TextRole.Accent;
            this.claudeStageButton.IconSize = 14;
            this.claudeStageButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M12 2l2.2 6.3 6.3 2.2-6.3 2.2L12 19l-2.2-6.3-6.3-2.2 6.3-2.2z\"/><path fill=\"curre" +
                "ntColor\" d=\"M19 15l.9 2.6 2.6.9-2.6.9L19 22l-.9-2.6-2.6-.9 2.6-.9z\"/></svg>";
            this.claudeStageButton.Location = new System.Drawing.Point(270, 8);
            this.claudeStageButton.Name = "claudeStageButton";
            this.claudeStageButton.Gap = 7;
            this.claudeStageButton.Size = new System.Drawing.Size(110, 28);
            this.claudeStageButton.TabIndex = 4;
            this.claudeStageButton.Text = "Smart stage";
            this.claudeStageButton.ToolTipText = "Stage with Claude: describe what to stage, or split everything into commits";
            this.claudeStageButton.Click += new System.EventHandler(this.claudeStageButton_Click);
            //
            // overflowButton
            //
            this.overflowButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.overflowButton.DropDownMenu = this.overflowMenu;
            this.overflowButton.IconRole = GitClient.Controls.TextRole.Tertiary;
            this.overflowButton.IconSize = 14;
            this.overflowButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><circle fill=\"currentColo" +
                "r\" cx=\"12\" cy=\"5\" r=\"1.9\"/><circle fill=\"currentColor\" cx=\"12\" cy=\"12\" r=\"1." +
                "9\"/><circle fill=\"currentColor\" cx=\"12\" cy=\"19\" r=\"1.9\"/></svg>";
            this.overflowButton.Location = new System.Drawing.Point(390, 8);
            this.overflowButton.Name = "overflowButton";
            this.overflowButton.PaddingX = 7;
            this.overflowButton.Size = new System.Drawing.Size(28, 28);
            this.overflowButton.TabIndex = 3;
            //
            // rightSplit
            //
            this.rightSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.rightSplit.Location = new System.Drawing.Point(0, 0);
            this.rightSplit.Name = "rightSplit";
            this.rightSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.rightSplit.Panel1.Controls.Add(this.changesCard);
            this.rightSplit.Panel1MinSize = 120;
            this.rightSplit.Panel2.Controls.Add(this.composerCard);
            this.rightSplit.Panel2MinSize = 200;
            this.rightSplit.Size = new System.Drawing.Size(436, 634);
            this.rightSplit.SplitterDistance = 354;
            this.rightSplit.SplitterWidth = 12;
            this.rightSplit.TabIndex = 0;
            this.rightSplit.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.split_SplitterMoved);
            //
            // composerCard
            //
            this.composerCard.Controls.Add(this.commitToLabel);
            this.composerCard.Controls.Add(this.branchChip);
            this.composerCard.Controls.Add(this.amendLabel);
            this.composerCard.Controls.Add(this.amendSwitch);
            this.composerCard.Controls.Add(this.fieldGroup);
            this.composerCard.Controls.Add(this.commitButton);
            this.composerCard.Controls.Add(this.commitPushButton);
            this.composerCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.composerCard.Location = new System.Drawing.Point(0, 0);
            this.composerCard.Name = "composerCard";
            this.composerCard.Size = new System.Drawing.Size(436, 268);
            this.composerCard.TabIndex = 2;
            //
            // commitToLabel
            //
            this.commitToLabel.Location = new System.Drawing.Point(14, 14);
            this.commitToLabel.Name = "commitToLabel";
            this.commitToLabel.Semibold = true;
            this.commitToLabel.Size = new System.Drawing.Size(76, 22);
            this.commitToLabel.SizePx = 14F;
            this.commitToLabel.TabIndex = 0;
            this.commitToLabel.Text = "Commit to";
            //
            // branchChip
            //
            this.branchChip.CornerRadius = 4;
            this.branchChip.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 16 16\"><path fill=\"none\" stroke" +
                "=\"currentColor\" stroke-width=\"1.6\" d=\"M3 3.5h10v7H3z\"/><path fill=\"currentColor\" d" +
                "=\"M1 12h14v1.5H1z\"/></svg>";
            this.branchChip.Location = new System.Drawing.Point(96, 14);
            this.branchChip.Name = "branchChip";
            this.branchChip.PaddingX = 8;
            this.branchChip.Size = new System.Drawing.Size(80, 22);
            this.branchChip.Style = GitClient.Controls.ChipStyle.Lane;
            this.branchChip.TabIndex = 1;
            this.branchChip.Text = "master";
            this.branchChip.TextSizePx = 12F;
            //
            // amendLabel
            //
            this.amendLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.amendLabel.Location = new System.Drawing.Point(300, 14);
            this.amendLabel.Name = "amendLabel";
            this.amendLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.amendLabel.Size = new System.Drawing.Size(56, 22);
            this.amendLabel.SizePx = 12.5F;
            this.amendLabel.TabIndex = 2;
            this.amendLabel.Text = "Amend";
            this.amendLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // amendSwitch
            //
            this.amendSwitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.amendSwitch.Location = new System.Drawing.Point(368, 15);
            this.amendSwitch.Name = "amendSwitch";
            this.amendSwitch.Size = new System.Drawing.Size(40, 20);
            this.amendSwitch.TabIndex = 3;
            this.amendSwitch.CheckedChanged += new System.EventHandler(this.amendSwitch_CheckedChanged);
            //
            // fieldGroup
            //
            this.fieldGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fieldGroup.Controls.Add(this.descriptionPlaceholder);
            this.fieldGroup.Controls.Add(this.descriptionBox);
            this.fieldGroup.Controls.Add(this.hintRow);
            this.fieldGroup.Controls.Add(this.summaryRow);
            this.fieldGroup.CornerRadius = 6;
            this.fieldGroup.Location = new System.Drawing.Point(14, 46);
            this.fieldGroup.Name = "fieldGroup";
            this.fieldGroup.Padding = new System.Windows.Forms.Padding(1);
            this.fieldGroup.Size = new System.Drawing.Size(396, 162);
            this.fieldGroup.Surface = GitClient.Controls.SurfaceKind.Fill;
            this.fieldGroup.TabIndex = 4;
            //
            // descriptionBox
            //
            this.descriptionBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descriptionBox.Location = new System.Drawing.Point(1, 37);
            this.descriptionBox.Multiline = true;
            this.descriptionBox.Name = "descriptionBox";
            this.descriptionBox.PlaceholderText = "Description (optional)";
            this.descriptionBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.descriptionBox.Size = new System.Drawing.Size(394, 98);
            this.descriptionBox.TabIndex = 1;
            this.descriptionBox.TextChanged += new System.EventHandler(this.descriptionBox_TextChanged);
            this.descriptionBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.commitBox_KeyDown);
            //
            // descriptionPlaceholder
            //
            this.descriptionPlaceholder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionPlaceholder.Location = new System.Drawing.Point(13, 47);
            this.descriptionPlaceholder.Name = "descriptionPlaceholder";
            this.descriptionPlaceholder.Role = GitClient.Controls.TextRole.Tertiary;
            this.descriptionPlaceholder.Size = new System.Drawing.Size(370, 19);
            this.descriptionPlaceholder.SizePx = 13.5F;
            this.descriptionPlaceholder.TabIndex = 3;
            this.descriptionPlaceholder.Text = "Description (optional)";
            this.descriptionPlaceholder.Click += new System.EventHandler(this.descriptionPlaceholder_Click);
            //
            // hintRow
            //
            this.hintRow.Controls.Add(this.hintLabel);
            this.hintRow.CornerRadius = 0;
            this.hintRow.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hintRow.Location = new System.Drawing.Point(1, 135);
            this.hintRow.Name = "hintRow";
            this.hintRow.Size = new System.Drawing.Size(394, 26);
            this.hintRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.hintRow.TabIndex = 2;
            this.hintRow.TopDivider = true;
            //
            // hintLabel
            //
            this.hintLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hintLabel.Location = new System.Drawing.Point(0, 0);
            this.hintLabel.Name = "hintLabel";
            this.hintLabel.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.hintLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.hintLabel.Size = new System.Drawing.Size(394, 26);
            this.hintLabel.SizePx = 11.5F;
            this.hintLabel.TabIndex = 0;
            this.hintLabel.Text = "Ctrl+Enter to commit";
            //
            // summaryRow
            //
            this.summaryRow.BottomDivider = true;
            this.summaryRow.Controls.Add(this.summaryBox);
            this.summaryRow.Controls.Add(this.aiButton);
            this.summaryRow.CornerRadius = 0;
            this.summaryRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.summaryRow.Location = new System.Drawing.Point(1, 1);
            this.summaryRow.Name = "summaryRow";
            this.summaryRow.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.summaryRow.Size = new System.Drawing.Size(394, 36);
            this.summaryRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.summaryRow.TabIndex = 0;
            //
            // summaryBox
            //
            this.summaryBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.summaryBox.Location = new System.Drawing.Point(0, 0);
            this.summaryBox.Name = "summaryBox";
            this.summaryBox.PlaceholderText = "Summary";
            this.summaryBox.Size = new System.Drawing.Size(228, 35);
            this.summaryBox.TabIndex = 0;
            this.summaryBox.TextChanged += new System.EventHandler(this.summaryBox_TextChanged);
            this.summaryBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.commitBox_KeyDown);
            //
            // aiButton
            //
            this.aiButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.aiButton.Gap = 6;
            this.aiButton.IconRole = GitClient.Controls.TextRole.Accent;
            this.aiButton.IconSize = 13;
            this.aiButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M12 2l2.2 6.3 6.3 2.2-6.3 2.2L12 19l-2.2-6.3-6.3-2.2 6.3-2.2z\"/><path fill=\"curre" +
                "ntColor\" d=\"M19 15l.9 2.6 2.6.9-2.6.9L19 22l-.9-2.6-2.6-.9 2.6-.9z\"/></svg>";
            this.aiButton.Location = new System.Drawing.Point(228, 0);
            this.aiButton.Name = "aiButton";
            this.aiButton.PaddingX = 9;
            this.aiButton.Size = new System.Drawing.Size(166, 36);
            this.aiButton.TabIndex = 1;
            this.aiButton.Text = "Write with Claude";
            this.aiButton.TextSizePx = 12.5F;
            this.aiButton.ToolTipText = "Ask Claude Code to write the commit message";
            this.aiButton.Click += new System.EventHandler(this.aiButton_Click);
            //
            // commitButton
            //
            this.commitButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commitButton.Appearance = GitClient.Controls.ButtonAppearance.Accent;
            this.commitButton.CornerRadius = 5;
            this.commitButton.Enabled = false;
            this.commitButton.IconSize = 15;
            this.commitButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M9 16.2l-3.5-3.5-1.4 1.4L9 19 20 8l-1.4-1.4z\"/></svg>";
            this.commitButton.Location = new System.Drawing.Point(14, 216);
            this.commitButton.Name = "commitButton";
            this.commitButton.Size = new System.Drawing.Size(260, 38);
            this.commitButton.TabIndex = 5;
            this.commitButton.Text = "Commit";
            this.commitButton.TextSizePx = 14F;
            this.commitButton.Click += new System.EventHandler(this.commitButton_Click);
            //
            // commitPushButton
            //
            this.commitPushButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.commitPushButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.commitPushButton.CornerRadius = 5;
            this.commitPushButton.Enabled = false;
            this.commitPushButton.Location = new System.Drawing.Point(282, 216);
            this.commitPushButton.Name = "commitPushButton";
            this.commitPushButton.Size = new System.Drawing.Size(128, 38);
            this.commitPushButton.TabIndex = 6;
            this.commitPushButton.Text = "Commit & push";
            this.commitPushButton.TextSizePx = 13.5F;
            this.commitPushButton.Click += new System.EventHandler(this.commitPushButton_Click);
            //
            // RepositoryView
            //
            this.Controls.Add(this.body);
            this.Controls.Add(this.commandBar);
            this.Controls.Add(this.outputPanel);
            this.Controls.Add(this.statusBar);
            this.Name = "RepositoryView";
            this.Size = new System.Drawing.Size(1400, 900);
            this.commandBar.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.outputPanel.ResumeLayout(false);
            this.body.ResumeLayout(false);
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.mainSplit.ResumeLayout(false);
            this.historySplit.Panel1.ResumeLayout(false);
            this.historySplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.historySplit)).EndInit();
            this.historySplit.ResumeLayout(false);
            this.detailSplit.Panel1.ResumeLayout(false);
            this.detailSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.detailSplit)).EndInit();
            this.detailSplit.ResumeLayout(false);
            this.rightSplit.Panel1.ResumeLayout(false);
            this.rightSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rightSplit)).EndInit();
            this.rightSplit.ResumeLayout(false);
            this.historyCard.ResumeLayout(false);
            this.historyHeader.ResumeLayout(false);
            this.detailCard.ResumeLayout(false);
            this.detailBody.ResumeLayout(false);
            this.diffColumn.ResumeLayout(false);
            this.diffHeader.ResumeLayout(false);
            this.commitFilesPanel.ResumeLayout(false);
            this.detailHeader.ResumeLayout(false);
            this.rightColumn.ResumeLayout(false);
            this.changesCard.ResumeLayout(false);
            this.conflictBanner.ResumeLayout(false);
            this.changesHeader.ResumeLayout(false);
            this.composerCard.ResumeLayout(false);
            this.fieldGroup.ResumeLayout(false);
            this.hintRow.ResumeLayout(false);
            this.summaryRow.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernContextMenu pullMenu;
        private ModernWinForms.ModernContextMenuItem pullDefaultItem;
        private ModernWinForms.ModernContextMenuItem pullFastForwardItem;
        private ModernWinForms.ModernContextMenuItem pullRebaseItem;
        private ModernWinForms.ModernContextMenu pushMenu;
        private ModernWinForms.ModernContextMenuItem pushDefaultItem;
        private ModernWinForms.ModernContextMenuItem pushUpstreamItem;
        private ModernWinForms.ModernContextMenuItem pushForceItem;
        private ModernWinForms.ModernContextMenu stashMenu;
        private ModernWinForms.ModernContextMenuItem stashPushItem;
        private ModernWinForms.ModernContextMenuItem stashPushUntrackedItem;
        private ModernWinForms.ModernContextMenu overflowMenu;
        private ModernWinForms.ModernContextMenuItem unstageAllItem;
        private ModernWinForms.ModernContextMenuItem discardAllItem;
        private ModernWinForms.ModernContextMenuItem stashSelectedItem;
        private ModernWinForms.ModernContextMenuItem refreshItem;
        private ModernWinForms.ModernContextMenuItem editIgnoreItem;
        private ModernWinForms.ModernContextMenuItem remotesItem;
        private ModernWinForms.ModernContextMenuItem createOnGitHubItem;
        private ModernWinForms.ModernContextMenuItem githubAccountItem;
        private GitClient.Controls.SurfacePanel commandBar;
        private GitClient.Controls.CommandButton branchButton;
        private GitClient.Controls.SurfacePanel commandSeparator1;
        private GitClient.Controls.CommandButton fetchButton;
        private GitClient.Controls.CommandButton pullButton;
        private GitClient.Controls.CommandButton pushButton;
        private GitClient.Controls.SurfacePanel commandSeparator2;
        private GitClient.Controls.CommandButton newBranchButton;
        private GitClient.Controls.CommandButton stashButton;
        private ModernWinForms.ModernTextBox filterBox;
        private GitClient.Controls.CommandButton terminalButton;
        private GitClient.Controls.CommandButton logButton;
        private GitClient.Controls.CommandButton settingsButton;
        private GitClient.Controls.SurfacePanel statusBar;
        private GitClient.Controls.TextLabel statusBranchLabel;
        private GitClient.Controls.TextLabel statusUpstreamLabel;
        private GitClient.Controls.TextLabel statusCountLabel;
        private GitClient.Controls.TextLabel statusMessageLabel;
        private ModernWinForms.ModernProgressBar progressBar;
        private GitClient.Controls.SurfacePanel outputPanel;
        private ModernWinForms.ModernTextBox outputBox;
        private GitClient.Controls.SurfacePanel body;
        private ModernWinForms.ModernSplitContainer mainSplit;
        private ModernWinForms.ModernSplitContainer historySplit;
        private ModernWinForms.ModernSplitContainer detailSplit;
        private ModernWinForms.ModernSplitContainer rightSplit;
        private GitClient.Controls.SurfacePanel historyCard;
        private GitClient.Controls.SurfacePanel historyHeader;
        private GitClient.Controls.TextLabel historyTitleLabel;
        private GitClient.Controls.TextLabel historyCountLabel;
        private GitClient.Controls.CommandButton scopeButton;
        private GitClient.Controls.HistoryListControl historyList;
        private GitClient.Controls.SurfacePanel detailCard;
        private GitClient.Controls.SurfacePanel detailHeader;
        private GitClient.Controls.AvatarBox detailAvatar;
        private GitClient.Controls.TextLabel detailSubjectLabel;
        private GitClient.Controls.TextLabel detailMetaLabel;
        private GitClient.Controls.Chip detailShaChip;
        private GitClient.Controls.TextLabel detailParentLabel;
        private GitClient.Controls.CommandButton copyShaButton;
        private GitClient.Controls.CommandButton revertButton;
        private GitClient.Controls.SurfacePanel detailBody;
        private GitClient.Controls.SurfacePanel commitFilesPanel;
        private GitClient.Controls.FileListControl commitFilesList;
        private GitClient.Controls.SurfacePanel diffColumn;
        private GitClient.Controls.SurfacePanel diffHeader;
        private GitClient.Controls.TextLabel diffPathLabel;
        private GitClient.Controls.SegmentedControl diffLayoutToggle;
        private GitClient.Controls.DiffViewControl diffView;
        private GitClient.Controls.SurfacePanel rightColumn;
        private GitClient.Controls.SurfacePanel changesCard;
        private GitClient.Controls.SurfacePanel changesHeader;
        private GitClient.Controls.TextLabel changesTitleLabel;
        private GitClient.Controls.Chip changesCountChip;
        private GitClient.Controls.CommandButton stageAllButton;
        private GitClient.Controls.CommandButton claudeStageButton;
        private ModernWinForms.ModernContextMenuItem claudeStageItem;
        private ModernWinForms.ModernContextMenuItem claudeSplitItem;
        private GitClient.Controls.CommandButton overflowButton;
        private GitClient.Controls.SurfacePanel conflictBanner;
        private GitClient.Controls.TextLabel conflictIcon;
        private GitClient.Controls.TextLabel conflictTitleLabel;
        private GitClient.Controls.TextLabel conflictBodyLabel;
        private GitClient.Controls.CommandButton continueButton;
        private GitClient.Controls.CommandButton abortButton;
        private GitClient.Controls.ChangesListControl changesList;
        private GitClient.Controls.SurfacePanel composerCard;
        private GitClient.Controls.TextLabel commitToLabel;
        private GitClient.Controls.Chip branchChip;
        private GitClient.Controls.TextLabel amendLabel;
        private GitClient.Controls.ToggleSwitchControl amendSwitch;
        private GitClient.Controls.SurfacePanel fieldGroup;
        private GitClient.Controls.SurfacePanel summaryRow;
        private ModernWinForms.ModernTextBox summaryBox;
        private GitClient.Controls.CommandButton aiButton;
        private ModernWinForms.ModernTextBox descriptionBox;
        private GitClient.Controls.TextLabel descriptionPlaceholder;
        private GitClient.Controls.SurfacePanel hintRow;
        private GitClient.Controls.TextLabel hintLabel;
        private GitClient.Controls.CommandButton commitButton;
        private GitClient.Controls.CommandButton commitPushButton;
    }
}
