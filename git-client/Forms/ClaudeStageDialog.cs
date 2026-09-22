using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitClient.Controls;
using GitClient.Git;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Forms
{
    /// <summary>
    /// Lets Claude decide what to stage: either what the user describes in their own words, or a
    /// split of all the changes into separate commits. Claude only reads; the plan is shown first,
    /// and applying it only ever adds to the index (and, for a split, commits).
    /// </summary>
    public partial class ClaudeStageDialog : ModernForm
    {
        private const int DescribeMode = 0;
        private const int SplitMode = 1;

        private readonly GitRepository _repository;
        private readonly bool _operationInProgress;
        private readonly string _executable;

        private CancellationTokenSource _asking;
        private bool _applying;
        private bool _finished;
        private StageSnapshot _snapshot;
        private StageSelection _selection;
        private CommitSplit _split;

        public ClaudeStageDialog(GitRepository repository, bool operationInProgress, string executable)
        {
            InitializeComponent();
            Theme.Apply(skin);
            _repository = repository;
            _operationInProgress = operationInProgress;
            _executable = executable;

            StyleBox(promptBox, Fonts.Ui(13.5f));
            StyleBox(planBox, Fonts.Code(12.5f));
        }

        /// <summary>True when anything was staged or committed, so the caller refreshes.</summary>
        public bool Changed { get; private set; }

        /// <summary>Opens straight into the commit split instead of the description.</summary>
        public bool StartWithSplit { get; set; }

        private int Mode => modeToggle.SelectedIndex;

        private static void StyleBox(ModernTextBox box, System.Drawing.Font font)
        {
            var p = Theme.Palette;
            var fill = p.FillOn(p.Background);
            box.UseParentSkin = false;
            box.CornerStyle = ModernWinForms.Enums.CornerStyle.Square;
            box.Colors.BorderColor = System.Drawing.Color.Transparent;
            box.Colors.Normal.BackColor = fill;
            box.Colors.Normal.ForeColor = p.Foreground;
            box.Colors.Hover.BackColor = fill;
            box.Colors.Hover.BorderColor = System.Drawing.Color.Transparent;
            box.Colors.Active.BackColor = fill;
            box.Colors.Active.BorderColor = System.Drawing.Color.Transparent;
            box.ScrollBarColors.ThumbColor = p.Fill2On(fill);
            box.ScrollBarColors.ThumbHoverColor = p.Foreground3;
            box.OverrideSkinFont = true;
            box.Font = font;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            modeToggle.SelectedIndex = StartWithSplit ? SplitMode : DescribeMode;
            ApplyMode();
            promptBox.Focus();
        }

        private void modeToggle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_asking != null) _asking.Cancel();
            ApplyMode();
        }

        private void ApplyMode()
        {
            ClearPlan();
            if (Mode == SplitMode)
            {
                promptLabel.Text = "Anything Claude should know about the split? (optional)";
                promptBox.PlaceholderText = "e.g. keep the version bump in its own commit";
                planBox.Text = "Claude will read all your uncommitted changes and propose separate commits, one per feature " +
                    "or edit, with their messages. You review them here before anything is committed." + Environment.NewLine + Environment.NewLine +
                    "Changes that look like they should not be committed at all (debug output, secrets) are pointed out and left alone.";
            }
            else
            {
                promptLabel.Text = "What should be staged?";
                promptBox.PlaceholderText = "e.g. only the login form validation, not the logging changes";
                planBox.Text = "Describe the work you want staged, then ask Claude. It picks the matching files and the " +
                    "matching parts of files; you review them here before anything is staged." + Environment.NewLine + Environment.NewLine +
                    "For example: \"only the login form validation, not the logging changes\".";
            }
            UpdateButtons();
        }

        private void promptBox_TextChanged(object sender, EventArgs e)
        {
            // A different request makes the old plan meaningless.
            if (_selection != null) ClearPlan();
            UpdateButtons();
        }

        private void ClearPlan()
        {
            _snapshot = null;
            _selection = null;
            _split = null;
            planBox.Text = string.Empty;
            statusLabel.Text = string.Empty;
            askStatusLabel.Text = string.Empty;
        }

        private void UpdateButtons()
        {
            bool asking = _asking != null;
            askButton.Text = asking ? "Stop" : "Ask Claude";
            askButton.Enabled = !_applying && (asking || Mode == SplitMode || promptBox.Text.Trim().Length > 0);
            modeToggle.Enabled = !_applying;
            promptBox.Enabled = !_applying;

            if (Mode == SplitMode)
            {
                int count = _split?.Commits.Count ?? 0;
                applyButton.Text = count == 1 ? "Create 1 commit" : "Create " + (count == 0 ? string.Empty : count + " ") + "commits";
                applyButton.Enabled = !asking && !_applying && count > 0;
            }
            else
            {
                applyButton.Text = "Stage";
                applyButton.Enabled = !asking && !_applying && _selection != null && _selection.Units.Count > 0;
            }
            applyButton.Width = Math.Max(110, applyButton.PreferredWidth);
            applyButton.Left = closeButton.Left - 8 - applyButton.Width;
            closeButton.Enabled = !_applying;
            askButton.Invalidate();
            applyButton.Invalidate();
        }

        // ------------------------------------------------------------------ asking

        private async void askButton_Click(object sender, EventArgs e)
        {
            if (_asking != null)
            {
                _asking.Cancel();
                return;
            }
            if (_executable == null)
            {
                Dialogs.Warning(this, "Claude Code", "The Claude Code CLI was not found.\n\nInstall it, or set the path to claude.exe in Settings.");
                return;
            }

            int mode = Mode;
            ClearPlan();
            _asking = new CancellationTokenSource();
            var token = _asking.Token;
            UpdateButtons();
            try
            {
                if (mode == SplitMode && !await CanSplitAsync()) return;

                askStatusLabel.Text = "Reading your changes...";
                // Read afresh: the user may have staged or edited since the dialog opened.
                var status = await _repository.GetStatusAsync();
                var snapshot = await _repository.GetStageSnapshotAsync(status.Unstaged, token);
                if (!snapshot.Units.Any())
                {
                    askStatusLabel.Text = "There is nothing unstaged to work with.";
                    return;
                }

                int units = snapshot.Units.Count();
                askStatusLabel.Text = "Claude is reading " + Plural(snapshot.Files.Count, "file") + " (" + Plural(units, "change") + ")...";
                if (mode == SplitMode)
                {
                    var recent = await _repository.GetRecentSubjectsAsync(8);
                    var split = await ClaudeStagePlanner.SplitAsync(_repository.WorkingDirectory, snapshot, promptBox.Text, recent,
                        _executable, AppSettings.Current.ClaudeModel, token);
                    if (token.IsCancellationRequested || Mode != mode) return;
                    _snapshot = snapshot;
                    _split = split;
                    planBox.Text = DescribeSplit(snapshot, split);
                    askStatusLabel.Text = "Claude proposes " + Plural(split.Commits.Count, "commit") + ". Review them below.";
                }
                else
                {
                    var selection = await ClaudeStagePlanner.SelectAsync(_repository.WorkingDirectory, snapshot, promptBox.Text,
                        _executable, AppSettings.Current.ClaudeModel, token);
                    if (token.IsCancellationRequested || Mode != mode) return;
                    _snapshot = snapshot;
                    _selection = selection;
                    planBox.Text = DescribeSelection(snapshot, selection);
                    askStatusLabel.Text = selection.Units.Count == 0
                        ? "Claude found nothing matching that description."
                        : "Claude picked " + selection.Units.Count + " of " + Plural(units, "change") + ". Review them below.";
                }
            }
            catch (OperationCanceledException)
            {
                askStatusLabel.Text = "Stopped.";
            }
            catch (Exception ex)
            {
                if (token.IsCancellationRequested) askStatusLabel.Text = "Stopped.";
                else
                {
                    askStatusLabel.Text = string.Empty;
                    Dialogs.Error(this, "Claude Code", ex.Message);
                }
            }
            finally
            {
                _asking?.Dispose();
                _asking = null;
                if (!IsDisposed) UpdateButtons();
            }
        }

        /// <summary>
        /// Each commit of a split is made from the index, so it has to start out empty, and not in
        /// the middle of a merge or rebase.
        /// </summary>
        private async Task<bool> CanSplitAsync()
        {
            if (_operationInProgress)
            {
                askStatusLabel.Text = "Finish or abort the merge, rebase or cherry-pick in progress first.";
                return false;
            }
            if (await _repository.HasStagedChangesAsync())
            {
                askStatusLabel.Text = "Some changes are already staged. Commit or unstage them first — unstaging never touches your files.";
                return false;
            }
            return true;
        }

        // ------------------------------------------------------------------ the plan

        private static string DescribeSelection(StageSnapshot snapshot, StageSelection selection)
        {
            var text = new StringBuilder();
            if (selection.Note.Length > 0)
            {
                text.AppendLine("Claude: " + selection.Note);
                text.AppendLine();
            }
            var chosen = new HashSet<StageUnit>(selection.Units);
            if (chosen.Count > 0)
            {
                text.AppendLine("WILL BE STAGED");
                AppendUnits(text, snapshot.Units.Where(chosen.Contains), "  ");
                text.AppendLine();
            }
            var rest = snapshot.Units.Where(u => !chosen.Contains(u)).ToList();
            if (rest.Count > 0)
            {
                text.AppendLine("STAYS UNSTAGED (still in your files, as it is now)");
                AppendUnits(text, rest, "  ");
            }
            AppendSkipped(text, snapshot);
            return Crlf(text);
        }

        private static string DescribeSplit(StageSnapshot snapshot, CommitSplit split)
        {
            var text = new StringBuilder();
            for (int i = 0; i < split.Commits.Count; i++)
            {
                var commit = split.Commits[i];
                text.AppendLine((i + 1) + ". " + commit.Subject);
                foreach (var line in commit.Body.Split('\n').Where(l => l.Trim().Length > 0)) text.AppendLine("   " + line.Trim());
                AppendUnits(text, snapshot.Units.Where(commit.Units.Contains), "     ");
                text.AppendLine();
            }

            if (split.LeftOut.Count > 0)
            {
                text.AppendLine("NOT COMMITTED, ON CLAUDE'S ADVICE (stays in your files, unstaged)");
                if (split.LeftOutReason.Length > 0) text.AppendLine("   " + split.LeftOutReason);
                AppendUnits(text, snapshot.Units.Where(split.LeftOut.Contains), "     ");
                text.AppendLine();
            }

            var placed = new HashSet<StageUnit>(split.Commits.SelectMany(c => c.Units).Concat(split.LeftOut));
            var missing = snapshot.Units.Where(u => !placed.Contains(u)).ToList();
            if (missing.Count > 0)
            {
                text.AppendLine("NOT PLACED IN ANY COMMIT (stays in your files, unstaged)");
                AppendUnits(text, missing, "     ");
            }
            AppendSkipped(text, snapshot);
            return Crlf(text);
        }

        private static void AppendUnits(StringBuilder text, IEnumerable<StageUnit> units, string indent)
        {
            foreach (var group in units.GroupBy(u => u.File))
            {
                var file = group.Key;
                var list = group.ToList();
                bool all = list.Count == file.Units.Count;
                if (list.Count == 1 && list[0].Kind == StageUnitKind.WholeFile)
                {
                    text.AppendLine(indent + file.Path + "  (" + list[0].Label + ")");
                    continue;
                }
                text.AppendLine(indent + file.Path + (all ? "  (all changes)" : "  (" + list.Count + " of " + file.Units.Count + " changes)"));
                foreach (var unit in list)
                {
                    text.AppendLine(indent + "  · " + unit.Label + "  +" + unit.Additions + " -" + unit.Deletions + FirstChangedLine(unit));
                }
            }
        }

        /// <summary>The first added line (or removed, when nothing was added), to recognise the hunk by.</summary>
        private static string FirstChangedLine(StageUnit unit)
        {
            var lines = (unit.Patch ?? string.Empty).Split('\n').Skip(1).ToList();
            foreach (var sign in new[] { '+', '-' })
            {
                foreach (var raw in lines)
                {
                    if (raw.Length < 2 || raw[0] != sign) continue;
                    var body = raw.Substring(1).Trim();
                    if (body.Length == 0) continue;
                    if (body.Length > 70) body = body.Substring(0, 70) + "…";
                    return "   " + sign + " " + body;
                }
            }
            return string.Empty;
        }

        private static void AppendSkipped(StringBuilder text, StageSnapshot snapshot)
        {
            if (snapshot.Skipped.Count == 0) return;
            text.AppendLine();
            text.AppendLine("NOT OFFERED TO CLAUDE");
            foreach (var line in snapshot.Skipped) text.AppendLine("  " + line);
        }

        // ------------------------------------------------------------------ applying

        private async void applyButton_Click(object sender, EventArgs e)
        {
            if (_applying || _snapshot == null) return;
            _applying = true;
            UpdateButtons();
            try
            {
                if (Mode == SplitMode && _split != null) await ApplySplitAsync(_split);
                else if (_selection != null) await ApplySelectionAsync(_selection);
            }
            catch (Exception ex)
            {
                Dialogs.Error(this, "Stage with Claude", ex.Message + "\n\nYour files were not changed.");
            }
            finally
            {
                _applying = false;
                if (!IsDisposed) UpdateButtons();
            }
        }

        private async Task ApplySelectionAsync(StageSelection selection)
        {
            statusLabel.Text = "Staging...";
            var outcome = await _repository.StageUnitsAsync(selection.Units, CancellationToken.None);
            if (outcome.StagedUnits > 0) Changed = true;
            if (outcome.Skipped.Count == 0)
            {
                _finished = true;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
            ClearPlan();
            planBox.Text = Crlf(new StringBuilder()
                .AppendLine("Staged " + Plural(outcome.StagedUnits, "change") + ".")
                .AppendLine()
                .AppendLine("LEFT UNSTAGED (nothing in your files changed)")
                .Append(string.Concat(outcome.Skipped.Select(s => "  " + s + "\n"))));
            statusLabel.Text = "Some files were left unstaged.";
        }

        private async Task ApplySplitAsync(CommitSplit split)
        {
            if (!await CanSplitAsync())
            {
                statusLabel.Text = askStatusLabel.Text;
                return;
            }

            var report = new StringBuilder();
            int made = 0;
            for (int i = 0; i < split.Commits.Count; i++)
            {
                var commit = split.Commits[i];
                statusLabel.Text = "Commit " + (i + 1) + " of " + split.Commits.Count + ": " + commit.Subject;
                var outcome = await _repository.StageUnitsAsync(commit.Units, CancellationToken.None);
                if (outcome.StagedUnits > 0) Changed = true;

                if (outcome.Skipped.Count > 0)
                {
                    // Committing only part of what the message describes would be wrong: stop here,
                    // with whatever was staged for this commit still staged.
                    report.AppendLine("Stopped before commit " + (i + 1) + " (\"" + commit.Subject + "\"):");
                    foreach (var line in outcome.Skipped) report.AppendLine("  " + line);
                    if (outcome.StagedUnits > 0) report.AppendLine("  The rest of it is staged, ready for you to finish by hand.");
                    break;
                }
                if (outcome.StagedUnits == 0 || !await _repository.HasStagedChangesAsync())
                {
                    report.AppendLine("Skipped commit " + (i + 1) + " (\"" + commit.Subject + "\"): nothing left to stage for it.");
                    continue;
                }

                try
                {
                    await _repository.CommitAsync(commit.Message, false);
                }
                catch (Exception ex)
                {
                    report.AppendLine("Commit " + (i + 1) + " (\"" + commit.Subject + "\") failed: " + ex.Message.Trim());
                    report.AppendLine("  Its changes are staged.");
                    break;
                }
                made++;
                report.AppendLine("Committed " + (i + 1) + ". " + commit.Subject);
            }

            bool complete = made == split.Commits.Count;
            if (complete)
            {
                _finished = true;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
            ClearPlan();
            planBox.Text = Crlf(report.AppendLine().AppendLine("Nothing in your files changed; whatever was not committed is still there."));
            statusLabel.Text = "Made " + Plural(made, "commit") + " of " + split.Commits.Count + ".";
        }

        // ------------------------------------------------------------------ misc

        private void closeButton_Click(object sender, EventArgs e)
        {
            if (_applying) return;
            if (_asking != null) _asking.Cancel();
            DialogResult = Changed ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Half-way through a split the index is in use; let it finish.
            if (_applying && !_finished) e.Cancel = true;
            else if (_asking != null) _asking.Cancel();
            base.OnFormClosing(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                closeButton_Click(closeButton, EventArgs.Empty);
                return true;
            }
            if (keyData == (Keys.Control | Keys.Enter))
            {
                if (askButton.Enabled) askButton_Click(askButton, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private static string Plural(int count, string noun) => count + " " + noun + (count == 1 ? string.Empty : "s");

        private static string Crlf(StringBuilder text) =>
            text.ToString().Replace("\r\n", "\n").Replace("\n", Environment.NewLine).TrimEnd();
    }
}
