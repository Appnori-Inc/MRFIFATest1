using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonBase;
using UnityEngine.UI;
using System;
using TMPro;

public class StaticLocalizationCtrl_TextMesh : Singleton<StaticLocalizationCtrl_TextMesh>
{
    public TextMesh textMesh;
    public TextMeshPro textMeshPro;

    [System.Serializable]
    public class DataSlot
    {
        public string id;
        public TextMesh ui;
    }

    [System.Serializable]
    public class DataSlot_Pro
    {
        public string id;
        public TextMeshPro ui;
    }

    public DataSlot[] dataSlots;
    public DataSlot_Pro[] dataSlots_pro;

    private bool isInit = false;

    private void Start()
    {
        if (GameSettingCtrl.localizationInfos == null || GameSettingCtrl.localizationInfos.Count == 0)
        {
            return;
        }
        SetLocaliztion();
    }

    public void AddData(string _id, TextMesh ui_text)
    {
        Array.Resize(ref dataSlots, dataSlots.Length + 1);
        int index = dataSlots.Length - 1;
        dataSlots[index] = new DataSlot();
        dataSlots[index].id = _id;
        dataSlots[index].ui = ui_text;

        if (isInit)
        {
            dataSlots[index].ui.text = GameSettingCtrl.GetLocalizationText(dataSlots[index].id);
        }
    }
    public void AddData(string _id, TextMeshPro ui_text)
    {
        Array.Resize(ref dataSlots_pro, dataSlots_pro.Length + 1);
        int index = dataSlots_pro.Length - 1;
        dataSlots_pro[index] = new DataSlot_Pro();
        dataSlots_pro[index].id = _id;
        dataSlots_pro[index].ui = ui_text;

        if (isInit)
        {
            dataSlots_pro[index].ui.text = GameSettingCtrl.GetLocalizationText(dataSlots_pro[index].id);
        }
    }

    public void SetLocaliztion()
    {
        isInit = true;
        SetDataReduction();

        for (int i = 0; i < dataSlots.Length; i++)
        {
            dataSlots[i].ui.text = GameSettingCtrl.GetLocalizationText(dataSlots[i].id);
        }

        for (int i = 0; i < dataSlots_pro.Length; i++)
        {
            dataSlots_pro[i].ui.text = GameSettingCtrl.GetLocalizationText(dataSlots_pro[i].id);
        }
    }

    public void SetDataReduction()
    {
        List<DataSlot> list_dataSlot = new List<DataSlot>();
        for (int i = 0; i < dataSlots.Length; i++)
        {
            if (dataSlots[i].ui == null || string.IsNullOrWhiteSpace(dataSlots[i].id))
            {
                continue;
            }
            list_dataSlot.Add(dataSlots[i]);
        }

        dataSlots = list_dataSlot.ToArray();

        List<DataSlot_Pro> list_dataSlot_pro = new List<DataSlot_Pro>();
        for (int i = 0; i < dataSlots_pro.Length; i++)
        {
            if (dataSlots_pro[i].ui == null || string.IsNullOrWhiteSpace(dataSlots_pro[i].id))
            {
                continue;
            }
            list_dataSlot_pro.Add(dataSlots_pro[i]);
        }

        dataSlots_pro = list_dataSlot_pro.ToArray();
    }
}
