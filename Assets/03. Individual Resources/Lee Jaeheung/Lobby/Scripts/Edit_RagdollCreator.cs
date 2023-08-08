#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RagdollCreator))]
public class Edit_RagdollCreator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RagdollCreator ragdollCreator = (RagdollCreator)target;
        if (GUILayout.Button("Create"))
        {
            ragdollCreator.CreateRagdoll();
        }
    }
}
#endif