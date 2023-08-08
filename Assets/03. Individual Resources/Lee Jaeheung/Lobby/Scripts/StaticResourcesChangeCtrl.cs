using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonBase;
using UnityEngine.UI;
using System;
public class StaticResourcesChangeCtrl : Singleton<StaticResourcesChangeCtrl>
{
    public LanguageState[] languageStates;

    [System.Serializable]
    public class DataSlot_SetActive
    {
        public GameObject[] gos;
    }

    public DataSlot_SetActive[] dataSlots_setActive;

    [System.Serializable]
    public class DataSlot_SetTexture
    {
        public string name;
        public Material mat;
        public Texture[] tex;
    }

    public DataSlot_SetTexture[] dataSlots_setTexture;

    [System.Serializable]
    public class DataSlot_SetMesh
    {
        public MeshFilter meshFilter;
        public Mesh[] meshes;
    }

    public DataSlot_SetMesh[] dataSlots_setMesh;

    [System.Serializable]
    public class DataSlot_SetVoice
    {
        public string field;
        public MonoBehaviour sound_script;
        public AudioClipSlot[] clipSlots = new AudioClipSlot[0];
    }

    [System.Serializable]
    public class AudioClipSlot
    {
        public AudioClip[] clips = new AudioClip[0];
    }

    public DataSlot_SetVoice[] dataSlots_setVoice;

    private bool isInit = false;

    // Start is called before the first frame update
    void Start()
    {
        SetData();
    }

    public void SetData()
    {
        isInit = true;
        SetDataReduction();

        LanguageState languageState = GameSettingCtrl.GetLanguageState();

        int index_lang = languageStates.Length;
        for (int i = 0; i < languageStates.Length; i++)
        {
            if (languageState == languageStates[i])
            {
                index_lang = i;
                break;
            }
        }

        if (dataSlots_setActive != null)
        {
            for (int i = 0; i < dataSlots_setActive.Length; i++)
            {
                for (int j = 0; j < dataSlots_setActive[i].gos.Length; j++)
                {
                    dataSlots_setActive[i].gos[j].SetActive(j == index_lang);
                }
            }
        }

        if (dataSlots_setTexture != null)
        {
            for (int i = 0; i < dataSlots_setTexture.Length; i++)
            {
                dataSlots_setTexture[i].mat.SetTexture(dataSlots_setTexture[i].name, dataSlots_setTexture[i].tex[index_lang]);
            }
        }

        if (dataSlots_setMesh != null)
        {
            for (int i = 0; i < dataSlots_setMesh.Length; i++)
            {
                dataSlots_setMesh[i].meshFilter.mesh = dataSlots_setMesh[i].meshes[index_lang];
            }
        }

        if (dataSlots_setVoice != null)
        {
            for (int i = 0; i < dataSlots_setVoice.Length; i++)
            {
                Type t = dataSlots_setVoice[i].sound_script.GetType();
                System.Reflection.FieldInfo fieldInfo = t.GetField(dataSlots_setVoice[i].field);
                fieldInfo.SetValue(dataSlots_setVoice[i].sound_script, dataSlots_setVoice[i].clipSlots[index_lang].clips);
            }
        }
    }
    public void SetDataReduction()
    {
        //List<DataSlot> list_dataSlot = new List<DataSlot>();
        //for (int i = 0; i < dataSlots.Length; i++)
        //{
        //    if (dataSlots[i].ui == null || string.IsNullOrWhiteSpace(dataSlots[i].id))
        //    {
        //        continue;
        //    }
        //    list_dataSlot.Add(dataSlots[i]);
        //}

        //dataSlots = list_dataSlot.ToArray();
    }
}
