using System;
using System.Collections.Generic;
using GitClient.Git;

namespace GitClient.Graph
{
    /// <summary>A line segment drawn between one row and the next.</summary>
    public sealed class GraphEdge
    {
        public int FromColumn { get; set; }
        public int ToColumn { get; set; }
        public int ColorIndex { get; set; }
        public bool Dashed { get; set; }
    }

    /// <summary>One row of the commit graph: a commit (or the work-in-progress marker) and its outgoing edges.</summary>
    public sealed class GraphRow
    {
        public int Index { get; set; }
        public CommitInfo Commit { get; set; }
        public bool IsWorkInProgress { get; set; }
        public int Column { get; set; }
        public int ColorIndex { get; set; }

        /// <summary>Edges leaving this row towards the next row.</summary>
        public List<GraphEdge> Edges { get; } = new List<GraphEdge>();

        public string Sha => Commit?.Sha;
    }

    /// <summary>
    /// Assigns lanes (columns) to commits so the history can be drawn as a railway-style graph.
    /// Commits must be supplied newest-first with every child before its parents (git log --date-order).
    /// </summary>
    public sealed class GraphLayout
    {
        private sealed class Lane
        {
            public string Expected;
            public int ColorIndex;
            public bool Started;
        }

        public List<GraphRow> Rows { get; } = new List<GraphRow>();
        public Dictionary<string, GraphRow> RowsBySha { get; } = new Dictionary<string, GraphRow>(StringComparer.Ordinal);
        public int LaneCount { get; private set; }
        public GraphRow WorkInProgressRow { get; private set; }

        public static GraphLayout Build(IList<CommitInfo> commits, string headSha, bool includeWorkInProgress)
        {
            var layout = new GraphLayout();
            var lanes = new List<Lane>();
            int nextColor = 0;

            // Lane 0 is reserved for the checked-out commit so the current branch always hugs the left edge.
            if (!string.IsNullOrEmpty(headSha))
            {
                lanes.Add(new Lane { Expected = headSha, ColorIndex = nextColor++, Started = false });
                if (includeWorkInProgress)
                {
                    var wip = new GraphRow { Index = 0, IsWorkInProgress = true, Column = 0, ColorIndex = 0 };
                    layout.Rows.Add(wip);
                    layout.WorkInProgressRow = wip;
                }
            }

            var matching = new List<int>();
            var createdHere = new HashSet<int>();

            foreach (var commit in commits)
            {
                var row = new GraphRow { Index = layout.Rows.Count, Commit = commit };
                matching.Clear();
                createdHere.Clear();

                for (int j = 0; j < lanes.Count; j++)
                {
                    if (lanes[j] != null && lanes[j].Expected == commit.Sha) matching.Add(j);
                }

                int col;
                if (matching.Count > 0)
                {
                    col = matching[0];
                }
                else
                {
                    col = FindFreeSlot(lanes);
                    lanes[col] = new Lane { Expected = commit.Sha, ColorIndex = nextColor++, Started = true };
                }
                var lane = lanes[col];
                lane.Started = true;
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

                var parents = commit.Parents;
                if (parents.Count == 0)
                {
                    lanes[col] = null;
                }
                else
                {
                    var first = parents[0];
                    int existing = IndexOfLaneExpecting(lanes, first, col);
                    if (existing >= 0)
                    {
                        // This line joins a lane that already leads to our parent.
                        var target = lanes[existing];
                        row.Edges.Add(new GraphEdge { FromColumn = col, ToColumn = existing, ColorIndex = lane.ColorIndex, Dashed = false });
                        target.Started = true;
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
                                row.Edges.Add(new GraphEdge { FromColumn = col, ToColumn = existing, ColorIndex = lanes[existing].ColorIndex });
                                lanes[existing].Started = true;
                            }
                        }
                        else
                        {
                            int nc = FindFreeSlot(lanes);
                            lanes[nc] = new Lane { Expected = parent, ColorIndex = nextColor++, Started = true };
                            createdHere.Add(nc);
                            row.Edges.Add(new GraphEdge { FromColumn = col, ToColumn = nc, ColorIndex = lanes[nc].ColorIndex });
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
                    row.Edges.Add(new GraphEdge { FromColumn = j, ToColumn = j, ColorIndex = l.ColorIndex });
                }

                layout.Rows.Add(row);
                layout.RowsBySha[commit.Sha] = row;
                if (lanes.Count > layout.LaneCount) layout.LaneCount = lanes.Count;
            }

            if (layout.WorkInProgressRow != null)
            {
                layout.WorkInProgressRow.Edges.Add(new GraphEdge { FromColumn = 0, ToColumn = 0, ColorIndex = 0, Dashed = true });
                if (layout.LaneCount == 0) layout.LaneCount = 1;
            }
            return layout;
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

        private static int IndexOfLaneExpecting(List<Lane> lanes, string sha, int exclude)
        {
            for (int i = 0; i < lanes.Count; i++)
            {
                if (i == exclude) continue;
                var l = lanes[i];
                if (l != null && l.Expected == sha) return i;
            }
            return -1;
        }
    }
}
