﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D: RaycastController2D
{
    private Character character;

    public CharacterController2D(Character character)
    {
        this.Enabled= true;

        this.character = character;
        this.trans = character.Trans;
        this.currentColl = character.CurrentColl;

        SetRaySpacing(currentColl.bounds);
    }

    private void UpdatePhysics()
    {
        //gravity
        //Velocity.y += character.Attributes.Gravity;

        if (collisions.above || collisions.bellow)
        {
            //Debug.Log("velocity = " + Velocity);
            Velocity.y = 0f;
            //UnityEditor.EditorApplication.isPaused = true;
            //Debug.Log("velocity = " + Velocity);
        }

        //reset all collisions after all of it's consequences have been applied
        collisions.Reset();

        if (!Velocity.x.Equals(0f)) { HorizontalCollisions(ref Velocity); }
        if (!Velocity.y.Equals(0f)) { VerticalCollisions(ref Velocity); }
    }

    public void Move()
    {
        if (!Enabled) { return; }

        collisions.velocityOld = Velocity;

        UpdateColliderBounds();
        UpdatePhysics();

        if (Velocity.y<0f)
        {
            DescendSlope(ref Velocity);
        }

        trans.Translate(Velocity);
    }

    private void VerticalCollisions(ref Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLenght = Mathf.Abs(velocity.y) + SkinWidth;


        for (int i = 0; i < VerRayCount; i++)
        {
            Vector2 rayOrigin = directionY.Equals(-1f) ? CollBounds.bottomLeft : CollBounds.topLeft;
            rayOrigin.x += verRaySpacing * i + velocity.x;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLenght, StaticRefs.MASK_PALPABLE);
            if (hit)
            {
                velocity.y = (hit.distance - SkinWidth) * directionY;
                rayLenght = hit.distance;

                //HANDLE SLOPES
                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.bellow = directionY.Equals(-1f);
                collisions.above = directionY.Equals(1f);
            }

            Color debugColor = hit ? Color.red : Color.green;
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLenght, debugColor);
        }

        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLenght = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = (directionX.Equals(-1) ? CollBounds.bottomLeft : CollBounds.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, StaticRefs.MASK_PALPABLE);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (!slopeAngle.Equals(collisions.slopeAngle))
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    private void HorizontalCollisions(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLenght = Mathf.Abs(velocity.x) + SkinWidth;


        for (int i = 0; i < HorRayCount; i++)
        {
            Vector2 rayOrigin = directionX.Equals(-1f) ? CollBounds.bottomLeft : CollBounds.bottomRight;
            rayOrigin.y += horRaySpacing * i;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLenght, StaticRefs.MASK_PALPABLE);
            if (hit)
            {
                //HANDLE SLOPES
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i.Equals(0) && slopeAngle < character.Attributes.MaxClimbAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        //handle transitions between descending and rising slopes
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceToSlope = 0f;
                    if (!slopeAngle.Equals(collisions.slopeAngleOld))
                    {
                        distanceToSlope = hit.distance - skinWidth;
                        velocity.x -= distanceToSlope * directionX;
                    }
                    ClimbSlope(ref Velocity, slopeAngle);
                    velocity.x += distanceToSlope * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > character.Attributes.MaxClimbAngle)
                {
                    velocity.x = (hit.distance - SkinWidth) * directionX;
                    rayLenght = hit.distance;


                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad * Mathf.Abs(velocity.x));
                    }

                    collisions.left = directionX.Equals(-1f);
                    collisions.right = directionX.Equals(1f);
                }
            }

            Color debugColor = hit ? Color.red : Color.green;
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLenght, debugColor);
        }
    }

    private void ClimbSlope(ref Vector2 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.bellow = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    private void DescendSlope(ref Vector2 velocity)
    {
        float direciontX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = direciontX.Equals(-1) ? CollBounds.bottomRight : CollBounds.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin,
            Vector2.down,
            Mathf.Infinity,
            StaticRefs.MASK_PALPABLE);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (!slopeAngle.Equals(0f) && slopeAngle<=character.Attributes.MaxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x).Equals(direciontX))
                {
                    if (hit.distance-skinWidth<=Mathf.Tan(slopeAngle*Mathf.Deg2Rad)*Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.bellow = true;
                    }
                }
            }
        }
    }
}
