namespace TNTSim.Data;

internal struct Breadboard
{
    // Store 9x6 3-bit values (162 bits)
    // Rows = Charge # (row 6 is after charge 5)
    // Columns = One rail (9 because power goes out 8 from the powered rail)
    private long data0, data1, data2;

    public Connection this[int slice, int charge]
    {
        readonly get
        {
            int i = 4 * (charge + 6 * slice);
            int j = i % 64;
            return (Connection)((i / 64) switch
            {
                0 => (data0 & 0b1111 << j) >> j & 0b1111,
                1 => (data1 & 0b1111 << j) >> j & 0b1111,
                2 => (data2 & 0b1111 << j) >> j & 0b1111,
                _ => 0
            });
        }

        set
        {
            int i = 4 * (charge + 6 * slice);
            int j = i % 64;
            long mask = ~(0b1111 << j);
            long data = (byte)value << j;
            _ = (i / 64) switch
            {
                0 => data0 = data0 & mask | data,
                1 => data1 = data1 & mask | data,
                2 => data2 = data2 & mask | data,
                _ => 0
            };
        }
    }

    public enum Connection : byte
    {
        NONE = 0,
        INPUT = 1,
        UP = 2,
        RIGHT = 3,
        DOWN = 4,
        LEFT = 5,
        UP_OUT = 6,
        RIGHT_OUT = 7,
        DOWN_OUT = 8,
        LEFT_OUT = 9,
    }
}
