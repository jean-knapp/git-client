using System;
using System.Collections.Generic;
using System.Linq;
using GitClient.Git;

namespace GitClient.Graph
{
    /// <summary>A line segment drawn between one row and the next.</summary>
    public sealed class GraphEdge
    {
        public int FromColumn { get; set; }
        public int ToColumn { get; set; }

        /// <summary>Lane palette index, or <see cref="GraphLayout.StashColor"/> for a stash's line.</summary>
        public int ColorIndex { get; set; }
        public bool Dashed { get; set; }

        /// <summary>The line leads only from commits the checked-out branch does not contain.</summary>
        public bool OffHead { get; set; }
    }

    /// <summary>One row of the commit graph: a commit, a stash or the work-in-progress marker, and its outgoing edges.</summary>
    public sealed class GraphRow
    {
        public int Index { get; set; }

        /// <summary>The commit on this row; null on the work-in-progress and stash rows.</summary>
        public CommitInfo Commit { get; set; }

        /// <summary>The stash on this row, drawn above the commit it was made on.</summary>
        public StashInfo Stash { get; set; }

        public bool IsWorkInProgress { get; set; }
        public int Column { get; set; }
        public int ColorIndex { get; set; }

        /// <summary>
        /// The checked-out branch does not contain this commit (or, for a stash, the commit it was
        /// made on), so it is drawn greyed out.
        /// </summary>
        public bool OffHead { get; set; }

        /// <summary>The line arriving at this row's node comes only from commits off the checked-out branch.</summary>
        public bool StemOffHead { get; set; }

        /// <summary>
        /// A line comes down into this row's node from the row above. Not so for a branch tip, or
        /// for the checked-out commit when nothing above leads to it.
        /// </summary>
        public bool HasStem { get; set; }

        /// <summary>Only stash lines arrive at this row's node, so its stem is drawn as theirs.</summary>
        public bool StemFromStash { get; set; }

        /// <summary>Edges leaving this row towards the next row.</summary>
        public List<GraphEdge> Edges { get; } = new List<GraphEdge>();

        public string Sha => Commit?.Sha ?? Stash?.Sha;
    }

    /// <summary>
    /// Assigns lanes (columns) to commits so the history can be drawn as a railway-style graph.
    /// Commits must be supplied newest-first with every child before its parents (git log --date-order).
    /// </summary>
    public sealed class GraphLayout
    {
        /// <summary>The colour index of a stash's dashed line, which is drawn neutral rather than taking a lane colour.</summary>
        public const int StashColor = -1;

        private sealed class Lane
        {
            public string Expected;
            public int ColorIndex;
            public bool Started;

            /// <summary>A stash's line: dashed, neutral, and never continued by a commit.</summary>
            public bool IsStash;

            /// <summary>At least one commit on the checked-out branch feeds this line.</summary>
            public bool OnHead;
        }

        /// <summary>A row to lay out: a commit, or a stash standing in as one with its base as the only parent.</summary>
        private sealed class Entry
        {
            public string Sha;
            public IList<string> Parents;
            public CommitInfo Commit;
            public StashInfo Stash;
        }

        public List<GraphRow> Rows { get; } = new List<GraphRow>();
        public Dictionary<string, GraphRow> RowsBySha { get; } = new Dictionary<string, GraphRow>(StringComparer.Ordinal);
        public int LaneCount { get; private set; }
        public GraphRow WorkInProgressRow { get; private set; }

        /// <summary>Stashes whose base commit is in the history, and so have a row.</summary>
        public int StashCount { get; private set; }

