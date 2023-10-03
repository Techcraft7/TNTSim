using System.Diagnostics;

namespace TNTSim.Simulation;

internal class Analyzer
{
	public double PercentComplete => currentTick / 80.0;
	public IReadOnlyList<double> MSPT => mspt;
	public IReadOnlyList<int> TNTCounts => tntCounts;

	private readonly SimulationContext context;
	private readonly Task task;
	private readonly double[] mspt = new double[80];
	private readonly int[] tntCounts = new int[80];
	private int currentTick = 0;

	public Analyzer(SimulationSettings settings)
	{
		context = Simulator.Create(settings);
		task = Task.Run(Analyze);
	}

	private void Analyze()
	{
		MeasureMSPTAndTNTCount();

		CalculateExplosionMetrics();
	}

	private void CalculateExplosionMetrics()
	{
		var onGround = context.Explosions.Where(static v => Math.Abs(v.Y) <= EXPLOSION_SIZE);

		double minX = onGround.Min(static v => v.X);
		double minZ = onGround.Min(static v => v.Z);
		double maxX = onGround.Max(static v => v.X);
		double maxZ = onGround.Max(static v => v.Z);

		double damageRadiusX = (maxX - minX) / 2.0;
		double damageRadiusZ = (maxZ - minZ) / 2.0;

		double centerX = (minX + maxX) / 2.0;
		double centerZ = (minZ + maxZ) / 2.0;

		double damagedArea = onGround.Sum(static v => Math.PI * Math.Pow(Math.Sqrt((EXPLOSION_SIZE * EXPLOSION_SIZE) - v.Y), 2));
		double totalArea = Math.PI * damageRadiusX * damageRadiusZ;

		double damagePercent = damagedArea / totalArea;

		// TODO: store these
		Console.WriteLine($"R: ({damageRadiusX:F0}, {damageRadiusZ:F0}) C: ({centerX:F0}, {centerZ:F0}) D: {damagePercent * 100:F2}%");
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
