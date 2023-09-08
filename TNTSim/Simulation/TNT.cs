using System.Diagnostics.CodeAnalysis;

namespace TNTSim.Simulation;

internal struct TNT
{
    private static uint NEXT_ID = 0;

    public readonly uint id;
    public double x, y, z;
    public double velX, velY, velZ;
    public int fuse;

    public TNT()
    {
        fuse = 80;
        id = NEXT_ID++;
    }

    public void Tick(SimulationContext context)
    {
        velY -= 0.04;
        x += velX;
        y += velY;
        z += velZ;
        velX *= 0.98;
        velY *= 0.98;
        velZ *= 0.98;

        fuse--;
        if (fuse <= 0)
        {
            context.Remove(this);
            Explode(context);
        }
    }

    private readonly void Explode(SimulationContext context)
    {
        const double CENTER = 0.98F * 0.0625D;

        for (int i = 0; i < context.Entities.Count; i++)
        {
            TNT other = context.Entities[i];
            double dx = x - other.x;
            double dy = y + CENTER - other.y;
            double dz = z - other.z;
            // Not in 8 block radius
            double squareDist = (dx * dx) + (dy * dy) + (dz * dz);
            if (squareDist > 64)
            {
                continue;
            }
            double dist = Math.Sqrt(squareDist);
            dx /= dist;
            dy /= dist;
            dz /= dist;
            dx *= (8 - dist) / 8;
            dy *= (8 - dist) / 8;
            dz *= (8 - dist) / 8;

            other.velX += dx;
            other.velY += dy;
            other.velZ += dz;
        }
    }

    public static bool operator ==(TNT a, TNT b) => a.id == b.id;
    public static bool operator !=(TNT a, TNT b) => a.id != b.id;

	public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is TNT other && this == other;
	public override readonly int GetHashCode() => id.GetHashCode();
}
