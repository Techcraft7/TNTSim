namespace TNTSim.Screens;

internal static class ChargeEditorScreen
{
    private const int START_Y = CONTROL_HEIGHT + PADDING + CONTROL_HEIGHT + PADDING;
    private const int CHARGE_LABEL_WIDTH = 100;
    private const string TITLE = "Charge Editor";
    private const string DEFAULTS_TEXT = "Defaults";
    private static readonly int TITLE_X = (WINDOW_WIDTH - MeasureText(TITLE, FONT_SIZE)) / 2;
    private static readonly int START_X = (WINDOW_WIDTH - CHARGE_LABEL_WIDTH - ChargeEditor.WIDTH) / 2;

    private static readonly CenteredText[] CHARGE_LABELS = Enumerable
        .Range(0, 5)
        .Select(i => new CenteredText($"Charge #{i + 1}", START_X, START_Y + i * ChargeEditor.HEIGHT, CHARGE_LABEL_WIDTH, ChargeEditor.HEIGHT))
        .ToArray();
    private static readonly ChargeEditor[] CHARGE_EDITORS = Enumerable.Range(0, 5)
        .Select(i => new ChargeEditor(START_X + CHARGE_LABEL_WIDTH, CHARGE_LABELS[i].Y))
        .ToArray();
    private static readonly Button DEFAULTS_BUTTON = new(DEFAULTS_TEXT, (WINDOW_WIDTH - Button.GetMinWidth(DEFAULTS_TEXT)) / 2, CHARGE_EDITORS[^1].GetBottomSide() + PADDING, Button.GetMinWidth(DEFAULTS_TEXT), () => shouldLoadDefaults = true);

    private static bool shouldLoadDefaults = false;

    public static void UpdateAndDraw(ref CannonSettings settings)
    {
        DrawText(TITLE, TITLE_X, PADDING, FONT_SIZE, Color.GRAY);
        DrawLine(0, CONTROL_HEIGHT, WINDOW_WIDTH, CONTROL_HEIGHT, Color.GRAY);

        for (int i = 0; i < 5; i++)
        {
            CHARGE_LABELS[i].UpdateAndDraw();
            CHARGE_EDITORS[i].UpdateAndDraw();
            settings.SetCharge(i, CHARGE_EDITORS[i].Charge);
        }

        DEFAULTS_BUTTON.UpdateAndDraw();

        if (shouldLoadDefaults)
        {
            shouldLoadDefaults = false;
            for (int i = 0; i < 5; i++)
            {
                settings.SetCharge(i, Charge.DEFAULT);
                CHARGE_EDITORS[i].SetCharge(Charge.DEFAULT);
            }
        }
    }
}
