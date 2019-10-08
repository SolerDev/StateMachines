using System;
using UnityEngine;

public class SWalkPlayer : SGround
{
    #region Properties

    //name violation so I can hid the base property
    protected new PlayerCharacter character { get => (PlayerCharacter)base.character; set => base.character = value; }
    protected new PlayerReader inputReader { get => (PlayerReader)base.inputReader; set => base.inputReader = value; }

    private Vector2 moveAmount = Vector2.zero;
    private Vector2 facingDirection = Vector2.one;

    private readonly int velXHash = Animator.StringToHash("VelX");
    private readonly int jumpHash = Animator.StringToHash("Jump");

    public SWalkPlayer(PlayerCharacter character) : base(character)
    {
    }

    #endregion

    public override void OnExit()
    {
        base.OnExit();
        anim.SetFloat(velXHash, 0);

        //anim.SetFloat(velXHash, 0f);
    }

    public override Type FixedTick()
    {
        ft = base.FixedTick();
        if (ft != null)
        {
            return ft;
        }

        Vector2 direction = inputReader.Direction;
        int inputDirectionX = direction.x.Equals(0f) ? 0 : (int)Mathf.Sign(direction.x);

        if (direction.x.Equals(0f))
        {
            ft = typeof(SIdle);
        }
        else if (inputReader.Dash)
        {
            ft = typeof(SDash);
        }
        else if (direction.y.Equals(-1f))
        {
            ft = typeof(SCrouch);
        }
        else if (character.InputReader.JumpPress && character.CanJump())
        {
            character.Jump(Vector2.up, jumpHash);
        }
        else if (character.IsEdgeFromSide(direction.x))
        {
            ft = typeof(SWallClimb);
        }
        //else if (character.IsStepFromSide(direction.x))
        //{
        //    character.Step(direction.x);
        //}
        else
        {
            ft = Walk(inputDirectionX, ref controller.Velocity);
            anim.SetFloat(velXHash, Mathf.Abs(controller.Velocity.x));

            //Debug.Log(controller.Velocity);
        }
        return ft;
    }

    private Type Walk(int inputDirection, ref Vector2 velocity)
    {
        float velocityDirection = velocity.x.Equals(0f) ? 0f : Mathf.Sign(velocity.x);
        if (character.IsEdgeFromSide(inputDirection))
        {
            //character.WallClimb(inputDirection);
            return typeof(SWallClimb);
        }
        else if (character.IsStepFromSide(inputDirection))
        {
            character.Step(inputDirection);
            return null; //step doe'snt have a type, so just return the same current Type
        }

        velocity.x =
            Mathf.SmoothDamp(
            velocity.x,
            character.Attributes.GroundSpeed * inputDirection,
            ref character.Attributes.GroundSpeedSmoothing,
            character.Attributes.GroundAccelerationTime);

        //if the direction the character is facing is different from the one he's pressing
        if (!character.Controller.Collisions.faceDir.Equals(inputDirection))
        {
            //face the direction the character is trying to move
            character.Face(inputDirection);
        }

        if (velocity.y < 0f)
        {
            DescendSlope(ref velocity);
        }


        character.Controller.Velocity = velocity;

        return typeof(SWalkPlayer);
    }

    private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);

            controller.Collisions.below = true;
            controller.Collisions.climbingSlope = true;
            controller.Collisions.slopeAngle = slopeAngle;
            controller.Collisions.slopeNormal = slopeNormal;
        }
    }

    private void DescendSlope(ref Vector2 moveAmount)
    {
        //voltar daqui
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(controller.CollBounds.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + controller.SkinWidth, StaticRefs.MASK_FLOOR);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(controller.CollBounds.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + controller.SkinWidth, StaticRefs.MASK_FLOOR);
        //if (maxSlopeHitLeft ^ maxSlopeHitRight)
        //{
        //    SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
        //    SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        //}

        if (!controller.Collisions.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX.Equals(-1f)) ? controller.CollBounds.bottomRight : controller.CollBounds.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, StaticRefs.MASK_FLOOR);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (!slopeAngle.Equals(0f) && slopeAngle <= attributes.MaxClimbAngle)
                {
                    if (Mathf.Sign(hit.normal.x).Equals(directionX))
                    {
                        if (hit.distance - controller.SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            controller.Collisions.slopeAngle = slopeAngle;
                            controller.Collisions.descendingSlope = true;
                            controller.Collisions.below = true;
                            controller.Collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    private void Walk(float direction)
    {
    }
}
