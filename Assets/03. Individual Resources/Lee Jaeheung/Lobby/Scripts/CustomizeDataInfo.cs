using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonBase;

public class CustomizeDataInfo : Singleton<CustomizeDataInfo>
{
    [System.Serializable]
    public class JsonParsing<T> where T : class
    {

        public List<T> list_itemInfo;


        public JsonParsing(int group, string csvName)
        {
            string orizin_csv = File.ReadAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/" + csvName);
            string[] split_csv = orizin_csv.Split('\n');

            list_itemInfo = new List<T>();

            if (typeof(T) == typeof(ItemInfo))
            {
                for (int i = 1; i < split_csv.Length; i++)
                {
                    list_itemInfo.Add((new ItemInfo(group, split_csv[i])) as T);
                }
            }
            else if (typeof(T) == typeof(ItemSlotInfo))
            {
                for (int i = 1; i < split_csv.Length; i++)
                {
                    list_itemInfo.Add((new ItemSlotInfo(group, split_csv[i])) as T);
                }
            }
        }
    }

    [System.Serializable]
    public class ItemInfo
    {
        public string id;
        public string name;
        public string meshPath;
        public string matPath;

        public ItemInfo(int group, string csv)
        {
            string[] split_info = csv.Split(',');

            id = group.ToString() + split_info[2][0] + split_info[0];
            name = split_info[1];
            if (group != 1)
            {
                meshPath = split_info[3];
                matPath = split_info[4].Split('\r')[0];
            }
            else
            {
                matPath = split_info[3].Split('\r')[0];
            }
        }
    }


    [System.Serializable]
    public class ItemSlotInfo
    {
        public string id;
        public string name;
        public List<string> list_partId;

        public ItemSlotInfo(int group, string csv)
        {
            string[] split_info = csv.Split(',');

            id = group.ToString() + split_info[0];
            name = split_info[1];
            list_partId = new List<string>();
            for (int i = 2; i < split_info.Length; i++)
            {
                if (split_info[i] != "" && split_info[i] != "\r")
                {
                    list_partId.Add(group.ToString() + (i - 2).ToString() + split_info[i].Split('\r')[0]);
                }
            }
        }
    }

    private Dictionary<string, ItemInfo> dict_itemInfo;
    private Dictionary<string, ItemSlotInfo>[] dict_itemSlotInfos;

    [System.Serializable]
    public class WearInfo
    {
        public Vector3 bounds_center;
        public Vector3 bounds_extent;

        public int rootBone;
        public int[] bones;
    }

    [System.Serializable]
    public class AccInfo
    {
        public Vector3 pos;
        public Vector3 scale;
    }

