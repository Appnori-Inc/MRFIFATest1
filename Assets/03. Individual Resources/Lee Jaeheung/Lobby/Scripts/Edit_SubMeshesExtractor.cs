#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SubMeshesExtractor))]
public class Edit_SubMeshesExtractor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SubMeshesExtractor ragdollCreator = (SubMeshesExtractor)target;
        if (GUILayout.Button("Extract"))
        {
            ragdollCreator.Extract();
        }
    }
}
#endif