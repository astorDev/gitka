namespace Gitka.Api;

public record RepositoryConnection(string Url, string AuthorName, string AuthorEmail)
{
    public static RepositoryConnection Parse(string connectionString)
    {
        var parts = connectionString.Split(';');
        var url = parts[0].Trim();
        var dict = parts.Skip(1)
            .Select(p => p.Split('=', 2))
            .Where(p => p.Length == 2)
            .ToDictionary(p => p[0].Trim(), p => p[1].Trim());

        var authorName = dict.GetValueOrDefault("authorName", "Gitka Bot");
        var authorEmail = dict.GetValueOrDefault("authorEmail", "gitka@bot.local");
        return new(url, authorName, authorEmail);
    }
}
