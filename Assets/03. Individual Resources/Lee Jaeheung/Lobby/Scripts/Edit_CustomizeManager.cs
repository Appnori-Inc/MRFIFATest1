#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomizeManager))]
public class Edit_CustomizeManager : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CustomizeManager customizeManager = (CustomizeManager)target;
        if (GUILayout.Button("CSVToJson"))
        {
            customizeManager.CSVToJson();
        }
    }
}
#endif