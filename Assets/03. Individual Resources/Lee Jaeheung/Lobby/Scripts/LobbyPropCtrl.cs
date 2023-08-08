using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PropState
{
    Idle, Grab, Set, Keep
}

[System.Serializable]
public class LobbyPropData
{ 
    public MeshFilter mesh_head;
    public MeshFilter mesh_hair_f;
    public MeshFilter mesh_hair_b;
    public SkinnedMeshRenderer mesh_body;
    public SkinnedMeshRenderer mesh_upper;
    public SkinnedMeshRenderer mesh_lower;
    public SkinnedMeshRenderer mesh_lower_s;
    public SkinnedMeshRenderer mesh_foot;
    public MeshFilter mesh_foot_l;
    public MeshFilter mesh_foot_r;
    public MeshFilter mesh_acc;
}

public class LobbyPropCtrl : MonoBehaviour
{
    private Vector3 pos_orizin;
    private Quaternion rot_orizin;

    private PropState propState;

    public GameDataManager.GameType gameType = GameDataManager.GameType.None;

    [System.Serializable]
    public class CatchTransform
    {
        public Vector3 pos = new Vector3();
        public Vector3 rot = new Vector3();
        public Quaternion rot_q = new Quaternion();
    }

    public Vector3 pos_center = new Vector3();

    public CatchTransform[] catchTransforms = new CatchTransform[2];

    private Animator anim;

    private PropState animState;

    private LobbyPropData lobbyPropData;

    private Transform handTr;
    private int handNum;

    private GameObject shadow;

    private CustomModelSettingCtrl customModelSettingCtrl;

    void Start()
    {
        anim = transform.GetComponent<Animator>();

        shadow = transform.Find("Shadow").gameObject;

        animState = PropState.Idle;
        pos_orizin = transform.position;
        rot_orizin = transform.rotation;

        for (int i = 0; i < catchTransforms.Length; i++)
        {
            catchTransforms[i].rot_q = Quaternion.Euler(catchTransforms[i].rot);
        }

        customModelSettingCtrl = transform.GetComponent<CustomModelSettingCtrl>();

        LobbyPropManager.GetInstance.AddProp(this);

        StartCoroutine(CheckPropData());
    }

    IEnumerator CheckPropData()
    {
        if (gameType == GameDataManager.GameType.Customize)
        {
            while (GameDataManager.instance.state_userInfo != GameDataManager.UserInfoState.Completed)
            {
                yield return null;
            }

            customModelSettingCtrl.Init(GameDataManager.instance.userInfo_mine.customModelData);
        }
        else if (gameType != GameDataManager.GameType.None)
        {
            int genderNum = (int)Random.Range(0f, 1.9999f);

            transform.GetComponent<CustomModelSettingCtrl>().InitRandom(genderNum, CustomModelViewState.Normal);

            anim.SetBool("IsMale", genderNum == 0);
            anim.SetInteger("StateNum", (int)gameType);
            anim.SetTrigger("OnState");
            anim.SetBool("IsPlay", true);
            AnimSyncCtrl animSync = transform.GetComponent<AnimSyncCtrl>();
            if (animSync != null)
            {
                animSync.Init(genderNum == 0);
            }
        }

        yield return new WaitForSeconds(0.1f);

        while (customModelSettingCtrl.enabled)
        {
            yield return null;
        }

        Transform[] transform_temp = transform.GetComponentsInChildren<Transform>(true);

        lobbyPropData = new LobbyPropData();

        for (int i = 0; i < transform_temp.Length; i++)
        {
            switch (transform_temp[i].name)
            {
                case "ITEM_HEAD":
                    {
                        lobbyPropData.mesh_head = transform_temp[i].GetComponent<MeshFilter>();
                    }
                    break;
                case "ITEM_HAIR_F":
                    {
                        if (transform_temp[i].gameObject.activeSelf)
                        {
                            lobbyPropData.mesh_hair_f = transform_temp[i].GetComponent<MeshFilter>();
                        }
                        else
                        {
                            lobbyPropData.mesh_hair_f = null;
                        }
                    }
                    break;
                case "ITEM_HAIR_B":
                    {
                        if (transform_temp[i].gameObject.activeSelf)
                        {
                            lobbyPropData.mesh_hair_b = transform_temp[i].GetComponent<MeshFilter>();
                        }
                        else
                        {
                            lobbyPropData.mesh_hair_b = null;
                        }
                    }
                    break;
                case "BODY_ORG_HIDDEN":
                    {
                        lobbyPropData.mesh_body = transform_temp[i].GetComponent<SkinnedMeshRenderer>();
                    }
                    break;
                case "ITEM_UPPER":
                    {
                        lobbyPropData.mesh_upper = transform_temp[i].GetComponent<SkinnedMeshRenderer>();
                    }
                    break;
                case "ITEM_LOWER":
                    {
                        if (transform_temp[i].gameObject.activeSelf)
                        {
                            lobbyPropData.mesh_lower = transform_temp[i].GetComponent<SkinnedMeshRenderer>();
                        }
                        else
                        {
                            lobbyPropData.mesh_lower = null;
                        }
                    }
                    break;
                case "ITEM_LOWER_SUB":
                    {
                        if (transform_temp[i].gameObject.activeSelf)
                        {
                            lobbyPropData.mesh_lower_s = transform_temp[i].GetComponent<SkinnedMeshRenderer>();
                        }
                        else
                        {
                            lobbyPropData.mesh_lower_s = null;
                        }
                    }
                    break;
                case "ITEM_FOOT":
                    {
                        if (transform_temp[i].gameObject.activeSelf)
                        {
                            lobbyPropData.mesh_foot = transform_temp[i].GetComponent<SkinnedMeshRenderer>();
                        }
                        else
                        {
                            lobbyPropData.mesh_foot = null;
                        }
                    }
                    break;
                case "ITEM_HEAD_ACC":
                    {
                        if (transform_temp[i].gameObject.activeSelf)
                        {
                            lobbyPropData.mesh_acc = transform_temp[i].GetComponent<MeshFilter>();
                        }
                        else
                        {
                            lobbyPropData.mesh_acc = null;
                        }
                    }
                    break;
            }
        }
    }
    public Vector3 GetCenterPos()
    {
        return transform.TransformPoint(pos_center);
    }

