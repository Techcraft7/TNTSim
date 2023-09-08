namespace TNTSim.Simulation;

internal sealed record SimulationContext(IReadOnlyList<TNT> Entities)
{
	private readonly IList<TNT> tnt = (Entities as IList<TNT>)!;

	public void Remove(TNT entity) => tnt.Remove(entity);

	public void ModifyEntities(TNTModifier func)
	{
		for (int i = tnt.Count - 1; i >= 0; i--)
		{
			TNT item = tnt[i];
			func(ref item);
			tnt[i] = item;
		}
	}
}

internal delegate void TNTModifier(ref TNT other);