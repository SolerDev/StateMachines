using System;
using UnityEngine;

public class Attributes
{
    protected BaseStats baseStats;
    protected StateMachine stateMachine;

    public bool Alive;
    public bool Hurt;
    public bool Attacking;

    public int HealthMax;
    public int HealthCurrent;

    //public Vector2 FacingDirection;

    public float GroundSpeedSmoothing;
    public float AirSpeedSmoothing;
    public float WaterSpeedSmoothing;
    public Vector2 GroundSpeedSmoothingVector;
    public Vector2 AirSpeedSmoothingVector;
    public Vector2 WaterSpeedSmoothingVector;

    public float GroundSpeed;
    public float AirSpeed;
    public float WaterSpeed;
    public float WallSlideSpeed;

    public float GroundAccelerationTime;
    public float AirAccelerationTime;
    public float WaterAccelerationTime;
    public float WallSlideAccelerationTime;

    public float JumpHeight;
    public float TimeToJumpApex;
    public float Gravity;
    public float JumpVelocity;

    public int JumpAmmount;
    public int JumpsCount;

    public float MaxClimbAngle;
    public float MaxDescendAngle;

    public float WallGrabTimeLimit;
    public Vector2 wallJumpProportions;

    public Attributes(BaseStats baseStats, StateMachine stateMachine)
    {
        this.baseStats = baseStats;
        this.stateMachine = stateMachine;

        Initialize();
    }

    public virtual void Initialize()
    {
        Alive = true;
        Hurt = false;

        Attacking = false;

        HealthMax = baseStats.HealthMax;
        HealthCurrent = HealthMax;

        //FacingDirection = Vector2.one;

        GroundSpeed = baseStats.GroundSpeed;
        AirSpeed = baseStats.AirSpeed;
        WaterSpeed = baseStats.WaterSpeed;
        WallSlideSpeed = baseStats.WallSlideSpeed;

        GroundAccelerationTime = baseStats.GroundAccelerationTime;
        AirAccelerationTime = baseStats.AirAccelerationTime;
        WaterAccelerationTime = baseStats.WaterAccelerationTime;
        WallSlideAccelerationTime = baseStats.WallSlideAccelerationTime;

        JumpHeight = baseStats.JumpHeight;
        TimeToJumpApex = baseStats.TimeToJumpApex;
        Gravity = -(2 * JumpHeight) / Mathf.Pow(TimeToJumpApex, 2);
        JumpVelocity = Math.Abs(Gravity) * TimeToJumpApex;

        JumpAmmount = baseStats.JumpAmmount;
        JumpsCount = 0;

        MaxClimbAngle = baseStats.MaxClimbAngle;
        MaxDescendAngle = baseStats.MaxDescendAngle;

        WallGrabTimeLimit = baseStats.WallGrabTimeLimit;
        wallJumpProportions = baseStats.WalljumpProportions;
    }

    //passar para a character extensions
    public void TakeDamage(int ammount)
    {
        HealthCurrent -= ammount;

        //Debug.Log(this + " took " + ammount + " damage.");

        if (HealthCurrent <= 0)
        {
            HealthCurrent = 0;
            Die();
            return;
        }

        //Debug.Log(this + " remaining health is " + HealthCurrent + ".");
        stateMachine.SwitchToState(typeof(SHurt));
    }

    public void Heal(int ammount)
    {
        HealthCurrent += ammount;

        //Debug.Log(this + " took " + ammount + " damage.");

        if (HealthCurrent > HealthMax)
        {
            HealthCurrent = HealthMax;
            return;
        }

        //Debug.Log(this + " remaining health is " + HealthCurrent + ".");
        //stateMachine.SwitchToState(typeof(SHeal));
    }

    private void Die()
    {
        //Debug.Log(this + " died.");

        Alive = false;
        stateMachine.SwitchToState(typeof(SDie));
    }
}
