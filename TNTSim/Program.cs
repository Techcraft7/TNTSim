using TNTSim.Renderer;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("settings.json");
builder.Services.AddHostedService<RendererService>();

var app = builder.Build();

app.Run();