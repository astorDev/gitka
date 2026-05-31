using Confi;

dotenv.net.DotEnv.Load(new(envFilePaths: [ "../.env", ".env" ]));
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddFluentEnvironmentVariables();

builder.Logging.AddSimpleConsole(c => c.SingleLine = true);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();