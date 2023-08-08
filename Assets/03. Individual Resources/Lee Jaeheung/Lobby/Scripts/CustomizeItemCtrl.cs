using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemUIPart
{
    Face, Hair, Wear, Acc
}

public partial class CustomizeItemCtrl : CustomizeProp
{
    public CustomizeDataInfo.ItemSlotInfo slotInfo;

    public ItemUIPart item_part;

    private Vector3 pos_orizin;
    private Quaternion rot_orizin;

    private PropState propState;

    private Transform handTr;
    private int handNum;

    public bool isLock = false;

    public class CachingData
    {
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
    }

    private CachingData[] cachingDatas;

    private void Start()
    {
        pos_orizin = transform.position;
        rot_orizin = transform.rotation;

        switch (item_part)
        {
            case ItemUIPart.Face:
                {
                    cachingDatas = new CachingData[1];

                    Transform findTr = transform.Find("ITEM_HEAD");
                    cachingDatas[0] = new CachingData();
                    cachingDatas[0].meshFilter = findTr.GetComponent<MeshFilter>();
                    cachingDatas[0].meshRenderer = findTr.GetComponent<MeshRenderer>();
                }
                break;
            case ItemUIPart.Hair:
                {
                    cachingDatas = new CachingData[4];

                    Transform findTr = null;
                    for (int i = 0; i < cachingDatas.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    findTr = transform.Find("ITEM_HEAD");
                                }
                                break;
                            case 1:
                                {
                                    findTr = transform.Find("ITEM_HAIR_F");
                                }
                                break;
                            case 2:
                                {
                                    findTr = transform.Find("ITEM_HAIR_B");
                                }
                                break;
                        }
                        cachingDatas[i] = new CachingData();
                        cachingDatas[i].meshFilter = findTr.GetComponent<MeshFilter>();
                        cachingDatas[i].meshRenderer = findTr.GetComponent<MeshRenderer>();
                    }
                }
                break;
            case ItemUIPart.Wear:
                {
                    cachingDatas = new CachingData[3];

                    Transform findTr = null;
                    for (int i = 0; i < cachingDatas.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    findTr = transform.Find("ITEM_UPPER");
                                }
                                break;
                            case 1:
                                {
                                    findTr = transform.Find("ITEM_LOWER");
                                }
                                break;
                            case 2:
                                {
                                    findTr = transform.Find("ITEM_LOWER_SUB");
                                }
                                break;
                        }
                        cachingDatas[i] = new CachingData();
                        cachingDatas[i].meshFilter = findTr.GetComponent<MeshFilter>();
                        cachingDatas[i].meshRenderer = findTr.GetComponent<MeshRenderer>();
                    }
                }
                break;
            case ItemUIPart.Acc:
                {
                    cachingDatas = new CachingData[1];

                    Transform findTr = null;
                    for (int i = 0; i < cachingDatas.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    findTr = transform.Find("ITEM_HEAD_ACC");
                                }
                                break;
                        }
                        cachingDatas[i] = new CachingData();
                        cachingDatas[i].meshFilter = findTr.GetComponent<MeshFilter>();
                        cachingDatas[i].meshRenderer = findTr.GetComponent<MeshRenderer>();
                    }
                }
                break;
        }

        CustomizeManager.GetInstance.InitItemSlot(this);
        
    }

    public override Vector3 GetCenterPos()
    {
        return transform.position;
    }

    public void SetPropState(PropState _propState, Transform _handTr = null, int _handNum = 0)
    {
        propState = _propState;
        if (propState == PropState.Grab && _handTr != null)
        {
            LobbySoundManager.GetInstance.Play((int)Random.Range(0f, 2.9999f));
            handTr = _handTr;
            handNum = _handNum;
        }
    }

    public void SetIdlePos()
    {
        transform.position = pos_orizin;
        transform.rotation = rot_orizin;
    }

    public override void SetSize(bool isSelect)
    {
        if (orderYn == "N")
            return;
        if (isSelect)
        {
            transform.localScale = Vector3.one * 1.2f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }


    void LateUpdate()
    {
        if (orderYn == "N")
            return;

        switch (propState)
        {
            case PropState.Idle:
                {
                    transform.position = Vector3.MoveTowards(transform.position, pos_orizin, Time.deltaTime * 7f);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rot_orizin, Time.deltaTime * 700f);
                }
                break;
            case PropState.Grab:
                {
                    Vector3 hand_pos = handTr.position + handTr.forward * -0.05f + (handNum == 0 ? -handTr.right : handTr.right) * (item_part == ItemUIPart.Face ? -0.11f : -0.08f);
                    transform.position = Vector3.MoveTowards(transform.position, hand_pos, Time.deltaTime * 5f);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, handTr.rotation * Quaternion.Euler(0f, 90f, -45f), Time.deltaTime * 500f);
                }
                break;
            case PropState.Set:
                {
                    Transform setTr;
                    if (item_part == ItemUIPart.Wear)
                    {
                        setTr = CustomizeManager.GetInstance.customSetPos_body;
                    }
                    else
                    {
                        setTr = CustomizeManager.GetInstance.customSetPos_head;
                    }

                    transform.position = Vector3.MoveTowards(transform.position, setTr.position, Time.deltaTime * 3f);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, setTr.rotation, Time.deltaTime * 500f);

                    if ((transform.position - setTr.position).sqrMagnitude <= 0.0001f)
                    {
                        CustomizeManager.GetInstance.characterCtrl.SetItem(slotInfo);
                        SetIdlePos();
                        propState = PropState.Idle;
                    }
                }
                break;
        }
    }


    public void SetSlot(CustomizeDataInfo.ItemSlotInfo _slotInfo)
    {
        slotInfo = _slotInfo;
        if (slotInfo == null)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
            SetItemData(_slotInfo.id);
        }
       
        switch (item_part)
        {
            case ItemUIPart.Face:
                {
                    int genderNum = CustomizeManager.GetInstance.selectGenderNum;
                    cachingDatas[0].meshFilter.sharedMesh = CustomizeManager.GetInstance.meshes_head[genderNum];

                    Material mat_temp = cachingDatas[0].meshRenderer.material;
                    mat_temp.SetTexture("_SubTex_Eye", null);
                    mat_temp.SetTexture("_SubTex_Pupil", null);
                    mat_temp.SetTexture("_SubTex_Eyebrow", null);
                    mat_temp.SetTexture("_SubTex_Mouth", null);
                    mat_temp.SetTexture("_BaseTex", null);

                    for (int i = 0; i < _slotInfo.list_partId.Count; i++)
                    {
                        switch (_slotInfo.list_partId[i][1])
                        {
                            case '0':
                                {
                                    mat_temp.SetTexture("_SubTex_Eye", Resources.Load<Texture>("Customize/Faces/" + CustomizeDataInfo.GetInstance.GetItemInfo(slotInfo.list_partId[i]).matPath));
                                }
                                break;
                            case '1':
                                {
                                    mat_temp.SetTexture("_SubTex_Pupil", Resources.Load<Texture>("Customize/Faces/" + CustomizeDataInfo.GetInstance.GetItemInfo(slotInfo.list_partId[i]).matPath));
                                }
                                break;
                            case '2':
                                {
                                    mat_temp.SetTexture("_SubTex_Eyebrow", Resources.Load<Texture>("Customize/Faces/" + CustomizeDataInfo.GetInstance.GetItemInfo(slotInfo.list_partId[i]).matPath));
                                }
                                break;
                            case '3':
                                {
                                    mat_temp.SetTexture("_SubTex_Mouth", Resources.Load<Texture>("Customize/Faces/" + CustomizeDataInfo.GetInstance.GetItemInfo(slotInfo.list_partId[i]).matPath));
                                }
                                break;
                            case '4':
                                {
                                    mat_temp.SetTexture("_BaseTex", Resources.Load<Texture>("Customize/Faces/" + CustomizeDataInfo.GetInstance.GetItemInfo(slotInfo.list_partId[i]).matPath));
                                }
                                break;
                        }
                    }

                    Color temp_color;
                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Skin), out temp_color))
                    {
                        mat_temp.SetColor("_BaseColor", temp_color);
                    }
                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Eye), out temp_color))
                    {
                        mat_temp.SetColor("_EyeColor", temp_color);
                    }
                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Eyebrow), out temp_color))
                    {
                        mat_temp.SetColor("_EyebrowColor", temp_color);
                    }


                    gameObject.SetActive(CustomizeManager.GetInstance.modelData.modelDatas[genderNum].ID_Face_I != slotInfo.id);
                }
                break;
            case ItemUIPart.Hair:
                {
                    int genderNum = CustomizeManager.GetInstance.selectGenderNum;

                    cachingDatas[0].meshFilter.sharedMesh = CustomizeManager.GetInstance.meshes_head[genderNum];

                    cachingDatas[1].meshFilter.gameObject.SetActive(false);
                    cachingDatas[2].meshFilter.gameObject.SetActive(false);
                    cachingDatas[3].meshFilter.gameObject.SetActive(false);

                    for (int i = 0; i < _slotInfo.list_partId.Count; i++)
                    {
                        switch (_slotInfo.list_partId[i][1])
                        {
                            case '0':
                                {
                                    CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);
                                    cachingDatas[1].meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Hairs/" + itemInfo.meshPath);
                                    cachingDatas[1].meshRenderer.material = Resources.Load<Material>("Customize/Hairs/" + itemInfo.matPath);

                                    Color temp_color;
                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Hair), out temp_color))
                                    {
                                        cachingDatas[1].meshRenderer.material.SetColor("_BaseColor", temp_color);
                                    }

                                    cachingDatas[1].meshFilter.gameObject.SetActive(true);
                                }
                                break;
                            case '1':
                                {
                                    CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);
                                    cachingDatas[2].meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Hairs/" + itemInfo.meshPath);
                                    cachingDatas[2].meshRenderer.material = Resources.Load<Material>("Customize/Hairs/" + itemInfo.matPath);

                                    Color temp_color;
                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Hair), out temp_color))
                                    {
                                        cachingDatas[2].meshRenderer.material.SetColor("_BaseColor", temp_color);
                                    }

                                    cachingDatas[2].meshFilter.gameObject.SetActive(true);
                                }
                                break;
                        }
                    }

                    gameObject.SetActive(CustomizeManager.GetInstance.modelData.modelDatas[genderNum].ID_Hair_I != slotInfo.id);
                }
                break;
            case ItemUIPart.Wear:
                {
                    int genderNum = CustomizeManager.GetInstance.selectGenderNum;
                    cachingDatas[0].meshFilter.gameObject.SetActive(false);
                    cachingDatas[1].meshFilter.gameObject.SetActive(false);
                    cachingDatas[2].meshFilter.gameObject.SetActive(false);

                    for (int i = 0; i < _slotInfo.list_partId.Count; i++)
                    {
                        switch (_slotInfo.list_partId[i][1])
                        {
                            case '0':
                            case '6':
                                {
                                    CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);

                                    cachingDatas[0].meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                    cachingDatas[0].meshRenderer.material = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                    Color temp_color;
                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Upper), out temp_color))
                                    {
                                        cachingDatas[0].meshRenderer.material.SetColor("_BaseColor", temp_color);
                                    }

                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Pattern), out temp_color))
                                    {
                                        cachingDatas[0].meshRenderer.material.SetColor("_PatternColor", temp_color);
                                    }

                                    cachingDatas[0].meshRenderer.material.SetColor("_Color_R", Color.white);
                                    cachingDatas[0].meshRenderer.material.SetColor("_Color_G", Color.white);
                                    cachingDatas[0].meshRenderer.material.SetColor("_Color_B", Color.white);

                                    cachingDatas[0].meshFilter.gameObject.SetActive(true);
                                }
                                break;
                            case '2':
                                {
                                    CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);
                                    cachingDatas[1].meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                    cachingDatas[1].meshRenderer.material = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                    Color temp_color;
                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Lower), out temp_color))
                                    {
                                        cachingDatas[1].meshRenderer.material.SetColor("_BaseColor", temp_color);
                                    }

                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Pattern), out temp_color))
                                    {
                                        cachingDatas[1].meshRenderer.material.SetColor("_PatternColor", temp_color);
                                    }

                                    cachingDatas[1].meshRenderer.material.SetColor("_Color_R", Color.white);
                                    cachingDatas[1].meshRenderer.material.SetColor("_Color_G", Color.white);
                                    cachingDatas[1].meshRenderer.material.SetColor("_Color_B", Color.white);

                                    cachingDatas[1].meshFilter.gameObject.SetActive(true);
                                }
                                break;
                            case '3':
                                {
                                    CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);
                                    cachingDatas[2].meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                    cachingDatas[2].meshRenderer.material = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                    Color temp_color;
                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Lower), out temp_color))
                                    {
                                        cachingDatas[2].meshRenderer.material.SetColor("_BaseColor", temp_color);
                                    }

                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Pattern), out temp_color))
                                    {
                                        cachingDatas[2].meshRenderer.material.SetColor("_PatternColor", temp_color);
                                    }

                                    cachingDatas[2].meshRenderer.material.SetColor("_Color_R", Color.white);
                                    cachingDatas[2].meshRenderer.material.SetColor("_Color_G", Color.white);
                                    cachingDatas[2].meshRenderer.material.SetColor("_Color_B", Color.white);

                                    cachingDatas[2].meshFilter.gameObject.SetActive(true);
                                }
                                break;
                        }
                    }

                    gameObject.SetActive(CustomizeManager.GetInstance.modelData.modelDatas[genderNum].ID_Wear_I != slotInfo.id);
                }
                break;
            case ItemUIPart.Acc:
                {
                    int genderNum = CustomizeManager.GetInstance.selectGenderNum;

                    if (_slotInfo.list_partId[0] == "40000")
                    {
                        cachingDatas[0].meshFilter.sharedMesh = CustomizeManager.GetInstance.meshes_head[genderNum];
                        cachingDatas[0].meshRenderer.material = Resources.Load<Material>("Customize/Bodies/Mat_Custom_Face");
                    }
                    else
                    {
                        for (int i = 0; i < _slotInfo.list_partId.Count; i++)
                        {
                            CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);

                            cachingDatas[0].meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Accs/" + itemInfo.meshPath);
                            cachingDatas[0].meshRenderer.material = Resources.Load<Material>("Customize/Accs/" + itemInfo.matPath);
                            Color temp_color;
                            if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Acc), out temp_color))
                            {
                                cachingDatas[0].meshRenderer.material.SetColor("_BaseColor", temp_color);
                            }
                        }
                    }
                    gameObject.SetActive(CustomizeManager.GetInstance.modelData.modelDatas[genderNum].ID_Acc_I != slotInfo.id);
                }
                break;
        }

    }

    public void SetColor(ColorUIPart part, string setHexColor)
    {
        Color temp_color;
        switch (part)
        {
            case ColorUIPart.Skin:
                {
                    if (ColorUtility.TryParseHtmlString("#" + setHexColor, out temp_color))
                    {
                        cachingDatas[0].meshRenderer.material.SetColor("_BaseColor", temp_color);
                    }
                }
                break;
            case ColorUIPart.Eye:
                {
                    if (ColorUtility.TryParseHtmlString("#" + setHexColor, out temp_color))
                    {
                        cachingDatas[0].meshRenderer.material.SetColor("_EyeColor", temp_color);
                    }
                }
                break;
            case ColorUIPart.Eyebrow:
                {
                    if (ColorUtility.TryParseHtmlString("#" + setHexColor, out temp_color))
                    {
                        cachingDatas[0].meshRenderer.material.SetColor("_EyebrowColor", temp_color);
                    }
                }
                break;
            case ColorUIPart.Hair:
                {
                    if (ColorUtility.TryParseHtmlString("#" + setHexColor, out temp_color))
                    {
                        cachingDatas[1].meshRenderer.material.SetColor("_BaseColor", temp_color);
                        cachingDatas[2].meshRenderer.material.SetColor("_BaseColor", temp_color);
                    }
                }
                break;
            case ColorUIPart.Upper:
                {
                    if (ColorUtility.TryParseHtmlString("#" + setHexColor, out temp_color))
                    {
                        cachingDatas[0].meshRenderer.material.SetColor("_BaseColor", temp_color);
                    }
                }
                break;
            case ColorUIPart.Lower:
                {
                    if (ColorUtility.TryParseHtmlString("#" + setHexColor, out temp_color))
                    {
                        cachingDatas[1].meshRenderer.material.SetColor("_BaseColor", temp_color);
                        cachingDatas[2].meshRenderer.material.SetColor("_BaseColor", temp_color);
                    }
                }
                break;
            case ColorUIPart.Pattern:
                {
                    if (ColorUtility.TryParseHtmlString("#" + setHexColor, out temp_color))
                    {
                        cachingDatas[0].meshRenderer.material.SetColor("_PatternColor", temp_color);
                        cachingDatas[1].meshRenderer.material.SetColor("_PatternColor", temp_color);
                        cachingDatas[2].meshRenderer.material.SetColor("_PatternColor", temp_color);
                    }
                }
                break;
            case ColorUIPart.Acc:
                {
                    if (slotInfo == null || slotInfo.list_partId.Count == 0 || slotInfo.list_partId[0] == "40000")
                    {
                        return;
                    }

                    if (ColorUtility.TryParseHtmlString("#" + setHexColor, out temp_color))
                    {
                        cachingDatas[0].meshRenderer.material.SetColor("_BaseColor", temp_color);
                    }
                }
                break;
        }
    }
}
