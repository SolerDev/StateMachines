using SebCharCtrl;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class Character : MonoBehaviour
{
    #region Properties

    public Animator Anim { get; protected set; }
    //public Rigidbody2D RB { get; protected set; }
    public Transform Trans { get; protected set; }
    public BoxCollider2D CurrentColl { get; protected set; }
    public Collider2D[] Colls { get; protected set; }
    public SebController Controller { get; protected set; }

    [SerializeField]
    protected BaseStats baseStats;
    public BaseStats BaseStats { get => baseStats; protected set => baseStats = value; }

    //[SerializeField]
    protected Attributes attributes;
    public Attributes Attributes { get => attributes; protected set => attributes = value; }

    [SerializeField]
    protected InputReader inputReader;
    public InputReader InputReader { get => inputReader; protected set => inputReader = value; }

    [HideInInspector]
    public StateMachine StateMachine;
    protected Dictionary<Type, State> states;

    protected SIdle idleState;
    protected SDie dieState;
    private SHurt hurtState;

    #endregion

    protected virtual void Awake()
    {
        Trans = transform;
        CurrentColl = GetComponent<BoxCollider2D>();
        Colls = new Collider2D[] { CurrentColl };
        Anim = GetComponentInChildren<Animator>();
        //RB = GetComponent<Rigidbody2D>();

        Attributes = GetComponent<Attributes>();
        Controller = GetComponent<SebController>();

        InitializeStates();
    }

    protected virtual void Update()
    {
        inputReader.Read();
    }

    protected virtual void FixedUpdate()
    {
        inputReader.ClearInput();
    }

    protected virtual void InitializeStates()
    {
        StateMachine = GetComponent<StateMachine>();
        states = new Dictionary<Type, State>();
        StateMachine.AvailableStates = states;

        idleState = new SIdle(this);
        states.Add(typeof(SIdle), idleState);

        dieState = new SDie(this);
        states.Add(typeof(SDie), dieState);

        hurtState = new SHurt(this);
        states.Add(typeof(SHurt), hurtState);
    }
}
