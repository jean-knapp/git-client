namespace GitClient.Forms
{
    partial class CloneDialog
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
            this.urlLabel = new GitClient.Controls.TextLabel();
            this.urlBox = new ModernWinForms.ModernTextBox();
            this.githubButton = new GitClient.Controls.CommandButton();
            this.folderLabel = new GitClient.Controls.TextLabel();
            this.folderBox = new ModernWinForms.ModernTextBox();
            this.browseButton = new GitClient.Controls.CommandButton();
            this.nameLabel = new GitClient.Controls.TextLabel();
            this.nameBox = new ModernWinForms.ModernTextBox();
            this.resetNameButton = new GitClient.Controls.CommandButton();
            this.targetLabel = new GitClient.Controls.TextLabel();
            this.optionsLabel = new GitClient.Controls.TextLabel();
            this.optionsCard = new GitClient.Controls.SurfacePanel();
            this.sparseRow = new GitClient.Controls.SurfacePanel();
            this.sparseTitle = new GitClient.Controls.TextLabel();
            this.sparseHint = new GitClient.Controls.TextLabel();
            this.sparseBox = new ModernWinForms.ModernTextBox();
            this.sparseSwitch = new GitClient.Controls.ToggleSwitchControl();
            this.bloblessRow = new GitClient.Controls.SurfacePanel();
            this.bloblessTitle = new GitClient.Controls.TextLabel();
            this.bloblessHint = new GitClient.Controls.TextLabel();
            this.bloblessSwitch = new GitClient.Controls.ToggleSwitchControl();
            this.submodulesRow = new GitClient.Controls.SurfacePanel();
            this.submodulesTitle = new GitClient.Controls.TextLabel();
            this.submodulesHint = new GitClient.Controls.TextLabel();
            this.submodulesSwitch = new GitClient.Controls.ToggleSwitchControl();
            this.progressLabel = new GitClient.Controls.TextLabel();
            this.progressBar = new ModernWinForms.ModernProgressBar();
            this.cloneButton = new GitClient.Controls.CommandButton();
            this.cancelButton = new GitClient.Controls.CommandButton();
            this.content.SuspendLayout();
            this.optionsCard.SuspendLayout();
            this.sparseRow.SuspendLayout();
            this.bloblessRow.SuspendLayout();
            this.submodulesRow.SuspendLayout();
            this.SuspendLayout();
            //
            // headingLabel
            //
            this.headingLabel.Location = new System.Drawing.Point(24, 12);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Semibold = true;
            this.headingLabel.Size = new System.Drawing.Size(400, 28);
            this.headingLabel.SizePx = 20F;
            this.headingLabel.TabIndex = 0;
            this.headingLabel.Text = "Clone a repository";
            //
            // urlLabel
            //
            this.urlLabel.Location = new System.Drawing.Point(24, 52);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.urlLabel.Size = new System.Drawing.Size(300, 18);
            this.urlLabel.SizePx = 13F;
            this.urlLabel.TabIndex = 1;
            this.urlLabel.Text = "Repository URL";
            //
            // urlBox
            //
            this.urlBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.urlBox.Location = new System.Drawing.Point(24, 77);
            this.urlBox.Name = "urlBox";
            this.urlBox.PlaceholderText = "https://github.com/owner/repository.git";
            this.urlBox.Size = new System.Drawing.Size(440, 34);
            this.urlBox.TabIndex = 2;
            this.urlBox.TextChanged += new System.EventHandler(this.urlBox_TextChanged);
            //
            // githubButton
            //
            this.githubButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.githubButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.githubButton.CornerRadius = 5;
            this.githubButton.IconSize = 14;
            this.githubButton.IconSvg = GitClient.Controls.Icons.Github;
            this.githubButton.Location = new System.Drawing.Point(472, 77);
            this.githubButton.Name = "githubButton";
            this.githubButton.PaddingX = 10;
            this.githubButton.Size = new System.Drawing.Size(104, 34);
            this.githubButton.TabIndex = 3;
            this.githubButton.Text = "GitHub...";
            this.githubButton.TextSizePx = 13.5F;
            this.githubButton.Click += new System.EventHandler(this.githubButton_Click);
            //
            // folderLabel
            //
            this.folderLabel.Location = new System.Drawing.Point(24, 129);
            this.folderLabel.Name = "folderLabel";
            this.folderLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.folderLabel.Size = new System.Drawing.Size(300, 18);
            this.folderLabel.SizePx = 13F;
            this.folderLabel.TabIndex = 3;
            this.folderLabel.Text = "Clone into";
            //
            // folderBox
            //
            this.folderBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.folderBox.Location = new System.Drawing.Point(24, 154);
            this.folderBox.Name = "folderBox";
            this.folderBox.Size = new System.Drawing.Size(440, 34);
            this.folderBox.TabIndex = 4;
            this.folderBox.TextChanged += new System.EventHandler(this.folderBox_TextChanged);
            //
            // browseButton
            //
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.browseButton.CornerRadius = 5;
            this.browseButton.Location = new System.Drawing.Point(472, 154);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(104, 34);
            this.browseButton.TabIndex = 5;
            this.browseButton.Text = "Browse...";
            this.browseButton.TextSizePx = 13.5F;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            //
            // nameLabel
            //
            this.nameLabel.Location = new System.Drawing.Point(24, 194);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.nameLabel.Size = new System.Drawing.Size(300, 18);
            this.nameLabel.SizePx = 13F;
            this.nameLabel.TabIndex = 6;
            this.nameLabel.Text = "Folder name";
            //
            // nameBox
            //
            this.nameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameBox.Location = new System.Drawing.Point(24, 219);
            this.nameBox.Name = "nameBox";
            this.nameBox.PlaceholderText = "the repository\'s own name";
            this.nameBox.Size = new System.Drawing.Size(440, 34);
            this.nameBox.TabIndex = 7;
            this.nameBox.TextChanged += new System.EventHandler(this.nameBox_TextChanged);
            //
            // resetNameButton
            //
            this.resetNameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resetNameButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.resetNameButton.CornerRadius = 5;
            this.resetNameButton.Enabled = false;
            this.resetNameButton.Location = new System.Drawing.Point(472, 219);
            this.resetNameButton.Name = "resetNameButton";
            this.resetNameButton.PaddingX = 10;
            this.resetNameButton.Size = new System.Drawing.Size(104, 34);
            this.resetNameButton.TabIndex = 8;
            this.resetNameButton.Text = "Repo name";
            this.resetNameButton.TextSizePx = 13.5F;
            this.resetNameButton.Click += new System.EventHandler(this.resetNameButton_Click);
            //
            // targetLabel
            //
            this.targetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetLabel.Location = new System.Drawing.Point(24, 261);
            this.targetLabel.Name = "targetLabel";
            this.targetLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.targetLabel.Size = new System.Drawing.Size(552, 18);
            this.targetLabel.SizePx = 12.5F;
            this.targetLabel.TabIndex = 6;
            //
            // optionsLabel
            //
            this.optionsLabel.Location = new System.Drawing.Point(24, 291);
            this.optionsLabel.Name = "optionsLabel";
            this.optionsLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.optionsLabel.Size = new System.Drawing.Size(400, 18);
            this.optionsLabel.SizePx = 13F;
            this.optionsLabel.TabIndex = 9;
            this.optionsLabel.Text = "What to download";
            //
            // optionsCard
            //
            this.optionsCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.optionsCard.Controls.Add(this.sparseRow);
            this.optionsCard.Controls.Add(this.bloblessRow);
            this.optionsCard.Controls.Add(this.submodulesRow);
            this.optionsCard.CornerRadius = 7;
            this.optionsCard.Location = new System.Drawing.Point(24, 316);
            this.optionsCard.Name = "optionsCard";
            this.optionsCard.Padding = new System.Windows.Forms.Padding(1);
            this.optionsCard.Size = new System.Drawing.Size(552, 184);
            this.optionsCard.TabIndex = 10;
            //
            // submodulesRow
            //
            this.submodulesRow.Controls.Add(this.submodulesTitle);
            this.submodulesRow.Controls.Add(this.submodulesHint);
            this.submodulesRow.Controls.Add(this.submodulesSwitch);
            this.submodulesRow.CornerRadius = 0;
            this.submodulesRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.submodulesRow.Location = new System.Drawing.Point(1, 1);
            this.submodulesRow.Name = "submodulesRow";
            this.submodulesRow.Size = new System.Drawing.Size(550, 52);
            this.submodulesRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.submodulesRow.TabIndex = 0;
            //
            // submodulesTitle
            //
            this.submodulesTitle.Location = new System.Drawing.Point(16, 8);
            this.submodulesTitle.Name = "submodulesTitle";
            this.submodulesTitle.Size = new System.Drawing.Size(440, 19);
            this.submodulesTitle.SizePx = 13.5F;
            this.submodulesTitle.TabIndex = 0;
            this.submodulesTitle.Text = "Also fetch submodules";
            //
            // submodulesHint
            //
            this.submodulesHint.Location = new System.Drawing.Point(16, 27);
            this.submodulesHint.Name = "submodulesHint";
            this.submodulesHint.Role = GitClient.Controls.TextRole.Tertiary;
            this.submodulesHint.Size = new System.Drawing.Size(460, 17);
            this.submodulesHint.SizePx = 12F;
            this.submodulesHint.TabIndex = 1;
            this.submodulesHint.Text = "Clone the repositories this one points at as well";
            //
            // submodulesSwitch
            //
            this.submodulesSwitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.submodulesSwitch.Location = new System.Drawing.Point(494, 16);
            this.submodulesSwitch.Name = "submodulesSwitch";
            this.submodulesSwitch.Size = new System.Drawing.Size(40, 20);
            this.submodulesSwitch.TabIndex = 2;
            //
            // bloblessRow
            //
            this.bloblessRow.Controls.Add(this.bloblessTitle);
            this.bloblessRow.Controls.Add(this.bloblessHint);
            this.bloblessRow.Controls.Add(this.bloblessSwitch);
            this.bloblessRow.CornerRadius = 0;
            this.bloblessRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.bloblessRow.Location = new System.Drawing.Point(1, 53);
            this.bloblessRow.Name = "bloblessRow";
            this.bloblessRow.Size = new System.Drawing.Size(550, 52);
            this.bloblessRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.bloblessRow.TabIndex = 1;
            //
            // bloblessTitle
            //
            this.bloblessTitle.Location = new System.Drawing.Point(16, 8);
            this.bloblessTitle.Name = "bloblessTitle";
            this.bloblessTitle.Size = new System.Drawing.Size(440, 19);
            this.bloblessTitle.SizePx = 13.5F;
            this.bloblessTitle.TabIndex = 0;
            this.bloblessTitle.Text = "Download file contents only when opened";
            //
            // bloblessHint
            //
            this.bloblessHint.Location = new System.Drawing.Point(16, 27);
            this.bloblessHint.Name = "bloblessHint";
            this.bloblessHint.Role = GitClient.Controls.TextRole.Tertiary;
            this.bloblessHint.Size = new System.Drawing.Size(460, 17);
            this.bloblessHint.SizePx = 12F;
            this.bloblessHint.TabIndex = 1;
            this.bloblessHint.Text = "Full history, far smaller on disk. Old file versions arrive when you ask for them.";
            //
            // bloblessSwitch
            //
            this.bloblessSwitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bloblessSwitch.Location = new System.Drawing.Point(494, 16);
            this.bloblessSwitch.Name = "bloblessSwitch";
            this.bloblessSwitch.Size = new System.Drawing.Size(40, 20);
            this.bloblessSwitch.TabIndex = 2;
            this.bloblessSwitch.CheckedChanged += new System.EventHandler(this.option_CheckedChanged);
            //
            // sparseRow
            //
            this.sparseRow.Controls.Add(this.sparseTitle);
            this.sparseRow.Controls.Add(this.sparseHint);
            this.sparseRow.Controls.Add(this.sparseBox);
            this.sparseRow.Controls.Add(this.sparseSwitch);
            this.sparseRow.CornerRadius = 0;
            this.sparseRow.Dock = System.Windows.Forms.DockStyle.Top;
            this.sparseRow.Location = new System.Drawing.Point(1, 105);
            this.sparseRow.Name = "sparseRow";
            this.sparseRow.Size = new System.Drawing.Size(550, 78);
            this.sparseRow.Surface = GitClient.Controls.SurfaceKind.None;
            this.sparseRow.TabIndex = 2;
            //
            // sparseTitle
            //
            this.sparseTitle.Location = new System.Drawing.Point(16, 8);
            this.sparseTitle.Name = "sparseTitle";
            this.sparseTitle.Size = new System.Drawing.Size(440, 19);
            this.sparseTitle.SizePx = 13.5F;
            this.sparseTitle.TabIndex = 0;
            this.sparseTitle.Text = "Check out only some folders";
            //
            // sparseHint
            //
            this.sparseHint.Location = new System.Drawing.Point(16, 27);
            this.sparseHint.Name = "sparseHint";
            this.sparseHint.Role = GitClient.Controls.TextRole.Tertiary;
            this.sparseHint.Size = new System.Drawing.Size(460, 17);
            this.sparseHint.SizePx = 12F;
            this.sparseHint.TabIndex = 1;
            this.sparseHint.Text = "The rest stays in history but off your disk. Separate folders with commas.";
            //
            // sparseBox
            //
            this.sparseBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sparseBox.Enabled = false;
            this.sparseBox.Location = new System.Drawing.Point(16, 46);
            this.sparseBox.Name = "sparseBox";
            this.sparseBox.PlaceholderText = "src, docs";
            this.sparseBox.Size = new System.Drawing.Size(462, 28);
            this.sparseBox.TabIndex = 2;
            this.sparseBox.TextChanged += new System.EventHandler(this.option_CheckedChanged);
            //
            // sparseSwitch
            //
            this.sparseSwitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sparseSwitch.Location = new System.Drawing.Point(494, 16);
            this.sparseSwitch.Name = "sparseSwitch";
            this.sparseSwitch.Size = new System.Drawing.Size(40, 20);
            this.sparseSwitch.TabIndex = 3;
            this.sparseSwitch.CheckedChanged += new System.EventHandler(this.sparseSwitch_CheckedChanged);
            //
            // progressLabel
            //
            this.progressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel.Location = new System.Drawing.Point(24, 508);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.progressLabel.Size = new System.Drawing.Size(552, 18);
            this.progressLabel.SizePx = 12.5F;
            this.progressLabel.TabIndex = 8;
            //
            // progressBar
            //
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(24, 530);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(552, 6);
            this.progressBar.Style = ModernWinForms.ModernProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 9;
            this.progressBar.Visible = false;
            //
            // cloneButton
            //
            this.cloneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cloneButton.Appearance = GitClient.Controls.ButtonAppearance.Accent;
            this.cloneButton.CornerRadius = 5;
            this.cloneButton.Enabled = false;
            this.cloneButton.Location = new System.Drawing.Point(370, 550);
            this.cloneButton.Name = "cloneButton";
            this.cloneButton.PaddingX = 20;
            this.cloneButton.Size = new System.Drawing.Size(96, 34);
            this.cloneButton.TabIndex = 10;
            this.cloneButton.Text = "Clone";
            this.cloneButton.TextSizePx = 13.5F;
            this.cloneButton.Click += new System.EventHandler(this.cloneButton_Click);
            //
            // cancelButton
            //
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.cancelButton.CornerRadius = 5;
            this.cancelButton.Location = new System.Drawing.Point(474, 550);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.PaddingX = 20;
            this.cancelButton.Size = new System.Drawing.Size(102, 34);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextSizePx = 13.5F;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            //
            // CloneDialog
            //
            this.ClientSize = new System.Drawing.Size(600, 640);
            this.content.Location = new System.Drawing.Point(0, 32);
            this.content.Size = new System.Drawing.Size(600, 608);
            this.content.Controls.Add(this.headingLabel);
            this.content.Controls.Add(this.urlLabel);
            this.content.Controls.Add(this.urlBox);
            this.content.Controls.Add(this.githubButton);
            this.content.Controls.Add(this.folderLabel);
            this.content.Controls.Add(this.folderBox);
            this.content.Controls.Add(this.browseButton);
            this.content.Controls.Add(this.nameLabel);
            this.content.Controls.Add(this.nameBox);
            this.content.Controls.Add(this.resetNameButton);
            this.content.Controls.Add(this.targetLabel);
            this.content.Controls.Add(this.optionsLabel);
            this.content.Controls.Add(this.optionsCard);
            this.content.Controls.Add(this.progressLabel);
            this.content.Controls.Add(this.progressBar);
            this.content.Controls.Add(this.cloneButton);
            this.content.Controls.Add(this.cancelButton);
            this.content.CornerRadius = 0;
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Name = "content";
            this.content.Surface = GitClient.Controls.SurfaceKind.Base;
            this.Controls.Add(this.content);
            this.MinimumSize = new System.Drawing.Size(520, 640);
            this.Name = "CloneDialog";
            this.Skin = this.skin;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Clone repository";
            this.TitleBar.ShowMaximizeBox = false;
            this.TitleBar.ShowMinimizeBox = false;
            this.optionsCard.ResumeLayout(false);
            this.sparseRow.ResumeLayout(false);
            this.bloblessRow.ResumeLayout(false);
            this.submodulesRow.ResumeLayout(false);
            this.content.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernSkin skin;
        private GitClient.Controls.SurfacePanel content;
        private GitClient.Controls.TextLabel headingLabel;
        private GitClient.Controls.TextLabel urlLabel;
        private ModernWinForms.ModernTextBox urlBox;
        private GitClient.Controls.CommandButton githubButton;
        private GitClient.Controls.TextLabel folderLabel;
        private ModernWinForms.ModernTextBox folderBox;
        private GitClient.Controls.CommandButton browseButton;
        private GitClient.Controls.TextLabel nameLabel;
        private ModernWinForms.ModernTextBox nameBox;
        private GitClient.Controls.CommandButton resetNameButton;
        private GitClient.Controls.TextLabel targetLabel;
        private GitClient.Controls.TextLabel optionsLabel;
        private GitClient.Controls.SurfacePanel optionsCard;
        private GitClient.Controls.SurfacePanel submodulesRow;
        private GitClient.Controls.TextLabel submodulesTitle;
        private GitClient.Controls.TextLabel submodulesHint;
        private GitClient.Controls.ToggleSwitchControl submodulesSwitch;
        private GitClient.Controls.SurfacePanel bloblessRow;
        private GitClient.Controls.TextLabel bloblessTitle;
        private GitClient.Controls.TextLabel bloblessHint;
        private GitClient.Controls.ToggleSwitchControl bloblessSwitch;
        private GitClient.Controls.SurfacePanel sparseRow;
        private GitClient.Controls.TextLabel sparseTitle;
        private GitClient.Controls.TextLabel sparseHint;
        private ModernWinForms.ModernTextBox sparseBox;
        private GitClient.Controls.ToggleSwitchControl sparseSwitch;
        private GitClient.Controls.TextLabel progressLabel;
        private ModernWinForms.ModernProgressBar progressBar;
        private GitClient.Controls.CommandButton cloneButton;
        private GitClient.Controls.CommandButton cancelButton;
    }
}
