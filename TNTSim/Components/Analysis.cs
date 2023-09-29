namespace TNTSim.Components;
internal sealed class Analysis : Component
{
    public const int HEIGHT = 200;
    public const int TEXT_Y = PADDING;
    public const int GRAPH_Y = TEXT_Y + CONTROL_HEIGHT;
    public const int GRAPH_HEIGHT = HEIGHT - GRAPH_Y - PADDING;
    public const int AVG_Y = TEXT_Y + CONTROL_HEIGHT;
    public const int MED_Y = AVG_Y + CONTROL_HEIGHT;
    public const int MIN_Y = MED_Y + CONTROL_HEIGHT;
    public const int MAX_Y = MIN_Y + CONTROL_HEIGHT;
    public const int GRAPH_Y_PAD = PADDING * 10;
    public const int GRAPH_X_PAD = PADDING * 5;
    public const int SPECIAL_MIN = 0;
    public const int SPECIAL_MAX = 1;
    public const int SPECIAL_MED = 2;
    public const float SPECIAL_POINT_SIZE = 2.5f;
    private static readonly Color[] SPECIAL_POINT_COLORS = new Color[3] { Color.RED, Color.BLUE, Color.ORANGE };

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
                + (PADDING * 3) ;

            minPos = data.Zip(Enumerable.Range(0, data.Count)).OrderBy(static t => t.First).FirstOrDefault().Second;
            maxPos = data.Zip(Enumerable.Range(0, data.Count)).OrderBy(static t => t.First).LastOrDefault().Second;
            medianPos = data.Zip(Enumerable.Range(0, data.Count)).Where(t => t.First == median).FirstOrDefault().Second;
        }
    }

    private readonly string name;
    private readonly int textX;
    private IReadOnlyList<double> data = Array.Empty<double>();
    private double average, min, max, median;
    private int minPos, maxPos, medianPos;
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
        DrawText($"Median: {median:F2}", PADDING, Y + MED_Y, FONT_SIZE, SPECIAL_POINT_COLORS[SPECIAL_MED]);
        DrawText($"Minimum: {min:F2}", PADDING, Y + MIN_Y, FONT_SIZE, SPECIAL_POINT_COLORS[SPECIAL_MIN]);
        DrawText($"Maximum: {max:F2}", PADDING, Y + MAX_Y, FONT_SIZE, SPECIAL_POINT_COLORS[SPECIAL_MAX]);

        int graphPaddingX = PADDING * 2;
        int width = (Width - graphStartX - graphPaddingX) / 2;
        DrawLineChart(graphStartX, Y + GRAPH_Y, width);

        // TODO: draw bar chart
    }

    private void DrawLineChart(int startX, int startY, int width)
    {
        DrawAxes(startX, startY, width);

        DrawAverageLine(startX, startY, width);

        double dx = (width - GRAPH_X_PAD) / (data.Count - 1.0);

        Span<(int x, int y)> points = stackalloc (int x, int y)[3];
        points.Fill((-1, -1));

        int last = 0;
        for (int i = 0; i < data.Count; i++)
        {
            double temp = data[i];
            temp *= (GRAPH_HEIGHT - GRAPH_Y_PAD) / (max - min);
            temp -= min;
            temp += GRAPH_Y_PAD / 2;
            temp = startY + GRAPH_HEIGHT - temp;
            int y = (int)temp;

            int x = (int)(startX + (i * dx) + (GRAPH_X_PAD / 2));

            if (i == minPos)
            {
                points[SPECIAL_MIN] = (x, y);
            }
            if (i == maxPos)
            {
                points[SPECIAL_MAX] = (x, y);
            }
            if (i == medianPos)
            {
                points[SPECIAL_MED] = (x, y);
            }

            if (i != 0)
            {
                DrawLine((int)(x - dx), last, x, y, Color.GREEN);
            }

            last = y;
        }

        for (int i = 0; i < points.Length; i++)
        {
            (int x, int y) = points[i];
            if (x > 0 && y > 0)
            {
                DrawCircle(x, y, SPECIAL_POINT_SIZE, SPECIAL_POINT_COLORS[i]);
            }
        }
    }

    private static void DrawAxes(int startX, int startY, int width)
    {
        DrawLine(startX, startY, startX, startY + GRAPH_HEIGHT + 1, Color.GRAY);
        DrawLine(startX, startY + GRAPH_HEIGHT, startX + width, startY + GRAPH_HEIGHT, Color.GRAY);
    }

    private void DrawAverageLine(int startX, int startY, int width)
    {
        double temp = average;
        temp *= (GRAPH_HEIGHT - GRAPH_Y_PAD) / (max - min);
        temp -= min;
        temp += GRAPH_Y_PAD / 2;
        temp = startY + GRAPH_HEIGHT - temp;
        int y = (int)temp;
        DrawLine(startX + (GRAPH_X_PAD / 2), y, startX + width - (GRAPH_X_PAD / 2), y, Color.GRAY);
    }
}
