namespace TNTSim.Simulation;

internal sealed class SimulationContext
{
	public ICollection<TNT> TNT => tnt;
	public ICollection<Vec3> Explosions => explosions;
	public bool HasTNT => tnt.Count > 0;
	public SimulationSettings Settings { get; init; }

	private readonly List<TNT> tnt;
	private readonly List<TNT> toRemove = new();
	private readonly List<Vec3> explosions = new();

	public SimulationContext(SimulationSettings settings, List<TNT> tnt)
	{
		Settings = settings;
		this.tnt = tnt;
	}

	public void Remove(TNT entity) => toRemove.Add(entity);

	public void ModifyEntities(TNTModifier func, bool async = false)
	{
        if (async)
        {
            Parallel.For(0, tnt.Count, i =>
            {
                TNT item = tnt[i];
                if (!item.Removed)
                {
                    func(ref item);
                    tnt[i] = item;
                }
            });
        }
        else
        {
            for (int i = 0; i < tnt.Count; i++)
            {
                TNT item = tnt[i];
                if (!item.Removed)
                {
                    func(ref item);
                    tnt[i] = item;
                }
            }
        }
    }

    public void LogExplosion(Vec3 center)
    {
		// We only care about the explosions that hit the ground
		if (center.Y <= EXPLOSION_SIZE)
		{
            explosions.Add(center);
        }
    }

    public void Tick(bool isFirst = false)
    {
        if (tnt.Count == 0)
        {
            if (tnt.Capacity > 0)
            {
                tnt.Capacity = 0;
            }
            return;
        }
        ModifyEntities(isFirst ? TickTNTFirstTime : TickTNT);
        RemoveExploded();
    }

    private void RemoveExploded()
    {
        if (toRemove.Count > 0)
        {
            toRemove.ForEach(e => tnt.Remove(e));
            toRemove.Clear();
            toRemove.Capacity = 0;
        }
    }

    // Adding method to prevent closure
    private void TickTNT(ref TNT tnt) => tnt.Tick(this);
    private void TickTNTFirstTime(ref TNT tnt) => tnt.Tick(this, true);
}

internal delegate void TNTModifier(ref TNT other);