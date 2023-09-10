namespace TNTSim.Simulation;

internal static class Simulator
{
    public static SimulationContext Create(SimulationSettings settings)
    {
        List<TNT> payload = CreatePayload(settings);

        SimulationContext context = new(settings, payload);

        TickOnceAndCap(context);

        return context;
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
            charge.tntCount *= charge.single ? 1 : 8;
            for (int i = 0; i < charge.tntCount; i++)
            {
                double theta = Random.Shared.NextDouble() * Math.Tau;
                double vx = charge.cancelX ? 0 : 0.2 * Math.Sin(theta);
                double vz = charge.cancelZ ? 0 : 0.2 * Math.Cos(theta);

                TNT tnt = new()
                {
                    // Random momentum and fuse timer are mutually exclusive
                    fuse = 80 - (charge.cancelX && charge.cancelZ ? charge.fuse : 1),
                    position = new(0, settings.payloadY, 0)
                };

                if (!charge.cancelX)
                {
                    tnt.velocity += new Vec3(vx, 0, 0);
                }
                if (!charge.cancelZ)
                {
                    tnt.velocity += new Vec3(0, 0, vz);
                }

				list.Add(tnt);
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
    /// Simulates the teleportation of the payload. Steps 1 tick, then cancels velocity greater than 10m/s (component-wise)
    /// </summary>
    private static void TickOnceAndCap(SimulationContext context)
	{
        context.Tick();
        context.ModifyEntities((ref TNT tnt) =>
        {
            if (Math.Abs(tnt.velocity.X) > 10)
            {
                tnt.velocity.X = 0;
            }
            if (Math.Abs(tnt.velocity.Y) > 10)
            {
                tnt.velocity.Y = 0;
            }
            if (Math.Abs(tnt.velocity.Z) > 10)
            {
                tnt.velocity.Z = 0;
            }
        });
    }
}
