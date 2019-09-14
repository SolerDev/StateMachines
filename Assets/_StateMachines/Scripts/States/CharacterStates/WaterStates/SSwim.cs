using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSwim : SWater
{
    private Vector2 facingDirection;

    private readonly int movingHash = Animator.StringToHash("Moving");

    public SSwim(Character character) : base(character)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        anim.SetBool(movingHash, true);
    }

    public override void OnExit()
    {
        base.OnExit();
        anim.SetBool(movingHash, false);
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();
        if (ft != null)
        {
            return ft;
        }

        Vector2 direction = character.Reader.Direction;

        if (direction.Equals(Vector2.zero))
        {
            ft = typeof(SFloat);
        }
        else if (character.IsWaterAbove())
        {
            character.Swim(direction);
        }

        return ft;
    }
}
