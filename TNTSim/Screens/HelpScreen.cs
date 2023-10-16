using System.Text;

namespace TNTSim.Screens;

internal static class HelpScreen
{
	private const string TITLE = "Help";
	private const int START_Y = CONTROL_HEIGHT + (PADDING * 10);
	private const int START_X = 50;
	private const int MAX_WIDTH = WINDOW_WIDTH - (2 * START_X);
	private static readonly string TEXT = string.Join("\n", new string[]
	{
		"Welcome to TNTSim, the high-performance Minecraft TNT physics simulation designed for cubicmeter's Orbital Strike Cannon v2",
		"",
		"-- Charge Menu --",
		"TODO",
		"-- Breadboard Menu --",
		"TODO"
	});
	private static readonly string[] PARTS;
	private static readonly int TITLE_X = (WINDOW_WIDTH - MeasureText(TITLE, FONT_SIZE)) / 2;


	static HelpScreen()
	{
		List<string> parts = new();
		StringBuilder sb = new();

		foreach (string line in TEXT.Split('\n'))
		{
			foreach (string word in line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
			{
				string str = sb.ToString();
				int w = MeasureText(str, FONT_SIZE) + MeasureText(word, FONT_SIZE);
				if (w > MAX_WIDTH)
				{
					parts.Add(str);
					sb.Clear();
				}
				sb.Append(word).Append(' ');
			}
			parts.Add(sb.ToString());
			sb.Clear();
		}

		if (sb.Length > 0)
		{
			parts.Add(sb.ToString());
			sb.Clear();
		}

		PARTS = parts.ToArray();
	}

	public static void UpdateAndDraw()
	{
		DrawText(TITLE, TITLE_X, 2 * PADDING, FONT_SIZE, Color.GRAY);
		DrawLine(0, CONTROL_HEIGHT + PADDING, WINDOW_WIDTH, CONTROL_HEIGHT + PADDING, Color.GRAY);

		for (int i = 0; i < PARTS.Length; i++)
		{
			DrawText(PARTS[i], START_X, START_Y + (i * (FONT_SIZE + PADDING)), FONT_SIZE, Color.GRAY);
		}
	}
}
