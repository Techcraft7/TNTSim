InitWindow(800, 600, "TNT Sim");

SetTargetFPS(Enumerable.Range(0, GetMonitorCount()).Max(GetMonitorRefreshRate));

Charge[] charges = new Charge[5];

while (!WindowShouldClose())
{
    BeginDrawing();
    ClearBackground(Color.RAYWHITE);
    ChargeEditorGUI.UpdateAndDraw(in charges);
#if DEBUG
    DrawFPS(0, 0);
#endif
    EndDrawing();
}

CloseWindow();