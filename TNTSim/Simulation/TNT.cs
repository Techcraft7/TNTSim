using System.Diagnostics.CodeAnalysis;

namespace TNTSim.Simulation;

internal sealed class TNT
{
    private const double CENTER_OFFSET = 0.98F * 0.0625D;
    private static uint NEXT_ID = 0;


    public bool Removed { get; private set; } = false;
    public readonly uint id;
    public Vec3 position, velocity;
    public int fuse;
    public bool loaded;

    public TNT()
    {
        fuse = 80;
        id = NEXT_ID++;
    }

    public void Tick(SimulationContext context, bool firstTick = false)
    {
        loaded = true;
        velocity.Y -= 0.04;

        if (firstTick)
        {
            velocity.X *= 0.9;
            velocity.Y *= 1.5;
            velocity.Z *= 0.9;
        }

        position += velocity;

        if (firstTick)
        {
            velocity = default;
        }

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

    private void Explode(SimulationContext context)
    {
        Vec3 center = position + new Vec3(0, CENTER_OFFSET, 0);
        context.LogExplosion(center);
        uint thisID = id;
        context.ModifyEntities((ref TNT other) =>
        {
            if (other.id == thisID || !other.loaded)
            {
                return;
            }
            other.velocity += ExplosionCalculator.GetVelocity(center, other.position);
        }, true);
    }

    public static bool operator ==(TNT a, TNT b) => a.id == b.id;
    public static bool operator !=(TNT a, TNT b) => a.id != b.id;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is TNT other && this == other;
    public override int GetHashCode() => id.GetHashCode();
}
