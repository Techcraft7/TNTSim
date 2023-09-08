namespace TNTSim.Cannon;

internal struct Breadboard : ICloneable<Breadboard>
{
    // Store 9x6 3-bit values (162 bits)
    // Rows = Charge # (row 6 is after charge 5)
    // Columns = One rail (9 because power goes out 8 from the powered rail)
    private long data0, data1, data2;

    public Connection this[int slice, int charge]
    {
        readonly get
        {
            int i = 4 * (charge + (6 * slice));
            int j = i % 64;
            return (Connection)((i / 64) switch
            {
                0 => ((data0 & (0b1111L << j)) >> j) & 0b1111L,
                1 => ((data1 & (0b1111L << j)) >> j) & 0b1111L,
                2 => ((data2 & (0b1111L << j)) >> j) & 0b1111L,
                _ => 0
            });
        }
        set
        {
            int i = 4 * (charge + (6 * slice));
            int j = i % 64;
            long mask = ~(0b1111L << j);
            long data = ((long)value) << j;
            _ = (i / 64) switch
            {
                0 => data0 = (data0 & mask) | data,
                1 => data1 = (data1 & mask) | data,
                2 => data2 = (data2 & mask) | data,
                _ => 0
            };
        }
    }

    public Breadboard Clone() => new()
    {
        data0 = data0,
        data1 = data1,
        data2 = data2,
    };


    public enum Connection : byte
    {
        NONE = 0,
        INPUT = 1,
        NEXT_CHARGE = 2,
        RIGHT = 3,
        PREV_CHARGE = 4,
        LEFT = 5,
        NEXT_OUT = 6,
        RIGHT_OUT = 7,
        PREV_OUT = 8,
        LEFT_OUT = 9,
    }
}

internal static class ConnectionExt
{
    public static Connection Next(this Connection c) => c switch
    {
        Connection.NONE => Connection.INPUT,
        Connection.INPUT => Connection.NEXT_CHARGE,
        Connection.NEXT_CHARGE => Connection.RIGHT,
        Connection.RIGHT => Connection.PREV_CHARGE,
        Connection.PREV_CHARGE => Connection.LEFT,
        Connection.LEFT => Connection.NEXT_OUT,
        Connection.NEXT_OUT => Connection.RIGHT_OUT,
        Connection.RIGHT_OUT => Connection.PREV_OUT,
        Connection.PREV_OUT => Connection.LEFT_OUT,
        Connection.LEFT_OUT => Connection.NONE,
        _ => Connection.NONE,
    };

    public static Connection Previous(this Connection c) => c switch
    {
        Connection.NONE => Connection.LEFT_OUT,
        Connection.INPUT => Connection.NONE,
        Connection.NEXT_CHARGE => Connection.INPUT,
        Connection.RIGHT => Connection.NEXT_CHARGE,
        Connection.PREV_CHARGE => Connection.RIGHT,
        Connection.LEFT => Connection.PREV_CHARGE,
        Connection.NEXT_OUT => Connection.LEFT,
        Connection.RIGHT_OUT => Connection.NEXT_OUT,
        Connection.PREV_OUT => Connection.RIGHT_OUT,
        Connection.LEFT_OUT => Connection.PREV_OUT,
        _ => Connection.NONE,
    };
}