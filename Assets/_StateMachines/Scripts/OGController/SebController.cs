using System.Collections;
using UnityEngine;

namespace SebCharCtrl
{
    //[RequireComponent(typeof(SebPlayer))]
    [RequireComponent(typeof(StateMachine))]
    public class SebController : SebRaycast
    {
        private Transform trans;
        private Attributes attributes;

        public CollisionInfo Collisions;
        //public Vector2 Velocity;
        public Vector2 Velocity;
        //[HideInInspector]
        //public Vector2 directionalInput;

        private readonly WaitForSeconds resetPlatformWait = new WaitForSeconds(.5f);

        protected override void Awake()
        {
            base.Awake();
            attributes = GetComponent<Attributes>();
            trans = transform;
            Collisions.faceDir = 1;
        }

        private void FixedUpdate()
        {
            Move(Velocity * Time.deltaTime);
        }

        //old platform Move
        //public void Move(Vector2 moveAmount, bool standingOnPlatform)
        //{
        //    Move(moveAmount, standingOnPlatform);
        //}

        public void Move(Vector2 moveAmount/*, bool standingOnPlatform = false*/)
        {
            UpdateColliderBounds();
            Collisions.Reset();

            Collisions.moveAmountOld = moveAmount;
            //directionalInput = input;




            if (!moveAmount.x.Equals(0f))
            {
                Collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
            }
            HorizontalCollisions(ref moveAmount);


            if (!moveAmount.y.Equals(0f))
            {
                VerticalCollisions(ref moveAmount);
            }

            //Debug.Log(moveAmount);
            trans.Translate(moveAmount);

            //if (standingOnPlatform)
            //{
            //    collisions.below = true;
            //}
        }

        private void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = Collisions.faceDir;
            float rayLength = Mathf.Abs(moveAmount.x) + SkinWidth;

            if (Mathf.Abs(moveAmount.x) < SkinWidth)
            {
                rayLength = 2 * SkinWidth;
            }

            for (int i = 0; i < horRayCount; i++)
            {
                Vector2 rayOrigin = (directionX.Equals(-1)) ? CollBounds.bottomLeft : CollBounds.bottomRight;
                rayOrigin += Vector2.up * (horRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

                if (hit)
                {
                    if (hit.distance.Equals(0f))
                    {
                        continue;
                    }

                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (i.Equals(0f) && slopeAngle <= attributes.MaxClimbAngle)
                    {
                        if (Collisions.descendingSlope)
                        {
                            Collisions.descendingSlope = false;
                            moveAmount = Collisions.moveAmountOld;
                        }
                        float distanceToSlopeStart = 0;
                        if (!slopeAngle.Equals(Collisions.slopeAngleOld))
                        {
                            distanceToSlopeStart = hit.distance - SkinWidth;
                            moveAmount.x -= distanceToSlopeStart * directionX;
                        }
                        Debug.Log("Should be Climbing Slope");
                        //ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                        moveAmount.x += distanceToSlopeStart * directionX;
                    }

                    if (!Collisions.climbingSlope || slopeAngle > attributes.MaxClimbAngle)
                    {
                        moveAmount.x = (hit.distance - SkinWidth) * directionX;
                        rayLength = hit.distance;

                        if (Collisions.climbingSlope)
                        {
                            moveAmount.y = Mathf.Tan(Collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                        }

                        Collisions.left = directionX.Equals(-1f);
                        Collisions.right = directionX.Equals(1f);
                    }
                }
            }
        }

        private void VerticalCollisions(ref Vector2 moveAmount)
        {
            float directionY = Mathf.Sign(moveAmount.y);
            float rayLength = Mathf.Abs(moveAmount.y) + SkinWidth;

            for (int i = 0; i < verRayCount; i++)
            {

                Vector2 rayOrigin = directionY.Equals(-1f) ? CollBounds.bottomLeft : CollBounds.topLeft;
                rayOrigin += Vector2.right * (verRaySpacing * i + moveAmount.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

                if (hit)
                {
                    if (hit.collider.CompareTag(StaticRefs.TAG_THROUGH))
                    {
                        if (directionY.Equals(1f) || hit.distance.Equals(0f))
                        {
                            continue;
                        }
                        if (Collisions.fallingThroughPlatform)
                        {
                            continue;
                        }
                        //if (directionalInput.y.Equals(-1f))
                        //{
                        //    collisions.fallingThroughPlatform = true;
                        //    StartCoroutine(ResetFallingThroughPlatform());
                        //    continue;
                        //}
                    }

                    moveAmount.y = (hit.distance - SkinWidth) * directionY;
                    //Debug.Log($"{hit.distance} {SkinWidth} {directionY}");
                    rayLength = hit.distance;

                    if (Collisions.climbingSlope)
                    {
                        moveAmount.x = moveAmount.y / Mathf.Tan(Collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                    }

                    Collisions.below = directionY == -1;
                    Collisions.above = directionY == 1;
                }
            }

            if (Collisions.climbingSlope)
            {
                float directionX = Mathf.Sign(moveAmount.x);
                rayLength = Mathf.Abs(moveAmount.x) + SkinWidth;
                Vector2 rayOrigin = ((directionX.Equals(-1f)) ? CollBounds.bottomLeft : CollBounds.bottomRight) + Vector2.up * moveAmount.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (!slopeAngle.Equals(Collisions.slopeAngle))
                    {
                        moveAmount.x = (hit.distance - SkinWidth) * directionX;
                        Collisions.slopeAngle = slopeAngle;
                        Collisions.slopeNormal = hit.normal;
                    }
                }
            }
        }

        #region Controller Verbs

        //private void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
        //{
        //    if (hit)
        //    {
        //        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        //        if (slopeAngle > attributes.MaxClimbAngle)
        //        {
        //            moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

        //            collisions.slopeAngle = slopeAngle;
        //            collisions.slidingDownMaxSlope = true;
        //            collisions.slopeNormal = hit.normal;
        //        }
        //    }

        //}

        public IEnumerator ResetFallingThroughPlatform()
        {
            yield return resetPlatformWait;
            Collisions.fallingThroughPlatform = false;
        }

        #endregion

        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;

            public bool climbingSlope;
            public bool descendingSlope;
            public bool slidingDownMaxSlope;

            public float slopeAngle, slopeAngleOld;
            public Vector2 slopeNormal;
            public Vector2 moveAmountOld;
            public float faceDir;
            public bool fallingThroughPlatform;

            public void Reset()
            {
                above = below = false;
                left = right = false;
                climbingSlope = false;
                descendingSlope = false;
                slidingDownMaxSlope = false;
                slopeNormal = Vector2.zero;

                slopeAngleOld = slopeAngle;
                slopeAngle = 0;
            }
        }
    }
}