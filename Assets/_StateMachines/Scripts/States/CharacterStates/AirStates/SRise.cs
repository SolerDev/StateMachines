using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRise : SAir
{
    private readonly int jumpHash = Animator.StringToHash("Jump");

    public SRise(Character character) : base(character)
    {
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();
        if (ft != null)
        {
            return ft;
        }

        Vector2 direction = character.InputReader.Direction;

        if (character.Controller.Velocity.y < 0f)
        {
            ft = typeof(SFall);
        }
        else if (character.InputReader.Jump && character.CanJump())
        {
            character.Jump(Vector2.up, jumpHash);
        }
        else
        {
            character.Controller.Velocity.y += character.Attributes.Gravity;
            character.Glide(direction);
        }


        return ft;
    }
}
