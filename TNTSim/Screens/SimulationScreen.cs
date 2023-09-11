using System.Diagnostics;
using System.Numerics;

namespace TNTSim.Screens;

internal static class SimulationScreen
{
    private const int START_Y = CONTROL_HEIGHT + PADDING + PADDING;
    private const string TITLE = "Simulator";
    private const string RUN_BTN_TEXT = "KABOOM!";
    private const string PAUSED = "Paused";
    private const string RUNNING = "Running";
    private const string CONTROLS = "WASD - Move\nSpace - Up\nShift - Down\nArrows - Rotate\nP - Pause/Play\nT - Step 1 tick\nEscape - Exit";
    private static readonly int TITLE_X = (WINDOW_WIDTH - MeasureText(TITLE, FONT_SIZE)) / 2;
    private static readonly int RUN_BTN_WIDTH = Button.GetMinWidth(RUN_BTN_TEXT);
    private static readonly int RUN_BTN_X = (WINDOW_WIDTH - RUN_BTN_WIDTH) / 2;
    private static readonly int PAUSED_X = (WINDOW_WIDTH - MeasureText(PAUSED, FONT_SIZE)) / 2;
    private static readonly int RUNNING_X = (WINDOW_WIDTH - MeasureText(RUNNING, FONT_SIZE)) / 2;
    private static readonly Vector2 CONTROLS_V = new Vector2(WINDOW_WIDTH - PADDING, WINDOW_HEIGHT - PADDING) - MeasureTextEx(GetFontDefault(), CONTROLS, FONT_SIZE, 1.0f);

    private static readonly Stopwatch TIMER = new();
    private static Camera3D camera;
    private static readonly Button runButton = new(RUN_BTN_TEXT, RUN_BTN_X, START_Y, RUN_BTN_WIDTH, () => shouldStart = true)
    {
        PrimaryColor = Color.RED
    };
    private static SimulationContext? current = null;
    private static bool shouldStart = false;
    private static double lastMSPT = 0;

    public static void UpdateAndDraw(ref CannonSettings settings)
    {
        if (current != null)
        {
            DisableCursor();
            UpdateSimulation();
        }
        else
        {
            if (IsCursorHidden())
            {
                EnableCursor();
            }
            runButton.UpdateAndDraw();
        }

        DrawRectangle(0, 0, WINDOW_WIDTH, CONTROL_HEIGHT + PADDING, Color.WHITE);
        DrawText(TITLE, TITLE_X, 2 * PADDING, FONT_SIZE, Color.GRAY);
        DrawLine(0, CONTROL_HEIGHT + PADDING, WINDOW_WIDTH, CONTROL_HEIGHT + PADDING, Color.GRAY);

        if (shouldStart)
        {
            camera = new()
            {
                fovy = 90f,
                position = 255 * new Vector3(0.8f, 1.25f, 0.8f),
                target = default,
                up = Vector3.UnitY,
                projection = CameraProjection.CAMERA_PERSPECTIVE
            };
            shouldStart = false;
            TIMER.Reset();
            current = Simulator.Create(new()
            {
                cannonSettings = settings,
                payloadY = 255 // TODO: make this configurable
            });
        }
    }

    private static void UpdateSimulation()
    {
        if (current == null)
        {
            TraceLog(TraceLogLevel.LOG_WARNING, "Attempted to update simulation when there is no current simulation");
            return;
        }
        BeginMode3D(camera);

        DrawGrid(640, 1);

        if (TIMER.ElapsedMilliseconds >= 50)
        {
            TimeSpan start = TIMER.Elapsed;
            current.Tick();
            TimeSpan end = TIMER.Elapsed;
            lastMSPT = (end - start).TotalMilliseconds;
            TIMER.Restart();
        }
        DrawEntities();

        EndMode3D();

        if (TIMER.IsRunning)
        {
            DrawText(RUNNING, RUNNING_X, WINDOW_HEIGHT - PADDING - FONT_SIZE, FONT_SIZE, Color.GREEN);
        }
        else
        {
            DrawText(PAUSED, PAUSED_X, WINDOW_HEIGHT - PADDING - FONT_SIZE, FONT_SIZE, Color.RED);
        }
        DrawText(CONTROLS, (int)CONTROLS_V.X, (int)CONTROLS_V.Y, FONT_SIZE, Color.BLACK);

        DrawText($"MSPT: {lastMSPT}ms", PADDING, WINDOW_HEIGHT - FONT_SIZE - PADDING, FONT_SIZE, Color.BLACK);
        DrawText($"TNT: {current.TNT.Count}", PADDING, WINDOW_HEIGHT - FONT_SIZE - PADDING - CONTROL_HEIGHT - PADDING, FONT_SIZE, Color.BLACK);

        UpdateCameraControls();
        UpdateSimulationControls();
    }

    private static void DrawEntities()
    {
        if (current is null)
        {
            return;
        }
        foreach (TNT tnt in current.TNT)
        {
            Vec3 offset = tnt.velocity * (TIMER.ElapsedTicks / (50.0 * TimeSpan.TicksPerMillisecond));
            Vec3 pos = tnt.position + offset;

            DrawCube(pos, 0.98f, 0.98f, 0.98f, tnt.fuse / 5 % 2 == 0 ? Color.WHITE : Color.RED);
            DrawLine3D(pos, pos + tnt.velocity, Color.GREEN);
        }
        foreach (Vec3 exp in current.Explosions)
        {
            DrawCircle3D(exp, MathF.Sqrt(16 - MathF.Pow((float)exp.Y, 2)), Vector3.UnitX, 90f, Color.ORANGE);
        }
    }

    private static void UpdateCameraControls()
    {
        if (current == null)
        {
            TraceLog(TraceLogLevel.LOG_WARNING, "Attempted to update simulation when there is no current simulation");
            return;
        }
        float speed = 50f * GetFrameTime();

        bool space = IsKeyDown(KeyboardKey.KEY_SPACE);
        bool shift = IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) || IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT);

        UpdateCamera(ref camera, CameraMode.CAMERA_FIRST_PERSON);
        UpdateCamera(ref camera, CameraMode.CAMERA_FIRST_PERSON);
        camera.up = Vector3.UnitY;

        if (space && !shift)
        {
            camera.position += Vector3.UnitY * speed;
        }
        else if (shift && !space)
        {
            camera.position -= Vector3.UnitY * speed;
        }
    }

    private static void UpdateSimulationControls()
    {
        if (current == null)
        {
            TraceLog(TraceLogLevel.LOG_WARNING, "Attempted to update simulation when there is no current simulation");
            return;
        }
        if (IsKeyPressed(KeyboardKey.KEY_P))
        {
            if (TIMER.IsRunning)
            {
                TIMER.Stop();
            }
            else
            {
                TIMER.Start();
            }
        }
        if (IsKeyPressed(KeyboardKey.KEY_T))
        {
            current.Tick();
        }
        if (IsKeyPressed(KeyboardKey.KEY_ESCAPE) || IsKeyPressed(KeyboardKey.KEY_TAB))
        {
            current = null;
        }
    }
}
