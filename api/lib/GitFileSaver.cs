namespace Gitka.Api;

public static class GitFileSaver
{
    public static async Task PushFile(this GitClient client, Uri repositoryUrl, string branch, string filepath, string content, CancellationToken ct = default)
    {
        var cloneDir = Path.Combine(Path.GetTempPath(), "gitka-put", Guid.NewGuid().ToString("N")[..8]);

        Task<string> Run(params string[] args) => client.Run(cloneDir, args, ct);

        var lsRemote = await client.Run(Path.GetTempPath(), ["ls-remote", "--heads", repositoryUrl.ToString(), branch], ct);
        var branchExists = lsRemote.Trim().Length > 0;

        Directory.CreateDirectory(cloneDir);
        try
        {
            if (branchExists)
                await Run("clone", "--depth", "1", "--single-branch", "--branch", branch, repositoryUrl.ToString(), ".");
            else
            {
                await Run("clone", "--depth", "1", repositoryUrl.ToString(), ".");
                await Run("checkout", "-b", branch);
            }

            var fullPath = Path.Combine(cloneDir, filepath.Replace('/', Path.DirectorySeparatorChar));
            var resolvedPath = Path.GetFullPath(fullPath);
            var resolvedCloneDir = Path.GetFullPath(cloneDir) + Path.DirectorySeparatorChar;
            if (!resolvedPath.StartsWith(resolvedCloneDir, StringComparison.Ordinal))
                throw new ArgumentException("filepath attempts to escape the repository directory.", nameof(filepath));
            Directory.CreateDirectory(Path.GetDirectoryName(resolvedPath)!);
            await File.WriteAllTextAsync(resolvedPath, content, ct);

            await Run("add", filepath);
            await Run("commit", "-m", $"gitka: update {filepath}");
            await Run("push", "origin", branch);
        }
        finally
        {
            Directory.Delete(cloneDir, recursive: true);
        }
    }
}