    private bool isInit = false;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        Init();
    }

    public void Init()
    {
        if (isInit)
        {
            return;
        }

        dict_itemInfo = new Dictionary<string, ItemInfo>();

        TextAsset jsonText = Resources.Load("ItemInfos/ItemInfo_Face_Item") as TextAsset;
        List<ItemInfo> itemInfos = JsonUtility.FromJson<JsonParsing<ItemInfo>>(jsonText.text).list_itemInfo;

        for (int i = 0; i < itemInfos.Count; i++)
        {
            //Debug.Log(itemInfos[i].id);
            dict_itemInfo.Add(itemInfos[i].id, itemInfos[i]);
        }

        jsonText = Resources.Load("ItemInfos/ItemInfo_Hair_Item") as TextAsset;
        itemInfos.Clear();
        itemInfos = JsonUtility.FromJson<JsonParsing<ItemInfo>>(jsonText.text).list_itemInfo;

        for (int i = 0; i < itemInfos.Count; i++)
        {
            dict_itemInfo.Add(itemInfos[i].id, itemInfos[i]);
        }

        jsonText = Resources.Load("ItemInfos/ItemInfo_Wear_Item") as TextAsset;
        itemInfos.Clear();
        itemInfos = JsonUtility.FromJson<JsonParsing<ItemInfo>>(jsonText.text).list_itemInfo;

        for (int i = 0; i < itemInfos.Count; i++)
        {
            dict_itemInfo.Add(itemInfos[i].id, itemInfos[i]);
        }

        jsonText = Resources.Load("ItemInfos/ItemInfo_Acc_Item") as TextAsset;
        itemInfos.Clear();
        itemInfos = JsonUtility.FromJson<JsonParsing<ItemInfo>>(jsonText.text).list_itemInfo;

        for (int i = 0; i < itemInfos.Count; i++)
        {
            dict_itemInfo.Add(itemInfos[i].id, itemInfos[i]);
        }

        ///////////////////////


        dict_itemSlotInfos = new Dictionary<string, ItemSlotInfo>[2];

        for (int i = 0; i < dict_itemSlotInfos.Length; i++)
        {
            dict_itemSlotInfos[i] = new Dictionary<string, ItemSlotInfo>();

            if (i == 0)
            {
                jsonText = Resources.Load("ItemInfos/ItemInfo_Face_M_ItemUISlot") as TextAsset;
            }
            else
            {
                jsonText = Resources.Load("ItemInfos/ItemInfo_Face_F_ItemUISlot") as TextAsset;
            }

            List<ItemSlotInfo> list_itemSlot_face = JsonUtility.FromJson<JsonParsing<ItemSlotInfo>>(jsonText.text).list_itemInfo;

            for (int j = 0; j < list_itemSlot_face.Count; j++)
            {
                //Debug.Log(itemInfos[i].id);
                dict_itemSlotInfos[i].Add(list_itemSlot_face[j].id, list_itemSlot_face[j]);
            }
            ////////////////////////
            if (i == 0)
            {
                jsonText = Resources.Load("ItemInfos/ItemInfo_Hair_M_ItemUISlot") as TextAsset;
            }
            else
            {
                jsonText = Resources.Load("ItemInfos/ItemInfo_Hair_F_ItemUISlot") as TextAsset;
            }

            List<ItemSlotInfo> list_itemSlot_hair = JsonUtility.FromJson<JsonParsing<ItemSlotInfo>>(jsonText.text).list_itemInfo;

            for (int j = 0; j < list_itemSlot_hair.Count; j++)
            {
                //Debug.Log(itemInfos[i].id);
                dict_itemSlotInfos[i].Add(list_itemSlot_hair[j].id, list_itemSlot_hair[j]);
            }
            /////////////////////////
            if (i == 0)
            {
                jsonText = Resources.Load("ItemInfos/ItemInfo_Wear_M_ItemUISlot") as TextAsset;
            }
            else
            {
                jsonText = Resources.Load("ItemInfos/ItemInfo_Wear_F_ItemUISlot") as TextAsset;
            }

            List<ItemSlotInfo> list_itemSlot_wear = JsonUtility.FromJson<JsonParsing<ItemSlotInfo>>(jsonText.text).list_itemInfo;

            for (int j = 0; j < list_itemSlot_wear.Count; j++)
            {
                //Debug.Log(itemInfos[i].id);
                dict_itemSlotInfos[i].Add(list_itemSlot_wear[j].id, list_itemSlot_wear[j]);
            }
            ///////////////////////////
            if (i == 0)
            {
                jsonText = Resources.Load("ItemInfos/ItemInfo_Acc_M_ItemUISlot") as TextAsset;
            }
            else
            {
                jsonText = Resources.Load("ItemInfos/ItemInfo_Acc_F_ItemUISlot") as TextAsset;
            }

            List<ItemSlotInfo> list_itemSlot_acc = JsonUtility.FromJson<JsonParsing<ItemSlotInfo>>(jsonText.text).list_itemInfo;

            for (int j = 0; j < list_itemSlot_acc.Count; j++)
            {
                //Debug.Log(itemInfos[i].id);
                dict_itemSlotInfos[i].Add(list_itemSlot_acc[j].id, list_itemSlot_acc[j]);
            }
        }

        isInit = true;
    }

    public bool IsInit()
    {
        return isInit;
    }

    public ItemInfo GetItemInfo(string _id)
    {
        Init();

        if (dict_itemInfo.ContainsKey(_id))
        {
            return dict_itemInfo[_id];
        }

        return null;
    }

    public ItemSlotInfo GetItemSlotInfo(int genderNum, string _id)
    {
        Init();

        if (dict_itemSlotInfos[genderNum].ContainsKey(_id))
        {
            return dict_itemSlotInfos[genderNum][_id];
        }

        return null;
    }
}
