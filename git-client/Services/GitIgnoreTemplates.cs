using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitClient.Services
{
    /// <summary>A named block of .gitignore rules that can be inserted into a repository's file.</summary>
    public sealed class GitIgnoreTemplate
    {
        public GitIgnoreTemplate(string name, string description, string content, string filePath = null)
        {
            Name = name;
            Description = description;
            Content = content;
            FilePath = filePath;
        }

        public string Name { get; }
        public string Description { get; }
        public string Content { get; }

        /// <summary>Where a user template lives; null for the built-in ones.</summary>
        public string FilePath { get; }

        public bool IsCustom => FilePath != null;

        public override string ToString() => Name;
    }

    /// <summary>
    /// The built-in .gitignore templates plus the user's own, kept as plain files under
    /// %APPDATA%\GitClient\gitignore-templates so they can also be edited outside the app.
    /// </summary>
    public static class GitIgnoreTemplates
    {
        public static string CustomDirectory =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GitClient", "gitignore-templates");

        /// <summary>Built-in templates first, then the user's, both in name order.</summary>
        public static List<GitIgnoreTemplate> All()
        {
            var list = new List<GitIgnoreTemplate>(BuiltIn);
            list.AddRange(Custom());
            return list;
        }

        public static List<GitIgnoreTemplate> Custom()
        {
            var list = new List<GitIgnoreTemplate>();
            try
            {
                if (!Directory.Exists(CustomDirectory)) return list;
                foreach (var file in Directory.GetFiles(CustomDirectory, "*.gitignore").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
                {
                    list.Add(new GitIgnoreTemplate(Path.GetFileNameWithoutExtension(file), "Your template", File.ReadAllText(file), file));
                }
            }
            catch (IOException)
            {
                // A template folder we cannot read simply contributes nothing.
            }
            catch (UnauthorizedAccessException)
            {
            }
            return list;
        }

        /// <summary>Writes a user template, overwriting one of the same name.</summary>
        public static GitIgnoreTemplate Save(string name, string content)
        {
            var safe = SanitizeName(name);
            if (safe.Length == 0) throw new ArgumentException("A template needs a name.", nameof(name));
            Directory.CreateDirectory(CustomDirectory);
            var path = Path.Combine(CustomDirectory, safe + ".gitignore");
            File.WriteAllText(path, content ?? string.Empty);
            return new GitIgnoreTemplate(safe, "Your template", content ?? string.Empty, path);
        }

        public static void Delete(GitIgnoreTemplate template)
        {
            if (template == null || !template.IsCustom) return;
            if (File.Exists(template.FilePath)) File.Delete(template.FilePath);
        }

        private static string SanitizeName(string name)
        {
            var trimmed = (name ?? string.Empty).Trim();
            foreach (var invalid in Path.GetInvalidFileNameChars()) trimmed = trimmed.Replace(invalid, '-');
            return trimmed;
        }

        // ------------------------------------------------------------------ built-ins

        private static readonly GitIgnoreTemplate[] BuiltIn =
        {
            new GitIgnoreTemplate("Visual Studio", "C#, C++ and .NET projects", VisualStudio),
            new GitIgnoreTemplate("Visual Studio Code", "Editor folder, keeping shared settings", VisualStudioCode),
            new GitIgnoreTemplate("Windows", "Explorer and shell leftovers", Windows),
            new GitIgnoreTemplate("Node", "node_modules, logs and build output", Node),
            new GitIgnoreTemplate("Python", "Byte code, virtual environments and caches", Python),
        };

        private const string VisualStudio = @"# ---- Visual Studio ----
# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates
*.userprefs

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio cache and options
.vs/
*.VisualState.xml
TestResult.xml
[Dd]ebugPS/
[Rr]eleasePS/

# Build and test output
[Tt]est[Rr]esult*/
[Bb]uild[Ll]og.*
*_i.c
*_p.c
*_h.h
*.ilk
*.meta
*.obj
*.iobj
*.pch
*.pdb
*.ipdb
*.pgc
*.pgd
*.rsp
*.sbr
*.tlb
*.tli
*.tlh
*.tmp
*.tmp_proj
*_wpftmp.csproj
*.log
*.tlog
*.vspscc
*.vssscc
.builds
*.pidb
*.svclog
*.scc

# NuGet
*.nupkg
*.snupkg
**/[Pp]ackages/*
!**/[Pp]ackages/build/
*.nuget.props
*.nuget.targets

# ClickOnce and publish output
publish/
[Pp]ublish[Pp]rofiles/*.pubxml.user
*.publishsettings
*.azurePubxml

# ReSharper, Rider and other tools
_ReSharper*/
*.[Rr]e[Ss]harper
*.DotSettings.user
.idea/
*.sln.iml

# Others
*.cache
*.bak
*.dbmdl
*.dbproj.schemaview
*.jfm
node_modules/
";

        private const string VisualStudioCode = @"# ---- Visual Studio Code ----
.vscode/*
!.vscode/settings.json
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json
!.vscode/*.code-snippets
.history/
*.vsix
";

        private const string Windows = @"# ---- Windows ----
Thumbs.db
Thumbs.db:encryptable
ehthumbs.db
ehthumbs_vista.db
*.stackdump
[Dd]esktop.ini
$RECYCLE.BIN/
*.lnk
";

        private const string Node = @"# ---- Node ----
node_modules/
npm-debug.log*
yarn-debug.log*
yarn-error.log*
pnpm-debug.log*
.npm
.yarn/cache
.pnp.*
dist/
build/
coverage/
.env
.env.local
.cache/
";

        private const string Python = @"# ---- Python ----
__pycache__/
*.py[cod]
*$py.class
*.egg-info/
.eggs/
build/
dist/
.venv/
venv/
env/
.tox/
.pytest_cache/
.mypy_cache/
.coverage
htmlcov/
.ipynb_checkpoints/
";
    }
}
