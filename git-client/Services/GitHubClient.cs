using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace GitClient.Services
{
    /// <summary>An account or organisation a repository can be created under.</summary>
    public sealed class GitHubOwner
    {
        public GitHubOwner(string login, bool isOrganization)
        {
            Login = login;
            IsOrganization = isOrganization;
        }

        public string Login { get; }
        public bool IsOrganization { get; }

        public override string ToString() => Login;
    }

    /// <summary>A repository as GitHub reports it back after creation.</summary>
    public sealed class GitHubRepository
    {
        public string FullName { get; set; }
        public string CloneUrl { get; set; }
        public string SshUrl { get; set; }
        public string HtmlUrl { get; set; }
        public string DefaultBranch { get; set; }
    }

    /// <summary>An error GitHub reported, with its status code so callers can react to 401/403.</summary>
    public sealed class GitHubException : Exception
    {
        public GitHubException(HttpStatusCode status, string message) : base(message)
        {
            Status = status;
        }

        public HttpStatusCode Status { get; }

        /// <summary>The token is missing, expired, or lacks the scope the call needs.</summary>
        public bool IsAuthenticationProblem =>
            Status == HttpStatusCode.Unauthorized || Status == HttpStatusCode.Forbidden;
    }

    /// <summary>
    /// The slice of the GitHub REST API the client needs: who the token belongs to, which
    /// organisations it can create in, and creating a repository.
    /// </summary>
    public static class GitHubClient
    {
        private const string ApiRoot = "https://api.github.com";

        public static async Task<GitHubOwner> GetUserAsync(string token)
        {
            var json = await GetAsync(token, "/user").ConfigureAwait(false);
            var login = Text(json, "login");
            if (string.IsNullOrEmpty(login)) throw new GitHubException(HttpStatusCode.OK, "GitHub did not return an account for this token.");
            return new GitHubOwner(login, false);
        }

        /// <summary>The signed-in account first, then every organisation it belongs to.</summary>
        public static async Task<List<GitHubOwner>> GetOwnersAsync(string token)
        {
            var owners = new List<GitHubOwner> { await GetUserAsync(token).ConfigureAwait(false) };
            try
            {
                var organizations = await GetAsync(token, "/user/orgs").ConfigureAwait(false) as object[];
                if (organizations != null)
                {
                    foreach (var entry in organizations)
                    {
                        var login = Text(entry, "login");
                        if (!string.IsNullOrEmpty(login)) owners.Add(new GitHubOwner(login, true));
                    }
                }
            }
            catch (GitHubException)
            {
                // A token without the read:org scope still creates personal repositories.
            }
            return owners;
        }

        public static async Task<GitHubRepository> CreateRepositoryAsync(
            string token, GitHubOwner owner, string name, string description, bool isPrivate)
        {
            var body = new StringBuilder();
            body.Append('{');
            body.Append("\"name\":").Append(Quote(name));
            if (!string.IsNullOrWhiteSpace(description)) body.Append(",\"description\":").Append(Quote(description.Trim()));
            body.Append(",\"private\":").Append(isPrivate ? "true" : "false");
            // The local repository supplies the first commit, so GitHub must not write one.
            body.Append(",\"auto_init\":false");
            body.Append('}');

            var path = owner != null && owner.IsOrganization ? "/orgs/" + owner.Login + "/repos" : "/user/repos";
            var json = await PostAsync(token, path, body.ToString()).ConfigureAwait(false);
            return new GitHubRepository
            {
                FullName = Text(json, "full_name"),
                CloneUrl = Text(json, "clone_url"),
                SshUrl = Text(json, "ssh_url"),
                HtmlUrl = Text(json, "html_url"),
                DefaultBranch = Text(json, "default_branch"),
            };
        }

        // ------------------------------------------------------------------ transport

        private static HttpClient CreateClient(string token)
        {
            // .NET Framework leaves TLS to the process default, which can still exclude 1.2.
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("GitClient");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private static async Task<object> GetAsync(string token, string path)
        {
            using (var client = CreateClient(token))
            using (var response = await client.GetAsync(ApiRoot + path).ConfigureAwait(false))
            {
                return await ReadAsync(response).ConfigureAwait(false);
            }
        }

        private static async Task<object> PostAsync(string token, string path, string body)
        {
            using (var client = CreateClient(token))
            using (var content = new StringContent(body, Encoding.UTF8, "application/json"))
            using (var response = await client.PostAsync(ApiRoot + path, content).ConfigureAwait(false))
            {
                return await ReadAsync(response).ConfigureAwait(false);
            }
        }

        private static async Task<object> ReadAsync(HttpResponseMessage response)
        {
            var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            object json = null;
            try { json = new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.DeserializeObject(text); }
            catch (ArgumentException) { }

            if (response.IsSuccessStatusCode) return json;
            throw new GitHubException(response.StatusCode, DescribeError(response, json, text));
        }

        private static string DescribeError(HttpResponseMessage response, object json, string text)
        {
            var message = Text(json, "message");
            var detail = new StringBuilder(string.IsNullOrEmpty(message) ? (int)response.StatusCode + " " + response.ReasonPhrase : message);

            var map = json as Dictionary<string, object>;
            object errors;
            if (map != null && map.TryGetValue("errors", out errors))
            {
                var list = errors as object[];
                if (list != null)
                {
                    foreach (var entry in list)
                    {
                        var line = Text(entry, "message");
                        if (string.IsNullOrEmpty(line))
                        {
                            var field = Text(entry, "field");
                            var code = Text(entry, "code");
                            if (!string.IsNullOrEmpty(field) || !string.IsNullOrEmpty(code)) line = (field + " " + code).Trim();
                        }
                        if (!string.IsNullOrEmpty(line)) detail.Append(Environment.NewLine).Append(line);
                    }
                }
            }
            if (detail.Length == 0) detail.Append(text);
            return detail.ToString();
        }

        private static string Text(object json, string key)
        {
            var map = json as Dictionary<string, object>;
            object value;
            if (map == null || !map.TryGetValue(key, out value) || value == null) return null;
            return Convert.ToString(value);
        }

        private static string Quote(string value)
        {
            var builder = new StringBuilder("\"");
            foreach (var c in value ?? string.Empty)
            {
                switch (c)
                {
                    case '"': builder.Append("\\\""); break;
                    case '\\': builder.Append("\\\\"); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\r': builder.Append("\\r"); break;
                    case '\t': builder.Append("\\t"); break;
                    default:
                        if (c < ' ') builder.Append("\\u").Append(((int)c).ToString("x4"));
                        else builder.Append(c);
                        break;
                }
            }
            return builder.Append('"').ToString();
        }
    }
}
