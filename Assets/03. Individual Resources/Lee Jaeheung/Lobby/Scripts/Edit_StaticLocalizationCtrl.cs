#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;

[CustomEditor(typeof(StaticLocalizationCtrl))]
public class Edit_StaticLocalizationCtrl : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        StaticLocalizationCtrl staticLocaliztionCtrl = (StaticLocalizationCtrl)target;

        StaticLocalizationCtrl.DataSlot[] dataSlots = staticLocaliztionCtrl.dataSlots;

        if (dataSlots == null)
        {
            dataSlots = new StaticLocalizationCtrl.DataSlot[0];
        }

        EditorGUILayout.BeginHorizontal();

        int leng = dataSlots.Length;
        EditorGUILayout.LabelField("[ Langth - " + leng + " ]");

        if (GUILayout.Button("+") && leng < 70)
        {
            leng++;
            Array.Resize(ref dataSlots, leng);
            staticLocaliztionCtrl.dataSlots = dataSlots;
        }
        if (GUILayout.Button("-") && leng >= 1)
        {
            leng--;
            Array.Resize(ref dataSlots, leng);
            staticLocaliztionCtrl.dataSlots = dataSlots;
        }

        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < dataSlots.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

            if (dataSlots[i] == null)
            {
                dataSlots[i] = new StaticLocalizationCtrl.DataSlot();
            }

            EditorGUILayout.LabelField("Slot " + i + (dataSlots[i].ui != null ? (" - " + dataSlots[i].ui.text) : ""));

            dataSlots[i].id = EditorGUILayout.TextField(dataSlots[i].id);
            dataSlots[i].ui = (Text)EditorGUILayout.ObjectField(dataSlots[i] == null ? null : dataSlots[i].ui, typeof(Text), true);

            //object ob = EditorGUILayout.ObjectField(dataSlots[i].ui == null ? null : dataSlots[i].ui, typeof(Text), true);

            //dataSlots[i].ui = (Text)EditorGUILayout.ObjectField(dataSlots[i].ui == null ? null : dataSlots[i].ui, typeof(Text), true);
            //dataSlots[i].id = EditorGUILayout.TextField(dataSlots[i].id);
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("DataReduction"))
        {
            staticLocaliztionCtrl.SetDataReduction();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
#endif