using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : Attributes
{
    public float DashImpulse;
    protected new PlayerBaseStats baseStats { get => (PlayerBaseStats)base.baseStats; set => base.baseStats = value; }

    public PlayerAttributes(PlayerBaseStats baseStats, StateMachine stateMachine) : base(baseStats, stateMachine)
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();

        DashImpulse = baseStats.DashImpulse;
    }
}
