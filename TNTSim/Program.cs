using TNTSim.Renderer;

var builder = Host.CreateEmptyApplicationBuilder(new()
{
	Args = args
});

builder.Logging.AddConsole();

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("settings.json");
builder.Services.AddHostedService<RendererService>();

var app = builder.Build();

app.Run();