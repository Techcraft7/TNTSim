namespace TNTSim.Screens;

internal enum Screen
{
    CHARGES = 0,
    BREADBOARDS = 1,
    SIMULATION = 2
}

internal static class ScreenExt
{
    public static Screen Next(this Screen s) => s switch
    {
        Screen.CHARGES => Screen.BREADBOARDS,
        Screen.BREADBOARDS => Screen.SIMULATION,
        Screen.SIMULATION => Screen.CHARGES,
        _ => Screen.CHARGES
    };
}