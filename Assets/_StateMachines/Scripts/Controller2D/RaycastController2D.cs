using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RaycastController2D
{
    #region Properties

    protected Transform trans;
    protected BoxCollider2D currentColl;
    
    //wtf do I do with these three omg its killing me I'm going to die bem morrido
    protected const float skinWidth = .5f;
    public float SkinWidth => skinWidth;

    protected int HorRayCount = 3;
    protected int VerRayCount = 3;

    protected float horRaySpacing;
    protected float verRaySpacing;
    
    public bool Enabled = true;
    public Vector2 Velocity;

    public struct ColliderBounds
    {
        public Vector2 topRight, topLeft;
        public Vector2 bottomRight, bottomLeft;
        public Vector2 middleTop, middleBottom;
        public Vector2 middleRight, middleLeft;
        public Vector2 middle;
    }
    protected ColliderBounds collBounds;
    public ColliderBounds CollBounds { get => collBounds; }

    public struct CollisionInfo
    {
        public bool above, bellow;
        public bool left, right;
        public bool climbingSlope, descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector2 velocityOld;

        public void Reset()
        {
            above = bellow = false;
            left = right = false;
            climbingSlope = descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0f;
        }
    }
    protected CollisionInfo collisions;
    public CollisionInfo Collisions { get => collisions; }

    #endregion

    protected void UpdateColliderBounds()
    {
        Bounds bounds = currentColl.bounds;
        //hardcoded -2f
        bounds.Expand(SkinWidth * -2f);

        //corners
        collBounds.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        collBounds.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        collBounds.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        collBounds.topRight = new Vector2(bounds.max.x, bounds.max.y);

        //middle
        collBounds.middleBottom = new Vector2(bounds.center.x, bounds.min.y);
        collBounds.middleTop = new Vector2(bounds.center.x, bounds.max.y);
        collBounds.middleLeft = new Vector2(bounds.min.x, bounds.center.y);
        collBounds.middleRight = new Vector2(bounds.max.x, bounds.center.y);

        collBounds.middle = bounds.center;
    }

    protected void SetRaySpacing(Bounds reference)
    {
        Bounds bounds = reference;
        //still can't figure out why exactly -2...
        bounds.Expand(SkinWidth * -2f);

        HorRayCount = Mathf.Clamp(HorRayCount, 2, int.MaxValue);
        VerRayCount = Mathf.Clamp(VerRayCount, 2, int.MaxValue);

        horRaySpacing = bounds.size.y / (HorRayCount - 1);
        verRaySpacing = bounds.size.x / (VerRayCount - 1);
    }
}
