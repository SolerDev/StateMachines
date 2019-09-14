using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWater : State
{
    private readonly int underwaterHash = Animator.StringToHash("Underwater");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    public SWater(Character character) : base(character)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        //anim.SetBool(underwaterHash,true);
        anim.SetBool("Underwater", true);
    }

    public override void OnExit()
    {
        base.OnExit();
        //anim.SetBool(underwaterHash, false);
        anim.SetBool("Underwater", false);
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();
        if (ft!=null)
        {
            return ft;
        }

        if (!character.IsUnderwater())
        {
            ft = typeof(SFall);
        }
        else if (character.Reader.JumpPress && !character.IsWaterAbove())
        {
            character.Jump(Vector2.up, jumpHash);
        }

        return ft;
    }
}
