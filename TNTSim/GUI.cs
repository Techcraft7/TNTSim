namespace TNTSim;

internal static class GUI
{
    private static readonly ChargeGUI CHARGE1 = new(4, 50);

    public static void UpdateAndDraw()
    {
        CHARGE1.UpdateAndDraw();
    }
}
