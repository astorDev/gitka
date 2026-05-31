using CliWrap;

namespace Gitka.Api;

public static class GitkaNames
{
    const string AuthorName = "Gitka Bot";
    const string AuthorEmail = "gitka@bot.local";

    public static Command WithGitkaNameEnvironmentVariables(this Command command)
    {
        return command.WithEnvironmentVariables(e => e
            .Set("GIT_AUTHOR_NAME", AuthorName)
            .Set("GIT_AUTHOR_EMAIL", AuthorEmail)
            .Set("GIT_COMMITTER_NAME", AuthorName)
            .Set("GIT_COMMITTER_EMAIL", AuthorEmail)
            .SetExistingOf(command)
        );
    }
}
