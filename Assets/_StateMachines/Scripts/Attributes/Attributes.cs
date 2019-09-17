using System;
using UnityEngine;

public class Attributes : MonoBehaviour
{
    #region Properties

    protected BaseStats stats;
    protected StateMachine machine;

    [Header("Combat")]
    public bool Alive;
    public bool Hurt;
    public bool Attacking;

    [Space(10f)]
    [Header("Health")]
    public int HealthMax;
    public int HealthCurrent;

    [Space(10f)]
    [Header("Movement")]
    [Header("Smoothing")]
    public float GroundSpeedSmoothing;
    public float AirSpeedSmoothing;
    public float WaterSpeedSmoothing;
    [HideInInspector] public Vector2 GroundSpeedSmoothingVector;
    [HideInInspector] public Vector2 AirSpeedSmoothingVector;
    [HideInInspector] public Vector2 WaterSpeedSmoothingVector;

    [Header("Speed")]
    public float GroundSpeed;
    public float AirSpeed;
    public float WaterSpeed;
    public float WallSlideSpeed;

    [Header("Acceleration")]
    public float GroundAccelerationTime;
    public float AirAccelerationTime;
    public float WaterAccelerationTime;
    public float WallSlideAccelerationTime;

    [Header("Jump")]
    public float MaxJumpHeight;
    public float MinJumpHeight;
    public float TimeToJumpApex;
    public float Gravity;
    public float MaxJumpVelocity;
    public float MinJumpVelocity;

    public int JumpAmmount;
    public int JumpsCount;

    [Header("Slopes")]
    public float MaxClimbAngle;
    public float MaxDescendAngle;

    [Header("Wall")]
    public float WallGrabTimeLimit;
    public Vector2 wallJumpProportions;

    #endregion

    protected void Awake()
    {
        machine = GetComponent<StateMachine>();

        SetBaseStats(stats);
    }

    public virtual void SetBaseStats(BaseStats newStats)
    {
        Alive = true;
        Hurt = false;

        Attacking = false;

        stats = newStats;

        HealthMax = stats.HealthMax;
        HealthCurrent = HealthMax;

        //FacingDirection = Vector2.one;

        GroundSpeed = stats.GroundSpeed;
        AirSpeed = stats.AirSpeed;
        WaterSpeed = stats.WaterSpeed;
        WallSlideSpeed = stats.WallSlideSpeed;

        GroundAccelerationTime = stats.GroundAccelerationTime;
        AirAccelerationTime = stats.AirAccelerationTime;
        WaterAccelerationTime = stats.WaterAccelerationTime;
        WallSlideAccelerationTime = stats.WallSlideAccelerationTime;

        MaxJumpHeight = stats.MaxJumpHeight;
        MinJumpHeight = stats.MinJumpHeight;
        TimeToJumpApex = stats.TimeToJumpApex;
        Gravity = -(2 * MaxJumpHeight) / Mathf.Pow(TimeToJumpApex, 2);
        MaxJumpVelocity = Math.Abs(Gravity) * TimeToJumpApex;
        MinJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Gravity) * MinJumpHeight);


        JumpAmmount = stats.JumpAmmount;
        JumpsCount = 0;

        MaxClimbAngle = stats.MaxClimbAngle;
        MaxDescendAngle = stats.MaxDescendAngle;

        WallGrabTimeLimit = stats.WallGrabTimeLimit;
        wallJumpProportions = stats.WalljumpProportions;
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
        machine.SwitchToState(typeof(SHurt));
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
        machine.SwitchToState(typeof(SDie));
    }
}
