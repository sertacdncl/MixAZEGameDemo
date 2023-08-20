using UnityEngine;

public partial class BezierSpline
{
    public float SplineLength => _splineLength;

    [SerializeField, HideInInspector] private float _splineLength;

    public void CalculateSplineLength(int stepsPerCurve)
    {
        var distance = 0f;
        var lineStart = GetLocalPoint(0f);
        var steps = stepsPerCurve * CurveCount;
        for (int i = 1; i <= steps; i++)
        {
            var t = (float)i / steps;
            var lineEnd = GetLocalPoint(t);
            distance += Vector2.Distance(lineStart, lineEnd);
            lineStart = lineEnd;
        }

        _splineLength = distance;
    }
}
