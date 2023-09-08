namespace TNTSim.Simulation;

internal static class Simulator
{
    public static void Simulate(SimulationSettings settings)
    {
        List<TNT> list = new();

        CreatePayload(settings, list);

        SimulationContext context = new(list);

        TickOnceAndCap(list, context);

        // TODO: extract method
        // THEN run
        while (list.Any())
        {
            foreach (TNT tnt in list)
            {
                tnt.Tick(context);
            }
            // TODO: do something with this idk
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
                });
            }

            charge.scheduleCount--;
            if (charge.scheduleCount > 0)
            {
                index = FollowBreadboard(index, settings.cannonSettings.schedulingBoard);
            }
            else
            {
                // Reset for next use
                copy.SetCharge(index, settings.cannonSettings.GetCharge(index));
                index = FollowBreadboard(index, settings.cannonSettings.continuationBoard);
            }
        }
    }

    /// <summary>
    /// Simulates the teleportation of the payload. Steps 1 tick, then caps velocity to 10m/s (component-wise)
    /// </summary>
    private static void TickOnceAndCap(List<TNT> list, SimulationContext context)
    {
        foreach (TNT tnt in list)
        {
            tnt.Tick(context);
        }
        for (int i = 0; i < list.Count; i++)
        {
            TNT tnt = list[i];
            list[i] = tnt with
            {
                velX = Math.Clamp(tnt.velX, -10, 10),
                velY = Math.Clamp(tnt.velY, -10, 10),
                velZ = Math.Clamp(tnt.velZ, -10, 10)
            };
        }
    }

    // Caching follow breadboard calculations because they're expensive
    private static readonly Dictionary<Breadboard, int[]> FOLLOW_CACHE = new();

    /// <summary>
    /// Gets the index of the next charge after following the breadboard's pattern
    /// </summary>
    private static int FollowBreadboard(int startCharge, Breadboard breadboard)
    {
        if (FOLLOW_CACHE.TryGetValue(breadboard, out int[]? arr))
        {
            if (arr[startCharge] is >= 0 and < 6)
            {
                return arr[startCharge];
            }
        }

        for (int startSlice = 0; startSlice < 9; startSlice++)
        {
            if (breadboard[startSlice, startCharge] == Connection.INPUT)
            {
                int r = FollowFrom(startCharge, startSlice, breadboard);
                if (!FOLLOW_CACHE.ContainsKey(breadboard))
                {
                    FOLLOW_CACHE[breadboard] = new int[6] { -1, -1, -1, -1, -1, -1 };
                }
                FOLLOW_CACHE[breadboard][startCharge] = r;
                return r;
            }
        }
        return -1;
    }

    private static int FollowFrom(int startCharge, int startSlice, Breadboard breadboard)
    {
        List<(int s, int c)> seen = new()
        {
            (startSlice, startCharge)
        };
        Queue<(int s, int c)> open = new();
        HashSet<int> ends = new();
        open.Enqueue((startSlice, startCharge));

        void AddNeighboors(int s, int c, int ds, int dc, Connection normal, Connection end)
        {
            if (s is < 0 or >= 9 || c is < 0 or >= 6 || s + ds is < 0 or >= 9 || c + dc is < 0 or >= 6)
            {
                return;
            }
            if (seen.Any(t => t.s == s && t.c == c))
            {
                return;
            }

            (int s, int c) t = (s + ds, c + dc);
            if (breadboard[s + ds, c + dc] == normal)
            {
                seen.Add(t);
                open.Enqueue(t);
            }
            if (breadboard[s + ds, c + dc] == end)
            {
                seen.Add(t);
                ends.Add(c);
            }
        }

        while (open.TryDequeue(out var t))
        {
            (int s, int c) = t;
            AddNeighboors(s, c, 1, 0, Connection.NEXT_SLICE, Connection.NEXT_SLICE_OUT);
            AddNeighboors(s, c, -1, 0, Connection.PREV_SLICE, Connection.PREV_SLICE_OUT);
            AddNeighboors(s, c, 0, 1, Connection.NEXT_CHARGE, Connection.NEXT_CHARGE_OUT);
            AddNeighboors(s, c, 0, -1, Connection.PREV_CHARGE, Connection.PREV_CHARGE_OUT);
        }

        return ends.Count switch
        {
            1 => ends.First(),
            0 => -1,
            _ => throw new ArgumentException("Multiple outputs detected")
        };
    }
}
