using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor
{
    [MenuItem("GameObject/Create Other/Spline/Line", false, 0)]
    public static void CreateLine()
    {
        var o = new GameObject("Line");
        o.AddComponent<Line>();
        Selection.activeObject = o;
    }

    void OnSceneGUI()
    {
        var line = (Line)target;

        var handleTransform = line.transform;
        var handleRotation = Tools.pivotRotation == PivotRotation.Local
            ? handleTransform.rotation
            : Quaternion.identity;

        var p0 = handleTransform.TransformPoint(line.P0);
        var p1 = handleTransform.TransformPoint(line.P1);

        EditorGUI.BeginChangeCheck();
        p0 = Handles.DoPositionHandle(p0, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            line.P0 = handleTransform.InverseTransformPoint(p0);
            EditorUtility.SetDirty(line);
        }

        EditorGUI.BeginChangeCheck();
        p1 = Handles.DoPositionHandle(p1, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            line.P1 = handleTransform.InverseTransformPoint(p1);
            EditorUtility.SetDirty(line);
        }

        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);
    }
}
