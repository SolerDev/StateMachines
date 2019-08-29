using System;
using UnityEngine;

public class SWallGrab : SAir
{
    private float wallSide;
    private Vector2 wallJumpDirection = Vector2.zero;
    private float timeInState = 0f;

    private readonly int climbingHash = Animator.StringToHash("Climbing");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    //muito similar ao SWallSlide. Talvez devesse criar uma classe antes de ambos chama SWall
    public SWallGrab(Character character) : base(character)
    {
        wallJumpDirection.x = character.Attributes.wallJumpProportions.x;
        wallJumpDirection.y = character.Attributes.wallJumpProportions.y;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        timeInState = 0f;

        wallSide = Physics2D.Raycast(
            character.Controller.CollBounds.middle,
            Vector2.left,
            character.Width() / 2 + character.Controller.SkinWidth,
            StaticRefs.MASK_GROUND) ?
            -1 : 1;

        wallJumpDirection.x *= -wallSide;

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


        if (character.InputReader.Jump && !character.Controller.Collisions.above)
        {
            character.Attributes.JumpsCount--;
            character.Jump(wallJumpDirection, jumpHash);
        }
        else if (
            character.InputReader.DirY < 0f ||
            character.InputReader.DirX * wallSide < 0f)
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