        public static GraphLayout Build(IList<CommitInfo> commits, string headSha, bool includeWorkInProgress, IList<StashInfo> stashes = null)
        {
            var layout = new GraphLayout();
            var lanes = new List<Lane>();
            int nextColor = 0;
            var onHead = ReachableFrom(commits, headSha);

            // Lane 0 is reserved for the checked-out commit so the current branch always hugs the left edge.
            if (!string.IsNullOrEmpty(headSha))
            {
                lanes.Add(new Lane { Expected = headSha, ColorIndex = nextColor++, Started = false, OnHead = true });
                if (includeWorkInProgress)
                {
                    var wip = new GraphRow { Index = 0, IsWorkInProgress = true, Column = 0, ColorIndex = 0 };
                    layout.Rows.Add(wip);
                    layout.WorkInProgressRow = wip;
                }
            }

            var matching = new List<int>();
            var createdHere = new HashSet<int>();

            foreach (var entry in Merge(commits, stashes))
            {
                bool isStash = entry.Stash != null;
                bool rowOnHead = onHead == null || onHead.Contains(isStash ? entry.Parents[0] : entry.Sha);
                var row = new GraphRow { Index = layout.Rows.Count, Commit = entry.Commit, Stash = entry.Stash, OffHead = !rowOnHead };
                if (isStash) layout.StashCount++;
                matching.Clear();
                createdHere.Clear();

                for (int j = 0; j < lanes.Count; j++)
                {
                    if (lanes[j] != null && lanes[j].Expected == entry.Sha) matching.Add(j);
                }
                // A commit lands in a branch's lane, not in the dashed line of a stash made on it.
                matching.Sort((a, b) => lanes[a].IsStash != lanes[b].IsStash ? (lanes[a].IsStash ? 1 : -1) : a.CompareTo(b));

                int col;
                if (matching.Count > 0)
                {
                    col = matching[0];
                    row.HasStem = lanes[col].Started;
                    row.StemOffHead = !lanes[col].OnHead;
                    row.StemFromStash = lanes[col].IsStash;
                }
                else
                {
                    col = FindFreeSlot(lanes);
                    lanes[col] = new Lane { Expected = entry.Sha, ColorIndex = isStash ? StashColor : nextColor++, Started = true, IsStash = isStash, OnHead = rowOnHead };
                    row.StemOffHead = !rowOnHead;
                }
                var lane = lanes[col];
                if (lane.IsStash && !isStash)
                {
                    // Only stash lines were waiting: this commit starts a lane of its own colour.
                    lane = lanes[col] = new Lane { Expected = entry.Sha, ColorIndex = nextColor++ };
                }
                lane.Started = true;
                lane.OnHead = rowOnHead;
                row.Column = col;
                row.ColorIndex = lane.ColorIndex;

                // Other lanes that were waiting for this commit end here: bend their incoming edge into this column.
                if (layout.Rows.Count > 0 && matching.Count > 1)
                {
                    var prev = layout.Rows[layout.Rows.Count - 1];
                    for (int m = 1; m < matching.Count; m++)
                    {
                        int j = matching[m];
                        foreach (var edge in prev.Edges)
                        {
                            if (edge.ToColumn == j) edge.ToColumn = col;
                        }
                        lanes[j] = null;
                    }
                }

                var parents = entry.Parents;
                if (parents.Count == 0)
                {
                    lanes[col] = null;
                }
                else
                {
                    var first = parents[0];
                    // A stash keeps its own dashed line down to the commit it was made on.
                    int existing = isStash ? -1 : IndexOfLaneExpecting(lanes, first, col);
                    if (existing >= 0)
                    {
                        // This line joins a lane that already leads to our parent.
                        var target = lanes[existing];
                        row.Edges.Add(new GraphEdge { FromColumn = col, ToColumn = existing, ColorIndex = lane.ColorIndex, OffHead = !rowOnHead });
                        target.Started = true;
                        target.OnHead |= rowOnHead;
                        lanes[col] = null;
                    }
                    else
                    {
                        lane.Expected = first;
                    }

                    for (int k = 1; k < parents.Count; k++)
                    {
                        var parent = parents[k];
                        existing = IndexOfLaneExpecting(lanes, parent, -1);
                        if (existing >= 0)
                        {
                            if (existing != col)
                            {
                                row.Edges.Add(new GraphEdge { FromColumn = col, ToColumn = existing, ColorIndex = lanes[existing].ColorIndex, OffHead = !rowOnHead });
                                lanes[existing].Started = true;
                                lanes[existing].OnHead |= rowOnHead;
                            }
                        }
                        else
                        {
                            int nc = FindFreeSlot(lanes);
                            lanes[nc] = new Lane { Expected = parent, ColorIndex = nextColor++, Started = true, OnHead = rowOnHead };
                            createdHere.Add(nc);
                            row.Edges.Add(new GraphEdge { FromColumn = col, ToColumn = nc, ColorIndex = lanes[nc].ColorIndex, OffHead = !rowOnHead });
                        }
                    }
                }

                // Pass-through segments for every lane that continues below this row.
                for (int j = 0; j < lanes.Count; j++)
                {
                    var l = lanes[j];
                    if (l == null || createdHere.Contains(j)) continue;
                    if (!l.Started)
                    {
                        if (!includeWorkInProgress) continue;
                        row.Edges.Add(new GraphEdge { FromColumn = j, ToColumn = j, ColorIndex = l.ColorIndex, Dashed = true });
                        continue;
                    }
                    row.Edges.Add(new GraphEdge { FromColumn = j, ToColumn = j, ColorIndex = l.ColorIndex, Dashed = l.IsStash, OffHead = !l.OnHead });
                }

                layout.Rows.Add(row);
                layout.RowsBySha[entry.Sha] = row;
                if (lanes.Count > layout.LaneCount) layout.LaneCount = lanes.Count;
            }

            if (layout.WorkInProgressRow != null)
            {
                layout.WorkInProgressRow.Edges.Add(new GraphEdge { FromColumn = 0, ToColumn = 0, ColorIndex = 0, Dashed = true });
                if (layout.LaneCount == 0) layout.LaneCount = 1;
            }
            return layout;
        }

