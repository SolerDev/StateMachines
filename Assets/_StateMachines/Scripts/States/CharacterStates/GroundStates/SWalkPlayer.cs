using System;
using UnityEngine;

public class SWalkPlayer : SGround
{
    #region Properties

    protected new PlayerCharacter character { get => (PlayerCharacter)base.character; set => base.character = value; }
    protected new PlayerReader reader { get => (PlayerReader)base.reader; set => base.reader = value; }

    private Vector2 moveVector = Vector2.zero;
    private Vector2 facingDirection = Vector2.one;

    //voltar daqui e tirar bookmark
    private readonly int dirXHash = Animator.StringToHash("DirX");
    private readonly int walkingHash = Animator.StringToHash("Moving");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    public SWalkPlayer(PlayerCharacter character) : base(character)
    {
    }

    #endregion


    public override void OnEnter()
    {
        base.OnEnter();
        anim.SetBool(walkingHash, true);
    }

    public override void OnExit()
    {
        base.OnExit();
        anim.SetBool(walkingHash, false);
        //anim.SetFloat(velXHash, 0f);
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();
        if (ft != null)
        {
            return ft;
        }

        Vector2 direction = reader.Direction;
        Vector2 moveAmount = direction * attributes.GroundSpeed;

        if (direction.x.Equals(0f))
        {
            ft = typeof(SIdle);
        }
        else if (reader.Dash)
        {
            ft = typeof(SDash);
        }
        else if (character.InputReader.JumpPress && character.CanJump())
        {
            character.Jump(Vector2.up, jumpHash);
        }
        else if (character.IsEdgeFromSide(direction.x))
        {
            ft = typeof(SWallClimb);
        }
        else if (character.IsStepFromSide(direction.x))
        {
            character.Step(direction.x);
        }
        else
        {
            character.Walk(direction.x);
            //anim.SetFloat(velXHash, Mathf.Abs(character.Controller.Velocity.x));
        }
        return ft;
    }

    private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);

            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
    }

    private void DescendSlope(ref Vector2 moveAmount)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(Bounds.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + SkinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(Bounds.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + SkinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }

        if (!collisions.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX.Equals(-1f)) ? Bounds.bottomRight : Bounds.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (!slopeAngle.Equals(0f) && slopeAngle <= attributes.MaxClimbAngle)
                {
                    if (Mathf.Sign(hit.normal.x).Equals(directionX))
                    {
                        if (hit.distance - SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

}
