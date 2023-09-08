using TNTSim.Cannon;

InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "TNT Sim");

SetTargetFPS(Enumerable.Range(0, GetMonitorCount()).Max(GetMonitorRefreshRate));


Screen screen = Screen.CHARGES;
CannonSettings settings = new();
settings.LoadDefaults();

bool nextTabPressed = false;
Button nextTabButton = new(">", WINDOW_WIDTH - PADDING - CONTROL_HEIGHT, PADDING / 2, CONTROL_HEIGHT, () => nextTabPressed = true);

while (!WindowShouldClose())
{
    BeginDrawing();
    ClearBackground(Color.RAYWHITE);

    nextTabButton.UpdateAndDraw();

    if (nextTabPressed || IsKeyPressed(KeyboardKey.KEY_TAB))
    {
        nextTabPressed = false;
        screen = screen.Next();
    }

    switch (screen)
    {
        case Screen.CHARGES:
            ChargeEditorScreen.UpdateAndDraw(ref settings);
            break;
        case Screen.BREADBOARDS:
            BreadboardEditorScreen.UpdateAndDraw(ref settings);
            break;
        case Screen.SIMULATION:
            if (IsKeyPressed(KeyboardKey.KEY_SPACE))
            {
                Simulator.Simulate(new()
                {
                    cannonSettings = settings,
                    payloadY = 319
                });
            }
            break;
        default:
            screen = Screen.CHARGES;
            break;
    }

#if DEBUG
    DrawFPS(PADDING, PADDING);
#endif
    EndDrawing();
}

CloseWindow();
