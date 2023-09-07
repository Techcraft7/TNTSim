using System.Reflection.Metadata;

namespace TNTSim.Util;

internal sealed class ScrollHandler
{
    private readonly float amountPerTick;
    private float amount = 0;
    private int scrolled = 0;

    public ScrollHandler(float amountPerTick)
    {
        this.amountPerTick = amountPerTick;
    }

    public void Update()
    {
        amount += GetMouseWheelMoveV().Y;

        if (Math.Abs(amount) >= amountPerTick)
        {
            scrolled = amount < 0 ? -1 : 1;
            amount = 0;
        }
    }

    public bool HasScrolledOneTick(out int sign)
    {
        sign = scrolled;
        scrolled = 0;
        return sign != 0;
    }
}
