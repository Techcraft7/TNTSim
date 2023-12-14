using static Raylib_cs.Raylib;

namespace TNTSim;

internal sealed class Renderer(ILogger<Renderer> logger, IHostApplicationLifetime lifetime) : BackgroundService
{
	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Initializing Raylib");
		SetTraceLogLevel(TraceLogLevel.LOG_ERROR);

		logger.LogInformation("Creating window");
		InitWindow(800, 600, "TNT Cannon Simulator");

		while (!WindowShouldClose() && !stoppingToken.IsCancellationRequested)
		{
			BeginDrawing();

			EndDrawing();
		}

		logger.LogInformation("Closing window");
		CloseWindow();
		
		lifetime.StopApplication();
		return Task.CompletedTask;
	}
}
