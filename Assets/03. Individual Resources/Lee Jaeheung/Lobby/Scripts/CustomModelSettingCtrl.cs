using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomModelData
{
    public string Gender;

    public string ID_Face_I;
    public string Hex_Skin_C;
    public string Hex_Eye_C;
    public string Hex_Eyebrow_C;

    public string ID_Hair_I;
    public string Hex_Hair_C;

    public string ID_Wear_I;
    public string Hex_Upper_C;
    public string Hex_Lower_C;
    public string Hex_Foot_C;
    public string Hex_Pattern_C;
    public string Hex_Acc_C;

    public string ID_Acc_I;
}

public enum CustomModelViewState
{
    Normal, HandCut, HalfCut, HalfCut2
}

public class CustomModelSettingCtrl : MonoBehaviour
{
    private string user_id;
    private CustomModelViewState state;
    private Renderer renderer_hand = null;
    private CustomModelData modelData;
    private float toneVelue = 0.5f;
    private bool isOptimizeGameObjects = false;

    private MeshRenderer meshRenderer_face;
    private List<MeshRenderer> list_meshRenderer = new List<MeshRenderer>();
    private List<MeshFilter> list_meshFilter = new List<MeshFilter>();
    private List<SkinnedMeshRenderer> list_skinnedMesh = new List<SkinnedMeshRenderer>();
    private List<int[]> list_boneInfos = new List<int[]>();

    public enum OptimizeQualityState
    {
        None, High, Normal, Low
    }
    public OptimizeQualityState state_optimize = OptimizeQualityState.None;

    private Transform[] boneTrs = new Transform[59];

    private SkinnedMeshRenderer skin_body_origin;

    private List<RenderTexture> renderTextures = new List<RenderTexture>();
   
    public enum InitState
    {
        Manual, Auto_Random, Auto_Random_Cut, Mine
    }

    public InitState state_init = InitState.Mine;

    public enum RenderState
    {
        Enable, Ghost, Disable
    }

    public RenderState state_render = RenderState.Enable;

    public class KeepRenderData
    {
        public Renderer renderer;
        public Material material;

        public KeepRenderData(Renderer r, Material m)
        {
            renderer = r;
            material = m;
        }
    }

    private List<KeepRenderData> list_keepRenderData;
    private Material mat_ghost;

    private int render_queue = 2550;

    private void Start()
    {
        StartCoroutine(StartInit());
    }

    IEnumerator StartInit()
    {
        while (GameDataManager.instance.state_userInfo != GameDataManager.UserInfoState.Completed)
            yield return null;


        if (state_init == InitState.Auto_Random)
        {
            InitRandom();
        }
        else if (state_init == InitState.Auto_Random_Cut)
        {
            InitRandom(CustomModelViewState.HalfCut2);
        }
        else if (state_init == InitState.Mine)
        {
            Init(GameDataManager.instance.userInfo_mine.customModelData);
        }
    }

    //public void Init(string _user_id, CustomModelViewState _state = CustomModelViewState.Normal, Renderer _renderer_hand = null, float _toneVelue = 0.3f)
    //{
    //    user_id = _user_id;
    //    state = _state;
    //    renderer_hand = _renderer_hand;
    //    toneVelue = _toneVelue;

    //    GameDataManager.instance.StartDownloadModelData(this, user_id);
    //}

    public  void Init(CustomModelData _modelData, CustomModelViewState _state = CustomModelViewState.Normal, Renderer _renderer_hand = null, float _toneVelue = 0.3f)
    {
        modelData = _modelData;
        state = _state;
        renderer_hand = _renderer_hand;
        toneVelue = _toneVelue;

        SetMedel();
    }

    public void InitRandom(CustomModelViewState _state = CustomModelViewState.Normal, Renderer _renderer_hand = null, float _toneVelue = 0.3f)
    {
        user_id = "";
        state = _state;
        renderer_hand = _renderer_hand;
        toneVelue = _toneVelue;

        modelData = GetRandomModelData();
        SetMedel();
    }

    public void InitRandom(int genderNum, CustomModelViewState _state = CustomModelViewState.Normal, Renderer _renderer_hand = null, float _toneVelue = 0.3f)
    {
        user_id = "";
        state = _state;
        renderer_hand = _renderer_hand;
        toneVelue = _toneVelue;

        modelData = GetRandomModelData(genderNum);
        SetMedel();
    }

    public void InitCPU(CustomModelViewState _state = CustomModelViewState.Normal, Renderer _renderer_hand = null, float _toneVelue = 0.3f)
    {
        modelData = GameDataManager.instance.userInfo_mine.customModelData;
        if (modelData == null)
        {
            InitRandom(_state, _renderer_hand, _toneVelue);
            return;
        }
        user_id = "";
        state = _state;
        renderer_hand = _renderer_hand;
        toneVelue = _toneVelue;

        SetMedel();
    }

