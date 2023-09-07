namespace TNTSim.Screens;

internal static class BreadboardEditorScreen
{
	private const int START_Y = CONTROL_HEIGHT + PADDING + PADDING;
	private const string TITLE = "Breadboard Editor";
	private static readonly int TITLE_X = (WINDOW_WIDTH - MeasureText(TITLE, FONT_SIZE)) / 2;

	public static void UpdateAndDraw()
    {
		DrawText(TITLE, TITLE_X, 2 * PADDING, FONT_SIZE, Color.GRAY);
		DrawLine(0, CONTROL_HEIGHT + PADDING, WINDOW_WIDTH, CONTROL_HEIGHT + PADDING, Color.GRAY);
	}
}
