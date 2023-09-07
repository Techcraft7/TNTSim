using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TNTSim.Components;

internal sealed class NumberBox : Component
{
    public int Value
    {
        get => value;
        private set
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

    public NumberBox(int min, int max, int x, int y, int w) : base(x, y, w, CONTROL_HEIGHT)
    {
        this.min = min;
        this.max = max;
        value = min;
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
}