    public static CustomModelData GetRandomModelData(int genderNum = -1)
    {
        CustomModelData modelData_r = new CustomModelData();
        switch (genderNum)
        {
            case 0:
                {
                    modelData_r.Gender = "m";
                }
                break;
            case 1:
                {
                    modelData_r.Gender = "f";
                }
                break;
            case -1:
                {
                    modelData_r.Gender = ((Random.Range(0f, 1f) >= 0.5f) ? "m" : "f");
                }
                break;
        }

        modelData_r.ID_Face_I = ((int)Random.Range(1000f, 1013.9999f)).ToString();
        modelData_r.ID_Hair_I = ((int)Random.Range(2000f, 2008.9999f)).ToString();
        modelData_r.ID_Wear_I = ((int)Random.Range(3000f, 3015.9999f)).ToString();
        modelData_r.ID_Acc_I = ((int)Random.Range(4000f, 4035.9999f)).ToString();

        float b = Random.Range(0.1f, 0.7f);

        float g = Random.Range(b, b + 0.15f);

        float r = Mathf.Clamp01(Random.Range(g + 0.1f, g + 0.5f));

        modelData_r.Hex_Skin_C = ColorUtility.ToHtmlStringRGB(new Color(r, g, b));

        Color hairColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        modelData_r.Hex_Hair_C = ColorUtility.ToHtmlStringRGB(hairColor);

        modelData_r.Hex_Eyebrow_C = ColorUtility.ToHtmlStringRGB(Color.Lerp(hairColor, Color.black, Random.Range(0f, 1f)));

        modelData_r.Hex_Eye_C = ColorUtility.ToHtmlStringRGB(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

        modelData_r.Hex_Upper_C = ColorUtility.ToHtmlStringRGB(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        modelData_r.Hex_Lower_C = ColorUtility.ToHtmlStringRGB(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        modelData_r.Hex_Foot_C = ColorUtility.ToHtmlStringRGB(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        modelData_r.Hex_Pattern_C = ColorUtility.ToHtmlStringRGB(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        modelData_r.Hex_Acc_C = ColorUtility.ToHtmlStringRGB(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));

        return modelData_r;
    }

    public static CustomModelData GetDefualModelData(int genderNum = -1)
    {
        CustomModelData modelData_r = new CustomModelData();
        switch (genderNum)
        {
            case 0:
                {
                    modelData_r.Gender = "m";
                }
                break;
            case 1:
                {
                    modelData_r.Gender = "f";
                }
                break;
            case -1:
                {
                    modelData_r.Gender = ((Random.Range(0f, 1f) >= 0.5f) ? "m" : "f");
                }
                break;
        }

        modelData_r.ID_Face_I = "1000";
        modelData_r.ID_Hair_I = "2000";
        modelData_r.ID_Wear_I = "3000";
        modelData_r.ID_Acc_I = "4000";

        modelData_r.Hex_Skin_C = ColorUtility.ToHtmlStringRGB(new Color32(224, 162, 138, 255));

        Color hairColor = Color.black;
        modelData_r.Hex_Hair_C = ColorUtility.ToHtmlStringRGB(hairColor);

        modelData_r.Hex_Eyebrow_C = ColorUtility.ToHtmlStringRGB(Color.black);

        modelData_r.Hex_Eye_C = ColorUtility.ToHtmlStringRGB(Color.black);

        modelData_r.Hex_Upper_C = ColorUtility.ToHtmlStringRGB(Color.gray);
        modelData_r.Hex_Lower_C = ColorUtility.ToHtmlStringRGB(Color.gray);
        modelData_r.Hex_Foot_C = ColorUtility.ToHtmlStringRGB(Color.gray);
        modelData_r.Hex_Pattern_C = ColorUtility.ToHtmlStringRGB(Color.black);
        modelData_r.Hex_Acc_C = ColorUtility.ToHtmlStringRGB(Color.black);

        return modelData_r;
    }

    public void SetModelData(CustomModelData _modelData)
    {
        modelData = _modelData;
    }

    public void SetMedel()
    {
        int genderNum;
        if (modelData.Gender.ToUpper() == "M")
        {
            genderNum = 0;
        }
        else
        {
            genderNum = 1;
        }

        isOptimizeGameObjects = false;

        Transform[] boneTrs_temp = transform.GetComponentsInChildren<Transform>(true);

        Animator anim = null;
        Transform headTr = null;
        Transform accTr = null;
        CustomizeDataInfo.AccInfo accInfo = null;
        bool isCustomF = false;

        if (state != CustomModelViewState.HalfCut)
        {
            anim = transform.GetComponentInParent<Animator>();

            if (modelData.Gender.ToUpper() == "M")
            {
                anim.avatar = Resources.Load<Avatar>("Customize/Bodies/Avatar_MALE");
            }
            else
            {
                anim.avatar = Resources.Load<Avatar>("Customize/Bodies/Avatar_FEMALE");
            }

            for (int i = 0; i < boneTrs_temp.Length; i++)
            {
                int index = WearInfoCreator.BoneNameToIndex(boneTrs_temp[i].name);
                if (index == -1)
                {
                    if (boneTrs_temp[i].name == "BODY_ORG_HIDDEN")
                    {
                        isOptimizeGameObjects = transform.Find("BODY_ORG_HIDDEN").GetComponent<SkinnedMeshRenderer>().rootBone == null;
                    }
                    continue;
                }

                boneTrs[index] = boneTrs_temp[i];
            }
        }

        TextAsset jsonText;

        for (int i = 0; i < boneTrs_temp.Length; i++)
        {
            switch (boneTrs_temp[i].name)
            {
                case "ITEM_HEAD":
                    {
                        headTr = boneTrs_temp[i];

                        MeshFilter meshFilter = boneTrs_temp[i].GetComponent<MeshFilter>();
                        if (modelData.Gender.ToUpper() == "M")
                        {
                            meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_HEAD");
                        }
                        else
                        {
                            meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_HEAD");
                        }

                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Face_I);

                        MeshRenderer meshRenderer = boneTrs_temp[i].GetComponent<MeshRenderer>();
                        meshRenderer.material = Resources.Load<Material>("Customize/Bodies/Mat_Custom_Face");
                        meshRenderer.material.SetTexture("_SubTex_Eye", null);
                        meshRenderer.material.SetTexture("_SubTex_Pupil", null);
                        meshRenderer.material.SetTexture("_SubTex_Eyebrow", null);
                        meshRenderer.material.SetTexture("_SubTex_Mouth", null);
                        meshRenderer.material.SetTexture("_BaseTex", null);

                        meshRenderer.material.SetFloat("_ToneP", toneVelue);

                        Color temp_color;

                        if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Skin_C, out temp_color))
                        {
                            meshRenderer.material.SetColor("_BaseColor", temp_color);
                        }

                        if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Eye_C, out temp_color))
                        {
                            meshRenderer.material.SetColor("_EyeColor", temp_color);
                        }

                        if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Eyebrow_C, out temp_color))
                        {
                            meshRenderer.material.SetColor("_EyebrowColor", temp_color);
                        }

                        for (int j = 0; j < itemSlotInfo.list_partId.Count; j++)
                        {
                            switch (itemSlotInfo.list_partId[j][1])
                            {
                                case '0':
                                    {
                                        meshRenderer.material.SetTexture("_SubTex_Eye", Resources.Load<Texture>("Customize/Faces/"
                                            + CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]).matPath));
                                    }
                                    break;
                                case '1':
                                    {
                                        meshRenderer.material.SetTexture("_SubTex_Pupil", Resources.Load<Texture>("Customize/Faces/"
                                            + CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]).matPath));
                                    }
                                    break;
                                case '2':
                                    {
                                        meshRenderer.material.SetTexture("_SubTex_Eyebrow", Resources.Load<Texture>("Customize/Faces/"
                                            + CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]).matPath));
                                    }
                                    break;
                                case '3':
                                    {
                                        meshRenderer.material.SetTexture("_SubTex_Mouth", Resources.Load<Texture>("Customize/Faces/"
                                            + CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]).matPath));
                                    }
                                    break;
                                case '4':
                                    {
                                        meshRenderer.material.SetTexture("_BaseTex", Resources.Load<Texture>("Customize/Faces/"
                                            + CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]).matPath));
                                    }
                                    break;
                            }
                        }

                        bool isActive = true;

                        itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Acc_I);

                        if (itemSlotInfo.list_partId[0] != "40000" && itemSlotInfo.list_partId[0][1] == '0')
                        {
                            isActive = false;
                        }

                        if (isActive)
                        {
                            meshRenderer_face = meshRenderer;
                        }
                        meshRenderer.enabled = isActive;
                    }
                    break;
                case "BODY_HALF":
                    {
                        MeshRenderer meshRenderer = boneTrs_temp[i].GetComponent<MeshRenderer>();
                        MeshFilter meshFilter = boneTrs_temp[i].GetComponent<MeshFilter>();

                        if (modelData.Gender.ToUpper() == "M")
                        {
                            meshRenderer.material = Resources.Load<Material>("Customize/Bodies/Mat_M_SKIN");
                            meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_CUTN_HIDDEN_02");
                        }
                        else
                        {
                            meshRenderer.material = Resources.Load<Material>("Customize/Bodies/Mat_F_SKIN");
                            meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_CUTN_HIDDEN_02");
                        }

                        if (state == CustomModelViewState.HalfCut)
                        {
                            meshRenderer.material.SetInt("_STATE", 3);
                            meshRenderer.material.shader = Shader.Find(meshRenderer.material.shader.name);
                        }

                        meshRenderer.material.SetFloat("_ToneP", toneVelue);

                        Color temp_color;
                        if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Skin_C, out temp_color))
                        {
                            meshRenderer.material.SetColor("_BaseColor", temp_color);
                        }
                    }
                    break;
                case "WEAR_HALF":
                    {
                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Wear_I);

                        for (int j = 0; j < itemSlotInfo.list_partId.Count; j++)
                        {
                            if (itemSlotInfo.list_partId[j][1] == '1' || itemSlotInfo.list_partId[j][1] == '7')
                            {
                                CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]);

                                MeshRenderer meshRenderer = boneTrs_temp[i].GetComponent<MeshRenderer>();
                                MeshFilter meshFilter = boneTrs_temp[i].GetComponent<MeshFilter>();
                                meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                meshRenderer.material = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                meshRenderer.material.SetFloat("_ToneP", toneVelue);

                                Color temp_color;
                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Upper_C, out temp_color))
                                {
                                    meshRenderer.material.SetColor("_BaseColor", temp_color);
                                }

                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Pattern_C, out temp_color))
                                {
                                    meshRenderer.material.SetColor("_PatternColor", temp_color);
                                }

                                meshRenderer.material.SetColor("_Color_R", Color.white);
                                meshRenderer.material.SetColor("_Color_G", Color.white);
                                meshRenderer.material.SetColor("_Color_B", Color.white);
                            }
                        }
                    }
                    break;
                case "BODY_CUT":
                    {
                        SkinnedMeshRenderer skinnedMesh = boneTrs_temp[i].GetComponent<SkinnedMeshRenderer>();

                        if (modelData.Gender.ToUpper() == "M")
                        {
                            skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_M_SKIN");
                            skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_CUT");
                            jsonText = Resources.Load("ItemInfos/BodyInfo_M_Cut") as TextAsset;
                        }
                        else
                        {
                            skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_F_SKIN");
                            skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_CUT");
                            jsonText = Resources.Load("ItemInfos/BodyInfo_F_Cut") as TextAsset;
                        }

                        CustomizeDataInfo.WearInfo bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                        skinnedMesh.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                        if (!isOptimizeGameObjects)
                        {
                            skinnedMesh.rootBone = boneTrs[bodyInfo.rootBone];

                            Transform[] boneTrs_set = new Transform[bodyInfo.bones.Length];
                            for (int j = 0; j < bodyInfo.bones.Length; j++)
                            {
                                boneTrs_set[j] = boneTrs[bodyInfo.bones[j]];
                            }
                            skinnedMesh.bones = boneTrs_set;
                        }

                        skinnedMesh.material.SetFloat("_ToneP", toneVelue);

                        Color temp_color;
                        if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Skin_C, out temp_color))
                        {
                            skinnedMesh.material.SetColor("_BaseColor", temp_color);
                        }

                        boneTrs_temp[i].gameObject.SetActive(false);
                    }
                    break;
                case "BODY_CUT_02":
                    {
                        SkinnedMeshRenderer skinnedMesh = boneTrs_temp[i].GetComponent<SkinnedMeshRenderer>();

                        if (modelData.Gender.ToUpper() == "M")
                        {
                            skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_M_SKIN");
                            skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_CUT_02");
                            jsonText = Resources.Load("ItemInfos/BodyInfo_M_Cut_02") as TextAsset;
                        }
                        else
                        {
                            skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_F_SKIN");
                            skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_CUT_02");
                            jsonText = Resources.Load("ItemInfos/BodyInfo_F_Cut_02") as TextAsset;
                        }

                        CustomizeDataInfo.WearInfo bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                        skinnedMesh.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                        if (!isOptimizeGameObjects)
                        {
                            skinnedMesh.rootBone = boneTrs[bodyInfo.rootBone];

                            Transform[] boneTrs_set = new Transform[bodyInfo.bones.Length];
                            for (int j = 0; j < bodyInfo.bones.Length; j++)
                            {
                                boneTrs_set[j] = boneTrs[bodyInfo.bones[j]];
                            }
                            skinnedMesh.bones = boneTrs_set;
                        }

                        skinnedMesh.material.SetFloat("_ToneP", toneVelue);

                        Color temp_color;
                        if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Skin_C, out temp_color))
                        {
                            skinnedMesh.material.SetColor("_BaseColor", temp_color);
                        }

                        boneTrs_temp[i].gameObject.SetActive(false);
                    }
                    break;
                case "BODY_CUT_HIDDEN":
                    {
                        SkinnedMeshRenderer skinnedMesh = boneTrs_temp[i].GetComponent<SkinnedMeshRenderer>();

                        if (modelData.Gender.ToUpper() == "M")
                        {
                            skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_M_SKIN");
                            skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_CUT_HIDDEN");
                            jsonText = Resources.Load("ItemInfos/BodyInfo_M_Cut_H") as TextAsset;
                        }
                        else
                        {
                            skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_F_SKIN");
                            skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_CUT_HIDDEN");
                            jsonText = Resources.Load("ItemInfos/BodyInfo_F_Cut_H") as TextAsset;
                        }

                        CustomizeDataInfo.WearInfo bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                        skinnedMesh.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                        if (!isOptimizeGameObjects)
                        {
                            skinnedMesh.rootBone = boneTrs[bodyInfo.rootBone];

                            Transform[] boneTrs_set = new Transform[bodyInfo.bones.Length];
                            for (int j = 0; j < bodyInfo.bones.Length; j++)
                            {
                                boneTrs_set[j] = boneTrs[bodyInfo.bones[j]];
                            }
                            skinnedMesh.bones = boneTrs_set;
                        }

                        skinnedMesh.material.SetFloat("_ToneP", toneVelue);

                        Color temp_color;
                        if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Skin_C, out temp_color))
                        {
                            skinnedMesh.material.SetColor("_BaseColor", temp_color);
                        }
                        if (state != CustomModelViewState.Normal)
                        {
                            boneTrs_temp[i].gameObject.SetActive(true);
                            list_skinnedMesh.Add(skinnedMesh);
                        }
                        else
                        {
                            boneTrs_temp[i].gameObject.SetActive(false);
                        }
                    }
                    break;
                case "BODY_CUT_HIDDEN_02":
                    {
                        SkinnedMeshRenderer skinnedMesh = boneTrs_temp[i].GetComponent<SkinnedMeshRenderer>();

                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Wear_I);

                        int fullBodyNum = -1;

                        for (int j = 0; j < itemSlotInfo.list_partId.Count; j++)
                        {
                            if (itemSlotInfo.list_partId[j][1] == '7')
                            {
                                fullBodyNum = j;
                                break;
                            }
                        }
                        if (fullBodyNum != -1)
                        {
                            CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[fullBodyNum]);

                            skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_M_SKIN");
                            skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/" + itemInfo.meshPath + "_BODY");
                            jsonText = Resources.Load("ItemInfos/BodyInfo_" + itemInfo.meshPath + "_BODY") as TextAsset;
                        }
                        else
                        {
                            if (modelData.Gender.ToUpper() == "M")
                            {
                                skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_M_SKIN");
                                skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_CUTN_HIDDEN_02");
                                jsonText = Resources.Load("ItemInfos/BodyInfo_M_CutN_H") as TextAsset;
                            }
                            else
                            {
                                skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_F_SKIN");
                                skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_CUTN_HIDDEN_02");
                                jsonText = Resources.Load("ItemInfos/BodyInfo_F_CutN_H") as TextAsset;
                            }
                        }

                        CustomizeDataInfo.WearInfo bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                        skinnedMesh.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                        if (!isOptimizeGameObjects)
                        {
                            skinnedMesh.rootBone = boneTrs[bodyInfo.rootBone];

                            Transform[] boneTrs_set = new Transform[bodyInfo.bones.Length];
                            for (int j = 0; j < bodyInfo.bones.Length; j++)
                            {
                                boneTrs_set[j] = boneTrs[bodyInfo.bones[j]];
                            }
                            skinnedMesh.bones = boneTrs_set;
                        }

                        skinnedMesh.material.SetFloat("_ToneP", toneVelue);

                        Color temp_color;
                        if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Skin_C, out temp_color))
                        {
                            skinnedMesh.material.SetColor("_BaseColor", temp_color);
                        }

                        if (state == CustomModelViewState.HalfCut2)
                        {
                            boneTrs_temp[i].gameObject.SetActive(true);
                            list_skinnedMesh.Add(skinnedMesh);
                        }
                        else
                        {
                            boneTrs_temp[i].gameObject.SetActive(false);
                        }
                    }
                    break;
                case "BODY_ORG":
                    {
                        SkinnedMeshRenderer skinnedMesh = boneTrs_temp[i].GetComponent<SkinnedMeshRenderer>();
                        skin_body_origin = skinnedMesh;
                        if (modelData.Gender.ToUpper() == "M")
                        {
                            skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_M_SKIN");
                            skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_ORG");
                            jsonText = Resources.Load("ItemInfos/BodyInfo_M_Origin") as TextAsset;
                        }
                        else
                        {
                            skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_F_SKIN");
                            skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_ORG");
                            jsonText = Resources.Load("ItemInfos/BodyInfo_F_Origin") as TextAsset;
                        }

                        CustomizeDataInfo.WearInfo bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                        skinnedMesh.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                        if (!isOptimizeGameObjects)
                        {
                            skinnedMesh.rootBone = boneTrs[bodyInfo.rootBone];

                            Transform[] boneTrs_set = new Transform[bodyInfo.bones.Length];
                            for (int j = 0; j < bodyInfo.bones.Length; j++)
                            {
                                boneTrs_set[j] = boneTrs[bodyInfo.bones[j]];
                            }
                            skinnedMesh.bones = boneTrs_set;
                        }

                        skinnedMesh.material.SetFloat("_ToneP", toneVelue);

                        Color temp_color;
                        if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Skin_C, out temp_color))
                        {
                            skinnedMesh.material.SetColor("_BaseColor", temp_color);
                        }

                        boneTrs_temp[i].gameObject.SetActive(false);
                    }
                    break;
                case "BODY_ORG_HIDDEN":
                    {
                        SkinnedMeshRenderer skinnedMesh = boneTrs_temp[i].GetComponent<SkinnedMeshRenderer>();

                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Wear_I);

                        int fullBodyNum = -1;

                        for (int j = 0; j < itemSlotInfo.list_partId.Count; j++)
                        {
                            if (itemSlotInfo.list_partId[j][1] == '6')
                            {
                                fullBodyNum = j;
                                break;
                            }
                        }
                        if (fullBodyNum != -1)
                        {
                            CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[fullBodyNum]);

                            skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_M_SKIN");
                            skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/" + itemInfo.meshPath + "_BODY");
                            jsonText = Resources.Load("ItemInfos/BodyInfo_" + itemInfo.meshPath + "_BODY") as TextAsset;
                        }
                        else
                        {
                            if (modelData.Gender.ToUpper() == "M")
                            {
                                skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_M_SKIN");
                                skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_ORGN_HIDDEN");
                                jsonText = Resources.Load("ItemInfos/BodyInfo_M_OriginN_H") as TextAsset;
                            }
                            else
                            {
                                skinnedMesh.material = Resources.Load<Material>("Customize/Bodies/Mat_F_SKIN");
                                skinnedMesh.sharedMesh = Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_ORGN_HIDDEN");
                                jsonText = Resources.Load("ItemInfos/BodyInfo_F_OriginN_H") as TextAsset;
                            }
                        }

                        CustomizeDataInfo.WearInfo bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                        skinnedMesh.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);

                        if (!isOptimizeGameObjects)
                        {
                            skinnedMesh.rootBone = boneTrs[bodyInfo.rootBone];

                            Transform[] boneTrs_set = new Transform[bodyInfo.bones.Length];
                            for (int j = 0; j < bodyInfo.bones.Length; j++)
                            {
                                boneTrs_set[j] = boneTrs[bodyInfo.bones[j]];
                            }
                            skinnedMesh.bones = boneTrs_set;
                        }

                        skinnedMesh.material.SetFloat("_ToneP", toneVelue);

                        Color temp_color;
                        if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Skin_C, out temp_color))
                        {
                            skinnedMesh.material.SetColor("_BaseColor", temp_color);
                        }
                        if (state == CustomModelViewState.Normal)
                        {
                            boneTrs_temp[i].gameObject.SetActive(true);
                            list_skinnedMesh.Add(skinnedMesh);
                            list_boneInfos.Add(bodyInfo.bones);
                        }
                        else
                        {
                            boneTrs_temp[i].gameObject.SetActive(false);
                        }
                    }
                    break;
                case "ITEM_HAIR_F":
                    {
                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Hair_I);

                        boneTrs_temp[i].gameObject.SetActive(false);
                        for (int j = 0; j < itemSlotInfo.list_partId.Count; j++)
                        {
                            if (itemSlotInfo.list_partId[j][1] == '0')
                            {

                                CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]);

                                MeshRenderer meshRenderer = boneTrs_temp[i].GetComponent<MeshRenderer>();
                                MeshFilter meshFilter = boneTrs_temp[i].GetComponent<MeshFilter>();
                                meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Hairs/" + itemInfo.meshPath);
                                meshRenderer.material = Resources.Load<Material>("Customize/Hairs/" + itemInfo.matPath);

                                meshRenderer.material.SetFloat("_ToneP", toneVelue);

                                Color temp_color;
                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Hair_C, out temp_color))
                                {
                                    meshRenderer.material.SetColor("_BaseColor", temp_color);
                                }


                                bool isActive = true;

                                itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Acc_I);

                                if (itemSlotInfo.list_partId[0] != "40000" && (itemSlotInfo.list_partId[0][1] == '0' || itemSlotInfo.list_partId[0][1] == '1' || itemSlotInfo.list_partId[0][1] == '2'))
                                {
                                    isActive = false;
                                }

                                if (isActive)
                                {
                                    list_meshRenderer.Add(meshRenderer);
                                    list_meshFilter.Add(meshFilter);
                                }
                                boneTrs_temp[i].gameObject.SetActive(isActive);
                            }
                        }
                    }
                    break;
                case "ITEM_HAIR_B":
                    {
                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Hair_I);

                        boneTrs_temp[i].gameObject.SetActive(false);

                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo2 = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Acc_I);
                        if (itemSlotInfo2.list_partId[0][1] == '2')
                        {
                            CustomizeDataInfo.ItemInfo itemInfo;
                            if (genderNum == 0)
                            {
                                itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo("21001");
                            }
                            else
                            {
                                itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo("21018");
                            }

                            MeshRenderer meshRenderer = boneTrs_temp[i].GetComponent<MeshRenderer>();
                            MeshFilter meshFilter = boneTrs_temp[i].GetComponent<MeshFilter>();
                            list_meshRenderer.Add(meshRenderer);
                            list_meshFilter.Add(meshFilter);
                            meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Hairs/" + itemInfo.meshPath);
                            meshRenderer.material = Resources.Load<Material>("Customize/Hairs/" + itemInfo.matPath);

                            meshRenderer.material.SetFloat("_ToneP", toneVelue);

                            Color temp_color;
                            if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Hair_C, out temp_color))
                            {
                                meshRenderer.material.SetColor("_BaseColor", temp_color);
                            }

                            boneTrs_temp[i].gameObject.SetActive(true);
                            break;
                        }

                        for (int j = 0; j < itemSlotInfo.list_partId.Count; j++)
                        {
                            if (itemSlotInfo.list_partId[j][1] == '1')
                            {
                                CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]);

                                MeshRenderer meshRenderer = boneTrs_temp[i].GetComponent<MeshRenderer>();
                                MeshFilter meshFilter = boneTrs_temp[i].GetComponent<MeshFilter>();

                                meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Hairs/" + itemInfo.meshPath);
                                meshRenderer.material = Resources.Load<Material>("Customize/Hairs/" + itemInfo.matPath);
                                meshRenderer.material.SetFloat("_ToneP", toneVelue);

                                Color temp_color;
                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Hair_C, out temp_color))
                                {
                                    meshRenderer.material.SetColor("_BaseColor", temp_color);
                                }

                                bool isActive = true;

                                if (itemSlotInfo2.list_partId[0] != "40000" && (itemSlotInfo2.list_partId[0][1] == '0' || itemSlotInfo2.list_partId[0][1] == '1'))
                                {
                                    isActive = false;
                                }

                                if (isActive)
                                {
                                    list_meshRenderer.Add(meshRenderer);
                                    list_meshFilter.Add(meshFilter);
                                }

                                boneTrs_temp[i].gameObject.SetActive(isActive);
                            }
                        }
                    }
                    break;
                case "ITEM_UPPER":
                    {
                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Wear_I);

                        boneTrs_temp[i].gameObject.SetActive(false);

                        for (int j = 0; j < itemSlotInfo.list_partId.Count; j++)
                        {
                            if (itemSlotInfo.list_partId[j][1] == '0' || itemSlotInfo.list_partId[j][1] == '6')
                            {
                                CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]);

                                SkinnedMeshRenderer skin = boneTrs_temp[i].GetComponent<SkinnedMeshRenderer>();

                                skin.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                skin.material = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                jsonText = Resources.Load("ItemInfos/WearInfo_" + itemInfo.meshPath) as TextAsset;
                                CustomizeDataInfo.WearInfo wearInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                                skin.localBounds = new Bounds(wearInfo.bounds_center, wearInfo.bounds_extent * 2f);

                                if (!isOptimizeGameObjects)
                                {
                                    skin.rootBone = boneTrs[wearInfo.rootBone];

                                    Transform[] boneTrs_set = new Transform[wearInfo.bones.Length];
                                    for (int k = 0; k < wearInfo.bones.Length; k++)
                                    {
                                        boneTrs_set[k] = boneTrs[wearInfo.bones[k]];
                                    }
                                    skin.bones = boneTrs_set;
                                }

                                skin.material.SetFloat("_ToneP", toneVelue);

                                Color temp_color;
                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Upper_C, out temp_color))
                                {
                                    skin.material.SetColor("_BaseColor", temp_color);
                                }

                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Pattern_C, out temp_color))
                                {
                                    skin.material.SetColor("_PatternColor", temp_color);
                                }

                                skin.material.SetColor("_Color_R", Color.white);
                                skin.material.SetColor("_Color_G", Color.white);
                                skin.material.SetColor("_Color_B", Color.white);

                                if (state == CustomModelViewState.Normal)
                                {
                                    skin.gameObject.SetActive(true);
                                    list_skinnedMesh.Add(skin);
                                    list_boneInfos.Add(wearInfo.bones);
                                }
                                else
                                {
                                    skin.gameObject.SetActive(false);
                                }
                            }
                        }
                    }
                    break;
                case "ITEM_UPPER_CUT":
                    {
                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Wear_I);

                        boneTrs_temp[i].gameObject.SetActive(false);

                        for (int j = 0; j < itemSlotInfo.list_partId.Count; j++)
                        {
                            if (itemSlotInfo.list_partId[j][1] == '1' || itemSlotInfo.list_partId[j][1] == '7')
                            {
                                CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]);

                                SkinnedMeshRenderer skin = boneTrs_temp[i].GetComponent<SkinnedMeshRenderer>();

                                skin.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                skin.material = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                jsonText = Resources.Load("ItemInfos/WearInfo_" + itemInfo.meshPath) as TextAsset;
                                CustomizeDataInfo.WearInfo wearInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                                skin.localBounds = new Bounds(wearInfo.bounds_center, wearInfo.bounds_extent * 2f);

                                if (!isOptimizeGameObjects)
                                {
                                    skin.rootBone = boneTrs[wearInfo.rootBone];

                                    Transform[] boneTrs_set = new Transform[wearInfo.bones.Length];
                                    for (int k = 0; k < wearInfo.bones.Length; k++)
                                    {
                                        boneTrs_set[k] = boneTrs[wearInfo.bones[k]];
                                    }
                                    skin.bones = boneTrs_set;
                                }

                                skin.material.SetFloat("_ToneP", toneVelue);

                                Color temp_color;
                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Upper_C, out temp_color))
                                {
                                    skin.material.SetColor("_BaseColor", temp_color);
                                }

                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Pattern_C, out temp_color))
                                {
                                    skin.material.SetColor("_PatternColor", temp_color);
                                }

                                skin.material.SetColor("_Color_R", Color.white);
                                skin.material.SetColor("_Color_G", Color.white);
                                skin.material.SetColor("_Color_B", Color.white);

                                if (state != CustomModelViewState.Normal)
                                {
                                    skin.gameObject.SetActive(true);
                                    list_skinnedMesh.Add(skin);
                                }
                                else
                                {
                                    skin.gameObject.SetActive(false);
                                }

                            }
                        }
                    }
                    break;
                case "ITEM_LOWER":
                    {
                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Wear_I);

                        boneTrs_temp[i].gameObject.SetActive(false);

                        for (int j = 0; j < itemSlotInfo.list_partId.Count; j++)
                        {
                            if (itemSlotInfo.list_partId[j][1] == '2')
                            {
                                CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]);

                                SkinnedMeshRenderer skin = boneTrs_temp[i].GetComponent<SkinnedMeshRenderer>();

                                skin.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                skin.material = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                jsonText = Resources.Load("ItemInfos/WearInfo_" + itemInfo.meshPath) as TextAsset;
                                CustomizeDataInfo.WearInfo wearInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                                skin.localBounds = new Bounds(wearInfo.bounds_center, wearInfo.bounds_extent * 2f);

                                if (!isOptimizeGameObjects)
                                {
                                    skin.rootBone = boneTrs[wearInfo.rootBone];

                                    Transform[] boneTrs_set = new Transform[wearInfo.bones.Length];
                                    for (int k = 0; k < wearInfo.bones.Length; k++)
                                    {
                                        boneTrs_set[k] = boneTrs[wearInfo.bones[k]];
                                    }
                                    skin.bones = boneTrs_set;
                                }

                                skin.material.SetFloat("_ToneP", toneVelue);

                                Color temp_color;
                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Lower_C, out temp_color))
                                {
                                    skin.material.SetColor("_BaseColor", temp_color);
                                }

                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Pattern_C, out temp_color))
                                {
                                    skin.material.SetColor("_PatternColor", temp_color);
                                }

                                skin.material.SetColor("_Color_R", Color.white);
                                skin.material.SetColor("_Color_G", Color.white);
                                skin.material.SetColor("_Color_B", Color.white);

                                skin.gameObject.SetActive(true);
                                list_skinnedMesh.Add(skin);
                                list_boneInfos.Add(wearInfo.bones);
                            }
                        }
                    }
                    break;
                case "ITEM_LOWER_SUB":
                    {
                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Wear_I);

                        boneTrs_temp[i].gameObject.SetActive(false);

                        for (int j = 0; j < itemSlotInfo.list_partId.Count; j++)
                        {
                            if (itemSlotInfo.list_partId[j][1] == '3')
                            {
                                CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]);

                                SkinnedMeshRenderer skin = boneTrs_temp[i].GetComponent<SkinnedMeshRenderer>();

                                skin.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                skin.material = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                jsonText = Resources.Load("ItemInfos/WearInfo_" + itemInfo.meshPath) as TextAsset;
                                CustomizeDataInfo.WearInfo wearInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                                skin.localBounds = new Bounds(wearInfo.bounds_center, wearInfo.bounds_extent * 2f);

                                if (!isOptimizeGameObjects)
                                {
                                    skin.rootBone = boneTrs[wearInfo.rootBone];

                                    Transform[] boneTrs_set = new Transform[wearInfo.bones.Length];
                                    for (int k = 0; k < wearInfo.bones.Length; k++)
                                    {
                                        boneTrs_set[k] = boneTrs[wearInfo.bones[k]];
                                    }
                                    skin.bones = boneTrs_set;
                                }

                                skin.material.SetFloat("_ToneP", toneVelue);

                                Color temp_color;
                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Lower_C, out temp_color))
                                {
                                    skin.material.SetColor("_BaseColor", temp_color);
                                }

                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Pattern_C, out temp_color))
                                {
                                    skin.material.SetColor("_PatternColor", temp_color);
                                }

                                skin.material.SetColor("_Color_R", Color.white);
                                skin.material.SetColor("_Color_G", Color.white);
                                skin.material.SetColor("_Color_B", Color.white);

                                skin.gameObject.SetActive(true);
                                list_skinnedMesh.Add(skin);
                                list_boneInfos.Add(wearInfo.bones);
                            }
                        }
                    }
                    break;
                case "ITEM_FOOT":
                    {
                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Wear_I);

                        boneTrs_temp[i].gameObject.SetActive(false);

                        for (int j = 0; j < itemSlotInfo.list_partId.Count; j++)
                        {
                            if (itemSlotInfo.list_partId[j][1] == '4')
                            {
                                CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]);
                                SkinnedMeshRenderer skin = boneTrs_temp[i].GetComponent<SkinnedMeshRenderer>();

                                skin.sharedMesh = Resources.Load<Mesh>("Customize/Wears/" + itemInfo.meshPath);
                                skin.material = Resources.Load<Material>("Customize/Wears/" + itemInfo.matPath);

                                jsonText = Resources.Load("ItemInfos/WearInfo_" + itemInfo.meshPath) as TextAsset;
                                CustomizeDataInfo.WearInfo wearInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

                                skin.localBounds = new Bounds(wearInfo.bounds_center, wearInfo.bounds_extent * 2f);

                                if (!isOptimizeGameObjects)
                                {
                                    skin.rootBone = boneTrs[wearInfo.rootBone];

                                    Transform[] boneTrs_set = new Transform[wearInfo.bones.Length];
                                    for (int k = 0; k < wearInfo.bones.Length; k++)
                                    {
                                        boneTrs_set[k] = boneTrs[wearInfo.bones[k]];
                                    }
                                    skin.bones = boneTrs_set;
                                }

                                skin.material.SetFloat("_ToneP", toneVelue);

                                Color temp_color;
                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Foot_C, out temp_color))
                                {
                                    skin.material.SetColor("_BaseColor", temp_color);
                                }

                                if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Pattern_C, out temp_color))
                                {
                                    skin.material.SetColor("_PatternColor", temp_color);
                                }

                                skin.material.SetColor("_Color_R", Color.white);
                                skin.material.SetColor("_Color_G", Color.white);
                                skin.material.SetColor("_Color_B", Color.white);

                                skin.gameObject.SetActive(true);
                                list_skinnedMesh.Add(skin);
                                list_boneInfos.Add(wearInfo.bones);
                            }
                        }
                    }
                    break;
                case "ITEM_HEAD_ACC":
                    {
                        CustomizeDataInfo.ItemSlotInfo itemSlotInfo = CustomizeDataInfo.GetInstance.GetItemSlotInfo(genderNum, modelData.ID_Acc_I);
                        boneTrs_temp[i].gameObject.SetActive(false);

                        if (modelData.ID_Acc_I == "4000")
                        {
                            break;
                        }

                        for (int j = 0; j < itemSlotInfo.list_partId.Count; j++)
                        {
                            CustomizeDataInfo.ItemInfo itemInfo = CustomizeDataInfo.GetInstance.GetItemInfo(itemSlotInfo.list_partId[j]);

                            MeshRenderer meshRenderer = boneTrs_temp[i].GetComponent<MeshRenderer>();
                            MeshFilter meshFilter = boneTrs_temp[i].GetComponent<MeshFilter>();
                            meshFilter.sharedMesh = Resources.Load<Mesh>("Customize/Accs/" + itemInfo.meshPath);
                            meshRenderer.material = Resources.Load<Material>("Customize/Accs/" + itemInfo.matPath);

                            meshRenderer.material.SetFloat("_ToneP", toneVelue);

                            Color temp_color;
                            if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Acc_C, out temp_color))
                            {
                                meshRenderer.material.SetColor("_BaseColor", temp_color);
                            }


                            if (genderNum == 0)
                            {
                                jsonText = Resources.Load("ItemInfos/AccInfo_M_" + itemSlotInfo.list_partId[j]) as TextAsset;
                            }
                            else
                            {
                                jsonText = Resources.Load("ItemInfos/AccInfo_F_" + itemSlotInfo.list_partId[j]) as TextAsset;
                            }

                            if (jsonText != null)
                            {
                                accInfo = JsonUtility.FromJson<CustomizeDataInfo.AccInfo>(jsonText.text);
                            }

                            accTr = boneTrs_temp[i];

                            boneTrs_temp[i].gameObject.SetActive(true);
                            list_meshRenderer.Add(meshRenderer);
                            list_meshFilter.Add(meshFilter);
                        }
                    }
                    break;
            }
        }

        if (headTr != null && accTr != null)
        {
            accTr.parent = headTr;
            accTr.localRotation = Quaternion.identity;
            if (accInfo != null)
            {
                accTr.localPosition = accInfo.pos;
                accTr.localScale = accInfo.scale;
            }
            else
            {
                if (genderNum == 0)
                {
                    accTr.localPosition = Vector3.zero;
                    accTr.localScale = Vector3.one * 1f;
                }
                else
                {
                    accTr.localPosition = Vector3.zero;
                    accTr.localScale = Vector3.one * 0.925f;
                }
            }
        }

        if (headTr != null && isCustomF)
        {
            headTr.localPosition = Vector3.right * 0.05f;
        }


        if (renderer_hand != null)
        {
            if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Skin_C, out Color temp_color))
            {
                renderer_hand.sharedMaterial.SetColor("_BaseColor", temp_color);
            }
        }

        if (anim != null)
        {
            anim.Rebind();
        }


        if (state_optimize == OptimizeQualityState.None)
        {
            End();

            return;
        }

        Invoke("SetOptimizeDatas", 1f);
    }

    private void End()
    {
        list_keepRenderData = new List<KeepRenderData>();
        mat_ghost = new Material(Shader.Find("Shader Graphs/RimShader"));
        mat_ghost.SetFloat("_Power", 2f);
        if (meshRenderer_face != null)
        {
            list_keepRenderData.Add(new KeepRenderData(meshRenderer_face, meshRenderer_face.material));
        }

        if (state_optimize == OptimizeQualityState.None || state_optimize == OptimizeQualityState.High)
        {
            for (int i = 0; i < list_meshRenderer.Count; i++)
            {
                list_keepRenderData.Add(new KeepRenderData(list_meshRenderer[i], list_meshRenderer[i].material));
            }

            for (int i = 0; i < list_skinnedMesh.Count; i++)
            {
                list_keepRenderData.Add(new KeepRenderData(list_skinnedMesh[i], list_skinnedMesh[i].material));
            }
        }
        else
        {
            for (int i = 0; i < list_meshRenderer.Count; i++)
            {
                list_keepRenderData.Add(new KeepRenderData(list_meshRenderer[i], list_meshRenderer[i].material));
            }

            list_keepRenderData.Add(new KeepRenderData(skin_body_origin, skin_body_origin.material));
        }

        SetRenderQueue(render_queue);

        list_meshRenderer.Clear();
        list_skinnedMesh.Clear();
        this.enabled = false;
    }

    public void SetRenderQueue(int _render_queue)
    {
        render_queue = _render_queue;

        if (list_keepRenderData == null)
        {
            return;
        }

        for (int i = 0; i < list_keepRenderData.Count; i++)
        {
            list_keepRenderData[i].material.renderQueue = _render_queue;
        }
    }

    public void SetGhost(bool isGhost)
    {
        if (list_keepRenderData == null)
        {
            return;
        }

        if (isGhost)
        {
            for (int i = 0; i < list_keepRenderData.Count; i++)
            {
                list_keepRenderData[i].renderer.sharedMaterial = mat_ghost;
            }
        }
        else
        {
            for (int i = 0; i < list_keepRenderData.Count; i++)
            {
                list_keepRenderData[i].renderer.sharedMaterial = list_keepRenderData[i].material;
            }
        }
    }

    public RenderState GetRenderState()
    {
        return state_render;
    }

    public void SetRenderState(RenderState renderState)
    {
        if (list_keepRenderData == null)
        {
            return;
        }

        if (renderState == state_render)
        {
            return;
        }

        state_render = renderState;

        switch (state_render)
        {
            case RenderState.Enable:
                {
                    for (int i = 0; i < list_keepRenderData.Count; i++)
                    {
                        list_keepRenderData[i].renderer.sharedMaterial = list_keepRenderData[i].material;
                        list_keepRenderData[i].renderer.enabled = true;
                    }
                }
                break;
            case RenderState.Ghost:
                {
                    for (int i = 0; i < list_keepRenderData.Count; i++)
                    {
                        list_keepRenderData[i].renderer.sharedMaterial = mat_ghost;
                        list_keepRenderData[i].renderer.enabled = true;
                    }
                }
                break;
            case RenderState.Disable:
                {
                    for (int i = 0; i < list_keepRenderData.Count; i++)
                    {
                        list_keepRenderData[i].renderer.enabled = false;
                    }
                }
                break;
        }
    }

    private void SetOptimizeDatas() // 개별 텍스처로 합성.
    {
        for (int i = 0; i < list_meshRenderer.Count; i++)
        {
            renderTextures.Add(Appnori.Util.BakeTextureManager.SetBakeTexture(list_meshRenderer[i]));
        }

        for (int i = 0; i < list_skinnedMesh.Count; i++)
        {
            renderTextures.Add(Appnori.Util.BakeTextureManager.SetBakeTexture(list_skinnedMesh[i]));
        }

        if (state_optimize == OptimizeQualityState.High)
        {
            End();
            return;
        }

        SetCombine();
    }

    private void SetCombine() // Normal부터.
    {
        List<Rect> rects_tex = new List<Rect>();
        List<Texture> list_base = new List<Texture>();
        List<Texture> list_normal = new List<Texture>();
        int count_uv = 0;
        int index_body = 0;
        for (int i = 0; i < list_skinnedMesh.Count; i++)
        {
            if (list_skinnedMesh[i].name.Contains("BODY_ORG"))
            {
                index_body = i;
                rects_tex.Add(new Rect(0f, 0f, 0f, 0f));
                continue;
            }

            list_base.Add(list_skinnedMesh[i].material.GetTexture("_BaseMap"));
            list_normal.Add(list_skinnedMesh[i].material.GetTexture("_BumpMap"));
            switch (count_uv)
            {
                case 0:
                    {
                        rects_tex.Add(new Rect(0, 0.5f, 0.5f, 0.5f));
                    }
                    break;
                case 1:
                    {
                        rects_tex.Add(new Rect(0.5f, 0.5f, 0.5f, 0.5f));
                    }
                    break;
                case 2:
                    {
                        rects_tex.Add(new Rect(0, 0, 0.5f, 0.5f));
                    }
                    break;
                case 3:
                    {
                        rects_tex.Add(new Rect(0.5f, 0, 0.5f, 0.5f));
                    }
                    break;
            }
            count_uv++;
        }

        SkinnedMeshRenderer combineSkin = skin_body_origin;
        combineSkin.transform.parent = transform;
        combineSkin.transform.localPosition = Vector3.zero;
        combineSkin.transform.localRotation = Quaternion.identity;
        combineSkin.transform.localScale = Vector3.one;

        Material material;
        material = new Material(Shader.Find("Shader Graphs/CharShader_Opt"));

        RenderTexture baseMap;

        if (ColorUtility.TryParseHtmlString("#" + modelData.Hex_Skin_C, out Color temp_color))
        {
            baseMap = Appnori.Util.BakeTextureManager.GetMainAtlasTexture(list_base.ToArray(), temp_color);
        }
        else
        {
            baseMap = Appnori.Util.BakeTextureManager.GetMainAtlasTexture(list_base.ToArray(), new Color(1f, 0.55f, 0.45f));
        }

        renderTextures.Add(baseMap);

        for (int i = 0; i < list_base.Count; i++)
        {
            RenderTexture.ReleaseTemporary((RenderTexture)list_base[i]);
        }

        RenderTexture normalMap = Appnori.Util.BakeTextureManager.GetOtherAtlasTexture(list_normal.ToArray());
        renderTextures.Add(normalMap);
        material.SetTexture("_BaseMap", baseMap);
        material.SetTexture("_BumpMap", normalMap);
        material.SetFloat("_ToneP", toneVelue);

        combineSkin.sharedMaterial = material;

        Dictionary<int, int> vertexIndices = new Dictionary<int, int>();
        List<int> tris = new List<int>();

        List<Vector3> verts = new List<Vector3>();
        //List<Color> colors = new List<Color>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector4> tangents = new List<Vector4>();

        //List<Vector2> uvs, uvs2, uvs3, uvs4;
        List<Vector2> uvs = new List<Vector2>();
        //uvs2 = new List<Vector2>(); uvs3 = new List<Vector2>(); uvs4 = new List<Vector2>();

        List<BoneWeight> boneWeights = new List<BoneWeight>();

        Mesh mesh_combine = new Mesh();
        TextAsset jsonText;

        if (modelData.Gender.ToUpper() == "M")
        {
            mesh_combine = Instantiate(Resources.Load<Mesh>("Customize/Bodies/Mesh_M_BODY_ORG"));
            jsonText = Resources.Load("ItemInfos/BodyInfo_M_Origin") as TextAsset;
        }
        else
        {
            mesh_combine = Instantiate(Resources.Load<Mesh>("Customize/Bodies/Mesh_F_BODY_ORG"));
            jsonText = Resources.Load("ItemInfos/BodyInfo_F_Origin") as TextAsset;
        }

        CustomizeDataInfo.WearInfo bodyInfo = JsonUtility.FromJson<CustomizeDataInfo.WearInfo>(jsonText.text);

        combineSkin.localBounds = new Bounds(bodyInfo.bounds_center, bodyInfo.bounds_extent * 2f);


        Transform[] boneTrs_set = new Transform[bodyInfo.bones.Length];
        for (int i = 0; i < bodyInfo.bones.Length; i++)
        {
            if (!isOptimizeGameObjects)
            {
                boneTrs_set[i] = boneTrs[bodyInfo.bones[i]];
            }
        }

        if (!isOptimizeGameObjects)
        {
            combineSkin.rootBone = boneTrs[bodyInfo.rootBone];
            combineSkin.bones = boneTrs_set;
        }

        List<Dictionary<int, int>> list_convert_boneIndex = new List<Dictionary<int, int>>();

        for (int i = 0; i < list_boneInfos.Count; i++)
        {
            Dictionary<int, int> convert_boneIndex = new Dictionary<int, int>();
            for (int j = 0; j < list_boneInfos[i].Length; j++)
            {
                for (int k = 0; k < bodyInfo.bones.Length; k++)
                {
                    if (bodyInfo.bones[k] == list_boneInfos[i][j])
                    {
                        convert_boneIndex.Add(j, k);

                        break;
                    }
                }
            }
            list_convert_boneIndex.Add(convert_boneIndex);
        }

        int startIndex = 0;
        int curSubVertIndex = 0;

        Vector4 tangent_body = new Vector4(0, 0, 0, -1);

        if (state_optimize == OptimizeQualityState.Low)
        {
            for (int i = 0; i < list_meshFilter.Count; i++)
            {
                Mesh mesh = Appnori.Util.LowMeshCreator.GetWeldMesh(list_meshFilter[i].sharedMesh, true, 0.002f, 0.0005f);
                list_meshFilter[i].sharedMesh = mesh;
            }
        }
        for (int rect_i = 0; rect_i < list_skinnedMesh.Count; rect_i++)
        {
            Mesh mesh = list_skinnedMesh[rect_i].sharedMesh;
            if (state_optimize == OptimizeQualityState.Low)
            {
                mesh = Appnori.Util.LowMeshCreator.GetWeldMesh(list_skinnedMesh[rect_i].sharedMesh);
                list_skinnedMesh[rect_i].sharedMesh = mesh;
            }

            Dictionary<int, int> convert_boneIndex = list_convert_boneIndex[rect_i];

            Vector2[] arr_uv = mesh.uv;
            BoneWeight[] arr_boneWeight = mesh.boneWeights;

            Rect rect = rects_tex[rect_i];
            int[] triangles = mesh.GetTriangles(0);

            int triangle;
            int curVertIndex;
            int newVertIndex;

            for (int i = 0; i < triangles.Length; i++)
            {
                triangle = triangles[i];
                curVertIndex = triangle + startIndex;

                if (!vertexIndices.TryGetValue(curVertIndex, out int getIndex))
                {
                    newVertIndex = curSubVertIndex;
                    curSubVertIndex++;

                    vertexIndices.Add(curVertIndex, newVertIndex);

                    uvs.Add(new Vector2(arr_uv[triangle].x * rect.width, arr_uv[triangle].y * rect.height) + rect.position);

                    arr_boneWeight[triangle].boneIndex0 = convert_boneIndex[arr_boneWeight[triangle].boneIndex0];
                    arr_boneWeight[triangle].boneIndex1 = convert_boneIndex[arr_boneWeight[triangle].boneIndex1];
                    arr_boneWeight[triangle].boneIndex2 = convert_boneIndex[arr_boneWeight[triangle].boneIndex2];
                    arr_boneWeight[triangle].boneIndex3 = convert_boneIndex[arr_boneWeight[triangle].boneIndex3];
                }
                else
                {
                    newVertIndex = getIndex;
                }

                tris.Add(newVertIndex);
            }

            verts.AddRange(mesh.vertices);
            normals.AddRange(mesh.normals);

            if (rect_i == index_body)
            {
                tangents.AddRange(System.Linq.Enumerable.Repeat(tangent_body, mesh.tangents.Length));
            }
            else
            {
                tangents.AddRange(mesh.tangents);
            }

            boneWeights.AddRange(arr_boneWeight);

            startIndex += (int)list_skinnedMesh[rect_i].sharedMesh.GetIndexCount(0);
            list_skinnedMesh[rect_i].gameObject.SetActive(false);
        }

        mesh_combine.triangles = null;

        mesh_combine.vertices = verts.ToArray();
        mesh_combine.normals = normals.ToArray();
        mesh_combine.tangents = tangents.ToArray();

        mesh_combine.triangles = tris.ToArray();
        mesh_combine.uv = uvs.ToArray();

        mesh_combine.boneWeights = boneWeights.ToArray();

        combineSkin.sharedMesh = mesh_combine;
        combineSkin.gameObject.SetActive(true);

        End();
    }

    private void OnDestroy()
    {
        for (int i = renderTextures.Count - 1; i >= 0; i--)
        {
            if (renderTextures[i] != null)
            {
                RenderTexture.ReleaseTemporary(renderTextures[i]);
            }
        }
    }

}
