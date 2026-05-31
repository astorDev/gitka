using CliWrap;
using CliWrap.Buffered;

namespace Gitka.Api;

public class GitService(RepositoryConnection connection)
{
    readonly string _workDir = Path.Combine(Path.GetTempPath(), "gitka", Guid.NewGuid().ToString("N")[..8]);

    async Task EnsureCloned(CancellationToken ct = default)
    {
        if (Directory.Exists(Path.Combine(_workDir, ".git")))
        {
            await Git(["fetch", "--all"], ct);
            return;
        }

        Directory.CreateDirectory(_workDir);
        await Git(["clone", connection.Url, "."], ct);
    }

    Command BaseGit(string[] args) =>
        Cli.Wrap("git")
            .WithWorkingDirectory(_workDir)
            .WithArguments(args)
            .WithEnvironmentVariables(e => e
                .Set("GIT_AUTHOR_NAME", connection.AuthorName)
                .Set("GIT_AUTHOR_EMAIL", connection.AuthorEmail)
                .Set("GIT_COMMITTER_NAME", connection.AuthorName)
                .Set("GIT_COMMITTER_EMAIL", connection.AuthorEmail)
                .Set("GIT_TERMINAL_PROMPT", "0")
                .Set("GIT_PAGER", "cat"));

    async Task Git(string[] args, CancellationToken ct = default)
    {
        var result = await BaseGit(args)
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync(ct);
        if (result.ExitCode != 0)
            throw new InvalidOperationException($"git {args[0]} failed (exit {result.ExitCode}): {result.StandardError}");
    }

    async Task<string> GitOutput(string[] args, CancellationToken ct = default)
    {
        var result = await BaseGit(args).ExecuteBufferedAsync(ct);
        return result.StandardOutput;
    }

    public async Task<string?> GetFile(string branch, string filepath, CancellationToken ct = default)
    {
        await EnsureCloned(ct);
        try
        {
            return await GitOutput(["show", $"origin/{branch}:{filepath}"], ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task PutFile(string branch, string filepath, string content, CancellationToken ct = default)
    {
        await EnsureCloned(ct);

        // Check if branch exists on remote
        var remoteBranches = await GitOutput(["branch", "-r"], ct);
        var branchExists = remoteBranches.Split('\n').Any(b => b.Trim() == $"origin/{branch}");

        if (branchExists)
            await Git(["checkout", "-B", branch, $"origin/{branch}"], ct);
        else
            await Git(["checkout", "-b", branch], ct);

        // Write the file
        var fullPath = Path.Combine(_workDir, filepath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await File.WriteAllTextAsync(fullPath, content, ct);

        await Git(["add", filepath], ct);
        await Git(["commit", "-m", $"gitka: update {filepath}"], ct);
        await Git(["push", "origin", branch], ct);
    }
}
