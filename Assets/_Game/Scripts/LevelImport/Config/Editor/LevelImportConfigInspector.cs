using UnityEditor;
using UnityEngine;

namespace Game
{
    [CustomEditor(typeof(LevelImportConfig))]
    public class LevelImportConfigInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var levelConfig = (LevelImportConfig)target;

            SerializedProperty prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
					if (prop.name != "Scaling" && prop.name != "PointsPerUnit")
					{
						EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
					}
                } while (prop.NextVisible(false));
            }

            if (GUILayout.Button("Build Levels"))
            {
                levelConfig.GenerateAndSerialize();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
