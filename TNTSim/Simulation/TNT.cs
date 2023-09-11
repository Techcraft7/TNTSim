using System.Diagnostics.CodeAnalysis;

namespace TNTSim.Simulation;

internal struct TNT
{
    private static uint NEXT_ID = 0;

    public bool Removed { get; private set; } = false;
    public readonly uint id;
    public Vec3 position, velocity;
    public int fuse;

    public TNT()
    {
        fuse = 80;
        id = NEXT_ID++;
    }

    public void Tick(SimulationContext context)
    {
        velocity.Y -= 0.04;
        position += velocity;
        velocity *= 0.98;

        // Do not go under ground
        position.Y = Math.Max(0, position.Y);

        fuse--;
        if (fuse <= 0)
        {
            Removed = true;
            context.Remove(this);
            Explode(context);
        }
    }

    private readonly void Explode(SimulationContext context)
    {
		Vec3 center = position + new Vec3(0, 0.98F * 0.0625D, 0);
        context.LogExplosion(center);

        uint thisID = id;

        context.ModifyEntities((ref TNT other) =>
        {
            if (other.id == thisID)
            {
                return;
            }

            double squareDist = center.SquareDistanceTo(other.position);

            if (squareDist >= 64)
            {
                return;
            }

            Vec3 dir = other.position - center;
            if (dir.SquareLength() == 0.0)
            {
                return;
            }

            dir = dir.NormalizeFast();
            dir *= 1 - (dir.Length() / 8.0);

            other.velocity += dir;
        });
    }

    public static bool operator ==(TNT a, TNT b) => a.id == b.id;
    public static bool operator !=(TNT a, TNT b) => a.id != b.id;

	public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is TNT other && this == other;
	public override readonly int GetHashCode() => id.GetHashCode();
}
