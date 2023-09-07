InitWindow(800, 600, "TNT Sim");

while (!WindowShouldClose())
{
    BeginDrawing();
    ClearBackground(Color.RAYWHITE);
    GUI.UpdateAndDraw();
#if DEBUG
    DrawFPS(0, 0);
#endif
    EndDrawing();
}

CloseWindow();