namespace TNTSim.Cannon;

internal static class BreadboardFollower
{
    // Caching follow breadboard calculations because they're expensive
    private static readonly Dictionary<Breadboard, int[]> FOLLOW_CACHE = new();

    /// <summary>
    /// Gets the index of the next charge after following the breadboard's pattern
    /// </summary>
    public static int FollowBreadboard(int startCharge, Breadboard breadboard)
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
        List<(int s, int c)> seen = new();
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
                ends.Add(c + dc);
            }
        }

        while (open.TryDequeue(out var t))
        {
            (int s, int c) = t;
            if (breadboard[s, c] == Connection.IN_AND_OUT)
            {
                ends.Add(c);
            }
            else
            {
                AddNeighboors(s, c, 1, 0, Connection.NEXT_SLICE, Connection.NEXT_SLICE_OUT);
                AddNeighboors(s, c, -1, 0, Connection.PREV_SLICE, Connection.PREV_SLICE_OUT);
                AddNeighboors(s, c, 0, 1, Connection.NEXT_CHARGE, Connection.NEXT_CHARGE_OUT);
                AddNeighboors(s, c, 0, -1, Connection.PREV_CHARGE, Connection.PREV_CHARGE_OUT);
            }
            seen.Add((startSlice, startCharge));
        }

        return ends.Count switch
        {
            1 => ends.First(),
            0 => -1,
            _ => throw new ArgumentException("Multiple outputs detected")
        };
    }
}
