public class PlayerAttributes : Attributes
{
    public float DashImpulse;
    protected new PlayerBaseStats stats { get => (PlayerBaseStats)base.stats; set => base.stats = value; }

    public PlayerAttributes(PlayerBaseStats initialStats, StateMachine machine) : base(initialStats, machine)
    {
        SetBaseStats(initialStats);
    }

    public override void SetBaseStats(BaseStats newStats)
    {
        base.SetBaseStats(newStats);

        DashImpulse = stats.DashImpulse;
    }
}
