namespace GitClient.Forms
{
    partial class GitIgnoreDialog
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
            this.pathLabel = new GitClient.Controls.TextLabel();
            this.editorCard = new GitClient.Controls.SurfacePanel();
            this.editorBox = new ModernWinForms.ModernTextBox();
            this.templateButton = new GitClient.Controls.CommandButton();
            this.saveTemplateButton = new GitClient.Controls.CommandButton();
            this.statusLabel = new GitClient.Controls.TextLabel();
            this.saveButton = new GitClient.Controls.CommandButton();
            this.cancelButton = new GitClient.Controls.CommandButton();
            this.templateMenu = new ModernWinForms.ModernContextMenu(this.components);
            this.content.SuspendLayout();
            this.editorCard.SuspendLayout();
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
            this.headingLabel.Text = "Edit .gitignore";
            //
            // pathLabel
            //
            this.pathLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathLabel.Location = new System.Drawing.Point(20, 42);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.pathLabel.Size = new System.Drawing.Size(700, 18);
            this.pathLabel.SizePx = 12.5F;
            this.pathLabel.TabIndex = 1;
            //
            // editorCard
            //
            this.editorCard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editorCard.Controls.Add(this.editorBox);
            this.editorCard.CornerRadius = 6;
            this.editorCard.Location = new System.Drawing.Point(20, 70);
            this.editorCard.Name = "editorCard";
            this.editorCard.Padding = new System.Windows.Forms.Padding(1);
            this.editorCard.Size = new System.Drawing.Size(700, 396);
            this.editorCard.Surface = GitClient.Controls.SurfaceKind.Fill;
            this.editorCard.TabIndex = 2;
            //
            // editorBox
            //
            this.editorBox.AcceptsTab = true;
            this.editorBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorBox.Location = new System.Drawing.Point(1, 1);
            this.editorBox.Multiline = true;
            this.editorBox.Name = "editorBox";
            this.editorBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.editorBox.Size = new System.Drawing.Size(698, 394);
            this.editorBox.TabIndex = 0;
            this.editorBox.WordWrap = false;
            this.editorBox.TextChanged += new System.EventHandler(this.editorBox_TextChanged);
            //
            // templateButton
            //
            this.templateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.templateButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.templateButton.Chevron = GitClient.Controls.ChevronMode.Trailing;
            this.templateButton.CornerRadius = 5;
            this.templateButton.Location = new System.Drawing.Point(20, 478);
            this.templateButton.Name = "templateButton";
            this.templateButton.PaddingX = 14;
            this.templateButton.Size = new System.Drawing.Size(150, 34);
            this.templateButton.TabIndex = 3;
            this.templateButton.Text = "Insert template";
            this.templateButton.TextSizePx = 13.5F;
            this.templateButton.Click += new System.EventHandler(this.templateButton_Click);
            //
            // saveTemplateButton
            //
            this.saveTemplateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveTemplateButton.Appearance = GitClient.Controls.ButtonAppearance.Subtle;
            this.saveTemplateButton.CornerRadius = 5;
            this.saveTemplateButton.Location = new System.Drawing.Point(178, 478);
            this.saveTemplateButton.Name = "saveTemplateButton";
            this.saveTemplateButton.PaddingX = 14;
            this.saveTemplateButton.Size = new System.Drawing.Size(168, 34);
            this.saveTemplateButton.TabIndex = 4;
            this.saveTemplateButton.Text = "Save as template...";
            this.saveTemplateButton.TextSizePx = 13.5F;
            this.saveTemplateButton.Click += new System.EventHandler(this.saveTemplateButton_Click);
            //
            // statusLabel
            //
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.Location = new System.Drawing.Point(20, 522);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.statusLabel.Size = new System.Drawing.Size(700, 18);
            this.statusLabel.SizePx = 12.5F;
            this.statusLabel.TabIndex = 5;
            //
            // saveButton
            //
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Appearance = GitClient.Controls.ButtonAppearance.Accent;
            this.saveButton.CornerRadius = 5;
            this.saveButton.Location = new System.Drawing.Point(518, 478);
            this.saveButton.Name = "saveButton";
            this.saveButton.PaddingX = 20;
            this.saveButton.Size = new System.Drawing.Size(96, 34);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "Save";
            this.saveButton.TextSizePx = 13.5F;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            //
            // cancelButton
            //
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.cancelButton.CornerRadius = 5;
            this.cancelButton.Location = new System.Drawing.Point(622, 478);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.PaddingX = 20;
            this.cancelButton.Size = new System.Drawing.Size(98, 34);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextSizePx = 13.5F;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            //
            // GitIgnoreDialog
            //
            this.ClientSize = new System.Drawing.Size(740, 584);
            this.content.Location = new System.Drawing.Point(0, 32);
            this.content.Size = new System.Drawing.Size(740, 552);
            this.content.Controls.Add(this.headingLabel);
            this.content.Controls.Add(this.pathLabel);
            this.content.Controls.Add(this.editorCard);
            this.content.Controls.Add(this.templateButton);
            this.content.Controls.Add(this.saveTemplateButton);
            this.content.Controls.Add(this.statusLabel);
            this.content.Controls.Add(this.saveButton);
            this.content.Controls.Add(this.cancelButton);
            this.content.CornerRadius = 0;
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Name = "content";
            this.content.Surface = GitClient.Controls.SurfaceKind.Base;
            this.Controls.Add(this.content);
            this.MinimumSize = new System.Drawing.Size(560, 420);
            this.Name = "GitIgnoreDialog";
            this.Skin = this.skin;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit .gitignore";
            this.TitleBar.ShowMaximizeBox = false;
            this.TitleBar.ShowMinimizeBox = false;
            this.editorCard.ResumeLayout(false);
            this.content.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernSkin skin;
        private GitClient.Controls.SurfacePanel content;
        private GitClient.Controls.TextLabel headingLabel;
        private GitClient.Controls.TextLabel pathLabel;
        private GitClient.Controls.SurfacePanel editorCard;
        private ModernWinForms.ModernTextBox editorBox;
        private GitClient.Controls.CommandButton templateButton;
        private GitClient.Controls.CommandButton saveTemplateButton;
        private GitClient.Controls.TextLabel statusLabel;
        private GitClient.Controls.CommandButton saveButton;
        private GitClient.Controls.CommandButton cancelButton;
        private ModernWinForms.ModernContextMenu templateMenu;
    }
}
