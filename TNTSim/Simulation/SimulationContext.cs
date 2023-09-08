﻿namespace TNTSim.Simulation;

internal sealed class SimulationContext
{
	public bool HasTNT => tnt.Count > 0;
	public SimulationSettings Settings { get; init; }

	private readonly IList<TNT> tnt;
	private readonly List<TNT> toRemove = new();

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
			func(ref item);
			tnt[i] = item;
		}
		toRemove.ForEach(e => tnt.Remove(e));
		toRemove.Clear();
	}
}

internal delegate void TNTModifier(ref TNT other);