namespace LibTNT;

public static class TNTMath
{
	public const double TNT_GRAVITY = 0.04;
	public const double TNT_DRAG = 0.02;

	public static Vec3 GetVelocity(Vec3 explosionCenter, Vec3 entityPos)
	{
		double distanceNormalized = (double)(entityPos.DistanceTo(explosionCenter) / 8f);
		// Exit if distance is too high
		if (distanceNormalized > 1.0D)
		{
			return default;
		}
		// Get direction vector, different for tnt for some reason
		Vec3 dir = entityPos - explosionCenter;
		// Exit if direction vector is 0
		if (dir.Length() == 0.0D)
		{
			return default;
		}
		// Normalize direction vector
		dir *= 1 / dir.Length();
		// Velocity magnitude is (1 - distance/2power) * exposure (exposure is always one bc we are in the air)
		return dir * (1.0D - distanceNormalized);
	}

	// TODO: determine if this is correct
	public static (Vec3 position, Vec3 velocity) PredictMovement(Vec3 r0, Vec3 v0, int t)
	{
		double totalDragMultiplier = Math.Pow(1 - TNT_DRAG, t);
		Vec3 a = new(0, -TNT_GRAVITY, 0);

		// Formula taken from wiki
		Vec3 vt = v0 + (a * (1 - totalDragMultiplier) / TNT_DRAG * (1 - TNT_DRAG));

		// Above formula integrated from 0 to t
		Vec3 rt = r0 + (v0 * t) + (a * (TNT_DRAG - 1) * (((totalDragMultiplier - 1) / Math.Log(1 - TNT_DRAG)) - t) / TNT_DRAG);

		return (rt, vt);
	}

	// TODO: determine if this is correct
	public static (Vec3 position, Vec3 velocity) UnpredictMovement(Vec3 r, Vec3 v, int ticks)
	{
		for (int i = 0; i < ticks; i++)
		{
			v /= 1 - TNT_DRAG;
			r -= v;
			v.Y += TNT_GRAVITY;
		}
		return (r, v);
	}
}
