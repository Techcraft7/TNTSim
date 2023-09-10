﻿namespace TNTSim.Simulation;

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
    /// Simulates the teleportation of the payload. Steps 1 tick, then cancels velocity greater than 10m/s (component-wise)
    /// </summary>
    private static void TickOnceAndCap(SimulationContext context)
	{
        context.ModifyEntities((ref TNT tnt) =>
        {
            tnt.Tick(context);

            tnt.velocity.X = Math.Abs(tnt.velocity.X) >= 10 ? 0 : tnt.velocity.X;
            tnt.velocity.Y = Math.Abs(tnt.velocity.Y) >= 10 ? 0 : tnt.velocity.Y;
            tnt.velocity.Z = Math.Abs(tnt.velocity.Z) >= 10 ? 0 : tnt.velocity.Z;
        });
    }
}
