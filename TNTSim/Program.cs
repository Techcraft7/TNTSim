InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "TNT Sim");
SetExitKey(KeyboardKey.KEY_NULL);

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

    switch (screen)
    {
        case Screen.CHARGES:
            ChargeEditorScreen.UpdateAndDraw(ref settings);
            break;
        case Screen.BREADBOARDS:
            BreadboardEditorScreen.UpdateAndDraw(ref settings);
            break;
        case Screen.SIMULATION:
            SimulationScreen.UpdateAndDraw(ref settings);
            break;
        default:
            screen = Screen.CHARGES;
            break;
    }

    nextTabButton.UpdateAndDraw();
    if (nextTabPressed || IsKeyPressed(KeyboardKey.KEY_TAB))
    {
        nextTabPressed = false;
        screen = screen.Next();
        if (IsCursorHidden())
        {
            EnableCursor();
        }
    }

    DrawFPS(PADDING, PADDING);
    EndDrawing();
}

CloseWindow();
