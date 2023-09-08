namespace TNTSim.Components;

internal sealed class BreadboardEditor : Component
{
    public Breadboard Breadboard => breadboard;
    private Breadboard breadboard = default;
    private readonly int buttonSize;
    private readonly ScrollHandler scroller = new(1f);

    public BreadboardEditor(int x, int y, int w, int h) : base(x, y, w, h)
    {
        buttonSize = Math.Min(w / 9, h / 6);
    }

    public override void UpdateAndDraw()
    {
        DrawRectangle(X, Y + ((buttonSize - 1) * 5), (buttonSize - 1) * 9, buttonSize, Color.YELLOW);

        bool hasHovered = false;
        for (int i = 0; i < 54; i++)
        {
            int charge = i % 6;
            int slice = i / 6;
            int x = X + (slice * (buttonSize - 1));
            int y = Y + (charge * (buttonSize - 1));

            if (slice == 0)
            {
                string str = charge == 5 ? "Fire" : $"#{charge + 1}";
                int ox = buttonSize - MeasureText(str, FONT_SIZE) - PADDING;
                int oy = (buttonSize - FONT_SIZE) / 2;
                DrawText(str, x - buttonSize + ox, y + oy, FONT_SIZE, PrimaryColor);
            }

            DrawRectangleLines(x, y, buttonSize, buttonSize, PrimaryColor);

            int mx = GetMouseX() - x;
            int my = GetMouseY() - y;
            if (!hasHovered && mx >= 0 && mx < buttonSize && my > 0 && my < buttonSize)
            {
                hasHovered = true;
                DrawRectangle(x + 1, y + 1, buttonSize - 2, buttonSize - 2, SeconaryColor);
                OnCellHovered(charge, slice);
            }

            DrawConnection(breadboard[slice, charge], x, y);
        }
    }

    private void OnCellHovered(int charge, int slice)
    {
        if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
        {
            breadboard[slice, charge] = breadboard[slice, charge].Next();
        }

        scroller.Update();
        if (scroller.HasScrolledOneTick(out int sign))
        {
            breadboard[slice, charge] = sign > 0 ? breadboard[slice, charge].Next() : breadboard[slice, charge].Previous();
        }

        if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT))
        {
            breadboard[slice, charge] = Connection.NONE;
        }
    }

    private void DrawConnection(Connection c, int x, int y)
    {
        int r = (int)((buttonSize - 1) / 2 * 0.75);
        int diagonal = (int)(r / Math.Sqrt(2));
        int centerX = x + ((buttonSize - 1) / 2);
        int centerY = y + ((buttonSize - 1) / 2);
        switch (c)
        {
            case Connection.INPUT:
                DrawCircleLines(centerX, centerY, r, Color.RED);
                break;
            case Connection.NEXT_CHARGE:
                DrawLine(centerX, centerY, centerX, y, Color.RED);
                break;
            case Connection.NEXT_SLICE:
                DrawLine(centerX, centerY, x + buttonSize, centerY, Color.RED);
                break;
            case Connection.PREV_CHARGE:
                DrawLine(centerX, centerY, centerX, y + buttonSize, Color.RED);
                break;
            case Connection.PREV_SLICE:
                DrawLine(centerX, centerY, x, centerY, Color.RED);
                break;
            case Connection.NEXT_CHARGE_OUT:
                DrawLine(centerX - diagonal, centerY - diagonal, centerX + diagonal, centerY + diagonal, Color.RED);
                DrawLine(centerX - diagonal, centerY + diagonal, centerX + diagonal, centerY - diagonal, Color.RED);
                DrawConnection(Connection.NEXT_CHARGE, x, y);
                break;
            case Connection.NEXT_SLICE_OUT:
                DrawLine(centerX - diagonal, centerY - diagonal, centerX + diagonal, centerY + diagonal, Color.RED);
                DrawLine(centerX - diagonal, centerY + diagonal, centerX + diagonal, centerY - diagonal, Color.RED);
                DrawConnection(Connection.NEXT_SLICE, x, y);
                break;
            case Connection.PREV_CHARGE_OUT:
                DrawLine(centerX - diagonal, centerY - diagonal, centerX + diagonal, centerY + diagonal, Color.RED);
                DrawLine(centerX - diagonal, centerY + diagonal, centerX + diagonal, centerY - diagonal, Color.RED);
                DrawConnection(Connection.PREV_CHARGE, x, y);
                break;
            case Connection.PREV_SLICE_OUT:
                DrawLine(centerX - diagonal, centerY - diagonal, centerX + diagonal, centerY + diagonal, Color.RED);
                DrawLine(centerX - diagonal, centerY + diagonal, centerX + diagonal, centerY - diagonal, Color.RED);
                DrawConnection(Connection.PREV_SLICE, x, y);
                break;
        }
    }

    public void Load(in Breadboard b) => breadboard = b.Clone();
}
