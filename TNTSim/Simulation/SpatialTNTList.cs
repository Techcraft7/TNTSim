using System.Collections;
using System.Diagnostics;

namespace TNTSim.Simulation;

internal sealed class SpatialTNTList : IReadOnlyCollection<TNT>
{
    private readonly SortedDictionary<Vec3B, SortedList<uint, TNT>> groups = new();
    private readonly LinkedList<TNT> inOrder;

    public int Count => inOrder.Count;

    public SpatialTNTList(IEnumerable<TNT> tnt)
    {
        foreach (TNT x in tnt)
        {
            x.spatialBucket = Vec3B.FromPosition(x.position);
            if (groups.TryGetValue(x.spatialBucket, out var list))
            {
                list.Add(x.order, x);
            }
            else
            {
                groups[x.spatialBucket] = new()
                {
                    { x.order, x }
                };
            }
        }
        inOrder = new(tnt.OrderBy(static x => x.order));
    }

    public void MoveTo(TNT tnt, Vec3B to)
    {
        if (!groups.TryGetValue(tnt.spatialBucket, out var fromList))
        {
            return;
        }
        if (!fromList.Remove(tnt.order))
        {
            return;
        }
        if (fromList.Count == 0)
        {
            fromList.TrimExcess();
        }

        if (tnt.Removed)
        {
            return;
        }

        if (!groups.TryGetValue(to, out var toList))
        {
            groups[to] = new()
            {
                { tnt.order, tnt }
            };
            return;
        }
        toList.Add(tnt.order, tnt);
    }

	public void ModifyInBucket(Vec3B bucket, TNTModifier func)
	{
		if (!groups.TryGetValue(bucket, out var list))
        {
            return;
        }
        for (int i = 0; i < list.Count; i++)
        {
            TNT r = list.Values[i];
            if (!r.Removed)
            {
            func(ref r);
				list[r.order] = r;
        }
	}
	}

	public void ModifyInOrder(TNTModifier func)
    {
        LinkedListNode<TNT>? n = inOrder.First;
        while (n is not null)
        {
            TNT tnt = n.Value;
            func(ref tnt);
            n.Value = tnt;
            n = n.Next;
        }
    }

    public void RemoveExploded()
    {
        LinkedListNode<TNT>? n = inOrder.First;
        while (n is not null)
        {
            if (n.Value.Removed)
            {
                var next = n.Next;
                inOrder.Remove(n);
                if (groups.TryGetValue(n.Value.spatialBucket, out var list))
                {
                    Debug.Assert(list.Remove(n.Value.order));
                    if (list.Count == 0)
                    {
                        list.TrimExcess();
                    }
                }
                n = next;
            }
            else
            {
                n = n.Next;
            }
        }
    }

    public IEnumerator<TNT> GetEnumerator() => inOrder.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => inOrder.GetEnumerator();
}