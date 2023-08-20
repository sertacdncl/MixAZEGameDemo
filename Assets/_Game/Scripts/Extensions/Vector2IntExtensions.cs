using System.Collections.Generic;
using UnityEngine;

public static class Vector2IntExtensions
{
    public static Vector2Int SetX(this Vector2Int v, int x) => new Vector2Int(x, v.y);
    public static Vector2Int SetY(this Vector2Int v, int y) => new Vector2Int(v.x, y);
    public static Vector2Int Abs(this Vector2Int v) => new Vector2Int(Mathf.Abs(v.x), Mathf.Abs(v.y));

    public static RectInt GetBounds(this IEnumerable<Vector2Int> positions)
    {
        int xMin, xMax, yMin, yMax;
        xMin = yMin = int.MaxValue;
        xMax = yMax = int.MinValue;

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

        var min = new Vector2Int(xMin, yMin);
        var max = new Vector2Int(xMax, yMax);

        return new RectInt()
        {
            min = min,
            max = max
        };
    }

    public static Vector2 GetCenter(this IEnumerable<Vector2Int> positions)
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

    public static Vector2Int RotateCCW(this Vector2Int v)
    {
        v = new Vector2Int(-v.y, v.x);
        return v;
    }

    public static Vector2Int RotateCW(this Vector2Int v)
    {
        v = new Vector2Int(v.y, -v.x);
        return v;
    }

    public static void MirrorVertical(ref this Vector2 point, Vector2 anchor)
    {
        point.y = anchor.y - point.y;
        point.x -= anchor.x;
    }

    public static void MirrorVertical(this Vector2[] points, Vector2 anchor)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].y = anchor.y - points[i].y;
            points[i].x -= anchor.x;
        }
    }

    public static void Scale(ref this Vector2 point, float scale)
    {
        point *= scale;
    }

    public static void Scale(this Vector2[] points, float scale)
    {
        for (int i = 0; i < points.Length; i++)
            points[i] *= scale;
    }
}
