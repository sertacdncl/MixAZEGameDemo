using UnityEngine;

// https://catlikecoding.com/unity/tutorials/curves-and-splines/

public static class Bezier
{
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        var omt = 1f - t;
        var omt2 = omt * omt;
        var t2 = t * t;
        return
            p0 * omt2 +
            p1 * 2f * omt * t +
            p2 * t2;
    }

    public static Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return
            (p1 - p0) * 2f * (1f - t) +
            (p2 - p1) * 2f * t;
    }

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        var omt = 1f - t;
        var omt2 = omt * omt;
        var t2 = t * t;
        return
            p0 * omt2 * omt +
            p1 * 3f * omt2 * t +
            p2 * 3f * omt * t2 +
            p3 * t2 * t;
    }

    public static Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        var omt = 1f - t;
        var omt2 = omt * omt;
        var t2 = t * t;
        return
            (p1 - p0) * 3f * omt2 +
            (p2 - p1) * 6f * omt * t +
            (p3 - p2) * 3f * t2;
    }

    public static Vector2 GetPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        t = Mathf.Clamp01(t);
        var omt = 1f - t;
        var omt2 = omt * omt;
        var t2 = t * t;
        return
            p0 * omt2 +
            p1 * 2f * omt * t +
            p2 * t2;
    }

    public static Vector2 GetTangent(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        return
            (p1 - p0) * 2f * (1f - t) +
            (p2 - p1) * 2f * t;
    }

    public static Vector2 GetPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        t = Mathf.Clamp01(t);
        var omt = 1f - t;
        var omt2 = omt * omt;
        var t2 = t * t;
        return
            p0 * omt2 * omt +
            p1 * 3f * omt2 * t +
            p2 * 3f * omt * t2 +
            p3 * t2 * t;
    }

    public static Vector2 GetTangent(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        t = Mathf.Clamp01(t);
        var omt = 1f - t;
        var omt2 = omt * omt;
        var t2 = t * t;
        return
            (p1 - p0) * 3f * omt2 +
            (p2 - p1) * 6f * omt * t +
            (p3 - p2) * 3f * t2;
    }

    public static float GetPoint(float p0, float p1, float p2, float t)
    {
        t = Mathf.Clamp01(t);
        var omt = 1f - t;
        var omt2 = omt * omt;
        var t2 = t * t;
        return
            p0 * omt2 +
            p1 * (2f * omt * t) +
            p2 * t2;
    }

    public static float GetTangent(float p0, float p1, float p2, float t)
    {
        return
            (p1 - p0) * 2f * (1f - t) +
            (p2 - p1) * 2f * t;
    }

    public static float GetPoint(float p0, float p1, float p2, float p3, float t)
    {
        t = Mathf.Clamp01(t);
        var omt = 1f - t;
        var omt2 = omt * omt;
        var t2 = t * t;
        return
            p0 * omt2 * omt +
            p1 * 3f * omt2 * t +
            p2 * 3f * omt * t2 +
            p3 * t2 * t;
    }

    public static float GetTangent(float p0, float p1, float p2, float p3, float t)
    {
        t = Mathf.Clamp01(t);
        var omt = 1f - t;
        var omt2 = omt * omt;
        var t2 = t * t;
        return
            (p1 - p0) * 3f * omt2 +
            (p2 - p1) * 6f * omt * t +
            (p3 - p2) * 3f * t2;
    }
}
