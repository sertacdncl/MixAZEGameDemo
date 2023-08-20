using UnityEngine;

public partial class BezierSpline : MonoBehaviour
{
    public Vector3 GetTangent(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = _points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return transform.TransformPoint(Bezier.GetTangent(
                   _points[i], _points[i + 1], _points[i + 2], _points[i + 3], t)) - transform.position;
    }

    public Vector3 GetNormal(Vector3 tangent) => Quaternion.LookRotation(tangent) * Vector3.up;
    public Vector3 GetBinormal(Vector3 tangent, Vector3 normal) => Vector3.Cross(normal, tangent);
}
