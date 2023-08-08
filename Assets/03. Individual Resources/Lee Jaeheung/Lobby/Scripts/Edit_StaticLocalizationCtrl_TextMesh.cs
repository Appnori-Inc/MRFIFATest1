#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using TMPro;

[CustomEditor(typeof(StaticLocalizationCtrl_TextMesh))]
public class Edit_StaticLocalizationCtrl_TextMesh : Editor
{
    public override void OnInspectorGUI()
    {
        StaticLocalizationCtrl_TextMesh staticLocaliztionCtrl = (StaticLocalizationCtrl_TextMesh)target;

        StaticLocalizationCtrl_TextMesh.DataSlot[] dataSlots = staticLocaliztionCtrl.dataSlots;

        if (dataSlots == null)
        {
            dataSlots = new StaticLocalizationCtrl_TextMesh.DataSlot[0];
        }

        EditorGUILayout.BeginHorizontal();

        int leng = dataSlots.Length;
        EditorGUILayout.LabelField("[ TextMesh - " + leng + " ]");

        if (GUILayout.Button("+") && leng < 50)
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
                dataSlots[i] = new StaticLocalizationCtrl_TextMesh.DataSlot();
            }

            EditorGUILayout.LabelField("Slot " + i + (dataSlots[i].ui != null ? (" - " + dataSlots[i].ui.text) : ""));

            dataSlots[i].id = EditorGUILayout.TextField(dataSlots[i].id);
            dataSlots[i].ui = (TextMesh)EditorGUILayout.ObjectField(dataSlots[i] == null ? null : dataSlots[i].ui, typeof(TextMesh), true);

            EditorGUILayout.EndHorizontal();
        }

        //////////////////////

        StaticLocalizationCtrl_TextMesh.DataSlot_Pro[] dataSlots_pro = staticLocaliztionCtrl.dataSlots_pro;

        if (dataSlots_pro == null)
        {
            dataSlots_pro = new StaticLocalizationCtrl_TextMesh.DataSlot_Pro[0];
        }

        EditorGUILayout.BeginHorizontal();

        leng = dataSlots_pro.Length;
        EditorGUILayout.LabelField("[ TextMeshPro - " + leng + " ]");

        if (GUILayout.Button("+") && leng < 50)
        {
            leng++;
            Array.Resize(ref dataSlots_pro, leng);
            staticLocaliztionCtrl.dataSlots_pro = dataSlots_pro;
        }
        if (GUILayout.Button("-") && leng >= 1)
        {
            leng--;
            Array.Resize(ref dataSlots_pro, leng);
            staticLocaliztionCtrl.dataSlots_pro = dataSlots_pro;
        }

        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < dataSlots_pro.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

            if (dataSlots_pro[i] == null)
            {
                dataSlots_pro[i] = new StaticLocalizationCtrl_TextMesh.DataSlot_Pro();
            }

            EditorGUILayout.LabelField("Slot " + i + (dataSlots_pro[i].ui != null ? (" - " + dataSlots_pro[i].ui.text) : ""));

            dataSlots_pro[i].id = EditorGUILayout.TextField(dataSlots_pro[i].id);
            dataSlots_pro[i].ui = (TextMeshPro)EditorGUILayout.ObjectField(dataSlots_pro[i] == null ? null : dataSlots_pro[i].ui, typeof(TextMeshPro), true);

            EditorGUILayout.EndHorizontal();
        }

        ///////////////////////

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