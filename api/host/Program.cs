using Confi;
using Gitka.Api;

dotenv.net.DotEnv.Load(new(envFilePaths: [ "../.env", ".env" ]));
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddFluentEnvironmentVariables();

builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

var repositoryUrl = builder.Configuration["Repository:Url"]
    ?? throw new InvalidOperationException("Repository:Url is required");

builder.Services.AddSingleton(new Uri(repositoryUrl));
builder.Services.AddSingleton(_ => GitClient.Factory.Create(c => c.WithGitkaNameEnvironmentVariables()));
builder.Services.AddSingleton<GitFilesCache>();
builder.Services.AddSingleton<GitFileSaver>();

var app = builder.Build();

app.MapGet("/{branch}/{**filepath}", async (string branch, string filepath, GitFilesCache cache, CancellationToken ct) =>
{
    var content = await cache.GetFile(branch, filepath, ct);
    return content is null ? Results.NotFound() : Results.Text(content);
});

app.MapPut("/{branch}/{**filepath}", async (string branch, string filepath, HttpRequest request, GitFileSaver saver, CancellationToken ct) =>
{
    using var reader = new StreamReader(request.Body);
    var content = await reader.ReadToEndAsync(ct);
    await saver.PushFile(branch, filepath, content, ct);
    return Results.Ok();
});

app.Run();