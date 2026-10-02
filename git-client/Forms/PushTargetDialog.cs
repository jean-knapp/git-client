using System;
using System.Drawing;
using System.Windows.Forms;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Forms
{
    public enum PushTarget
    {
        None,

        /// <summary>Push to the remote branch the local one already tracks, whatever its name.</summary>
        TrackedBranch,

        /// <summary>Push to a remote branch with the local branch's name, and track that instead.</summary>
        SameName,

        /// <summary>Rename the local branch to the tracked branch's name, then push as usual.</summary>
        RenameLocal,
    }

    /// <summary>
    /// Asked when a branch tracks a remote branch of another name and git will not guess where to
    /// push it: the tracked branch, a new branch of the same name, or renaming the local branch.
    /// </summary>
    public partial class PushTargetDialog : ModernForm
    {
        public PushTargetDialog(string localBranch, string remote, string trackedBranch, bool canRename)
        {
            InitializeComponent();
            Theme.Apply(skin);

            var tracked = remote + "/" + trackedBranch;
            explanationLabel.Text = "Your branch " + localBranch + " tracks " + tracked + ", which has a different name, so git will not " +
                                    "guess where to push it. Choose where it goes:";

            upstreamCard.Title = "Push to " + tracked;
            upstreamCard.Description = "The branch it already tracks. Nothing is renamed.";
            sameNameCard.Title = "Push as " + remote + "/" + localBranch;
            sameNameCard.Description = "A remote branch with the local name, tracked from now on.";
            renameCard.Title = "Rename " + localBranch + " to " + trackedBranch;
            renameCard.Description = canRename
                ? "The names match for good, then it pushes."
                : "Not available: a local " + trackedBranch + " already exists.";
            renameCard.Enabled = canRename;

            var p = Theme.Palette;
            upstreamCard.TileFill = Color.FromArgb(36, p.AccentFill);
            sameNameCard.TileFill = Color.FromArgb(41, p.Lane2);
            renameCard.TileFill = Color.FromArgb(36, p.Added);
        }

        public PushTarget Target { get; private set; }

        /// <summary>
        /// With <see cref="PushTarget.TrackedBranch"/>: set <c>push.default = upstream</c> for the
        /// repository, so a plain push always goes to the branch a branch tracks.
        /// </summary>
        public bool Remember => rememberBox.Checked;

        private void Choose(PushTarget target)
        {
            Target = target;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void upstreamCard_Click(object sender, EventArgs e) => Choose(PushTarget.TrackedBranch);
        private void sameNameCard_Click(object sender, EventArgs e) => Choose(PushTarget.SameName);
        private void renameCard_Click(object sender, EventArgs e) { if (renameCard.Enabled) Choose(PushTarget.RenameLocal); }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                cancelButton_Click(cancelButton, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
