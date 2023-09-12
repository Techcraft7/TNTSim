using System.Diagnostics.CodeAnalysis;

namespace TNTSim.Simulation;

internal struct TNT
{
    private static readonly Vec3 CENTER_OFFSET = new(0, 0.98F * 0.0625D, 0);
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

    public void Tick(SimulationContext context, bool isWarpingToTarget = false)
    {
        velocity.Y -= 0.04;

        // Powdered snow scaling
        if (isWarpingToTarget)
        {
            velocity.X *= 0.9f;
            velocity.Y *= 1.5;
            velocity.Z *= 0.9f;
        }

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

        // Powdered snow canceling velocity
        if (isWarpingToTarget)
        {
            velocity = default;
        }
    }

    private readonly void Explode(SimulationContext context) => context.ModifyEntities(ExplodeOnto);

    public readonly void ExplodeOnto(ref TNT other)
    {
        if (other.id == id)
        {
            return;
        }

        Vec3 dir = other.position - (position + CENTER_OFFSET); // This is D_e and D_f because everything is TNT
        double sqaureDistance = dir.SquareLength(); // No sqrt needed

        if (sqaureDistance is 0 or >= 64) // If on top of eachother or too far away
        {
            return;
        }

        dir = dir.Normalize(); // \hat{D_e}
        dir *= 1 - (Math.Sqrt(sqaureDistance) / 8.0); // (2P - ||D_f||)/2P = 1 - (||D_f||/2P)

        other.velocity += dir;
    }

    public static bool operator ==(TNT a, TNT b) => a.id == b.id;
    public static bool operator !=(TNT a, TNT b) => a.id != b.id;

	public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is TNT other && this == other;
	public override readonly int GetHashCode() => id.GetHashCode();
}
