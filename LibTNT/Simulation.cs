namespace LibTNT;

public sealed class Simulation
{
	public IReadOnlyCollection<TNT> TNT => tnt;
	public ICollection<Vec3> Explosions => explosions;
	public SimulationSettings Settings { get; init; }

	private readonly SpatialTNTList tnt;
	private readonly List<Vec3> explosions = new();

	public Simulation(SimulationSettings settings, List<TNT> tnt)
	{
		Settings = settings;
		this.tnt = new(tnt);
	}

	public void ModifyEntitiesInBucket(Vec3B bucket, TNTModifier func) => tnt.ModifyInBucket(bucket, func);
	public void MoveToSpatialBucket(TNT tnt, Vec3B to) => this.tnt.MoveTo(tnt, to);


	public void ModifyEntitiesInOrder(TNTModifier func) => tnt.ModifyInOrder(func);

	public void LogExplosion(Vec3 center)
	{
		if (center.Y < 10)
		{
			explosions.Add(center);
		}
	}

	public void Tick(bool isFirst = false)
	{
		ModifyEntitiesInOrder(isFirst ? TickTNTFirstTime : TickTNT);
		tnt.RemoveExploded();
	}

	private void TickTNT(TNT tnt) => tnt.Tick();
	private void TickTNTFirstTime(TNT tnt) => tnt.Tick(true);
}

public delegate void TNTModifier(TNT other);