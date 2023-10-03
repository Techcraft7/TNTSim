namespace TNTSim.Components;

internal abstract class Component
{
	public Color PrimaryColor { get; set; } = Color.GRAY;
	public int X { get; init; }
	public int Y { get; init; }
	public int Width { get; init; }
	public int Height { get; init; }
	protected Color SeconaryColor => new()
	{
		r = (byte)Math.Min(PrimaryColor.r + 70, 255),
		g = (byte)Math.Min(PrimaryColor.g + 70, 255),
		b = (byte)Math.Min(PrimaryColor.b + 70, 255),
		a = PrimaryColor.a
	};

	public Component(int x, int y, int w, int h)
	{
		X = x;
		Y = y;
		Width = w;
		Height = h;
	}

	public abstract void UpdateAndDraw();

	protected void DrawOutline(bool addHover = false)
	{
		DrawRectangleLines(X, Y, Width, Height, PrimaryColor);
		if (addHover && IsMouseOverSelf())
		{
			DrawRectangle(X + 1, Y + 1, Width - 2, Height - 2, SeconaryColor);
		}
	}

	protected bool IsMouseOverSelf()
	{
		int mx = GetMouseX() - X;
		int my = GetMouseY() - Y;
		return mx >= 0 && mx <= Width && my >= 0 && my <= Height;
	}

	public int GetRightSide() => X + Width;
	public int GetBottomSide() => Y + Height;
}