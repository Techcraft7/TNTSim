namespace TNTSim;

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
			int i = 3 * (charge + (6 * slice));
			int j = i % 63;
			return (Connection)((i / 63) switch
			{
				0 => ((data0 & (0b111 << j)) >> j) & 0b111,
				1 => ((data1 & (0b111 << j)) >> j) & 0b111,
				2 => ((data2 & (0b111 << j)) >> j) & 0b111,
				_ => 0
			});
		}

		set
		{
			int i = 3 * (charge + (6 * slice));
			int j = i % 63;
			long mask = ~(0b111 << j);
			long data = (byte)value << j;
			_ = (i / 63) switch
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
		OUTPUT = 6
	}
}
