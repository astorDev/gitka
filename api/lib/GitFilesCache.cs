namespace Gitka.Api;

public class GitFilesCache(GitClient gitClient, Uri repositoryUrl)
{
    readonly string _cacheDir = Path.Combine(Path.GetTempPath(), "gitka", Guid.NewGuid().ToString("N")[..8]);

    private async Task<string> Git(string[] args, CancellationToken ct = default)
    {
        return await gitClient.Run(_cacheDir, args, ct);
    }

    async Task EnsureCloned(CancellationToken ct = default)
    {
        if (Directory.Exists(Path.Combine(_cacheDir, ".git")))
        {
            await Git(["fetch", "--all"], ct);
            return;
        }

        Directory.CreateDirectory(_cacheDir);
        await Git(["clone", repositoryUrl.ToString(), "."], ct);
    }

    public async Task<string> GetFile(string branch, string filepath, CancellationToken ct = default)
    {
        await EnsureCloned(ct);
        return await Git(["show", $"origin/{branch}:{filepath}"], ct);
    }
}
