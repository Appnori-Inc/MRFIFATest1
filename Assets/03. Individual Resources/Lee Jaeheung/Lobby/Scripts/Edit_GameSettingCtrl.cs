#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameSettingCtrl))]
public class Edit_GameSettingCtrl : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameSettingCtrl settingManager = (GameSettingCtrl)target;
        if (GUILayout.Button("CSVToJson"))
        {
            settingManager.CSVToJson();
        }
    }
}
#endif