using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector2 XZ(this Vector3 v) => new Vector2(v.x, v.z);

    public static Vector3 SetX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
    public static Vector3 SetY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
    public static Vector3 SetZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

    public static Vector3 Abs(this Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

    public static Bounds GetBounds(this IEnumerable<Vector3> positions)
    {
        float xMin, xMax, yMin, yMax, zMin, zMax;
        xMin = yMin = zMin = float.MaxValue;
        xMax = yMax = zMax = -float.MaxValue;

        foreach (var position in positions)
        {
            if (position.x < xMin)
                xMin = position.x;
            if (position.y < yMin)
                yMin = position.y;
            if (position.z < zMin)
                zMin = position.z;

            if (position.x > xMax)
                xMax = position.x;
            if (position.y > yMax)
                yMax = position.y;
            if (position.z > zMax)
                zMax = position.z;
        }

        var min = new Vector3(xMin, yMin, zMin);
        var size = new Vector3(xMax - xMin, yMax - yMin, zMax - zMin);
        return new Bounds(min + size / 2f, size);
    }

    public static Vector3 GetCenter(this IEnumerable<Vector3> positions)
    {
        var center = Vector3.zero;
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

    public static float Max(this Vector3 v)
    {
        var max = 0f;
        for (var i = 0; i < 3; ++i)
            max = Mathf.Max(Mathf.Abs(v[i]));

        return max;
    }

    public static float GetTotalLength(this IEnumerable<Vector3> collection)
    {
        var prev = collection.First();
        var length = 0f;

        foreach (var v in collection.Skip(1))
        {
            length += Vector3.Distance(prev, v);
            prev = v;
        }

        return length;
    }
}
