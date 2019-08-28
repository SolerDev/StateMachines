using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SAir : State
{
    private Vector2 moveVector;
    private Vector2 facingDirection;

    protected readonly int velYHash = Animator.StringToHash("VelY");

    public SAir(Character character) : base(character)
    {
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();
        if (ft!=null)
        {
            return ft;
        }


        if (character.IsGrounded())
        {
            ft = typeof(SIdle);
        }
        else if (character.IsUnderwater())
        {
            ft = typeof(SFloat);
        }
        //else
        //{
        //    character.HorizontalMovement(
        //        character.InputReader.Direction,
        //        character.Attributes.AirSpeed,
        //        ref character.Attributes.SpeedSmoothing,
        //        character.Attributes.AirAcceleration);
        //}

        return ft;
    }
}
