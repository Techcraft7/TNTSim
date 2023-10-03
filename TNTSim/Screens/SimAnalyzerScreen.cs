namespace TNTSim.Screens;

internal static class SimAnalyzerScreen
{
	private const int START_Y = CONTROL_HEIGHT + PADDING + PADDING;
	private const int PROGRESS_Y = START_Y + CONTROL_HEIGHT;
	private const int TNT_GRAPH_Y = START_Y + Analysis.HEIGHT + PADDING;
	private const int EXPLOSION_INFO_HEADER_Y = TNT_GRAPH_Y + Analysis.HEIGHT + (PADDING * 3);
	private const int EXPLOSION_INFO_Y = EXPLOSION_INFO_HEADER_Y + FONT_SIZE + PADDING;
	private const string LOADING_TEXT = "Loading...";
	private const string EXPLOSION_INFO_HEADER = "Explosions";

	private static readonly int LOADING_TEXT_X = (WINDOW_WIDTH - MeasureText(LOADING_TEXT, FONT_SIZE)) / 2;
	private static readonly int EXPLOSION_INFO_HEADER_X = (WINDOW_WIDTH - MeasureText(EXPLOSION_INFO_HEADER, FONT_SIZE)) / 2;
	private static readonly Analysis MSPT_GRAPH = new("MSPT", START_Y);
	private static readonly Analysis TNT_GRAPH = new("TNT", TNT_GRAPH_Y);

	private static Analyzer? current = null;
	private static bool firstAfterDone = true;

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

			DrawRectangleLines(PADDING, PROGRESS_Y, WINDOW_WIDTH - (2 * PADDING), FONT_SIZE, Color.GRAY);
			int w = (int)(current.PercentComplete * (WINDOW_WIDTH - (4 * PADDING)));
			DrawRectangle(PADDING * 2, PROGRESS_Y + PADDING, w, FONT_SIZE - (2 * PADDING), Color.GREEN);
			firstAfterDone = true;
			return true;
		}

		if (firstAfterDone)
		{
			MSPT_GRAPH.Data = current.MSPT;
			TNT_GRAPH.Data = current.TNTCounts.Select(static i => (double)i).ToList();
			firstAfterDone = false;
		}

		MSPT_GRAPH.UpdateAndDraw();
		TNT_GRAPH.UpdateAndDraw();

		DrawText(EXPLOSION_INFO_HEADER, EXPLOSION_INFO_HEADER_X, EXPLOSION_INFO_HEADER_Y, FONT_SIZE, Color.GRAY);
		if (current.TNTCounts.Any(static x => x > 0))
		{
			DrawText($"Center: ({current.CenterX:F1}, {current.CenterZ:F1})", PADDING, EXPLOSION_INFO_Y, FONT_SIZE, Color.GRAY);
			DrawText($"Damaged Radius: ({current.DamageRadiusX:F1}, {current.DamageRadiusZ:F1})", PADDING, EXPLOSION_INFO_Y + FONT_SIZE + PADDING, FONT_SIZE, Color.GRAY);
			DrawText($"Damage % Area: {current.DamagePercentage * 100:F1}", PADDING, EXPLOSION_INFO_Y + ((FONT_SIZE + PADDING) * 2), FONT_SIZE, Color.GRAY);
		}
		else
		{
			DrawText("No explosions!", PADDING, EXPLOSION_INFO_Y, FONT_SIZE, Color.RED);
		}

		return true;
	}

	public static void Start(SimulationSettings settings)
	{
		firstAfterDone = true;
		current = new(settings);
	}
}
