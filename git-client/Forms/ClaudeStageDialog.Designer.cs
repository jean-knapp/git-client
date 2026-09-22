namespace GitClient.Forms
{
    partial class ClaudeStageDialog
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
            this.skin = new ModernWinForms.ModernSkin();
            this.content = new GitClient.Controls.SurfacePanel();
            this.headingLabel = new GitClient.Controls.TextLabel();
            this.introLabel = new GitClient.Controls.TextLabel();
            this.modeToggle = new GitClient.Controls.SegmentedControl();
            this.promptLabel = new GitClient.Controls.TextLabel();
            this.promptCard = new GitClient.Controls.SurfacePanel();
            this.promptBox = new ModernWinForms.ModernTextBox();
            this.askButton = new GitClient.Controls.CommandButton();
            this.askStatusLabel = new GitClient.Controls.TextLabel();
            this.planLabel = new GitClient.Controls.TextLabel();
            this.planCard = new GitClient.Controls.SurfacePanel();
            this.planBox = new ModernWinForms.ModernTextBox();
            this.statusLabel = new GitClient.Controls.TextLabel();
            this.applyButton = new GitClient.Controls.CommandButton();
            this.closeButton = new GitClient.Controls.CommandButton();
            this.content.SuspendLayout();
            this.promptCard.SuspendLayout();
            this.planCard.SuspendLayout();
            this.SuspendLayout();
            //
            // headingLabel
            //
            this.headingLabel.Location = new System.Drawing.Point(20, 14);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Semibold = true;
            this.headingLabel.Size = new System.Drawing.Size(500, 28);
            this.headingLabel.SizePx = 20F;
            this.headingLabel.TabIndex = 0;
            this.headingLabel.Text = "Stage with Claude";
            //
            // introLabel
            //
            this.introLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.introLabel.Location = new System.Drawing.Point(20, 44);
            this.introLabel.MultiLine = true;
            this.introLabel.Name = "introLabel";
            this.introLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.introLabel.Size = new System.Drawing.Size(740, 36);
            this.introLabel.SizePx = 12.5F;
            this.introLabel.TabIndex = 1;
            this.introLabel.Text = "Claude only reads your changes and answers with a plan. Nothing happens until you apply it, and then only the stagin" +
    "g area changes: your files on disk are never modified, and nothing already staged is taken out.";
            //
            // modeToggle
            //
            this.modeToggle.Items.Add("Stage what I describe");
            this.modeToggle.Items.Add("Split into commits");
            this.modeToggle.Location = new System.Drawing.Point(20, 88);
            this.modeToggle.Name = "modeToggle";
            this.modeToggle.SegmentPadding = 16;
            this.modeToggle.Size = new System.Drawing.Size(320, 30);
            this.modeToggle.TabIndex = 2;
            this.modeToggle.TextSizePx = 13F;
            this.modeToggle.SelectedIndexChanged += new System.EventHandler(this.modeToggle_SelectedIndexChanged);
            //
            // promptLabel
            //
            this.promptLabel.Location = new System.Drawing.Point(20, 130);
            this.promptLabel.Name = "promptLabel";
            this.promptLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.promptLabel.Size = new System.Drawing.Size(740, 18);
            this.promptLabel.SizePx = 12.5F;
            this.promptLabel.TabIndex = 3;
            this.promptLabel.Text = "What should be staged?";
            //
            // promptCard
            //
            this.promptCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.promptCard.Controls.Add(this.promptBox);
            this.promptCard.CornerRadius = 6;
            this.promptCard.Location = new System.Drawing.Point(20, 152);
            this.promptCard.Name = "promptCard";
            this.promptCard.Padding = new System.Windows.Forms.Padding(1);
            this.promptCard.Size = new System.Drawing.Size(740, 78);
            this.promptCard.Surface = GitClient.Controls.SurfaceKind.Fill;
            this.promptCard.TabIndex = 4;
            //
            // promptBox
            //
            this.promptBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.promptBox.Location = new System.Drawing.Point(1, 1);
            this.promptBox.Multiline = true;
            this.promptBox.Name = "promptBox";
            this.promptBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.promptBox.Size = new System.Drawing.Size(738, 76);
            this.promptBox.TabIndex = 0;
            this.promptBox.TextChanged += new System.EventHandler(this.promptBox_TextChanged);
            //
            // askButton
            //
            this.askButton.Appearance = GitClient.Controls.ButtonAppearance.Accent;
            this.askButton.CornerRadius = 5;
            this.askButton.Gap = 7;
            this.askButton.IconSize = 14;
            this.askButton.IconSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path fill=\"currentColor" +
    "\" d=\"M12 2l2.2 6.3 6.3 2.2-6.3 2.2L12 19l-2.2-6.3-6.3-2.2 6.3-2.2z\"/><path fill=\"curre" +
    "ntColor\" d=\"M19 15l.9 2.6 2.6.9-2.6.9L19 22l-.9-2.6-2.6-.9 2.6-.9z\"/></svg>";
            this.askButton.Location = new System.Drawing.Point(20, 242);
            this.askButton.Name = "askButton";
            this.askButton.PaddingX = 16;
            this.askButton.Size = new System.Drawing.Size(130, 34);
            this.askButton.TabIndex = 5;
            this.askButton.Text = "Ask Claude";
            this.askButton.TextSizePx = 13.5F;
            this.askButton.Click += new System.EventHandler(this.askButton_Click);
            //
            // askStatusLabel
            //
            this.askStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.askStatusLabel.Location = new System.Drawing.Point(164, 250);
            this.askStatusLabel.Name = "askStatusLabel";
            this.askStatusLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.askStatusLabel.Size = new System.Drawing.Size(596, 18);
            this.askStatusLabel.SizePx = 12.5F;
            this.askStatusLabel.TabIndex = 6;
            //
            // planLabel
            //
            this.planLabel.Location = new System.Drawing.Point(20, 292);
            this.planLabel.Name = "planLabel";
            this.planLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.planLabel.Size = new System.Drawing.Size(740, 18);
            this.planLabel.SizePx = 12.5F;
            this.planLabel.TabIndex = 7;
            this.planLabel.Text = "Plan";
            //
            // planCard
            //
            this.planCard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.planCard.Controls.Add(this.planBox);
            this.planCard.CornerRadius = 6;
            this.planCard.Location = new System.Drawing.Point(20, 314);
            this.planCard.Name = "planCard";
            this.planCard.Padding = new System.Windows.Forms.Padding(1);
            this.planCard.Size = new System.Drawing.Size(740, 262);
            this.planCard.Surface = GitClient.Controls.SurfaceKind.Fill;
            this.planCard.TabIndex = 8;
            //
            // planBox
            //
            this.planBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.planBox.Location = new System.Drawing.Point(1, 1);
            this.planBox.Multiline = true;
            this.planBox.Name = "planBox";
            this.planBox.ReadOnly = true;
            this.planBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.planBox.Size = new System.Drawing.Size(738, 260);
            this.planBox.TabIndex = 0;
            //
            // statusLabel
            //
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.Location = new System.Drawing.Point(20, 602);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Role = GitClient.Controls.TextRole.Tertiary;
            this.statusLabel.Size = new System.Drawing.Size(460, 18);
            this.statusLabel.SizePx = 12.5F;
            this.statusLabel.TabIndex = 9;
            //
            // applyButton
            //
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.Appearance = GitClient.Controls.ButtonAppearance.Accent;
            this.applyButton.CornerRadius = 5;
            this.applyButton.Enabled = false;
            this.applyButton.Location = new System.Drawing.Point(488, 594);
            this.applyButton.Name = "applyButton";
            this.applyButton.PaddingX = 20;
            this.applyButton.Size = new System.Drawing.Size(166, 34);
            this.applyButton.TabIndex = 10;
            this.applyButton.Text = "Stage";
            this.applyButton.TextSizePx = 13.5F;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            //
            // closeButton
            //
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.closeButton.CornerRadius = 5;
            this.closeButton.Location = new System.Drawing.Point(662, 594);
            this.closeButton.Name = "closeButton";
            this.closeButton.PaddingX = 20;
            this.closeButton.Size = new System.Drawing.Size(98, 34);
            this.closeButton.TabIndex = 11;
            this.closeButton.Text = "Close";
            this.closeButton.TextSizePx = 13.5F;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            //
            // ClaudeStageDialog
            //
            this.ClientSize = new System.Drawing.Size(780, 680);
            this.content.Location = new System.Drawing.Point(0, 32);
            this.content.Size = new System.Drawing.Size(780, 648);
            this.content.Controls.Add(this.headingLabel);
            this.content.Controls.Add(this.introLabel);
            this.content.Controls.Add(this.modeToggle);
            this.content.Controls.Add(this.promptLabel);
            this.content.Controls.Add(this.promptCard);
            this.content.Controls.Add(this.askButton);
            this.content.Controls.Add(this.askStatusLabel);
            this.content.Controls.Add(this.planLabel);
            this.content.Controls.Add(this.planCard);
            this.content.Controls.Add(this.statusLabel);
            this.content.Controls.Add(this.applyButton);
            this.content.Controls.Add(this.closeButton);
            this.content.CornerRadius = 0;
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Name = "content";
            this.content.Surface = GitClient.Controls.SurfaceKind.Base;
            this.Controls.Add(this.content);
            this.MinimumSize = new System.Drawing.Size(620, 560);
            this.Name = "ClaudeStageDialog";
            this.Skin = this.skin;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Stage with Claude";
            this.TitleBar.ShowMaximizeBox = false;
            this.TitleBar.ShowMinimizeBox = false;
            this.planCard.ResumeLayout(false);
            this.promptCard.ResumeLayout(false);
            this.content.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernSkin skin;
        private GitClient.Controls.SurfacePanel content;
        private GitClient.Controls.TextLabel headingLabel;
        private GitClient.Controls.TextLabel introLabel;
        private GitClient.Controls.SegmentedControl modeToggle;
        private GitClient.Controls.TextLabel promptLabel;
        private GitClient.Controls.SurfacePanel promptCard;
        private ModernWinForms.ModernTextBox promptBox;
        private GitClient.Controls.CommandButton askButton;
        private GitClient.Controls.TextLabel askStatusLabel;
        private GitClient.Controls.TextLabel planLabel;
        private GitClient.Controls.SurfacePanel planCard;
        private ModernWinForms.ModernTextBox planBox;
        private GitClient.Controls.TextLabel statusLabel;
        private GitClient.Controls.CommandButton applyButton;
        private GitClient.Controls.CommandButton closeButton;
    }
}
