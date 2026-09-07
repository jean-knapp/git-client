namespace GitClient.Forms
{
    partial class NewBranchDialog
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
            this.nameLabel = new GitClient.Controls.TextLabel();
            this.nameBox = new ModernWinForms.ModernTextBox();
            this.startPointLabel = new GitClient.Controls.TextLabel();
            this.startPointValueLabel = new GitClient.Controls.TextLabel();
            this.checkoutSwitch = new GitClient.Controls.ToggleSwitchControl();
            this.checkoutLabel = new GitClient.Controls.TextLabel();
            this.createButton = new GitClient.Controls.CommandButton();
            this.cancelButton = new GitClient.Controls.CommandButton();
            this.content.SuspendLayout();
            this.SuspendLayout();
            //
            // headingLabel
            //
            this.headingLabel.Location = new System.Drawing.Point(24, 12);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Semibold = true;
            this.headingLabel.Size = new System.Drawing.Size(300, 28);
            this.headingLabel.SizePx = 20F;
            this.headingLabel.TabIndex = 0;
            this.headingLabel.Text = "New branch";
            //
            // nameLabel
            //
            this.nameLabel.Location = new System.Drawing.Point(24, 52);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.nameLabel.Size = new System.Drawing.Size(300, 18);
            this.nameLabel.SizePx = 13F;
            this.nameLabel.TabIndex = 1;
            this.nameLabel.Text = "Branch name";
            //
            // nameBox
            //
            this.nameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameBox.Location = new System.Drawing.Point(24, 77);
            this.nameBox.Name = "nameBox";
            this.nameBox.PlaceholderText = "feature/my-change";
            this.nameBox.Size = new System.Drawing.Size(404, 34);
            this.nameBox.TabIndex = 2;
            this.nameBox.TextChanged += new System.EventHandler(this.nameBox_TextChanged);
            //
            // startPointLabel
            //
            this.startPointLabel.Location = new System.Drawing.Point(24, 125);
            this.startPointLabel.Name = "startPointLabel";
            this.startPointLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.startPointLabel.Size = new System.Drawing.Size(300, 18);
            this.startPointLabel.SizePx = 13F;
            this.startPointLabel.TabIndex = 3;
            this.startPointLabel.Text = "Starting from";
            //
            // startPointValueLabel
            //
            this.startPointValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startPointValueLabel.Location = new System.Drawing.Point(24, 145);
            this.startPointValueLabel.Name = "startPointValueLabel";
            this.startPointValueLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.startPointValueLabel.Size = new System.Drawing.Size(404, 20);
            this.startPointValueLabel.SizePx = 12.5F;
            this.startPointValueLabel.TabIndex = 4;
            this.startPointValueLabel.Text = "HEAD";
            //
            // checkoutSwitch
            //
            this.checkoutSwitch.Checked = true;
            this.checkoutSwitch.Location = new System.Drawing.Point(24, 178);
            this.checkoutSwitch.Name = "checkoutSwitch";
            this.checkoutSwitch.Size = new System.Drawing.Size(40, 20);
            this.checkoutSwitch.TabIndex = 5;
            //
            // checkoutLabel
            //
            this.checkoutLabel.Location = new System.Drawing.Point(76, 177);
            this.checkoutLabel.Name = "checkoutLabel";
            this.checkoutLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.checkoutLabel.Size = new System.Drawing.Size(240, 22);
            this.checkoutLabel.SizePx = 13F;
            this.checkoutLabel.TabIndex = 6;
            this.checkoutLabel.Text = "Check out after creating";
            //
            // createButton
            //
            this.createButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.createButton.Appearance = GitClient.Controls.ButtonAppearance.Accent;
            this.createButton.CornerRadius = 5;
            this.createButton.Enabled = false;
            this.createButton.Location = new System.Drawing.Point(226, 216);
            this.createButton.Name = "createButton";
            this.createButton.PaddingX = 20;
            this.createButton.Size = new System.Drawing.Size(96, 34);
            this.createButton.TabIndex = 7;
            this.createButton.Text = "Create";
            this.createButton.TextSizePx = 13.5F;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            //
            // cancelButton
            //
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.cancelButton.CornerRadius = 5;
            this.cancelButton.Location = new System.Drawing.Point(330, 216);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.PaddingX = 20;
            this.cancelButton.Size = new System.Drawing.Size(98, 34);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextSizePx = 13.5F;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            //
            // NewBranchDialog
            //
            this.ClientSize = new System.Drawing.Size(452, 299);
            this.content.Location = new System.Drawing.Point(0, 32);
            this.content.Size = new System.Drawing.Size(452, 267);
            this.content.Controls.Add(this.headingLabel);
            this.content.Controls.Add(this.nameLabel);
            this.content.Controls.Add(this.nameBox);
            this.content.Controls.Add(this.startPointLabel);
            this.content.Controls.Add(this.startPointValueLabel);
            this.content.Controls.Add(this.checkoutSwitch);
            this.content.Controls.Add(this.checkoutLabel);
            this.content.Controls.Add(this.createButton);
            this.content.Controls.Add(this.cancelButton);
            this.content.CornerRadius = 0;
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Name = "content";
            this.content.Surface = GitClient.Controls.SurfaceKind.Base;
            this.Controls.Add(this.content);
            this.MinimumSize = new System.Drawing.Size(400, 299);
            this.Name = "NewBranchDialog";
            this.Skin = this.skin;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New branch";
            this.TitleBar.ShowMaximizeBox = false;
            this.TitleBar.ShowMinimizeBox = false;
            this.content.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernSkin skin;
        private GitClient.Controls.SurfacePanel content;
        private GitClient.Controls.TextLabel headingLabel;
        private GitClient.Controls.TextLabel nameLabel;
        private ModernWinForms.ModernTextBox nameBox;
        private GitClient.Controls.TextLabel startPointLabel;
        private GitClient.Controls.TextLabel startPointValueLabel;
        private GitClient.Controls.ToggleSwitchControl checkoutSwitch;
        private GitClient.Controls.TextLabel checkoutLabel;
        private GitClient.Controls.CommandButton createButton;
        private GitClient.Controls.CommandButton cancelButton;
    }
}