        /// <summary>
        /// The loaded commits the checked-out commit contains: itself and every ancestor. Null when
        /// there is no HEAD among them, in which case nothing is greyed out.
        /// </summary>
        /// <remarks>
        /// Walking parents within the loaded commits is enough: with --date-order a loaded commit's
        /// descendants are loaded too, so every path from HEAD to it is.
        /// </remarks>
        private static HashSet<string> ReachableFrom(IList<CommitInfo> commits, string headSha)
        {
            if (string.IsNullOrEmpty(headSha)) return null;
            var bySha = new Dictionary<string, CommitInfo>(StringComparer.Ordinal);
            foreach (var c in commits) bySha[c.Sha] = c;
            if (!bySha.ContainsKey(headSha)) return null;

            var reached = new HashSet<string>(StringComparer.Ordinal);
            var pending = new Stack<string>();
            pending.Push(headSha);
            while (pending.Count > 0)
            {
                var sha = pending.Pop();
                CommitInfo commit;
                if (!reached.Add(sha) || !bySha.TryGetValue(sha, out commit)) continue;
                foreach (var parent in commit.Parents) pending.Push(parent);
            }
            return reached;
        }

        /// <summary>
        /// The commits in order with each stash placed by date, but never below the commit it was
        /// made on. A stash whose commit is not loaded has nowhere to attach and is left out.
        /// </summary>
        private static List<Entry> Merge(IList<CommitInfo> commits, IList<StashInfo> stashes)
        {
            var position = new Dictionary<string, int>(StringComparer.Ordinal);
            for (int i = 0; i < commits.Count; i++) position[commits[i].Sha] = i;

            var before = new Dictionary<int, List<StashInfo>>();
            foreach (var stash in stashes ?? new List<StashInfo>())
            {
                int baseIndex;
                if (stash.BaseSha == null || !position.TryGetValue(stash.BaseSha, out baseIndex)) continue;
                int at = 0;
                while (at < baseIndex && commits[at].CommitDate > stash.Date) at++;
                List<StashInfo> list;
                if (!before.TryGetValue(at, out list)) before[at] = list = new List<StashInfo>();
                list.Add(stash);
            }

            var entries = new List<Entry>(commits.Count + before.Values.Sum(l => l.Count));
            for (int i = 0; i < commits.Count; i++)
            {
                List<StashInfo> list;
                if (before.TryGetValue(i, out list))
                {
                    foreach (var stash in list.OrderByDescending(s => s.Date))
                        entries.Add(new Entry { Sha = stash.Sha, Parents = new[] { stash.BaseSha }, Stash = stash });
                }
                var c = commits[i];
                entries.Add(new Entry { Sha = c.Sha, Parents = c.Parents, Commit = c });
            }
            return entries;
        }

        private static int FindFreeSlot(List<Lane> lanes)
        {
            for (int i = 0; i < lanes.Count; i++)
            {
                if (lanes[i] == null) return i;
            }
            lanes.Add(null);
            return lanes.Count - 1;
        }

        /// <summary>A lane leading to <paramref name="sha"/> to join; a stash's dashed line is never joined.</summary>
        private static int IndexOfLaneExpecting(List<Lane> lanes, string sha, int exclude)
        {
            for (int i = 0; i < lanes.Count; i++)
            {
                if (i == exclude) continue;
                var l = lanes[i];
                if (l != null && !l.IsStash && l.Expected == sha) return i;
            }
            return -1;
        }
    }
}
