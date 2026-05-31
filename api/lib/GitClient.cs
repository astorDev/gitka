using CliWrap;

namespace Gitka.Api;

public class GitClient
{
    private readonly Command baseCommand;

    public string CacheDir { get; } = Path.Combine(Path.GetTempPath(), "gitka", Guid.NewGuid().ToString("N")[..8]);

    private GitClient(Command baseCommand)
    {
        this.baseCommand = baseCommand;
    }

    public async Task<string> Run(string dir, string[] args, CancellationToken ct = default)
    {
        return await baseCommand
            .WithWorkingDirectory(dir)
            .WithArguments(args)
            .RunAsync(ct);
    }

    public static class Factory
    {
        public static GitClient Create(Func<Command, Command> configure)
        {
            var command = Cli.Wrap("git");
            command = configure(command);
            command = command.WithExtraEnvironmentVariables(e => e
                .Set("GIT_TERMINAL_PROMPT", "0")
                .Set("GIT_PAGER", "cat")
            );

            return new GitClient(command);
        }
    }
}
