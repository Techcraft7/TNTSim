namespace TNTSim;

internal struct Charge : ICloneable<Charge>
{
    public int tntCount, scheduleCount, fuse;
    public bool cancelX, cancelY;
    public bool single;

    public Charge Clone() => new()
    {
        tntCount = tntCount,
        scheduleCount = scheduleCount,
        fuse = fuse,
        cancelX = cancelX,
        cancelY = cancelY,
        single = single
    };
}
