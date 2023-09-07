namespace TNTSim.Components;

internal sealed class ChargeGUI : Component
{
    private const int BOX_SIZE = 50;
    private const string TNT_TEXT = "TNT Count";
    private static readonly int TNT_TEXT_SIZE = MeasureText(TNT_TEXT, FONT_SIZE);
    private const string SCHEDULE_TEXT = "Schedule Count";
    private static readonly int SCHEDULE_TEXT_SIZE = MeasureText(SCHEDULE_TEXT, FONT_SIZE);
    private const string FUSE_TEXT = "Fuse";
    private static readonly int FUSE_TEXT_SIZE = MeasureText(FUSE_TEXT, FONT_SIZE);
    private static readonly int WIDTH = PADDING + 3 * (BOX_SIZE + PADDING + PADDING) + TNT_TEXT_SIZE + SCHEDULE_TEXT_SIZE + FUSE_TEXT_SIZE;

    public Charge Charge { get; private set; }

    private readonly int textY;
    private readonly NumberBox tntCountBox;
    private readonly NumberBox scheduleCountBox;
    private readonly NumberBox fuseBox;

    public ChargeGUI(int x, int y) : base(x, y, WIDTH, CONTROL_HEIGHT + 2 * PADDING)
    {
        int top = y + 4;
        tntCountBox = new(0, DROPPER_COUNT, x + PADDING + TNT_TEXT_SIZE + PADDING, top, BOX_SIZE);
        scheduleCountBox = new(1, DROPPER_COUNT, tntCountBox.GetRightSide() + PADDING + SCHEDULE_TEXT_SIZE + PADDING, top, BOX_SIZE);
        fuseBox = new(1, 80, scheduleCountBox.GetRightSide() + PADDING + FUSE_TEXT_SIZE + PADDING, top, BOX_SIZE);

        textY = Y + (Height - FONT_SIZE) / 2;
    }

    public override void UpdateAndDraw()
    {
        DrawOutline();

        DrawText(TNT_TEXT, X + PADDING, textY, FONT_SIZE, PrimaryColor);
        tntCountBox.UpdateAndDraw();
        Charge = Charge with { tntCount = tntCountBox.Value };

        DrawText(SCHEDULE_TEXT, X + tntCountBox.GetRightSide(), textY, FONT_SIZE, PrimaryColor);
        scheduleCountBox.UpdateAndDraw();
        Charge = Charge with { scheduleCount = scheduleCountBox.Value };

        DrawText(FUSE_TEXT, X + scheduleCountBox.GetRightSide(), textY, FONT_SIZE, PrimaryColor);
        fuseBox.UpdateAndDraw();
        Charge = Charge with { fuse = fuseBox.Value };
    }
}