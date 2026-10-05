namespace GitClient.Views
{
    partial class WelcomeView
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
            this.contentPanel = new GitClient.Controls.SurfacePanel();
            this.headingLabel = new GitClient.Controls.TextLabel();
            this.subheadLabel = new GitClient.Controls.TextLabel();
            this.openCard = new GitClient.Controls.ActionCard();
            this.cloneCard = new GitClient.Controls.ActionCard();
            this.recentLabel = new GitClient.Controls.TextLabel();
            this.recentRule = new GitClient.Controls.SurfacePanel();
            this.recentCard = new GitClient.Controls.SurfacePanel();
            this.recentList = new GitClient.Controls.RecentListControl();
            this.statusBar = new GitClient.Controls.SurfacePanel();
            this.statusLabel = new GitClient.Controls.TextLabel();
            this.contentPanel.SuspendLayout();
            this.recentCard.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            //
            // statusBar
            //
            this.statusBar.Controls.Add(this.statusLabel);
            this.statusBar.CornerRadius = 0;
            this.statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusBar.Location = new System.Drawing.Point(0, 532);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1280, 28);
            this.statusBar.Surface = GitClient.Controls.SurfaceKind.Base;
            this.statusBar.TabIndex = 1;
            this.statusBar.TopDivider = true;
            //
            // statusLabel
            //
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLabel.Location = new System.Drawing.Point(0, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Padding = new System.Windows.Forms.Padding(14, 0, 0, 0);
            this.statusLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.statusLabel.Size = new System.Drawing.Size(1280, 28);
            this.statusLabel.SizePx = 12F;
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "No repository open";
            //
            // contentPanel
            //
            this.contentPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.contentPanel.Controls.Add(this.headingLabel);
            this.contentPanel.Controls.Add(this.subheadLabel);
            this.contentPanel.Controls.Add(this.openCard);
            this.contentPanel.Controls.Add(this.cloneCard);
            this.contentPanel.Controls.Add(this.recentLabel);
            this.contentPanel.Controls.Add(this.recentRule);
            this.contentPanel.Controls.Add(this.recentCard);
            this.contentPanel.CornerRadius = 0;
            this.contentPanel.Location = new System.Drawing.Point(200, 40);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(880, 476);
            this.contentPanel.Surface = GitClient.Controls.SurfaceKind.None;
            this.contentPanel.TabIndex = 0;
            //
            // headingLabel
            //
            this.headingLabel.Location = new System.Drawing.Point(0, 0);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Semibold = true;
            this.headingLabel.Size = new System.Drawing.Size(500, 42);
            this.headingLabel.SizePx = 32F;
            this.headingLabel.TabIndex = 0;
            this.headingLabel.Text = "Git Client";
            //
            // subheadLabel
            //
            this.subheadLabel.Location = new System.Drawing.Point(0, 48);
            this.subheadLabel.Name = "subheadLabel";
            this.subheadLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.subheadLabel.Size = new System.Drawing.Size(600, 22);
            this.subheadLabel.SizePx = 15F;
            this.subheadLabel.TabIndex = 1;
            this.subheadLabel.Text = "Open a local repository, or clone one to get started.";
            //
            // openCard
            //
            this.openCard.Description = "Pick a project folder; if it isn't a repository yet, you can create one there";
            this.openCard.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M3 5h6l2 2h10v12H3z\"/></svg>";
            this.openCard.Location = new System.Drawing.Point(0, 102);
            this.openCard.Name = "openCard";
            this.openCard.Size = new System.Drawing.Size(434, 96);
            this.openCard.TabIndex = 2;
            this.openCard.Title = "Open a repository";
            this.openCard.Click += new System.EventHandler(this.openCard_Click);
            //
            // cloneCard
            //
            this.cloneCard.Description = "GitHub, Azure DevOps, or any git remote";
            this.cloneCard.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
                "\" d=\"M11 3h2v9.2l3.5-3.5 1.4 1.4L12 16 6.1 10.1l1.4-1.4L11 12.2z\"/><path fill=\"none\" " +
                "stroke=\"currentColor\" stroke-width=\"2\" d=\"M4 15v5h16v-5\"/></svg>";
            this.cloneCard.Location = new System.Drawing.Point(446, 102);
            this.cloneCard.Name = "cloneCard";
            this.cloneCard.Size = new System.Drawing.Size(434, 96);
            this.cloneCard.TabIndex = 3;
            this.cloneCard.Title = "Clone from a URL";
            this.cloneCard.Click += new System.EventHandler(this.cloneCard_Click);
            //
            // recentLabel
            //
            this.recentLabel.Location = new System.Drawing.Point(0, 230);
            this.recentLabel.Name = "recentLabel";
            this.recentLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.recentLabel.Semibold = true;
            this.recentLabel.Size = new System.Drawing.Size(72, 20);
            this.recentLabel.SizePx = 13F;
            this.recentLabel.TabIndex = 5;
            this.recentLabel.Text = "Recent";
            this.recentLabel.Uppercase = true;
            //
            // recentRule
            //
            this.recentRule.BottomDivider = true;
            this.recentRule.CornerRadius = 0;
            this.recentRule.Location = new System.Drawing.Point(82, 239);
            this.recentRule.Name = "recentRule";
            this.recentRule.Size = new System.Drawing.Size(798, 1);
            this.recentRule.Surface = GitClient.Controls.SurfaceKind.None;
            this.recentRule.TabIndex = 6;
            //
            // recentCard
            //
            this.recentCard.Controls.Add(this.recentList);
            this.recentCard.Location = new System.Drawing.Point(0, 258);
            this.recentCard.Name = "recentCard";
            this.recentCard.Padding = new System.Windows.Forms.Padding(1);
            this.recentCard.Size = new System.Drawing.Size(880, 218);
            this.recentCard.TabIndex = 7;
            //
            // recentList
            //
            this.recentList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recentList.Location = new System.Drawing.Point(1, 1);
            this.recentList.Name = "recentList";
            this.recentList.Size = new System.Drawing.Size(878, 216);
            this.recentList.TabIndex = 0;
            this.recentList.RowDoubleClick += new System.EventHandler<GitClient.Controls.RowMouseEventArgs>(this.recentList_RowDoubleClick);
            this.recentList.RowClick += new System.EventHandler<GitClient.Controls.RowMouseEventArgs>(this.recentList_RowClick);
            //
            // WelcomeView
            //
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.statusBar);
            this.Name = "WelcomeView";
            this.Size = new System.Drawing.Size(1280, 560);
            this.contentPanel.ResumeLayout(false);
            this.recentCard.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private GitClient.Controls.SurfacePanel contentPanel;
        private GitClient.Controls.TextLabel headingLabel;
        private GitClient.Controls.TextLabel subheadLabel;
        private GitClient.Controls.ActionCard openCard;
        private GitClient.Controls.ActionCard cloneCard;
        private GitClient.Controls.TextLabel recentLabel;
        private GitClient.Controls.SurfacePanel recentRule;
        private GitClient.Controls.SurfacePanel recentCard;
        private GitClient.Controls.RecentListControl recentList;
        private GitClient.Controls.SurfacePanel statusBar;
        private GitClient.Controls.TextLabel statusLabel;
    }
}
