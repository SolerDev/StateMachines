using System;
using UnityEngine;

public class SDie : State
{
    private Collider2D[] colls;

    private readonly int dieHash = Animator.StringToHash("Die");

    public SDie(Character character) : base(character)
    {
        this.colls = character.Colls;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        anim.SetTrigger(dieHash);

        //rb.simulated = false;
        character.Controller.enabled = false;

        for (int i = 0; i < colls.Length; i++)
        {
            colls[i].enabled = false;
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        //rb.simulated = true;
        character.Controller.enabled = true;

        for (int i = 0; i < colls.Length; i++)
        {
            colls[i].enabled = true;
        }
    }

    public override Type Tick()
    {
        t = base.Tick();

        if (character.Attributes.Alive)
        {
            t = typeof(SSpawn);
        }

        return t;
    }
}
