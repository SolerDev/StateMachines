using System;
using UnityEngine;

public class SFall : SAir
{
    private readonly int jumpHash = Animator.StringToHash("Jump");

    public SFall(Character character) : base(character)
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

        if (character.Reader.JumpPress && character.CanJump())
        {
            character.Jump(Vector2.up, jumpHash);
        }
        else if (character.IsEdgeFromSide(direction.x))
        {
            ft = typeof(SWallClimb);
        }
        else
        {
            //apply gravity
            character.Controller.Velocity.y += character.Attributes.Gravity;

            //update jump velocity if the player releases the jumpKey
            if (character.Reader.JumpRelease && character.Controller.Velocity.y > character.Attributes.MinJumpVelocity)
            {
                character.Controller.Velocity.y = character.Attributes.MinJumpVelocity;
            }


            character.Glide(direction.x);
            anim.SetFloat(velYHash, character.Controller.Velocity.y);
        }
        return ft;
    }
}
