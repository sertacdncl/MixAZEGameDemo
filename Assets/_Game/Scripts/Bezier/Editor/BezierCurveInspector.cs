using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierCurve))]
public partial class BezierCurveInspector : Editor
{
    [MenuItem("GameObject/Create Other/Spline/Bezier Curve", false, 1)]
    public static void CreateBezierCurve()
    {
        var o = new GameObject("Bezier Curve");
        o.AddComponent<BezierCurve>();
        Selection.activeObject = o;
    }

    static bool _previewBezier;
    static bool _debug;

    BezierCurve _curve;

    public override void OnInspectorGUI()
    {
        _curve = (BezierCurve)target;
        EditorGUI.BeginChangeCheck();
        _previewBezier = EditorGUILayout.Toggle("Preview Bezier", _previewBezier);
        _debug = EditorGUILayout.Toggle("Debug", _debug);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(_curve);
        }

        base.OnInspectorGUI();
    }
}
