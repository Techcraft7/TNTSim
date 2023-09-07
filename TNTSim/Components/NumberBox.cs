namespace TNTSim.Components;

internal sealed class NumberBox : Component
{
	public int Value
	{
		get => value;
		set
		{
			int old = this.value;
			this.value = Math.Clamp(value, min, max);
			if (old != this.value)
			{
				valueText = value.ToString();
			}
		}
	}

	private readonly DragHandler handler = new();
	private readonly int min;
	private readonly int max;
	private int value;
	private string valueText;
	private int valueOnDragStart;

	public NumberBox(int min, int max, int x, int y, int w, int? initial = null) : base(x, y, w, CONTROL_HEIGHT)
	{
		this.min = min;
		this.max = max;
		value = initial ?? min;
		valueText = value.ToString();
	}

	public override void UpdateAndDraw()
	{
		handler.Update(IsMouseOverSelf);

		if (handler.IsDragging)
		{
			int delta = -handler.DeltaY / 10;

			int old = value;
			value = Math.Clamp(valueOnDragStart + delta, min, max);
			if (old != value)
			{
				valueText = value.ToString();
			}
		}
		else
		{
			valueOnDragStart = value;
		}

		DrawOutline(true);
		DrawText(valueText, X + PADDING, Y + PADDING, FONT_SIZE, PrimaryColor);
	}

	public static int GetMinWidth(int min, int max) => (2 * PADDING) + Math.Max(MeasureText(min.ToString(), FONT_SIZE), MeasureText(max.ToString(), FONT_SIZE));
}
