namespace Gitka.Api;

public static class GitFilesCache
{
    static Task<string> Git(GitClient client, string[] args, CancellationToken ct) =>
        client.Run(client.CacheDir, args, ct);

    public static async Task EnsureCacheIsUpToDate(this GitClient client, Uri repositoryUrl, CancellationToken ct)
    {
        if (Directory.Exists(Path.Combine(client.CacheDir, ".git")))
        {
            await Git(client, ["fetch", "--all"], ct);
            return;
        }

        Directory.CreateDirectory(client.CacheDir);
        await Git(client, ["clone", repositoryUrl.ToString(), "."], ct);
    }

    public static async Task<string> GetFileFromCache(this GitClient client, string branch, string filepath, CancellationToken ct = default)
    {
        GitPathGuard.Validate(branch, nameof(branch));
        GitPathGuard.Validate(filepath, nameof(filepath));
        return await Git(client, ["show", $"origin/{branch}:{filepath}"], ct);
    }
}
