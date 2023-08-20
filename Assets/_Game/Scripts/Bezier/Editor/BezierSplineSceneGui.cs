using UnityEditor;
using UnityEngine;

public partial class BezierSplineInspector
{
    const float HandleSize = 0.04f;
    const float PickSize = 0.06f;

    static readonly Color[] _modeColors =
    {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    int _selectedIndex = -1;

    Transform _handleTransform;
    Quaternion _handleRotation;

    void OnSceneGUI()
    {
        _spline = (BezierSpline)target;
        _handleTransform = _spline.transform;
        _handleRotation = Tools.pivotRotation == PivotRotation.Local
            ? _handleTransform.rotation
            : Quaternion.identity;

        DrawBezierCurve();
        DrawVertexPath();
    }

    void DrawBezierCurve()
    {
        var p0 = DrawPoint(0);
        for (int i = 1; i < _spline.ControlPointCount; i += 3)
        {
            var p1 = DrawPoint(i);
            var p2 = DrawPoint(i + 1);
            var p3 = DrawPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            if (_previewBezier)
            {
                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            }

            p0 = p3;
        }

        var lineStart = _spline.GetGlobalPoint(0f);

        if (_debug)
            DrawDebug(0f);

        var steps = _spline.StepsPerCurve * _spline.CurveCount;
        for (int i = 1; i <= steps; i++)
        {
            var t = (float)i / steps;
            var lineEnd = _spline.GetGlobalPoint(t);

            if (!_previewBezier)
            {
                Handles.color = Color.white;
                Handles.DrawLine(lineStart, lineEnd);
            }

            if (_debug)
                DrawDebug(t);

            lineStart = lineEnd;
        }
    }

    void DrawVertexPath()
    {
        if (_debug)
        {
            foreach (var distance in _spline.QuantizedDistances)
            {
                var t = _spline.GetTForDistance(distance);
                var point = _spline.GetGlobalPoint(t);
                var size = HandleUtility.GetHandleSize(point);
                Handles.color = Color.red;
                Handles.Button(point, _handleRotation, size * HandleSize * 2, size * PickSize * 2, Handles.SphereHandleCap);
            }
        }
    }

    Vector3 DrawPoint(int index)
    {
        var point = _handleTransform.TransformPoint(_spline.GetControlPoint(index));
        var size = HandleUtility.GetHandleSize(point);
        Handles.color = _modeColors[(int)_spline.GetControlPointMode(index)];
        if (Handles.Button(point, _handleRotation, size * HandleSize, size * PickSize, Handles.DotHandleCap))
        {
            _selectedIndex = index;
            Repaint();
        }

        if (_selectedIndex == index)
        {
            RecordChange("Move Point",
                () => Handles.DoPositionHandle(point, _handleRotation),
                v => _spline.SetControlPoint(index, _handleTransform.InverseTransformPoint(v)));
        }

        return point;
    }

    void DrawDebug(float t)
    {
        var point = _spline.GetGlobalPoint(t);
        var tangent = _spline.GetTangent(t);
        var normal = _spline.GetNormal(tangent);
        var binormal = _spline.GetBinormal(tangent, normal);
        Handles.color = Color.blue;
        Handles.DrawLine(point, point + tangent.normalized);
        Handles.color = Color.green;
        Handles.DrawLine(point, point + normal.normalized);
        Handles.color = Color.red;
        Handles.DrawLine(point, point + binormal.normalized);
    }
}
