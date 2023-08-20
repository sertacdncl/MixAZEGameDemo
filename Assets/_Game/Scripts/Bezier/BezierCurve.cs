using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public int LineSteps = 10;
    public Vector3[] Points;

    public Vector3 GetPoint(float t) =>
        transform.TransformPoint(Bezier.GetPoint(Points[0], Points[1], Points[2], Points[3], t));

    public Vector3 GetTangent(float t) =>
        transform.TransformPoint(Bezier.GetTangent(Points[0], Points[1], Points[2], Points[3], t)) -
        transform.position;

    public void Reset()
    {
        Points = new[]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 1f, 0f),
            new Vector3(2f, 1f, 0f),
            new Vector3(3f, 0f, 0f)
        };
    }
}
