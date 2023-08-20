using System;
using UnityEngine;

public partial class BezierSpline : MonoBehaviour
{
    public int StepsPerCurve = 10;

    public BezierControlPointMode[] Modes;

    [SerializeField] private Vector3[] _points;

    public int CurveCount => (_points.Length - 1) / 3;

    public int ControlPointCount => _points.Length;

    public Vector3 GetControlPoint(int index) => _points[index];

    public void SetControlPoint(int index, Vector3 point)
    {
        if (index % 3 == 0)
        {
            var delta = point - _points[index];
            if (index > 0)
                _points[index - 1] += delta;

            if (index + 1 < _points.Length)
                _points[index + 1] += delta;
        }

        _points[index] = point;
        EnforceMode(index);
    }

    public Vector3 GetLocalPoint(float t)
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

        return Bezier.GetPoint(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t);
    }

    public Vector3 GetGlobalPoint(float t) => transform.TransformPoint(GetLocalPoint(t));

    public void AddCurve()
    {
        var point = _points[_points.Length - 1];
        Array.Resize(ref _points, _points.Length + 3);
        point.x += 1f;
        point.y += 1f;
        _points[_points.Length - 3] = point;
        point.x += 1f;
        _points[_points.Length - 2] = point;
        point.x += 1f;
        point.y -= 1f;
        _points[_points.Length - 1] = point;

        Array.Resize(ref Modes, Modes.Length + 1);
        Modes[Modes.Length - 1] = Modes[Modes.Length - 2];
        EnforceMode(_points.Length - 4);
    }

    public void RemoveCurve()
    {
        Array.Resize(ref _points, _points.Length - 3);
        Array.Resize(ref Modes, Modes.Length - 1);
        EnforceMode(_points.Length - 1);
    }

    public BezierControlPointMode GetControlPointMode(int index) => Modes[(index + 1) / 3];

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        Modes[(index + 1) / 3] = mode;
        EnforceMode(index);
    }

	private void EnforceMode(int index)
    {
        var modeIndex = (index + 1) / 3;
        var mode = Modes[modeIndex];
        if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == Modes.Length - 1)
        {
            return;
        }

        var middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            enforcedIndex = middleIndex + 1;
        }
        else
        {
            fixedIndex = middleIndex + 1;
            enforcedIndex = middleIndex - 1;
        }

        var middle = _points[middleIndex];
        var enforcedTangent = middle - _points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, _points[enforcedIndex]);
        }

        _points[enforcedIndex] = middle + enforcedTangent;
    }

    public void Reset()
    {
        _points = new[]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 1f, 0f),
            new Vector3(2f, 1f, 0f),
            new Vector3(3f, 0f, 0f)
        };

        Modes = new[]
        {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };

        CalculateSplineLength(StepsPerCurve);
        CalculateVertexPath();
    }
}
