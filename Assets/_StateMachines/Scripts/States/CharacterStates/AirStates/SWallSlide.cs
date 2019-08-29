using System;
using UnityEngine;

public class SWallSlide : SAir
{
    private float wallSide;
    private Vector2 wallJumpDirection = Vector2.zero;

    private readonly int slidingHash = Animator.StringToHash("Sliding");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    //muito similar ao SWallGrab. Talvez devesse criar uma classe antes de ambos chama SWall
    public SWallSlide(Character character) : base(character)
    {
        wallJumpDirection.x = character.Attributes.wallJumpProportions.x;
        wallJumpDirection.y = character.Attributes.wallJumpProportions.y;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        wallSide = Physics2D.Raycast(
            character.Controller.CollBounds.middle,
            Vector2.left,
            character.Width() / 2 + character.Controller.SkinWidth,
            StaticRefs.MASK_GROUND) ?
            -1 : 1;

        wallJumpDirection.x *= -wallSide;

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

        if (character.InputReader.Jump && !character.Controller.Collisions.above)
        {
            character.Attributes.JumpsCount--;
            character.Jump(wallJumpDirection, jumpHash);
        }
        else if (
            character.InputReader.DirY < 0f ||
            character.InputReader.DirX * wallSide < 0f ||
            !character.IsWalledFromSide(wallSide))
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
