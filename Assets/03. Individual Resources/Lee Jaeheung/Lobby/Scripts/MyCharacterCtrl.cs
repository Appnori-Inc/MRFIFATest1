using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterCtrl : MonoBehaviour
{
    public Animator anim;

    private Material mat_body;

    private MeshFilter meshFilter_head;
    private MeshRenderer renderer_head;
    private SkinnedMeshRenderer renderer_body_cut;
    private SkinnedMeshRenderer renderer_body_cut_h;
    private SkinnedMeshRenderer renderer_body_origin;
    private SkinnedMeshRenderer renderer_body_origin_h;

    private MeshFilter meshFilter_hair_f;
    private MeshFilter meshFilter_hair_b;
    private MeshRenderer renderer_hair_f;
    private MeshRenderer renderer_hair_b;

    private Mesh mesh_hair_origin;
    private Material mat_hair_origin;
    private Mesh mesh_hair_m;
    private Material mat_hair_m;
    private Mesh mesh_hair_f;
    private Material mat_hair_f;

    private SkinnedMeshRenderer renderer_wear_u;
    private SkinnedMeshRenderer renderer_wear_u_c;
    private SkinnedMeshRenderer renderer_wear_l;
    private SkinnedMeshRenderer renderer_wear_l_s;
    private SkinnedMeshRenderer renderer_wear_h;
    private SkinnedMeshRenderer renderer_wear_f;

    private MeshFilter meshFilter_acc;
    private MeshRenderer renderer_acc;

    public bool isInit = false;

    private Transform[] boneTrs;

    private bool isOptimizeGameObjects = false;

    public Avatar[] avatars;

    // Start is called before the first frame update
    void Start()
    {
        InitData();
    }

    void InitData()
    {
        if (isInit)
        {
            return;
        }

        boneTrs = new Transform[59];

        anim = transform.GetComponent<Animator>();

        isOptimizeGameObjects = (transform.Find("BODY_ORG_HIDDEN").GetComponent<SkinnedMeshRenderer>().rootBone == null);

        if (isOptimizeGameObjects)
        {
            meshFilter_head = transform.Find("ITEM_HEAD").GetComponent<MeshFilter>();
            renderer_head = transform.Find("ITEM_HEAD").GetComponent<MeshRenderer>();

            renderer_body_cut = transform.Find("BODY_CUT").GetComponent<SkinnedMeshRenderer>();
            renderer_body_cut_h = transform.Find("BODY_CUT_HIDDEN").GetComponent<SkinnedMeshRenderer>();
            renderer_body_origin = transform.Find("BODY_ORG").GetComponent<SkinnedMeshRenderer>();
            renderer_body_origin_h = transform.Find("BODY_ORG_HIDDEN").GetComponent<SkinnedMeshRenderer>();
            mat_body = renderer_body_cut.sharedMaterial;

            meshFilter_hair_f = transform.Find("ITEM_HAIR_F").GetComponent<MeshFilter>();
            meshFilter_hair_b = transform.Find("ITEM_HAIR_B").GetComponent<MeshFilter>();

            renderer_hair_f = transform.Find("ITEM_HAIR_F").GetComponent<MeshRenderer>();
            renderer_hair_b = transform.Find("ITEM_HAIR_B").GetComponent<MeshRenderer>();

            renderer_wear_u = transform.Find("ITEM_UPPER").GetComponent<SkinnedMeshRenderer>();
            renderer_wear_u_c = transform.Find("ITEM_UPPER_CUT").GetComponent<SkinnedMeshRenderer>();
            renderer_wear_l = transform.Find("ITEM_LOWER").GetComponent<SkinnedMeshRenderer>();
            renderer_wear_l_s = transform.Find("ITEM_LOWER_SUB").GetComponent<SkinnedMeshRenderer>();

            renderer_wear_f = transform.Find("ITEM_FOOT").GetComponent<SkinnedMeshRenderer>();

            meshFilter_acc = transform.Find("ITEM_HEAD_ACC").GetComponent<MeshFilter>();
            renderer_acc = transform.Find("ITEM_HEAD_ACC").GetComponent<MeshRenderer>();
        }
        else
        {
            Transform[] boneTrs_temp = transform.Find("Bip001").GetComponentsInChildren<Transform>();

            for (int i = 0; i < boneTrs_temp.Length; i++)
            {
                int index = WearInfoCreator.BoneNameToIndex(boneTrs_temp[i].name);
                if (index == -1)
                {
                    continue;
                }

                boneTrs[index] = boneTrs_temp[i];
            }

            Transform headTr = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 Head");

            meshFilter_head = headTr.Find("ITEM_HEAD").GetComponent<MeshFilter>();
            renderer_head = headTr.Find("ITEM_HEAD").GetComponent<MeshRenderer>();

            renderer_body_cut = transform.Find("BODY_CUT").GetComponent<SkinnedMeshRenderer>();
            renderer_body_cut_h = transform.Find("BODY_CUT_HIDDEN").GetComponent<SkinnedMeshRenderer>();
            renderer_body_origin = transform.Find("BODY_ORG").GetComponent<SkinnedMeshRenderer>();
            renderer_body_origin_h = transform.Find("BODY_ORG_HIDDEN").GetComponent<SkinnedMeshRenderer>();

            meshFilter_hair_f = headTr.Find("ITEM_HAIR_F").GetComponent<MeshFilter>();
            meshFilter_hair_b = headTr.Find("ITEM_HAIR_B").GetComponent<MeshFilter>();

            renderer_hair_f = headTr.Find("ITEM_HAIR_F").GetComponent<MeshRenderer>();
            renderer_hair_b = headTr.Find("ITEM_HAIR_B").GetComponent<MeshRenderer>();

            renderer_wear_u = transform.Find("ITEM_UPPER").GetComponent<SkinnedMeshRenderer>();
            renderer_wear_u_c = transform.Find("ITEM_UPPER_CUT").GetComponent<SkinnedMeshRenderer>();
            renderer_wear_l = transform.Find("ITEM_LOWER").GetComponent<SkinnedMeshRenderer>();
            renderer_wear_l_s = transform.Find("ITEM_LOWER_SUB").GetComponent<SkinnedMeshRenderer>();

            renderer_wear_f = transform.Find("ITEM_FOOT").GetComponent<SkinnedMeshRenderer>();

            meshFilter_acc = headTr.Find("ITEM_HEAD_ACC").GetComponent<MeshFilter>();
            renderer_acc = headTr.Find("ITEM_HEAD_ACC").GetComponent<MeshRenderer>();
        }
        meshFilter_acc.transform.parent = meshFilter_head.transform;


        CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo("21001");
        mesh_hair_m = Resources.Load<Mesh>("Customize/Hairs/" + itemInfo.meshPath);
        mat_hair_m = Resources.Load<Material>("Customize/Hairs/" + itemInfo.matPath);

        itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo("21018");
        mesh_hair_f = Resources.Load<Mesh>("Customize/Hairs/" + itemInfo.meshPath);
        mat_hair_f = Resources.Load<Material>("Customize/Hairs/" + itemInfo.matPath);

        isInit = true;
    }

    public void SetGender(int genderNum)
    {
        InitData();
        CustomizeManager.GetInstance.SetSelectGenderUI(genderNum);
        switch (genderNum)
        {
            case 0:
                {
                    anim.avatar = avatars[0];
                    meshFilter_head.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_HEAD");
                    mat_body = Resources.Load<Material>("Customize/Bodies/Mat_M_SKIN");
                    renderer_body_cut.sharedMaterial = mat_body;
                    renderer_body_cut_h.sharedMaterial = mat_body;
                    renderer_body_origin.sharedMaterial = mat_body;
                    renderer_body_origin_h.sharedMaterial = mat_body;

                    renderer_body_cut.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_CUT");

                    TextAsset jsonText = Resources.Load("ItemInfos/BodyInfo_M_Cut") as TextAsset;
                    CustomizeDataInfo.WearInfo bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                    renderer_body_cut.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                    if (!isOptimizeGameObjects)
                    {
                        renderer_body_cut.rootBone = boneTrs[bodyInfo.rootBone];

                        Transform[] boneTrs_temp = new Transform[bodyInfo.bones.Length];
                        for (int j = 0; j < bodyInfo.bones.Length; j++)
                        {
                            boneTrs_temp[j] = boneTrs[bodyInfo.bones[j]];
                        }
                        renderer_body_cut.bones = boneTrs_temp;
                    }

                    renderer_body_cut_h.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_CUT_HIDDEN");

                    jsonText = Resources.Load("ItemInfos/BodyInfo_M_Cut_H") as TextAsset;
                    bodyInfo = null;
                    bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                    renderer_body_cut_h.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                    if (!isOptimizeGameObjects)
                    {
                        renderer_body_cut_h.rootBone = boneTrs[bodyInfo.rootBone];

                        Transform[] boneTrs_temp = new Transform[bodyInfo.bones.Length];
                        for (int j = 0; j < bodyInfo.bones.Length; j++)
                        {
                            boneTrs_temp[j] = boneTrs[bodyInfo.bones[j]];
                        }
                        renderer_body_cut_h.bones = boneTrs_temp;
                    }


                    renderer_body_origin.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_ORG");

                    jsonText = Resources.Load("ItemInfos/BodyInfo_M_Origin") as TextAsset;
                    bodyInfo = null;
                    bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                    renderer_body_origin.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                    if (!isOptimizeGameObjects)
                    {
                        renderer_body_origin.rootBone = boneTrs[bodyInfo.rootBone];

                        Transform[] boneTrs_temp = new Transform[bodyInfo.bones.Length];
                        for (int j = 0; j < bodyInfo.bones.Length; j++)
                        {
                            boneTrs_temp[j] = boneTrs[bodyInfo.bones[j]];
                        }
                        renderer_body_origin.bones = boneTrs_temp;
                    }

                    renderer_body_origin_h.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_ORGN_HIDDEN");

                    jsonText = Resources.Load("ItemInfos/BodyInfo_M_OriginN_H") as TextAsset;
                    bodyInfo = null;
                    bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                    renderer_body_origin_h.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);


                    if (!isOptimizeGameObjects)
                    {
                        renderer_body_origin_h.rootBone = boneTrs[bodyInfo.rootBone];

                        Transform[] boneTrs_temp = new Transform[bodyInfo.bones.Length];
                        for (int j = 0; j < bodyInfo.bones.Length; j++)
                        {
                            boneTrs_temp[j] = boneTrs[bodyInfo.bones[j]];
                        }
                        renderer_body_origin_h.bones = boneTrs_temp;
                    }

                    SetItem(CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[0].ID_Face_I));
                    SetItem(CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[0].ID_Hair_I));
                    SetItem(CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[0].ID_Wear_I));
                    SetItem(CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[0].ID_Acc_I));

                    SetColor(ColorUIPart.Skin, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Skin));
                    SetColor(ColorUIPart.Eye, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Eye));
                    SetColor(ColorUIPart.Eyebrow, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Eyebrow));
                    SetColor(ColorUIPart.Hair, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Hair));
                    SetColor(ColorUIPart.Upper, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Upper));
                    SetColor(ColorUIPart.Lower, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Lower));
                    SetColor(ColorUIPart.Foot, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Foot));
                    SetColor(ColorUIPart.Pattern, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Pattern));
                    SetColor(ColorUIPart.Acc, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Acc));

                    CustomizeManager.GetInstance.SetUI();
                }
                break;
            case 1:
                {
                    anim.avatar = avatars[1];
                    meshFilter_head.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_HEAD");
                    mat_body = Resources.Load<Material>("Customize/Bodies/Mat_F_SKIN");
                    renderer_body_cut.sharedMaterial = mat_body;
                    renderer_body_cut_h.sharedMaterial = mat_body;
                    renderer_body_origin.sharedMaterial = mat_body;
                    renderer_body_origin_h.sharedMaterial = mat_body;

                    renderer_body_cut.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_CUT");

                    TextAsset jsonText = Resources.Load("ItemInfos/BodyInfo_F_Cut") as TextAsset;
                    CustomizeDataInfo.WearInfo bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                    renderer_body_cut.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                    if (!isOptimizeGameObjects)
                    {
                        renderer_body_cut.rootBone = boneTrs[bodyInfo.rootBone];

                        Transform[] boneTrs_temp = new Transform[bodyInfo.bones.Length];
                        for (int j = 0; j < bodyInfo.bones.Length; j++)
                        {
                            boneTrs_temp[j] = boneTrs[bodyInfo.bones[j]];
                        }
                        renderer_body_cut.bones = boneTrs_temp;
                    }

                    renderer_body_cut_h.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_CUT_HIDDEN");

                    jsonText = Resources.Load("ItemInfos/BodyInfo_F_Cut_H") as TextAsset;
                    bodyInfo = null;
                    bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                    renderer_body_cut.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                    if (!isOptimizeGameObjects)
                    {
                        renderer_body_cut.rootBone = boneTrs[bodyInfo.rootBone];

                        Transform[] boneTrs_temp = new Transform[bodyInfo.bones.Length];
                        for (int j = 0; j < bodyInfo.bones.Length; j++)
                        {
                            boneTrs_temp[j] = boneTrs[bodyInfo.bones[j]];
                        }
                        renderer_body_cut.bones = boneTrs_temp;
                    }


                    renderer_body_origin.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_ORG");

                    jsonText = Resources.Load("ItemInfos/BodyInfo_F_Origin") as TextAsset;
                    bodyInfo = null;
                    bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                    renderer_body_cut.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                    if (!isOptimizeGameObjects)
                    {
                        renderer_body_cut.rootBone = boneTrs[bodyInfo.rootBone];

                        Transform[] boneTrs_temp = new Transform[bodyInfo.bones.Length];
                        for (int j = 0; j < bodyInfo.bones.Length; j++)
                        {
                            boneTrs_temp[j] = boneTrs[bodyInfo.bones[j]];
                        }
                        renderer_body_cut.bones = boneTrs_temp;
                    }

                    renderer_body_origin_h.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_ORGN_HIDDEN");

                    jsonText = Resources.Load("ItemInfos/BodyInfo_F_OriginN_H") as TextAsset;
                    bodyInfo = null;
                    bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                    renderer_body_cut.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                    if (!isOptimizeGameObjects)
                    {
                        renderer_body_cut.rootBone = boneTrs[bodyInfo.rootBone];

                        Transform[] boneTrs_temp = new Transform[bodyInfo.bones.Length];
                        for (int j = 0; j < bodyInfo.bones.Length; j++)
                        {
                            boneTrs_temp[j] = boneTrs[bodyInfo.bones[j]];
                        }
                        renderer_body_cut.bones = boneTrs_temp;
                    }

                    SetItem(CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[1].ID_Face_I));
                    SetItem(CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[1].ID_Hair_I));
                    SetItem(CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[1].ID_Wear_I));
                    SetItem(CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[1].ID_Acc_I));

                    SetColor(ColorUIPart.Skin, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Skin));
                    SetColor(ColorUIPart.Eye, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Eye));
                    SetColor(ColorUIPart.Eyebrow, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Eyebrow));
                    SetColor(ColorUIPart.Hair, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Hair));
                    SetColor(ColorUIPart.Upper, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Upper));
                    SetColor(ColorUIPart.Lower, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Lower));
                    SetColor(ColorUIPart.Foot, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Foot));
                    SetColor(ColorUIPart.Pattern, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Pattern));
                    SetColor(ColorUIPart.Acc, CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Acc));

                    CustomizeManager.GetInstance.SetUI();
                }
                break;
        }
        anim.Rebind();
    }

    public void SetItem(CustomizeDataInfo.ItemSlotInfo _slotInfo)
    {
        CustomizeManager.GetInstance.SetSelectUISlotID(_slotInfo.id);
        switch (_slotInfo.id[0])
        {
            case '1':
                {
                    renderer_head.sharedMaterial.SetTexture("_SubTex_Eye", null);
                    renderer_head.sharedMaterial.SetTexture("_SubTex_Pupil", null);
                    renderer_head.sharedMaterial.SetTexture("_SubTex_Eyebrow", null);
                    renderer_head.sharedMaterial.SetTexture("_SubTex_Mouth", null);
                    renderer_head.sharedMaterial.SetTexture("_BaseTex", null);

                    for (int i = 0; i < _slotInfo.list_partId.Count; i++)
                    {
                        switch (_slotInfo.list_partId[i][1])
                        {
                            case '0':
                                {
                                    renderer_head.sharedMaterial.SetTexture("_SubTex_Eye", Resources.Load<Texture>("Customize/Faces/" + CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]).matPath));
                                }
                                break;
                            case '1':
                                {
                                    renderer_head.sharedMaterial.SetTexture("_SubTex_Pupil", Resources.Load<Texture>("Customize/Faces/" + CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]).matPath));
                                }
                                break;
                            case '2':
                                {
                                    renderer_head.sharedMaterial.SetTexture("_SubTex_Eyebrow", Resources.Load<Texture>("Customize/Faces/" + CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]).matPath));
                                }
                                break;
                            case '3':
                                {
                                    renderer_head.sharedMaterial.SetTexture("_SubTex_Mouth", Resources.Load<Texture>("Customize/Faces/" + CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]).matPath));
                                }
                                break;
                            case '4':
                                {
                                    renderer_head.sharedMaterial.SetTexture("_BaseTex", Resources.Load<Texture>("Customize/Faces/" + CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]).matPath));
                                }
                                break;
                        }
                    }
                    Color temp_color;
                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Skin), out temp_color))
                    {
                        renderer_head.sharedMaterial.SetColor("_BaseColor", temp_color);
                    }
                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Eye), out temp_color))
                    {
                        renderer_head.sharedMaterial.SetColor("_EyeColor", temp_color);
                    }
                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Eyebrow), out temp_color))
                    {
                        renderer_head.sharedMaterial.SetColor("_EyebrowColor", temp_color);
                    }
                    int genderNum = CustomizeManager.GetInstance.selectGenderNum;

                    string acc_id = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[genderNum].ID_Acc_I).list_partId[0];

                    if (acc_id != "40000" && acc_id[1] == '0' && acc_id != "40004")
                    {
                        meshFilter_head.GetComponent<MeshRenderer>().enabled = false;
                    }
                    else
                    {
                        meshFilter_head.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
                break;
            case '2':
                {
                    renderer_hair_f.gameObject.SetActive(false);
                    renderer_hair_b.gameObject.SetActive(false);
                    mesh_hair_origin = null;
                    mat_hair_origin = null;

                    for (int i = 0; i < _slotInfo.list_partId.Count; i++)
                    {
                        switch (_slotInfo.list_partId[i][1])
                        {
                            case '0':
                                {
                                    CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);
                                    meshFilter_hair_f.sharedMesh = Resources.Load<Mesh>("Customize/Hairs/" + itemInfo.meshPath);
                                    renderer_hair_f.sharedMaterial = Resources.Load<Material>("Customize/Hairs/" + itemInfo.matPath);

                                    Color temp_color;
                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Hair), out temp_color))
                                    {
                                        renderer_hair_f.sharedMaterial.SetColor("_BaseColor", temp_color);
                                    }

                                    int genderNum = CustomizeManager.GetInstance.selectGenderNum;

                                    string acc_id = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[genderNum].ID_Acc_I).list_partId[0];

                                    if (acc_id != "40000" && (acc_id[1] == '0' || acc_id[1] == '1' || acc_id[1] == '2'))
                                    {
                                        renderer_hair_f.gameObject.SetActive(false);
                                    }
                                    else
                                    {
                                        renderer_hair_f.gameObject.SetActive(true);
                                    }
                                }
                                break;
                            case '1':
                                {
                                    CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);

                                    mesh_hair_origin = Resources.Load<Mesh>("Customize/Hairs/" + itemInfo.meshPath);
                                    mat_hair_origin = Resources.Load<Material>("Customize/Hairs/" + itemInfo.matPath);

                                    int genderNum = CustomizeManager.GetInstance.selectGenderNum;

                                    string acc_id = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[genderNum].ID_Acc_I).list_partId[0];

                                    if (acc_id != "40000" && (acc_id[1] == '0' || acc_id[1] == '1'))
                                    {
                                        renderer_hair_b.gameObject.SetActive(false);
                                    }
                                    else if (acc_id[1] == '2')
                                    {
                                        if (genderNum == 0)
                                        {
                                            meshFilter_hair_b.sharedMesh = mesh_hair_m;
                                            renderer_hair_b.sharedMaterial = mat_hair_m;
                                        }
                                        else
                                        {
                                            meshFilter_hair_b.sharedMesh = mesh_hair_f;
                                            renderer_hair_b.sharedMaterial = mat_hair_f;
                                        }

                                        renderer_hair_b.gameObject.SetActive(true);
                                    }
                                    else
                                    {
                                        meshFilter_hair_b.sharedMesh = mesh_hair_origin;
                                        renderer_hair_b.sharedMaterial = mat_hair_origin;
                                        renderer_hair_b.gameObject.SetActive(true);
                                    }

                                    Color temp_color;
                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Hair), out temp_color))
                                    {
                                        renderer_hair_b.sharedMaterial.SetColor("_BaseColor", temp_color);
                                    }
                                }
                                break;
                        }
                    }
                }
                break;
            case '3':
                {
                    renderer_wear_u.gameObject.SetActive(false);
                    renderer_wear_u_c.gameObject.SetActive(false);
                    renderer_wear_l.gameObject.SetActive(false);
                    renderer_wear_l_s.gameObject.SetActive(false);
                    renderer_wear_f.gameObject.SetActive(false);

                    for (int i = 0; i < _slotInfo.list_partId.Count; i++)
                    {
                        switch (_slotInfo.list_partId[i][1])
                        {
                            case '0': // 상의.
                            case '6': // 전신.
                                {
                                    CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);
                                    renderer_wear_u.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                    renderer_wear_u.sharedMaterial = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                    TextAsset jsonText = Resources.Load("ItemInfos/WearInfo_" + itemInfo.meshPath) as TextAsset;
                                    CustomizeDataInfo.WearInfo wearInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                                    renderer_wear_u.localBounds = new Bounds(wearInfo.bounds_center, wearInfo.bounds_extent * 2f);

                                    if (!isOptimizeGameObjects)
                                    {
                                        renderer_wear_u.rootBone = boneTrs[wearInfo.rootBone];

                                        Transform[] boneTrs_temp = new Transform[wearInfo.bones.Length];
                                        for (int j = 0; j < wearInfo.bones.Length; j++)
                                        {
                                            boneTrs_temp[j] = boneTrs[wearInfo.bones[j]];
                                        }
                                        renderer_wear_u.bones = boneTrs_temp;
                                    }

                                    Color temp_color;
                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Upper), out temp_color))
                                    {
                                        renderer_wear_u.sharedMaterial.SetColor("_BaseColor", temp_color);
                                    }

                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Pattern), out temp_color))
                                    {
                                        renderer_wear_u.sharedMaterial.SetColor("_PatternColor", temp_color);
                                    }

                                    renderer_wear_u.sharedMaterial.SetColor("_Color_R", Color.white);
                                    renderer_wear_u.sharedMaterial.SetColor("_Color_G", Color.white);
                                    renderer_wear_u.sharedMaterial.SetColor("_Color_B", Color.white);

                                    renderer_wear_u.gameObject.SetActive(true);

                                    CustomizeDataInfo.WearInfo bodyInfo = null;

                                    if (_slotInfo.list_partId[i][1] == '6')
                                    {
                                        renderer_body_origin_h.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/" + itemInfo.meshPath + "_BODY");

                                        jsonText = Resources.Load("ItemInfos/BodyInfo_" + itemInfo.meshPath + "_BODY") as TextAsset;

                                        bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                                        renderer_body_origin_h.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);
                                    }
                                    else
                                    {
                                        if (CustomizeManager.GetInstance.selectGenderNum == 0)
                                        {
                                            renderer_body_origin_h.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_ORGN_HIDDEN");
                                            jsonText = Resources.Load("ItemInfos/BodyInfo_M_OriginN_H") as TextAsset;

                                        }
                                        else
                                        {
                                            renderer_body_origin_h.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_ORGN_HIDDEN");
                                            jsonText = Resources.Load("ItemInfos/BodyInfo_F_OriginN_H") as TextAsset;
                                        }
                                        bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                                        renderer_body_origin_h.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);
                                    }

                                    if (!isOptimizeGameObjects)
                                    {
                                        renderer_body_origin_h.rootBone = boneTrs[bodyInfo.rootBone];

                                        Transform[] boneTrs_temp = new Transform[bodyInfo.bones.Length];
                                        for (int j = 0; j < bodyInfo.bones.Length; j++)
                                        {
                                            boneTrs_temp[j] = boneTrs[bodyInfo.bones[j]];
                                        }
                                        renderer_body_origin_h.bones = boneTrs_temp;
                                    }
                                }
                                break;
                            case '1': // 상의컷.
                            case '7': // 전신컷.
                                {
                                    CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);
                                    renderer_wear_u_c.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                    renderer_wear_u_c.sharedMaterial = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                    TextAsset jsonText = Resources.Load("ItemInfos/WearInfo_" + itemInfo.meshPath) as TextAsset;
                                    CustomizeDataInfo.WearInfo wearInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                                    renderer_wear_u_c.localBounds = new Bounds(wearInfo.bounds_center, wearInfo.bounds_extent * 2f);
                                    if (!isOptimizeGameObjects)
                                    {
                                        renderer_wear_u_c.rootBone = boneTrs[wearInfo.rootBone];

                                        Transform[] boneTrs_temp = new Transform[wearInfo.bones.Length];
                                        for (int j = 0; j < wearInfo.bones.Length; j++)
                                        {
                                            boneTrs_temp[j] = boneTrs[wearInfo.bones[j]];
                                        }
                                        renderer_wear_u_c.bones = boneTrs_temp;
                                    }

                                    Color temp_color;
                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Upper), out temp_color))
                                    {
                                        renderer_wear_u_c.sharedMaterial.SetColor("_BaseColor", temp_color);
                                    }

                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Pattern), out temp_color))
                                    {
                                        renderer_wear_u_c.sharedMaterial.SetColor("_PatternColor", temp_color);
                                    }

                                    renderer_wear_u_c.sharedMaterial.SetColor("_Color_R", Color.white);
                                    renderer_wear_u_c.sharedMaterial.SetColor("_Color_G", Color.white);
                                    renderer_wear_u_c.sharedMaterial.SetColor("_Color_B", Color.white);
                                }
                                break;
                            case '2': // 하의.
                                {
                                    CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);
                                    renderer_wear_l.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                    renderer_wear_l.sharedMaterial = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                    TextAsset jsonText = Resources.Load("ItemInfos/WearInfo_" + itemInfo.meshPath) as TextAsset;
                                    CustomizeDataInfo.WearInfo wearInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                                    renderer_wear_l.localBounds = new Bounds(wearInfo.bounds_center, wearInfo.bounds_extent * 2f);
                                    if (!isOptimizeGameObjects)
                                    {
                                        renderer_wear_l.rootBone = boneTrs[wearInfo.rootBone];

                                        Transform[] boneTrs_temp = new Transform[wearInfo.bones.Length];
                                        for (int j = 0; j < wearInfo.bones.Length; j++)
                                        {
                                            boneTrs_temp[j] = boneTrs[wearInfo.bones[j]];
                                        }
                                        renderer_wear_l.bones = boneTrs_temp;
                                    }

                                    Color temp_color;
                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Lower), out temp_color))
                                    {
                                        renderer_wear_l.sharedMaterial.SetColor("_BaseColor", temp_color);
                                    }

                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Pattern), out temp_color))
                                    {
                                        renderer_wear_l.sharedMaterial.SetColor("_PatternColor", temp_color);
                                    }

                                    renderer_wear_l.sharedMaterial.SetColor("_Color_R", Color.white);
                                    renderer_wear_l.sharedMaterial.SetColor("_Color_G", Color.white);
                                    renderer_wear_l.sharedMaterial.SetColor("_Color_B", Color.white);

                                    renderer_wear_l.gameObject.SetActive(true);
                                }
                                break;
                            case '3': // 하의서브.
                                {
                                    CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);
                                    renderer_wear_l_s.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                    renderer_wear_l_s.sharedMaterial = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                    TextAsset jsonText = Resources.Load("ItemInfos/WearInfo_" + itemInfo.meshPath) as TextAsset;
                                    CustomizeDataInfo.WearInfo wearInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                                    renderer_wear_l_s.localBounds = new Bounds(wearInfo.bounds_center, wearInfo.bounds_extent * 2f);
                                    if (!isOptimizeGameObjects)
                                    {
                                        renderer_wear_l_s.rootBone = boneTrs[wearInfo.rootBone];

                                        Transform[] boneTrs_temp = new Transform[wearInfo.bones.Length];
                                        for (int j = 0; j < wearInfo.bones.Length; j++)
                                        {
                                            boneTrs_temp[j] = boneTrs[wearInfo.bones[j]];
                                        }
                                        renderer_wear_l_s.bones = boneTrs_temp;
                                    }

                                    Color temp_color;
                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Lower), out temp_color))
                                    {
                                        renderer_wear_l_s.sharedMaterial.SetColor("_BaseColor", temp_color);
                                    }

                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Pattern), out temp_color))
                                    {
                                        renderer_wear_l_s.sharedMaterial.SetColor("_PatternColor", temp_color);
                                    }

                                    renderer_wear_l_s.sharedMaterial.SetColor("_Color_R", Color.white);
                                    renderer_wear_l_s.sharedMaterial.SetColor("_Color_G", Color.white);
                                    renderer_wear_l_s.sharedMaterial.SetColor("_Color_B", Color.white);

                                    renderer_wear_l_s.gameObject.SetActive(true);
                                }
                                break;
                            case '4': // 신발.
                                {
                                    CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);
                                    renderer_wear_f.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                    renderer_wear_f.sharedMaterial = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                    TextAsset jsonText = Resources.Load("ItemInfos/WearInfo_" + itemInfo.meshPath) as TextAsset;
                                    CustomizeDataInfo.WearInfo wearInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                                    renderer_wear_f.localBounds = new Bounds(wearInfo.bounds_center, wearInfo.bounds_extent * 2f);
                                    if (!isOptimizeGameObjects)
                                    {
                                        renderer_wear_f.rootBone = boneTrs[wearInfo.rootBone];

                                        Transform[] boneTrs_temp = new Transform[wearInfo.bones.Length];
                                        for (int j = 0; j < wearInfo.bones.Length; j++)
                                        {
                                            boneTrs_temp[j] = boneTrs[wearInfo.bones[j]];
                                        }
                                        renderer_wear_f.bones = boneTrs_temp;
                                    }

                                    Color temp_color;
                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Foot), out temp_color))
                                    {
                                        renderer_wear_f.sharedMaterial.SetColor("_BaseColor", temp_color);
                                    }

                                    if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Pattern), out temp_color))
                                    {
                                        renderer_wear_f.sharedMaterial.SetColor("_PatternColor", temp_color);
                                    }

                                    renderer_wear_f.sharedMaterial.SetColor("_Color_R", Color.white);
                                    renderer_wear_f.sharedMaterial.SetColor("_Color_G", Color.white);
                                    renderer_wear_f.sharedMaterial.SetColor("_Color_B", Color.white);

                                    renderer_wear_f.gameObject.SetActive(true);
                                }
                                break;
                            case '5': // 손.
                                {
                                    //ItemInfo itemInfo = CustomizeManager.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);
                                    //renderer_wear_h.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                    //renderer_wear_h.sharedMaterial = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                    //TextAsset jsonText = Resources.Load("ItemInfos/WearInfo_" + _slotInfo.list_partId[i]) as TextAsset;
                                    //WearInfo wearInfo = JsonUtility.FromJson<WearInfo>(jsonText.text);

                                    //renderer_wear_h.localBounds = new Bounds(wearInfo.bounds_center, wearInfo.bounds_extent * 2f);
                                    //renderer_wear_h.rootBone = boneTrs[wearInfo.rootBone];

                                    //Transform[] boneTrs_temp = new Transform[wearInfo.bones.Length];
                                    //for (int j = 0; j < wearInfo.bones.Length; j++)
                                    //{
                                    //    boneTrs_temp[j] = boneTrs[wearInfo.bones[j]];
                                    //    //Debug.LogError(boneTrs_temp[j]);
                                    //}
                                    //renderer_wear_h.bones = boneTrs_temp;

                                    //renderer_wear_h.gameObject.SetActive(true);
                                }
                                break;
                        }
                    }
                }
                break;
            case '4':
                {
                    renderer_acc.gameObject.SetActive(false);

                    if (_slotInfo.id != "4000")
                    {
                        for (int i = 0; i < _slotInfo.list_partId.Count; i++)
                        {
                            CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(_slotInfo.list_partId[i]);
                            meshFilter_acc.sharedMesh = Resources.Load<Mesh>("Customize/Accs/" + itemInfo.meshPath);
                            renderer_acc.sharedMaterial = Resources.Load<Material>("Customize/Accs/" + itemInfo.matPath);


                            Color temp_color;
                            if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Acc), out temp_color))
                            {
                                renderer_acc.sharedMaterial.SetColor("_BaseColor", temp_color);
                            }

                            int genderNum = CustomizeManager.GetInstance.selectGenderNum;

                            TextAsset jsonText;

                            if (genderNum == 0)
                            {
                                jsonText = Resources.Load("ItemInfos/AccInfo_M_" + _slotInfo.list_partId[i]) as TextAsset;
                            }
                            else
                            {
                                jsonText = Resources.Load("ItemInfos/AccInfo_F_" + _slotInfo.list_partId[i]) as TextAsset;
                            }
                            CustomizeDataInfo.AccInfo accInfo = null;
                            if (jsonText != null)
                            {
                                accInfo = JsonUtility.FromJson<CustomizeDataInfo.AccInfo>(jsonText.text);
                            }

                            if (accInfo != null)
                            {
                                renderer_acc.transform.localPosition = accInfo.pos;
                                renderer_acc.transform.localScale = accInfo.scale;
                            }
                            else
                            {
                                if (genderNum == 0)
                                {
                                    renderer_acc.transform.localPosition = Vector3.zero;
                                    renderer_acc.transform.localScale = Vector3.one * 1f;
                                }
                                else
                                {
                                    renderer_acc.transform.localPosition = Vector3.zero;
                                    renderer_acc.transform.localScale = Vector3.one * 0.925f;
                                }
                            }
                            renderer_acc.transform.localRotation = Quaternion.identity;
                            renderer_acc.gameObject.SetActive(true);
                        }

                        if (_slotInfo.list_partId[0][1] == '0')
                        {
                            renderer_head.enabled = false;
                            meshFilter_hair_f.gameObject.SetActive(false);
                            meshFilter_hair_b.gameObject.SetActive(false);
                        }
                        else if (_slotInfo.list_partId[0][1] == '1')
                        {
                            renderer_head.enabled = true;
                            meshFilter_hair_f.gameObject.SetActive(false);
                            meshFilter_hair_b.gameObject.SetActive(false);
                        }
                        else
                        {
                            renderer_head.enabled = true;
                            int genderNum = CustomizeManager.GetInstance.selectGenderNum;

                            if (_slotInfo.list_partId[0][1] == '2')
                            {
                                if (genderNum == 0)
                                {
                                    meshFilter_hair_b.sharedMesh = mesh_hair_m;
                                    renderer_hair_b.sharedMaterial = mat_hair_m;
                                }
                                else
                                {
                                    meshFilter_hair_b.sharedMesh = mesh_hair_f;
                                    renderer_hair_b.sharedMaterial = mat_hair_f;
                                }

                                meshFilter_hair_f.gameObject.SetActive(false);
                            }
                            else
                            {
                                meshFilter_hair_b.sharedMesh = mesh_hair_origin;
                                renderer_hair_b.sharedMaterial = mat_hair_origin;

                                CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[genderNum].ID_Hair_I);

                                meshFilter_hair_f.gameObject.SetActive(false);

                                for (int i = 0; i < itemSlotInfo.list_partId.Count; i++)
                                {
                                    if (itemSlotInfo.list_partId[i][1] == '0')
                                    {
                                        meshFilter_hair_f.gameObject.SetActive(true);
                                    }
                                }
                            }

                            if (meshFilter_hair_b.sharedMesh != null && renderer_hair_b.sharedMaterial != null)
                            {
                                meshFilter_hair_b.gameObject.SetActive(true);

                                if (ColorUtility.TryParseHtmlString("#" + CustomizeManager.GetInstance.GetColorInfo(ColorUIPart.Hair), out Color temp_color))
                                {
                                    renderer_hair_b.sharedMaterial.SetColor("_BaseColor", temp_color);
                                }
                            }
                        }
                    }
                    else
                    {
                        renderer_head.enabled = true;

                        int genderNum = CustomizeManager.GetInstance.selectGenderNum;
                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, CustomizeManager.GetInstance.modelData.modelDatas[genderNum].ID_Hair_I);

                        for (int i = 0; i < itemSlotInfo.list_partId.Count; i++)
                        {
                            if (itemSlotInfo.list_partId[i][1] == '0')
                            {
                                meshFilter_hair_f.gameObject.SetActive(true);
                            }
                            else if (itemSlotInfo.list_partId[i][1] == '1')
                            {
                                meshFilter_hair_b.gameObject.SetActive(true);
                            }
                        }
                    }
                }
                break;
        }
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        anim.Update(1f);
    }

    public void SetColor(ColorUIPart part, string setHexColor)
    {
        Color temp_color;
        if (ColorUtility.TryParseHtmlString("#" + setHexColor, out temp_color))
        {
            CustomizeManager.GetInstance.SetColorInfo(part, setHexColor);

            switch (part)
            {
                case ColorUIPart.Skin:
                    {
                        renderer_head.sharedMaterial.SetColor("_BaseColor", temp_color);
                        mat_body.SetColor("_BaseColor", temp_color);
                    }
                    break;
                case ColorUIPart.Eye:
                    {
                        renderer_head.sharedMaterial.SetColor("_EyeColor", temp_color);
                    }
                    break;
                case ColorUIPart.Eyebrow:
                    {
                        renderer_head.sharedMaterial.SetColor("_EyebrowColor", temp_color);
                    }
                    break;
                case ColorUIPart.Hair:
                    {
                        if (renderer_hair_f.sharedMaterial != null)
                        {
                            renderer_hair_f.sharedMaterial.SetColor("_BaseColor", temp_color);
                        }
                        if (renderer_hair_b.sharedMaterial != null)
                        {
                            renderer_hair_b.sharedMaterial.SetColor("_BaseColor", temp_color);
                        }
                    }
                    break;
                case ColorUIPart.Upper:
                    {
                        if (renderer_wear_u.sharedMaterial != null)
                        {
                            renderer_wear_u.sharedMaterial.SetColor("_BaseColor", temp_color);
                        }
                        if (renderer_wear_u_c.sharedMaterial != null)
                        {
                            renderer_wear_u_c.sharedMaterial.SetColor("_BaseColor", temp_color);
                        }
                    }
                    break;
                case ColorUIPart.Lower:
                    {
                        if (renderer_wear_l.sharedMaterial != null)
                        {
                            renderer_wear_l.sharedMaterial.SetColor("_BaseColor", temp_color);
                        }
                        if (renderer_wear_l_s.sharedMaterial != null)
                        {
                            renderer_wear_l_s.sharedMaterial.SetColor("_BaseColor", temp_color);
                        }
                    }
                    break;
                case ColorUIPart.Foot:
                    {
                        if (renderer_wear_f.sharedMaterial != null)
                        {
                            renderer_wear_f.sharedMaterial.SetColor("_BaseColor", temp_color);
                        }
                    }
                    break;
                case ColorUIPart.Pattern:
                    {
                        if (renderer_wear_u.sharedMaterial != null)
                        {
                            renderer_wear_u.sharedMaterial.SetColor("_PatternColor", temp_color);
                        }
                        if (renderer_wear_u_c.sharedMaterial != null)
                        {
                            renderer_wear_u_c.sharedMaterial.SetColor("_PatternColor", temp_color);
                        }
                        if (renderer_wear_l.sharedMaterial != null)
                        {
                            renderer_wear_l.sharedMaterial.SetColor("_PatternColor", temp_color);
                        }
                        if (renderer_wear_l_s.sharedMaterial != null)
                        {
                            renderer_wear_l_s.sharedMaterial.SetColor("_PatternColor", temp_color);
                        }
                        if (renderer_wear_f.sharedMaterial != null)
                        {
                            renderer_wear_f.sharedMaterial.SetColor("_PatternColor", temp_color);
                        }
                    }
                    break;
                case ColorUIPart.Acc:
                    {
                        renderer_acc.sharedMaterial.SetColor("_BaseColor", temp_color);
                    }
                    break;
            }
            CustomizeManager.GetInstance.SetColor_Others(part, setHexColor);
        }
    }
}
