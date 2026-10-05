namespace GitClient.Forms
{
    partial class NewRepositoryDialog
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
            this.messageLabel = new GitClient.Controls.TextLabel();
            this.pathLabel = new GitClient.Controls.TextLabel();
            this.ignoreLabel = new GitClient.Controls.TextLabel();
            this.ignoreCombo = new ModernWinForms.ModernComboBox();
            this.ignoreHint = new GitClient.Controls.TextLabel();
            this.createButton = new GitClient.Controls.CommandButton();
            this.cancelButton = new GitClient.Controls.CommandButton();
            this.content.SuspendLayout();
            this.SuspendLayout();
            //
            // messageLabel
            //
            this.messageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageLabel.Location = new System.Drawing.Point(24, 22);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.messageLabel.Size = new System.Drawing.Size(432, 20);
            this.messageLabel.SizePx = 13F;
            this.messageLabel.TabIndex = 0;
            this.messageLabel.Text = "git init will run in this folder.";
            //
            // pathLabel
            //
            this.pathLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathLabel.Location = new System.Drawing.Point(24, 46);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Semibold = true;
            this.pathLabel.Size = new System.Drawing.Size(432, 20);
            this.pathLabel.SizePx = 13.5F;
            this.pathLabel.TabIndex = 1;
            this.pathLabel.Text = "Folder";
            //
            // ignoreLabel
            //
            this.ignoreLabel.Location = new System.Drawing.Point(24, 86);
            this.ignoreLabel.Name = "ignoreLabel";
            this.ignoreLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.ignoreLabel.Size = new System.Drawing.Size(200, 20);
            this.ignoreLabel.SizePx = 13F;
            this.ignoreLabel.TabIndex = 2;
            this.ignoreLabel.Text = ".gitignore template";
            //
            // ignoreCombo
            //
            this.ignoreCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ignoreCombo.Location = new System.Drawing.Point(24, 110);
            this.ignoreCombo.Name = "ignoreCombo";
            this.ignoreCombo.Size = new System.Drawing.Size(432, 34);
            this.ignoreCombo.TabIndex = 3;
            this.ignoreCombo.SelectedIndexChanged += new System.EventHandler<ModernWinForms.SelectedIndexChangedEventArgs>(this.ignoreCombo_SelectedIndexChanged);
            //
            // ignoreHint
            //
            this.ignoreHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ignoreHint.Location = new System.Drawing.Point(24, 150);
            this.ignoreHint.Name = "ignoreHint";
            this.ignoreHint.Role = GitClient.Controls.TextRole.Tertiary;
            this.ignoreHint.Size = new System.Drawing.Size(432, 17);
            this.ignoreHint.SizePx = 12F;
            this.ignoreHint.TabIndex = 4;
            //
            // createButton
            //
            this.createButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.createButton.Appearance = GitClient.Controls.ButtonAppearance.Accent;
            this.createButton.CornerRadius = 5;
            this.createButton.Location = new System.Drawing.Point(258, 190);
            this.createButton.Name = "createButton";
            this.createButton.PaddingX = 20;
            this.createButton.Size = new System.Drawing.Size(92, 34);
            this.createButton.TabIndex = 5;
            this.createButton.Text = "Create";
            this.createButton.TextSizePx = 13.5F;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            //
            // cancelButton
            //
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.cancelButton.CornerRadius = 5;
            this.cancelButton.Location = new System.Drawing.Point(358, 190);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.PaddingX = 20;
            this.cancelButton.Size = new System.Drawing.Size(98, 34);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextSizePx = 13.5F;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            //
            // content
            //
            this.content.Controls.Add(this.messageLabel);
            this.content.Controls.Add(this.pathLabel);
            this.content.Controls.Add(this.ignoreLabel);
            this.content.Controls.Add(this.ignoreCombo);
            this.content.Controls.Add(this.ignoreHint);
            this.content.Controls.Add(this.createButton);
            this.content.Controls.Add(this.cancelButton);
            this.content.CornerRadius = 0;
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Location = new System.Drawing.Point(0, 32);
            this.content.Name = "content";
            this.content.Size = new System.Drawing.Size(480, 246);
            this.content.Surface = GitClient.Controls.SurfaceKind.Base;
            //
            // NewRepositoryDialog
            //
            this.ClientSize = new System.Drawing.Size(480, 278);
            this.Controls.Add(this.content);
            this.MinimumSize = new System.Drawing.Size(400, 278);
            this.Name = "NewRepositoryDialog";
            this.Skin = this.skin;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create a repository";
            this.TitleBar.ShowMaximizeBox = false;
            this.TitleBar.ShowMinimizeBox = false;
            this.content.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernSkin skin;
        private GitClient.Controls.SurfacePanel content;
        private GitClient.Controls.TextLabel messageLabel;
        private GitClient.Controls.TextLabel pathLabel;
        private GitClient.Controls.TextLabel ignoreLabel;
        private ModernWinForms.ModernComboBox ignoreCombo;
        private GitClient.Controls.TextLabel ignoreHint;
        private GitClient.Controls.CommandButton createButton;
        private GitClient.Controls.CommandButton cancelButton;
    }
}
