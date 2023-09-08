namespace TNTSim.Simulation;

internal static class Simulator
{
    public static void Simulate(SimulationSettings settings)
    {
        List<TNT> list = CreatePayload(settings);

        SimulationContext context = new(list);

        TickOnceAndCap(context);

        Run(context);
    }

    private static void Run(SimulationContext context)
	{
        while (context.HasTNT)
        {
            context.ModifyEntities((ref TNT tnt) => tnt.Tick(context));
            // TODO: collect info
        }

        // TODO: return something idk
    }

    /// <summary>
    /// Spawns the TNT of the payload
    /// </summary>
    private static List<TNT> CreatePayload(SimulationSettings settings)
    {
        List<TNT> list = new();

        // Make changes on this, so we can keep the original
        CannonSettings copy = settings.cannonSettings.Clone();

        int index = 0;
        while (index is >= 0 and < 5)
        {
            Charge charge = copy.GetCharge(index);
            for (int i = 0; i < charge.tntCount; i++)
            {
                double vx = charge.cancelX ? 0 : 0.2 * Math.Sin(Random.Shared.NextDouble() * Math.Tau);
                double vz = charge.cancelZ ? 0 : 0.2 * Math.Sin(Random.Shared.NextDouble() * Math.Tau);

				list.Add(new()
                {
                    // Random momentum and fuse timer are mutually exclusive
                    fuse = 80 - (charge.cancelX && charge.cancelZ ? charge.fuse : 1) + 1,
                    velocity = new(vx, 0, vz),
                    position = new(0, settings.payloadY, 0)
                });
            }

            charge.scheduleCount--;
            copy.SetCharge(index, charge);
            if (charge.scheduleCount > 0)
            {
                index = BreadboardFollower.FollowBreadboard(index, settings.cannonSettings.schedulingBoard);
            }
            else
            {
                // Reset for next use
                copy.SetCharge(index, settings.cannonSettings.GetCharge(index));
                index = BreadboardFollower.FollowBreadboard(index, settings.cannonSettings.continuationBoard);
            }
        }

        return list;
    }

    /// <summary>
    /// Simulates the teleportation of the payload. Steps 1 tick, then caps velocity to 10m/s (component-wise)
    /// </summary>
    private static void TickOnceAndCap(SimulationContext context)
	{
        context.ModifyEntities((ref TNT tnt) =>
        {
            tnt.Tick(context);

            tnt.velocity.X = Math.Clamp(tnt.velocity.X, -10, 10);
            tnt.velocity.Y = Math.Clamp(tnt.velocity.Y, -10, 10);
            tnt.velocity.Z = Math.Clamp(tnt.velocity.Z, -10, 10);
        });
    }
}
