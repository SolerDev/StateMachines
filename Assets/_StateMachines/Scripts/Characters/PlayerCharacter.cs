using UnityEngine;

public class PlayerCharacter : Character
{
    #region Properties

    public new PlayerAttributes Attributes { get => (PlayerAttributes)base.attributes; protected set => base.attributes = value; }
    public new PlayerReader InputReader { get => (PlayerReader)base.inputReader; protected set => base.inputReader = value; }
    public new PlayerBaseStats BaseStats { get => (PlayerBaseStats)base.baseStats; protected set => base.baseStats = value; }

    protected SSpawn spawnState;
    private SWalkPlayer walkState;
    //private SRise riseState;
    private SFall fallState;
    private SDash dashState;
    private SFloat floatState;
    private SSwim swimState;
    private SWallGrab wallGrabState;
    private SWallSlide wallSlideState;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        #region Cache


        #endregion

        #region Initialize

        Attributes = new PlayerAttributes(BaseStats, StateMachine);
        InitializeStates();

        #endregion
    }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        #region Create and Add States

        //Generic ------------------------------------------------------------------------
        spawnState = new SSpawn(this);
        states.Add(typeof(SSpawn), spawnState);

        //riseState = new SRise(this);
        //states.Add(typeof(SRise), riseState);

        fallState = new SFall(this);
        states.Add(typeof(SFall), fallState);

        dashState = new SDash(this);
        states.Add(typeof(SDash), dashState);

        floatState = new SFloat(this);
        states.Add(typeof(SFloat), floatState);

        swimState = new SSwim(this);
        states.Add(typeof(SSwim), swimState);

        wallGrabState = new SWallGrab(this);
        states.Add(typeof(SWallGrab), wallGrabState);

        wallSlideState= new SWallSlide(this);
        states.Add(typeof(SWallSlide), wallSlideState);

        //playerSpecific ------------------------------------------------------------------
        walkState = new SWalkPlayer(this);
        states.Add(typeof(SWalkPlayer), walkState);

        #endregion

        //Set the state machine's states' Dictionary
        //StateMachine.AvailableStates = states;
    }
}
