using System;
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

        Vector2 direction = character.Reader.Direction;

        if (character.Controller.Velocity.y < 0f)
        {
            ft = typeof(SFall);
        }
        else if (character.Reader.JumpPress && character.CanJump())
        {
            character.Jump(Vector2.up, jumpHash);
        }
        else
        {
            character.Controller.Velocity.y += character.Attributes.Gravity;


            character.Glide(direction.x);
        }


        return ft;
    }
}
