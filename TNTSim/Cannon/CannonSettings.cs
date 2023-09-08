namespace TNTSim.Cannon;

internal struct CannonSettings : ICloneable<CannonSettings>
{
    public Charge charge1, charge2, charge3, charge4, charge5;
    public Breadboard schedulingBoard, continuationBoard;

    public void SetCharge(int i, Charge newCharge) => _ = i switch
    {
        0 => charge1 = newCharge,
        1 => charge2 = newCharge,
        2 => charge3 = newCharge,
        3 => charge4 = newCharge,
        4 => charge5 = newCharge,
        _ => default
    };

    public void LoadDefaults()
    {
        charge1 = Charge.DEFAULT.Clone();
        charge2 = Charge.DEFAULT.Clone();
        charge3 = Charge.DEFAULT.Clone();
        charge4 = Charge.DEFAULT.Clone();
        charge5 = Charge.DEFAULT.Clone();

        schedulingBoard = default;
        continuationBoard = default;
        for (int i = 0; i < 5; i++)
        {
            continuationBoard[i, i] = Connection.INPUT;
            continuationBoard[i, i + 1] = Connection.NEXT_OUT;
        }
    }

    public CannonSettings Clone() => new()
    {
        charge1 = charge1.Clone(),
        charge2 = charge2.Clone(),
        charge3 = charge3.Clone(),
        charge4 = charge4.Clone(),
        charge5 = charge5.Clone(),
        schedulingBoard = schedulingBoard.Clone(),
        continuationBoard = continuationBoard.Clone()
    };
}
