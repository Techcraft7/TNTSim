namespace TNTSim.Components;

internal sealed class ChargeEditor : ComponentGroup
{
	public static readonly int WIDTH = new ComponentGroup(0, 0, GetComponents(0, 0)).Width;
	public static readonly int HEIGHT = new ComponentGroup(0, 0, GetComponents(0, 0)).Height;

	public Charge Charge => charge;
	private Charge charge = new();

	public ChargeEditor(int x, int y) : base(x, y, GetComponents(x, y))
	{

	}

	public override void UpdateAndDraw()
	{
		base.UpdateAndDraw();
		charge.tntCount = GetComponent<NumberBox>(1).Value;
		charge.scheduleCount = GetComponent<NumberBox>(3).Value;
		charge.fuse = GetComponent<NumberBox>(5).Value;

		charge.cancelX = GetComponent<CheckBox>(7).Value;
		charge.cancelZ = GetComponent<CheckBox>(8).Value;
		charge.single = GetComponent<CheckBox>(9).Value;
	}

	private static Component[] GetComponents(int x, int y) => new ComponentGroupBuilder()
		.AddText("TNT Count")
		.AddNumberBox(0, DROPPER_COUNT)
		.AddText("Schedule Count")
		.AddNumberBox(1, DROPPER_COUNT)
		.AddText("Fuse Timer")
		.AddNumberBox(1, 80)
		.EndRow()
		.AddText("TNT Settings")
		.AddCheckBox("Cancel X")
		.AddCheckBox("Cancel Z")
		.AddCheckBox("Single")
		.ToComponentArray(x, y);
}
