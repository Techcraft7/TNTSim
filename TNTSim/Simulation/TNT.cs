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

    public void Tick(SimulationContext context, bool isWarpingToTarget = false)
    {
        velocity.Y -= 0.04;

        // Powdered snow scaling
        if (isWarpingToTarget)
        {
            velocity *= 0.9;
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

    private readonly void Explode(SimulationContext context)
    {
        //TraceLog(TraceLogLevel.LOG_INFO, $"BEGIN {id} EXPLODE");
		Vec3 center = position + new Vec3(0, 0.98F * 0.0625D, 0);
        context.LogExplosion(center);

        uint thisID = id;

        context.ModifyEntities((ref TNT other) =>
        {
            if (other.id == thisID)
            {
                return;
            }

            Vec3 dir = other.position - center;
            double sqaureDistance = dir.SquareLength();

            if (sqaureDistance is 0 or >= 64)
            {
                return;
            }

            dir = dir.Normalize();
            dir *= 1 - (Math.Sqrt(sqaureDistance) / 8.0);

            other.velocity += dir;

            //TraceLog(TraceLogLevel.LOG_INFO, $"TNT {other.id} exploded by {thisID}: added {dir.Y} | centerY = {center.Y} | otherY = {other.position.Y}");
        });
    }

    public static bool operator ==(TNT a, TNT b) => a.id == b.id;
    public static bool operator !=(TNT a, TNT b) => a.id != b.id;

	public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is TNT other && this == other;
	public override readonly int GetHashCode() => id.GetHashCode();
}
