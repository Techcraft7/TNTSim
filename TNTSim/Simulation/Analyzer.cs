using System.Diagnostics;
using System.Security.Cryptography;

namespace TNTSim.Simulation;

internal class Analyzer
{
	public double PercentComplete => currentTick / 80.0;
	public IReadOnlyList<double> MSPT => mspt;
	public IReadOnlyList<int> TNTCounts => tntCounts;
	public double CenterX { get; private set; } = double.NaN;
	public double CenterZ { get; private set; } = double.NaN;
	public double DamageRadiusX { get; private set; } = double.NaN;
	public double DamageRadiusZ { get; private set; } = double.NaN;
	public double DamagePercentage { get; private set; } = double.NaN;

	private readonly Simulation context;
	private readonly Task task;
	private readonly double[] mspt = new double[80];
	private readonly int[] tntCounts = new int[80];
	private int currentTick = 0;

	public Analyzer(SimulationSettings settings)
	{
		context = SimulationFactory.Create(settings);
		task = Task.Run(Analyze);
	}

	private void Analyze()
	{
		MeasureMSPTAndTNTCount();

		CalculateExplosionMetrics();
	}

	private void CalculateExplosionMetrics()
	{
		CenterX = CenterZ = DamageRadiusX = DamageRadiusZ = DamagePercentage = double.NaN;
		if (context.Explosions.Count == 0)
		{
			return;
		}

		List<Vec3> onGround = context.Explosions.Where(static v => Math.Abs(v.Y) <= EXPLOSION_SIZE).ToList();

		double minX = onGround.Min(static v => v.X);
		double minZ = onGround.Min(static v => v.Z);
		double maxX = onGround.Max(static v => v.X);
		double maxZ = onGround.Max(static v => v.Z);

		DamageRadiusX = (maxX - minX) / 2.0;
		DamageRadiusZ = (maxZ - minZ) / 2.0;

		CenterX = (minX + maxX) / 2.0;
		CenterZ = (minZ + maxZ) / 2.0;

		// TODO: account for overlapping
		double damagedArea = 0;
		double totalArea = Math.PI * DamageRadiusX * DamageRadiusZ;
		DamagePercentage = damagedArea / totalArea;
	}

	private void MeasureMSPTAndTNTCount()
	{
		Stopwatch sw = new();
		while (currentTick < 80)
		{
			tntCounts[currentTick] = context.TNT.Count;

			sw.Restart();
			context.Tick();

			TimeSpan elapsed = sw.Elapsed;
			mspt[currentTick] = elapsed.TotalMilliseconds;

			currentTick++;
		}
	}

	public bool IsComplete() => task.IsCompleted;
}
