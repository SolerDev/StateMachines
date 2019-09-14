using UnityEngine;

public static class CharacterExtension
{
    #region Character Verbs

    public static void Walk(this Character character, float direction)
    {
        Vector2 newVelocity = character.Controller.Velocity;
        float velocityDirection = newVelocity.x.Equals(0f) ? 0f : Mathf.Sign(newVelocity.x);
        float inputDirection = direction.Equals(0f) ? 0f : Mathf.Sign(direction);

        if (character.IsEdgeFromSide(inputDirection))
        {
            //character.WallClimb(inputDirection);
            //return;
        }
        else if (character.IsStepFromSide(inputDirection))
        {
            character.Step(inputDirection);
            return;
        }


        newVelocity.x =
            Mathf.SmoothDamp(
            newVelocity.x,
            character.Attributes.GroundSpeed * direction,
            ref character.Attributes.GroundSpeedSmoothing,
            character.Attributes.GroundAccelerationTime);

        if (!character.Controller.Collisions.facingDirection.Equals(inputDirection))
        {
            //face the direction the character is trying to move
            character.Face(inputDirection);
        }

        character.Controller.Velocity = newVelocity;
    }

    public static void Swim(this Character character, Vector2 direction)
    {
        Vector2 newVelocity = character.Controller.Velocity;
        float velocityDirection = newVelocity.x.Equals(0f) ? 0f : Mathf.Sign(newVelocity.x);
        float inputDirection = direction.x.Equals(0f) ? 0f : Mathf.Sign(direction.x);

        newVelocity = Vector2.SmoothDamp(
            newVelocity,
            direction * character.Attributes.WaterSpeed,
            ref character.Attributes.WaterSpeedSmoothingVector,
            character.Attributes.WaterAccelerationTime);

        if (!character.Controller.Collisions.facingDirection.Equals(inputDirection))
        {
            character.Face(inputDirection);
        }
    }

    public static void Glide(this Character character, float direction)
    {
        Vector2 newVelocity = character.Controller.Velocity;
        float velocityDirection = newVelocity.x.Equals(0f) ? 0f : Mathf.Sign(newVelocity.x);
        float inputDirection = direction.Equals(0f) ? 0f : Mathf.Sign(direction);

        if (character.IsWalledFromSide(velocityDirection))
        {
            //Debug.Log("velDir: " + velocityDirection + " newVel: " + newVelocity);
            character.WallGrab(velocityDirection);
            return;
        }


        //glide zerando air speed quando direction=0
        newVelocity.x = Mathf.SmoothDamp(
            newVelocity.x,
            character.Attributes.AirSpeed * direction,
            ref character.Attributes.AirSpeedSmoothing,
            character.Attributes.AirAccelerationTime);

        character.Controller.Velocity = newVelocity;

        if (!character.Controller.Collisions.facingDirection.Equals(inputDirection))
        {
            character.Face(inputDirection);
        }
    }

    public static void Face(this Character character, float direction)
    {
        if (!(direction.Equals(-1f) || direction.Equals(0f) || direction.Equals(1f)))
        { Debug.LogError("The character is trying to face an invalid direction"); }


        if (!direction.Equals(0f))
        {
            character.Controller.Collisions.facingDirection = direction;
            character.Trans.localScale = Vector2.up + Vector2.right * character.Controller.Collisions.facingDirection;
        }
    }

    public static void Step(this Character character, float direction)
    {
        Vector2 rayOrigin = direction.Equals(1f) ? character.Controller.CollBounds.middleRight : character.Controller.CollBounds.middleLeft;
        Vector2 originOffset = Vector2.right * direction * character.Controller.SkinWidth;
        rayOrigin += originOffset;

        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin,
            Vector2.down,
            character.Height() / 2,
            StaticRefs.MASK_FLOOR);

        Vector2 finalPos = hit.point + Vector2.up * (character.Height() / 2 + character.Controller.SkinWidth * 2f);
        character.Trans.position = finalPos;


        #region Debugging

        Color debugColor = hit ? Color.red : Color.yellow;
        Debug.DrawRay(rayOrigin, Vector2.down * character.Height() / 2, debugColor, 2f);

