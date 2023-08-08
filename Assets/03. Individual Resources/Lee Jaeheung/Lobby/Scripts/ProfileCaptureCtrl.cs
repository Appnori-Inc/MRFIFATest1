using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class ProfileCaptureCtrl : MonoBehaviour
{
    public enum ShotState
    {
        Multi, Profile
    }

    private ShotState shotState;
    private int index_win;

    public CustomModelSettingCtrl[] customModels;
    public Transform[] customModels_center;
    public Transform[] customModels_hand;
    public Animator[] anims_customModel;
    private Camera cam;

    public Light[] lights;

    private GameObject parentGO;

    private RenderTexture[] renderTextures_single = new RenderTexture[5];
    private RenderTexture[] renderTextures_multi = new RenderTexture[2];
    private RenderTexture[] renderTextures_profile = new RenderTexture[2];

    private SpriteRenderer sprite_backGround;
    public Sprite[] sprites_backGround;

    public Transform[] propTrs;

    public MeshRenderer renderer_cam_view;

    private void Awake()
    {
        renderTextures_multi = new RenderTexture[2];
        renderTextures_profile = new RenderTexture[2];

        for (int i = 0; i < renderTextures_single.Length; i++)
        {
            renderTextures_single[i] = Resources.Load<RenderTexture>("UI/RenderTexture_Single0" + (i + 1));
        }

        for (int i = 0; i < renderTextures_multi.Length; i++)
        {
            renderTextures_multi[i] = Resources.Load<RenderTexture>("UI/RenderTexture_Multi0" + (i + 1));
        }

        for (int i = 0; i < renderTextures_profile.Length; i++)
        {
            renderTextures_profile[i] = Resources.Load<RenderTexture>("UI/RenderTexture_Profile0" + (i + 1));
        }

        cam = transform.GetComponentInChildren<Camera>();

        int layerIndex = 31;

        cam.cullingMask = 1 << layerIndex;

        Transform[] transforms = transform.GetComponentsInChildren<Transform>();

        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].name == "Post-process Volume")
            {
                continue;
            }
            transforms[i].gameObject.layer = layerIndex;
        }

        sprite_backGround = cam.transform.Find("BackGround").GetComponent<SpriteRenderer>();

        parentGO = transform.GetChild(0).gameObject;
        parentGO.SetActive(false);
    }

    public void ShotSingleImages()
    {
        sprite_backGround.enabled = false;

        Light[] lights_temp = Light.GetLights(LightType.Directional, 0);
        lights[0].gameObject.SetActive(true);
        lights[1].gameObject.SetActive(true);
        for (int i = 0; i < lights_temp.Length; i++)
        {
            if (lights_temp[i] == lights[0])
            {
                lights_temp[i] = null;
            }
            else if (lights_temp[i].enabled)
            {
                lights_temp[i].enabled = false;
            }
            else
            {
                lights_temp[i] = null;
            }
        }

        parentGO.SetActive(true);
        renderer_cam_view.enabled = false;
        cam.fieldOfView = 50f;

        RenderTexture currentRT = RenderTexture.active;
        GameDataManager.instance.customModelDatas_single = new CustomModelData[renderTextures_single.Length];
        customModels[0].gameObject.SetActive(true);
        customModels[1].gameObject.SetActive(false);
        customModels[0].transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.Euler(0f, 15f, 0f);
        for (int i = 0; i < propTrs.Length; i++)
        {
            propTrs[i].gameObject.SetActive(false);
        }

        Animator[] anims_prop = null;

        for (int i = 0; i < GameDataManager.instance.customModelDatas_single.Length; i++)
        {
            customModels[0].enabled = true;
            GameDataManager.instance.customModelDatas_single[i] = CustomModelSettingCtrl.GetRandomModelData();
            customModels[0].Init(GameDataManager.instance.customModelDatas_single[i], CustomModelViewState.Normal, null, 0.3f);

            customModels[0].gameObject.SetActive(false);
            customModels[0].gameObject.SetActive(true);

            string stateName = "";

            switch (GameDataManager.instance.gameType)
            {
                case GameDataManager.GameType.Bowling:
                    {
                        if (GameDataManager.instance.customModelDatas_single[i].Gender == "m")
                        {
                            stateName = "Bowling";
                        }
                        else
                        {
                            stateName = "Bowling_F";
                        }
                        propTrs[0].gameObject.SetActive(true);
                        anims_prop = new Animator[1];
                        anims_prop[0] = propTrs[0].GetComponent<Animator>();
                    }
                    break;
                case GameDataManager.GameType.Archery:
                    {
                        if (GameDataManager.instance.customModelDatas_single[i].Gender == "m")
                        {
                            stateName = "Archery";
                        }
                        else
                        {
                            stateName = "Archery_F";
                        }
                        propTrs[1].gameObject.SetActive(true);
                        anims_prop = new Animator[1];
                        anims_prop[0] = propTrs[1].GetComponent<Animator>();
                    }
                    break;
                case GameDataManager.GameType.Basketball:
                    {
                        if (GameDataManager.instance.customModelDatas_single[i].Gender == "m")
                        {
                            stateName = "Basketball";
                        }
                        else
                        {
                            stateName = "Basketball_F";
                        }
                        propTrs[2].gameObject.SetActive(true);
                        anims_prop = new Animator[1];
                        anims_prop[0] = propTrs[2].GetComponent<Animator>();
                    }
                    break;
                case GameDataManager.GameType.Badminton:
                    {
                        stateName = "Badminton";
                        propTrs[3].gameObject.SetActive(true);
                        propTrs[4].gameObject.SetActive(true);
                        anims_prop = new Animator[2];
                        anims_prop[0] = propTrs[3].GetComponent<Animator>();
                        anims_prop[1] = propTrs[4].GetComponent<Animator>();
                    }
                    break;
                case GameDataManager.GameType.Billiards:
                    {
                        if (GameDataManager.instance.customModelDatas_single[i].Gender == "m")
                        {
                            stateName = "Billiard";
                        }
                        else
                        {
                            stateName = "Billiard_F";
                        }
                        propTrs[5].gameObject.SetActive(true);
                        anims_prop = new Animator[1];
                        anims_prop[0] = propTrs[5].GetComponent<Animator>();
                    }
                    break;
                case GameDataManager.GameType.Darts:
                    {
                        if (GameDataManager.instance.customModelDatas_single[i].Gender == "m")
                        {
                            stateName = "Darts";
                        }
                        else
                        {
                            stateName = "Darts_F";
                        }
                        propTrs[6].gameObject.SetActive(true);
                        anims_prop = new Animator[1];
                        anims_prop[0] = propTrs[6].GetComponent<Animator>();
                    }
                    break;
                case GameDataManager.GameType.TableTennis:
                    {
                        stateName = "TableTennis";
                        propTrs[7].gameObject.SetActive(true);
                    }
                    break;
                case GameDataManager.GameType.Boxing:
                    {
                        stateName = "Boxing";
                        propTrs[8].gameObject.SetActive(true);
                    }
                    break;
                case GameDataManager.GameType.Golf:
                    {
                        if (GameDataManager.instance.customModelDatas_single[i].Gender == "m")
                        {
                            stateName = "Golf";
                        }
                        else
                        {
                            stateName = "Golf_F";
                        }
                        propTrs[9].gameObject.SetActive(true);
                        anims_prop = new Animator[1];
                        anims_prop[0] = propTrs[9].GetComponent<Animator>();
                    }
                    break;
                case GameDataManager.GameType.Baseball:
                    {
                        stateName = "Baseball";
                        propTrs[10].gameObject.SetActive(true);
                        anims_prop = new Animator[1];
                        anims_prop[0] = propTrs[10].GetComponent<Animator>();
                    }
                    break;
                case GameDataManager.GameType.Tennis:
                    {
                        stateName = "Tennis";
                        propTrs[11].gameObject.SetActive(true);
                        propTrs[12].gameObject.SetActive(true);
                        anims_prop = new Animator[1];
                        anims_prop[0] = propTrs[12].GetComponent<Animator>();
                    }
                    break;
            }

            float normalTime = 0f;

            switch (i)
            {
                case 1:
                    {
                        normalTime = 0.25f;
                    }
                    break;
                case 2:
                    {
                        normalTime = 0.5f;
                    }
                    break;
                case 3:
                    {
                        normalTime = 0.75f;
                    }
                    break;
                case 4:
                    {
                        normalTime = 1f;
                    }
                    break;
            }


            anims_customModel[0].Play(stateName, -1, normalTime);
            anims_customModel[0].Update(1f);
            if (anims_prop != null)
            {
                for (int j = 0; j < anims_prop.Length; j++)
                {
                    anims_prop[j].Play(stateName, -1, normalTime);
                    anims_prop[j].Update(1f);
                }
            }

            Vector3 calcul_pos = customModels_hand[0].position;
            calcul_pos += customModels_hand[1].position;
            calcul_pos *= 0.5f;

            cam.transform.position = customModels_center[0].position + new Vector3(-0.45f, 0.1f, -1.8f);
            calcul_pos = Vector3.Lerp(customModels_center[0].position, calcul_pos, 0.3f);
            calcul_pos.y = customModels_center[0].position.y;
            cam.transform.position = calcul_pos + new Vector3(-0.45f, 0.1f, -1.8f);

            cam.targetTexture = renderTextures_single[i];

            RenderTexture.active = renderTextures_single[i];

            cam.Render();
        }

        cam.targetTexture = null;

        RenderTexture.active = currentRT;
        renderer_cam_view.enabled = true;
        parentGO.SetActive(false);

        for (int i = 0; i < lights_temp.Length; i++)
        {
            if (lights_temp[i] != null)
            {
                lights_temp[i].enabled = true;
            }
        }
    }

    public void ShotImages(string[] user_id, ShotState _shotState)
    {
        if (shotImagesCoroutine != null)
        {
            StopCoroutine(shotImagesCoroutine);
        }

        shotState = _shotState;

        shotImagesCoroutine = StartCoroutine(ShotImagesCoroutine(user_id));
    }

    Coroutine shotImagesCoroutine;

    IEnumerator ShotImagesCoroutine(string[] user_id)
    {
        sprite_backGround.enabled = true;

        bool[] isOk = new bool[user_id.Length];
        cam.fieldOfView = 20f;

        for (int i = 0; i < user_id.Length; i++)
        {
            if (user_id[i] == "AI")
            {
                GameDataManager.UserInfo userInfo = GameDataManager.instance.userInfos[i];
                //userInfo.customModelData = GameDataManager.instance.GetCustomModelData(GameDataManager.level);
                userInfo.customModelData = GameDataManager.instance.customModelDatas_single[GameDataManager.level - 1];
                GameDataManager.instance.userInfos[i] = userInfo;
            }

            isOk[i] = false;
        }

        while (true)
        {
            for (int i = 0; i < isOk.Length; i++)
            {
                if (!isOk[i])
                {
                    if (GameDataManager.instance.userInfos[i].customModelData != null)
                    {
                        SetBackGroundColor(GameDataManager.instance.userInfos[i].customModelData.Hex_Hair_C);

                        Light[] lights_temp = Light.GetLights(LightType.Directional, 31);

                        for (int j = 0; j < lights_temp.Length; j++)
                        {
                            if (lights_temp[j] == lights[0])
                            {
                                lights_temp[j] = null;
                            }
                            else if (lights_temp[j].enabled)
                            {
                                lights_temp[j].enabled = false;
                            }
                            else
                            {
                                lights_temp[j] = null;
                            }
                        }

                        parentGO.SetActive(true);

                        if (i == 0)
                        {
                            cam.transform.localPosition = new Vector3(-0.32f, 1.37f, -2.5f);
                            cam.transform.localRotation = Quaternion.Euler(0f, 7.5f, 0f);
                        }
                        else
                        {
                            cam.transform.localPosition = new Vector3(0.32f, 1.37f, -2.5f);
                            cam.transform.localRotation = Quaternion.Euler(0f, -7.5f, 0f);
                        }

                        RenderTexture currentRT = RenderTexture.active;

                        customModels[0].enabled = true;
                        customModels[0].Init(GameDataManager.instance.userInfos[i].customModelData);
                        customModels[0].gameObject.SetActive(false);
                        customModels[0].gameObject.SetActive(true);
                        customModels[0].GetComponent<Animator>().Update(1f);

                        if (shotState == ShotState.Multi)
                        {
                            cam.targetTexture = renderTextures_multi[i];
                            RenderTexture.active = renderTextures_multi[i];
                        }
                        else
                        {
                            cam.targetTexture = renderTextures_profile[i];
                            RenderTexture.active = renderTextures_profile[i];
                        }

                        cam.Render();
                        RenderTexture.active = currentRT;

                        cam.targetTexture = null;
                        isOk[i] = true;

                        parentGO.SetActive(false);

                        for (int j = 0; j < lights_temp.Length; j++)
                        {
                            if (lights_temp[j] != null)
                            {
                                lights_temp[j].enabled = true;
                            }
                        }
                    }
                }
            }


            if (!isOk.Contains(false))
            {
                yield break;
            }
            yield return null;
        }
    }


    public void PlayImages(string[] user_id, ShotState _shotState, int _index_win = 0)
    {
        if (shotImagesCoroutine != null)
        {
            StopCoroutine(shotImagesCoroutine);
        }

        shotState = _shotState;
        index_win = _index_win;

        shotImagesCoroutine = StartCoroutine(PlayImagesCoroutine(user_id));
    }

    public void StopImages()
    {
        if (shotImagesCoroutine != null)
        {
            StopCoroutine(shotImagesCoroutine);
        }

        parentGO.SetActive(false);
    }

    IEnumerator PlayImagesCoroutine(string[] user_id)
    {
        sprite_backGround.enabled = false;

        bool[] isOk = new bool[user_id.Length];
        Vector3[] poss = new Vector3[user_id.Length];

        cam.fieldOfView = 40f;
        cam.backgroundColor = Color.clear;
        sprite_backGround.transform.localPosition = new Vector3(0f, 0f, 2.8f);

        for (int i = 0; i < propTrs.Length; i++)
        {
            propTrs[i].gameObject.SetActive(false);
        }

        customModels[0].gameObject.SetActive(false);
        customModels[1].gameObject.SetActive(false);

        if (shotState == ShotState.Multi)
        {
            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = renderTextures_multi[0];
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = renderTextures_multi[1];
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rt;
        }
        else
        {
            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = renderTextures_profile[0];
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = renderTextures_profile[1];
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rt;
        }

        for (int i = 0; i < user_id.Length; i++)
        {
            if (user_id[i] == "AI")
            {
                GameDataManager.UserInfo userInfo = GameDataManager.instance.userInfos[i];
                //userInfo.customModelData = GameDataManager.instance.GetCustomModelData(GameDataManager.level);
                userInfo.customModelData = GameDataManager.instance.customModelDatas_single[GameDataManager.level - 1];
                GameDataManager.instance.userInfos[i] = userInfo;
            }

            isOk[i] = false;
        }
        yield return new WaitForSecondsRealtime(0.5f);
        parentGO.SetActive(true);
        sprite_backGround.color = Color.white;
        sprite_backGround.sprite = sprites_backGround[2];
        while (true)
        {
            Light[] lights_temp = Light.GetLights(LightType.Directional, 0);

            for (int j = 0; j < lights_temp.Length; j++)
            {
                if (lights_temp[j] == lights[0])
                {
                    lights_temp[j] = null;
                }
                else if (lights_temp[j].enabled)
                {
                    lights_temp[j].enabled = false;
                }
                else
                {
                    lights_temp[j] = null;
                }
            }
            lights[0].gameObject.SetActive(true);
            lights[1].gameObject.SetActive(true);
            renderer_cam_view.enabled = false;
            RenderTexture currentRT = RenderTexture.active;

            for (int i = 0; i < isOk.Length; i++)
            {
                if (GameDataManager.instance.userInfos[i].customModelData != null)
                {
                    if (!isOk[i])
                    {
                        isOk[i] = true;

                        customModels[i].gameObject.SetActive(true);
                        customModels[i].enabled = true;
                        customModels[i].Init(GameDataManager.instance.userInfos[i].customModelData, CustomModelViewState.Normal, null, 0.3f);

                        string stateName = "";
                        if (shotState == ShotState.Multi)
                        {
                            if (GameDataManager.instance.userInfos[i].customModelData.Gender == "m")
                            {
                                stateName = "Wait_Pose";
                            }
                            else
                            {
                                stateName = "Wait_Pose_F";
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                if (index_win == 0)
                                {
                                    stateName = "Result_Win";
                                }
                                else if (index_win == 1)
                                {
                                    stateName = "Result_Lose";
                                }
                                else
                                {
                                    stateName = "Result_Draw";
                                }
                            }
                            else
                            {
                                if (index_win == 1)
                                {
                                    stateName = "Result_Win";
                                }
                                else if (index_win == 0)
                                {
                                    stateName = "Result_Lose";
                                }
                                else
                                {
                                    stateName = "Result_Draw";
                                }
                            }

                            cam.transform.localRotation = Quaternion.identity;
                        }

                        anims_customModel[i].Play(stateName, -1, 0f);

                        customModels[i].transform.localPosition = Vector3.zero; 

                        poss[i] = customModels_center[i].position + new Vector3(0f, 0.1f, -4f);


                    }

                    customModels[i].transform.localPosition = Vector3.zero;

                    if (shotState == ShotState.Multi)
                    {
                        if (i == 0)
                        {
                            poss[i] = Vector3.Lerp(poss[i], customModels_center[i].position + new Vector3(-0.35f, 0.15f, -1.3f), Time.unscaledDeltaTime * 5f);
                            cam.transform.position = poss[i];
                            cam.transform.localRotation = Quaternion.Euler(0f, 20f, 0f);
                        }
                        else
                        {
                            poss[i] = Vector3.Lerp(poss[i], customModels_center[i].position + new Vector3(0.35f, 0.15f, -1.3f), Time.unscaledDeltaTime * 5f);
                            cam.transform.position = poss[i];
                            cam.transform.localRotation = Quaternion.Euler(0f, -20f, 0f);
                        }

                        cam.targetTexture = renderTextures_multi[i];
                        RenderTexture.active = renderTextures_multi[i];
                        cam.Render();

                    }
                    else
                    {
                        if (i == 0)
                        {
                            poss[i] = Vector3.Lerp(poss[i], customModels_center[i].position + new Vector3(0f, 0.1f, -1.5f), Time.unscaledDeltaTime * 5f);
                            cam.transform.position = poss[i];
                            PublicGameUIManager.GetInstance.SyncProfileLight(anims_customModel[i].GetCurrentAnimatorStateInfo(0).normalizedTime);

                            if (index_win == 0)
                            {
                                sprite_backGround.sprite = sprites_backGround[0];
                            }
                            else if (index_win == 1)
                            {
                                sprite_backGround.sprite = sprites_backGround[1];
                            }
                            else
                            {
                                sprite_backGround.sprite = sprites_backGround[2];
                            }
                        }
                        else
                        {
                            poss[i] = Vector3.Lerp(poss[i], customModels_center[i].position + new Vector3(0f, 0.05f, -1.5f), Time.unscaledDeltaTime * 5f);
                            cam.transform.position = poss[i];

                            if (index_win == 1)
                            {
                                sprite_backGround.sprite = sprites_backGround[0];
                            }
                            else if (index_win == 0)
                            {
                                sprite_backGround.sprite = sprites_backGround[1];
                            }
                            else
                            {
                                sprite_backGround.sprite = sprites_backGround[2];
                            }
                        }
                        sprite_backGround.enabled = true;
                        cam.targetTexture = renderTextures_profile[i];
                        RenderTexture.active = renderTextures_profile[i];
                        cam.Render();
                        sprite_backGround.enabled = false;
                    }

                    customModels[i].transform.localPosition = new Vector3(0f,-100f,0f);
                }
            }

            RenderTexture.active = currentRT;
            cam.targetTexture = null;
            renderer_cam_view.enabled = true;

            lights[0].gameObject.SetActive(false);
            lights[1].gameObject.SetActive(false);
            for (int j = 0; j < lights_temp.Length; j++)
            {
                if (lights_temp[j] != null)
                {
                    lights_temp[j].enabled = true;
                }
            }
            yield return null;
        }
    }

    void SetBackGroundColor(string hexColor_origin)
    {
        if (ColorUtility.TryParseHtmlString("#" + hexColor_origin, out Color color_origin))
        {
            Color.RGBToHSV(color_origin, out float h, out float s, out float v);
            h = (h + 0.5f) % 1f;
            sprite_backGround.color = Color.HSVToRGB(h, s, 1f);
        }
        else
        {
            sprite_backGround.color = Color.white;
        }
    }


    public void ShotImages_Baseball()
    {
        sprite_backGround.enabled = false;

        Light[] lights_temp = Light.GetLights(LightType.Directional, 0);
        lights[0].gameObject.SetActive(true);
        lights[1].gameObject.SetActive(true);
        for (int i = 0; i < lights_temp.Length; i++)
        {
            if (lights_temp[i] == lights[0])
            {
                lights_temp[i] = null;
            }
            else if (lights_temp[i].enabled)
            {
                lights_temp[i].enabled = false;
            }
            else
            {
                lights_temp[i] = null;
            }
        }

        parentGO.SetActive(true);
        cam.fieldOfView = 50f;
        RenderTexture currentRT = RenderTexture.active;
        customModels[0].gameObject.SetActive(true);
        customModels[1].gameObject.SetActive(false);
        customModels[0].transform.localPosition = Vector3.zero;

        cam.transform.localRotation = Quaternion.Euler(0f, 15f, 0f);
        for (int i = 0; i < propTrs.Length; i++)
        {
            propTrs[i].gameObject.SetActive(false);
        }

        Animator[] anims_prop = null;

        List<int> list_anims = new List<int>();


        for (int i = 0; i < 5; i++)
        {
            list_anims.Add(i);
        }

        for (int i = 0; i < 2; i++)
        {
            customModels[0].enabled = true;
            customModels[0].Init(GameDataManager.instance.GetCustomModelData(i), CustomModelViewState.Normal, null, 0.3f);

            customModels[0].gameObject.SetActive(false);
            customModels[0].gameObject.SetActive(true);

            string stateName = "";

            stateName = "Baseball";
            propTrs[10].gameObject.SetActive(true);
            anims_prop = new Animator[1];
            anims_prop[0] = propTrs[10].GetComponent<Animator>();


            int selectNum = list_anims[(int)Random.Range(0f, list_anims.Count - 0.0001f)];
            list_anims.Remove(selectNum);

            float normalTime = 0f;

            switch (selectNum)
            {
                case 1:
                    {
                        normalTime = 0.25f;
                    }
                    break;
                case 2:
                    {
                        normalTime = 0.5f;
                    }
                    break;
                case 3:
                    {
                        normalTime = 0.75f;
                    }
                    break;
                case 4:
                    {
                        normalTime = 1f;
                    }
                    break;
            }


            anims_customModel[0].Play(stateName, -1, normalTime);
            anims_customModel[0].Update(1f);
            if (anims_prop != null)
            {
                for (int j = 0; j < anims_prop.Length; j++)
                {
                    anims_prop[j].Play(stateName, -1, normalTime);
                    anims_prop[j].Update(1f);
                }
            }

            Vector3 calcul_pos = customModels_hand[0].position;
            calcul_pos += customModels_hand[1].position;

            calcul_pos *= 0.5f;

            cam.transform.position = customModels_center[0].position + new Vector3(-0.45f, 0.1f, -1.8f);
            calcul_pos = Vector3.Lerp(customModels_center[0].position, calcul_pos, 0.3f);
            calcul_pos.y = customModels_center[0].position.y;
            cam.transform.position = calcul_pos + new Vector3(-0.45f, 0.1f, -1.8f);

            cam.targetTexture = renderTextures_single[i];

            RenderTexture.active = renderTextures_single[i];

            cam.Render();
        }

        cam.targetTexture = null;

        RenderTexture.active = currentRT;

        parentGO.SetActive(false);

        for (int i = 0; i < lights_temp.Length; i++)
        {
            if (lights_temp[i] != null)
            {
                lights_temp[i].enabled = true;
            }
        }
    }
}
