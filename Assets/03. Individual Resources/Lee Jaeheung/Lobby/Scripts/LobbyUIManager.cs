using Photon.Pun;
using SingletonBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class LobbyUIManager : Singleton<LobbyUIManager>
{
    public LobbyPlayerCtrl lobbyPlayer;

    public bool isStartGame = false;

    public Text text_code;

    private int codeSize_min = 4;
    private int codeSize_max = 4;
    private string inputString = "";

    public Animator anim_mainUI;
    public Animator anim_keyboardUI;
    public Animator anim_otherGameUI;
    public Animator anim_serverUI;

    public GameObject[] gos_ui;

    private bool isActive = false;

    public GameObject button_connect;
    public GameObject go_connect;
    public GameObject go_disconnect;
    public GameObject go_update;
    public GameObject go_roomOut;
    public GameObject button_match;

    public LobbyUIState lobbyUIState = LobbyUIState.Close;

    public Text text_gameType;

    public Text text_time;

    public Text[] text_userNick;
    public Text[] text_userInfo;

    public MeshButtonCtrl[] meshButtons_gameType;

    public MeshButtonCtrl[] meshButtons_level_05;
    public MeshButtonCtrl[] meshButtons_level_10;

    public GameObject[] medals_level;

    public Material[] materials_lobbyUI;

    public Sprite[] sprites_ping;
    public Text text_serverName;
    public Text text_ping;
    public Image image_ping;
    public GameObject pref_page;
    private List<GameObject> list_pageIcon = new List<GameObject>();

    public HorizontalLayoutGroup layoutGroup;

    public AchievementCtrl achievement;
    public GameSettingCtrl gameSetting;
    public MedalViewCtrl medalView;

    public XRRayInteractor[] rayInteractors;

    [Serializable]
    public class ModeButton
    {
        public Transform tr;
        public Text name;
        public MeshFilter meshFilter;
    }

    public ModeButton[] modeButtons;

    public Mesh[] mesh_modeButtons;

    public enum LobbyUIState
    {
        Close, Menu, Mode, Level, InputCode, Random, Friend, Match_S, Server, RoomOut
    }

    private GameDataManager gameDataManager;
    public Sprite[] sprites_match_slot; // 0: 회색, 1:컬러.

    public enum RootState
    {
        None, Single, Random, Friend
    }
    public RootState rootState = RootState.None;

    public Transform exitButtonTr;
    private float exitDelayTime = 2f;

    void Start()
    {
        Physics.autoSimulation = true;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        Physics.bounceThreshold = 1f;
        Physics.sleepThreshold = 0.005f;
        Physics.defaultContactOffset = 0.01f;
        Physics.defaultSolverIterations = 6;
        Physics.defaultSolverVelocityIterations = 1;

        gameDataManager = GameDataManager.instance;
        gameDataManager.playType = GameDataManager.PlayType.None;

        lobbyUIState = LobbyUIState.Close;
        for (int i = 0; i < gos_ui.Length; i++)
        {
            gos_ui[i].SetActive(false);
        }
        anim_keyboardUI.gameObject.SetActive(false);
        anim_otherGameUI.gameObject.SetActive(false);
        anim_serverUI.gameObject.SetActive(false);

        GameSettingCtrl.AddLocalizationChangedEvent(SetModeLocalization);

        gameSetting.View();

        Invoke("DelayInit", 1f);

#if SERVER_CHINA
        MeshButtonCtrl[] meshButtons = anim_serverUI.GetComponentsInChildren<MeshButtonCtrl>();
        for (int i = 0; i < meshButtons.Length; i++)
        {
            meshButtons[i].gameObject.SetActive(false);
        }
#endif

#if QIYU_ACCOUNT || PICO_PLATFORM
        LobbyUIManager.GetInstance.achievement.gameObject.SetActive(false);
#endif
        if (PublicGameUIManager.GetInstance.startViewState == PublicGameUIManager.StartViewState.Disconnect)
        {
            PublicGameUIManager.GetInstance.startViewState = PublicGameUIManager.StartViewState.Idle;
            SetLobbyUI(LobbyUIState.RoomOut);
        }

        gameDataManager.gameType = GameDataManager.GameType.None;
        gameDataManager.playType = GameDataManager.PlayType.None;

        PublicGameUIManager.SetHotKeyList(PublicGameUIManager.StateHotKeyList.Lobby_Close);
        PublicGameUIManager.SetHotKeyList(PublicGameUIManager.StateHotKeyList.Public_Close);
        PublicGameUIManager.SetHotKeyList(PublicGameUIManager.StateHotKeyList.Setting);
    }
    void DelayInit()
    {
        for (int i = 0; i < rayInteractors.Length; i++)
        {
            rayInteractors[i].enabled = true;
        }

        MeshFadeCtrl.instance.StartFade(true);
    }

    public void OpenStartPage()
    {
        if (GameDataManager.instance.gameType == GameDataManager.GameType.Baseball
            || GameDataManager.instance.gameType == GameDataManager.GameType.Basketball
            || GameDataManager.instance.gameType == GameDataManager.GameType.Bowling)
        {
            GameDataManager.mode = 2;
        }
        else
        {
            GameDataManager.mode = 1;
        }

        LobbySoundManager.GetInstance.Play(4);

        SetLobbyUI(LobbyUIState.Menu);
    }

    public void SetPageLevel()
    {
        PublicGameUIManager.profileCapture.ShotSingleImages();

        if ((gameDataManager.gameType == GameDataManager.GameType.Baseball && GameDataManager.mode == 1) || (gameDataManager.gameType == GameDataManager.GameType.Boxing && GameDataManager.mode == 3))
        {
            gameDataManager.customModelDatas_single = null;
            LobbySoundManager.GetInstance.Play(4);

            gameDataManager.userInfos.Clear();

            for (int i = 0; i < 2; i++)
            {
                GameDataManager.UserInfo userInfo = new GameDataManager.UserInfo();
                if (i == 0)
                {
                    userInfo.id = gameDataManager.userInfo_mine.id;
                    userInfo.nick = gameDataManager.userInfo_mine.nick;
                }
                else
                {
                    userInfo.id = "AI";
                    userInfo.nick = GameSettingCtrl.GetLocalizationText("0173") + 1;
                }
                gameDataManager.userInfos.Add(userInfo);
            }

            GameDataManager.level = 1;
            gameDataManager.playType = GameDataManager.PlayType.Single;
            StartGame();
            anim_mainUI.SetTrigger("OnClose");
            isActive = false;
        }
        else
        {
            LobbySoundManager.GetInstance.Play(4);
            SetLobbyUI(LobbyUIState.Level);
        }
    }

    public void SetLobbyUI(LobbyUIState setState)
    {
        isActive = false;

        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
        }
        stateCoroutine = StartCoroutine(StateCoroutine(setState));
    }

    Coroutine stateCoroutine;

    IEnumerator StateCoroutine(LobbyUIState setState)
    {
        switch (lobbyUIState)
        {
            case LobbyUIState.InputCode:
                {
                    StopPasswordUI();
                    anim_keyboardUI.SetTrigger("OnClose");
                }
                break;
            case LobbyUIState.Random:
                {
                    if (serverChangeCoroutine != null)
                    {
                        StopCoroutine(serverChangeCoroutine);
                    }
                    anim_otherGameUI.SetTrigger("OnClose");
                    anim_serverUI.SetTrigger("OnClose");
                    LobbyPhotonManager.GetInstance.StartDisconnect();
                }
                break;
            case LobbyUIState.Friend:
                {
                    if (serverChangeCoroutine != null)
                    {
                        StopCoroutine(serverChangeCoroutine);
                    }
                    anim_serverUI.SetTrigger("OnClose");
                    LobbyPhotonManager.GetInstance.StartDisconnect();
                }
                break;
        }
        if (lobbyUIState != LobbyUIState.Close)
        {
            anim_mainUI.SetTrigger("OnClose");
            yield return new WaitForSeconds(0.1f);
            while (1f > anim_mainUI.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                yield return null;
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                lobbyPlayer.handInfos[i].anim.SetBool("IsPoint", true);
            }
        }

        for (int i = 0; i < gos_ui.Length; i++)
        {
            gos_ui[i].SetActive(false);
        }
        LobbyUIState tempState = lobbyUIState;
        lobbyUIState = setState;

        if (lobbyUIState == LobbyUIState.Close)
        {
            if (LobbyPropManager.GetInstance.keep_prop != null)
            {
                LobbyPropManager.GetInstance.keep_prop.SetIdlePos();
                LobbyPropManager.GetInstance.keep_prop.SetPropState(PropState.Idle);
                LobbyPropManager.GetInstance.keep_prop = null;
                //lobbyPlayer.grab_prop_lobby = null;
                gameDataManager.gameType = GameDataManager.GameType.None;
            }
            for (int i = 0; i < 2; i++)
            {
                lobbyPlayer.handInfos[i].anim.SetBool("IsPoint", false);
            }
            PublicGameUIManager.SetHotKeyList(PublicGameUIManager.StateHotKeyList.Lobby_Close);
            yield break;
        }

        anim_mainUI.SetTrigger("OnOpen");
        yield return null;

        switch (lobbyUIState)
        {
            case LobbyUIState.Menu:
                {
                    PublicGameUIManager.SetHotKeyList(PublicGameUIManager.StateHotKeyList.Lobby_Menu);
                    gos_ui[0].SetActive(true);
                }
                break;
            case LobbyUIState.Mode:
                {
                    switch (gameDataManager.gameType)
                    {
                        case GameDataManager.GameType.Bowling:
                        case GameDataManager.GameType.Basketball:
                            {
                                modeButtons[0].tr.gameObject.SetActive(true);
                                modeButtons[1].tr.gameObject.SetActive(true);
                                modeButtons[2].tr.gameObject.SetActive(false);

                                modeButtons[0].tr.localPosition = new Vector3(-0.1f, -0.03f, 0f);
                                modeButtons[1].tr.localPosition = new Vector3(0.1f, -0.03f, 0f);

                                modeButtons[0].meshFilter.mesh = mesh_modeButtons[0];
                                modeButtons[1].meshFilter.mesh = mesh_modeButtons[1];

                                SetModeLocalization();
                            }
                            break;
                        case GameDataManager.GameType.Boxing:
                            {
                                modeButtons[0].tr.gameObject.SetActive(true);
                                modeButtons[1].tr.gameObject.SetActive(true);
                                modeButtons[2].tr.gameObject.SetActive(true);

                                modeButtons[0].tr.localPosition = new Vector3(0.15f, -0.03f, 0f);
                                modeButtons[1].tr.localPosition = new Vector3(0f, -0.03f, 0f);
                                modeButtons[2].tr.localPosition = new Vector3(-0.15f, -0.03f, 0f);

                                modeButtons[0].meshFilter.mesh = mesh_modeButtons[6];
                                modeButtons[1].meshFilter.mesh = mesh_modeButtons[2];
                                modeButtons[2].meshFilter.mesh = mesh_modeButtons[3];

                                SetModeLocalization();
                            }
                            break;
                        case GameDataManager.GameType.Baseball:
                            {
                                modeButtons[0].tr.gameObject.SetActive(true);
                                modeButtons[1].tr.gameObject.SetActive(true);
                                modeButtons[2].tr.gameObject.SetActive(false);

                                modeButtons[0].tr.localPosition = new Vector3(-0.1f, -0.03f, 0f);
                                modeButtons[1].tr.localPosition = new Vector3(0.1f, -0.03f, 0f);

                                modeButtons[0].meshFilter.mesh = mesh_modeButtons[0];
                                modeButtons[1].meshFilter.mesh = mesh_modeButtons[1];

                                SetModeLocalization();
                            }
                            break;
                    }

                    gos_ui[1].SetActive(true);
                }
                break;
            case LobbyUIState.Level:
                {
                    int clearData = gameDataManager.GetClearLevelData();
                    while (clearData == -1)
                    {
                        yield return null;
                        clearData = gameDataManager.GetClearLevelData();
                    }

                    if (gameDataManager.gameType == GameDataManager.GameType.Golf)
                    {
                        for (int i = 0; i < meshButtons_level_10.Length; i++)
                        {
                            if (i <= clearData)
                            {
                                meshButtons_level_10[i].SetMaterial(materials_lobbyUI[1]);
                                meshButtons_level_10[i].SetInteractable(true);
                            }
                            else
                            {
                                meshButtons_level_10[i].SetMaterial(materials_lobbyUI[0]);
                                meshButtons_level_10[i].SetInteractable(false);
                            }

                            if (i >= 6)
                            {
                                switch (i)
                                {
                                    case 8:
                                        {
                                            medals_level[3].SetActive(i < clearData);
                                        }
                                        break;
                                    case 7:
                                        {
                                            medals_level[4].SetActive(i < clearData);
                                        }
                                        break;
                                    case 6:
                                        {
                                            medals_level[5].SetActive(i < clearData);
                                        }
                                        break;
                                }
                            }

                        }
                        meshButtons_level_05[0].transform.parent.gameObject.SetActive(false);
                        meshButtons_level_10[0].transform.parent.gameObject.SetActive(true);
                    }
                    else
                    {
                        for (int i = 0; i < meshButtons_level_05.Length; i++)
                        {
                            if (i <= clearData)
                            {
                                meshButtons_level_05[i].SetMaterial(materials_lobbyUI[1]);
                                meshButtons_level_05[i].SetInteractable(true);
                            }
                            else
                            {
                                meshButtons_level_05[i].SetMaterial(materials_lobbyUI[0]);
                                meshButtons_level_05[i].SetInteractable(false);
                            }

                            if (i >= 2)
                            {
                                switch (i)
                                {
                                    case 4:
                                        {
                                            if (gameDataManager.gameType == GameDataManager.GameType.Boxing && GameDataManager.mode == 1)
                                            {
                                                medals_level[0].SetActive(false);
                                            }
                                            else
                                            {
                                                medals_level[0].SetActive(i < clearData);
                                            }
                                        }
                                        break;
                                    case 3:
                                        {
                                            if (gameDataManager.gameType == GameDataManager.GameType.Boxing && GameDataManager.mode == 1)
                                            {
                                                medals_level[1].SetActive(false);
                                            }
                                            else
                                            {
                                                medals_level[1].SetActive(i < clearData);
                                            }
                                        }
                                        break;
                                    case 2:
                                        {
                                            if (gameDataManager.gameType == GameDataManager.GameType.Boxing && GameDataManager.mode == 1)
                                            {
                                                medals_level[2].SetActive(false);
                                            }
                                            else
                                            {
                                                medals_level[2].SetActive(i < clearData);
                                            }
                                        }
                                        break;
                                }
                            }

                        }
                        meshButtons_level_10[0].transform.parent.gameObject.SetActive(false);
                        meshButtons_level_05[0].transform.parent.gameObject.SetActive(true);
                        PublicGameUIManager.SetHotKeyList(PublicGameUIManager.StateHotKeyList.Lobby_Level);
                    }
                   
                    gos_ui[2].SetActive(true);
                }
                break;
            case LobbyUIState.InputCode:
                {
                    inputString = "";
                    GameDataManager.roomCode = inputString;
                    StartPasswordUI();
                    gos_ui[3].SetActive(true);
                    gos_ui[6].SetActive(true);
                    PublicGameUIManager.SetHotKeyList(PublicGameUIManager.StateHotKeyList.Lobby_InputCode);
                }
                break;
            case LobbyUIState.Random:
                {
                    gos_ui[5].SetActive(true);

                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        button_connect.SetActive(true);
                        go_connect.SetActive(false);
                        go_disconnect.SetActive(true);
                        go_update.SetActive(false);
                        go_roomOut.SetActive(false);
                        isActive = true;
                        break;
                    }

                    button_connect.SetActive(false);
                    go_connect.SetActive(true);
                    go_disconnect.SetActive(false);
                    go_update.SetActive(false);
                    go_roomOut.SetActive(false);

                    float countTime = 5f;
                    while (gameDataManager.state_userInfo != GameDataManager.UserInfoState.Completed)
                    {
                        yield return null;
                        if (countTime > 0f)
                        {
                            countTime -= Time.deltaTime;
                        }
                        else if (!button_connect.activeSelf)
                        {
                            button_connect.SetActive(true);
                            isActive = true;
                        }
                    }
                    isActive = false;
                    LobbyPhotonManager.GetInstance.ClearServerInfos();
                    LobbyPhotonManager.GetInstance.StartConnect(true);

                    yield return new WaitForSeconds(0.1f);

                    while (1f > anim_mainUI.GetCurrentAnimatorStateInfo(0).normalizedTime)
                    {
                        yield return null;
                    }
                    LobbySoundManager.GetInstance.Play(9, true);

                    countTime = 5f;
                    while (!LobbyPhotonManager.GetInstance.isReady)
                    {
                        yield return null;
                        if (countTime > 0f)
                        {
                            countTime -= Time.deltaTime;
                        }
                        else if (!button_connect.activeSelf)
                        {
                            button_connect.SetActive(true);
                            isActive = true;
                        }
                    }
                    isActive = false;
                    LobbySoundManager.GetInstance.Play(4);
                    anim_mainUI.SetTrigger("OnClose");

                    yield return new WaitForSeconds(0.1f);

                    while (1f > anim_mainUI.GetCurrentAnimatorStateInfo(0).normalizedTime)
                    {
                        yield return null;
                    }
                    gos_ui[5].SetActive(false);
                    gos_ui[4].SetActive(true);

                    anim_mainUI.SetTrigger("OnOpen");
                    PublicGameUIManager.SetHotKeyList(PublicGameUIManager.StateHotKeyList.Lobby_Random);

                    yield return null;
                }
                break;
            case LobbyUIState.Friend:
                {
                    gos_ui[5].SetActive(true);
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        button_connect.SetActive(true);
                        go_connect.SetActive(false);
                        go_disconnect.SetActive(true);
                        go_update.SetActive(false);
                        go_roomOut.SetActive(false);
                        isActive = true;
                        break;
                    }

                    button_connect.SetActive(false);
                    go_connect.SetActive(true);
                    go_disconnect.SetActive(false);
                    go_update.SetActive(false);
                    go_roomOut.SetActive(false);

                    float countTime = 5f;
                    while (gameDataManager.state_userInfo != GameDataManager.UserInfoState.Completed)
                    {
                        yield return null;
                        if (countTime > 0f)
                        {
                            countTime -= Time.deltaTime;
                        }
                        else if (!button_connect.activeSelf)
                        {
                            button_connect.SetActive(true);
                            isActive = true;
                        }
                    }
                    isActive = false;
                    LobbyPhotonManager.GetInstance.ClearServerInfos();
                    LobbyPhotonManager.GetInstance.StartConnect(false);

                    yield return new WaitForSeconds(0.1f);

                    while (1f > anim_mainUI.GetCurrentAnimatorStateInfo(0).normalizedTime)
                    {
                        yield return null;
                    }
                    LobbySoundManager.GetInstance.Play(9, true);

                    countTime = 5f;
                    while (!LobbyPhotonManager.GetInstance.isReady)
                    {
                        yield return null;
                        if (countTime > 0f)
                        {
                            countTime -= Time.deltaTime;
                        }
                        else if (!button_connect.activeSelf)
                        {
                            button_connect.SetActive(true);
                            isActive = true;
                        }
                    }
                    isActive = false;
                    LobbySoundManager.GetInstance.Play(4);
                    anim_mainUI.SetTrigger("OnClose");

                    yield return new WaitForSeconds(0.1f);

                    while (1f > anim_mainUI.GetCurrentAnimatorStateInfo(0).normalizedTime)
                    {
                        yield return null;
                    }
                    gos_ui[5].SetActive(false);
                    gos_ui[4].SetActive(true);

                    anim_mainUI.SetTrigger("OnOpen");
                    PublicGameUIManager.SetHotKeyList(PublicGameUIManager.StateHotKeyList.Lobby_Friend);

                    yield return null;
                }
                break;
            case LobbyUIState.Match_S:
                {
                    gos_ui[4].SetActive(true);
                    anim_mainUI.SetTrigger("OnOpen");

                    GameDataManager.UserInfo userInfo = gameDataManager.userInfos[0];
                    userInfo.customModelData = gameDataManager.GetMyCustomModelData();
                    gameDataManager.userInfos[0] = userInfo;

                    PublicGameUIManager.profileCapture.PlayImages(new string[2] { gameDataManager.userInfo_mine.id, "AI" }, ProfileCaptureCtrl.ShotState.Multi);

                    for (int i = 0; i < 2; i++)
                    {
                        text_userNick[i].text = gameDataManager.userInfos[i].nick;
                        text_userInfo[i].text = "";
                    }

                    LobbyUIManager.GetInstance.SetViewGameType();

                    LobbyUIManager.GetInstance.SetViewMatchText();
                    yield return new WaitForSecondsRealtime(1f);
                    isActive = true;
                    int countTime = 3;
                    while (countTime > 0)
                    {
                        text_time.text = countTime.ToString();
                        yield return new WaitForSecondsRealtime(1f);
                        countTime--;
                    }

                    SetMatchBackButton(false);
                    StartGame();
                    isActive = false;
                }
                break;
            case LobbyUIState.Server:
                {
                    gos_ui[5].SetActive(true);

                    button_connect.SetActive(false);

                    float countTime = 5f;
                    while (gameDataManager.state_userInfo != GameDataManager.UserInfoState.Completed)
                    {
                        yield return null;
                        if (countTime > 0f)
                        {
                            countTime -= Time.deltaTime;
                        }
                        else if (!button_connect.activeSelf)
                        {
                            button_connect.SetActive(true);
                            isActive = true;
                        }
                    }
                    isActive = false;
                    LobbyPhotonManager.GetInstance.StartConnect(LobbyPhotonManager.GetInstance.GetServerInfo().code);
                    yield return new WaitForSeconds(0.1f);
                    while (1f > anim_mainUI.GetCurrentAnimatorStateInfo(0).normalizedTime)
                    {
                        yield return null;
                    }
                    LobbySoundManager.GetInstance.Play(9, true);
                    countTime = 5f;
                    while (!LobbyPhotonManager.GetInstance.isReady)
                    {
                        yield return null;
                        if (countTime > 0f)
                        {
                            countTime -= Time.deltaTime;
                        }
                        else if (!button_connect.activeSelf)
                        {
                            button_connect.SetActive(true);
                            isActive = true;
                        }
                    }
                    isActive = false;
                    LobbySoundManager.GetInstance.Play(4);
                    anim_mainUI.SetTrigger("OnClose");
                    yield return new WaitForSeconds(0.1f);
                    while (1f > anim_mainUI.GetCurrentAnimatorStateInfo(0).normalizedTime)
                    {
                        yield return null;
                    }
                    gos_ui[5].SetActive(false);
                    gos_ui[4].SetActive(true);

                    anim_mainUI.SetTrigger("OnOpen");
                    lobbyUIState = tempState;
                    if (lobbyUIState == LobbyUIState.Random)
                    {
                        PublicGameUIManager.SetHotKeyList(PublicGameUIManager.StateHotKeyList.Lobby_Random);
                    }
                    else
                    {
                        PublicGameUIManager.SetHotKeyList(PublicGameUIManager.StateHotKeyList.Lobby_Friend);
                    }
                    yield return null;
                }
                break;
            case LobbyUIState.RoomOut:
                {
                    gameDataManager.playType = GameDataManager.PlayType.None;

                    gos_ui[5].SetActive(true);

                    button_connect.SetActive(true);
                    go_connect.SetActive(false);
                    go_disconnect.SetActive(false);
                    go_update.SetActive(false);
                    go_roomOut.SetActive(true);
                    gameDataManager.userInfos.Clear();
                    LobbyPhotonManager.GetInstance.StartDisconnect();
                    PublicGameUIManager.SetHotKeyList(PublicGameUIManager.StateHotKeyList.Lobby_Back);
                }
                break;
        }

        yield return new WaitForSeconds(0.1f);
        while (1f > anim_mainUI.GetCurrentAnimatorStateInfo(0).normalizedTime)
        {
            yield return null;
        }
        isActive = true;
    }

    public void SetRoomOutUI()
    {
        isActive = false;

        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
        }
    }
    public void SetModeLocalization()
    {
        switch (gameDataManager.gameType)
        {
            case GameDataManager.GameType.Bowling:
            case GameDataManager.GameType.Basketball:
                {
                    modeButtons[1].name.text = GameSettingCtrl.GetLocalizationText("0006");
                }
                break;
            case GameDataManager.GameType.Boxing:
                {
                    modeButtons[0].name.text = GameSettingCtrl.GetLocalizationText("0006");
                    modeButtons[1].name.text = GameSettingCtrl.GetLocalizationText("0092");
                    modeButtons[2].name.text = GameSettingCtrl.GetLocalizationText("0093");
                }
                break;
            case GameDataManager.GameType.Baseball:
                {
                    modeButtons[0].name.text = GameSettingCtrl.GetLocalizationText("0007");
                    modeButtons[1].name.text = GameSettingCtrl.GetLocalizationText("0006");
                }
                break;
        }
    }

    private void Update()
    {
        if (exitDelayTime > 0f)
        {
            exitDelayTime -= Time.deltaTime;
        }

        if (lobbyUIState == LobbyUIState.Close)
        {
            if (gameDataManager.state_userInfo != GameDataManager.UserInfoState.Completed)
            {
                return;
            }

            if (((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Alpha1)) || ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Keypad1)))
            {
                Debug.Log("GameType - Tennis");
                gameDataManager.gameType = GameDataManager.GameType.Tennis;
                OpenStartPage();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                Debug.Log("GameType - Bowling");
                gameDataManager.gameType = GameDataManager.GameType.Bowling;
                OpenStartPage();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                Debug.Log("GameType - Archery");
                gameDataManager.gameType = GameDataManager.GameType.Archery;
                OpenStartPage();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                Debug.Log("GameType - BasketBall");
                gameDataManager.gameType = GameDataManager.GameType.Basketball;
                OpenStartPage();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                Debug.Log("GameType - Badminton");
                gameDataManager.gameType = GameDataManager.GameType.Badminton;
                OpenStartPage();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
            {
                Debug.Log("GameType - Billiards");
                gameDataManager.gameType = GameDataManager.GameType.Billiards;
                OpenStartPage();
            }
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
            {
                Debug.Log("GameType - Darts");
                gameDataManager.gameType = GameDataManager.GameType.Darts;
                OpenStartPage();
            }
            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
            {
                Debug.Log("GameType - PingPong");
                gameDataManager.gameType = GameDataManager.GameType.TableTennis;
                OpenStartPage();
            }
            if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
            {
                Debug.Log("GameType - Boxing");
                gameDataManager.gameType = GameDataManager.GameType.Boxing;
                OpenStartPage();
            }
            if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
            {
                Debug.Log("GameType - Golf");
                gameDataManager.gameType = GameDataManager.GameType.Golf;
                OpenStartPage();
            }
            if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
            {
                Debug.Log("GameType - BaseBall");
                gameDataManager.gameType = GameDataManager.GameType.Baseball;
                OpenStartPage();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("GameType - CharCustom");
                gameDataManager.playType = GameDataManager.PlayType.None;
                MeshFadeCtrl.instance.LoadScene("Scene_Custom_Q");
            }
        }

        if (!isActive)
        {
            return;
        }

        switch (lobbyUIState)
        {     
            case LobbyUIState.Menu:
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                    {
                        Debug.Log("Menu - Single");
                        Click_InputKey("Single");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                    {
                        Debug.Log("Menu - RandomMatch");
                        Click_InputKey("Random");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                    {
                        Debug.Log("Menu - FriendMatch");
                        Click_InputKey("Code");
                    }
                    if (Input.GetKeyDown(KeyCode.B))
                    {
                        Debug.Log("Back State");
                        Click_InputKey("PageBack");
                    }
                }
                break;
            case LobbyUIState.Mode:
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                    {
                        Debug.Log("Mode - Mode1");
                        Click_InputKey("Mode1");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                    {
                        Debug.Log("Mode - Mode2");
                        Click_InputKey("Mode2");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                    {
                        Debug.Log("Mode - Mode3");
                        Click_InputKey("Mode3");
                    }
                    if (Input.GetKeyDown(KeyCode.B))
                    {
                        Debug.Log("Back State");
                        Click_InputKey("PageBack");
                    }
                }
                break;
            case LobbyUIState.Level:
                if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                {
                    Debug.Log("Level_1 Start");
                    Click_InputKey("Level1");
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                {
#if !UNITY_EDITOR
                    if (GameDataManager.instance.GetClearLevelData() < 1)
                    {
                        return;
                    }
#endif
                    Debug.Log("Level_2 Start");
                    Click_InputKey("Level2");
                }
                if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                {
#if !UNITY_EDITOR
                    if (GameDataManager.instance.GetClearLevelData() < 2)
                    {
                        return;
                    }
#endif
                    Debug.Log("Level_3 Start");
                    Click_InputKey("Level3");
                }
                if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                {
#if !UNITY_EDITOR
                    if (GameDataManager.instance.GetClearLevelData() < 3)
                    {
                        return;
                    }
#endif
                    Debug.Log("Level_4 Start");
                    Click_InputKey("Level4");

                }
                if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                {
#if !UNITY_EDITOR
                    if (GameDataManager.instance.GetClearLevelData() < 4)
                    {
                        return;
                    }
#endif
                    Debug.Log("Level_5 Start");
                    Click_InputKey("Level5");
                }
                if ((Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) && gameDataManager.gameType == GameDataManager.GameType.Golf)
                {
#if !UNITY_EDITOR
                    if (GameDataManager.instance.GetClearLevelData() < 5)
                    {
                        return;
                    }
#endif
                    Debug.Log("Level_6 Start");
                    Click_InputKey("Level6");
                }
                if ((Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) && gameDataManager.gameType == GameDataManager.GameType.Golf)
                {
#if !UNITY_EDITOR
                    if (GameDataManager.instance.GetClearLevelData() < 6)
                    {
                        return;
                    }
#endif
                    Debug.Log("Level_7 Start");
                    Click_InputKey("Level7");
                }
                if ((Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) && gameDataManager.gameType == GameDataManager.GameType.Golf)
                {
#if !UNITY_EDITOR
                    if (GameDataManager.instance.GetClearLevelData() < 7)
                    {
                        return;
                    }
#endif
                    Debug.Log("Level_8 Start");
                    Click_InputKey("Level8");
                }
                if ((Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) && gameDataManager.gameType == GameDataManager.GameType.Golf)
                {
#if !UNITY_EDITOR
                    if (GameDataManager.instance.GetClearLevelData() < 8)
                    {
                        return;
                    }
#endif
                    Debug.Log("Level_9 Start");
                    Click_InputKey("Level9");
                }
                if (Input.GetKeyDown(KeyCode.B))
                {
                    Debug.Log("Back State");
                    Click_InputKey("PageBack");
                }
                break;
            case LobbyUIState.InputCode:
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
                    {
                        Click_InputKey("0");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                    {
                        Click_InputKey("1");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                    {
                        Click_InputKey("2");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                    {
                        Click_InputKey("3");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                    {
                        Click_InputKey("4");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
                    {
                        Click_InputKey("5");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
                    {
                        Click_InputKey("6");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
                    {
                        Click_InputKey("7");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
                    {
                        Click_InputKey("8");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
                    {
                        Click_InputKey("9");
                    }
                    if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                        Click_InputKey("Back");
                    }
                    if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                    {
                        Click_InputKey("Enter");
                    }
                    if (Input.GetKeyDown(KeyCode.B))
                    {
                        Debug.Log("Back State");
                        Click_InputKey("PageBack");
                    }
                }
                break;
            case LobbyUIState.Random:
                {
#if SERVER_GLOBAL
                    if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                    {
                        Click_InputKey("ServerB");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                    {
                        Click_InputKey("ServerN");
                    }
#endif
                    if (Input.GetKeyDown(KeyCode.B))
                    {
                        Debug.Log("Back State");
                        Click_InputKey("PageBack");
                    }
                }
                break;
            case LobbyUIState.Friend:
                {
#if SERVER_GLOBAL
                    if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                    {
                        Click_InputKey("ServerB");
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                    {
                        Click_InputKey("ServerN");
                    }
#endif
                    if (Input.GetKeyDown(KeyCode.B))
                    {
                        Debug.Log("Back State");
                        Click_InputKey("PageBack");
                    }
                }
                break;
            case LobbyUIState.RoomOut:
                {
                    if (Input.GetKeyDown(KeyCode.B))
                    {
                        Debug.Log("Back State");
                        Click_InputKey("PageBack");
                    }
                }
                break;
        }
    }
    private bool CheckExitButton()
    {
        if (exitDelayTime > 0f)
        {
            return false;
        }

        if (Camera.main == null)
        {
            return false;
        }

        Vector3 dir = (exitButtonTr.position - Camera.main.transform.position).normalized;

        return (Vector3.Dot(dir, Camera.main.transform.forward) >= 0.7f);
    }

    public void Click_InputKey(string key)
    {
        if (CheckExitButton())
        {
            switch (key)
            {
                case "Exit":
                    {
                        gameSetting.SaveData();
                        Application.Quit();
                    }
                    break;
                case "Setting":
                    {
                        LobbySoundManager.GetInstance.Play(4);
                        PublicGameUIManager.GetInstance.Click_InputKey("Setting");
                    }
                    break;
            }
        }

        if (!isActive)
        {
            return;
        }

        switch (key)
        {
            case "0":
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
                {
                    SetCodeText(key);
                }
                break;
            case "ServerB":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    SetServerPage(false);
                }
                break;
            case "ServerN":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    SetServerPage(true);
                }
                break;
            case "Back":
                {
                    if (inputString.Length <= 0)
                    {
                        LobbySoundManager.GetInstance.Play(8);
                        return;
                    }
                    LobbySoundManager.GetInstance.Play(6);
                    inputString = inputString.Remove(inputString.Length - 1);

                    StartPasswordUI();
                }
                break;
            case "Enter":
                {
                    if (inputString.Length < codeSize_min)
                    {
                        LobbySoundManager.GetInstance.Play(8);
                        return;
                    }
                    LobbySoundManager.GetInstance.Play(7);
                    GameDataManager.roomCode = inputString;
                    SetLobbyUI(LobbyUIState.Friend);
                }
                break;
            case "Menu":
                {
                    OpenStartPage();
                }
                break;
            case "Single":
                {
                    LobbySoundManager.GetInstance.Play_Voice(13);
                    rootState = RootState.Single;
                    if (gameDataManager.gameType == GameDataManager.GameType.Baseball || gameDataManager.gameType == GameDataManager.GameType.Boxing)
                    {
                        LobbySoundManager.GetInstance.Play(4);
                        SetLobbyUI(LobbyUIState.Mode);
                    }
                    else
                    {
                        Click_InputKey("Level");
                    }
                }
                break;
            case "Level":
                {
                    SetPageLevel();
                }
                break;
            case "Code":
                {
                    GameDataManager.mode = 1;
                    rootState = RootState.Friend;

                    if (gameDataManager.gameType == GameDataManager.GameType.Baseball
                    || gameDataManager.gameType == GameDataManager.GameType.Basketball
                    || gameDataManager.gameType == GameDataManager.GameType.Bowling)
                    {
                        GameDataManager.mode = 2;
                    }
                    else
                    {
                        GameDataManager.mode = 1;
                    }
                    LobbySoundManager.GetInstance.Play_Voice(15);
                    LobbySoundManager.GetInstance.Play(4);
                    SetLobbyUI(LobbyUIState.InputCode);
                }
                break;
            case "Random":
                {
                    GameDataManager.mode = 1;
                    rootState = RootState.Random;

                    if (gameDataManager.gameType == GameDataManager.GameType.Baseball
                    || gameDataManager.gameType == GameDataManager.GameType.Basketball
                    || gameDataManager.gameType == GameDataManager.GameType.Bowling)
                    {
                        GameDataManager.mode = 2;
                    }
                    else
                    {
                        GameDataManager.mode = 1;
                    }
                    LobbySoundManager.GetInstance.Play_Voice(14);
                    LobbySoundManager.GetInstance.Play(4);
                    SetLobbyUI(LobbyUIState.Random);
                }
                break;
            case "Close":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    SetLobbyUI(LobbyUIState.Close);
                }
                break;
            case "Mode1":
                {
                    GameDataManager.mode = 1;
                    Click_InputKey("Level");
                }
                break;
            case "Mode2":
                {
                    GameDataManager.mode = 2;
                    Click_InputKey("Level");
                }
                break;
            case "Mode3":
                {
                    GameDataManager.mode = 3;
                    Click_InputKey("Level");
                }
                break;
            case "Level1":
            case "Level2":
            case "Level3":
            case "Level4":
            case "Level5":
            case "Level6":
            case "Level7":
            case "Level8":
            case "Level9":
                {
                    LobbySoundManager.GetInstance.Play(4);

                    int get_level = int.Parse(key.Substring(5));
                    GameDataManager.level = get_level;

                    gameDataManager.userInfos.Clear();

                    for (int i = 0; i < 2; i++)
                    {
                        GameDataManager.UserInfo userInfo = new GameDataManager.UserInfo();
                        if (i == 0)
                        {
                            userInfo.id = gameDataManager.userInfo_mine.id;
                            userInfo.nick = gameDataManager.userInfo_mine.nick;
                        }
                        else
                        {
                            userInfo.id = "AI";
                            userInfo.nick = GameSettingCtrl.GetLocalizationText("0173") + GameDataManager.level;
                        }
                        gameDataManager.userInfos.Add(userInfo);
                    }

                    gameDataManager.playType = GameDataManager.PlayType.Single;

                    if (gameDataManager.gameType == GameDataManager.GameType.Golf || (gameDataManager.gameType == GameDataManager.GameType.Boxing && GameDataManager.mode == 2))
                    {
                        gameDataManager.customModelDatas_single = null;
                        StartGame();
                        anim_mainUI.SetTrigger("OnClose");
                        isActive = false;
                        return;
                    }

                    LobbySoundManager.GetInstance.Play_Voice(15 + get_level);
                    SetLobbyUI(LobbyUIState.Match_S);
                }
                break;
            case "Bowling":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    LobbyPhotonManager.GetInstance.SetFindRoom(GameDataManager.GameType.Bowling);
                    SetLobbyUI(LobbyUIState.Random);
                }
                break;
            case "Archery":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    LobbyPhotonManager.GetInstance.SetFindRoom(GameDataManager.GameType.Archery);
                    SetLobbyUI(LobbyUIState.Random);
                }
                break;
            case "BasketBall":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    LobbyPhotonManager.GetInstance.SetFindRoom(GameDataManager.GameType.Basketball);
                    SetLobbyUI(LobbyUIState.Random);
                }
                break;
            case "Badminton":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    LobbyPhotonManager.GetInstance.SetFindRoom(GameDataManager.GameType.Badminton);
                    SetLobbyUI(LobbyUIState.Random);
                }
                break;
            case "Billiards":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    LobbyPhotonManager.GetInstance.SetFindRoom(GameDataManager.GameType.Billiards);
                    SetLobbyUI(LobbyUIState.Random);
                }
                break;
            case "Darts":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    LobbyPhotonManager.GetInstance.SetFindRoom(GameDataManager.GameType.Darts);
                    SetLobbyUI(LobbyUIState.Random);
                }
                break;
            case "TableTennis":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    LobbyPhotonManager.GetInstance.SetFindRoom(GameDataManager.GameType.TableTennis);
                    SetLobbyUI(LobbyUIState.Random);
                }
                break;
            case "Boxing":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    LobbyPhotonManager.GetInstance.SetFindRoom(GameDataManager.GameType.Boxing);
                    SetLobbyUI(LobbyUIState.Random);
                }
                break;
            case "Golf":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    LobbyPhotonManager.GetInstance.SetFindRoom(GameDataManager.GameType.Golf);
                    SetLobbyUI(LobbyUIState.Random);
                }
                break;
            case "BaseBall":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    LobbyPhotonManager.GetInstance.SetFindRoom(GameDataManager.GameType.Baseball);
                    SetLobbyUI(LobbyUIState.Random);
                }
                break;
            case "Tennis":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    LobbyPhotonManager.GetInstance.SetFindRoom(GameDataManager.GameType.Tennis);
                    SetLobbyUI(LobbyUIState.Random);
                }
                break;
            case "PageBack":
                {
                    LobbySoundManager.GetInstance.Play(4);
                    switch (lobbyUIState)
                    {
                        case LobbyUIState.Level:
                            {
                                if (gameDataManager.gameType == GameDataManager.GameType.Baseball || gameDataManager.gameType == GameDataManager.GameType.Boxing)
                                {
                                    SetLobbyUI(LobbyUIState.Mode);
                                }
                                else
                                {
                                    SetLobbyUI(LobbyUIState.Menu);
                                }
                            }
                            break;
                        case LobbyUIState.Mode:
                            {
                                SetLobbyUI(LobbyUIState.Menu);
                            }
                            break;
                        case LobbyUIState.InputCode:
                        case LobbyUIState.Random:
                        case LobbyUIState.Friend:
                            {
                                SetLobbyUI(LobbyUIState.Menu);
                            }
                            break;
                        case LobbyUIState.Match_S:
                            {
                                SetLobbyUI(LobbyUIState.Level);
                            }
                            break;
                        default:
                            {
                                SetLobbyUI(LobbyUIState.Close);
                            }
                            break;
                    }
                }
                break;
        }
    }


    public void SetCodeText(string key)
    {
        if (inputString.Length >= codeSize_max)
        {
            LobbySoundManager.GetInstance.Play(8);
            return;
        }
        LobbySoundManager.GetInstance.Play(5);
        inputString += key;

        if (inputString.Length < codeSize_max)
        {
            StartPasswordUI();
        }
        else
        {
            StopPasswordUI();
        }
    }

    public void StartPasswordUI()
    {
        StopPasswordUI();
        coroutine_textAnim = StartCoroutine(TextAnimCoroutine());
    }

    public void StopPasswordUI()
    {
        if (coroutine_textAnim != null)
        {
            StopCoroutine(coroutine_textAnim);
        }
        text_code.text = inputString;
    }

    Coroutine coroutine_textAnim;
    IEnumerator TextAnimCoroutine()
    {
        while (true)
        {
            text_code.text = inputString + "<size=30> </size><size=25>_</size>";
            for (int i = inputString.Length + 1; i < codeSize_max; i++)
            {
                text_code.text += "<size=20>  </size><size=25>*</size>";
            }
            yield return new WaitForSeconds(0.5f);
            text_code.text = inputString + "  ";
            for (int i = inputString.Length + 1; i < codeSize_max; i++)
            {
                text_code.text += "<size=20>  </size><size=25>*</size>";
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void SetMatchBackButton(bool isActive)
    {
        button_match.SetActive(isActive);
    }

    public void SetOtherGames(bool isActive)
    {
        if (isActive)
        {
            gos_ui[7].transform.localScale = Vector3.zero;
        }
        gos_ui[7].SetActive(isActive);
    }

    public bool IsOtherGames()
    {
        return gos_ui[7].activeSelf;
    }

    public void SetServerUI(bool isActive)
    {
        if (isActive)
        {
            gos_ui[8].transform.localScale = Vector3.zero;
        }
        gos_ui[8].SetActive(isActive);
    }

    public void InitServerUI()
    {
        LobbyPhotonManager.GetInstance.index_server = 0;
        LobbyPhotonManager.ServerInfo serverInfo = LobbyPhotonManager.GetInstance.GetServerInfo();
        text_serverName.text = serverInfo.name;
        text_serverName.rectTransform.sizeDelta = new Vector2(text_serverName.preferredWidth, 80f);
        text_ping.text = serverInfo.ping;
        image_ping.sprite = sprites_ping[serverInfo.grade];
        pref_page.SetActive(true);

        for (int i = list_pageIcon.Count - 1; i >= 0; i--)
        {
            Destroy(list_pageIcon[i]);
        }

        list_pageIcon.Clear();

        for (int i = 0; i < LobbyPhotonManager.GetInstance.serverInfos.Length; i++)
        {
            list_pageIcon.Add(Instantiate(pref_page, pref_page.transform.parent));
        }

        pref_page.SetActive(false);
        SetServerPageIcon();
    }

    public void SetServerPageIcon()
    {
        int index = LobbyPhotonManager.GetInstance.index_server;
        for (int i = 0; i < list_pageIcon.Count; i++)
        {
            if (index == i)
            {
                list_pageIcon[i].transform.localScale = Vector3.one * 1.5f;
            }
            else
            {
                list_pageIcon[i].transform.localScale = Vector3.one;
            }
        }
    }

    public void SetServerPage(bool isNext)
    {
        int index = LobbyPhotonManager.GetInstance.index_server;
        if (isNext)
        {
            index += 1;
        }
        else
        {
            index -= 1;
        }

        if (index > LobbyPhotonManager.GetInstance.serverInfos.Length - 1)
        {
            index = 0;
        }
        else if (index < 0)
        {
            index = LobbyPhotonManager.GetInstance.serverInfos.Length - 1;
        }
        LobbyPhotonManager.GetInstance.index_server = index;
        LobbyPhotonManager.ServerInfo serverInfo = LobbyPhotonManager.GetInstance.GetServerInfo();
        text_serverName.text = serverInfo.name;
        text_serverName.rectTransform.sizeDelta = new Vector2(text_serverName.preferredWidth, 80f);
        text_ping.text = serverInfo.ping;
        image_ping.sprite = sprites_ping[serverInfo.grade];

        SetServerPageIcon();
        StartServerChange();
    }

    public void StartServerChange()
    {
        if (serverChangeCoroutine != null)
        {
            StopCoroutine(serverChangeCoroutine);
        }
        LobbyPhotonManager.GetInstance.StartDisconnect();
        serverChangeCoroutine = StartCoroutine(ServerChangeCoroutine());
    }

    Coroutine serverChangeCoroutine;

    IEnumerator ServerChangeCoroutine()
    {
        yield return new WaitForSeconds(3f);
        SetLobbyUI(LobbyUIState.Server);
    }

    public void SetViewGameType()
    {
        switch (gameDataManager.gameType)
        {
            case GameDataManager.GameType.Bowling:
                text_gameType.text = GameSettingCtrl.GetLocalizationText("0159");
                break;
            case GameDataManager.GameType.Archery:
                text_gameType.text = GameSettingCtrl.GetLocalizationText("0160");
                break;
            case GameDataManager.GameType.Basketball:
                text_gameType.text = GameSettingCtrl.GetLocalizationText("0161");
                break;
            case GameDataManager.GameType.Badminton:
                text_gameType.text = GameSettingCtrl.GetLocalizationText("0162");
                break;
            case GameDataManager.GameType.Billiards:
                text_gameType.text = GameSettingCtrl.GetLocalizationText("0163");
                break;
            case GameDataManager.GameType.Darts:
                text_gameType.text = GameSettingCtrl.GetLocalizationText("0164");
                break;
            case GameDataManager.GameType.TableTennis:
                text_gameType.text = GameSettingCtrl.GetLocalizationText("0165");
                break;
            case GameDataManager.GameType.Boxing:
                text_gameType.text = GameSettingCtrl.GetLocalizationText("0166");
                break;
            case GameDataManager.GameType.Golf:
                text_gameType.text = GameSettingCtrl.GetLocalizationText("0167");
                break;
            case GameDataManager.GameType.Baseball:
                text_gameType.text = GameSettingCtrl.GetLocalizationText("0168");
                break;
            case GameDataManager.GameType.Tennis:
                text_gameType.text = GameSettingCtrl.GetLocalizationText("0172");
                break;
        }
    }

    public void SetViewMatchText()
    {
        text_time.text = GameSettingCtrl.GetLocalizationText("0170");
    }

    public void StartGame()
    {
        if (isStartGame)
        {
            return;
        }

        isStartGame = true;

        LobbySoundManager.GetInstance.Play_Voice(21);

        if (gameDataManager.playType == GameDataManager.PlayType.Multi && !PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            MeshFadeCtrl.instance.StartFade(false);
            return;
        }


        switch (gameDataManager.gameType)
        {
            case GameDataManager.GameType.Bowling:
                {
                    MeshFadeCtrl.instance.LoadScene("Scene_Game_Bowling");
                }
                break;
            case GameDataManager.GameType.Archery:
                {
                    MeshFadeCtrl.instance.LoadScene("Scene_Game_Archery");
                }
                break;
            case GameDataManager.GameType.Basketball:
                {
                    MeshFadeCtrl.instance.LoadScene("Scene_Game_BasketBall");
                }
                break;
            case GameDataManager.GameType.Badminton:
                {
                    MeshFadeCtrl.instance.LoadScene("Scene_Game_Badminton");
                }
                break;
            case GameDataManager.GameType.Billiards:
                {
                    MeshFadeCtrl.instance.LoadScene("Scene_Game_Billiards");
                }
                break;
            case GameDataManager.GameType.Darts:
                {
                    MeshFadeCtrl.instance.LoadScene("Scene_Game_Darts");
                }
                break;
            case GameDataManager.GameType.TableTennis:
                {
                    MeshFadeCtrl.instance.LoadScene("Scene_Game_PingPong");
                }
                break;
            case GameDataManager.GameType.Boxing:
                {
                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        MeshFadeCtrl.instance.LoadScene("Scene_Game_Boxing_MT");
                        break;
                    }
                    switch (GameDataManager.mode)
                    {
                        case 2:
                            {
                                MeshFadeCtrl.instance.LoadScene("Scene_Game_Boxing");
                            }
                            break;
                        case 3:
                            {
                                MeshFadeCtrl.instance.LoadScene("Scene_Game_Boxing_HB");
                            }
                            break;
                        default:
                            {
                                MeshFadeCtrl.instance.LoadScene("Scene_Game_Boxing_AI");
                            }
                            break;
                    }
                }
                break;
            case GameDataManager.GameType.Golf:
                {
                    MeshFadeCtrl.instance.LoadScene("Scene_Game_Golf");
                }
                break;
            case GameDataManager.GameType.Baseball:
                {
                    MeshFadeCtrl.instance.LoadScene("Scene_Game_BaseBall");
                }
                break;
            case GameDataManager.GameType.Tennis:
                {
                    MeshFadeCtrl.instance.LoadScene("Scene_Game_Tennis");
                }
                break;
        }

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }
}
