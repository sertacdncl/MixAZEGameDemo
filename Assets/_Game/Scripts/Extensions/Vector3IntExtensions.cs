using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Vector3IntExtensions
{
    public static Vector2Int XZ(this Vector3Int v) => new Vector2Int(v.x, v.z);

    public static Vector3Int SetX(this Vector3Int v, int x) => new Vector3Int(x, v.y, v.z);
    public static Vector3Int SetY(this Vector3Int v, int y) => new Vector3Int(v.x, y, v.z);
    public static Vector3Int SetZ(this Vector3Int v, int z) => new Vector3Int(v.x, v.y, z);

    public static Vector3Int Abs(this Vector3Int v) => new Vector3Int(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

    public static BoundsInt GetBounds(this IEnumerable<Vector3Int> positions)
    {
        int xMin, xMax, yMin, yMax, zMin, zMax;
        xMin = yMin = zMin = int.MaxValue;
        xMax = yMax = zMax = int.MinValue;

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

        var min = new Vector3Int(xMin, yMin, zMin);
        var max = new Vector3Int(xMax, yMax, zMax);

        return new BoundsInt()
        {
            min = min,
            max = max
        };
    }

    public static Vector3 GetCenter(this IEnumerable<Vector3Int> positions)
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
