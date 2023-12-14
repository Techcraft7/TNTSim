using static Raylib_cs.Raylib;

namespace TNTSim.Renderer;

internal sealed class RendererService(ILogger<RendererService> logger, IHostApplicationLifetime lifetime) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Initializing Raylib");
        SetTraceLogLevel(TraceLogLevel.LOG_ERROR);

        logger.LogInformation("Creating window");
        InitWindow(800, 600, "TNT Cannon Simulator");
        SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE);

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
