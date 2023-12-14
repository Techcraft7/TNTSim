using static Raylib_cs.Raylib;

namespace TNTSim;

internal sealed class Renderer : BackgroundService
{
	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		InitWindow(800, 600, "TNT Cannon Simulator");

		while (!WindowShouldClose() && !stoppingToken.IsCancellationRequested)
		{
			BeginDrawing();

			EndDrawing();
		}

		CloseWindow();

		return Task.CompletedTask;
	}
}
