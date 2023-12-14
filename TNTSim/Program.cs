var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Renderer>();

var app = builder.Build();

app.Run();