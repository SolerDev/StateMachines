using System;
using UnityEngine;

public class SIdle : SGround
{
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
            character.Jump(Vector2.up);
        }
        else
        {

            //if (character.Controller.Velocity.x.Equals(0f))
            //{
            //    character.Walk(Vector2.zero);
            //}
            character.Walk(Vector2.zero);
        }

        return ft;
    }
}
