namespace GitClient.Forms
{
    partial class RemoteDialog
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
            this.subLabel = new GitClient.Controls.TextLabel();
            this.remoteLabel = new GitClient.Controls.TextLabel();
            this.remoteButton = new GitClient.Controls.CommandButton();
            this.removeButton = new GitClient.Controls.CommandButton();
            this.nameLabel = new GitClient.Controls.TextLabel();
            this.nameBox = new ModernWinForms.ModernTextBox();
            this.urlLabel = new GitClient.Controls.TextLabel();
            this.urlBox = new ModernWinForms.ModernTextBox();
            this.hintLabel = new GitClient.Controls.TextLabel();
            this.statusLabel = new GitClient.Controls.TextLabel();
            this.saveButton = new GitClient.Controls.CommandButton();
            this.closeButton = new GitClient.Controls.CommandButton();
            this.remoteMenu = new ModernWinForms.ModernContextMenu(this.components);
            this.content.SuspendLayout();
            this.SuspendLayout();
            //
            // headingLabel
            //
            this.headingLabel.Location = new System.Drawing.Point(20, 14);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Semibold = true;
            this.headingLabel.Size = new System.Drawing.Size(400, 28);
            this.headingLabel.SizePx = 20F;
            this.headingLabel.TabIndex = 0;
            this.headingLabel.Text = "Remotes";
            //
            // subLabel
            //
            this.subLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subLabel.Location = new System.Drawing.Point(20, 44);
            this.subLabel.Name = "subLabel";
            this.subLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.subLabel.Size = new System.Drawing.Size(520, 18);
            this.subLabel.SizePx = 12.5F;
            this.subLabel.TabIndex = 1;
            this.subLabel.Text = "Where this repository fetches from and pushes to.";
            //
            // remoteLabel
            //
            this.remoteLabel.Location = new System.Drawing.Point(20, 78);
            this.remoteLabel.Name = "remoteLabel";
            this.remoteLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.remoteLabel.Size = new System.Drawing.Size(200, 18);
            this.remoteLabel.SizePx = 13F;
            this.remoteLabel.TabIndex = 2;
            this.remoteLabel.Text = "Remote";
            //
            // remoteButton
            //
            this.remoteButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.remoteButton.Chevron = GitClient.Controls.ChevronMode.Trailing;
            this.remoteButton.CornerRadius = 5;
            this.remoteButton.Location = new System.Drawing.Point(20, 100);
            this.remoteButton.Name = "remoteButton";
            this.remoteButton.PaddingX = 12;
            this.remoteButton.Size = new System.Drawing.Size(220, 34);
            this.remoteButton.TabIndex = 3;
            this.remoteButton.Text = "origin";
            this.remoteButton.TextSizePx = 13.5F;
            this.remoteButton.Click += new System.EventHandler(this.remoteButton_Click);
            //
            // removeButton
            //
            this.removeButton.Appearance = GitClient.Controls.ButtonAppearance.Subtle;
            this.removeButton.CornerRadius = 5;
            this.removeButton.Location = new System.Drawing.Point(250, 100);
            this.removeButton.Name = "removeButton";
            this.removeButton.PaddingX = 12;
            this.removeButton.Size = new System.Drawing.Size(110, 34);
            this.removeButton.TabIndex = 4;
            this.removeButton.Text = "Remove";
            this.removeButton.TextSizePx = 13.5F;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            //
            // nameLabel
            //
            this.nameLabel.Location = new System.Drawing.Point(20, 146);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.nameLabel.Size = new System.Drawing.Size(160, 18);
            this.nameLabel.SizePx = 13F;
            this.nameLabel.TabIndex = 5;
            this.nameLabel.Text = "Name";
            //
            // nameBox
            //
            this.nameBox.Location = new System.Drawing.Point(20, 168);
            this.nameBox.Name = "nameBox";
            this.nameBox.PlaceholderText = "origin";
            this.nameBox.Size = new System.Drawing.Size(160, 34);
            this.nameBox.TabIndex = 6;
            this.nameBox.TextChanged += new System.EventHandler(this.field_TextChanged);
            //
            // urlLabel
            //
            this.urlLabel.Location = new System.Drawing.Point(196, 146);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.urlLabel.Size = new System.Drawing.Size(200, 18);
            this.urlLabel.SizePx = 13F;
            this.urlLabel.TabIndex = 7;
            this.urlLabel.Text = "URL";
            //
            // urlBox
            //
            this.urlBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.urlBox.Location = new System.Drawing.Point(196, 168);
            this.urlBox.Name = "urlBox";
            this.urlBox.PlaceholderText = "https://github.com/owner/repository.git";
            this.urlBox.Size = new System.Drawing.Size(344, 34);
            this.urlBox.TabIndex = 8;
            this.urlBox.TextChanged += new System.EventHandler(this.field_TextChanged);
            //
            // hintLabel
            //
            this.hintLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hintLabel.Location = new System.Drawing.Point(20, 208);
            this.hintLabel.Name = "hintLabel";
            this.hintLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.hintLabel.Size = new System.Drawing.Size(520, 18);
            this.hintLabel.SizePx = 12.5F;
            this.hintLabel.TabIndex = 9;
            this.hintLabel.Text = "Paste a GitHub URL, or just owner/repository.";
            //
            // statusLabel
            //
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.Location = new System.Drawing.Point(20, 236);
            this.statusLabel.MultiLine = true;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.statusLabel.Size = new System.Drawing.Size(520, 18);
            this.statusLabel.SizePx = 12.5F;
            this.statusLabel.TabIndex = 10;
            //
            // saveButton
            //
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Appearance = GitClient.Controls.ButtonAppearance.Accent;
            this.saveButton.CornerRadius = 5;
            this.saveButton.Location = new System.Drawing.Point(352, 258);
            this.saveButton.Name = "saveButton";
            this.saveButton.PaddingX = 20;
            this.saveButton.Size = new System.Drawing.Size(96, 34);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "Save";
            this.saveButton.TextSizePx = 13.5F;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            //
            // closeButton
            //
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.closeButton.CornerRadius = 5;
            this.closeButton.Location = new System.Drawing.Point(456, 258);
            this.closeButton.Name = "closeButton";
            this.closeButton.PaddingX = 20;
            this.closeButton.Size = new System.Drawing.Size(84, 34);
            this.closeButton.TabIndex = 12;
            this.closeButton.Text = "Close";
            this.closeButton.TextSizePx = 13.5F;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            //
            // RemoteDialog
            //
            this.ClientSize = new System.Drawing.Size(560, 344);
            this.content.Location = new System.Drawing.Point(0, 32);
            this.content.Size = new System.Drawing.Size(560, 312);
            this.content.Controls.Add(this.headingLabel);
            this.content.Controls.Add(this.subLabel);
            this.content.Controls.Add(this.remoteLabel);
            this.content.Controls.Add(this.remoteButton);
            this.content.Controls.Add(this.removeButton);
            this.content.Controls.Add(this.nameLabel);
            this.content.Controls.Add(this.nameBox);
            this.content.Controls.Add(this.urlLabel);
            this.content.Controls.Add(this.urlBox);
            this.content.Controls.Add(this.hintLabel);
            this.content.Controls.Add(this.statusLabel);
            this.content.Controls.Add(this.saveButton);
            this.content.Controls.Add(this.closeButton);
            this.content.CornerRadius = 0;
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Name = "content";
            this.content.Surface = GitClient.Controls.SurfaceKind.Base;
            this.Controls.Add(this.content);
            this.MinimumSize = new System.Drawing.Size(480, 344);
            this.Name = "RemoteDialog";
            this.Skin = this.skin;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Remotes";
            this.TitleBar.ShowMaximizeBox = false;
            this.TitleBar.ShowMinimizeBox = false;
            this.content.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernSkin skin;
        private GitClient.Controls.SurfacePanel content;
        private GitClient.Controls.TextLabel headingLabel;
        private GitClient.Controls.TextLabel subLabel;
        private GitClient.Controls.TextLabel remoteLabel;
        private GitClient.Controls.CommandButton remoteButton;
        private GitClient.Controls.CommandButton removeButton;
        private GitClient.Controls.TextLabel nameLabel;
        private ModernWinForms.ModernTextBox nameBox;
        private GitClient.Controls.TextLabel urlLabel;
        private ModernWinForms.ModernTextBox urlBox;
        private GitClient.Controls.TextLabel hintLabel;
        private GitClient.Controls.TextLabel statusLabel;
        private GitClient.Controls.CommandButton saveButton;
        private GitClient.Controls.CommandButton closeButton;
        private ModernWinForms.ModernContextMenu remoteMenu;
    }
}
