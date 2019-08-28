using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSpawn : State
{
    private Collider2D[] colls;

    private readonly int spawnHash = Animator.StringToHash("Spawn");

    private float timeInState = 0f;
    private readonly float timeInStateMax = 1.5f;

    public SSpawn(Character character) : base(character)
    {
        this.colls = character.Colls;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        anim.SetTrigger(spawnHash);

        for (int i = 0; i < colls.Length; i++)
        {
            colls[i].enabled = false;
        }

        //rb.simulated = false;
        character.Controller.Enabled = false;


        timeInState = 0f;
    }

    public override void OnExit()
    {
        base.OnExit();

        for (int i = 0; i < colls.Length; i++)
        {
            colls[i].enabled = true;
        }

        //rb.simulated = true;
        character.Controller.Enabled = true;

    }

    public override Type Tick()
    {
        t= base.Tick();

        timeInState +=Time.deltaTime;

        if (timeInState>timeInStateMax)
        {
            t = typeof(SIdle);
        }


        return t;
    }
}
