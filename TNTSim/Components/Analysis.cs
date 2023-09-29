namespace TNTSim.Components;
internal sealed class Analysis : Component
{
    public const int HEIGHT = 200;
    public const int TEXT_Y = PADDING;
    public const int GRAPH_Y = TEXT_Y + CONTROL_HEIGHT;
    public const int AVG_Y = TEXT_Y + CONTROL_HEIGHT;
    public const int MED_Y = AVG_Y + CONTROL_HEIGHT;
    public const int MIN_Y = MED_Y + CONTROL_HEIGHT;
    public const int MAX_Y = MIN_Y + CONTROL_HEIGHT;

    public IReadOnlyList<double> Data
    {
        get => data;
        set
        {
            data = value;
            average = data.Average();
            min = data.Min();
            max = data.Max();
            median = data.Order().ElementAtOrDefault(data.Count / 2);

            // "Average" is biggest
            graphStartX = MeasureText("Average: ", FONT_SIZE)
                + new double[] { average, min, max, median }
                .Max(static d => MeasureText(d.ToString("F2"), FONT_SIZE))
                + (PADDING * 3);
        }
    }

    private readonly string name;
    private readonly int textX;
    private IReadOnlyList<double> data = Array.Empty<double>();
    private double average, min, max, median;
    private int graphStartX;

    public Analysis(string name, int y) : base(PADDING, y, WINDOW_WIDTH - (PADDING * 2), HEIGHT)
    {
        this.name = name;
        textX = X + ((Width - MeasureText(name, FONT_SIZE)) / 2);
    }

    public override void UpdateAndDraw()
    {
        DrawText(name, textX, Y + TEXT_Y, FONT_SIZE, PrimaryColor);
        
        DrawText($"Average: {average:F2}", PADDING, Y + AVG_Y, FONT_SIZE, PrimaryColor);
        DrawText($"Median: {median:F2}", PADDING, Y + MED_Y, FONT_SIZE, PrimaryColor);
        DrawText($"Minimum: {min:F2}", PADDING, Y + MIN_Y, FONT_SIZE, PrimaryColor);
        DrawText($"Maximum: {max:F2}", PADDING, Y + MAX_Y, FONT_SIZE, PrimaryColor);

        // TODO: actually draw some graphs
        DrawRectangleLines(graphStartX, Y + GRAPH_Y, Width - graphStartX, Height - GRAPH_Y, PrimaryColor);
    }
}
