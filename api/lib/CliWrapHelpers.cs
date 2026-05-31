using CliWrap;
using CliWrap.Buffered;

namespace Gitka.Api;

// TO DO: Move to nice-shell repository
public static class CliWrapHelpers
{
    public static CliWrap.Builders.EnvironmentVariablesBuilder SetExistingOf(this CliWrap.Builders.EnvironmentVariablesBuilder builder, Command command)
    {
        foreach (var kvp in command.EnvironmentVariables)
        {
            builder.Set(kvp.Key, kvp.Value);
        }

        return builder;
    }

    public static Command WithExtraEnvironmentVariables(this Command command, Action<CliWrap.Builders.EnvironmentVariablesBuilder> configure)
    {
        return command.WithEnvironmentVariables(builder =>
        {
            builder.SetExistingOf(command);
            configure(builder);
        });
    }

    public static async Task<string> RunAsync(this Command command, CancellationToken ct = default)
    {
        var result = await command.ExecuteBufferedAsync(ct);
        return result.ExitCode == 0 ? result.StandardOutput : throw new InvalidOperationException($"Command failed (exit {result.ExitCode}): {result.StandardError}");
    }
}
