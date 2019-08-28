using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWallSlide : SAir
{
    private float wallSide;
    private Vector2 wallJumpDirection;

    private readonly int slidingHash = Animator.StringToHash("Sliding");

    public SWallSlide(Character character) : base(character)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        //wallSide = character.Controller.Collisions.left ? -1 : 1;

        wallSide = Physics2D.Raycast(
            character.Controller.CollBounds.middle,
            Vector2.left,
            character.Width() / 2 + character.Controller.SkinWidth,
            StaticRefs.MASK_GROUND) ?
            -1 : 1;



        //proportions must add to One
        //hardcoded
        wallJumpDirection = new Vector2(-wallSide * .3f, .7f);
        //Debug.Log(wallJumpDirection);

        anim.SetBool(slidingHash, true);
    }

    public override void OnExit()
    {
        base.OnExit();

        anim.SetBool(slidingHash, false);
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
        else if (character.InputReader.DirY < 0f || !character.IsWalledFromSide(wallSide) || (character.InputReader.DirX + wallSide).Equals(0f))
        {
            ft = typeof(SFall);
        }
        else
        {
            character.WallSlide();
        }

        return ft;
    }
}
