using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWallGrab : SAir
{
    private float wallSide;
    private Vector2 wallJumpDirection;
    private float timeInState = 0f;

    private readonly int climbingHash = Animator.StringToHash("Climbing");

    public SWallGrab(Character character) : base(character)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        timeInState = 0f;

        //wallSide = character.Controller.Collisions.left ? -1 : 1;

        wallSide = Physics2D.Raycast(
            character.Controller.CollBounds.middle,
            Vector2.left,
            character.Width() / 2 + character.Controller.SkinWidth,
            StaticRefs.MASK_GROUND) ?
            -1 : 1;
        //Debug.Log("Wall side: " + wallSide);

        //hardcoded
        wallJumpDirection = new Vector2(-wallSide * .3f, .7f);


        anim.SetBool(climbingHash, true);
    }

    public override void OnExit()
    {
        base.OnExit();

        anim.SetBool(climbingHash, false);
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();
        if (ft != null)
        {
            return ft;
        }


        if (character.InputReader.Jump && character.CanJump())
        {
            character.Jump(wallJumpDirection);
            //Debug.Log("Wall Jump Direction: " + wallJumpDirection);
            character.Attributes.JumpsCount--;
        }
        else if(character.InputReader.DirY<0f /*|| (character.InputReader.DirX + wallSide).Equals(0f)*/)
        {
            ft = typeof(SFall);
        }
        else
        {
            timeInState += Time.fixedDeltaTime;

            if (timeInState > character.Attributes.WallGrabTimeLimit)
            {
                ft = typeof(SWallSlide);
            }
        }

        return ft;
    }
}
