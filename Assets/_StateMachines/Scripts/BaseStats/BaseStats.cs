using UnityEngine;

[CreateAssetMenu(fileName = "NewBaseStats", menuName = "Character Stats/Generic", order = 0)]
public class BaseStats : ScriptableObject
{
    [Space(5f)]
    [SerializeField]
    private int healthMax = 100;
    public int HealthMax { get => healthMax; private set => healthMax = value; }

    [Header("Movement")]
    [Header("Speed")]
    [SerializeField]
    private float groundSpeed = 10f;
    public float GroundSpeed { get => groundSpeed; private set => groundSpeed = value; }
    [SerializeField]
    private float airSpeed = 5f;
    public float AirSpeed { get => airSpeed; private set => airSpeed = value; }
    [SerializeField]
    private float waterSpeed = 5f;
    public float WaterSpeed { get => waterSpeed; private set => waterSpeed = value; }
    [SerializeField]
    private float wallSlideSpeed = 2f;
    public float WallSlideSpeed { get => wallSlideSpeed; private set => wallSlideSpeed = value; }

    [Header("Acceleration")]
    [SerializeField]
    private float groundAccelerationTime = .1f;
    public float GroundAccelerationTime { get => groundAccelerationTime; private set => groundAccelerationTime = value; }
    [SerializeField]
    private float airAccelerationTime = .3f;
    public float AirAccelerationTime { get => airAccelerationTime; private set => airAccelerationTime = value; }
    [SerializeField]
    private float waterAccelerationTime = .2f;
    public float WaterAccelerationTime { get => waterAccelerationTime; private set => waterAccelerationTime = value; }
    [SerializeField]
    private float wallSlideAccelerationTime = .3f;
    public float WallSlideAccelerationTime { get => wallSlideAccelerationTime; private set => wallSlideAccelerationTime = value; }

    [Header("Jump")]
    [SerializeField]
    private float jumpHeight = 2f;
    public float JumpHeight { get => jumpHeight; private set => jumpHeight = value; }
    [SerializeField]
    private float timeToJumpApex = .3f;
    public float TimeToJumpApex { get => timeToJumpApex; private set => timeToJumpApex = value; }
    [SerializeField]
    private int jumpAmmount = 1;
    public int JumpAmmount { get => jumpAmmount; private set => jumpAmmount = value; }

    [Header("Slopes")]
    [SerializeField]
    private float maxClimbAngle = 50f;
    public float MaxClimbAngle { get => maxClimbAngle; private set => maxClimbAngle = value; }
    [SerializeField]
    private float maxDescendAngle = 75f;
    public float MaxDescendAngle { get => maxDescendAngle; private set => maxDescendAngle = value; }

    [Header("WallGrab")]
    [SerializeField]
    private float wallGrabTimeLimit = 2f;
    public float WallGrabTimeLimit { get => wallGrabTimeLimit; private set => wallGrabTimeLimit = value; }
    [SerializeField]
    private Vector2 wallJumpProportions = new Vector2(2, .5f);
    public Vector2 WalljumpProportions { get => wallJumpProportions; private set => wallJumpProportions = value; }

    [Space(10f)]
    [Header("Combat")]
    [SerializeField]
    private float attack = 3f;
    public float Attack { get => attack; private set => attack = value; }
}
