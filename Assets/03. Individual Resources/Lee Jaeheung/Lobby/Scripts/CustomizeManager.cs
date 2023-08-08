using SingletonBase;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public partial class CustomizeManager : Singleton<CustomizeManager>
{
    private List<CustomizeDataInfo.ItemSlotInfo> list_itemSlot_face_M = new List<CustomizeDataInfo.ItemSlotInfo>();
    private List<CustomizeDataInfo.ItemSlotInfo> list_itemSlot_hair_M = new List<CustomizeDataInfo.ItemSlotInfo>();
    private List<CustomizeDataInfo.ItemSlotInfo> list_itemSlot_wear_M = new List<CustomizeDataInfo.ItemSlotInfo>();
    private List<CustomizeDataInfo.ItemSlotInfo> list_itemSlot_acc_M = new List<CustomizeDataInfo.ItemSlotInfo>();

    private List<CustomizeDataInfo.ItemSlotInfo> list_itemSlot_face_F = new List<CustomizeDataInfo.ItemSlotInfo>();
    private List<CustomizeDataInfo.ItemSlotInfo> list_itemSlot_hair_F = new List<CustomizeDataInfo.ItemSlotInfo>();
    private List<CustomizeDataInfo.ItemSlotInfo> list_itemSlot_wear_F = new List<CustomizeDataInfo.ItemSlotInfo>();
    private List<CustomizeDataInfo.ItemSlotInfo> list_itemSlot_acc_F = new List<CustomizeDataInfo.ItemSlotInfo>();

    private List<CustomizeProp> list_ui_prop = new List<CustomizeProp>();
    private CustomizeItemCtrl[] items_face = new CustomizeItemCtrl[9];
    private CustomizeItemCtrl[] items_hair = new CustomizeItemCtrl[9];
    private CustomizeItemCtrl[] items_wear = new CustomizeItemCtrl[9];
    private CustomizeItemCtrl[] items_acc = new CustomizeItemCtrl[9];

    private UIPageCtrl[] uiPages = new UIPageCtrl[4];
    private int page_max_face;
    private int page_current_face;
    private int page_max_hair;
    private int page_current_hair;
    private int page_max_wear; 
    private int page_current_wear;
    private int page_max_acc;
    private int page_current_acc;

    public ColorPaletteCtrl colorPalette = null;

    private bool isInit = false;

    private CustomizeProp[] keep_items = new CustomizeProp[2];

    public Transform customSetPos_head;
    public Transform customSetPos_body;

    public MyCharacterCtrl characterCtrl;

    public AllModelData modelData;

    public int selectGenderNum;

    public MeshButtonCtrl[] buttons_gender;

    public Mesh[] meshes_head;

    public Material[] materials_customUI;

    public GameObject[] icons_lock;
    public LockPropInfo[] lockPropInfos;

    private string keep_check_id;
    public Text text_lock;
   

   
    [System.Serializable]
    public class LockPropInfo
    {
        public string slot_id;
        public string api_name;
    }

    [System.Serializable]
    public class AllModelData
    {
        public CustomModelData[] modelDatas;
    }

    public GameDataManager gameDataManager;

    // Start is called before the first frame update
    void Start()
    {
        isInit = false;

        keep_check_id = "";
        text_lock.transform.parent.gameObject.SetActive(false);

        gameDataManager = GameDataManager.instance;

       /* if (PublicGameUIManager.GetInstance)
        {
            //PublicGameUIManager.GetInstance.AddMenuEvent((a) => { Debug.LogError("MenuClick - " + a); });
            //PublicGameUIManager.GetInstance.AddMenuEvent(MenuClick);
        }*/

        Read_UIInfo();
        Download_UserData();
        InitPurchase();
        Invoke("SetAchieve", 1.5f);
        StartCoroutine(InitDataCoroutine());
    }

    void SetAchieve()
    {
        gameDataManager.UnlockAchieve("Ach10", 1);
    }

    public void Read_UIInfo()
    {
        meshes_head = new Mesh[2];
        meshes_head[0] = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_HEAD");
        meshes_head[1] = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_HEAD");

        list_itemSlot_face_M = new List<CustomizeDataInfo.ItemSlotInfo>();

        TextAsset jsonText = Resources.Load("ItemInfos/ItemInfo_Face_M_ItemUISlot") as TextAsset;
        list_itemSlot_face_M = JsonUtility.FromJson<CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>>(jsonText.text).list_itemInfo;

        list_itemSlot_face_F = new List<CustomizeDataInfo.ItemSlotInfo>();

        jsonText = Resources.Load("ItemInfos/ItemInfo_Face_F_ItemUISlot") as TextAsset;
        list_itemSlot_face_F = JsonUtility.FromJson<CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>>(jsonText.text).list_itemInfo;

        list_itemSlot_hair_M = new List<CustomizeDataInfo.ItemSlotInfo>();

        jsonText = Resources.Load("ItemInfos/ItemInfo_Hair_M_ItemUISlot") as TextAsset;
        list_itemSlot_hair_M = JsonUtility.FromJson<CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>>(jsonText.text).list_itemInfo;

        list_itemSlot_hair_F = new List<CustomizeDataInfo.ItemSlotInfo>();

        jsonText = Resources.Load("ItemInfos/ItemInfo_Hair_F_ItemUISlot") as TextAsset;
        list_itemSlot_hair_F = JsonUtility.FromJson<CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>>(jsonText.text).list_itemInfo;

        list_itemSlot_wear_M = new List<CustomizeDataInfo.ItemSlotInfo>();

        jsonText = Resources.Load("ItemInfos/ItemInfo_Wear_M_ItemUISlot") as TextAsset;
        list_itemSlot_wear_M = JsonUtility.FromJson<CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>>(jsonText.text).list_itemInfo;

        list_itemSlot_wear_F = new List<CustomizeDataInfo.ItemSlotInfo>();

        jsonText = Resources.Load("ItemInfos/ItemInfo_Wear_F_ItemUISlot") as TextAsset;
        list_itemSlot_wear_F = JsonUtility.FromJson<CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>>(jsonText.text).list_itemInfo;

        list_itemSlot_acc_M = new List<CustomizeDataInfo.ItemSlotInfo>();

        jsonText = Resources.Load("ItemInfos/ItemInfo_Acc_M_ItemUISlot") as TextAsset;
        list_itemSlot_acc_M = JsonUtility.FromJson<CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>>(jsonText.text).list_itemInfo;

        list_itemSlot_acc_F = new List<CustomizeDataInfo.ItemSlotInfo>();

        jsonText = Resources.Load("ItemInfos/ItemInfo_Acc_F_ItemUISlot") as TextAsset;
        list_itemSlot_acc_F = JsonUtility.FromJson<CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>>(jsonText.text).list_itemInfo;

        list_itemSlot_face_M.Reverse();
        list_itemSlot_face_F.Reverse();
        list_itemSlot_hair_M.Reverse();
        list_itemSlot_hair_F.Reverse();
        list_itemSlot_wear_M.Reverse();
        list_itemSlot_wear_F.Reverse();
        list_itemSlot_acc_M.Reverse(1, list_itemSlot_acc_M.Count - 1);
        list_itemSlot_acc_F.Reverse(1, list_itemSlot_acc_M.Count - 1);
    }

    IEnumerator InitDataCoroutine()
    {
        while (modelData == null || modelData.modelDatas == null || modelData.modelDatas.Length == 0)
        {
            yield return null;
        }

        while (!CustomizeDataInfo.GetInstance.IsInit())
        {
            yield return null;
        }

        for (int i = 0; i < items_face.Length; i++)
        {
            while (items_face[i] == null)
            {
                yield return null;
            }
        }

        for (int i = 0; i < items_hair.Length; i++)
        {
            while (items_hair[i] == null)
            {
                yield return null;
            }
        }

        for (int i = 0; i < items_wear.Length; i++)
        {
            while (items_wear[i] == null)
            {
                yield return null;
            }
        }

        for (int i = 0; i < items_acc.Length; i++)
        {
            while (items_acc[i] == null)
            {
                yield return null;
            }
        }

        for (int i = 0; i < uiPages.Length; i++)
        {
            while (uiPages[i] == null)
            {
                yield return null;
            }
        }

        while (colorPalette == null)
        {
            yield return null;
        }

       
       
       
        characterCtrl.SetGender(selectGenderNum);

        isInit = true;
        MeshFadeCtrl.instance.StartFade(true);
    }

    public void CheckSlotLock(CustomizeItemCtrl item)
    {
#if PICO_PLATFORM || QIYU_ACCOUNT
        return;
#endif

        if (item == null || keep_check_id == item.slotInfo.id)
        {
            return;
        }

        keep_check_id = item.slotInfo.id;

        if (item.isLock)
        {
            for (int i = 0; i < lockPropInfos.Length; i++)
            {
                if (lockPropInfos[i].slot_id == keep_check_id)
                {
                    text_lock.text = gameDataManager.GetAchieveInfo(lockPropInfos[i].api_name);
                    text_lock.transform.parent.position = item.transform.position + item.transform.up * 0.1f + item.transform.right * -0.1f + item.transform.forward * -0.75f * item.transform.localPosition.x;
                    text_lock.transform.parent.gameObject.SetActive(true);

                    return;
                }
            }
        }
        text_lock.transform.parent.gameObject.SetActive(false);
    }

    public void SetUI()
    {
        if (setUICoroutine != null)
        {
            StopCoroutine(setUICoroutine);
        }
        setUICoroutine = StartCoroutine(SetUICoroutine());
    }

    private Coroutine setUICoroutine;

    IEnumerator SetUICoroutine()
    {
        while (colorPalette == null)
        {
            yield return null;
        }

        colorPalette.SetSlot();

        switch (selectGenderNum)
        {
            case 0:
                {
                    page_max_face = (list_itemSlot_face_M.Count - 1) / 9;
                    int i = 0;
                    for (i = 0; i < list_itemSlot_face_M.Count; i++)
                    {
                        if (list_itemSlot_face_M[i].id == modelData.modelDatas[selectGenderNum].ID_Face_I)
                        {
                            break;
                        }
                    }
                    page_current_face = (i - 1) / 9;
                    uiPages[0].SetPage(page_current_face, page_max_face);

                    page_max_hair = (list_itemSlot_hair_M.Count - 1) / 9;
                    i = 0;
                    for (i = 0; i < list_itemSlot_hair_M.Count; i++)
                    {
                        if (list_itemSlot_hair_M[i].id == modelData.modelDatas[selectGenderNum].ID_Hair_I)
                        {
                            break;
                        }
                    }
                    page_current_hair = (i - 1) / 9;
                    uiPages[1].SetPage(page_current_hair, page_max_hair);

                    page_max_wear = (list_itemSlot_wear_M.Count - 1) / 9;
                    i = 0;
                    for (i = 0; i < list_itemSlot_wear_M.Count; i++)
                    {
                        if (list_itemSlot_wear_M[i].id == modelData.modelDatas[selectGenderNum].ID_Wear_I)
                        {
                            break;
                        }
                    }
                    page_current_wear = (i - 1) / 9;
                    uiPages[2].SetPage(page_current_wear, page_max_wear);

                    page_max_acc = (list_itemSlot_acc_M.Count - 1) / 9;
                    i = 0;
                    for (i = 0; i < list_itemSlot_acc_M.Count; i++)
                    {
                        if (list_itemSlot_acc_M[i].id == modelData.modelDatas[selectGenderNum].ID_Acc_I)
                        {
                            break;
                        }
                    }
                    page_current_acc = (i - 1) / 9;
                    uiPages[3].SetPage(page_current_acc, page_max_acc);
                }
                break;
            case 1:
                {
                    page_max_face = (list_itemSlot_face_F.Count - 1) / 9;
                    int i = 0;
                    for (i = 0; i < list_itemSlot_face_F.Count; i++)
                    {
                        if (list_itemSlot_face_F[i].id == modelData.modelDatas[selectGenderNum].ID_Face_I)
                        {
                            break;
                        }
                    }
                    page_current_face = (i - 1) / 9;
                    uiPages[0].SetPage(page_current_face, page_max_face);

                    page_max_hair = (list_itemSlot_hair_F.Count - 1) / 9;
                    i = 0;
                    for (i = 0; i < list_itemSlot_hair_F.Count; i++)
                    {
                        if (list_itemSlot_hair_F[i].id == modelData.modelDatas[selectGenderNum].ID_Hair_I)
                        {
                            break;
                        }
                    }
                    page_current_hair = (i - 1) / 9;
                    uiPages[1].SetPage(page_current_hair, page_max_hair);

                    page_max_wear = (list_itemSlot_wear_F.Count - 1) / 9;
                    i = 0;
                    for (i = 0; i < list_itemSlot_wear_F.Count; i++)
                    {
                        if (list_itemSlot_wear_F[i].id == modelData.modelDatas[selectGenderNum].ID_Wear_I)
                        {
                            break;
                        }
                    }
                    page_current_wear = (i - 1) / 9;
                    uiPages[2].SetPage(page_current_wear, page_max_wear);

                    page_max_acc = (list_itemSlot_acc_F.Count - 1) / 9;
                    i = 0;
                    for (i = 0; i < list_itemSlot_acc_F.Count; i++)
                    {
                        if (list_itemSlot_acc_F[i].id == modelData.modelDatas[selectGenderNum].ID_Acc_I)
                        {
                            break;
                        }
                    }
                    page_current_acc = (i - 1) / 9;
                    uiPages[3].SetPage(page_current_acc, page_max_acc);
                }
                break;
        }

        for (int i = 0; i < items_face.Length; i++)
        {
            while (items_face[i] == null)
            {
                yield return null;
            }

            switch (selectGenderNum)
            {
                case 0:
                    {
                        if ((page_current_face * 9 + i) < list_itemSlot_face_M.Count)
                        {
                            items_face[i].SetSlot(list_itemSlot_face_M[page_current_face * 9 + i]);
                        }
                        else
                        {
                            items_face[i].SetSlot(null);
                        }
                    }
                    break;
                case 1:
                    {
                        if ((page_current_face * 9 + i) < list_itemSlot_face_F.Count)
                        {
                            items_face[i].SetSlot(list_itemSlot_face_F[page_current_face * 9 + i]);
                        }
                        else
                        {
                            items_face[i].SetSlot(null);
                        }
                    }
                    break;
            }
        }

        for (int i = 0; i < items_hair.Length; i++)
        {
            while (items_hair[i] == null)
            {
                yield return null;
            }

            switch (selectGenderNum)
            {
                case 0:
                    {
                        if ((page_current_hair * 9 + i) < list_itemSlot_hair_M.Count)
                        {
                            items_hair[i].SetSlot(list_itemSlot_hair_M[page_current_hair * 9 + i]);
                        }
                        else
                        {
                            items_hair[i].SetSlot(null);
                        }
                    }
                    break;
                case 1:
                    {
                        if ((page_current_hair * 9 + i) < list_itemSlot_hair_F.Count)
                        {
                            items_hair[i].SetSlot(list_itemSlot_hair_F[page_current_hair * 9 + i]);
                        }
                        else
                        {
                            items_hair[i].SetSlot(null);
                        }
                    }
                    break;
            }
        }

        for (int i = 0; i < items_wear.Length; i++)
        {
            while (items_wear[i] == null)
            {
                yield return null;
            }

            switch (selectGenderNum)
            {
                case 0:
                    {
                        if ((page_current_wear * 9 + i) < list_itemSlot_wear_M.Count)
                        {
                            items_wear[i].SetSlot(list_itemSlot_wear_M[page_current_wear * 9 + i]);
                        }
                        else
                        {
                            items_wear[i].SetSlot(null);
                        }
                    }
                    break;
                case 1:
                    {
                        if ((page_current_wear * 9 + i) < list_itemSlot_wear_F.Count)
                        {
                            items_wear[i].SetSlot(list_itemSlot_wear_F[page_current_wear * 9 + i]);
                        }
                        else
                        {
                            items_wear[i].SetSlot(null);
                        }
                    }
                    break;
            }
        }

        for (int i = 0; i < items_acc.Length; i++)
        {
            while (items_acc[i] == null)
            {
                yield return null;
            }
            int index = page_current_acc * 9 + i;
            switch (selectGenderNum)
            {
                case 0:
                    {
                        if (index < list_itemSlot_acc_M.Count)
                        {
                            items_acc[i].SetSlot(list_itemSlot_acc_M[index]);
                        }
                        else
                        {
                            items_acc[i].SetSlot(null);
                        }
                    }
                    break;
                case 1:
                    {
                        if (index < list_itemSlot_acc_F.Count)
                        {
                            items_acc[i].SetSlot(list_itemSlot_acc_F[index]);
                        }
                        else
                        {
                            items_acc[i].SetSlot(null);
                        }
                    }
                    break;
            }

#if PICO_PLATFORM || QIYU_ACCOUNT
            items_acc[i].isLock = false;
            icons_lock[i].SetActive(false);
#else
            int j = 0;
            for (; j < lockPropInfos.Length; j++)
            {
                if (items_acc[i].slotInfo != null && lockPropInfos[j].slot_id == items_acc[i].slotInfo.id)
                {
                    if (lockPropInfos[j].api_name == "-" || gameDataManager.IsUnlockAchieve(lockPropInfos[j].api_name))
                    {
                        items_acc[i].isLock = false;
                        icons_lock[i].SetActive(false);
                    }
                    else
                    {
                        items_acc[i].isLock = true;
                        icons_lock[i].SetActive(true);
                    }
                    break;
                }
            }

            if (j == lockPropInfos.Length)
            {
                items_acc[i].isLock = false;
                icons_lock[i].SetActive(false);
            }
#endif
        }
    }

    public void SetNextPage(int state)
    {
        switch (state)
        {
            case 0:
                {
                    if (page_current_face + 1 > page_max_face)
                    {
                        break;
                    }
                    page_current_face++;

                    for (int i = 0; i < items_face.Length; i++)
                    {
                        if (items_face[i] == null)
                        {
                            break;
                        }

                        switch (selectGenderNum)
                        {
                            case 0:
                                {
                                    if ((page_current_face * 9 + i) < list_itemSlot_face_M.Count)
                                    {
                                        items_face[i].SetSlot(list_itemSlot_face_M[page_current_face * 9 + i]);
                                    }
                                    else
                                    {
                                        items_face[i].SetSlot(null);
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if ((page_current_face * 9 + i) < list_itemSlot_face_F.Count)
                                    {
                                        items_face[i].SetSlot(list_itemSlot_face_F[page_current_face * 9 + i]);
                                    }
                                    else
                                    {
                                        items_face[i].SetSlot(null);
                                    }
                                }
                                break;
                        }
                    }
                    uiPages[state].SetPage(page_current_face, page_max_face);
                }
                break;
            case 1:
                {
                    if (page_current_hair + 1 > page_max_hair)
                    {
                        break;
                    }
                    page_current_hair++;

                    for (int i = 0; i < items_hair.Length; i++)
                    {
                        if (items_hair[i] == null)
                        {
                            break;
                        }

                        switch (selectGenderNum)
                        {
                            case 0:
                                {
                                    if ((page_current_hair * 9 + i) < list_itemSlot_hair_M.Count)
                                    {
                                        items_hair[i].SetSlot(list_itemSlot_hair_M[page_current_hair * 9 + i]);
                                    }
                                    else
                                    {
                                        items_hair[i].SetSlot(null);
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if ((page_current_hair * 9 + i) < list_itemSlot_hair_F.Count)
                                    {
                                        items_hair[i].SetSlot(list_itemSlot_hair_F[page_current_hair * 9 + i]);
                                    }
                                    else
                                    {
                                        items_hair[i].SetSlot(null);
                                    }
                                }
                                break;
                        }
                    }
                    uiPages[state].SetPage(page_current_hair, page_max_hair);
                }
                break;
            case 2:
                {
                    if (page_current_wear + 1 > page_max_wear)
                    {
                        break;
                    }
                    page_current_wear++;

                    for (int i = 0; i < items_wear.Length; i++)
                    {
                        if (items_wear[i] == null)
                        {
                            break;
                        }

                        switch (selectGenderNum)
                        {
                            case 0:
                                {
                                    if ((page_current_wear * 9 + i) < list_itemSlot_wear_M.Count)
                                    {
                                        items_wear[i].SetSlot(list_itemSlot_wear_M[page_current_wear * 9 + i]);
                                    }
                                    else
                                    {
                                        items_wear[i].SetSlot(null);
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if ((page_current_wear * 9 + i) < list_itemSlot_wear_F.Count)
                                    {
                                        items_wear[i].SetSlot(list_itemSlot_wear_F[page_current_wear * 9 + i]);
                                    }
                                    else
                                    {
                                        items_wear[i].SetSlot(null);
                                    }
                                }
                                break;
                        }
                    }
                    uiPages[state].SetPage(page_current_wear, page_max_wear);
                }
                break;
            case 3:
                {
                    if (page_current_acc + 1 > page_max_acc)
                    {
                        break;
                    }
                    page_current_acc++;

                    for (int i = 0; i < items_acc.Length; i++)
                    {
                        if (items_acc[i] == null)
                        {
                            break;
                        }
                        int index = page_current_acc * 9 + i;
                        switch (selectGenderNum)
                        {
                            case 0:
                                {
                                    if (index < list_itemSlot_acc_M.Count)
                                    {
                                        items_acc[i].SetSlot(list_itemSlot_acc_M[index]);
                                    }
                                    else
                                    {
                                        items_acc[i].SetSlot(null);
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if (index < list_itemSlot_acc_F.Count)
                                    {
                                        items_acc[i].SetSlot(list_itemSlot_acc_F[index]);
                                    }
                                    else
                                    {
                                        items_acc[i].SetSlot(null);
                                    }
                                }
                                break;
                        }
#if PICO_PLATFORM || QIYU_ACCOUNT
                        items_acc[i].isLock = false;
                        icons_lock[i].SetActive(false);
#else
                        int j = 0;
                        for (; j < lockPropInfos.Length; j++)
                        {
                            if (items_acc[i].slotInfo != null && lockPropInfos[j].slot_id == items_acc[i].slotInfo.id)
                            {
                                if (lockPropInfos[j].api_name == "-" || gameDataManager.IsUnlockAchieve(lockPropInfos[j].api_name))
                                {
                                    items_acc[i].isLock = false;
                                    icons_lock[i].SetActive(false);
                                }
                                else
                                {
                                    items_acc[i].isLock = true;
                                    icons_lock[i].SetActive(true);
                                }
                                break;
                            }
                        }

                        if (j == lockPropInfos.Length)
                        {
                            items_acc[i].isLock = false;
                            icons_lock[i].SetActive(false);
                        }
#endif
                    }
                    uiPages[state].SetPage(page_current_acc, page_max_acc);
                }
                break;
        }
    }

    public void SetBackPage(int state)
    {
        switch (state)
        {
            case 0:
                {
                    if (page_current_face - 1 < 0)
                    {
                        break;
                    }
                    page_current_face--;

                    for (int i = 0; i < items_face.Length; i++)
                    {
                        if (items_face[i] == null)
                        {
                            break;
                        }

                        switch (selectGenderNum)
                        {
                            case 0:
                                {
                                    items_face[i].SetSlot(list_itemSlot_face_M[page_current_face * 9 + i]);
                                }
                                break;
                            case 1:
                                {
                                    items_face[i].SetSlot(list_itemSlot_face_F[page_current_face * 9 + i]);
                                }
                                break;
                        }
                    }
                    uiPages[state].SetPage(page_current_face, page_max_face);
                }
                break;
            case 1:
                {
                    if (page_current_hair - 1 < 0)
                    {
                        break;
                    }
                    page_current_hair--;

                    for (int i = 0; i < items_hair.Length; i++)
                    {
                        if (items_hair[i] == null)
                        {
                            break;
                        }

                        switch (selectGenderNum)
                        {
                            case 0:
                                {
                                    items_hair[i].SetSlot(list_itemSlot_hair_M[page_current_hair * 9 + i]);
                                }
                                break;
                            case 1:
                                {
                                    items_hair[i].SetSlot(list_itemSlot_hair_F[page_current_hair * 9 + i]);
                                }
                                break;
                        }
                    }
                    uiPages[state].SetPage(page_current_hair, page_max_hair);
                }
                break;
            case 2:
                {
                    if (page_current_wear - 1 < 0)
                    {
                        break;
                    }
                    page_current_wear--;

                    for (int i = 0; i < items_wear.Length; i++)
                    {
                        if (items_wear[i] == null)
                        {
                            break;
                        }

                        switch (selectGenderNum)
                        {
                            case 0:
                                {
                                    items_wear[i].SetSlot(list_itemSlot_wear_M[page_current_wear * 9 + i]);
                                }
                                break;
                            case 1:
                                {
                                    items_wear[i].SetSlot(list_itemSlot_wear_F[page_current_wear * 9 + i]);
                                }
                                break;
                        }
                    }
                    uiPages[state].SetPage(page_current_wear, page_max_wear);
                }
                break;
            case 3:
                {
                    if (page_current_acc - 1 < 0)
                    {
                        break;
                    }
                    page_current_acc--;

                    for (int i = 0; i < items_acc.Length; i++)
                    {
                        if (items_acc[i] == null)
                        {
                            break;
                        }

                        int index = page_current_acc * 9 + i;
                        switch (selectGenderNum)
                        {
                            case 0:
                                {
                                    items_acc[i].SetSlot(list_itemSlot_acc_M[index]);
                                }
                                break;
                            case 1:
                                {
                                    items_acc[i].SetSlot(list_itemSlot_acc_F[index]);
                                }
                                break;
                        }
#if PICO_PLATFORM || QIYU_ACCOUNT
                        items_acc[i].isLock = false;
                        icons_lock[i].SetActive(false);
#else
                        int j = 0;
                        for (; j < lockPropInfos.Length; j++)
                        {
                            if (items_acc[i].slotInfo != null && lockPropInfos[j].slot_id == items_acc[i].slotInfo.id)
                            {
                                if (lockPropInfos[j].api_name == "-" || gameDataManager.IsUnlockAchieve(lockPropInfos[j].api_name))
                                {
                                    items_acc[i].isLock = false;
                                    icons_lock[i].SetActive(false);
                                }
                                else
                                {
                                    items_acc[i].isLock = true;
                                    icons_lock[i].SetActive(true);
                                }
                                break;
                            }
                        }

                        if (j == lockPropInfos.Length)
                        {
                            items_acc[i].isLock = false;
                            icons_lock[i].SetActive(false);
                        }
#endif
                    }
                    uiPages[state].SetPage(page_current_acc, page_max_acc);
                }
                break;
        }
    }

    public void CSVToJson()
    {
        // 1.Face.
        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_ItemInfo - Face_Item.csv"))
        {
            string jsonString = JsonUtility.ToJson(new CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemInfo>(1, "Sheet_ItemInfo - Face_Item.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/ItemInfo_Face_Item.json", jsonString);

            Debug.Log("Save JsonFile - Face_Item");
        }
        else
        {
            Debug.LogError("Not find Face_Item_Sheet");
        }

        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_ItemInfo - Face_M_ItemUISlot.csv"))
        {
            string jsonString = JsonUtility.ToJson(new CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>(1, "Sheet_ItemInfo - Face_M_ItemUISlot.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/ItemInfo_Face_M_ItemUISlot.json", jsonString);

            Debug.Log("Save JsonFile - Face_M_ItemUISlot");
        }
        else
        {
            Debug.LogError("Not find Face_M_ItemUISlot_Sheet");
        }

        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_ItemInfo - Face_F_ItemUISlot.csv"))
        {
            string jsonString = JsonUtility.ToJson(new CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>(1, "Sheet_ItemInfo - Face_F_ItemUISlot.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/ItemInfo_Face_F_ItemUISlot.json", jsonString);

            Debug.Log("Save JsonFile - Face_F_ItemUISlot");
        }
        else
        {
            Debug.LogError("Not find Face_F_ItemUISlot_Sheet");
        }

        // 2.Hair.
        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_ItemInfo - Hair_Item.csv"))
        {
            string jsonString = JsonUtility.ToJson(new CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemInfo>(2, "Sheet_ItemInfo - Hair_Item.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/ItemInfo_Hair_Item.json", jsonString);

            Debug.Log("Save JsonFile - Hair_Item");
        }
        else
        {
            Debug.LogError("Not find Hair_Item_Sheet");
        }

        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_ItemInfo - Hair_M_ItemUISlot.csv"))
        {
            string jsonString = JsonUtility.ToJson(new CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>(2, "Sheet_ItemInfo - Hair_M_ItemUISlot.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/ItemInfo_Hair_M_ItemUISlot.json", jsonString);

            Debug.Log("Save JsonFile - Hair_M_ItemUISlot");
        }
        else
        {
            Debug.LogError("Not find Hair_M_ItemUISlot_Sheet");
        }

        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_ItemInfo - Hair_F_ItemUISlot.csv"))
        {
            string jsonString = JsonUtility.ToJson(new CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>(2, "Sheet_ItemInfo - Hair_F_ItemUISlot.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/ItemInfo_Hair_F_ItemUISlot.json", jsonString);

            Debug.Log("Save JsonFile - Hair_F_ItemUISlot");
        }
        else
        {
            Debug.LogError("Not find Hair_F_ItemUISlot_Sheet");
        }

        // 3.Wear.
        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_ItemInfo - Wear_Item.csv"))
        {
            string jsonString = JsonUtility.ToJson(new CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemInfo>(3, "Sheet_ItemInfo - Wear_Item.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/ItemInfo_Wear_Item.json", jsonString);

            Debug.Log("Save JsonFile - Wear_Item");
        }
        else
        {
            Debug.LogError("Not find Wear_Item_Sheet");
        }

        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_ItemInfo - Wear_M_ItemUISlot.csv"))
        {
            string jsonString = JsonUtility.ToJson(new CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>(3, "Sheet_ItemInfo - Wear_M_ItemUISlot.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/ItemInfo_Wear_M_ItemUISlot.json", jsonString);

            Debug.Log("Save JsonFile - Wear_M_ItemUISlot");
        }
        else
        {
            Debug.LogError("Not find Wear_M_ItemUISlot_Sheet");
        }

        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_ItemInfo - Wear_F_ItemUISlot.csv"))
        {
            string jsonString = JsonUtility.ToJson(new CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>(3, "Sheet_ItemInfo - Wear_F_ItemUISlot.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/ItemInfo_Wear_F_ItemUISlot.json", jsonString);

            Debug.Log("Save JsonFile - Wear_F_ItemUISlot");
        }
        else
        {
            Debug.LogError("Not find Wear_F_ItemUISlot_Sheet");
        }

        // 4.Acc.
        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_ItemInfo - Acc_Item.csv"))
        {
            string jsonString = JsonUtility.ToJson(new CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemInfo>(4, "Sheet_ItemInfo - Acc_Item.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/ItemInfo_Acc_Item.json", jsonString);

            Debug.Log("Save JsonFile - Acc_Item");
        }
        else
        {
            Debug.LogError("Not find Acc_Item_Sheet");
        }

        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_ItemInfo - Acc_M_ItemUISlot.csv"))
        {
            string jsonString = JsonUtility.ToJson(new CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>(4, "Sheet_ItemInfo - Acc_M_ItemUISlot.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/ItemInfo_Acc_M_ItemUISlot.json", jsonString);

            Debug.Log("Save JsonFile - Acc_M_ItemUISlot");
        }
        else
        {
            Debug.LogError("Not find Acc_M_ItemUISlot_Sheet");
        }

        if (File.Exists(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Sheets/Sheet_ItemInfo - Acc_F_ItemUISlot.csv"))
        {
            string jsonString = JsonUtility.ToJson(new CustomizeDataInfo.JsonParsing<CustomizeDataInfo.ItemSlotInfo>(4, "Sheet_ItemInfo - Acc_F_ItemUISlot.csv"));

            File.WriteAllText(Application.dataPath + "/03. Individual Resources/Lee Jaeheung/Resources/ItemInfos/ItemInfo_Acc_F_ItemUISlot.json", jsonString);

            Debug.Log("Save JsonFile - Acc_F_ItemUISlot");
        }
        else
        {
            Debug.LogError("Not find Acc_F_ItemUISlot_Sheet");
        }
    }

    public string GetColorInfo(ColorUIPart part)
    {
        switch (part)
        {
            case ColorUIPart.Skin:
                {
                    return modelData.modelDatas[selectGenderNum].Hex_Skin_C;
                }
            case ColorUIPart.Eye:
                {
                    return modelData.modelDatas[selectGenderNum].Hex_Eye_C;
                }
            case ColorUIPart.Eyebrow:
                {
                    return modelData.modelDatas[selectGenderNum].Hex_Eyebrow_C;
                }
            case ColorUIPart.Hair:
                {
                    return modelData.modelDatas[selectGenderNum].Hex_Hair_C;
                }
            case ColorUIPart.Upper:
                {
                    return modelData.modelDatas[selectGenderNum].Hex_Upper_C;
                }
            case ColorUIPart.Lower:
                {
                    return modelData.modelDatas[selectGenderNum].Hex_Lower_C;
                }
            case ColorUIPart.Foot:
                {
                    return modelData.modelDatas[selectGenderNum].Hex_Foot_C;
                }
            case ColorUIPart.Pattern:
                {
                    return modelData.modelDatas[selectGenderNum].Hex_Pattern_C;
                }
            case ColorUIPart.Acc:
                {
                    return modelData.modelDatas[selectGenderNum].Hex_Acc_C;
                }
        }
        return "000000";
    }

    public void SetColorInfo(ColorUIPart part, string hexColor)
    {
        switch (part)
        {
            case ColorUIPart.Skin:
                {
                    modelData.modelDatas[selectGenderNum].Hex_Skin_C = hexColor;
                }
                break;
            case ColorUIPart.Eye:
                {
                    modelData.modelDatas[selectGenderNum].Hex_Eye_C = hexColor;
                }
                break;
            case ColorUIPart.Eyebrow:
                {
                    modelData.modelDatas[selectGenderNum].Hex_Eyebrow_C = hexColor;
                }
                break;
            case ColorUIPart.Hair:
                {
                    modelData.modelDatas[selectGenderNum].Hex_Hair_C = hexColor;
                }
                break;
            case ColorUIPart.Upper:
                {
                    modelData.modelDatas[selectGenderNum].Hex_Upper_C = hexColor;
                }
                break;
            case ColorUIPart.Lower:
                {
                    modelData.modelDatas[selectGenderNum].Hex_Lower_C = hexColor;
                }
                break;
            case ColorUIPart.Foot:
                {
                    modelData.modelDatas[selectGenderNum].Hex_Foot_C = hexColor;
                }
                break;
            case ColorUIPart.Pattern:
                {
                    modelData.modelDatas[selectGenderNum].Hex_Pattern_C = hexColor;
                }
                break;
            case ColorUIPart.Acc:
                {
                    modelData.modelDatas[selectGenderNum].Hex_Acc_C = hexColor;
                }
                break;
        }
    }

    public void InitItemSlot(CustomizeProp itemCtrl)
    {
        Transform parentTr = itemCtrl.transform.parent;
        for (int i = 0; i < parentTr.childCount; i++)
        {
            if (parentTr.GetChild(i) == itemCtrl.transform)
            {
                list_ui_prop.Add(itemCtrl);
                switch (((CustomizeItemCtrl)itemCtrl).item_part)
                {
                    case ItemUIPart.Face:
                        {
                            items_face[i] = (CustomizeItemCtrl)itemCtrl;
                        }
                        return;
                    case ItemUIPart.Hair:
                        {
                            items_hair[i] = (CustomizeItemCtrl)itemCtrl;
                        }
                        return;
                    case ItemUIPart.Wear:
                        {
                            items_wear[i] = (CustomizeItemCtrl)itemCtrl;
                        }
                        return;
                    case ItemUIPart.Acc:
                        {
                            items_acc[i] = (CustomizeItemCtrl)itemCtrl;
                        }
                        return;
                }
            }
        }
    }

    public void InitPageUI(UIPageCtrl pageCtrl)
    {
        uiPages[(int)pageCtrl.item_part] = pageCtrl;
    }

    public void InitColorSlot(ColorPaletteCtrl colorCtrl)
    {
        colorPalette = colorCtrl;
    }

    public CustomizeProp FindNearProp(UIHandInfo handInfo, int index)
    {
        if (!isInit)
        {
            return null;
        }

        CustomizeProp findProp = null;

        // 손 거리에서 가까운.
        for (int i = 0; i < list_ui_prop.Count; i++)
        {
            if (!list_ui_prop[i].gameObject.activeSelf)
            {
                continue;
            }

            if (findProp == null && (handInfo.rayDir.position - list_ui_prop[i].GetCenterPos()).sqrMagnitude - (list_ui_prop[i] == keep_items[index] ? 0.001f : 0f) <= 0.01f)
            {
                findProp = list_ui_prop[i];
            }
            else if (findProp != null && (findProp.GetCenterPos() - handInfo.rayDir.position).sqrMagnitude - (findProp == keep_items[index] ? 0.001f : 0f)
                > (list_ui_prop[i].GetCenterPos() - handInfo.rayDir.position).sqrMagnitude - (list_ui_prop[i] == keep_items[index] ? 0.001f : 0f))
            {
                findProp = list_ui_prop[i];
            }
        }

        if (findProp != null)
        {
            if (keep_items[index] != findProp)
            {
                if (keep_items[index] != null)
                {
                    keep_items[index].SetSize(false);
                }
                keep_items[index] = findProp;
                findProp.SetSize(true);
            }
            return findProp;
        }


        // 레이 각도에서 가까운.
        for (int i = 0; i < list_ui_prop.Count; i++)
        {
            if (!list_ui_prop[i].gameObject.activeSelf)
            {
                continue;
            }

            if (findProp == null && Vector3.Dot(handInfo.rayDir.forward, (list_ui_prop[i].GetCenterPos() - handInfo.rayDir.position).normalized)
                + (list_ui_prop[i] == keep_items[index] ? 0.002f : 0f) >= 0.85f)
            {
                findProp = list_ui_prop[i];
            }
            else if (findProp != null && Vector3.Dot(handInfo.rayDir.forward, (findProp.GetCenterPos() - handInfo.rayDir.position).normalized) + (findProp == keep_items[index] ? 0.002f : 0f)
                < Vector3.Dot(handInfo.rayDir.forward, (list_ui_prop[i].GetCenterPos() - handInfo.rayDir.position).normalized) + (list_ui_prop[i] == keep_items[index] ? 0.002f : 0f))
            {
                findProp = list_ui_prop[i];
            }
        }

        if (findProp != null)
        {
            if (keep_items[index] != null)
            {
                keep_items[index].SetSize(false);
            }
            keep_items[index] = findProp;
            findProp.SetSize(true);
            handInfo.line_transform.position = handInfo.rayDir.position;
            Vector3 dir = findProp.GetCenterPos() - handInfo.rayDir.position;
            handInfo.line_transform.rotation = Quaternion.Lerp(handInfo.rayDir.rotation, Quaternion.LookRotation(dir), 0.7f);
            handInfo.line_transform.localScale = Vector3.forward * dir.magnitude;
        }
        else
        {
            if (keep_items[index] != null)
            {
                keep_items[index].SetSize(false);
                keep_items[index] = null;
            }
            handInfo.line_transform.position = handInfo.rayDir.position;
            handInfo.line_transform.rotation = handInfo.rayDir.rotation;
            handInfo.line_transform.localScale = Vector3.one;
        }

        return findProp;
    }

    public void SetSelectUISlotID(string _id)
    {
        switch (_id[0])
        {
            case '1':
                {
                    for (int i = 0; i < items_face.Length; i++)
                    {
                        if (items_face[i].slotInfo != null)
                        {
                            if (items_face[i].slotInfo.id == _id)
                            {
                                items_face[i].gameObject.SetActive(false);
                            }
                            else if (items_face[i].slotInfo.id == modelData.modelDatas[selectGenderNum].ID_Face_I)
                            {
                                items_face[i].gameObject.SetActive(true);
                            }
                        }
                    }
                    modelData.modelDatas[selectGenderNum].ID_Face_I = _id;
                }
                break;
            case '2':
                {
                    for (int i = 0; i < items_hair.Length; i++)
                    {
                        if (items_hair[i].slotInfo != null)
                        {
                            if (items_hair[i].slotInfo.id == _id)
                            {
                                items_hair[i].gameObject.SetActive(false);
                            }
                            else if (items_hair[i].slotInfo.id == modelData.modelDatas[selectGenderNum].ID_Hair_I)
                            {
                                items_hair[i].gameObject.SetActive(true);
                            }
                        }
                    }
                    modelData.modelDatas[selectGenderNum].ID_Hair_I = _id;
                }
                break;
            case '3':
                {
                    for (int i = 0; i < items_wear.Length; i++)
                    {
                        if (items_wear[i].slotInfo != null)
                        {
                            if (items_wear[i].slotInfo.id == _id)
                            {
                                items_wear[i].gameObject.SetActive(false);
                            }
                            else if (items_wear[i].slotInfo.id == modelData.modelDatas[selectGenderNum].ID_Wear_I)
                            {
                                items_wear[i].gameObject.SetActive(true);
                            }
                        }
                    }
                    modelData.modelDatas[selectGenderNum].ID_Wear_I = _id;
                }
                break;
            case '4':
                {
                    for (int i = 0; i < items_acc.Length; i++)
                    {
                        if (items_acc[i].slotInfo != null)
                        {
                            if (items_acc[i].slotInfo.id == _id)
                            {
                                items_acc[i].gameObject.SetActive(false);
                            }
                            else if (items_acc[i].slotInfo.id == modelData.modelDatas[selectGenderNum].ID_Acc_I)
                            {
                                items_acc[i].gameObject.SetActive(true);
                            }
                        }
                    }
                    modelData.modelDatas[selectGenderNum].ID_Acc_I = _id;
                }
                break;
        }
    }

    public void SetSelectGenderUI(int _genderNum)
    {
        selectGenderNum = _genderNum;
        for (int i = 0; i < buttons_gender.Length; i++)
        {
            if (selectGenderNum == i)
            {
                buttons_gender[i].SetMaterial(materials_customUI[1]);
                buttons_gender[i].SetInteractable(false);
            }
            else
            {
                buttons_gender[i].SetMaterial(materials_customUI[0]);
                buttons_gender[i].SetInteractable(true);
            }
        }
    }

    public void SetColor_Others(ColorUIPart part, string setHexColor)
    {
        switch (part)
        {
            case ColorUIPart.Skin:
            case ColorUIPart.Eye:
            case ColorUIPart.Eyebrow:
                {
                    for (int i = 0; i < items_face.Length; i++)
                    {
                        items_face[i].SetColor(part, setHexColor);
                    }
                }
                break;
            case ColorUIPart.Hair:
                {
                    for (int i = 0; i < items_hair.Length; i++)
                    {
                        items_hair[i].SetColor(part, setHexColor);
                    }
                }
                break;
            case ColorUIPart.Upper:
            case ColorUIPart.Lower:
            case ColorUIPart.Foot:
            case ColorUIPart.Pattern:
                {
                    for (int i = 0; i < items_hair.Length; i++)
                    {
                        items_wear[i].SetColor(part, setHexColor);
                    }
                }
                break;
            case ColorUIPart.Acc:
                {
                    for (int i = 0; i < items_hair.Length; i++)
                    {
                        items_acc[i].SetColor(part, setHexColor);
                    }
                }
                break;
        }
    }

    public void SetRotationCharacter(float rotP)
    {
        characterCtrl.transform.rotation *= Quaternion.Euler(0f, rotP, 0f);
    }

    public void Upload_UserData()
    {
        PlayerPrefs.SetString("gender", (selectGenderNum == 0 ? "m" : "f"));

        modelData.modelDatas[0].Gender = "m";

        PlayerPrefs.SetString("id_item_face_m", modelData.modelDatas[0].ID_Face_I);
        PlayerPrefs.SetString("hex_color_skin_m", modelData.modelDatas[0].Hex_Skin_C);
        PlayerPrefs.SetString("hex_color_eye_m", modelData.modelDatas[0].Hex_Eye_C);
        PlayerPrefs.SetString("hex_color_eyebrow_m", modelData.modelDatas[0].Hex_Eyebrow_C);

        PlayerPrefs.SetString("id_item_hair_m", modelData.modelDatas[0].ID_Hair_I);
        PlayerPrefs.SetString("hex_color_hair_m", modelData.modelDatas[0].Hex_Hair_C);

        PlayerPrefs.SetString("id_item_wear_m", modelData.modelDatas[0].ID_Wear_I);
        PlayerPrefs.SetString("hex_color_upper_m", modelData.modelDatas[0].Hex_Upper_C);
        PlayerPrefs.SetString("hex_color_lower_m", modelData.modelDatas[0].Hex_Lower_C);
        PlayerPrefs.SetString("hex_color_foot_m", modelData.modelDatas[0].Hex_Foot_C);
        PlayerPrefs.SetString("hex_color_pattern_m", modelData.modelDatas[0].Hex_Pattern_C);

        PlayerPrefs.SetString("id_item_acc_m", modelData.modelDatas[0].ID_Acc_I);
        PlayerPrefs.SetString("hex_color_acc_m", modelData.modelDatas[0].Hex_Acc_C);

        modelData.modelDatas[1].Gender = "f";

        PlayerPrefs.SetString("id_item_face_f", modelData.modelDatas[1].ID_Face_I);
        PlayerPrefs.SetString("hex_color_skin_f", modelData.modelDatas[1].Hex_Skin_C);
        PlayerPrefs.SetString("hex_color_eye_f", modelData.modelDatas[1].Hex_Eye_C);
        PlayerPrefs.SetString("hex_color_eyebrow_f", modelData.modelDatas[1].Hex_Eyebrow_C);

        PlayerPrefs.SetString("id_item_hair_f", modelData.modelDatas[1].ID_Hair_I);
        PlayerPrefs.SetString("hex_color_hair_f", modelData.modelDatas[1].Hex_Hair_C);

        PlayerPrefs.SetString("id_item_wear_f", modelData.modelDatas[1].ID_Wear_I);
        PlayerPrefs.SetString("hex_color_upper_f", modelData.modelDatas[1].Hex_Upper_C);
        PlayerPrefs.SetString("hex_color_lower_f", modelData.modelDatas[1].Hex_Lower_C);
        PlayerPrefs.SetString("hex_color_foot_f", modelData.modelDatas[1].Hex_Foot_C);
        PlayerPrefs.SetString("hex_color_pattern_f", modelData.modelDatas[1].Hex_Pattern_C);

        PlayerPrefs.GetString("id_item_acc_f", modelData.modelDatas[1].ID_Acc_I);
        PlayerPrefs.SetString("hex_color_acc_f", modelData.modelDatas[1].Hex_Acc_C);

        gameDataManager.SetMyCustomModelData(modelData.modelDatas[selectGenderNum]);
        StartCoroutine(AppnoriWebRequest.API_SetCustomCharData(modelData.modelDatas));

#if !TEST_ACCOUNT && !(QIYU_ACCOUNT && UNITY_EDITOR) && !(PICO_PLATFORM && UNITY_EDITOR)
        //StartCoroutine(Cor_Upload_UserData());
#endif
    }


    IEnumerator Cor_Upload_UserData()
    {
        WWWForm formData = new WWWForm();

        formData.AddField("UserID", GameDataManager.instance.userInfo_mine.id);
        formData.AddField("Gender", (selectGenderNum == 0 ? "m" : "f"));

        formData.AddField("ID_Face_I_M", modelData.modelDatas[0].ID_Face_I);
        formData.AddField("Hex_Skin_C_M", modelData.modelDatas[0].Hex_Skin_C);
        formData.AddField("Hex_Eye_C_M", modelData.modelDatas[0].Hex_Eye_C);
        formData.AddField("Hex_Eyebrow_C_M", modelData.modelDatas[0].Hex_Eyebrow_C);

        formData.AddField("ID_Hair_I_M", modelData.modelDatas[0].ID_Hair_I);
        formData.AddField("Hex_Hair_C_M", modelData.modelDatas[0].Hex_Hair_C);

        formData.AddField("ID_Wear_I_M", modelData.modelDatas[0].ID_Wear_I);
        formData.AddField("Hex_Upper_C_M", modelData.modelDatas[0].Hex_Upper_C);
        formData.AddField("Hex_Lower_C_M", modelData.modelDatas[0].Hex_Lower_C);
        formData.AddField("Hex_Foot_C_M", modelData.modelDatas[0].Hex_Foot_C);
        formData.AddField("Hex_Pattern_C_M", modelData.modelDatas[0].Hex_Pattern_C);

        formData.AddField("ID_Acc_I_M", modelData.modelDatas[0].ID_Acc_I);
        formData.AddField("Hex_Acc_C_M", modelData.modelDatas[0].Hex_Acc_C);

        formData.AddField("ID_Face_I_F", modelData.modelDatas[1].ID_Face_I);
        formData.AddField("Hex_Skin_C_F", modelData.modelDatas[1].Hex_Skin_C);
        formData.AddField("Hex_Eye_C_F", modelData.modelDatas[1].Hex_Eye_C);
        formData.AddField("Hex_Eyebrow_C_F", modelData.modelDatas[1].Hex_Eyebrow_C);

        formData.AddField("ID_Hair_I_F", modelData.modelDatas[1].ID_Hair_I);
        formData.AddField("Hex_Hair_C_F", modelData.modelDatas[1].Hex_Hair_C);

        formData.AddField("ID_Wear_I_F", modelData.modelDatas[1].ID_Wear_I);
        formData.AddField("Hex_Upper_C_F", modelData.modelDatas[1].Hex_Upper_C);
        formData.AddField("Hex_Lower_C_F", modelData.modelDatas[1].Hex_Lower_C);
        formData.AddField("Hex_Foot_C_F", modelData.modelDatas[1].Hex_Foot_C);
        formData.AddField("Hex_Pattern_C_F", modelData.modelDatas[1].Hex_Pattern_C);

        formData.AddField("ID_Acc_I_F", modelData.modelDatas[1].ID_Acc_I);
        formData.AddField("Hex_Acc_C_F", modelData.modelDatas[1].Hex_Acc_C);

        UnityWebRequest www = UnityWebRequest.Post(GameDataManager.addr + "AIO/SetCustomInfo.php", formData);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
            Debug.Log(www.downloadHandler.text);
        }
        www.Dispose();
    }

    public void Download_UserData()
    {
        
        StartCoroutine(AppnoriWebRequest.API_GetCustomCharData(SetModelData));

        return;
#if TEST_ACCOUNT || (QIYU_ACCOUNT && UNITY_EDITOR) || (PICO_PLATFORM && UNITY_EDITOR)
       /* switch (PlayerPrefs.GetString("gender", "m"))
        {
            case "m":
                {
                    selectGenderNum = 0;
                }
                break;
            case "f":
                {
                    selectGenderNum = 1;
                }
                break;
        }

        modelData = new AllModelData();
        modelData.modelDatas = new CustomModelData[2];
        modelData.modelDatas[0] = new CustomModelData();
        modelData.modelDatas[0].Gender = "m";

        modelData.modelDatas[0].ID_Face_I = PlayerPrefs.GetString("id_item_face_m", "1000");
        modelData.modelDatas[0].Hex_Skin_C = PlayerPrefs.GetString("hex_color_skin_m", "FFAA80");
        modelData.modelDatas[0].Hex_Eye_C = PlayerPrefs.GetString("hex_color_eye_m", "000000");
        modelData.modelDatas[0].Hex_Eyebrow_C = PlayerPrefs.GetString("hex_color_eyebrow_m", "000000");

        modelData.modelDatas[0].ID_Hair_I = PlayerPrefs.GetString("id_item_hair_m", "2000");
        modelData.modelDatas[0].Hex_Hair_C = PlayerPrefs.GetString("hex_color_hair_m", "000000");

        modelData.modelDatas[0].ID_Wear_I = PlayerPrefs.GetString("id_item_wear_m", "3000");
        modelData.modelDatas[0].Hex_Upper_C = PlayerPrefs.GetString("hex_color_upper_m", "808080");
        modelData.modelDatas[0].Hex_Lower_C = PlayerPrefs.GetString("hex_color_lower_m", "808080");
        modelData.modelDatas[0].Hex_Foot_C = PlayerPrefs.GetString("hex_color_foot_m", "808080");
        modelData.modelDatas[0].Hex_Pattern_C = PlayerPrefs.GetString("hex_color_pattern_m", "000000");

        modelData.modelDatas[0].ID_Acc_I = PlayerPrefs.GetString("id_item_acc_m", "4000");
        modelData.modelDatas[0].Hex_Acc_C = PlayerPrefs.GetString("hex_color_acc_m", "808080");

        modelData.modelDatas[1] = new CustomModelData();
        modelData.modelDatas[1].Gender = "f";

        modelData.modelDatas[1].ID_Face_I = PlayerPrefs.GetString("id_item_face_f", "1000");
        modelData.modelDatas[1].Hex_Skin_C = PlayerPrefs.GetString("hex_color_skin_f", "FFAA80");
        modelData.modelDatas[1].Hex_Eye_C = PlayerPrefs.GetString("hex_color_eye_f", "000000");
        modelData.modelDatas[1].Hex_Eyebrow_C = PlayerPrefs.GetString("hex_color_eyebrow_f", "000000");

        modelData.modelDatas[1].ID_Hair_I = PlayerPrefs.GetString("id_item_hair_f", "2000");
        modelData.modelDatas[1].Hex_Hair_C = PlayerPrefs.GetString("hex_color_hair_f", "000000");

        modelData.modelDatas[1].ID_Wear_I = PlayerPrefs.GetString("id_item_wear_f", "3000");
        modelData.modelDatas[1].Hex_Upper_C = PlayerPrefs.GetString("hex_color_upper_f", "808080");
        modelData.modelDatas[1].Hex_Lower_C = PlayerPrefs.GetString("hex_color_lower_f", "808080");
        modelData.modelDatas[1].Hex_Foot_C = PlayerPrefs.GetString("hex_color_foot_f", "808080");
        modelData.modelDatas[1].Hex_Pattern_C = PlayerPrefs.GetString("hex_color_pattern_f", "000000");

        modelData.modelDatas[1].ID_Acc_I = PlayerPrefs.GetString("id_item_acc_f", "4000");
        modelData.modelDatas[1].Hex_Acc_C = PlayerPrefs.GetString("hex_color_acc_f", "808080");*/

        //StartCoroutine(InitData());
#else
       // StartCoroutine(Cor_Download_UserData());
#endif
    }


    public void SetModelData(AppnoriWebRequest.Response_GetCustomCharData response_modelData)
    {
        if (response_modelData == null || response_modelData.rs == null)
        {
            switch (PlayerPrefs.GetString("gender", "m"))
            {
                case "m":
                    {
                        selectGenderNum = 0;
                    }
                    break;
                case "f":
                    {
                        selectGenderNum = 1;
                    }
                    break;
            }

            modelData.modelDatas = new CustomModelData[2];
            modelData.modelDatas[0] = new CustomModelData();
            modelData.modelDatas[0].Gender = "m";

            modelData.modelDatas[0].ID_Face_I = PlayerPrefs.GetString("id_item_face_m", "1000");
            modelData.modelDatas[0].Hex_Skin_C = PlayerPrefs.GetString("hex_color_skin_m", "FFAA80");
            modelData.modelDatas[0].Hex_Eye_C = PlayerPrefs.GetString("hex_color_eye_m", "000000");
            modelData.modelDatas[0].Hex_Eyebrow_C = PlayerPrefs.GetString("hex_color_eyebrow_m", "000000");

            modelData.modelDatas[0].ID_Hair_I = PlayerPrefs.GetString("id_item_hair_m", "2000");
            modelData.modelDatas[0].Hex_Hair_C = PlayerPrefs.GetString("hex_color_hair_m", "000000");

            modelData.modelDatas[0].ID_Wear_I = PlayerPrefs.GetString("id_item_wear_m", "3000");
            modelData.modelDatas[0].Hex_Upper_C = PlayerPrefs.GetString("hex_color_upper_m", "808080");
            modelData.modelDatas[0].Hex_Lower_C = PlayerPrefs.GetString("hex_color_lower_m", "808080");
            modelData.modelDatas[0].Hex_Foot_C = PlayerPrefs.GetString("hex_color_foot_m", "808080");
            modelData.modelDatas[0].Hex_Pattern_C = PlayerPrefs.GetString("hex_color_pattern_m", "000000");

            modelData.modelDatas[0].ID_Acc_I = PlayerPrefs.GetString("id_item_acc_m", "4000");
            modelData.modelDatas[0].Hex_Acc_C = PlayerPrefs.GetString("hex_color_acc_m", "808080");

            modelData.modelDatas[1] = new CustomModelData();
            modelData.modelDatas[1].Gender = "f";

            modelData.modelDatas[1].ID_Face_I = PlayerPrefs.GetString("id_item_face_f", "1000");
            modelData.modelDatas[1].Hex_Skin_C = PlayerPrefs.GetString("hex_color_skin_f", "FFAA80");
            modelData.modelDatas[1].Hex_Eye_C = PlayerPrefs.GetString("hex_color_eye_f", "000000");
            modelData.modelDatas[1].Hex_Eyebrow_C = PlayerPrefs.GetString("hex_color_eyebrow_f", "000000");

            modelData.modelDatas[1].ID_Hair_I = PlayerPrefs.GetString("id_item_hair_f", "2000");
            modelData.modelDatas[1].Hex_Hair_C = PlayerPrefs.GetString("hex_color_hair_f", "000000");

            modelData.modelDatas[1].ID_Wear_I = PlayerPrefs.GetString("id_item_wear_f", "3000");
            modelData.modelDatas[1].Hex_Upper_C = PlayerPrefs.GetString("hex_color_upper_f", "808080");
            modelData.modelDatas[1].Hex_Lower_C = PlayerPrefs.GetString("hex_color_lower_f", "808080");
            modelData.modelDatas[1].Hex_Foot_C = PlayerPrefs.GetString("hex_color_foot_f", "808080");
            modelData.modelDatas[1].Hex_Pattern_C = PlayerPrefs.GetString("hex_color_pattern_f", "000000");

            modelData.modelDatas[1].ID_Acc_I = PlayerPrefs.GetString("id_item_acc_f", "4000");
            modelData.modelDatas[1].Hex_Acc_C = PlayerPrefs.GetString("hex_color_acc_f", "808080");
        }
        else
        {
            modelData.modelDatas = new CustomModelData[2];
            modelData.modelDatas[0] = new CustomModelData();
            modelData.modelDatas[0].Gender = "m";

            modelData.modelDatas[0].ID_Face_I = response_modelData.rs.idFaceIM;
            modelData.modelDatas[0].Hex_Skin_C = response_modelData.rs.hexSkinCM;
            modelData.modelDatas[0].Hex_Eye_C = response_modelData.rs.hexEyeCM;
            modelData.modelDatas[0].Hex_Eyebrow_C = response_modelData.rs.hexEyebrowCM;

            modelData.modelDatas[0].ID_Hair_I = response_modelData.rs.idHairIM;
            modelData.modelDatas[0].Hex_Hair_C = response_modelData.rs.hexHairCM;

            modelData.modelDatas[0].ID_Wear_I = response_modelData.rs.idWearIM;
            modelData.modelDatas[0].Hex_Upper_C = response_modelData.rs.hexUpperCM;
            modelData.modelDatas[0].Hex_Lower_C = response_modelData.rs.hexLowerCM;
            modelData.modelDatas[0].Hex_Foot_C = response_modelData.rs.hexFootCM;
            modelData.modelDatas[0].Hex_Pattern_C = response_modelData.rs.hexPatternCM;

            modelData.modelDatas[0].ID_Acc_I = response_modelData.rs.idAccIM;
            modelData.modelDatas[0].Hex_Acc_C = response_modelData.rs.hexAccCM;

            modelData.modelDatas[1] = new CustomModelData();
            modelData.modelDatas[1].Gender = "f";

            modelData.modelDatas[1].ID_Face_I = response_modelData.rs.idFaceIF;
            modelData.modelDatas[1].Hex_Skin_C = response_modelData.rs.hexSkinCF;
            modelData.modelDatas[1].Hex_Eye_C = response_modelData.rs.hexEyeCF;
            modelData.modelDatas[1].Hex_Eyebrow_C = response_modelData.rs.hexEyebrowCF;

            modelData.modelDatas[1].ID_Hair_I = response_modelData.rs.idHairIF;
            modelData.modelDatas[1].Hex_Hair_C = response_modelData.rs.hexHairCF;

            modelData.modelDatas[1].ID_Wear_I = response_modelData.rs.idWearIF;
            modelData.modelDatas[1].Hex_Upper_C = response_modelData.rs.hexUpperCF;
            modelData.modelDatas[1].Hex_Lower_C = response_modelData.rs.hexLowerCF;
            modelData.modelDatas[1].Hex_Foot_C = response_modelData.rs.hexFootCF;
            modelData.modelDatas[1].Hex_Pattern_C = response_modelData.rs.hexPatternCF;

            modelData.modelDatas[1].ID_Acc_I = response_modelData.rs.idAccIF;
            modelData.modelDatas[1].Hex_Acc_C = response_modelData.rs.hexAccCF;

            switch (response_modelData.rs.gender)
            {
                case "M":
                case "m":
                    {
                        selectGenderNum = 0;
                    }
                    break;
                case "F":
                case "f":
                    {
                        selectGenderNum = 1;
                    }
                    break;
            }
        }
    }


}
