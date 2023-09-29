namespace TNTSim.Components;
internal sealed class Analysis : Component
{
    public const int HEIGHT = 200;
    public const int TEXT_Y = PADDING;

    public IReadOnlyList<double> Data { get; set; } = Array.Empty<double>();

    private readonly string name;
    private readonly int textX;

    public Analysis(string name, int y) : base(PADDING, y, WINDOW_WIDTH - (PADDING * 2), HEIGHT)
    {
        this.name = name;
        textX = X + ((Width - MeasureText(name, FONT_SIZE)) / 2);
    }

    public override void UpdateAndDraw()
    {
        DrawText(name, textX, Y + TEXT_Y, FONT_SIZE, PrimaryColor);
    }
}
