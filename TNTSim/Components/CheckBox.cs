namespace TNTSim.Components;

internal sealed class CheckBox : Component
{
    private const int BOX_SIZE = CONTROL_HEIGHT - (2 * PADDING);
    private const int INNER_BOX_SIZE = BOX_SIZE - (2 * PADDING);
    private const int TEXT_X = PADDING + BOX_SIZE + PADDING;

	public bool Value { get; set; }
    private readonly string text;
    
    public CheckBox(string text, int x, int y, int w, bool initial = false) : base(x, y, w, CONTROL_HEIGHT)
    {
        this.text = text;
        Value = initial;
    }

    public override void UpdateAndDraw()
    {
        DrawOutline();
        DrawRectangleLines(X + PADDING, Y + PADDING, BOX_SIZE, BOX_SIZE, PrimaryColor);

		if (IsMouseOverSelf())
		{
			DrawRectangle(X + PADDING + 1, Y + PADDING + 1, BOX_SIZE - 2, BOX_SIZE - 2, SeconaryColor);
            if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
			{
				Value = !Value;
			}
		}

		if (Value)
        {
            DrawRectangle(X + (2 * PADDING), Y + (2 * PADDING), INNER_BOX_SIZE, INNER_BOX_SIZE, PrimaryColor);
        }

        DrawText(text, X + TEXT_X, Y + PADDING, FONT_SIZE, PrimaryColor);
    }

    public static int GetMinWidth(string text) => (2 * PADDING) + BOX_SIZE + PADDING + MeasureText(text, FONT_SIZE);
}
