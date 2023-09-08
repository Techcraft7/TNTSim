namespace TNTSim.Simulation;

internal struct TNT
{
    public double x, y, z;
    public double velX, velY, velZ;
    public int fuse;

    public TNT() => fuse = 80;

    public void Tick()
    {
        velY -= 0.04;
        x += velX;
        y += velY;
        z += velZ;
        velX *= 0.98;
        velY *= 0.98;
        velZ *= 0.98;

        fuse--;
        if (fuse <= 0)
        {
            throw new NotImplementedException("KABOOM");
        }
    }
}
