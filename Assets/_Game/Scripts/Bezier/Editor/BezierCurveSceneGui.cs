using UnityEditor;
using UnityEngine;

public partial class BezierCurveInspector
{
    Transform _handleTransform;
    Quaternion _handleRotation;

    void OnSceneGUI()
    {
        _curve = (BezierCurve)target;
        _handleTransform = _curve.transform;
        _handleRotation = Tools.pivotRotation == PivotRotation.Local
            ? _handleTransform.rotation
            : Quaternion.identity;

        var p0 = DrawPoint(0);
        var p1 = DrawPoint(1);
        var p2 = DrawPoint(2);
        var p3 = DrawPoint(3);

        Handles.color = Color.gray;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);

        var lineStart = _curve.GetPoint(0f);

        if (_debug)
        {
            Handles.color = Color.blue;
            Handles.DrawLine(lineStart, lineStart + _curve.GetTangent(0f).normalized);
        }

        for (int i = 1; i <= _curve.LineSteps; i++)
        {
            var t = (float)i / _curve.LineSteps;
            var lineEnd = _curve.GetPoint(t);

            if (!_previewBezier)
            {
                Handles.color = Color.white;
                Handles.DrawLine(lineStart, lineEnd);
            }

            if (_debug)
            {
                Handles.color = Color.blue;
                Handles.DrawLine(lineEnd, lineEnd + _curve.GetTangent(t).normalized);
            }

            lineStart = lineEnd;
        }

        if (_previewBezier)
        {
            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
        }
    }

    Vector3 DrawPoint(int index)
    {
        var point = _handleTransform.TransformPoint(_curve.Points[index]);
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, _handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_curve, "Move Point");
            _curve.Points[index] = _handleTransform.InverseTransformPoint(point);
            EditorUtility.SetDirty(_curve);
        }

        return point;
    }
}
