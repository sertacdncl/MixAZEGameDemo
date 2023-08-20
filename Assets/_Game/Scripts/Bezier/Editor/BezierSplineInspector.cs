using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline))]
public partial class BezierSplineInspector : Editor
{
    [MenuItem("GameObject/Create Other/Spline/Bezier Spline", false, 2)]
    public static void CreateBezierSpline()
    {
        var o = new GameObject("Bezier Spline");
        o.AddComponent<BezierSpline>();
        Selection.activeObject = o;
    }

    static bool _previewBezier;
    static bool _debug;

    BezierSpline _spline;
    bool _showRawValue;

    public override void OnInspectorGUI()
    {
        _spline = (BezierSpline)target;
        EditorGUILayout.LabelField("Spline Length", _spline.SplineLength.ToString());

        RecordChange("Change StepsPerCurve",
            () => EditorGUILayout.IntField("Steps Per Curve", _spline.StepsPerCurve),
            v => _spline.StepsPerCurve = Mathf.Max(2, v));

        RecordChange("Change VertexPathMinDistance",
            () => EditorGUILayout.Slider("Vertex Path Min Distance", _spline.VertexPathMinDistance, 0.01f, 10),
            v => _spline.VertexPathMinDistance = v);

        EditorGUI.BeginChangeCheck();
        _previewBezier = EditorGUILayout.Toggle("Preview Bezier", _previewBezier);
        _debug = EditorGUILayout.Toggle("Debug", _debug);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(_spline);
        }

        if (_selectedIndex >= 0 && _selectedIndex < _spline.ControlPointCount)
        {
            DrawSelectedPointInspector();
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(_spline, "Add Curve");
            _spline.AddCurve();
            _spline.CalculateSplineLength(_spline.StepsPerCurve);
            _spline.CalculateVertexPath();
            EditorUtility.SetDirty(_spline);
        }

        EditorGUI.BeginDisabledGroup(_spline.CurveCount < 2);
        if (GUILayout.Button("Remove Curve"))
        {
            Undo.RecordObject(_spline, "Remove Curve");
            _spline.RemoveCurve();
            _spline.CalculateSplineLength(_spline.StepsPerCurve);
            _spline.CalculateVertexPath();
            EditorUtility.SetDirty(_spline);
        }

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        _showRawValue = EditorGUILayout.Foldout(_showRawValue, "Raw values");
        if (_showRawValue)
            base.OnInspectorGUI();
    }

    void DrawSelectedPointInspector()
    {
        EditorGUILayout.Space();
        GUILayout.Label("Selected Point", EditorStyles.boldLabel);

        RecordChange("Move Point",
            () => EditorGUILayout.Vector3Field("Position", _spline.GetControlPoint(_selectedIndex)),
            v => _spline.SetControlPoint(_selectedIndex, v));

        RecordChange("Change Point Mode",
            () => (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", _spline.GetControlPointMode(_selectedIndex)),
            v => _spline.SetControlPointMode(_selectedIndex, v));
    }

    void RecordChange<T>(string undoMessage, Func<T> drawEditor, Action<T> applyValue)
    {
        EditorGUI.BeginChangeCheck();
        var value = drawEditor();
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_spline, undoMessage);
            applyValue(value);
            _spline.CalculateSplineLength(_spline.StepsPerCurve);
            _spline.CalculateVertexPath();
            EditorUtility.SetDirty(_spline);
        }
    }
}
