using System;
using System.Collections.Generic;
using UnityEngine;
namespace SebController
{
    [RequireComponent(typeof(SebController))]
    public class SebPlayer : MonoBehaviour
    {
        public InputReader reader;
        protected Attributes attributes;
        protected SebController controller;

        protected StateMachine machine;
        protected Dictionary<Type, State> states;
        
        protected Animator anim;

        //public float attributes.MaxJumpHeight = 4;
        //public float attributes.MinJumpHeight = 1;
        //public float attributes.TimeToJumpApex = .4f;
        //private float attributes.AirAccelerationTime = .2f;
        //private float attributes.GroundAccelerationTime = .1f;
        //private float Attributes.GroundSpeed = 6;

        public Vector2 wallJumpClimb;
        public Vector2 wallJumpOff;
        public Vector2 wallLeap;

        public float wallSlideSpeedMax = 3;
        public float wallStickTime = .25f;
        private float timeToWallUnstick;
        private float gravity;
        private float maxJumpVelocity;
        private float minJumpVelocity;
        private Vector3 velocity;
        private float velocityXSmoothing;
        private Vector2 directionalInput;
        private bool wallSliding;
        private int wallDirX;

        private void Awake()
        {
            controller = GetComponent<SebController>();
            machine = new StateMachine();
            machine.AvailableStates = states;
            attributes = GetComponent<Attributes>();

            gravity = -(2 * attributes.MaxJumpHeight) / Mathf.Pow(attributes.TimeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * attributes.TimeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * attributes.MinJumpHeight);
        }

        private void FixedUpdate()
        {
            controller.Move(velocity * Time.deltaTime, directionalInput);

            if (controller.collisions.above || controller.collisions.below)
            {
                if (controller.collisions.slidingDownMaxSlope)
                {
                    velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
                }
                else
                {
                    velocity.y = 0;
                }
            }
        }

        public void SetDirectionalInput(Vector2 input)
        {
            directionalInput = input;
        }
        
        public void OnJumpInputDown()
        {
            if (wallSliding)
            {
                if (wallDirX == directionalInput.x)
                {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                else if (directionalInput.x == 0)
                {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else
                {
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            if (controller.collisions.below)
            {
                if (controller.collisions.slidingDownMaxSlope)
                {
                    if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                    { // not jumping against max slope
                        velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                        velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                    }
                }
                else
                {
                    velocity.y = maxJumpVelocity;
                }
            }
        }

        public void OnJumpInputUp()
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }

        private void HandleWallSliding()
        {
            wallDirX = (controller.collisions.left) ? -1 : 1;
            wallSliding = false;
            if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
            {
                wallSliding = true;

                if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }

                if (timeToWallUnstick > 0)
                {
                    velocityXSmoothing = 0;
                    velocity.x = 0;

                    if (directionalInput.x != wallDirX && directionalInput.x != 0)
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else
                    {
                        timeToWallUnstick = wallStickTime;
                    }
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }

            }

        }
    }
}