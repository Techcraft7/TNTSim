namespace TNTSim.Simulation;

internal static class Simulator
{
    public static void Simulate(SimulationSettings settings)
    {
        List<TNT> list = new();

        CreatePayload(settings, list);

        SimulationContext context = new(list);

        TickOnceAndCap(list, context);

        Run(list, context);
    }

    private static void Run(List<TNT> list, SimulationContext context)
    {
        while (list.Any())
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                TNT tnt = list[i];
                tnt.Tick(context);
                if (i < list.Count && tnt.id == list[i].id)
                {
                    list[i] = tnt;
                }
            }
            // TODO: collect info
        }

        // TODO: return something idk
    }

    /// <summary>
    /// Spawns the TNT of the payload
    /// </summary>
    private static void CreatePayload(SimulationSettings settings, List<TNT> list)
    {
        // Make changes on this, so we can keep the original
        CannonSettings copy = settings.cannonSettings.Clone();

        int index = 0;
        while (index is >= 0 and < 5)
        {
            Charge charge = copy.GetCharge(index);
            for (int i = 0; i < charge.tntCount; i++)
            {
                list.Add(new()
                {
                    // Random momentum and fuse timer are mutually exclusive
                    fuse = 80 - (charge.cancelX && charge.cancelZ ? charge.fuse : 1) + 1,
                    velX = charge.cancelX ? 0 : 0.2 * Math.Sin(Random.Shared.NextDouble() * Math.Tau),
                    velZ = charge.cancelZ ? 0 : 0.2 * Math.Sin(Random.Shared.NextDouble() * Math.Tau),
                    y = settings.payloadY
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
    }

    /// <summary>
    /// Simulates the teleportation of the payload. Steps 1 tick, then caps velocity to 10m/s (component-wise)
    /// </summary>
    private static void TickOnceAndCap(List<TNT> list, SimulationContext context)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            TNT tnt = list[i];
            tnt.Tick(context);
            list[i] = tnt with
            {
                velX = Math.Clamp(tnt.velX, -10, 10),
                velY = Math.Clamp(tnt.velY, -10, 10),
                velZ = Math.Clamp(tnt.velZ, -10, 10)
            };
        }
    }
}
