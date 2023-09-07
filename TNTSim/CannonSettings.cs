namespace TNTSim;

internal struct CannonSettings
{
    public Charge charge1, charge2, charge3, charge4, charge5;

    public void SetCharge(int i, Charge newCharge) => _ = i switch
    {
        0 => charge1 = newCharge,
        1 => charge2 = newCharge,
        2 => charge3 = newCharge,
        3 => charge4 = newCharge,
        4 => charge5 = newCharge,
        _ => default
    };
}
