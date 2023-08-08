#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using TMPro;

[CustomEditor(typeof(StaticResourcesChangeCtrl))]
public class Edit_StaticResourcesChangeCtrl : Editor
{
    public override void OnInspectorGUI()
    {
        StaticResourcesChangeCtrl staticResourcesChangeCtrl = (StaticResourcesChangeCtrl)target;

        LanguageState[] languageStates = staticResourcesChangeCtrl.languageStates;

        if (languageStates == null)
        {
            languageStates = new LanguageState[1];
            languageStates[0] = LanguageState.schinese;
            staticResourcesChangeCtrl.languageStates = languageStates;
        }

        EditorGUILayout.BeginHorizontal();

        int leng = languageStates.Length;
        EditorGUILayout.LabelField("[ LanguageStates - " + leng + " ]");

        if (GUILayout.Button("+") && leng < 10)
        {
            leng++;
            Array.Resize(ref languageStates, leng);
            languageStates[leng - 1] = LanguageState.schinese;
            staticResourcesChangeCtrl.languageStates = languageStates;
        }
        if (GUILayout.Button("-") && leng >= 2)
        {
            leng--;
            Array.Resize(ref languageStates, leng);
            staticResourcesChangeCtrl.languageStates = languageStates;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < languageStates.Length; i++)
        {
            GUILayout.Label("{" + i + "}");
            staticResourcesChangeCtrl.languageStates[i] = (LanguageState)EditorGUILayout.EnumPopup(languageStates[i]);
        }
        EditorGUILayout.EndHorizontal();

        //////////////////////////////////////////////////

        StaticResourcesChangeCtrl.DataSlot_SetActive[] dataSlots_setActive = staticResourcesChangeCtrl.dataSlots_setActive;

        if (dataSlots_setActive == null)
        {
            dataSlots_setActive = new StaticResourcesChangeCtrl.DataSlot_SetActive[0];
        }

        EditorGUILayout.BeginHorizontal();

        leng = dataSlots_setActive.Length;

        if (leng > 0 && dataSlots_setActive[0].gos.Length != (languageStates.Length + 1))
        {
            bool isAdd = (dataSlots_setActive[0].gos.Length < (languageStates.Length + 1));

            for (int i = 0; i < dataSlots_setActive.Length; i++)
            {
                if (!isAdd)
                {
                    dataSlots_setActive[i].gos[languageStates.Length] = dataSlots_setActive[i].gos[dataSlots_setActive[i].gos.Length - 1];
                }
                Array.Resize(ref dataSlots_setActive[i].gos, languageStates.Length + 1);
                if (isAdd)
                {
                    dataSlots_setActive[i].gos[languageStates.Length] = dataSlots_setActive[i].gos[languageStates.Length - 1];
                    dataSlots_setActive[i].gos[languageStates.Length - 1] = null;
                }
            }
            staticResourcesChangeCtrl.dataSlots_setActive = dataSlots_setActive;
        }

        EditorGUILayout.LabelField("[ SetActives - " + leng + " ]");

        if (GUILayout.Button("+") && leng < 50)
        {
            leng++;
            Array.Resize(ref dataSlots_setActive, leng);
            dataSlots_setActive[leng - 1] = new StaticResourcesChangeCtrl.DataSlot_SetActive();
            dataSlots_setActive[leng - 1].gos = new GameObject[languageStates.Length + 1];
            staticResourcesChangeCtrl.dataSlots_setActive = dataSlots_setActive;
        }
        if (GUILayout.Button("-") && leng >= 1)
        {
            leng--;
            Array.Resize(ref dataSlots_setActive, leng);
            staticResourcesChangeCtrl.dataSlots_setActive = dataSlots_setActive;
        }

        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < dataSlots_setActive.Length; i++)
        {
            if (dataSlots_setActive[i] == null)
            {
                dataSlots_setActive[i] = new StaticResourcesChangeCtrl.DataSlot_SetActive();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(" Slot " + i);
            EditorGUILayout.EndHorizontal();


            for (int j = 0; j < dataSlots_setActive[i].gos.Length; j++)
            {
                EditorGUILayout.BeginHorizontal();
                if (j == 0)
                {
                    EditorGUILayout.LabelField("    if (" + languageStates[j] + ")");
                }
                else if (j != dataSlots_setActive[i].gos.Length - 1)
                {
                    EditorGUILayout.LabelField("    else if (" + languageStates[j] + ")");           
                }
                else
                {
                    EditorGUILayout.LabelField("    else");
                }

                dataSlots_setActive[i].gos[j] = (GameObject)EditorGUILayout.ObjectField(dataSlots_setActive[i] == null ? null : dataSlots_setActive[i].gos[j], typeof(GameObject), true);
                
                EditorGUILayout.EndHorizontal();
            }

        }

        //////////////////////

        StaticResourcesChangeCtrl.DataSlot_SetTexture[] dataSlot_setTextures = staticResourcesChangeCtrl.dataSlots_setTexture;

        if (dataSlot_setTextures == null)
        {
            dataSlot_setTextures = new StaticResourcesChangeCtrl.DataSlot_SetTexture[0];
        }

        EditorGUILayout.BeginHorizontal();

        leng = dataSlot_setTextures.Length;

        if (leng > 0 && dataSlot_setTextures[0].tex.Length != (languageStates.Length + 1))
        {
            bool isAdd = (dataSlot_setTextures[0].tex.Length < (languageStates.Length + 1));

            for (int i = 0; i < dataSlot_setTextures.Length; i++)
            {
                if (!isAdd)
                {
                    dataSlot_setTextures[i].tex[languageStates.Length] = dataSlot_setTextures[i].tex[dataSlot_setTextures[i].tex.Length - 1];
                }
                Array.Resize(ref dataSlot_setTextures[i].tex, languageStates.Length + 1);
                if (isAdd)
                {
                    dataSlot_setTextures[i].tex[languageStates.Length] = dataSlot_setTextures[i].tex[languageStates.Length - 1];
                    dataSlot_setTextures[i].tex[languageStates.Length - 1] = null;
                }
            }
            staticResourcesChangeCtrl.dataSlots_setTexture = dataSlot_setTextures;
        }

        EditorGUILayout.LabelField("[ SetTextures - " + leng + " ]");

        if (GUILayout.Button("+") && leng < 50)
        {
            leng++;
            Array.Resize(ref dataSlot_setTextures, leng);
            dataSlot_setTextures[leng - 1] = new StaticResourcesChangeCtrl.DataSlot_SetTexture();
            dataSlot_setTextures[leng - 1].tex = new Texture[languageStates.Length + 1];
            staticResourcesChangeCtrl.dataSlots_setTexture = dataSlot_setTextures;
        }
        if (GUILayout.Button("-") && leng >= 1)
        {
            leng--;
            Array.Resize(ref dataSlot_setTextures, leng);
            staticResourcesChangeCtrl.dataSlots_setTexture = dataSlot_setTextures;
        }

        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < dataSlot_setTextures.Length; i++)
        {
            if (dataSlot_setTextures[i] == null)
            {
                dataSlot_setTextures[i] = new StaticResourcesChangeCtrl.DataSlot_SetTexture();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(" Slot " + i);

            dataSlot_setTextures[i].name = EditorGUILayout.TextField(dataSlot_setTextures[i].name);
            dataSlot_setTextures[i].mat = (Material)EditorGUILayout.ObjectField(dataSlot_setTextures[i] == null ? null : dataSlot_setTextures[i].mat, typeof(Material), true);
            EditorGUILayout.EndHorizontal();


            for (int j = 0; j < dataSlot_setTextures[i].tex.Length; j++)
            {
                EditorGUILayout.BeginHorizontal();
                if (j == 0)
                {
                    EditorGUILayout.LabelField("    if (" + languageStates[j] + ")");
                }
                else if (j != dataSlot_setTextures[i].tex.Length - 1)
                {
                    EditorGUILayout.LabelField("    else if (" + languageStates[j] + ")");
                }
                else
                {
                    EditorGUILayout.LabelField("    else");
                }

                dataSlot_setTextures[i].tex[j] = (Texture)EditorGUILayout.ObjectField(dataSlot_setTextures[i] == null ? null : dataSlot_setTextures[i].tex[j], typeof(Texture), true);

                EditorGUILayout.EndHorizontal();
            }

        }

        ///////////////////////
        ///
        StaticResourcesChangeCtrl.DataSlot_SetMesh[] dataSlot_setmesh = staticResourcesChangeCtrl.dataSlots_setMesh;

        if (dataSlot_setmesh == null)
        {
            dataSlot_setmesh = new StaticResourcesChangeCtrl.DataSlot_SetMesh[0];
        }

        EditorGUILayout.BeginHorizontal();

        leng = dataSlot_setmesh.Length;

        if (leng > 0 && dataSlot_setmesh[0].meshes.Length != (languageStates.Length + 1))
        {
            bool isAdd = (dataSlot_setmesh[0].meshes.Length < (languageStates.Length + 1));

            for (int i = 0; i < dataSlot_setmesh.Length; i++)
            {
                if (!isAdd)
                {
                    dataSlot_setmesh[i].meshes[languageStates.Length] = dataSlot_setmesh[i].meshes[dataSlot_setmesh[i].meshes.Length - 1];
                }
                Array.Resize(ref dataSlot_setmesh[i].meshes, languageStates.Length + 1);
                if (isAdd)
                {
                    dataSlot_setmesh[i].meshes[languageStates.Length] = dataSlot_setmesh[i].meshes[languageStates.Length - 1];
                    dataSlot_setmesh[i].meshes[languageStates.Length - 1] = null;
                }
            }
            staticResourcesChangeCtrl.dataSlots_setMesh = dataSlot_setmesh;
        }

        EditorGUILayout.LabelField("[ SetMeshes - " + leng + " ]");

        if (GUILayout.Button("+") && leng < 50)
        {
            leng++;
            Array.Resize(ref dataSlot_setmesh, leng);
            dataSlot_setmesh[leng - 1] = new StaticResourcesChangeCtrl.DataSlot_SetMesh();
            dataSlot_setmesh[leng - 1].meshes = new Mesh[languageStates.Length + 1];
            staticResourcesChangeCtrl.dataSlots_setMesh = dataSlot_setmesh;
        }
        if (GUILayout.Button("-") && leng >= 1)
        {
            leng--;
            Array.Resize(ref dataSlot_setmesh, leng);
            staticResourcesChangeCtrl.dataSlots_setMesh = dataSlot_setmesh;
        }

        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < dataSlot_setmesh.Length; i++)
        {
            if (dataSlot_setmesh[i] == null)
            {
                dataSlot_setmesh[i] = new StaticResourcesChangeCtrl.DataSlot_SetMesh();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(" Slot " + i);

            dataSlot_setmesh[i].meshFilter = (MeshFilter)EditorGUILayout.ObjectField(dataSlot_setmesh[i] == null ? null : dataSlot_setmesh[i].meshFilter, typeof(MeshFilter), true);
            EditorGUILayout.EndHorizontal();


            for (int j = 0; j < dataSlot_setmesh[i].meshes.Length; j++)
            {
                EditorGUILayout.BeginHorizontal();
                if (j == 0)
                {
                    EditorGUILayout.LabelField("    if (" + languageStates[j] + ")");
                }
                else if (j != dataSlot_setmesh[i].meshes.Length - 1)
                {
                    EditorGUILayout.LabelField("    else if (" + languageStates[j] + ")");
                }
                else
                {
                    EditorGUILayout.LabelField("    else");
                }

                dataSlot_setmesh[i].meshes[j] = (Mesh)EditorGUILayout.ObjectField(dataSlot_setmesh[i] == null ? null : dataSlot_setmesh[i].meshes[j], typeof(Mesh), true);

                EditorGUILayout.EndHorizontal();
            }

        }

        ///////////////////////
        ///
        StaticResourcesChangeCtrl.DataSlot_SetVoice[] dataSlots_setVoice = staticResourcesChangeCtrl.dataSlots_setVoice;

        if (dataSlots_setVoice == null)
        {
            dataSlots_setVoice = new StaticResourcesChangeCtrl.DataSlot_SetVoice[0];
        }

        EditorGUILayout.BeginHorizontal();

        leng = dataSlots_setVoice.Length;

        if (leng > 0 && dataSlots_setVoice[0].clipSlots.Length != (languageStates.Length + 1))
        {
            bool isAdd = (dataSlots_setVoice[0].clipSlots.Length < (languageStates.Length + 1));

            for (int i = 0; i < dataSlots_setVoice.Length; i++)
            {
                if (!isAdd)
                {
                    dataSlots_setVoice[i].clipSlots[languageStates.Length] = dataSlots_setVoice[i].clipSlots[dataSlots_setVoice[i].clipSlots.Length - 1];
                }
                Array.Resize(ref dataSlots_setVoice[i].clipSlots, languageStates.Length + 1);
                if (isAdd)
                {
                    dataSlots_setVoice[i].clipSlots[languageStates.Length] = dataSlots_setVoice[i].clipSlots[languageStates.Length - 1];
                    dataSlots_setVoice[i].clipSlots[languageStates.Length - 1] = null;
                }
            }
            staticResourcesChangeCtrl.dataSlots_setVoice = dataSlots_setVoice;
        }

        EditorGUILayout.LabelField("[ SetVoices - " + leng + " ]");

        if (GUILayout.Button("+") && leng < 50)
        {
            leng++;
            Array.Resize(ref dataSlots_setVoice, leng);
            dataSlots_setVoice[leng - 1] = new StaticResourcesChangeCtrl.DataSlot_SetVoice();
            dataSlots_setVoice[leng - 1].clipSlots = new StaticResourcesChangeCtrl.AudioClipSlot[languageStates.Length + 1];
            staticResourcesChangeCtrl.dataSlots_setVoice = dataSlots_setVoice;
        }
        if (GUILayout.Button("-") && leng >= 1)
        {
            leng--;
            Array.Resize(ref dataSlots_setVoice, leng);
            staticResourcesChangeCtrl.dataSlots_setVoice = dataSlots_setVoice;
        }

        EditorGUILayout.EndHorizontal();

        SerializedObject so = new SerializedObject(staticResourcesChangeCtrl);
        SerializedProperty serializedProperty = so.FindProperty("dataSlots_setVoice");
        //EditorGUILayout.PropertyField(serializedProperty.GetArrayElementAtIndex(0).FindPropertyRelative("clipSlots").GetArrayElementAtIndex(0));
  

        for (int i = 0; i < dataSlots_setVoice.Length; i++)
        {
            if (dataSlots_setVoice[i] == null)
            {
                dataSlots_setVoice[i] = new StaticResourcesChangeCtrl.DataSlot_SetVoice();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(" Slot " + i);

            dataSlots_setVoice[i].field = EditorGUILayout.TextField(dataSlots_setVoice[i].field);
            dataSlots_setVoice[i].sound_script = (MonoBehaviour)EditorGUILayout.ObjectField(dataSlots_setVoice[i] == null ? null : dataSlots_setVoice[i].sound_script, typeof(MonoBehaviour), true);
            EditorGUILayout.EndHorizontal();


            for (int j = 0; j < dataSlots_setVoice[i].clipSlots.Length; j++)
            {
                EditorGUILayout.BeginHorizontal();
                if (j == 0)
                {
                    //GUILayout.Label("    if (" + languageStates[j] + ")");
                    EditorGUILayout.PropertyField(serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("clipSlots").GetArrayElementAtIndex(j).FindPropertyRelative("clips"), new GUIContent("    if (" + languageStates[j] + ")"));
                }
                else if (j != dataSlots_setVoice[i].clipSlots.Length - 1)
                {
                    //GUILayout.Label("    else if (" + languageStates[j] + ")");
                    EditorGUILayout.PropertyField(serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("clipSlots").GetArrayElementAtIndex(j).FindPropertyRelative("clips"), new GUIContent("    else if (" + languageStates[j] + ")"));
                }
                else
                {
                    //GUILayout.Label("    else");
                    EditorGUILayout.PropertyField(serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("clipSlots").GetArrayElementAtIndex(j).FindPropertyRelative("clips"), new GUIContent("    else"));
                }

                //EditorGUILayout.PropertyField(serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("clipSlots").GetArrayElementAtIndex(j).FindPropertyRelative("clips"),new GUIContent());
                //dataSlots_setVoice[i].clipSlots[j] = (AudioClip)EditorGUILayout.ObjectField(dataSlots_setVoice[i] == null ? null : dataSlots_setVoice[i].clipSlots[j], typeof(AudioClip), true);

                EditorGUILayout.EndHorizontal();
            }

        }
        so.ApplyModifiedProperties();
        ///////////////////////

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
#endif