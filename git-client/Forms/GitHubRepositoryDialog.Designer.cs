namespace GitClient.Forms
{
    partial class GitHubRepositoryDialog
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
            this.content = new GitClient.Controls.SurfacePanel();
            this.headingLabel = new GitClient.Controls.TextLabel();
            this.accountLabel = new GitClient.Controls.TextLabel();
            this.tokenButton = new GitClient.Controls.CommandButton();
            this.refreshButton = new GitClient.Controls.CommandButton();
            this.filterBox = new ModernWinForms.ModernTextBox();
            this.listCard = new GitClient.Controls.SurfacePanel();
            this.list = new GitClient.Controls.GitHubRepositoryListControl();
            this.progressBar = new ModernWinForms.ModernProgressBar();
            this.chooseButton = new GitClient.Controls.CommandButton();
            this.cancelButton = new GitClient.Controls.CommandButton();
            this.content.SuspendLayout();
            this.listCard.SuspendLayout();
            this.SuspendLayout();
            //
            // content
            //
            this.content.Controls.Add(this.headingLabel);
            this.content.Controls.Add(this.accountLabel);
            this.content.Controls.Add(this.tokenButton);
            this.content.Controls.Add(this.refreshButton);
            this.content.Controls.Add(this.filterBox);
            this.content.Controls.Add(this.listCard);
            this.content.Controls.Add(this.progressBar);
            this.content.Controls.Add(this.chooseButton);
            this.content.Controls.Add(this.cancelButton);
            this.content.CornerRadius = 0;
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Location = new System.Drawing.Point(0, 32);
            this.content.Name = "content";
            this.content.Size = new System.Drawing.Size(660, 468);
            this.content.Surface = GitClient.Controls.SurfaceKind.Base;
            this.content.TabIndex = 0;
            //
            // headingLabel
            //
            this.headingLabel.Location = new System.Drawing.Point(24, 18);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Semibold = true;
            this.headingLabel.Size = new System.Drawing.Size(420, 28);
            this.headingLabel.SizePx = 20F;
            this.headingLabel.TabIndex = 0;
            this.headingLabel.Text = "Choose a repository";
            //
            // accountLabel
            //
            this.accountLabel.Location = new System.Drawing.Point(24, 50);
            this.accountLabel.Name = "accountLabel";
            this.accountLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.accountLabel.Size = new System.Drawing.Size(420, 18);
            this.accountLabel.SizePx = 13F;
            this.accountLabel.TabIndex = 1;
            this.accountLabel.Text = "Looking for a GitHub sign-in…";
            //
            // refreshButton
            //
            this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.refreshButton.CornerRadius = 5;
            this.refreshButton.IconSize = 14;
            this.refreshButton.IconSvg = GitClient.Controls.Icons.Refresh;
            this.refreshButton.Location = new System.Drawing.Point(452, 24);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.PaddingX = 12;
            this.refreshButton.Size = new System.Drawing.Size(96, 30);
            this.refreshButton.TabIndex = 2;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.TextSizePx = 13F;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            //
            // tokenButton
            //
            this.tokenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tokenButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.tokenButton.CornerRadius = 5;
            this.tokenButton.IconSize = 14;
            this.tokenButton.IconSvg = GitClient.Controls.Icons.Github;
            this.tokenButton.Location = new System.Drawing.Point(452, 60);
            this.tokenButton.Name = "tokenButton";
            this.tokenButton.PaddingX = 12;
            this.tokenButton.Size = new System.Drawing.Size(96, 30);
            this.tokenButton.TabIndex = 3;
            this.tokenButton.Text = "Sign in…";
            this.tokenButton.TextSizePx = 13F;
            this.tokenButton.Click += new System.EventHandler(this.tokenButton_Click);
            //
            // filterBox
            //
            this.filterBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterBox.Location = new System.Drawing.Point(24, 100);
            this.filterBox.Name = "filterBox";
            this.filterBox.PlaceholderText = "Filter by name or description…";
            this.filterBox.Size = new System.Drawing.Size(612, 34);
            this.filterBox.TabIndex = 4;
            this.filterBox.TextChanged += new System.EventHandler(this.filterBox_TextChanged);
            //
            // listCard
            //
            this.listCard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listCard.Controls.Add(this.list);
            this.listCard.CornerRadius = 7;
            this.listCard.Location = new System.Drawing.Point(24, 146);
            this.listCard.Name = "listCard";
            this.listCard.Padding = new System.Windows.Forms.Padding(1);
            this.listCard.Size = new System.Drawing.Size(612, 254);
            this.listCard.TabIndex = 5;
            //
            // list
            //
            this.list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list.Location = new System.Drawing.Point(1, 1);
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(610, 252);
            this.list.TabIndex = 0;
            //
            // progressBar
            //
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(24, 410);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(612, 6);
            this.progressBar.Style = ModernWinForms.ModernProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 6;
            this.progressBar.Visible = false;
            //
            // chooseButton
            //
            this.chooseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseButton.Appearance = GitClient.Controls.ButtonAppearance.Accent;
            this.chooseButton.CornerRadius = 5;
            this.chooseButton.Enabled = false;
            this.chooseButton.Location = new System.Drawing.Point(430, 426);
            this.chooseButton.Name = "chooseButton";
            this.chooseButton.PaddingX = 20;
            this.chooseButton.Size = new System.Drawing.Size(96, 34);
            this.chooseButton.TabIndex = 7;
            this.chooseButton.Text = "Choose";
            this.chooseButton.TextSizePx = 13.5F;
            this.chooseButton.Click += new System.EventHandler(this.chooseButton_Click);
            //
            // cancelButton
            //
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.cancelButton.CornerRadius = 5;
            this.cancelButton.Location = new System.Drawing.Point(534, 426);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.PaddingX = 20;
            this.cancelButton.Size = new System.Drawing.Size(102, 34);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextSizePx = 13.5F;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            //
            // GitHubRepositoryDialog
            //
            this.ClientSize = new System.Drawing.Size(660, 500);
            this.Controls.Add(this.content);
            this.MinimumSize = new System.Drawing.Size(560, 420);
            this.Name = "GitHubRepositoryDialog";
            this.Skin = this.skin;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose a repository";
            this.TitleBar.ShowMaximizeBox = false;
            this.TitleBar.ShowMinimizeBox = false;
            this.content.ResumeLayout(false);
            this.listCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernSkin skin;
        private GitClient.Controls.SurfacePanel content;
        private GitClient.Controls.TextLabel headingLabel;
        private GitClient.Controls.TextLabel accountLabel;
        private GitClient.Controls.CommandButton tokenButton;
        private GitClient.Controls.CommandButton refreshButton;
        private ModernWinForms.ModernTextBox filterBox;
        private GitClient.Controls.SurfacePanel listCard;
        private GitClient.Controls.GitHubRepositoryListControl list;
        private ModernWinForms.ModernProgressBar progressBar;
        private GitClient.Controls.CommandButton chooseButton;
        private GitClient.Controls.CommandButton cancelButton;
    }
}
