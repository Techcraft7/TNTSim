InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "TNT Sim");

SetTargetFPS(Enumerable.Range(0, GetMonitorCount()).Max(GetMonitorRefreshRate));


Screen screen = Screen.CHARGES;
CannonSettings settings = new();
settings.LoadDefaults();

while (!WindowShouldClose())
{
	BeginDrawing();
	ClearBackground(Color.RAYWHITE);

	if (IsKeyPressed(KeyboardKey.KEY_TAB))
	{
		screen = screen.Next();
	}

	switch (screen)
	{
		case Screen.CHARGES:
			ChargeEditorScreen.UpdateAndDraw(ref settings);
			break;
		case Screen.BREADBOARDS:
			BreadboardEditorScreen.UpdateAndDraw();
			break;
		case Screen.SIMULATION:
			// TODO: SIMULATION
			break;
		default:
			screen = Screen.CHARGES;
			break;
	}

#if DEBUG
	DrawFPS(0, 0);
#endif
	EndDrawing();
}

CloseWindow();
