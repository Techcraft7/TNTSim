using TNTSim.Renderer;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<RendererService>();

var app = builder.Build();

app.Run();