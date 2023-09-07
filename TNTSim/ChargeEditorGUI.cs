namespace TNTSim;

internal static class ChargeEditorGUI
{
    private const int START_Y = CONTROL_HEIGHT + PADDING;
    private const int CHARGE_LABEL_WIDTH = 100;
    private const string TITLE = "Charge Editor";
    private static readonly int TITLE_X = (800 - MeasureText(TITLE, FONT_SIZE)) / 2;
    private static readonly int START_X = (800 - CHARGE_LABEL_WIDTH - ChargeEditor.WIDTH) / 2;

    private static readonly CenteredText[] CHARGE_LABELS = Enumerable
        .Range(0, 5)
        .Select(i => new CenteredText($"Charge #{i + 1}", START_X, START_Y + (i * ChargeEditor.HEIGHT), CHARGE_LABEL_WIDTH, ChargeEditor.HEIGHT))
        .ToArray();
	private static readonly ChargeEditor[] CHARGE_EDITORS = Enumerable.Range(0, 5)
        .Select(i => new ChargeEditor(START_X + CHARGE_LABEL_WIDTH, CHARGE_LABELS[i].Y))
        .ToArray();

	public static void UpdateAndDraw(in Charge[] charges)
    {
        DrawText(TITLE, TITLE_X, PADDING, FONT_SIZE, Color.GRAY);
        DrawLine(0, CONTROL_HEIGHT, 800, CONTROL_HEIGHT, Color.GRAY);
        for (int i = 0; i < 5; i++)
        {
            CHARGE_LABELS[i].UpdateAndDraw();
            CHARGE_EDITORS[i].UpdateAndDraw();
            charges[i] = CHARGE_EDITORS[i].Charge;
        }
    }
}