        #endregion


    }

    //public static void WallClimb(this Character character, float direction)
    //{


    //}

    public static void Jump(this Character character, Vector2 direction, int jumpHash)
    {
        //Debug.Log("jumpDirection: " + direction);

        Vector2 newVelocity = character.Controller.Velocity;
        newVelocity.x += direction.x * character.Attributes.MaxJumpVelocity;
        newVelocity.y = direction.y * character.Attributes.MaxJumpVelocity;

        character.Controller.Velocity = newVelocity;
        character.Attributes.JumpsCount++;

        //latter change the way jumpHash is being passed
        character.Anim.SetTrigger(jumpHash);
        character.StateMachine.SwitchToState(typeof(SFall));


    }

    public static void WallJump(this Character character, Vector2 inputDirection, int jumpHash)
    {
        //Debug.Log("wallJumpDirection: " + direction);





        Vector2 newVelocity = character.Controller.Velocity;

        newVelocity.x += inputDirection.x * character.Attributes.MaxJumpVelocity;
        newVelocity.y = inputDirection.y * character.Attributes.MaxJumpVelocity;

        character.Controller.Velocity = newVelocity;
        character.Attributes.JumpsCount++;

        //latter change the way jumpHash is being passed
        character.Anim.SetTrigger(jumpHash);
        character.StateMachine.SwitchToState(typeof(SFall));

    }

    public static void WallGrab(this Character character, float wallSide)
    {
        character.Face(wallSide);
        character.Controller.Velocity = Vector2.zero;

        //state transitions should be handled by the state machine
        character.StateMachine.SwitchToState(typeof(SWallGrab));
    }

    public static void WallSlide(this Character character)
    {
        Vector2 newVelocity = Vector2.zero;

        newVelocity.y = Mathf.SmoothDamp(
            newVelocity.y,
            //negative target because the character must go down similarly to gravity
            -character.Attributes.WallSlideSpeed,
            ref character.Attributes.AirSpeedSmoothing,
            character.Attributes.WallSlideAccelerationTime);

        character.Controller.Velocity = newVelocity;
    }

    #endregion



    #region Character Circunstances

    #region Permision Checks


    public static bool CanMoveUp(this Character character)
    {
        bool canMoveUp = (character.IsUnderwater());
        return canMoveUp;
    }

    public static bool CanJump(this Character character)
    {
        bool canJump =
            character.Attributes.JumpsCount < character.Attributes.JumpAmmount &&
            !character.Controller.Collisions.above;

        return canJump;
    }


    #endregion




    #region Surface Checks

    public static bool IsGrounded(this Character character)
    {
        LayerMask floorMask = StaticRefs.MASK_FLOOR;
        Vector2 leftOrigin = character.Controller.CollBounds.bottomLeft;
        Vector2 rightOrigin = character.Controller.CollBounds.bottomRight;

        RaycastHit2D leftHit = Physics2D.Raycast(
            leftOrigin,
            Vector2.down,
            //character.Attributes.SkinTickness,
            character.Controller.SkinWidth * 2f,
            floorMask);
        RaycastHit2D rightHit = Physics2D.Raycast(
            rightOrigin,
            Vector2.down,
            //character.Attributes.SkinTickness,
            character.Controller.SkinWidth * 2f,
            floorMask);

        //float floorAngle = Vector2.Angle(ControllerColliderHit.nor)


        bool isGrounded = (leftHit || rightHit) ? true : false;

        #region Debuging

        Color leftColor = leftHit ? Color.red : Color.green;
        Color rightColor = rightHit ? Color.red : Color.green;

        //Debug.DrawRay(leftOrigin, Vector2.down * character.Attributes.SkinTickness, leftColor, .01f);
        //Debug.DrawRay(rightOrigin, Vector2.down * character.Attributes.SkinTickness, rightColor, .01f);
        Debug.DrawRay(leftOrigin, Vector2.down * character.Controller.SkinWidth, leftColor, .01f);
        Debug.DrawRay(rightOrigin, Vector2.down * character.Controller.SkinWidth, rightColor, .01f);

        #endregion

        //bool isGrounded = character.Controller.Collisions.bellow;
        return isGrounded;
    }

    /// <summary>
    ///Returns Vector2.up if the character isn't touching any surfaces    /// </summary>
    /// <param name="collisionPoint">Represents the collision point to check</param>
    //public static Vector2 SurfaceNormal(this Character character)
    //{
    //    RaycastHit2D hit = Physics2D.Raycast(
    //        character.Trans.position,
    //        -character.Trans.up,
    //        character.Height() / 2 + character.Controller.SkinWidth,
    //        StaticRefs.MASK_FLOOR);

    //    Vector2 r = hit ? hit.normal : Vector2.up;
    //    return r;
    //}

    #endregion




    #region Water Checks

    public static bool IsUnderwater(this Character character)
    {
        //the character is considered underwater if
        //there is water above OR
        //there is water bellow AND the water isn't shallow

        RaycastHit2D hit = Physics2D.Raycast(
            character.CurrentColl.bounds.center,
            Vector2.up,
            //character.Attributes.SkinTickness,
            character.Controller.SkinWidth,
            StaticRefs.MASK_WATER);

        bool isWaterSide = hit;

        bool isUnderwater = (character.IsWaterAbove() || isWaterSide);

        return isUnderwater;
    }

    public static bool IsWaterAbove(this Character character)
    {
        bool waterAbove = Physics2D.Raycast(
            character.CurrentColl.bounds.center,
            Vector2.up,
            character.Height() / 2 + character.Controller.SkinWidth,
            StaticRefs.MASK_WATER);

        return waterAbove;
    }

    public static bool IsWaterBellow(this Character character)
    {
        bool waterBellow = Physics2D.Raycast(
            character.CurrentColl.bounds.center,
            Vector2.down,
            character.Height() / 2 + character.Controller.SkinWidth,
            StaticRefs.MASK_WATER);

        return waterBellow;
    }

    public static bool IsWaterShallow(this Character character)
    {
        bool waterShallow = Physics2D.Raycast(
            character.CurrentColl.bounds.center,
            Vector2.up,
            character.Controller.SkinWidth,
            StaticRefs.MASK_WATER);

        return waterShallow;
    }

    #endregion




    #region Wall, Step and Edge Checks

    public static bool IsWalledFromSide(this Character character, float direction)
    {
        bool topCollided = character.SideTopCollided(direction);
        bool midCollided = character.SideMiddleCollided(direction);


        bool isWalledFromSide = (topCollided && midCollided);
        return isWalledFromSide;
    }

    public static bool IsStepFromSide(this Character character, float direction)
    {
        bool topCollided = character.SideTopCollided(direction);
        bool midCollided = character.SideMiddleCollided(direction);
        bool kneeCollided = character.SideKneeCollided(direction);
        bool botCollided = character.SideBottomCollided(direction);


        bool isStepFromSide = (!topCollided && !midCollided && !kneeCollided && botCollided);
        return isStepFromSide;
    }

    public static bool IsEdgeFromSide(this Character character, float direction)
    {
        bool topCollided = character.SideTopCollided(direction);
        bool midCollided = character.SideMiddleCollided(direction);


        bool isEdgeFromSide = (!topCollided && midCollided);
        return isEdgeFromSide;
    }

    #endregion




    #region SideChecks

    /// <summary>
    /// Checks if the character is colliding with a wall from a specific point on a specified side.   	
    /// </summary>
    /// <param name="collisionPoint">Represents the collision point to check</param>
    /// <param name="side">LEFT = -1. RIGHT = 1 </param>
    public static bool SideCollidedAtPoint(this Character character, Vector2 collisionPoint, float side)
    {
        if (side.Equals(0f))
        { return false; }

        //Debug.Log("side collided at point: " + side);

        side = Mathf.Sign(side);

        //voltar daqui
        Vector2 checkDirection = new Vector2(side, 0);

        //checkDirection = Vector2.right * side;
        //checkDirection.x = side;
        //checkDirection = side.Equals(1f) ? Vector2.right : Vector2.left;

        //Debug.Log(checkDirection);

        RaycastHit2D hit = Physics2D.Raycast(
            collisionPoint,
            checkDirection,
            //hardcoded
            //reason: accounting for cases in which the wall isn't perfectly vertical
            character.Controller.SkinWidth * 2f,
            StaticRefs.MASK_FLOOR);
        bool sideCollidedAtPoint = hit;


        #region Debuging


        Color rayColor = hit ? Color.red : Color.green;
        Debug.DrawRay(collisionPoint, checkDirection * character.Controller.SkinWidth);


        #endregion



        return sideCollidedAtPoint;
    }

    public static bool SideTopCollided(this Character character, float side)
    {
        Vector2 collisionPoint = side.Equals(1f) ? character.Controller.CollBounds.topRight : character.Controller.CollBounds.topLeft;
        bool sideCollided = character.SideCollidedAtPoint(collisionPoint, side);

        //Debug.Log(character + " collided Top: " + sideCollided);

        return sideCollided;
    }

    public static bool SideMiddleCollided(this Character character, float side)
    {
        Vector2 collisionPoint = side.Equals(1f) ? character.Controller.CollBounds.middleRight : character.Controller.CollBounds.middleLeft;
        bool sideCollided = character.SideCollidedAtPoint(collisionPoint, side);

        //Debug.Log(character + " collided Middle: " +sideCollided);

        return sideCollided;
    }

    public static bool SideKneeCollided(this Character character, float side)
    {
        Vector2 collisionPoint = side.Equals(1f) ? character.Controller.CollBounds.middleRight : character.Controller.CollBounds.middleLeft;
        collisionPoint.y -= character.Height() / 4;
        bool sideCollided = character.SideCollidedAtPoint(collisionPoint, side);

        //Debug.Log(character + " collided Knee: " + sideCollided);

        return sideCollided;
    }

    public static bool SideBottomCollided(this Character character, float side)
    {
        Vector2 collisionPoint = side.Equals(1f) ? character.Controller.CollBounds.bottomRight : character.Controller.CollBounds.bottomLeft;
        bool sideCollided = character.SideCollidedAtPoint(collisionPoint, side);

        //Debug.Log(character + " collided Bot: " + sideCollided);

        return sideCollided;
    }

    #endregion


    #endregion


    #region Character Usefull Info

    public static float Height(this Character character)
    {
        return character.CurrentColl.size.y;
    }

    public static float Width(this Character character)
    {
        return character.CurrentColl.size.x;
    }

    #endregion
}
