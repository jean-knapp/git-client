namespace GitClient.Forms
{
    partial class PushTargetDialog
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
            this.explanationLabel = new GitClient.Controls.TextLabel();
            this.upstreamCard = new GitClient.Controls.ActionCard();
            this.sameNameCard = new GitClient.Controls.ActionCard();
            this.renameCard = new GitClient.Controls.ActionCard();
            this.rememberBox = new GitClient.Controls.TokenCheckBox();
            this.cancelButton = new GitClient.Controls.CommandButton();
            this.content.SuspendLayout();
            this.SuspendLayout();
            //
            // headingLabel
            //
            this.headingLabel.Location = new System.Drawing.Point(20, 14);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Semibold = true;
            this.headingLabel.Size = new System.Drawing.Size(560, 28);
            this.headingLabel.SizePx = 20F;
            this.headingLabel.TabIndex = 0;
            this.headingLabel.Text = "Where should this branch go?";
            //
            // explanationLabel
            //
            this.explanationLabel.Location = new System.Drawing.Point(20, 46);
            this.explanationLabel.MultiLine = true;
            this.explanationLabel.Name = "explanationLabel";
            this.explanationLabel.Role = GitClient.Controls.TextRole.Secondary;
            this.explanationLabel.Size = new System.Drawing.Size(560, 54);
            this.explanationLabel.SizePx = 13F;
            this.explanationLabel.TabIndex = 1;
            //
            // upstreamCard
            //
            this.upstreamCard.IconSvg = GitClient.Controls.Icons.Push;
            this.upstreamCard.Location = new System.Drawing.Point(20, 110);
            this.upstreamCard.Name = "upstreamCard";
            this.upstreamCard.Size = new System.Drawing.Size(560, 76);
            this.upstreamCard.TabIndex = 2;
            this.upstreamCard.Click += new System.EventHandler(this.upstreamCard_Click);
            //
            // sameNameCard
            //
            this.sameNameCard.IconSvg = GitClient.Controls.Icons.Branch;
            this.sameNameCard.Location = new System.Drawing.Point(20, 194);
            this.sameNameCard.Name = "sameNameCard";
            this.sameNameCard.Size = new System.Drawing.Size(560, 76);
            this.sameNameCard.TabIndex = 3;
            this.sameNameCard.Click += new System.EventHandler(this.sameNameCard_Click);
            //
            // renameCard
            //
            this.renameCard.IconSvg = GitClient.Controls.Icons.Edit;
            this.renameCard.Location = new System.Drawing.Point(20, 278);
            this.renameCard.Name = "renameCard";
            this.renameCard.Size = new System.Drawing.Size(560, 76);
            this.renameCard.TabIndex = 4;
            this.renameCard.Click += new System.EventHandler(this.renameCard_Click);
            //
            // rememberBox
            //
            this.rememberBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rememberBox.Location = new System.Drawing.Point(20, 374);
            this.rememberBox.Name = "rememberBox";
            this.rememberBox.Size = new System.Drawing.Size(440, 22);
            this.rememberBox.TabIndex = 5;
            this.rememberBox.Text = "With the first choice, do it every time in this repository";
            //
            // cancelButton
            //
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Appearance = GitClient.Controls.ButtonAppearance.Standard;
            this.cancelButton.CornerRadius = 5;
            this.cancelButton.Location = new System.Drawing.Point(482, 368);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.PaddingX = 20;
            this.cancelButton.Size = new System.Drawing.Size(98, 34);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.TextSizePx = 13.5F;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            //
            // PushTargetDialog
            //
            this.ClientSize = new System.Drawing.Size(600, 454);
            this.content.Location = new System.Drawing.Point(0, 32);
            this.content.Size = new System.Drawing.Size(600, 422);
            this.content.Controls.Add(this.headingLabel);
            this.content.Controls.Add(this.explanationLabel);
            this.content.Controls.Add(this.upstreamCard);
            this.content.Controls.Add(this.sameNameCard);
            this.content.Controls.Add(this.renameCard);
            this.content.Controls.Add(this.rememberBox);
            this.content.Controls.Add(this.cancelButton);
            this.content.CornerRadius = 0;
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Name = "content";
            this.content.Surface = GitClient.Controls.SurfaceKind.Base;
            this.Controls.Add(this.content);
            this.Name = "PushTargetDialog";
            this.Skin = this.skin;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Push";
            this.TitleBar.ShowMaximizeBox = false;
            this.TitleBar.ShowMinimizeBox = false;
            this.content.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private ModernWinForms.ModernSkin skin;
        private GitClient.Controls.SurfacePanel content;
        private GitClient.Controls.TextLabel headingLabel;
        private GitClient.Controls.TextLabel explanationLabel;
        private GitClient.Controls.ActionCard upstreamCard;
        private GitClient.Controls.ActionCard sameNameCard;
        private GitClient.Controls.ActionCard renameCard;
        private GitClient.Controls.TokenCheckBox rememberBox;
        private GitClient.Controls.CommandButton cancelButton;
    }
}
