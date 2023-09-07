InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "TNT Sim");

SetTargetFPS(Enumerable.Range(0, GetMonitorCount()).Max(GetMonitorRefreshRate));

CannonSettings settings = new();

while (!WindowShouldClose())
{
    BeginDrawing();
    ClearBackground(Color.RAYWHITE);
    ChargeEditorGUI.UpdateAndDraw(ref settings);
#if DEBUG
    DrawFPS(0, 0);
#endif
    EndDrawing();
}

CloseWindow();