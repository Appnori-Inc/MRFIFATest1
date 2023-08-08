using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonBase;
using UnityEngine.UI;
using System;
public class StaticLocalizationCtrl : Singleton<StaticLocalizationCtrl>
{
    [System.Serializable]
    public class DataSlot
    {
        public string id;
        public Text ui;
    }

    public DataSlot[] dataSlots;

    private bool isInit = false;

    private void Start()
    {
        if (GameSettingCtrl.localizationInfos == null || GameSettingCtrl.localizationInfos.Count == 0)
        {
            return;
        }
        SetLocaliztion();
    }

    public void AddData(string _id, Text ui_text)
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

    public void SetLocaliztion()
    {
        isInit = true;
        SetDataReduction();

        for (int i = 0; i < dataSlots.Length; i++)
        {
            dataSlots[i].ui.text = GameSettingCtrl.GetLocalizationText(dataSlots[i].id);
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
    }
}
