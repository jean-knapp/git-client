namespace GitClient.Forms
{
    partial class CreateRepositoryDialog
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
            this.ownerLabel = new GitClient.Controls.TextLabel();
            this.ownerButton = new GitClient.Controls.CommandButton();
            this.nameLabel = new GitClient.Controls.TextLabel();
            this.nameBox = new ModernWinForms.ModernTextBox();
            this.descriptionLabel = new GitClient.Controls.TextLabel();
            this.descriptionBox = new ModernWinForms.ModernTextBox();
            this.privateSwitch = new GitClient.Controls.ToggleSwitchControl();
            this.privateLabel = new GitClient.Controls.TextLabel();
            this.pushSwitch = new GitClient.Controls.ToggleSwitchControl();
            this.pushLabel = new GitClient.Controls.TextLabel();
            this.statusLabel = new GitClient.Controls.TextLabel();
            this.createButton = new GitClient.Controls.CommandButton();
            this.cancelButton = new GitClient.Controls.CommandButton();
            this.ownerMenu = new ModernWinForms.ModernContextMenu(this.components);
            this.content.SuspendLayout();
            this.SuspendLayout();
            //
            // headingLabel
            //
            this.headingLabel.Location = new System.Drawing.Point(20, 14);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Semibold = true;
            this.headingLabel.Size = new System.Drawing.Size(420, 28);
            this.headingLabel.SizePx = 20F;
            this.headingLabel.TabIndex = 0;
            this.headingLabel.Text = "Create a repository on GitHub";
            //
            // accountLabel
            //
            this.accountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.accountLabel.Location = new System.Drawing.Point(20, 46);
            this.accountLabel.Name = "accountLabel";
            this.accountLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.accountLabel.Size = new System.Drawing.Size(348, 18);
            this.accountLabel.SizePx = 12.5F;
            this.accountLabel.TabIndex = 1;
            this.accountLabel.Text = "Looking for a GitHub sign-in...";
            //
            // tokenButton
            //
            this.tokenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tokenButton.Appearance = GitClient.Controls.ButtonAppearance.Subtle;
            this.tokenButton.CornerRadius = 5;
            this.tokenButton.Location = new System.Drawing.Point(376, 40);
            this.tokenButton.Name = "tokenButton";
            this.tokenButton.PaddingX = 10;
            this.tokenButton.Size = new System.Drawing.Size(164, 30);
            this.tokenButton.TabIndex = 2;
            this.tokenButton.Text = "Use a token...";
            this.tokenButton.TextSizePx = 13F;
            this.tokenButton.Click += new System.EventHandler(this.tokenButton_Click);
            //
            // ownerLabel
            //
            this.ownerLabel.Location = new System.Drawing.Point(20, 84);
            this.ownerLabel.Name = "ownerLabel";
            this.ownerLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.ownerLabel.Size = new System.Drawing.Size(200, 18);
            this.ownerLabel.SizePx = 13F;
            this.ownerLabel.TabIndex = 3;
            this.ownerLabel.Text = "Owner";
            //
            // ownerButton
            //
            this.ownerButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.ownerButton.Chevron = GitClient.Controls.ChevronMode.Trailing;
            this.ownerButton.CornerRadius = 5;
            this.ownerButton.Location = new System.Drawing.Point(20, 106);
            this.ownerButton.Name = "ownerButton";
            this.ownerButton.PaddingX = 12;
            this.ownerButton.Size = new System.Drawing.Size(196, 34);
            this.ownerButton.TabIndex = 4;
            this.ownerButton.Text = "-";
            this.ownerButton.TextSizePx = 13.5F;
            this.ownerButton.Click += new System.EventHandler(this.ownerButton_Click);
            //
            // nameLabel
            //
            this.nameLabel.Location = new System.Drawing.Point(232, 84);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.nameLabel.Size = new System.Drawing.Size(200, 18);
            this.nameLabel.SizePx = 13F;
            this.nameLabel.TabIndex = 5;
            this.nameLabel.Text = "Repository name";
            //
            // nameBox
            //
            this.nameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameBox.Location = new System.Drawing.Point(232, 106);
            this.nameBox.Name = "nameBox";
            this.nameBox.PlaceholderText = "my-repository";
            this.nameBox.Size = new System.Drawing.Size(308, 34);
            this.nameBox.TabIndex = 6;
            this.nameBox.TextChanged += new System.EventHandler(this.field_TextChanged);
            //
            // descriptionLabel
            //
            this.descriptionLabel.Location = new System.Drawing.Point(20, 152);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.descriptionLabel.Size = new System.Drawing.Size(300, 18);
            this.descriptionLabel.SizePx = 13F;
            this.descriptionLabel.TabIndex = 7;
            this.descriptionLabel.Text = "Description (optional)";
            //
            // descriptionBox
            //
            this.descriptionBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionBox.Location = new System.Drawing.Point(20, 174);
            this.descriptionBox.Name = "descriptionBox";
            this.descriptionBox.Size = new System.Drawing.Size(520, 34);
            this.descriptionBox.TabIndex = 8;
            //
            // privateSwitch
            //
            this.privateSwitch.Checked = true;
            this.privateSwitch.Location = new System.Drawing.Point(20, 224);
            this.privateSwitch.Name = "privateSwitch";
            this.privateSwitch.Size = new System.Drawing.Size(40, 20);
            this.privateSwitch.TabIndex = 9;
            //
            // privateLabel
            //
            this.privateLabel.Location = new System.Drawing.Point(70, 223);
            this.privateLabel.Name = "privateLabel";
            this.privateLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.privateLabel.Size = new System.Drawing.Size(300, 22);
            this.privateLabel.SizePx = 13F;
            this.privateLabel.TabIndex = 10;
            this.privateLabel.Text = "Private repository";
            //
            // pushSwitch
            //
            this.pushSwitch.Checked = true;
            this.pushSwitch.Location = new System.Drawing.Point(20, 256);
            this.pushSwitch.Name = "pushSwitch";
            this.pushSwitch.Size = new System.Drawing.Size(40, 20);
            this.pushSwitch.TabIndex = 11;
            //
            // pushLabel
            //
            this.pushLabel.Location = new System.Drawing.Point(70, 255);
            this.pushLabel.Name = "pushLabel";
            this.pushLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.pushLabel.Size = new System.Drawing.Size(420, 22);
            this.pushLabel.SizePx = 13F;
            this.pushLabel.TabIndex = 12;
            this.pushLabel.Text = "Push this branch afterwards";
            //
            // statusLabel
            //
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.Location = new System.Drawing.Point(20, 290);
            this.statusLabel.MultiLine = true;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.statusLabel.Size = new System.Drawing.Size(520, 36);
            this.statusLabel.SizePx = 12.5F;
            this.statusLabel.TabIndex = 13;
            //
            // createButton
            //
            this.createButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.createButton.Appearance = GitClient.Controls.ButtonAppearance.Accent;
            this.createButton.CornerRadius = 5;
            this.createButton.Enabled = false;
            this.createButton.Location = new System.Drawing.Point(338, 336);
            this.createButton.Name = "createButton";
            this.createButton.PaddingX = 20;
            this.createButton.Size = new System.Drawing.Size(96, 34);
            this.createButton.TabIndex = 14;
            this.createButton.Text = "Create";
            this.createButton.TextSizePx = 13.5F;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            //
            // cancelButton
            //
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.cancelButton.CornerRadius = 5;
            this.cancelButton.Location = new System.Drawing.Point(442, 336);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.PaddingX = 20;
            this.cancelButton.Size = new System.Drawing.Size(98, 34);
            this.cancelButton.TabIndex = 15;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextSizePx = 13.5F;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            //
            // CreateRepositoryDialog
            //
            this.ClientSize = new System.Drawing.Size(560, 422);
            this.content.Location = new System.Drawing.Point(0, 32);
            this.content.Size = new System.Drawing.Size(560, 390);
            this.content.Controls.Add(this.headingLabel);
            this.content.Controls.Add(this.accountLabel);
            this.content.Controls.Add(this.tokenButton);
            this.content.Controls.Add(this.ownerLabel);
            this.content.Controls.Add(this.ownerButton);
            this.content.Controls.Add(this.nameLabel);
            this.content.Controls.Add(this.nameBox);
            this.content.Controls.Add(this.descriptionLabel);
            this.content.Controls.Add(this.descriptionBox);
            this.content.Controls.Add(this.privateSwitch);
            this.content.Controls.Add(this.privateLabel);
            this.content.Controls.Add(this.pushSwitch);
            this.content.Controls.Add(this.pushLabel);
            this.content.Controls.Add(this.statusLabel);
            this.content.Controls.Add(this.createButton);
            this.content.Controls.Add(this.cancelButton);
            this.content.CornerRadius = 0;
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Name = "content";
            this.content.Surface = GitClient.Controls.SurfaceKind.Base;
            this.Controls.Add(this.content);
            this.MinimumSize = new System.Drawing.Size(480, 422);
            this.Name = "CreateRepositoryDialog";
            this.Skin = this.skin;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create a repository on GitHub";
            this.TitleBar.ShowMaximizeBox = false;
            this.TitleBar.ShowMinimizeBox = false;
            this.content.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernSkin skin;
        private GitClient.Controls.SurfacePanel content;
        private GitClient.Controls.TextLabel headingLabel;
        private GitClient.Controls.TextLabel accountLabel;
        private GitClient.Controls.CommandButton tokenButton;
        private GitClient.Controls.TextLabel ownerLabel;
        private GitClient.Controls.CommandButton ownerButton;
        private GitClient.Controls.TextLabel nameLabel;
        private ModernWinForms.ModernTextBox nameBox;
        private GitClient.Controls.TextLabel descriptionLabel;
        private ModernWinForms.ModernTextBox descriptionBox;
        private GitClient.Controls.ToggleSwitchControl privateSwitch;
        private GitClient.Controls.TextLabel privateLabel;
        private GitClient.Controls.ToggleSwitchControl pushSwitch;
        private GitClient.Controls.TextLabel pushLabel;
        private GitClient.Controls.TextLabel statusLabel;
        private GitClient.Controls.CommandButton createButton;
        private GitClient.Controls.CommandButton cancelButton;
        private ModernWinForms.ModernContextMenu ownerMenu;
    }
}
