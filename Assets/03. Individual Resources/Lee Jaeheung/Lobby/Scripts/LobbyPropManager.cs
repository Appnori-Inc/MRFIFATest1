using SingletonBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPropManager : Singleton<LobbyPropManager>
{
    private List<LobbyPropCtrl> propCtrls = new List<LobbyPropCtrl>();

    public Transform gameStartPos;
    public ParticleSystem par_gameStart;
    private Vector3 keep_pos;

    public Transform[] selectpointTrs;

    public LobbyPropCtrl keep_prop;

    public Material[] mats_prop;

    private GameDataManager gameDataManager;

    protected override void Awake()
    {
        base.Awake();

        keep_pos = par_gameStart.transform.position;
        par_gameStart.transform.position = Vector3.up * -10f;

        if (PublicGameUIManager.GetInstance)
        {
            //PublicGameUIManager.GetInstance.AddMenuEvent((a) => { Debug.LogError("MenuClick - " + a); });
            //PublicGameUIManager.GetInstance.AddMenuEvent(MenuClick);
        }

        gameDataManager = GameDataManager.instance;
    }

    public void AddProp(LobbyPropCtrl lobbyProp)
    {
        propCtrls.Add(lobbyProp);
    }

    public void SetSelectPointActive(bool isActive, int index)
    {
        if (isActive != selectpointTrs[index].gameObject.activeSelf)
        {
            selectpointTrs[index].gameObject.SetActive(isActive);
        }
    }

    public LobbyPropCtrl FindNearProp(UIHandInfo handInfo, int index)
    {
        if (propCtrls.Count == 0)
        {
            return null;
        }

        LobbyPropCtrl lobbyProp = null;

        // 손 거리에서 가까운.
        for (int i = 0; i < propCtrls.Count; i++)
        {
            if (lobbyProp == null && (handInfo.rayDir.position - propCtrls[i].GetCenterPos()).sqrMagnitude <= 0.1f)
            {
                lobbyProp = propCtrls[i];
            }
            else if (lobbyProp != null && (lobbyProp.GetCenterPos() - handInfo.rayDir.position).sqrMagnitude > (propCtrls[i].GetCenterPos() - handInfo.rayDir.position).sqrMagnitude)
            {
                lobbyProp = propCtrls[i];
            }
        }

        if (lobbyProp != null)
        {
            handInfo.SetPointState(UIHandInfo.PointState.Idle);
            SetSelectPointActive(true, index);
            selectpointTrs[index].position = lobbyProp.GetCenterPos() + Vector3.up * 0.15f;

            return lobbyProp;
        }

        // 레이 각도에서 가까운.
        for (int i = 0; i < propCtrls.Count; i++)
        {
            if (lobbyProp == null && Vector3.Dot(handInfo.rayDir.forward, (propCtrls[i].GetCenterPos() - handInfo.rayDir.position).normalized) >= 0.85f)
            {
                lobbyProp = propCtrls[i];
            }
            else if (lobbyProp != null && Vector3.Dot(handInfo.rayDir.forward, (lobbyProp.GetCenterPos() - handInfo.rayDir.position).normalized) < Vector3.Dot(handInfo.rayDir.forward, (propCtrls[i].GetCenterPos() - handInfo.rayDir.position).normalized))
            {
                lobbyProp = propCtrls[i];
            }
        }

        if (lobbyProp != null)
        {
            handInfo.SetPointState(UIHandInfo.PointState.PropLine);
            SetSelectPointActive(true, index);
            selectpointTrs[index].position = lobbyProp.GetCenterPos() + Vector3.up * 0.15f;

            Vector3 dir = lobbyProp.GetCenterPos() - handInfo.rayDir.position;
            handInfo.SetUITransform(Vector3.zero, handInfo.rayDir.position, Quaternion.Lerp(handInfo.rayDir.rotation, Quaternion.LookRotation(dir), 0.7f), dir.magnitude);
        }
        else
        {
            handInfo.SetPointState(UIHandInfo.PointState.Idle);
            SetSelectPointActive(false, index);
            handInfo.SetUITransform(Vector3.zero, handInfo.rayDir.position, handInfo.rayDir.rotation, 1f);
        }

        return lobbyProp;
    }

    public void SelectGame(LobbyPropCtrl prop)
    {
        gameDataManager.gameType = prop.gameType;
        keep_prop = prop;
        par_gameStart.transform.position = keep_pos;
        par_gameStart.Play();
        LobbySoundManager.GetInstance.Play(3);

        if (prop.gameType == GameDataManager.GameType.Customize)
        {
            LobbyUIManager.Instance.isStartGame = true;
            gameDataManager.playType = GameDataManager.PlayType.None;
            MeshFadeCtrl.instance.LoadScene("Scene_Custom_Q");
            return;
        }

        selectGameCoroutine = StartCoroutine(SelectGameCoroutine());
    }

    Coroutine selectGameCoroutine;
    public void StopSelectGame()
    {
        if (selectGameCoroutine != null)
        {
            StopCoroutine(selectGameCoroutine);
        }     
    }

    IEnumerator SelectGameCoroutine()
    {
        yield return new WaitForSeconds(1f);
        LobbyUIManager.GetInstance.OpenStartPage();
    }
}
