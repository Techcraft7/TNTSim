namespace TNTSim.Simulation;

internal sealed record SimulationContext(IReadOnlyList<TNT> Entities)
{
    private readonly IList<TNT> tnt = (Entities as IList<TNT>)!;

    public void Remove(TNT entity)
    {
        tnt.Remove(entity);
    }
}