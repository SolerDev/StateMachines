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

        Vector2 direction = character.InputReader.Direction;

        if (character.InputReader.Jump && character.CanJump())
        {
            character.Jump(Vector2.up, jumpHash);
        }
        else
        {
            //apply gravity
            character.Controller.Velocity.y += character.Attributes.Gravity;
            character.Glide(direction);
            anim.SetFloat(velYHash, character.Controller.Velocity.y);
        }
        return ft;
    }
}
