using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{
    public static Vector3 XZ(this Vector2 v, float y = 0f) => new Vector3(v.x, y, v.y);
    public static Vector3 ZY(this Vector2 v, float x = 0f) => new Vector3(x, v.y, v.x);

    public static Vector2 SetX(this Vector2 v, float x) => new Vector2(x, v.y);
    public static Vector2 SetY(this Vector2 v, float y) => new Vector2(v.x, y);
    public static Vector2 Abs(this Vector2 v) => new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));

    public static Rect GetBounds(this IEnumerable<Vector2> positions)
    {
        float xMin, xMax, yMin, yMax;
        xMin = yMin = float.MaxValue;
        xMax = yMax = -float.MaxValue;

        foreach (var position in positions)
        {
            if (position.x < xMin)
                xMin = position.x;
            if (position.y < yMin)
                yMin = position.y;

            if (position.x > xMax)
                xMax = position.x;
            if (position.y > yMax)
                yMax = position.y;
        }

        var min = new Vector2(xMin, yMin);
        var size = new Vector2(xMax - xMin, yMax - yMin);
        return new Rect(min, size);
    }

    public static Vector2 GetCenter(this IEnumerable<Vector2> positions)
    {
        var center = Vector2.zero;
        var count = 0;

        foreach (var position in positions)
        {
            center += position;
            ++count;
        }

        if (count > 0)
            center /= count;

        return center;
    }

    public static Vector2 RotateCCW(this Vector2 v)
    {
        v = new Vector2(-v.y, v.x);
        return v;
    }

    public static Vector2 RotateCW(this Vector2 v)
    {
        v = new Vector2(v.y, -v.x);
        return v;
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        var tx = v.x;
        var ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static Quaternion ToRotation(this Vector2 v)
    {
        return Quaternion.AngleAxis(v.y, Vector3.right) * Quaternion.AngleAxis(v.x, Vector3.down);
    }
}
