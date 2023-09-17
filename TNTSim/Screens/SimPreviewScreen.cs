using System.Diagnostics;
using System.Numerics;

namespace TNTSim.Screens;

internal static class SimPreviewScreen
{
    private const string RUNNING = "Running";
    private const string PAUSED = "Paused";
    private const string CONTROLS = "WASD - Move\nSpace - Up\nShift - Down\nArrows - Rotate\nP - Pause/Play\nT - Step 1 tick\nEscape - Exit";
    private static readonly int RUNNING_X = (WINDOW_WIDTH - MeasureText(RUNNING, FONT_SIZE)) / 2;
    private static readonly int PAUSED_X = (WINDOW_WIDTH - MeasureText(PAUSED, FONT_SIZE)) / 2;
    private static readonly Vector2 CONTROLS_V = new Vector2(WINDOW_WIDTH - PADDING, WINDOW_HEIGHT - PADDING) - MeasureTextEx(GetFontDefault(), CONTROLS, FONT_SIZE, 1.0f);

    private static readonly Stopwatch TIMER = new();
    private static Camera3D camera;
    private static SimulationContext? current = null;
    private static double lastMSPT = 0;
    static float cameraYaw = 5f * MathF.PI / 4f, cameraPitch = MathF.PI / 4f;

    public static void Start(ref CannonSettings cannonSettings, ComponentGroup simSettingsGroup)
    {
        TIMER.Reset();
        SimulationSettings settings = new()
        {
            cannonSettings = cannonSettings,
            payloadY = simSettingsGroup.GetComponent<NumberBox>(3).Value
        };
        camera = new()
        {
            fovy = 90f,
            position = new Vector3(25, (float)settings.payloadY + 20, 25),
            target = default,
            up = Vector3.UnitY,
            projection = CameraProjection.CAMERA_PERSPECTIVE
        };
        current = Simulator.Create(settings);
    }

    public static bool UpdateAndDraw(ref CannonSettings settings)
    {
        if (current == null)
        {
            TraceLog(TraceLogLevel.LOG_WARNING, "Attempted to update simulation when there is no current simulation");
            return false;
        }
        BeginMode3D(camera);

        DrawGrid(10, 100);

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
        if (IsKeyPressed(KeyboardKey.KEY_ESCAPE) || IsKeyPressed(KeyboardKey.KEY_TAB))
        {
            current = null;
            return false;
        }
        return true;
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
        float moveSpeed = 50f * GetFrameTime();
        float rotSpeed = 3f * GetFrameTime();

        bool space = IsKeyDown(KeyboardKey.KEY_SPACE);
        bool shift = IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) || IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT);
        bool w = IsKeyDown(KeyboardKey.KEY_W);
        bool a = IsKeyDown(KeyboardKey.KEY_A);
        bool s = IsKeyDown(KeyboardKey.KEY_S);
        bool d = IsKeyDown(KeyboardKey.KEY_D);
        bool up = IsKeyDown(KeyboardKey.KEY_UP);
        bool left = IsKeyDown(KeyboardKey.KEY_LEFT);
        bool down = IsKeyDown(KeyboardKey.KEY_DOWN);
        bool right = IsKeyDown(KeyboardKey.KEY_RIGHT);

        if (up && !down)
        {
            cameraPitch -= rotSpeed;
        }
        if (down && !up)
        {
            cameraPitch += rotSpeed;
        }

        if (left && !right)
        {
            cameraYaw += rotSpeed;
        }
        if (right && !left)
        {
            cameraYaw -= rotSpeed;
        }

        camera.up = Vector3.UnitY;
        cameraPitch = float.Clamp(cameraPitch, -(MathF.PI / 2) + 0.001f, (MathF.PI / 2) - 0.001f);
#if DEBUG
        cameraYaw -= MathF.Tau * MathF.Floor(cameraYaw / MathF.Tau); // Wrap at 2pi so it looks nicer in debugger
#endif
        Quaternion q = Quaternion.CreateFromYawPitchRoll(cameraYaw, cameraPitch, 0);
        Vector3 fwd = Vector3.Transform(Vector3.UnitZ, q);
        fwd /= fwd.Length();
        Vector3 side = Vector3.Cross(fwd, Vector3.UnitY);

        if (space && !shift)
        {
            camera.position += Vector3.UnitY * moveSpeed;
        }
        else if (shift && !space)
        {
            camera.position -= Vector3.UnitY * moveSpeed;
        }
        if (w && !s)
        {
            camera.position += fwd * moveSpeed;
        }
        if (s && !w)
        {
            camera.position -= fwd * moveSpeed;
        }
        if (a && !d)
        {
            camera.position -= side * moveSpeed;
        }
        if (d && !a)
        {
            camera.position += side * moveSpeed;
        }

        camera.target = fwd + camera.position;
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
    }
}