    public void SetPropState(PropState _propState, Transform _handTr = null, int _handNum = 0)
    {
        propState = _propState;

        if (gameType == GameDataManager.GameType.None)
        {
            return;
        }

        switch (propState)
        {
            case PropState.Idle:
                {
                    if (shadow != null)
                    {
                        shadow.SetActive(true);
                    }

                    LobbyHologramCtrl.GetInstance.StopHologram();
                }
                break;
            case PropState.Grab:
                {
                    if (shadow != null)
                    {
                        shadow.SetActive(false);
                    }

                    if (_handTr != null)
                    {
                        handTr = _handTr;
                        handNum = _handNum;
                    }

                    if (lobbyPropData != null)
                    {
                        LobbyHologramCtrl.GetInstance.SetHologram(lobbyPropData, gameType);
                        LobbyHologramCtrl.GetInstance.PlayHologram();
                    }

                    LobbySoundManager.GetInstance.Play((int)Random.Range(0f,2.9999f));
                    LobbySoundManager.GetInstance.Play_Voice((int)gameType - 1);
                }
                break;
            case PropState.Set:
                {
                    if (shadow != null)
                    {
                        shadow.SetActive(false);
                    }

                    LobbyHologramCtrl.GetInstance.StopHologram();
                }
                break;
        }
    }
    public void SetIdlePos()
    {
        transform.position = pos_orizin;
        transform.rotation = rot_orizin;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        switch (propState)
        {
            case PropState.Idle:
                {
                    if (gameType != GameDataManager.GameType.None && animState != PropState.Idle)
                    {
                        anim.SetBool("IsPlay", true);
                        anim.SetBool("IsCenter", false);
                        animState = PropState.Idle;
                    }

                    Vector3 pos = Vector3.MoveTowards(transform.position, pos_orizin, Time.deltaTime * 7f);
                    Quaternion q = Quaternion.RotateTowards(transform.rotation, rot_orizin, Time.deltaTime * 700f);
                    transform.SetPositionAndRotation(pos, q);
                }
                break;
            case PropState.Grab:
                {
                    if (gameType != GameDataManager.GameType.None && animState != PropState.Grab)
                    {
                        anim.SetBool("IsPlay", false);
                        anim.SetBool("IsCenter", false);
                        animState = PropState.Grab;
                    }

                    float dist = (handTr.position - LobbyPropManager.GetInstance.gameStartPos.position).sqrMagnitude;

                    Vector3 pos = handTr.TransformPoint(catchTransforms[handNum].pos);
                    Quaternion rot = handTr.rotation * Quaternion.Euler(catchTransforms[handNum].rot);

                    if (dist > 0.1f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * 5f);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, Time.deltaTime * 500f);
                    }
                    else
                    {
                        pos.y = Mathf.Clamp(pos.y, LobbyPropManager.GetInstance.gameStartPos.position.y, 5f);
                        transform.position = Vector3.MoveTowards(transform.position, Vector3.Lerp(LobbyPropManager.GetInstance.gameStartPos.position, pos, dist * 10f), Time.deltaTime * 3f);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, LobbyPropManager.GetInstance.gameStartPos.rotation, Time.deltaTime * 300f);
                    }
                }
                break;
            case PropState.Set:
                {
                    if (gameType != GameDataManager.GameType.None && animState != PropState.Set)
                    {
                        anim.SetBool("IsPlay", true);
                        anim.SetBool("IsCenter", true);
                        animState = PropState.Set;
                    }

                    Vector3 pos = Vector3.MoveTowards(transform.position, LobbyPropManager.GetInstance.gameStartPos.position, Time.deltaTime * 3f);
                    Quaternion q = Quaternion.RotateTowards(transform.rotation, LobbyPropManager.GetInstance.gameStartPos.rotation, Time.deltaTime * 500f);

                    if ((transform.position - LobbyPropManager.GetInstance.gameStartPos.position).sqrMagnitude <= 0.0001f && Vector3.Dot(transform.forward, LobbyPropManager.GetInstance.gameStartPos.forward) >= 0.98f)
                    {
                        transform.SetPositionAndRotation(LobbyPropManager.GetInstance.gameStartPos.position, LobbyPropManager.GetInstance.gameStartPos.rotation);
                        propState = PropState.Keep;
                        LobbyPropManager.GetInstance.SelectGame(this);
                    }
                    else
                    {
                        transform.SetPositionAndRotation(pos, q);
                    }
                }
                break;
        }
    }

    public PropState GetPropState()
    {
        return propState;
    }
}
