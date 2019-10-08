using SebCharCtrl;
using System;
using UnityEngine;

public class Attributes : MonoBehaviour
{
    #region Properties

    [SerializeField]
    protected BaseStats baseStats;
    protected StateMachine stateMachine;

    [Header("Combat")]
    public bool Alive;
    public bool Hurt;
    public bool Attacking;

    [Header("Health")]
    public int HealthMax;
    public int HealthCurrent;

    [Space(10f)]
    [Header("Movement")]
    [HideInInspector] public float GroundSpeedSmoothing;
    [HideInInspector] public float AirSpeedSmoothing;
    [HideInInspector] public float WaterSpeedSmoothing;
    [HideInInspector] public Vector2 GroundSpeedSmoothingVector;
    [HideInInspector] public Vector2 AirSpeedSmoothingVector;
    [HideInInspector] public Vector2 WaterSpeedSmoothingVector;

    [Header("Max Speed")]
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
    [HideInInspector] public float Gravity;
    public float MaxJumpVelocity;
    public float MinJumpVelocity;

    public int JumpAmmount;
    [HideInInspector] public int JumpsCount;

    [Header("Slopes")]
    public float MaxClimbAngle;
    public float MaxDescendAngle;

    [Header("Wall")]
    public float WallGrabTimeLimit;
    public Vector2 wallJumpProportions;

    #endregion

    protected void Awake()
    {
        stateMachine = GetComponent<StateMachine>();

        SetBaseStats(baseStats);
    }

    public virtual void SetBaseStats(BaseStats newStats)
    {
        Alive = true;
        Hurt = false;

        Attacking = false;

        baseStats = newStats;

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

        MaxJumpHeight = baseStats.MaxJumpHeight;
        MinJumpHeight = baseStats.MinJumpHeight;
        TimeToJumpApex = baseStats.TimeToJumpApex;
        Gravity = -(2 * MaxJumpHeight) / Mathf.Pow(TimeToJumpApex, 2);
        MaxJumpVelocity = Math.Abs(Gravity) * TimeToJumpApex;
        MinJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Gravity) * MinJumpHeight);


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
