using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGround : State
{
    private readonly int groundedHash = Animator.StringToHash("Grounded");

    public SGround(Character character) : base(character)
    {
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        character.Attributes.JumpsCount = 0;

        anim.SetBool(groundedHash, true);
    }

    public override void OnExit()
    {
        base.OnExit();
        anim.SetBool(groundedHash, false);
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();
        if (ft != null)
        {
            return ft;
        }

        if (!character.IsGrounded())
        {
            ft = typeof(SFall);
        }

        return ft;
    }
}
