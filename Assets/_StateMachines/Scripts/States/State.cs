using SebCharCtrl;
using System;
using UnityEngine;

public abstract class State
{
    protected Character character;
    protected SebController controller;
    protected InputReader reader;
    protected Attributes attributes;

    protected Transform trans;
    protected Animator anim;
    protected BoxCollider2D defaultCollider;
    //protected Rigidbody2D rb;

    protected Type t;   //"Update" return
    protected Type ft;  //"FixedUpdate" return

    //private readonly int velXHash = Animator.StringToHash("VelX");
    //private readonly int velYHash = Animator.StringToHash("VelY");

    public State(Character character)
    {
        this.character = character;
        this.controller = character.Controller;
        this.reader = character.Reader;
        this.attributes = character.Attributes;
        this.trans = character.Trans;
        this.anim = character.Anim;
        this.defaultCollider = character.CurrentColl;
        //this.rb = character.RB;
    }

    public virtual void OnEnter()
    {
        //Debug.Log(character + " ENTERED the state " + this);
    }

    public virtual void OnExit()
    {
        //Debug.Log(character + " LEFT the state " + this);
    }

    public virtual Type Tick()
    {
        t = null;
        return t;
    }

    public virtual Type FixedTick()
    {
        ft = null;

        //anim.SetFloat(velYHash, character.Controller.Velocity.y);
        //anim.SetFloat(velXHash, character.Controller.Velocity.x);

        return ft;
    }


}
