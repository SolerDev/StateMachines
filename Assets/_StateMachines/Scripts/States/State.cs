using System;
using UnityEngine;

public abstract class State
{
    protected Character character;
    protected Transform trans;
    protected Animator anim;
    protected BoxCollider2D coll;
    //protected Rigidbody2D rb;

    protected Type t;   //"Update" r
    protected Type ft;  //"FixedUpdate" r

    private readonly int velXHash = Animator.StringToHash("VelX");
    //private readonly int velYHash = Animator.StringToHash("VelY");

    public State(Character character)
    {
        this.character = character;
        this.trans = character.Trans;
        this.anim = character.Anim;
        this.coll = character.CurrentColl;
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
        anim.SetFloat(velXHash, character.Controller.Velocity.x);

        return ft;
    }


}
