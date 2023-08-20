using UnityEngine;

public partial class BezierSpline
{
    public float VertexPathMinDistance = 0.1f;

    public float[] QuantizedDistances => _quantizedDistances;

    [SerializeField] private float[] _distances;

    [SerializeField] private float[] _quantizedDistances;

    public void CalculateVertexPath()
    {
        CalculateDistancesLookup();
        Quantize();
    }

	private void CalculateDistancesLookup()
    {
        var steps = StepsPerCurve * CurveCount;
        _distances = new float[steps + 1];
        _distances[0] = 0f;
        var distance = 0f;
        var lineStart = GetLocalPoint(0f);
        for (int i = 1; i <= steps; i++)
        {
            var lineEnd = GetLocalPoint((float)i / steps);
            var stepDistance = Vector2.Distance(lineStart, lineEnd);
            distance += stepDistance;
            _distances[i] = distance;
            lineStart = lineEnd;
        }
    }

	private void Quantize()
    {
        var steps = (int)(_splineLength / VertexPathMinDistance);
        _quantizedDistances = new float[steps + 1];
        _quantizedDistances[0] = 0f;
        var distance = 0f;
        var lineStart = GetLocalPoint(0f);
        for (var i = 1; i <= _quantizedDistances.Length - 1; i++)
        {
            var t = GetTForDistance(i * VertexPathMinDistance);
            var lineEnd = GetLocalPoint(t);
            var stepDistance = Vector2.Distance(lineStart, lineEnd);
            distance += stepDistance;
            _quantizedDistances[i] = distance;
            lineStart = lineEnd;
        }
    }

    public float GetTForDistance(float distance)
    {
        for (int i = 1; i < _distances.Length; i++)
        {
            var d0 = _distances[i - 1];
            var d1 = _distances[i];
            if (d1 >= distance)
            {
                var fraction = (distance - d0) / (d1 - d0);
                return (i - 1 + fraction) / (_distances.Length - 1);
            }
        }

        return 1f;
    }
}
