#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CombineMeshCreator))]
public class Edit_CombineMeshCreator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CombineMeshCreator ragdollCreator = (CombineMeshCreator)target;
        if (GUILayout.Button("Create"))
        {
            ragdollCreator.Create();
        }
    }
}
#endif