namespace Gitka.Api;

public class GitFileSaver(Uri repositoryUrl, GitClient Git)
{
    public async Task PushFile(string branch, string filepath, string content, CancellationToken ct = default)
    {
        // We create a new clone for each push to avoid race conditions in the local repository.
        var cloneDir = Path.Combine(Path.GetTempPath(), "gitka-put", Guid.NewGuid().ToString("N")[..8]);

        Task<string> Run(params string[] args) => Git.Run(cloneDir, args, ct);

        Directory.CreateDirectory(cloneDir);
        try
        {
            await Run("clone", repositoryUrl.ToString(), ".");
            var remoteBranchesRaw = await Run("branch", "-r");
            var remoteBranches = remoteBranchesRaw.Split('\n').Select(b => b.Trim());

            var branchExists = remoteBranches.Any(b => b == $"origin/{branch}");

            if (branchExists)
                await Run("checkout", "-B", branch, $"origin/{branch}");
            else
                await Run("checkout", "-b", branch);

            var fullPath = Path.Combine(cloneDir, filepath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            await File.WriteAllTextAsync(fullPath, content, ct);

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
