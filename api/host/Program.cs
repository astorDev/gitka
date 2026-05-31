using Confi;
using Gitka.Api;
using Microsoft.AspNetCore.Mvc;

dotenv.net.DotEnv.Load(new(envFilePaths: [ "../.env", ".env" ]));
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddFluentEnvironmentVariables();

builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

var repositoryUrl = builder.Configuration["Repository:Url"]
    ?? throw new InvalidOperationException("Repository:Url is required");

builder.Services.AddSingleton(new Uri(repositoryUrl));
builder.Services.AddSingleton(_ => GitClient.Factory.Create(c => c.WithGitkaNameEnvironmentVariables()));

var app = builder.Build();

app.MapGet("/{branch}/{**filepath}", async (
    GitClient git, [FromServices]Uri repositoryUrl, string branch, string filepath, CancellationToken ct) =>
{
    await git.EnsureCacheIsUpToDate(repositoryUrl, ct);
    return await git.GetFileFromCache(branch, filepath, ct);
});

app.MapPut("/{branch}/{**filepath}", async (
    GitClient git, [FromServices] Uri repositoryUrl, string branch, string filepath, HttpRequest request, CancellationToken ct) =>
{
    using var reader = new StreamReader(request.Body);
    var content = await reader.ReadToEndAsync(ct);
    await git.PushFile(repositoryUrl, branch, filepath, content, ct);
});

app.Run();