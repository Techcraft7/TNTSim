using System.Diagnostics.CodeAnalysis;

namespace TNTSim.Simulation;

internal struct TNT
{
    private const double CENTER_OFFSET = 0.98F * 0.0625D;
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

    public void Tick(SimulationContext context, bool firstTick = false)
    {
        velocity.Y -= 0.04;

        position += velocity;
        velocity *= 0.98;

        // If on ground
        if (position.Y <= 0)
        {
            position.Y = 0;
            velocity.X *= 0.7;
            velocity.Z *= 0.7;
            velocity.Y = 0;
        }

        fuse--;
        if (fuse <= 0)
        {
            Removed = true;
            context.Remove(this);
            Explode(context);
        }
    }

    private readonly void Explode(SimulationContext context) => context.ModifyEntities(ExplodeOnto);

    public readonly void ExplodeOnto(ref TNT other)
    {
        if (other.id == id)
        {
            return;
        }

        // This is D_e and D_f because everything is TNT
        double dx = other.position.X - position.X;
        double dy = other.position.Y - position.Y;
        double dz = other.position.Z - position.Z;
        double squareDistance = (dx * dx) + (dy * dy) + (dz * dz);

        if (squareDistance is 0 or >= 64)
        {
            return;
        }

        // Same as d = d - dy^2 + (dy + offset)^2 
        squareDistance += (2 * dy * CENTER_OFFSET) + (CENTER_OFFSET * CENTER_OFFSET);
        double l = Math.Sqrt(squareDistance);
        double lInv = 1 / l;
        double k = lInv - 0.125; // lInv * (1 - (l / 8)) = lInv - 1/8
        dx *= k;
        dy *= k;
        dz *= k;
        other.velocity = new(other.velocity.X + dx, other.velocity.Y + dy, other.velocity.Z + dz);
    }

    public static bool operator ==(TNT a, TNT b) => a.id == b.id;
    public static bool operator !=(TNT a, TNT b) => a.id != b.id;

	public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is TNT other && this == other;
	public override readonly int GetHashCode() => id.GetHashCode();
}
