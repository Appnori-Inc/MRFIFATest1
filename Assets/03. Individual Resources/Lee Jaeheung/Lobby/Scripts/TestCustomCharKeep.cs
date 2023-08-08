using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class TestCustomCharKeep : MonoBehaviour
{
    public List<CustomModelData> customizeModelDatas;


    public void SetData(int index)
    {
        customizeModelDatas[index] = CustomModelSettingCtrl.GetRandomModelData();
    }

    public void AddData()
    {
        if (customizeModelDatas.Count == 50)
        {
            return;
        }
        customizeModelDatas.Add(new CustomModelData());
    }

    public void RemoveData()
    {
        if (customizeModelDatas.Count == 0)
        {
            return;
        }
        customizeModelDatas.RemoveAt(customizeModelDatas.Count - 1);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(TestCustomCharKeep))]
public class Edit_TestCustomCharKeep : Editor
{
    TestCustomCharKeep customCharKeep;
    //List<bool> isFolded;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        customCharKeep = (TestCustomCharKeep)target;

        if (customCharKeep.customizeModelDatas == null)
        {
            customCharKeep.customizeModelDatas = new List<CustomModelData>();
        }
        //if (isFolded == null)
        //{
        //    isFolded = new List<bool>();
        //}


        //int length = customCharKeep.customizeModelDatas.Count;
        //if (length != isFolded.Count)
        //{
        //    while (length > isFolded.Count)
        //    {
        //        isFolded.Add(new bool());
        //    }

        //    while (length < isFolded.Count)
        //    {
        //        isFolded.RemoveAt(isFolded.Count - 1);
        //    }
        //}
        EditorGUILayout.BeginHorizontal();
        int count = customCharKeep.customizeModelDatas.Count;
        EditorGUILayout.LabelField("Count : " + count);


        if (GUILayout.Button("Add"))
        {
            customCharKeep.AddData();
        }
        else if (GUILayout.Button("Remove"))
        {
            customCharKeep.RemoveData();
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < count; i++)
        {
            SetSlot(i);
        }
    }

    public void SetSlot(int index)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("[Slot " + index + "]");

        if (GUILayout.Button("SetRandom"))
        {
            customCharKeep.SetData(index);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("customizeModelDatas").GetArrayElementAtIndex(index));

    }
}
#endif