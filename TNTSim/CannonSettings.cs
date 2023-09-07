namespace TNTSim;

internal struct CannonSettings : ICloneable<CannonSettings>
{
    public Charge charge1, charge2, charge3, charge4, charge5;

    public CannonSettings Clone() => new()
    {
        charge1 = charge1.Clone(),
        charge2 = charge2.Clone(),
        charge3 = charge3.Clone(),
        charge4 = charge4.Clone(),
        charge5 = charge5.Clone(),
    };
}
