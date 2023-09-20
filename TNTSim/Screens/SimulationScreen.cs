using TNTSim.Cannon;

namespace TNTSim.Screens;

internal static class SimulationScreen
{
    private const int START_Y = CONTROL_HEIGHT + PADDING + PADDING;
    private const string TITLE = "Simulator";
    private static readonly int TITLE_X = (WINDOW_WIDTH - MeasureText(TITLE, FONT_SIZE)) / 2;
    private static readonly ComponentGroup SETTINGS = new ComponentGroupBuilder()
        .AddButton("KABOOM!", onClick: () => shouldStart = true)
        .AddButton("Analyze", onClick: () => throw new NotImplementedException())
        .EndRow()
        .AddText("Payload Y")
        .AddNumberBox(0, 319, initial: 255)
        .EndRow()
        .AddCheckBox("Even Spacing")
        .EndRow()
        .Build(START_Y);

    private static bool shouldStart = false;
    private static bool started = false;

    public static void UpdateAndDraw(ref CannonSettings cannonSettings)
    {
        if (started)
        {
            started = SimPreviewScreen.UpdateAndDraw();
        }
        else
        {
            SETTINGS.UpdateAndDraw();
        }

        DrawRectangle(0, 0, WINDOW_WIDTH, CONTROL_HEIGHT + PADDING, Color.WHITE);
        DrawText(TITLE, TITLE_X, 2 * PADDING, FONT_SIZE, Color.GRAY);
        DrawLine(0, CONTROL_HEIGHT + PADDING, WINDOW_WIDTH, CONTROL_HEIGHT + PADDING, Color.GRAY);

        if (shouldStart)
        {
			SimulationSettings settings = new()
			{
				cannonSettings = cannonSettings,
				payloadY = SETTINGS.GetComponent<NumberBox>(3).Value,
				evenSpacing = SETTINGS.GetComponent<CheckBox>(4).Value
			};
			SimPreviewScreen.Start(settings);
            shouldStart = false;
            started = true;
        }
    }
}
