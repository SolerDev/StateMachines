using System;
using System.Collections.Generic;
using UnityEngine;
namespace SebCharCtrl
{
    [RequireComponent(typeof(SebController))]
    [RequireComponent(typeof(StateMachine))]
    public class SebPlayer : MonoBehaviour
    {
        public InputReader Reader;
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

        public Vector2 WallJumpClimb;
        public Vector2 WallJumpOff;
        public Vector2 WallLeap;

        public float WallSlideSpeedMax = 3;
        public float WallStickTime = .25f;
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
            machine = GetComponent<StateMachine>();
            machine.AvailableStates = states;
            attributes = GetComponent<Attributes>();

            gravity = -(2 * attributes.MaxJumpHeight) / Mathf.Pow(attributes.TimeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * attributes.TimeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * attributes.MinJumpHeight);
        }

        private void FixedUpdate()
        {
            //controller.Move(velocity * Time.deltaTime, directionalInput);

            //if (controller.collisions.above || controller.collisions.below)
            //{
            //    if (controller.collisions.slidingDownMaxSlope)
            //    {
            //        velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            //    }
            //    else
            //    {
            //        velocity.y = 0;
            //    }
            //}
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
                    velocity.x = -wallDirX * WallJumpClimb.x;
                    velocity.y = WallJumpClimb.y;
                }
                else if (directionalInput.x == 0)
                {
                    velocity.x = -wallDirX * WallJumpOff.x;
                    velocity.y = WallJumpOff.y;
                }
                else
                {
                    velocity.x = -wallDirX * WallLeap.x;
                    velocity.y = WallLeap.y;
                }
            }
            if (controller.Collisions.below)
            {
                if (controller.Collisions.slidingDownMaxSlope)
                {
                    if (directionalInput.x != -Mathf.Sign(controller.Collisions.slopeNormal.x))
                    { // not jumping against max slope
                        velocity.y = maxJumpVelocity * controller.Collisions.slopeNormal.y;
                        velocity.x = maxJumpVelocity * controller.Collisions.slopeNormal.x;
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
            wallDirX = (controller.Collisions.left) ? -1 : 1;
            wallSliding = false;
            if ((controller.Collisions.left || controller.Collisions.right) && !controller.Collisions.below && velocity.y < 0)
            {
                wallSliding = true;

                if (velocity.y < -WallSlideSpeedMax)
                {
                    velocity.y = -WallSlideSpeedMax;
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
                        timeToWallUnstick = WallStickTime;
                    }
                }
                else
                {
                    timeToWallUnstick = WallStickTime;
                }

            }

        }
    }
}