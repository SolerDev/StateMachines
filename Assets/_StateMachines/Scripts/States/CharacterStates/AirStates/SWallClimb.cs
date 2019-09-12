using System;
using UnityEngine;


public class SWallClimb : State
{
    private Vector2 wallJumpDirection = Vector2.zero;
    private float wallSide;
    private Vector2 targetPosition;

    private float climbTime;
    private float climbDuration;

    private readonly int jumpHash = Animator.StringToHash("Jump");
    private readonly int climbHash = Animator.StringToHash("Climb");

    public SWallClimb(Character character) : base(character)
    {
        //hardcoded
        climbDuration = 1f;

    }

    public override void OnEnter()
    {
        base.OnEnter();

        //Debug.Log("Wall Climbing ON");

        climbTime = 0f;

        character.Controller.Velocity = Vector2.zero;

        //get wall side
        wallSide = Physics2D.Raycast(
             character.Controller.CollBounds.middle,
             Vector2.left,
             character.Width() / 2 + character.Controller.SkinWidth,
             StaticRefs.MASK_GROUND) ?
             -1 : 1;

        wallJumpDirection.x = character.Attributes.wallJumpProportions.x;
        wallJumpDirection.y = character.Attributes.wallJumpProportions.y;
        wallJumpDirection.x *= -wallSide;

        //get target Position
        //hardcoded
        RaycastHit2D hit;
        Vector2 rayOrigin = wallSide.Equals(-1f) ?
            character.Controller.CollBounds.topLeft - Vector2.right * character.Controller.SkinWidth * 2.3f :
            character.Controller.CollBounds.topRight + Vector2.right * character.Controller.SkinWidth * 2.3f;
        hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, StaticRefs.MASK_FLOOR);
        targetPosition = hit.point + Vector2.up * character.Height() / 2;

        anim.SetTrigger(climbHash);


        if (targetPosition.Equals(Vector2.zero))
        { Debug.LogError("No targetPosition found. " + character + " can't Climb the Wall"); }

        //Debug.Log("rayOrigin: " + rayOrigin);
        Debug.DrawRay(rayOrigin, Vector2.down * hit.distance, Color.red, 1f);

    }

    public override void OnExit()
    {
        base.OnExit();

        //Debug.Log("Wall Climbing OFF");
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();

        if (ft != null)
        {
            return ft;
        }

        if (!character.Controller.Collisions.above && character.InputReader.Jump)
        {
            character.Attributes.JumpsCount--;
            character.Jump(wallJumpDirection, jumpHash);
        }
        else
        {
            //move character to target position
            climbTime += Time.fixedDeltaTime;
            if (climbTime > climbDuration)
            {
                Climb();
                ft = typeof(SIdle);
            }
        }
        return ft;
    }

    private void Climb()
    {
        Vector2 oldPos = character.Trans.position;
        character.Trans.position = targetPosition;
        Vector2 newPos = character.Trans.position;

        Vector2 translationRay = newPos - oldPos;
        Debug.DrawRay(oldPos, translationRay, Color.cyan, 5f);
    }
}
