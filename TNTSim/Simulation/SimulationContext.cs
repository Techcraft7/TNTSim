﻿namespace TNTSim.Simulation;

internal sealed class SimulationContext
{
	public IReadOnlyCollection<TNT> TNT => tnt;
	public ICollection<Vec3> Explosions => explosions;
	public SimulationSettings Settings { get; init; }

	private readonly SpatialTNTList tnt;
	private readonly List<Vec3> explosions = new();

	public SimulationContext(SimulationSettings settings, List<TNT> tnt)
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

	// Adding method to prevent closure
	private void TickTNT(ref TNT tnt) => tnt.Tick(this);
	private void TickTNTFirstTime(ref TNT tnt) => tnt.Tick(this, true);
}

internal delegate void TNTModifier(ref TNT other);