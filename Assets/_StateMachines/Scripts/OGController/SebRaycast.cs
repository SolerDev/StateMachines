using UnityEngine;

namespace SebController
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SebRaycast : MonoBehaviour
    {
        public LayerMask collisionMask;

        public const float skinWidth = .015f;
        private const float dstBetweenRays = .25f;
        [HideInInspector]
        public int horizontalRayCount;
        [HideInInspector]
        public int verticalRayCount;

        [HideInInspector]
        public float horizontalRaySpacing;
        [HideInInspector]
        public float verticalRaySpacing;

        [HideInInspector]
        public BoxCollider2D collider;
        public ColliderBounds collBounds;

        public virtual void Awake()
        {
            collider = GetComponent<BoxCollider2D>();
        }

        public virtual void Start()
        {
            CalculateRaySpacing();
        }

        public void UpdateColliderBounds()
        {
            Bounds bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);

            collBounds.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            collBounds.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            collBounds.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            collBounds.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        public void CalculateRaySpacing()
        {
            Bounds bounds = collider.bounds;
            bounds.Expand(skinWidth * -2);

            float boundsWidth = bounds.size.x;
            float boundsHeight = bounds.size.y;

            horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
            verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        public struct ColliderBounds
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }
    }
}