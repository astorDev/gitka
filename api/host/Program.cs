using Confi;
using Gitka.Api;

dotenv.net.DotEnv.Load(new(envFilePaths: [ "../.env", ".env" ]));
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddFluentEnvironmentVariables();

builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

var connectionString = builder.Configuration.GetConnectionString("Repository")
    ?? throw new InvalidOperationException("ConnectionStrings:Repository is required");

var repoConnection = RepositoryConnection.Parse(connectionString);
builder.Services.AddSingleton(repoConnection);
builder.Services.AddSingleton<GitService>();

var app = builder.Build();

app.MapGet("/{branch}/{**filepath}", async (string branch, string filepath, GitService git, CancellationToken ct) =>
{
    var content = await git.GetFile(branch, filepath, ct);
    return content is null ? Results.NotFound() : Results.Text(content);
});

app.MapPut("/{branch}/{**filepath}", async (string branch, string filepath, HttpRequest request, GitService git, CancellationToken ct) =>
{
    using var reader = new StreamReader(request.Body);
    var content = await reader.ReadToEndAsync(ct);
    await git.PutFile(branch, filepath, content, ct);
    return Results.Ok();
});

app.Run();