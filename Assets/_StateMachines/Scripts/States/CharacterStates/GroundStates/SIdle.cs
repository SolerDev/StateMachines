using System;
using UnityEngine;

public class SIdle : SGround
{
    private readonly int jumpHash = Animator.StringToHash("Jump");

    public SIdle(Character character) : base(character)
    {
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();
        if (ft != null)
        {
            return ft;
        }
        
        if (!character.InputReader.DirX.Equals(0))
        {
            ft = typeof(SWalkPlayer);
        }
        else if (character.InputReader.Jump && character.CanJump())
        {
            character.Jump(Vector2.up, jumpHash);
        }
        else
        {

            //if (character.Controller.Velocity.x.Equals(0f))
            //{
            //    character.Walk(Vector2.zero);
            //}
            character.Walk(0f);
        }

        return ft;
    }
}
