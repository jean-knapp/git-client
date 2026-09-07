namespace GitClient.Controls
{
    /// <summary>
    /// SVG icon markup used by buttons, menus and tree nodes. All icons use <c>currentColor</c> so the
    /// ModernWinForms controls tint them to match the skin.
    /// </summary>
    public static class Icons
    {
        private const string Open = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\">";
        private const string Open16 = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 16 16\">";
        private const string Close = "</svg>";

        public const string Pull = Open + "<path fill=\"currentColor\" d=\"M11 3h2v9.2l3.5-3.5 1.4 1.4L12 16 6.1 10.1l1.4-1.4L11 12.2z\"/><path fill=\"currentColor\" d=\"M4 18h16v2H4z\"/>" + Close;
        public const string Push = Open + "<path fill=\"currentColor\" d=\"M12 3l5.9 5.9-1.4 1.4L13 6.8V16h-2V6.8L7.5 10.3 6.1 8.9z\"/><path fill=\"currentColor\" d=\"M4 18h16v2H4z\"/>" + Close;
        public const string Fetch = Open + "<path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M19 12c0 3.9-3.1 7-7 7s-7-3.1-7-7 3.1-7 7-7c2.4 0 4.5 1.2 5.8 3\"/><path fill=\"currentColor\" d=\"M20 3v6h-6z\"/>" + Close;
        public const string Branch = Open + "<circle fill=\"currentColor\" cx=\"6\" cy=\"5\" r=\"2.6\"/><circle fill=\"currentColor\" cx=\"6\" cy=\"19\" r=\"2.6\"/><circle fill=\"currentColor\" cx=\"18\" cy=\"8\" r=\"2.6\"/><path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M6 7.5v9M18 10.5c0 5-12 2-12 6\"/>" + Close;
        public const string Stash = Open + "<path fill=\"currentColor\" d=\"M3 5h18v5h-2V7H5v3H3z\"/><path fill=\"currentColor\" d=\"M3 12h7v2h4v-2h7v8H3z\"/>" + Close;
        public const string Pop = Open + "<path fill=\"currentColor\" d=\"M3 12h7v2h4v-2h7v8H3z\"/><path fill=\"currentColor\" d=\"M12 2l4.5 4.5H13V11h-2V6.5H7.5z\"/>" + Close;
        public const string Terminal = Open + "<path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M4 5h16v14H4z\"/><path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M7 9l3 3-3 3M12 15h5\"/>" + Close;
        public const string Output = Open + "<rect fill=\"currentColor\" x=\"3\" y=\"4\" width=\"18\" height=\"3\"/><path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M4 8v11h16V8\"/><path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M7 12h6M7 15h10\"/>" + Close;
        public const string Plus = Open + "<path fill=\"currentColor\" d=\"M11 5h2v6h6v2h-6v6h-2v-6H5v-2h6z\"/>" + Close;
        public const string Cross = Open + "<path fill=\"currentColor\" d=\"M6.4 5L19 17.6 17.6 19 5 6.4z\"/><path fill=\"currentColor\" d=\"M17.6 5L5 17.6 6.4 19 19 6.4z\"/>" + Close;
        public const string Sparkle = Open + "<path fill=\"currentColor\" d=\"M12 2l2.2 6.3 6.3 2.2-6.3 2.2L12 19l-2.2-6.3-6.3-2.2 6.3-2.2z\"/><path fill=\"currentColor\" d=\"M19 15l.9 2.6 2.6.9-2.6.9L19 22l-.9-2.6-2.6-.9 2.6-.9z\"/>" + Close;
        public const string Check = Open + "<path fill=\"currentColor\" d=\"M9 16.2l-3.5-3.5-1.4 1.4L9 19 20 8l-1.4-1.4z\"/>" + Close;
        public const string Folder = Open + "<path fill=\"currentColor\" d=\"M3 5h6l2 2h10v12H3z\"/>" + Close;
        public const string Clone = Open + "<path fill=\"currentColor\" d=\"M11 3h2v9.2l3.5-3.5 1.4 1.4L12 16 6.1 10.1l1.4-1.4L11 12.2z\"/><path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M4 15v5h16v-5\"/>" + Close;
        public const string Refresh = Fetch;
        public const string Undo = Open + "<path fill=\"currentColor\" d=\"M9 5l-6 6 6 6v-4h5a4 4 0 0 1 0 8h-3v2h3a6 6 0 0 0 0-12H9z\"/>" + Close;
        public const string Settings = Open + "<circle fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" cx=\"12\" cy=\"12\" r=\"3\"/><path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M12 2v3M12 19v3M2 12h3M19 12h3M4.9 4.9l2.1 2.1M17 17l2.1 2.1M4.9 19.1L7 17M17 7l2.1-2.1\"/>" + Close;
        public const string Merge = Open + "<circle fill=\"currentColor\" cx=\"6\" cy=\"5\" r=\"2.6\"/><circle fill=\"currentColor\" cx=\"6\" cy=\"19\" r=\"2.6\"/><circle fill=\"currentColor\" cx=\"18\" cy=\"12\" r=\"2.6\"/><path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M6 7.5v9M6 8c0 4 9 2 9 4\"/>" + Close;
        public const string CherryPick = Open + "<circle fill=\"currentColor\" cx=\"9\" cy=\"16\" r=\"4\"/><circle fill=\"currentColor\" cx=\"17\" cy=\"17\" r=\"3\"/><path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M9 12c0-4 4-7 8-8M17 14c0-3 1-6 0-10\"/>" + Close;
        public const string Trash = Open + "<path fill=\"currentColor\" d=\"M9 3h6l1 2h4v2H4V5h4z\"/><path fill=\"currentColor\" d=\"M6 8h12l-1 13H7z\"/>" + Close;
        public const string Tag = Open + "<path fill=\"currentColor\" d=\"M3 3h9l9 9-9 9-9-9z\"/><circle fill=\"#000\" fill-opacity=\"0.35\" cx=\"8\" cy=\"8\" r=\"2\"/>" + Close;
        public const string Copy = Open + "<path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M8 8h12v12H8z\"/><path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M16 8V4H4v12h4\"/>" + Close;
        public const string Reset = Open + "<path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M5 12a7 7 0 1 0 2-5\"/><path fill=\"currentColor\" d=\"M4 3v6h6z\"/>" + Close;
        public const string Checkout = Open + "<path fill=\"currentColor\" d=\"M3 11h11.2l-3.6-3.6L12 6l6 6-6 6-1.4-1.4 3.6-3.6H3z\"/><path fill=\"currentColor\" d=\"M19 4h2v16h-2z\"/>" + Close;
        public const string Revert = Open + "<path fill=\"currentColor\" d=\"M14 5l-6 6 6 6v-4h2a4 4 0 0 1 0 8h-3v2h3a6 6 0 0 0 0-12h-2z\"/>" + Close;
        public const string Github = Open + "<path fill=\"currentColor\" d=\"M12 2a10 10 0 0 0-3.2 19.5c.5.1.7-.2.7-.5v-1.7c-2.8.6-3.4-1.2-3.4-1.2-.4-1.2-1.1-1.5-1.1-1.5-.9-.6.1-.6.1-.6 1 .1 1.5 1 1.5 1 .9 1.6 2.4 1.1 3 .9.1-.7.4-1.1.6-1.4-2.2-.2-4.6-1.1-4.6-4.9 0-1.1.4-2 1-2.7-.1-.3-.4-1.3.1-2.7 0 0 .8-.3 2.8 1a9.5 9.5 0 0 1 5 0c1.9-1.3 2.8-1 2.8-1 .5 1.4.2 2.4.1 2.7.6.7 1 1.6 1 2.7 0 3.8-2.3 4.7-4.6 4.9.4.3.7.9.7 1.9v2.8c0 .3.2.6.7.5A10 10 0 0 0 12 2z\"/>" + Close;
        public const string Explorer = Folder;
        public const string Edit = Open + "<path fill=\"currentColor\" d=\"M3 17.3V21h3.7L17.8 9.9l-3.7-3.7zM20.7 7a1 1 0 0 0 0-1.4l-2.3-2.3a1 1 0 0 0-1.4 0l-1.8 1.8 3.7 3.7z\"/>" + Close;
        public const string Back = Open + "<path fill=\"currentColor\" d=\"M20 11H7.8l5.6-5.6L12 4l-8 8 8 8 1.4-1.4L7.8 13H20z\"/>" + Close;
        public const string Discard = Open + "<path fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" d=\"M6 6l12 12M18 6L6 18\"/><circle fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" cx=\"12\" cy=\"12\" r=\"9\"/>" + Close;
        public const string Search = Open + "<circle fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" cx=\"10.5\" cy=\"10.5\" r=\"6.5\"/><path fill=\"currentColor\" d=\"M15.6 17l1.4-1.4 4 4-1.4 1.4z\"/>" + Close;
        public const string Overflow = Open + "<circle fill=\"currentColor\" cx=\"12\" cy=\"5\" r=\"1.9\"/><circle fill=\"currentColor\" cx=\"12\" cy=\"12\" r=\"1.9\"/><circle fill=\"currentColor\" cx=\"12\" cy=\"19\" r=\"1.9\"/>" + Close;
        public const string ChevronDown = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 12 12\"><path fill=\"currentColor\" d=\"M1.5 4L6 8.5 10.5 4l-.9-.9L6 6.7 2.4 3.1z\"/></svg>";
        public const string Warning = Open + "<path fill=\"currentColor\" d=\"M12 2.4l10.4 18H1.6z\"/><path fill=\"#000\" fill-opacity=\"0.55\" d=\"M11 9h2v6h-2zM11 16.2h2v2.2h-2z\"/>" + Close;
        public const string Init = Open + "<circle fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" cx=\"12\" cy=\"12\" r=\"9\"/><path fill=\"currentColor\" d=\"M11 7h2v4h4v2h-4v4h-2v-4H7v-2h4z\"/>" + Close;

        // 16px file-status glyphs for the change lists (tinted by the row colour).
        public const string FileModified = Open16 + "<path fill=\"currentColor\" d=\"M2.5 11V13.5H5l7-7-2.5-2.5zM13.4 5.1l-2.5-2.5 1.2-1.2 2.5 2.5z\"/>" + Close;
        public const string FileAdded = Open16 + "<circle fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.6\" cx=\"8\" cy=\"8\" r=\"6.5\"/><path fill=\"currentColor\" d=\"M7 4.5h2V7h2.5v2H9v2.5H7V9H4.5V7H7z\"/>" + Close;
        public const string FileDeleted = Open16 + "<circle fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.6\" cx=\"8\" cy=\"8\" r=\"6.5\"/><path fill=\"currentColor\" d=\"M4.5 7h7v2h-7z\"/>" + Close;
        public const string FileRenamed = Open16 + "<path fill=\"currentColor\" d=\"M2 7h8.2L7.6 4.4 9 3l5 5-5 5-1.4-1.4L10.2 9H2z\"/>" + Close;
        public const string FileUntracked = Open16 + "<path fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.6\" d=\"M2.5 2.5h11v11h-11z\"/><path fill=\"currentColor\" d=\"M7 4.5h2V7h2.5v2H9v2.5H7V9H4.5V7H7z\"/>" + Close;
        public const string FileConflicted = Open16 + "<path fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.6\" d=\"M8 2.5L14.5 14h-13z\"/><path fill=\"currentColor\" d=\"M7 6.5h2V10H7zM7 11h2v2H7z\"/>" + Close;
        public const string FileGeneric = Open16 + "<path fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.6\" d=\"M3.5 1.5h6l3 3v10h-9z\"/><path fill=\"currentColor\" d=\"M9 1.5v3.5h3.5z\"/>" + Close;
        public const string Laptop = Open16 + "<path fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.6\" d=\"M3 3.5h10v7H3z\"/><path fill=\"currentColor\" d=\"M1 12h14v1.5H1z\"/>" + Close;
        public const string Cloud = Open16 + "<path fill=\"currentColor\" d=\"M4.5 13a3.5 3.5 0 0 1-.4-7 4.5 4.5 0 0 1 8.6 1.2A3 3 0 0 1 12.5 13z\"/>" + Close;
        public const string TagSmall = Open16 + "<path fill=\"currentColor\" d=\"M1.5 1.5h6.5l6.5 6.5-6.5 6.5-6.5-6.5z\"/>" + Close;
        public const string Commit = Open16 + "<circle fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.8\" cx=\"8\" cy=\"8\" r=\"3.2\"/><path fill=\"currentColor\" d=\"M0 7h4v2H0zM12 7h4v2h-4z\"/>" + Close;

        public static string ForChange(Git.FileChangeKind kind)
        {
            switch (kind)
            {
                case Git.FileChangeKind.Added: return FileAdded;
                case Git.FileChangeKind.Deleted: return FileDeleted;
                case Git.FileChangeKind.Renamed:
                case Git.FileChangeKind.Copied: return FileRenamed;
                case Git.FileChangeKind.Untracked: return FileUntracked;
                case Git.FileChangeKind.Conflicted: return FileConflicted;
                case Git.FileChangeKind.Modified:
                case Git.FileChangeKind.TypeChanged: return FileModified;
                default: return FileGeneric;
            }
        }
    }
}
