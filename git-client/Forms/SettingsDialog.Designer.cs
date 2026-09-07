namespace GitClient.Forms
{
    partial class SettingsDialog
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
            this.navRail = new GitClient.Controls.NavRailControl();
            this.contentPanel = new GitClient.Controls.SurfacePanel();
            this.pageTitleLabel = new GitClient.Controls.TextLabel();
            this.identityPage = new GitClient.Controls.SurfacePanel();
            this.identityGroup = new GitClient.Controls.SurfacePanel();
            this.nameRow = new GitClient.Controls.SurfacePanel();
            this.nameLabel = new GitClient.Controls.TextLabel();
            this.nameHint = new GitClient.Controls.TextLabel();
            this.userNameBox = new ModernWinForms.ModernTextBox();
            this.emailRow = new GitClient.Controls.SurfacePanel();
            this.emailLabel = new GitClient.Controls.TextLabel();
            this.emailBox = new ModernWinForms.ModernTextBox();
            this.toolsPage = new GitClient.Controls.SurfacePanel();
            this.toolsGroup = new GitClient.Controls.SurfacePanel();
            this.gitRow = new GitClient.Controls.SurfacePanel();
            this.gitLabel = new GitClient.Controls.TextLabel();
            this.gitHint = new GitClient.Controls.TextLabel();
            this.gitPathBox = new ModernWinForms.ModernTextBox();
            this.claudeRow = new GitClient.Controls.SurfacePanel();
            this.claudeLabel = new GitClient.Controls.TextLabel();
            this.claudeHint = new GitClient.Controls.TextLabel();
            this.claudePathBox = new ModernWinForms.ModernTextBox();
            this.modelRow = new GitClient.Controls.SurfacePanel();
            this.modelLabel = new GitClient.Controls.TextLabel();
            this.modelHint = new GitClient.Controls.TextLabel();
            this.modelBox = new ModernWinForms.ModernTextBox();
            this.commitsRow = new GitClient.Controls.SurfacePanel();
            this.commitsLabel = new GitClient.Controls.TextLabel();
            this.commitsHint = new GitClient.Controls.TextLabel();
            this.maxCommitsBox = new ModernWinForms.ModernNumericUpDown();
            this.historyPage = new GitClient.Controls.SurfacePanel();
            this.historyGroup = new GitClient.Controls.SurfacePanel();
            this.rowHeightRow = new GitClient.Controls.SurfacePanel();
            this.rowHeightLabel = new GitClient.Controls.TextLabel();
            this.rowHeightHint = new GitClient.Controls.TextLabel();
            this.rowHeightBox = new ModernWinForms.ModernNumericUpDown();
            this.relativeDatesRow = new GitClient.Controls.SurfacePanel();
            this.relativeDatesLabel = new GitClient.Controls.TextLabel();
            this.relativeDatesHint = new GitClient.Controls.TextLabel();
            this.relativeDatesSwitch = new GitClient.Controls.ToggleSwitchControl();
            this.appearancePage = new GitClient.Controls.SurfacePanel();
            this.appearanceGroup = new GitClient.Controls.SurfacePanel();
            this.themeRow = new GitClient.Controls.SurfacePanel();
            this.themeLabel = new GitClient.Controls.TextLabel();
            this.themeHint = new GitClient.Controls.TextLabel();
            this.themeToggle = new GitClient.Controls.SegmentedControl();
            this.saveButton = new GitClient.Controls.CommandButton();
            this.cancelButton = new GitClient.Controls.CommandButton();
            this.contentPanel.SuspendLayout();
            this.identityPage.SuspendLayout();
            this.identityGroup.SuspendLayout();
            this.nameRow.SuspendLayout();
            this.emailRow.SuspendLayout();
            this.toolsPage.SuspendLayout();
            this.toolsGroup.SuspendLayout();
            this.gitRow.SuspendLayout();
            this.claudeRow.SuspendLayout();
            this.modelRow.SuspendLayout();
            this.commitsRow.SuspendLayout();
            this.historyPage.SuspendLayout();
            this.historyGroup.SuspendLayout();
            this.rowHeightRow.SuspendLayout();
            this.relativeDatesRow.SuspendLayout();
            this.appearancePage.SuspendLayout();
            this.appearanceGroup.SuspendLayout();
            this.themeRow.SuspendLayout();
            this.SuspendLayout();
            //
            // navRail
            //
            this.navRail.Dock = System.Windows.Forms.DockStyle.Left;
            this.navRail.Items.Add("Identity");
            this.navRail.Items.Add("Tools & paths");
            this.navRail.Items.Add("History");
            this.navRail.Items.Add("Appearance");
            this.navRail.Location = new System.Drawing.Point(0, 31);
            this.navRail.Name = "navRail";
            this.navRail.Size = new System.Drawing.Size(188, 589);
            this.navRail.TabIndex = 0;
            this.navRail.SelectionChanged += new System.EventHandler(this.navRail_SelectionChanged);
            //
            // contentPanel
            //
            this.contentPanel.Controls.Add(this.identityPage);
            this.contentPanel.Controls.Add(this.toolsPage);
            this.contentPanel.Controls.Add(this.historyPage);
            this.contentPanel.Controls.Add(this.appearancePage);
            this.contentPanel.Controls.Add(this.pageTitleLabel);
            this.contentPanel.Controls.Add(this.saveButton);
            this.contentPanel.Controls.Add(this.cancelButton);
            this.contentPanel.CornerRadius = 0;
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(188, 31);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(532, 589);
            this.contentPanel.Surface = GitClient.Controls.SurfaceKind.Base;
            this.contentPanel.TabIndex = 1;
            //
            // pageTitleLabel
            //
            this.pageTitleLabel.Location = new System.Drawing.Point(8, 14);
            this.pageTitleLabel.Name = "pageTitleLabel";
            this.pageTitleLabel.Semibold = true;
            this.pageTitleLabel.Size = new System.Drawing.Size(400, 28);
            this.pageTitleLabel.SizePx = 20F;
            this.pageTitleLabel.TabIndex = 0;
            this.pageTitleLabel.Text = "Identity";
            //
            // identityPage
            //
            this.identityPage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.identityPage.Controls.Add(this.identityGroup);
            this.identityPage.CornerRadius = 0;
            this.identityPage.Location = new System.Drawing.Point(8, 52);
            this.identityPage.Name = "identityPage";
            this.identityPage.Size = new System.Drawing.Size(504, 400);
            this.identityPage.Surface = GitClient.Controls.SurfaceKind.None;
            this.identityPage.TabIndex = 1;
            //
            // identityGroup
            //
            this.identityGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.identityGroup.Controls.Add(this.emailRow);
            this.identityGroup.Controls.Add(this.nameRow);
            this.identityGroup.CornerRadius = 7;
            this.identityGroup.Location = new System.Drawing.Point(0, 0);
            this.identityGroup.Name = "identityGroup";
            this.identityGroup.Padding = new System.Windows.Forms.Padding(1);
            this.identityGroup.Size = new System.Drawing.Size(504, 118);
            this.identityGroup.TabIndex = 0;
            //
            // nameRow
            //
            this.nameRow.BottomDivider = true;
            this.nameRow.Controls.Add(this.nameLabel);
            this.nameRow.Controls.Add(this.nameHint);
            this.nameRow.Controls.Add(this.userNameBox);
            this.nameRow.CornerRadius = 0;
            this.nameRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.nameRow.Location = new System.Drawing.Point(1, 1);
            this.nameRow.Name = "nameRow";
            this.nameRow.Size = new System.Drawing.Size(502, 58);
            this.nameRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.nameRow.TabIndex = 0;
            //
            // nameLabel
            //
            this.nameLabel.Location = new System.Drawing.Point(16, 12);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(200, 19);
            this.nameLabel.SizePx = 13.5F;
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Name";
            //
            // nameHint
            //
            this.nameHint.Location = new System.Drawing.Point(16, 31);
            this.nameHint.Name = "nameHint";
            this.nameHint.Role = GitClient.Controls.TextRole.Tertiary;
            this.nameHint.Size = new System.Drawing.Size(200, 17);
            this.nameHint.SizePx = 12F;
            this.nameHint.TabIndex = 1;
            this.nameHint.Text = "Used as the author on every commit";
            //
            // userNameBox
            //
            this.userNameBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.userNameBox.Location = new System.Drawing.Point(206, 13);
            this.userNameBox.Name = "userNameBox";
            this.userNameBox.Size = new System.Drawing.Size(280, 32);
            this.userNameBox.TabIndex = 2;
            //
            // emailRow
            //
            this.emailRow.Controls.Add(this.emailLabel);
            this.emailRow.Controls.Add(this.emailBox);
            this.emailRow.CornerRadius = 0;
            this.emailRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.emailRow.Location = new System.Drawing.Point(1, 59);
            this.emailRow.Name = "emailRow";
            this.emailRow.Size = new System.Drawing.Size(502, 58);
            this.emailRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.emailRow.TabIndex = 1;
            //
            // emailLabel
            //
            this.emailLabel.Location = new System.Drawing.Point(16, 0);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(200, 58);
            this.emailLabel.SizePx = 13.5F;
            this.emailLabel.TabIndex = 0;
            this.emailLabel.Text = "Email";
            //
            // emailBox
            //
            this.emailBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.emailBox.Location = new System.Drawing.Point(206, 13);
            this.emailBox.Name = "emailBox";
            this.emailBox.Size = new System.Drawing.Size(280, 32);
            this.emailBox.TabIndex = 1;
            //
            // toolsPage
            //
            this.toolsPage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolsPage.Controls.Add(this.toolsGroup);
            this.toolsPage.CornerRadius = 0;
            this.toolsPage.Location = new System.Drawing.Point(8, 52);
            this.toolsPage.Name = "toolsPage";
            this.toolsPage.Size = new System.Drawing.Size(504, 400);
            this.toolsPage.Surface = GitClient.Controls.SurfaceKind.None;
            this.toolsPage.TabIndex = 2;
            this.toolsPage.Visible = false;
            //
            // toolsGroup
            //
            this.toolsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolsGroup.Controls.Add(this.commitsRow);
            this.toolsGroup.Controls.Add(this.modelRow);
            this.toolsGroup.Controls.Add(this.claudeRow);
            this.toolsGroup.Controls.Add(this.gitRow);
            this.toolsGroup.CornerRadius = 7;
            this.toolsGroup.Location = new System.Drawing.Point(0, 0);
            this.toolsGroup.Name = "toolsGroup";
            this.toolsGroup.Padding = new System.Windows.Forms.Padding(1);
            this.toolsGroup.Size = new System.Drawing.Size(504, 234);
            this.toolsGroup.TabIndex = 0;
            //
            // gitRow
            //
            this.gitRow.BottomDivider = true;
            this.gitRow.Controls.Add(this.gitLabel);
            this.gitRow.Controls.Add(this.gitHint);
            this.gitRow.Controls.Add(this.gitPathBox);
            this.gitRow.CornerRadius = 0;
            this.gitRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.gitRow.Location = new System.Drawing.Point(1, 1);
            this.gitRow.Name = "gitRow";
            this.gitRow.Size = new System.Drawing.Size(502, 58);
            this.gitRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.gitRow.TabIndex = 0;
            //
            // gitLabel
            //
            this.gitLabel.Location = new System.Drawing.Point(16, 12);
            this.gitLabel.Name = "gitLabel";
            this.gitLabel.Size = new System.Drawing.Size(200, 19);
            this.gitLabel.SizePx = 13.5F;
            this.gitLabel.TabIndex = 0;
            this.gitLabel.Text = "git.exe";
            //
            // gitHint
            //
            this.gitHint.Location = new System.Drawing.Point(16, 31);
            this.gitHint.Name = "gitHint";
            this.gitHint.Role = GitClient.Controls.TextRole.Tertiary;
            this.gitHint.Size = new System.Drawing.Size(210, 17);
            this.gitHint.SizePx = 12F;
            this.gitHint.TabIndex = 1;
            this.gitHint.Text = "Leave empty to use the copy found on PATH";
            //
            // gitPathBox
            //
            this.gitPathBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gitPathBox.Location = new System.Drawing.Point(206, 13);
            this.gitPathBox.Name = "gitPathBox";
            this.gitPathBox.PlaceholderText = "found on PATH";
            this.gitPathBox.Size = new System.Drawing.Size(280, 32);
            this.gitPathBox.TabIndex = 2;
            //
            // claudeRow
            //
            this.claudeRow.BottomDivider = true;
            this.claudeRow.Controls.Add(this.claudeLabel);
            this.claudeRow.Controls.Add(this.claudeHint);
            this.claudeRow.Controls.Add(this.claudePathBox);
            this.claudeRow.CornerRadius = 0;
            this.claudeRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.claudeRow.Location = new System.Drawing.Point(1, 59);
            this.claudeRow.Name = "claudeRow";
            this.claudeRow.Size = new System.Drawing.Size(502, 58);
            this.claudeRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.claudeRow.TabIndex = 1;
            //
            // claudeLabel
            //
            this.claudeLabel.Location = new System.Drawing.Point(16, 12);
            this.claudeLabel.Name = "claudeLabel";
            this.claudeLabel.Size = new System.Drawing.Size(200, 19);
            this.claudeLabel.SizePx = 13.5F;
            this.claudeLabel.TabIndex = 0;
            this.claudeLabel.Text = "claude.exe";
            //
            // claudeHint
            //
            this.claudeHint.Location = new System.Drawing.Point(16, 31);
            this.claudeHint.Name = "claudeHint";
            this.claudeHint.Role = GitClient.Controls.TextRole.Tertiary;
            this.claudeHint.Size = new System.Drawing.Size(200, 17);
            this.claudeHint.SizePx = 12F;
            this.claudeHint.TabIndex = 1;
            this.claudeHint.Text = "Used by Write with Claude";
            //
            // claudePathBox
            //
            this.claudePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.claudePathBox.Location = new System.Drawing.Point(206, 13);
            this.claudePathBox.Name = "claudePathBox";
            this.claudePathBox.PlaceholderText = "found on PATH";
            this.claudePathBox.Size = new System.Drawing.Size(280, 32);
            this.claudePathBox.TabIndex = 2;
            //
            // modelRow
            //
            this.modelRow.BottomDivider = true;
            this.modelRow.Controls.Add(this.modelLabel);
            this.modelRow.Controls.Add(this.modelHint);
            this.modelRow.Controls.Add(this.modelBox);
            this.modelRow.CornerRadius = 0;
            this.modelRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.modelRow.Location = new System.Drawing.Point(1, 117);
            this.modelRow.Name = "modelRow";
            this.modelRow.Size = new System.Drawing.Size(502, 58);
            this.modelRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.modelRow.TabIndex = 2;
            //
            // modelLabel
            //
            this.modelLabel.Location = new System.Drawing.Point(16, 12);
            this.modelLabel.Name = "modelLabel";
            this.modelLabel.Size = new System.Drawing.Size(200, 19);
            this.modelLabel.SizePx = 13.5F;
            this.modelLabel.TabIndex = 0;
            this.modelLabel.Text = "Claude model";
            //
            // modelHint
            //
            this.modelHint.Location = new System.Drawing.Point(16, 31);
            this.modelHint.Name = "modelHint";
            this.modelHint.Role = GitClient.Controls.TextRole.Tertiary;
            this.modelHint.Size = new System.Drawing.Size(200, 17);
            this.modelHint.SizePx = 12F;
            this.modelHint.TabIndex = 1;
            this.modelHint.Text = "Model used for commit messages";
            //
            // modelBox
            //
            this.modelBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.modelBox.Location = new System.Drawing.Point(206, 13);
            this.modelBox.Name = "modelBox";
            this.modelBox.PlaceholderText = "default";
            this.modelBox.Size = new System.Drawing.Size(280, 32);
            this.modelBox.TabIndex = 2;
            //
            // commitsRow
            //
            this.commitsRow.Controls.Add(this.commitsLabel);
            this.commitsRow.Controls.Add(this.commitsHint);
            this.commitsRow.Controls.Add(this.maxCommitsBox);
            this.commitsRow.CornerRadius = 0;
            this.commitsRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.commitsRow.Location = new System.Drawing.Point(1, 175);
            this.commitsRow.Name = "commitsRow";
            this.commitsRow.Size = new System.Drawing.Size(502, 58);
            this.commitsRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.commitsRow.TabIndex = 3;
            //
            // commitsLabel
            //
            this.commitsLabel.Location = new System.Drawing.Point(16, 12);
            this.commitsLabel.Name = "commitsLabel";
            this.commitsLabel.Size = new System.Drawing.Size(200, 19);
            this.commitsLabel.SizePx = 13.5F;
            this.commitsLabel.TabIndex = 0;
            this.commitsLabel.Text = "Commits to load";
            //
            // commitsHint
            //
            this.commitsHint.Location = new System.Drawing.Point(16, 31);
            this.commitsHint.Name = "commitsHint";
            this.commitsHint.Role = GitClient.Controls.TextRole.Tertiary;
            this.commitsHint.Size = new System.Drawing.Size(200, 17);
            this.commitsHint.SizePx = 12F;
            this.commitsHint.TabIndex = 1;
            this.commitsHint.Text = "Per repository, on open";
            //
            // maxCommitsBox
            //
            this.maxCommitsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maxCommitsBox.Location = new System.Drawing.Point(206, 13);
            this.maxCommitsBox.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.maxCommitsBox.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            this.maxCommitsBox.Name = "maxCommitsBox";
            this.maxCommitsBox.Size = new System.Drawing.Size(280, 32);
            this.maxCommitsBox.TabIndex = 2;
            this.maxCommitsBox.ThousandSeparator = "";
            //
            // historyPage
            //
            this.historyPage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.historyPage.Controls.Add(this.historyGroup);
            this.historyPage.CornerRadius = 0;
            this.historyPage.Location = new System.Drawing.Point(8, 52);
            this.historyPage.Name = "historyPage";
            this.historyPage.Size = new System.Drawing.Size(504, 400);
            this.historyPage.Surface = GitClient.Controls.SurfaceKind.None;
            this.historyPage.TabIndex = 3;
            this.historyPage.Visible = false;
            //
            // historyGroup
            //
            this.historyGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.historyGroup.Controls.Add(this.relativeDatesRow);
            this.historyGroup.Controls.Add(this.rowHeightRow);
            this.historyGroup.CornerRadius = 7;
            this.historyGroup.Location = new System.Drawing.Point(0, 0);
            this.historyGroup.Name = "historyGroup";
            this.historyGroup.Padding = new System.Windows.Forms.Padding(1);
            this.historyGroup.Size = new System.Drawing.Size(504, 118);
            this.historyGroup.TabIndex = 0;
            //
            // rowHeightRow
            //
            this.rowHeightRow.BottomDivider = true;
            this.rowHeightRow.Controls.Add(this.rowHeightLabel);
            this.rowHeightRow.Controls.Add(this.rowHeightHint);
            this.rowHeightRow.Controls.Add(this.rowHeightBox);
            this.rowHeightRow.CornerRadius = 0;
            this.rowHeightRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.rowHeightRow.Location = new System.Drawing.Point(1, 1);
            this.rowHeightRow.Name = "rowHeightRow";
            this.rowHeightRow.Size = new System.Drawing.Size(502, 58);
            this.rowHeightRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.rowHeightRow.TabIndex = 0;
            //
            // rowHeightLabel
            //
            this.rowHeightLabel.Location = new System.Drawing.Point(16, 12);
            this.rowHeightLabel.Name = "rowHeightLabel";
            this.rowHeightLabel.Size = new System.Drawing.Size(200, 19);
            this.rowHeightLabel.SizePx = 13.5F;
            this.rowHeightLabel.TabIndex = 0;
            this.rowHeightLabel.Text = "Row height";
            //
            // rowHeightHint
            //
            this.rowHeightHint.Location = new System.Drawing.Point(16, 31);
            this.rowHeightHint.Name = "rowHeightHint";
            this.rowHeightHint.Role = GitClient.Controls.TextRole.Tertiary;
            this.rowHeightHint.Size = new System.Drawing.Size(200, 17);
            this.rowHeightHint.SizePx = 12F;
            this.rowHeightHint.TabIndex = 1;
            this.rowHeightHint.Text = "History density, 30 to 48 pixels";
            //
            // rowHeightBox
            //
            this.rowHeightBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rowHeightBox.Location = new System.Drawing.Point(206, 13);
            this.rowHeightBox.Maximum = new decimal(new int[] { 48, 0, 0, 0 });
            this.rowHeightBox.Minimum = new decimal(new int[] { 30, 0, 0, 0 });
            this.rowHeightBox.Name = "rowHeightBox";
            this.rowHeightBox.Size = new System.Drawing.Size(280, 32);
            this.rowHeightBox.TabIndex = 2;
            this.rowHeightBox.ThousandSeparator = "";
            //
            // relativeDatesRow
            //
            this.relativeDatesRow.Controls.Add(this.relativeDatesLabel);
            this.relativeDatesRow.Controls.Add(this.relativeDatesHint);
            this.relativeDatesRow.Controls.Add(this.relativeDatesSwitch);
            this.relativeDatesRow.CornerRadius = 0;
            this.relativeDatesRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.relativeDatesRow.Location = new System.Drawing.Point(1, 59);
            this.relativeDatesRow.Name = "relativeDatesRow";
            this.relativeDatesRow.Size = new System.Drawing.Size(502, 58);
            this.relativeDatesRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.relativeDatesRow.TabIndex = 1;
            //
            // relativeDatesLabel
            //
            this.relativeDatesLabel.Location = new System.Drawing.Point(16, 12);
            this.relativeDatesLabel.Name = "relativeDatesLabel";
            this.relativeDatesLabel.Size = new System.Drawing.Size(200, 19);
            this.relativeDatesLabel.SizePx = 13.5F;
            this.relativeDatesLabel.TabIndex = 0;
            this.relativeDatesLabel.Text = "Relative dates";
            //
            // relativeDatesHint
            //
            this.relativeDatesHint.Location = new System.Drawing.Point(16, 31);
            this.relativeDatesHint.Name = "relativeDatesHint";
            this.relativeDatesHint.Role = GitClient.Controls.TextRole.Tertiary;
            this.relativeDatesHint.Size = new System.Drawing.Size(240, 17);
            this.relativeDatesHint.SizePx = 12F;
            this.relativeDatesHint.TabIndex = 1;
            this.relativeDatesHint.Text = "Show \"2 days ago\" instead of the date";
            //
            // relativeDatesSwitch
            //
            this.relativeDatesSwitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.relativeDatesSwitch.Location = new System.Drawing.Point(446, 19);
            this.relativeDatesSwitch.Name = "relativeDatesSwitch";
            this.relativeDatesSwitch.Size = new System.Drawing.Size(40, 20);
            this.relativeDatesSwitch.TabIndex = 2;
            //
            // appearancePage
            //
            this.appearancePage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appearancePage.Controls.Add(this.appearanceGroup);
            this.appearancePage.CornerRadius = 0;
            this.appearancePage.Location = new System.Drawing.Point(8, 52);
            this.appearancePage.Name = "appearancePage";
            this.appearancePage.Size = new System.Drawing.Size(504, 400);
            this.appearancePage.Surface = GitClient.Controls.SurfaceKind.None;
            this.appearancePage.TabIndex = 4;
            this.appearancePage.Visible = false;
            //
            // appearanceGroup
            //
            this.appearanceGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appearanceGroup.Controls.Add(this.themeRow);
            this.appearanceGroup.CornerRadius = 7;
            this.appearanceGroup.Location = new System.Drawing.Point(0, 0);
            this.appearanceGroup.Name = "appearanceGroup";
            this.appearanceGroup.Padding = new System.Windows.Forms.Padding(1);
            this.appearanceGroup.Size = new System.Drawing.Size(504, 60);
            this.appearanceGroup.TabIndex = 0;
            //
            // themeRow
            //
            this.themeRow.Controls.Add(this.themeLabel);
            this.themeRow.Controls.Add(this.themeHint);
            this.themeRow.Controls.Add(this.themeToggle);
            this.themeRow.CornerRadius = 0;
            this.themeRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.themeRow.Location = new System.Drawing.Point(1, 1);
            this.themeRow.Name = "themeRow";
            this.themeRow.Size = new System.Drawing.Size(502, 58);
            this.themeRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.themeRow.TabIndex = 0;
            //
            // themeLabel
            //
            this.themeLabel.Location = new System.Drawing.Point(16, 12);
            this.themeLabel.Name = "themeLabel";
            this.themeLabel.Size = new System.Drawing.Size(200, 19);
            this.themeLabel.SizePx = 13.5F;
            this.themeLabel.TabIndex = 0;
            this.themeLabel.Text = "Theme";
            //
            // themeHint
            //
            this.themeHint.Location = new System.Drawing.Point(16, 31);
            this.themeHint.Name = "themeHint";
            this.themeHint.Role = GitClient.Controls.TextRole.Tertiary;
            this.themeHint.Size = new System.Drawing.Size(220, 17);
            this.themeHint.SizePx = 12F;
            this.themeHint.TabIndex = 1;
            this.themeHint.Text = "Applies to every window straight away";
            //
            // themeToggle
            //
            this.themeToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.themeToggle.Items.Add("Dark");
            this.themeToggle.Items.Add("Light");
            this.themeToggle.Location = new System.Drawing.Point(376, 17);
            this.themeToggle.Name = "themeToggle";
            this.themeToggle.SegmentPadding = 16;
            this.themeToggle.Size = new System.Drawing.Size(110, 26);
            this.themeToggle.TabIndex = 2;
            this.themeToggle.TextSizePx = 13F;
            this.themeToggle.SelectedIndexChanged += new System.EventHandler(this.themeToggle_SelectedIndexChanged);
            //
            // saveButton
            //
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Appearance = GitClient.Controls.ButtonAppearance.Accent;
            this.saveButton.CornerRadius = 5;
            this.saveButton.Location = new System.Drawing.Point(300, 535);
            this.saveButton.Name = "saveButton";
            this.saveButton.PaddingX = 20;
            this.saveButton.Size = new System.Drawing.Size(96, 34);
            this.saveButton.TabIndex = 5;
            this.saveButton.Text = "Save";
            this.saveButton.TextSizePx = 13.5F;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            //
            // cancelButton
            //
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.cancelButton.CornerRadius = 5;
            this.cancelButton.Location = new System.Drawing.Point(404, 535);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.PaddingX = 20;
            this.cancelButton.Size = new System.Drawing.Size(102, 34);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextSizePx = 13.5F;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            //
            // SettingsDialog
            //
            this.ClientSize = new System.Drawing.Size(720, 620);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.navRail);
            this.MinimumSize = new System.Drawing.Size(640, 560);
            this.Name = "SettingsDialog";
            this.Skin = this.skin;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.TitleBar.ShowMaximizeBox = false;
            this.TitleBar.ShowMinimizeBox = false;
            this.contentPanel.ResumeLayout(false);
            this.identityPage.ResumeLayout(false);
            this.identityGroup.ResumeLayout(false);
            this.nameRow.ResumeLayout(false);
            this.emailRow.ResumeLayout(false);
            this.toolsPage.ResumeLayout(false);
            this.toolsGroup.ResumeLayout(false);
            this.gitRow.ResumeLayout(false);
            this.claudeRow.ResumeLayout(false);
            this.modelRow.ResumeLayout(false);
            this.commitsRow.ResumeLayout(false);
            this.historyPage.ResumeLayout(false);
            this.historyGroup.ResumeLayout(false);
            this.rowHeightRow.ResumeLayout(false);
            this.relativeDatesRow.ResumeLayout(false);
            this.appearancePage.ResumeLayout(false);
            this.appearanceGroup.ResumeLayout(false);
            this.themeRow.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernSkin skin;
        private GitClient.Controls.NavRailControl navRail;
        private GitClient.Controls.SurfacePanel contentPanel;
        private GitClient.Controls.TextLabel pageTitleLabel;
        private GitClient.Controls.SurfacePanel identityPage;
        private GitClient.Controls.SurfacePanel identityGroup;
        private GitClient.Controls.SurfacePanel nameRow;
        private GitClient.Controls.TextLabel nameLabel;
        private GitClient.Controls.TextLabel nameHint;
        private ModernWinForms.ModernTextBox userNameBox;
        private GitClient.Controls.SurfacePanel emailRow;
        private GitClient.Controls.TextLabel emailLabel;
        private ModernWinForms.ModernTextBox emailBox;
        private GitClient.Controls.SurfacePanel toolsPage;
        private GitClient.Controls.SurfacePanel toolsGroup;
        private GitClient.Controls.SurfacePanel gitRow;
        private GitClient.Controls.TextLabel gitLabel;
        private GitClient.Controls.TextLabel gitHint;
        private ModernWinForms.ModernTextBox gitPathBox;
        private GitClient.Controls.SurfacePanel claudeRow;
        private GitClient.Controls.TextLabel claudeLabel;
        private GitClient.Controls.TextLabel claudeHint;
        private ModernWinForms.ModernTextBox claudePathBox;
        private GitClient.Controls.SurfacePanel modelRow;
        private GitClient.Controls.TextLabel modelLabel;
        private GitClient.Controls.TextLabel modelHint;
        private ModernWinForms.ModernTextBox modelBox;
        private GitClient.Controls.SurfacePanel commitsRow;
        private GitClient.Controls.TextLabel commitsLabel;
        private GitClient.Controls.TextLabel commitsHint;
        private ModernWinForms.ModernNumericUpDown maxCommitsBox;
        private GitClient.Controls.SurfacePanel historyPage;
        private GitClient.Controls.SurfacePanel historyGroup;
        private GitClient.Controls.SurfacePanel rowHeightRow;
        private GitClient.Controls.TextLabel rowHeightLabel;
        private GitClient.Controls.TextLabel rowHeightHint;
        private ModernWinForms.ModernNumericUpDown rowHeightBox;
        private GitClient.Controls.SurfacePanel relativeDatesRow;
        private GitClient.Controls.TextLabel relativeDatesLabel;
        private GitClient.Controls.TextLabel relativeDatesHint;
        private GitClient.Controls.ToggleSwitchControl relativeDatesSwitch;
        private GitClient.Controls.SurfacePanel appearancePage;
        private GitClient.Controls.SurfacePanel appearanceGroup;
        private GitClient.Controls.SurfacePanel themeRow;
        private GitClient.Controls.TextLabel themeLabel;
        private GitClient.Controls.TextLabel themeHint;
        private GitClient.Controls.SegmentedControl themeToggle;
        private GitClient.Controls.CommandButton saveButton;
        private GitClient.Controls.CommandButton cancelButton;
    }
}
