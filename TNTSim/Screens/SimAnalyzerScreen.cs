namespace TNTSim.Screens;

internal static class SimAnalyzerScreen
{
    private const int START_Y = CONTROL_HEIGHT + PADDING + PADDING;
    private const string LOADING_TEXT = "Loading...";
    private static readonly int LOADING_TEXT_X = (WINDOW_WIDTH - MeasureText(LOADING_TEXT, FONT_SIZE)) / 2;
    private static Analyzer? current = null;

    public static bool UpdateAndDraw()
    {
        if (current is null || IsKeyPressed(KeyboardKey.KEY_ESCAPE) || IsKeyPressed(KeyboardKey.KEY_TAB))
        {
            current = null;
            return false;
        }

        if (!current.IsComplete())
        {
            DrawText(LOADING_TEXT, LOADING_TEXT_X, START_Y, FONT_SIZE, Color.GRAY);
            DrawRectangleLines(PADDING, START_Y + CONTROL_HEIGHT, WINDOW_WIDTH - (2 * PADDING), FONT_SIZE, Color.GRAY);
            int w = (int)(current.PercentComplete * (WINDOW_WIDTH - (4 * PADDING)));
            DrawRectangle(PADDING * 2, START_Y + CONTROL_HEIGHT + PADDING, w, FONT_SIZE - (2 * PADDING), Color.GREEN);
            return true;
        }

        return true;
    }

    public static void Start(SimulationSettings settings) => current = new(settings);
}
