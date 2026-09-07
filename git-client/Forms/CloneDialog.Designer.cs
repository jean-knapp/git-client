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
            this.folderLabel = new GitClient.Controls.TextLabel();
            this.folderBox = new ModernWinForms.ModernTextBox();
            this.browseButton = new GitClient.Controls.CommandButton();
            this.targetLabel = new GitClient.Controls.TextLabel();
            this.submodulesCheck = new GitClient.Controls.TokenCheckBox();
            this.progressLabel = new GitClient.Controls.TextLabel();
            this.progressBar = new ModernWinForms.ModernProgressBar();
            this.cloneButton = new GitClient.Controls.CommandButton();
            this.cancelButton = new GitClient.Controls.CommandButton();
            this.content.SuspendLayout();
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
            this.urlBox.Size = new System.Drawing.Size(552, 34);
            this.urlBox.TabIndex = 2;
            this.urlBox.TextChanged += new System.EventHandler(this.urlBox_TextChanged);
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
            // targetLabel
            //
            this.targetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetLabel.Location = new System.Drawing.Point(24, 194);
            this.targetLabel.Name = "targetLabel";
            this.targetLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.targetLabel.Size = new System.Drawing.Size(552, 18);
            this.targetLabel.SizePx = 12.5F;
            this.targetLabel.TabIndex = 6;
            //
            // submodulesCheck
            //
            this.submodulesCheck.Location = new System.Drawing.Point(24, 224);
            this.submodulesCheck.Name = "submodulesCheck";
            this.submodulesCheck.Size = new System.Drawing.Size(240, 22);
            this.submodulesCheck.TabIndex = 7;
            this.submodulesCheck.Text = "Also fetch submodules";
            //
            // progressLabel
            //
            this.progressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLabel.Location = new System.Drawing.Point(24, 258);
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
            this.progressBar.Location = new System.Drawing.Point(24, 280);
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
            this.cloneButton.Location = new System.Drawing.Point(370, 300);
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
            this.cancelButton.Location = new System.Drawing.Point(474, 300);
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
            this.ClientSize = new System.Drawing.Size(600, 389);
            this.content.Location = new System.Drawing.Point(0, 32);
            this.content.Size = new System.Drawing.Size(600, 357);
            this.content.Controls.Add(this.headingLabel);
            this.content.Controls.Add(this.urlLabel);
            this.content.Controls.Add(this.urlBox);
            this.content.Controls.Add(this.folderLabel);
            this.content.Controls.Add(this.folderBox);
            this.content.Controls.Add(this.browseButton);
            this.content.Controls.Add(this.targetLabel);
            this.content.Controls.Add(this.submodulesCheck);
            this.content.Controls.Add(this.progressLabel);
            this.content.Controls.Add(this.progressBar);
            this.content.Controls.Add(this.cloneButton);
            this.content.Controls.Add(this.cancelButton);
            this.content.CornerRadius = 0;
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Name = "content";
            this.content.Surface = GitClient.Controls.SurfaceKind.Base;
            this.Controls.Add(this.content);
            this.MinimumSize = new System.Drawing.Size(520, 389);
            this.Name = "CloneDialog";
            this.Skin = this.skin;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Clone repository";
            this.TitleBar.ShowMaximizeBox = false;
            this.TitleBar.ShowMinimizeBox = false;
            this.content.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernSkin skin;
        private GitClient.Controls.SurfacePanel content;
        private GitClient.Controls.TextLabel headingLabel;
        private GitClient.Controls.TextLabel urlLabel;
        private ModernWinForms.ModernTextBox urlBox;
        private GitClient.Controls.TextLabel folderLabel;
        private ModernWinForms.ModernTextBox folderBox;
        private GitClient.Controls.CommandButton browseButton;
        private GitClient.Controls.TextLabel targetLabel;
        private GitClient.Controls.TokenCheckBox submodulesCheck;
        private GitClient.Controls.TextLabel progressLabel;
        private ModernWinForms.ModernProgressBar progressBar;
        private GitClient.Controls.CommandButton cloneButton;
        private GitClient.Controls.CommandButton cancelButton;
    }
}
