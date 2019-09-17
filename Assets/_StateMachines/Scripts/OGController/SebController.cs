using System.Collections;
using UnityEngine;

namespace SebController
{
    public class SebController : SebRaycast
    {
        private Transform trans;
        private Attributes attributes;

        public CollisionInfo collisions;
        [HideInInspector]
        public Vector2 directionalInput;

        private WaitForSeconds resetPlatformWait = new WaitForSeconds(.5f);

        protected override void Awake()
        {
            base.Awake();
            attributes = GetComponent<Attributes>();
            trans = transform;
            collisions.faceDir = 1;
        }

        public void Move(Vector2 moveAmount, bool standingOnPlatform)
        {
            Move(moveAmount, Vector2.zero, standingOnPlatform);
        }

        public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
        {
            UpdateColliderBounds();

            collisions.Reset();
            collisions.moveAmountOld = moveAmount;
            directionalInput = input;

            if (moveAmount.y < 0)
            {
                DescendSlope(ref moveAmount);
            }

            if (!moveAmount.x.Equals(0f))
            {
                collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
            }

            HorizontalCollisions(ref moveAmount);
            if (!moveAmount.y.Equals(0f))
            {
                VerticalCollisions(ref moveAmount);
            }

            trans.Translate(moveAmount);

            if (standingOnPlatform)
            {
                collisions.below = true;
            }
        }

        private void HorizontalCollisions(ref Vector2 moveAmount)
        {
            float directionX = collisions.faceDir;
            float rayLength = Mathf.Abs(moveAmount.x) + SkinWidth;

            if (Mathf.Abs(moveAmount.x) < SkinWidth)
            {
                rayLength = 2 * SkinWidth;
            }

            for (int i = 0; i < horRayCount; i++)
            {
                Vector2 rayOrigin = (directionX.Equals(-1)) ? Bounds.bottomLeft : Bounds.bottomRight;
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
                        if (collisions.descendingSlope)
                        {
                            collisions.descendingSlope = false;
                            moveAmount = collisions.moveAmountOld;
                        }
                        float distanceToSlopeStart = 0;
                        if (!slopeAngle.Equals(collisions.slopeAngleOld))
                        {
                            distanceToSlopeStart = hit.distance - SkinWidth;
                            moveAmount.x -= distanceToSlopeStart * directionX;
                        }
                        ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                        moveAmount.x += distanceToSlopeStart * directionX;
                    }

                    if (!collisions.climbingSlope || slopeAngle > attributes.MaxClimbAngle)
                    {
                        moveAmount.x = (hit.distance - SkinWidth) * directionX;
                        rayLength = hit.distance;

                        if (collisions.climbingSlope)
                        {
                            moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                        }

                        collisions.left = directionX.Equals(-1f);
                        collisions.right = directionX.Equals(1f);
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

                Vector2 rayOrigin = directionY.Equals(-1f) ? Bounds.bottomLeft : Bounds.topLeft;
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
                        if (collisions.fallingThroughPlatform)
                        {
                            continue;
                        }
                        if (directionalInput.y.Equals(-1f))
                        {
                            collisions.fallingThroughPlatform = true;
                            StartCoroutine(ResetFallingThroughPlatform());
                            continue;
                        }
                    }

                    moveAmount.y = (hit.distance - SkinWidth) * directionY;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                    }

                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
            }

            if (collisions.climbingSlope)
            {
                float directionX = Mathf.Sign(moveAmount.x);
                rayLength = Mathf.Abs(moveAmount.x) + SkinWidth;
                Vector2 rayOrigin = ((directionX.Equals(-1f)) ? Bounds.bottomLeft : Bounds.bottomRight) + Vector2.up * moveAmount.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

                if (hit)
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (!slopeAngle.Equals(collisions.slopeAngle))
                    {
                        moveAmount.x = (hit.distance - SkinWidth) * directionX;
                        collisions.slopeAngle = slopeAngle;
                        collisions.slopeNormal = hit.normal;
                    }
                }
            }
        }

        #region Controller Verbs

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

        private void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
        {
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle > attributes.MaxClimbAngle)
                {
                    moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                    collisions.slopeAngle = slopeAngle;
                    collisions.slidingDownMaxSlope = true;
                    collisions.slopeNormal = hit.normal;
                }
            }

        }

        private IEnumerator ResetFallingThroughPlatform()
        {
            yield return resetPlatformWait;
            collisions.fallingThroughPlatform = false;
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
            public int faceDir;
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