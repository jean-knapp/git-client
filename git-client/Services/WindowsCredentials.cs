using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace GitClient.Services
{
    /// <summary>One sign-in Git Credential Manager has stored for a host.</summary>
    public sealed class StoredAccount
    {
        public StoredAccount(string target, string user)
        {
            Target = target;
            User = user;
        }

        /// <summary>Credential Manager target, e.g. <c>git:https://jean-knapp@github.com</c>.</summary>
        public string Target { get; }

        /// <summary>The account name, which is what the picker shows.</summary>
        public string User { get; }

        /// <summary>True for the host-wide entry that names no account in its target.</summary>
        public bool IsHostWide => Target != null && Target.IndexOf('@') < 0;

        public override string ToString() => User + "  (" + Target + ")";
    }

    /// <summary>
    /// Reads which sign-ins Windows Credential Manager holds for a host. Only the target and the
    /// account name are read - never the secret - which is enough to know whether Git Credential
    /// Manager will stop and ask which account to use.
    /// </summary>
    public static class WindowsCredentials
    {
        private const int CredTypeGeneric = 1;

        [DllImport("advapi32.dll", EntryPoint = "CredEnumerateW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredEnumerate(string filter, int flags, out int count, out IntPtr credentials);

        [DllImport("advapi32.dll", EntryPoint = "CredFree")]
        private static extern void CredFree(IntPtr buffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct Credential
        {
            public int Flags;
            public int Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public int CredentialBlobSize;
            public IntPtr CredentialBlob;
            public int Persist;
            public int AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;
        }

        /// <summary>
        /// Every git sign-in stored for <paramref name="host"/>. Two or more is what makes Git
        /// Credential Manager show its "Select an account" window on each push.
        /// </summary>
        public static List<StoredAccount> AccountsFor(string host)
        {
            var accounts = new List<StoredAccount>();
            if (string.IsNullOrEmpty(host)) return accounts;

            int count;
            IntPtr array;
            // git: is the prefix Git Credential Manager writes; anything else in the vault is none
            // of our business.
            if (!CredEnumerate("git:https://*", 0, out count, out array)) return accounts;

            try
            {
                for (int i = 0; i < count; i++)
                {
                    var entry = Marshal.ReadIntPtr(array, i * IntPtr.Size);
                    var credential = (Credential)Marshal.PtrToStructure(entry, typeof(Credential));
                    if (credential.Type != CredTypeGeneric) continue;

                    var target = credential.TargetName == IntPtr.Zero ? null : Marshal.PtrToStringUni(credential.TargetName);
                    if (target == null || target.IndexOf(host, StringComparison.OrdinalIgnoreCase) < 0) continue;

                    var user = credential.UserName == IntPtr.Zero ? null : Marshal.PtrToStringUni(credential.UserName);
                    accounts.Add(new StoredAccount(target, user));
                }
            }
            catch (Exception)
            {
                // A vault we cannot read simply tells us nothing.
            }
            finally
            {
                CredFree(array);
            }
            return accounts;
        }

        /// <summary>The distinct account names stored for a host, in the order they were found.</summary>
        public static List<string> LoginsFor(string host)
        {
            return AccountsFor(host)
                .Select(a => a.User)
                .Where(u => !string.IsNullOrWhiteSpace(u))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
