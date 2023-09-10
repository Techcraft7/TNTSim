namespace TNTSim.Simulation;

internal sealed class SimulationContext
{
	public ICollection<TNT> TNT => tnt;
	public ICollection<Vec3> Explosions => explosions;
	public bool HasTNT => tnt.Count > 0;
	public SimulationSettings Settings { get; init; }

	private readonly IList<TNT> tnt;
	private readonly List<TNT> toRemove = new();
	private readonly List<Vec3> explosions = new();

	public SimulationContext(SimulationSettings settings, IList<TNT> tnt)
	{
		Settings = settings;
		this.tnt = tnt;
	}

	public void Remove(TNT entity) => toRemove.Add(entity);

	public void ModifyEntities(TNTModifier func)
	{
		for (int i = 0; i < tnt.Count; i++)
		{
			TNT item = tnt[i];
            if (item.Removed)
            {
                continue;
            }
            func(ref item);
            if (!item.Removed)
            {
                tnt[i] = item;
            }
        }
        RemoveExploded();
    }

    public void LogExplosion(Vec3 center)
    {
		// We only care about the explosions that hit the ground
		if (center.Y < 10)
		{
            explosions.Add(center);
        }
    }

    public void Tick() => ModifyEntities(TickTNT);

    private void RemoveExploded()
    {
        if (toRemove.Count > 0)
        {
            toRemove.ForEach(e => tnt.Remove(e));
            toRemove.Clear();
        }
    }

    // Adding method to prevent closure
    private void TickTNT(ref TNT tnt) => tnt.Tick(this);
}

internal delegate void TNTModifier(ref TNT other);