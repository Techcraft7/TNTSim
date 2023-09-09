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
		Vec3 center = position.Copy() + new Vec3(0, 0.98F * 0.0625D, 0);

        context.ModifyEntities((ref TNT other) =>
        {
            Vec3 d = center - other.position;
            double squareDist = d.SquareLength();
            // Not in 8 block radius
            if (squareDist > 64)
            {
                return;
            }
            double dist = Math.Sqrt(squareDist);
            d /= dist;
            d *= (8 - dist) / 8;

            other.velocity += d;
        });
    }

    public static bool operator ==(TNT a, TNT b) => a.id == b.id;
    public static bool operator !=(TNT a, TNT b) => a.id != b.id;

	public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is TNT other && this == other;
	public override readonly int GetHashCode() => id.GetHashCode();
}
