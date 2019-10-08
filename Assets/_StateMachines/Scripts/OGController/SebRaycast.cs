using UnityEngine;

namespace SebCharCtrl
{
    public abstract class SebRaycast : MonoBehaviour
    {
        //requires
        public BoxCollider2D Collider;
        protected LayerMask collisionMask;

        public ColliderBounds CollBounds;

        [SerializeField]
        private const float skinWidth = .015f;
        public float SkinWidth { get { return skinWidth; } }

        private const float dstBetweenRays = .25f;

        protected int horRayCount;
        protected int verRayCount;

        protected float horRaySpacing;
        protected float verRaySpacing;


        protected virtual void Awake()
        {
            //todo: update collider based on state
            Collider = GetComponent<BoxCollider2D>();
            collisionMask = StaticRefs.MASK_PALPABLE;
            CalculateRaySpacing();
        }

        protected void UpdateColliderBounds()
        {
            Bounds bounds = Collider.bounds;
            bounds.Expand(SkinWidth * -2);

            CollBounds.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            CollBounds.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            CollBounds.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            CollBounds.topRight = new Vector2(bounds.max.x, bounds.max.y);
            CollBounds.middleLeft = new Vector2(bounds.min.x, bounds.center.y);
            CollBounds.middleRight = new Vector2(bounds.max.x, bounds.center.y);
            CollBounds.middleTop = new Vector2(bounds.center.x, bounds.max.y);
            CollBounds.middleBottom = new Vector2(bounds.center.x, bounds.max.y);
            CollBounds.middle = bounds.center;
        }

        protected void CalculateRaySpacing()
        {
            Bounds bounds = Collider.bounds;
            bounds.Expand(SkinWidth * -2);

            float boundsWidth = bounds.size.x;
            float boundsHeight = bounds.size.y;

            horRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
            verRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

            horRaySpacing = bounds.size.y / (horRayCount - 1);
            verRaySpacing = bounds.size.x / (verRayCount - 1);
        }

        public struct ColliderBounds
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
            public Vector2 middleLeft, middleRight;
            public Vector2 middleTop, middleBottom;
            public Vector2 middle;
        }
    }
}