namespace TNTSim.Util;

internal record struct Vec3(double X, double Y, double Z)
{
	// Bypass CS8859
	public readonly Vec3 Copy() => new(X, Y, Z);

	public readonly double Dot(Vec3 v) => (X * v.X) + (Y * v.Y) + (Z * v.Z);

	public readonly double SquareDistanceTo(Vec3 v)
	{
		double dx = X - v.X;
		double dy = Y - v.Y;
		double dz = Z - v.Z;
		return (dx * dx) + (dy * dy) + (dz * dz);
	}

	public readonly double SquareLength() => SquareDistanceTo(default);

	public readonly double Length() => Math.Sqrt(SquareDistanceTo(default));

	public Vec3 Normalize()
	{
		X /= Length();
		Y /= Length();
		Z /= Length();
		return this;
	}

	public Vec3 NormalizeFast()
	{
		// Math.ReciprocalSqrtEstimate only works on ARM for some reason
		// So use the Quake 3 method
		float number = (float)SquareDistanceTo(default);
		const float THREE_HALFS = 3f / 2f;

		float x2 = number * 0.5f;
		float y = number;
		unchecked
		{
			int i = BitConverter.SingleToInt32Bits(y); // Avoiding unsafe code
			i = 0x5F3759DF - (i >>> 1);
			y = BitConverter.Int32BitsToSingle(i); // Avoiding unsafe code
			y *= THREE_HALFS - (x2 * y * y);
			// Slightly better accuracy for release mode
#if !DEBUG
			y *= THREE_HALFS - (x2 * y * y);
#endif
		}

		X *= y;
		Y *= y;
		Z *= y;
		return this;
	}

	public static Vec3 operator *(Vec3 v, double x)
	{
		v.X *= x;
		v.Y *= x;
		v.Z *= x;
		return v;
	}

	public static Vec3 operator /(Vec3 v, double x)
	{
		v.X /= x;
		v.Y /= x;
		v.Z /= x;
		return v;
	}

	public static Vec3 operator +(Vec3 v1, Vec3 v2) => new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
	public static Vec3 operator -(Vec3 v1, Vec3 v2) => new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
	public static Vec3 operator -(Vec3 v)
	{
		v.X = -v.X;
		v.Y = -v.Y;
		v.Z = -v.Z;
		return v;
	}
}
