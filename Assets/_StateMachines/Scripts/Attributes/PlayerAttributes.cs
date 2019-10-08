public class PlayerAttributes : Attributes
{
    public float DashImpulse;
    protected new PlayerBaseStats baseStats { get => (PlayerBaseStats)base.baseStats; set => base.baseStats = value; }

    public override void SetBaseStats(BaseStats newStats)
    {
        base.SetBaseStats(newStats);

        DashImpulse = baseStats.DashImpulse;
    }
}
