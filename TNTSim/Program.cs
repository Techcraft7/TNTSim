InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "TNT Sim");
SetExitKey(KeyboardKey.KEY_NULL);

SetTargetFPS(Enumerable.Range(0, GetMonitorCount()).Max(GetMonitorRefreshRate));

Screen screen = Screen.CHARGES;
CannonSettings settings = new();
settings.LoadDefaults();

bool nextTabPressed = false;
const string NEXT_TEXT = "Next Page >";
int NEXT_TEXT_W = MeasureText(NEXT_TEXT, FONT_SIZE);
Button nextTabButton = new(NEXT_TEXT, WINDOW_WIDTH - (3 * PADDING) - NEXT_TEXT_W, PADDING / 2, NEXT_TEXT_W + (2 * PADDING), () => nextTabPressed = true);

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
		case Screen.HELP:
			HelpScreen.UpdateAndDraw();
			break;
		default:
			screen = Screen.CHARGES;
			break;
	}

	nextTabButton.UpdateAndDraw();
	if (nextTabPressed || IsKeyPressed(KeyboardKey.KEY_TAB))
	{
		nextTabPressed = false;
		screen = IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) ? screen.Prev() : screen.Next();
		if (IsCursorHidden())
		{
			EnableCursor();
		}
	}

	DrawFPS(PADDING, PADDING);
	EndDrawing();
}

CloseWindow();
