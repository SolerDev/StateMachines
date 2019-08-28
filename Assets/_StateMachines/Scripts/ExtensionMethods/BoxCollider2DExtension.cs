using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoxCollider2DExtension
{
    public static Vector2 BottomLeft(this BoxCollider2D collider)
    {
        Vector2 r = new Vector2(
            collider.bounds.center.x - collider.bounds.extents.x,
            collider.bounds.center.y - collider.bounds.extents.y);

        return r;
    }

    public static Vector2 BottomRight(this BoxCollider2D collider)
    {
        Vector2 r = new Vector2(
            collider.bounds.center.x + collider.bounds.extents.x,
            collider.bounds.center.y - collider.bounds.extents.y);

        return r;
    }

    public static Vector2 TopLeft(this BoxCollider2D collider)
    {
        Vector2 r = new Vector2(
            collider.bounds.center.x - collider.bounds.extents.x,
            collider.bounds.center.y + collider.bounds.extents.y);

        return r;
    }

    public static Vector2 TopRight(this BoxCollider2D collider)
    {
        Vector2 r = new Vector2(
            collider.bounds.center.x + collider.bounds.extents.x,
            collider.bounds.center.y + collider.bounds.extents.y);

        return r;
    }

    public static Vector2 MiddleRight(this BoxCollider2D collider)
    {
        Vector2 r = new Vector2(
            collider.bounds.center.x + collider.bounds.extents.x,
            collider.bounds.center.y);

        return r;
    }

    public static Vector2 MiddleLeft(this BoxCollider2D collider)
    {
        Vector2 r = new Vector2(
            collider.bounds.center.x - collider.bounds.extents.x,
            collider.bounds.center.y);

        return r;
    }

    public static Vector2 MiddleTop(this BoxCollider2D collider)
    {
        Vector2 r = new Vector2(
            collider.bounds.center.x,
            collider.bounds.center.y + collider.bounds.extents.y);

        return r;
    }

    public static Vector2 MiddleBottom(this BoxCollider2D collider)
    {
        Vector2 r = new Vector2(
            collider.bounds.center.x,
            collider.bounds.center.y - collider.bounds.extents.y);

        return r;
    }
}

