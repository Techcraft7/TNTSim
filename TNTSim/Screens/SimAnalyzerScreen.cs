namespace TNTSim.Screens;

internal static class SimAnalyzerScreen
{
    private static Analyzer? current = null;

    public static bool UpdateAndDraw()
    {
        if (current is null || IsKeyPressed(KeyboardKey.KEY_ESCAPE) || IsKeyPressed(KeyboardKey.KEY_TAB))
        {
            current = null;
            return false;
        }

        DrawText("TODO", 100, 100, FONT_SIZE, Color.RED);
        DrawText($"{current.PercentComplete:F2}%", 100, 100 + FONT_SIZE + PADDING, FONT_SIZE, Color.RED);

        return true;
    }

    public static void Start(SimulationSettings settings) => current = new(settings);
}
