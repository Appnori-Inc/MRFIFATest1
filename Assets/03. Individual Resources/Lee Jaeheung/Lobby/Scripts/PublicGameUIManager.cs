using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.EventSystems;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine.Audio;

public class PublicGameUIManager : MonoBehaviour
{
    private static PublicGameUIManager Instance;

    public static PublicGameUIManager GetInstance
    {
        get
        {
            if (Instance == null)
            {
                return Instantiate(Resources.Load<Transform>("UI/PublicGameUIManager")).GetComponent<PublicGameUIManager>();
            }
            return Instance;
        }
    }

    public static RenderOriginCamCtrl renderOriginCam;
    //public static LeaderBoardCtrl leaderBoard;
    public static GameSettingCtrl gameSetting;
    public static ProfileCaptureCtrl profileCapture;

    private event Action action_lobby;
    private event Action action_replay;
    private event Action<bool> action_menu;
    private event Action action_disconnect;

    private bool isInteractable = false;

    public enum ViewState
    {
        None, Menu, Result, Wait, Match, Setting
    }
    private ViewState viewState;
    private ViewState viewState_keep;

    private bool isViewCenter = false;
    private bool isOverlay = false;

    private Camera camera_main;
    private XRController[] controllers_main = new XRController[2];
    private Camera camera_overlay;
    private XRController[] controllers_overlay = new XRController[2];

#if PICO_PLATFORM
    private List<AudioSource> list_audioSource_bgm;
#endif

    private MeshButtonCtrl[] buttons_menu;
    private MeshButtonCtrl[] buttons_result;

    private Transform windowTr_menu;
    private Animator anim_result;
    private Transform windowTr_wait;
    private Transform windowTr_match;
    private MeshRenderer renderer_grid;
    private MeshRenderer renderer_arrow;
    private GameObject grid_clone;

    private float gridSize = 2f;
    private float distP_min = 0f;
    private float distP_max = 0f;
    private float distP_pause = 0f;

    private Vector3 targetPos;
    private Quaternion targetRot;

    public class UserInfo
    {
        public Text text_nick_result;
        public Text text_score;
        public Text text_nick_match;
        public Text text_info_match;
    }

    private UserInfo[] userInfos;
    private GameObject typeA;
    private GameObject typeB;
    private GameObject typeC;
    private Text text_info_A;
    private Text text_info_C;
    private Text text_userName_B;
    private Text text_mainScore_B;
    private GameObject mainScoreTitle_B;
    private GameObject[] subScore_images_B;
    private Text[] text_result_infos_B;
    private Text text_userName_C;
    private Text text_mainScore_C;
    private GameObject mainScoreTitle_C;
    private GameObject[] subScore_images_C;
    private Text[] text_result_infos_C;

    private Text text_wait_info;
    private Text text_wait_time;
    private GameObject button_wait;

    private Image image_medal;
    private Sprite[] sprites_medal;
    private GameObject eff_win_f;
    private GameObject eff_win_b;
    private Animator eff_win_light;
    private Image[] resultTexts;
    private Text text_matchTime;

    public Sprite[] sprite_resultText_eng;
    public Sprite[] sprite_resultText_cn;

    private bool lastButtonState = false;

    public UIHandInfo[] handInfos = new UIHandInfo[2];

    public LayerMask layerMask;

    public AudioClip[] audios_effect;
    public AudioClip[] audios_voice;
    public AudioClip[] audios_voice_cn;

    private AudioSource audioSource_effect;
    private AudioSource audioSource_voice;

    private bool isResultUpload = false;

    private bool isSetFirst = true;

    public StaticLocalizationCtrl.DataSlot[] dataSlots_localization;

    public bool isFocus = true;

    public Recorder recorder;

    private bool isDisCheck = false;

    public float playTime = 0;

    public enum StartViewState
    {
        Idle, Disconnect, Rematch
    }

    public StartViewState startViewState = StartViewState.Idle;

    private GameDataManager gameDataManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }


        Debug.Log("Awake_PublicGameUIManager");



         eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
         inputModule = GameObject.Find("EventSystem").GetComponent<XRUIInputModule>();
        SceneManager.sceneLoaded += ChangeSceneInit;
        handInfos = new UIHandInfo[2];
        Transform tempTr;
        for (int i = 0; i < handInfos.Length; i++)
        {
            handInfos[i] = new UIHandInfo();
            if (i == 0)
            {
                tempTr = transform.Find("LeftHand Controller");
            }
            else
            {
                tempTr = transform.Find("RightHand Controller");
            }
            handInfos[i].controller = tempTr.GetComponent<XRController>();
            handInfos[i].anim = tempTr.GetChild(0).GetComponent<Animator>();
            handInfos[i].rayDir = handInfos[i].anim.transform.Find("RayDir");
            if (i == 0)
            {
                tempTr = transform.Find("RayL");
            }
            else
            {
                tempTr = transform.Find("RayR");
            }
            handInfos[i].line_transform = tempTr.Find("Line");
            handInfos[i].ball_transform = tempTr.Find("Sphere");
        }

        recorder = PhotonVoiceNetwork.Instance.transform.GetComponent<Recorder>();
        recorder.IsRecording = true;

        audioSource_effect = gameObject.AddComponent<AudioSource>();
        audioSource_effect.loop = false;
        audioSource_effect.volume = 0.8f;
        audioSource_effect.playOnAwake = false;

        audioSource_voice = gameObject.AddComponent<AudioSource>();
        audioSource_voice.loop = false;
        audioSource_voice.volume = 0.8f;
        audioSource_voice.playOnAwake = false;

        InitSceneCamera();

        camera_overlay = transform.GetComponentInChildren<Camera>();
        controllers_overlay = camera_overlay.transform.parent.GetComponentsInChildren<XRController>();

        if (controllers_overlay != null)
        {
            for (int i = 0; i < controllers_overlay.Length; i++)
            {
                controllers_overlay[i].enableInputActions = false;
            }
        }

        renderOriginCam = transform.GetComponentInChildren<RenderOriginCamCtrl>();
        renderOriginCam.Init(camera_main.transform, camera_overlay.transform);

        windowTr_menu = transform.Find("Window_Menu");
        anim_result = transform.Find("Window_Result").GetComponent<Animator>();
        windowTr_wait = transform.Find("Window_Wait");
        windowTr_match = transform.Find("Window_Match");
        renderer_grid = transform.Find("Static/Grid").GetComponent<MeshRenderer>();
        renderer_arrow = transform.Find("Static/Arrow").GetComponent<MeshRenderer>();

        buttons_menu = new MeshButtonCtrl[3];

        buttons_menu[0] = windowTr_menu.Find("Button_Play").GetComponent<MeshButtonCtrl>();
        buttons_menu[1] = windowTr_menu.Find("Button_Lobby").GetComponent<MeshButtonCtrl>();
        buttons_menu[2] = windowTr_menu.Find("Button_Exit").GetComponent<MeshButtonCtrl>();

        buttons_result = new MeshButtonCtrl[3];

        buttons_result[0] = anim_result.transform.Find("Button_Restart").GetComponent<MeshButtonCtrl>();
        buttons_result[1] = anim_result.transform.Find("Button_Next").GetComponent<MeshButtonCtrl>();
        buttons_result[2] = anim_result.transform.Find("Button_Lobby").GetComponent<MeshButtonCtrl>();

        //leaderBoard = transform.GetComponentInChildren<LeaderBoardCtrl>(true);
        gameSetting = transform.GetComponentInChildren<GameSettingCtrl>(true);

        audioSource_effect.outputAudioMixerGroup = GameSettingCtrl.GetAudioMixerGroup("Effect");
        audioSource_voice.outputAudioMixerGroup = GameSettingCtrl.GetAudioMixerGroup("Effect");

        profileCapture = transform.GetComponentInChildren<ProfileCaptureCtrl>(true);

        image_medal = anim_result.transform.Find("Canvas/Image_Medal").GetComponent<Image>();
        sprites_medal = new Sprite[3];
        sprites_medal[0] = Resources.Load<Sprite>("UI/Image_Medal_Gold");
        sprites_medal[1] = Resources.Load<Sprite>("UI/Image_Medal_Silver");
        sprites_medal[2] = Resources.Load<Sprite>("UI/Image_Medal_Bronze");

        typeA = anim_result.transform.Find("Canvas/TypeA").gameObject;
        typeB = anim_result.transform.Find("Canvas/TypeB").gameObject;
        typeC = anim_result.transform.Find("Canvas/TypeC").gameObject;

        userInfos = new UserInfo[2];
        tempTr = null;

        for (int i = 0; i < userInfos.Length; i++)
        {
            userInfos[i] = new UserInfo();
            if (i == 0)
            {
                tempTr = windowTr_match.Find("Canvas/P1");
            }
            else
            {
                tempTr = windowTr_match.Find("Canvas/P2");
            }

            userInfos[i].text_nick_match = tempTr.Find("Text_Nick").GetComponent<Text>();
            userInfos[i].text_info_match = tempTr.Find("Text_Info").GetComponent<Text>();

            if (i == 0)
            {
                tempTr = typeA.transform.Find("User01");
            }
            else
            {
                tempTr = typeA.transform.Find("User02");
            }
            userInfos[i].text_nick_result = tempTr.Find("Text_Nick").GetComponent<Text>();
            userInfos[i].text_score = tempTr.Find("Text_Score").GetComponent<Text>();
        }

        /////////////////////

        text_wait_info = windowTr_wait.Find("Canvas/Text_Info").GetComponent<Text>();
        text_wait_time = windowTr_wait.Find("Canvas/Text_Time").GetComponent<Text>();
        button_wait = windowTr_wait.Find("Button_Lobby").gameObject;

        /////////////////////

        text_userName_B = typeB.transform.Find("User/Text_Nick").GetComponent<Text>();
        text_mainScore_B = typeB.transform.Find("Text_MainScore").GetComponent<Text>();
        mainScoreTitle_B = typeB.transform.Find("Text_MainScore_T").gameObject;

        tempTr = typeB.transform.Find("SubScores/Images");

        subScore_images_B = new GameObject[tempTr.childCount];
        for (int i = 0; i < subScore_images_B.Length; i++)
        {
            subScore_images_B[i] = tempTr.GetChild(i).gameObject;
        }

        tempTr = typeB.transform.Find("SubScores/Texts");

        text_result_infos_B = new Text[tempTr.childCount];
        for (int i = 0; i < text_result_infos_B.Length; i++)
        {
            text_result_infos_B[i] = tempTr.GetChild(i).GetComponent<Text>();
        }

        /////////////////////

        text_userName_C = typeC.transform.Find("User/Text_Nick").GetComponent<Text>();
        text_mainScore_C = typeC.transform.Find("Text_MainScore").GetComponent<Text>();
        mainScoreTitle_C = typeC.transform.Find("Text_MainScore_T").gameObject;

        tempTr = typeC.transform.Find("SubScores/Images");

        subScore_images_C = new GameObject[tempTr.childCount];
        for (int i = 0; i < subScore_images_C.Length; i++)
        {
            subScore_images_C[i] = tempTr.GetChild(i).gameObject;
        }

        tempTr = typeC.transform.Find("SubScores/Texts");

        text_result_infos_C = new Text[tempTr.childCount];
        for (int i = 0; i < text_result_infos_C.Length; i++)
        {
            text_result_infos_C[i] = tempTr.GetChild(i).GetComponent<Text>();
        }

        eff_win_b = typeA.transform.Find("Eff_Back").gameObject;
        eff_win_f = typeA.transform.Find("Eff_Front").gameObject;
        eff_win_light = eff_win_f.transform.Find("Image_Mask/Image").GetComponent<Animator>();
        resultTexts = new Image[3];
        resultTexts[0] = typeA.transform.Find("Image_Win").GetComponent<Image>();
        resultTexts[1] = typeA.transform.Find("Image_Lose").GetComponent<Image>();
        resultTexts[2] = typeA.transform.Find("Image_Draw").GetComponent<Image>();

        text_info_A = typeA.transform.Find("Text_Info").GetComponent<Text>();
        text_info_C = typeC.transform.Find("Text_Info").GetComponent<Text>();

        text_matchTime = windowTr_match.Find("Canvas/Text_Time").GetComponent<Text>();
        //findTr = userInfos[1].image_temp_match.transform.GetChild(0);


        // 레이어 셋팅.
        int layerIndex = 31;

        camera_overlay.cullingMask = 1 << layerIndex;

        transform.Find("LeftHand Controller/HandL/HAND").gameObject.layer = layerIndex;
        transform.Find("RightHand Controller/HandR/HAND").gameObject.layer = layerIndex;

        eff_win_f.layer = layerIndex;

        for (int i = 0; i < 2; i++)
        {
            handInfos[i].line_transform.gameObject.layer = layerIndex;
            handInfos[i].ball_transform.gameObject.layer = layerIndex;
        }

        Transform[] transforms = windowTr_menu.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i].gameObject.layer = layerIndex;
        }

        transforms = anim_result.transform.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i].gameObject.layer = layerIndex;
        }

        transforms = windowTr_wait.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i].gameObject.layer = layerIndex;
        }

        transforms = windowTr_match.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i].gameObject.layer = layerIndex;
        }

        transforms = transform.Find("Static").GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i].gameObject.layer = layerIndex;
        }

       // transforms = leaderBoard.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i].gameObject.layer = layerIndex;
        }

        transforms = gameSetting.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i].gameObject.layer = layerIndex;
        }

        // Transform 셋팅.
        gridSize = 3f;
        SetGridSize(gridSize);

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            buttons_menu[0].transform.localPosition = new Vector3(0.135f, 0f, 0f);
            buttons_menu[1].gameObject.SetActive(false);
            buttons_menu[2].transform.localPosition = new Vector3(-0.135f, 0f, 0f);
        }
        else
        {
            buttons_menu[0].transform.localPosition = new Vector3(0.19f, 0f, 0f);
            buttons_menu[1].gameObject.SetActive(true);
            buttons_menu[2].transform.localPosition = new Vector3(-0.19f, 0f, 0f);
        }

        // Active 끄기.
        for (int i = 0; i < 2; i++)
        {
            handInfos[i].anim.gameObject.SetActive(false);
            handInfos[i].line_transform.gameObject.SetActive(false);
            handInfos[i].ball_transform.gameObject.SetActive(false);
        }

        isOverlay = false;
        camera_overlay.enabled = false;
        viewState = ViewState.None;
        isViewCenter = false;
        windowTr_menu.gameObject.SetActive(false);
        anim_result.gameObject.SetActive(false);
        windowTr_wait.gameObject.SetActive(false);
        windowTr_match.gameObject.SetActive(false);
        renderer_arrow.gameObject.SetActive(false);
        renderer_grid.gameObject.SetActive(false);

        SetCloneGrid();
        isSetFirst = true;
        isInteractable = true;

        XRInteractionManager interactionManager = GameObject.FindObjectOfType<XRInteractionManager>();
        for (int i = 0; i < 2; i++)
        {
            xRRayInteractors[i].interactionManager = interactionManager;
        }

        EventSystem.current = eventSystem;

        if (GameSettingCtrl.localizationInfos == null || GameSettingCtrl.localizationInfos.Count == 0)
        {
            return;
        }
        SetLocaliztion();
#if UNITY_EDITOR
        CreateHotKeyList();
#endif
    }

    public enum StateHotKeyList
    {
        Lobby_Close, Lobby_Menu, Lobby_Mode, Lobby_Level, Lobby_InputCode, Lobby_Random, Lobby_Friend, Lobby_Back, Public_Close, Public_Menu, Public_Result, Setting
    }

#if UNITY_EDITOR
    Photon.Realtime.ClientState clientState;

    public class HotKeyList
    {
        public Text text;
        public Image image;
        public ContentSizeFitter contentSizeFitter;
    }

    private static bool isInitHotKey = false;
    private static HotKeyList[] hotKeyLists;

    private static readonly string name_hotKeyList = "IsHotKeyList";

    private static void CreateHotKeyList()
    {
        if (isInitHotKey)
        {
            return;
        }

        GameObject canvas_hotkeylist = new GameObject("Canvas_HotKey");
        Canvas canvas = canvas_hotkeylist.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;


        hotKeyLists = new HotKeyList[3];

        for (int i = 0; i < hotKeyLists.Length; i++)
        {
            hotKeyLists[i] = new HotKeyList();

            hotKeyLists[i].image = new GameObject("Image").AddComponent<Image>();
            hotKeyLists[i].image.color = new Color(0, 0, 0, 0.95f);
            hotKeyLists[i].image.transform.SetParent(canvas.transform);

            if (i == 0)
            {
                hotKeyLists[i].image.rectTransform.anchorMin = new Vector2(0f, 1f);
                hotKeyLists[i].image.rectTransform.anchorMax = new Vector2(0f, 1f);
                hotKeyLists[i].image.rectTransform.pivot = new Vector2(0f, 1f);
            }
            else if (i == 1)
            {
                hotKeyLists[i].image.rectTransform.anchorMin = new Vector2(1f, 1f);
                hotKeyLists[i].image.rectTransform.anchorMax = new Vector2(1f, 1f);
                hotKeyLists[i].image.rectTransform.pivot = new Vector2(1f, 1f);
            }
            else
            {
                hotKeyLists[i].image.rectTransform.anchorMin = new Vector2(1f, 0f);
                hotKeyLists[i].image.rectTransform.anchorMax = new Vector2(1f, 0f);
                hotKeyLists[i].image.rectTransform.pivot = new Vector2(1f, 0f);
            }


            hotKeyLists[i].image.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            hotKeyLists[i].image.rectTransform.sizeDelta = new Vector2(10f, 10f);

            hotKeyLists[i].text = new GameObject("Text").AddComponent<Text>();
            hotKeyLists[i].text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            hotKeyLists[i].text.transform.SetParent(hotKeyLists[i].image.transform);

            if (i == 0)
            {
                hotKeyLists[i].text.rectTransform.anchorMin = new Vector2(0f, 1f);
                hotKeyLists[i].text.rectTransform.anchorMax = new Vector2(0f, 1f);
                hotKeyLists[i].text.rectTransform.pivot = new Vector2(0f, 1f);
            }
            else if (i == 1)
            {
                hotKeyLists[i].text.rectTransform.anchorMin = new Vector2(1f, 1f);
                hotKeyLists[i].text.rectTransform.anchorMax = new Vector2(1f, 1f);
                hotKeyLists[i].text.rectTransform.pivot = new Vector2(1f, 1f);
            }
            else
            {
                hotKeyLists[i].text.rectTransform.anchorMin = new Vector2(1f, 0f);
                hotKeyLists[i].text.rectTransform.anchorMax = new Vector2(1f, 0f);
                hotKeyLists[i].text.rectTransform.pivot = new Vector2(1f, 0f);
            }

            hotKeyLists[i].text.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            hotKeyLists[i].text.rectTransform.sizeDelta = new Vector2(10f, 10f);

            hotKeyLists[i].contentSizeFitter = hotKeyLists[i].text.gameObject.AddComponent<ContentSizeFitter>();
            hotKeyLists[i].contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            hotKeyLists[i].contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            hotKeyLists[i].contentSizeFitter.enabled = false;
        }

        DontDestroyOnLoad(canvas_hotkeylist);

        bool isOn = PlayerPrefs.GetInt(name_hotKeyList, 1) == 1 ? true : false;
        ViewHotKeyList(isOn);

        isInitHotKey = true;
    }

    public static void ViewHotKeyList(bool isView)
    {
        for (int i = 0; i < hotKeyLists.Length; i++)
        {
            hotKeyLists[i].image.enabled = isView;
            hotKeyLists[i].text.enabled = isView;
            if (isView)
            {
                hotKeyLists[i].contentSizeFitter.SetLayoutHorizontal();
                hotKeyLists[i].contentSizeFitter.SetLayoutVertical();
                hotKeyLists[i].image.rectTransform.sizeDelta = hotKeyLists[i].text.rectTransform.sizeDelta;
            }
        }
    }

#endif
    public static void SetHotKeyList(StateHotKeyList state)
    {
#if UNITY_EDITOR
        CreateHotKeyList();

        switch (state)
        {
            case StateHotKeyList.Lobby_Close:
                {
                    hotKeyLists[0].text.text = "1) 볼링\n2) 양궁\n3) 농구\n4) 배드민턴\n5) 당구\n6) 다트\n7) 탁구\n8) 복싱\n9) 골프\n0) 야구\nShift+1) 테니스\nP) 커스텀룸";
                }
                break;
            case StateHotKeyList.Lobby_Menu:
                {
                    hotKeyLists[0].text.text = "1) 혼자하기\n2) 랜덤매치\n3) 비밀방\nB) 뒤로가기";
                }
                break;
            case StateHotKeyList.Lobby_Mode:
                {
                    hotKeyLists[0].text.text = "1) 모드1\n2) 모드2\n3) 모드3\nB) 뒤로가기";
                }
                break;
            case StateHotKeyList.Lobby_Level:
                {
                    hotKeyLists[0].text.text = "1) 레벨1\n2) 레벨2\n3) 레벨3\n4) 레벨4\n5) 레벨5\nB) 뒤로가기";
                }
                break;
            case StateHotKeyList.Lobby_InputCode:
                {
                    hotKeyLists[0].text.text = "1~9) 비번입력\nBack) 지우기\nEnter) 완료\nB) 뒤로가기";
                }
                break;
            case StateHotKeyList.Lobby_Random:
                {
#if SERVER_GLOBAL
                    hotKeyLists[0].text.text = "1) 다음서버\n2) 이전서버\nB) 뒤로가기";
#else
                    hotKeyLists[0].text.text = "B) 뒤로가기";
#endif
                }
                break;
            case StateHotKeyList.Lobby_Friend:
                {
#if SERVER_GLOBAL
                    hotKeyLists[0].text.text = "1) 다음서버\n2) 이전서버\nS) 게임시작\nB) 뒤로가기";
#else
                    hotKeyLists[0].text.text = "S) 게임시작\nB) 뒤로가기";
#endif
                }
                break;
            case StateHotKeyList.Lobby_Back:
                {
                    hotKeyLists[0].text.text = "B) 뒤로가기";
                }
                break;
            case StateHotKeyList.Public_Close:
                {
                    hotKeyLists[1].text.text = "M) 메뉴열기";
                }
                break;
            case StateHotKeyList.Public_Menu:
                {
                    if (Instance.buttons_menu[1].gameObject.activeSelf)
                    {
                        hotKeyLists[1].text.text = "Z) 계속하기\nX) 로비로\nS) 게임종료\nM) 메뉴닫기";
                    }
                    else
                    {
                        hotKeyLists[1].text.text = "Z) 계속하기\nS) 게임종료\nM) 메뉴닫기";
                    }
                }
                break;
            case StateHotKeyList.Public_Result:
                {

                    if (Instance.buttons_result[0].gameObject.activeSelf && Instance.buttons_result[1].gameObject.activeSelf)
                    {
                        hotKeyLists[1].text.text = "Z) 다시하기\nX) 다음레벨\nC) 로비로";
                    }
                    else if (Instance.buttons_result[0].gameObject.activeSelf || Instance.buttons_result[1].gameObject.activeSelf)
                    {
                        hotKeyLists[1].text.text = "Z) 다시하기\nC) 로비로";
                    }
                    else
                    {
                        hotKeyLists[1].text.text = "C) 로비로";
                    }
                }
                break;
            case StateHotKeyList.Setting:
                {
                    hotKeyLists[2].text.text = "Q) 배경음\nW) 효과음\nE) 진동\nR) 손잡이\nT) 보이스챗\nY) 언어\n+ 방향키) 값변경";
                }
                break;
        }

        for (int i = 0; i < hotKeyLists.Length; i++)
        {
            hotKeyLists[i].contentSizeFitter.SetLayoutHorizontal();
            hotKeyLists[i].contentSizeFitter.SetLayoutVertical();
            hotKeyLists[i].image.rectTransform.sizeDelta = hotKeyLists[i].text.rectTransform.sizeDelta;
        }
#endif
    }
    public static void ClearHotKeyList(int index)
    {
#if UNITY_EDITOR
        hotKeyLists[index].image.rectTransform.sizeDelta = new Vector2(50f, 50f);
        hotKeyLists[index].text.text = "";
#endif
    }

    public XRRayInteractor[] xRRayInteractors;
    public EventSystem eventSystem;
    public XRUIInputModule inputModule;

    private void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
        gameDataManager = GameDataManager.instance;
        eventSystem.transform.SetParent(transform);

#if PICO_PLATFORM
        Invoke("DelayInit", 0.5f);
        StartCoroutine(AddPxrManager());
#endif
    }

#if PICO_PLATFORM
    void DelayInit()
    {
        bgmCheckTime = 0f;
        list_audioSource_bgm = new List<AudioSource>();
        AudioMixerGroup audioMixerGroup = GameSettingCtrl.GetAudioMixerGroup("BGM");
        AudioSource[] bgms = FindObjectsOfType<AudioSource>();

        for (int i = 0; i < bgms.Length; i++)
        {
            if (bgms[i].outputAudioMixerGroup == audioMixerGroup)
            {
                list_audioSource_bgm.Add(bgms[i]);
            }
        }
    }
#endif
    void OnEnable()
    {
        InputDevices_Update(new InputDevice());

        InputDevices.deviceConnected += InputDevices_Update;
        InputDevices.deviceDisconnected += InputDevices_Update;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;

        InputDevices.deviceConnected -= InputDevices_Update;
        InputDevices.deviceDisconnected -= InputDevices_Update;
    }

    private void InputDevices_Update(InputDevice _device)
    {
        List<InputDevice> inputDevices = new List<InputDevice>();

        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, inputDevices);
        handInfos[0].isControllerOn = false;
        foreach (var device in inputDevices)
        {
            if (device.TryGetFeatureValue(CommonUsages.trigger, out float velue))
            {
                handInfos[0].isControllerOn = true;
            }
        }

        if (!handInfos[0].isControllerOn)
        {
            handInfos[0].InitData();
        }

        inputDevices.Clear();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, inputDevices);
        handInfos[1].isControllerOn = false;
        foreach (var device in inputDevices)
        {
            if (device.TryGetFeatureValue(CommonUsages.trigger, out float velue))
            {
                handInfos[1].isControllerOn = true;
            }
        }
        if (!handInfos[1].isControllerOn)
        {
            handInfos[1].InitData();
        }
    }

#if PICO_PLATFORM
    private float bgmCheckTime = 0f;
    private void FixedUpdate()
    {
        if (list_audioSource_bgm == null || list_audioSource_bgm.Count == 0)
        {
            return;
        }

        if (bgmCheckTime == -1f)
        {
            return;
        }

        if (list_audioSource_bgm[0] != null && !list_audioSource_bgm[0].isPlaying)
        {
            bool isTrueBGM = false;
            for (int i = 0; i < list_audioSource_bgm.Count; i++)
            {
                if (list_audioSource_bgm[i].clip != null && list_audioSource_bgm[i].clip.length > 5f)
                {
                    isTrueBGM = true;
                    break;
                }
            }

            if (!isTrueBGM)
            {
                return;
            }

            for (int i = 0; i < list_audioSource_bgm.Count; i++)
            {
                list_audioSource_bgm[i].time = bgmCheckTime + Time.deltaTime;
            }
        }
        else
        {
            if (list_audioSource_bgm[0] != null && list_audioSource_bgm[0].clip != null)
            {
                bgmCheckTime = list_audioSource_bgm[0].time;
            }
        }
    }
#endif

    void StartDisconnectCheck()
    {
        if (checkDisconnect != null)
        {
            StopCoroutine(checkDisconnect);
        }
        checkDisconnect = StartCoroutine(DisconnectCheck());
    }

    Coroutine checkDisconnect;

    IEnumerator DisconnectCheck()
    {
        WaitForSeconds wait_1 = new WaitForSeconds(1f);
        while (true)
        {
            yield return wait_1;

            if (Application.internetReachability != NetworkReachability.NotReachable && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                if (action_disconnect != null)
                {
                    action_disconnect.Invoke();
                }

                OpenResultBoard("", (gameDataManager.userInfo_mine.id == gameDataManager.userInfos[0].id ? 0 : 1));

                yield break;
            }

            if (!PhotonNetwork.InRoom)
            {
                if (action_disconnect != null)
                {
                    action_disconnect.Invoke();
                }

                OpenResultBoard("", (gameDataManager.userInfo_mine.id == gameDataManager.userInfos[0].id ? 1 : 0));

                yield break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        playTime += Time.deltaTime;
#if UNITY_EDITOR
        if (clientState != PhotonNetwork.NetworkClientState)
        {
            clientState = PhotonNetwork.NetworkClientState;
            Debug.Log(clientState);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            bool isOn = PlayerPrefs.GetInt(name_hotKeyList, 1) == 1 ? false : true;
            ViewHotKeyList(isOn);
            PlayerPrefs.SetInt(name_hotKeyList, isOn ? 1 : 0);
        }
#endif

#if OCULUS_PLATFORM && !UNITY_EDITOR
        CheckOculusFocus();
#endif
        if (!isInteractable)
        {
            isSetFirst = true;
            return;
        }

        switch (viewState)
        {
            case ViewState.Menu:
                {
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        Click_InputKey("Play");
                    }
                    else if (Input.GetKeyDown(KeyCode.X))
                    {
                        if (buttons_menu[1].gameObject.activeSelf)
                        {
                            Click_InputKey("Lobby");
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.C))
                    {
                        Click_InputKey("Exit");
                    }
                    else if (Input.GetKeyDown(KeyCode.M))
                    {
                        Click_InputKey("Menu");
                    }

                    Vector3 ui_pos = camera_overlay.transform.forward;
                    ui_pos.y = 0f;

                    float dot = Vector3.Dot(ui_pos.normalized, targetRot * -Vector3.forward);

                    Vector3 dir = targetPos - camera_overlay.transform.position;
                    dir.y = 0f;

                    float dist = dir.sqrMagnitude;

                    if (dot < 0.2f || dist < 0.05f || dist > 1f)
                    {
                        ui_pos = ui_pos.normalized * 0.6f;
                        ui_pos.y = -0.3f;

                        targetPos = (camera_overlay.transform.position + ui_pos);
                        targetRot = Quaternion.LookRotation(-ui_pos);
                    }

                    float speedP = (windowTr_menu.position - targetPos).sqrMagnitude + 0.02f;
                    windowTr_menu.position = Vector3.MoveTowards(windowTr_menu.position, targetPos, Time.unscaledDeltaTime * speedP * 30);
                    windowTr_menu.rotation = Quaternion.RotateTowards(windowTr_menu.rotation, targetRot, Time.unscaledDeltaTime * speedP * 3600f);

                    gameSetting.transform.position = windowTr_menu.TransformPoint(Vector3.right * -1.2f + Vector3.forward * -0.3f + Vector3.up * 0.3f);
                    Vector3 gameSetting_dir = camera_overlay.transform.position - gameSetting.transform.position;
                    gameSetting_dir.y = 0f;
                    gameSetting.transform.rotation = Quaternion.LookRotation(gameSetting_dir.normalized);
                }
                break;
            case ViewState.Result:
                {
                    if (Input.GetKeyDown(KeyCode.Z) && buttons_result[0].gameObject.activeSelf)
                    {
                        Click_InputKey("Replay");
                    }
                    else if (Input.GetKeyDown(KeyCode.X) && buttons_result[1].gameObject.activeSelf)
                    {
                        Click_InputKey("Next");
                    }
                    else if (Input.GetKeyDown(KeyCode.C))
                    {
                        Click_InputKey("Lobby");
                    }

                    Vector3 ui_pos = camera_overlay.transform.forward;
                    ui_pos.y = 0f;

                    float dot = Vector3.Dot(ui_pos.normalized, targetRot * -Vector3.forward);

                    Vector3 dir = targetPos - camera_overlay.transform.position;
                    dir.y = 0f;

                    float dist = dir.sqrMagnitude;

                    if (dot < 0.3f || dist < 0.05f || dist > 2.5f)
                    {
                        ui_pos = ui_pos.normalized * 1.3f;
                        ui_pos.y = -0.1f;

                        targetPos = (camera_overlay.transform.position + ui_pos);
                        targetRot = Quaternion.LookRotation(-ui_pos);
                    }

                    float speedP = (anim_result.transform.position - targetPos).sqrMagnitude + 0.02f;
                    speedP = Mathf.Clamp(speedP, 0f, 1f);
                    anim_result.transform.position = Vector3.MoveTowards(anim_result.transform.position, targetPos, Time.unscaledDeltaTime * speedP * 30);
                    anim_result.transform.rotation = Quaternion.RotateTowards(anim_result.transform.rotation, targetRot, Time.unscaledDeltaTime * speedP * 3600f);
                    /*if (leaderBoard.gameObject.activeSelf)
                    {
                        leaderBoard.transform.position = anim_result.transform.TransformPoint(Vector3.right * -0.9f + Vector3.forward * 0.1f); // anim_result.transform.forward * 1f +
                        Vector3 leaderBoard_dir = camera_overlay.transform.position - leaderBoard.transform.position;
                        leaderBoard_dir.y = 0f;
                        leaderBoard.transform.rotation = Quaternion.LookRotation(leaderBoard_dir.normalized);
                    }*/
                }
                break;
            case ViewState.Wait:
                {
                    Vector3 ui_pos = camera_overlay.transform.forward;
                    ui_pos.y = 0f;

                    float dot = Vector3.Dot(ui_pos.normalized, targetRot * -Vector3.forward);

                    Vector3 dir = targetPos - camera_overlay.transform.position;
                    dir.y = 0f;

                    float dist = dir.sqrMagnitude;

                    if (dot < 0.35f || dist < 0.05f || dist > 1f)
                    {
                        ui_pos = ui_pos.normalized * 0.9f;
                        ui_pos.y = -0.1f;

                        targetPos = (camera_overlay.transform.position + ui_pos);
                        targetRot = Quaternion.LookRotation(-ui_pos);
                    }

                    float speedP = (windowTr_wait.position - targetPos).sqrMagnitude + 0.02f;
                    windowTr_wait.position = Vector3.MoveTowards(windowTr_wait.position, targetPos, Time.unscaledDeltaTime * speedP * 30);
                    windowTr_wait.rotation = Quaternion.RotateTowards(windowTr_wait.rotation, targetRot, Time.unscaledDeltaTime * speedP * 3600f);
                }
                break;
            case ViewState.Match:
                {
                    Vector3 ui_pos = camera_overlay.transform.forward;
                    ui_pos.y = 0f;

                    float dot = Vector3.Dot(ui_pos.normalized, targetRot * -Vector3.forward);

                    Vector3 dir = targetPos - camera_overlay.transform.position;
                    dir.y = 0f;

                    float dist = dir.sqrMagnitude;

                    if (dot < 0.35f || dist < 0.05f || dist > 1f)
                    {
                        ui_pos = ui_pos.normalized * 0.9f;
                        ui_pos.y = -0.1f;

                        targetPos = (camera_overlay.transform.position + ui_pos);
                        targetRot = Quaternion.LookRotation(-ui_pos);
                    }

                    float speedP = (windowTr_match.position - targetPos).sqrMagnitude + 0.02f;
                    windowTr_match.position = Vector3.MoveTowards(windowTr_match.position, targetPos, Time.unscaledDeltaTime * speedP * 30);
                    windowTr_match.rotation = Quaternion.RotateTowards(windowTr_match.rotation, targetRot, Time.unscaledDeltaTime * speedP * 3600f);
                }
                break;
            case ViewState.Setting:
                {
                    Vector3 ui_pos = camera_overlay.transform.forward;
                    ui_pos.y = 0f;

                    float dot = Vector3.Dot(ui_pos.normalized, targetRot * -Vector3.forward);

                    Vector3 dir = targetPos - camera_overlay.transform.position;
                    dir.y = 0f;

                    float dist = dir.sqrMagnitude;

                    if (dot < 0.3f || dist < 0.05f || dist > 2.5f)
                    {
                        ui_pos.y = 0f;
                        ui_pos = ui_pos.normalized * 1.3f;
    
                        targetPos = (camera_overlay.transform.position + ui_pos);

                        targetRot = Quaternion.LookRotation(-ui_pos);
                    }

                    float speedP = (gameSetting.transform.position - targetPos).sqrMagnitude + 0.02f;
                    gameSetting.transform.position = Vector3.MoveTowards(gameSetting.transform.position, targetPos, Time.unscaledDeltaTime * speedP * 30);
                    gameSetting.transform.rotation = Quaternion.RotateTowards(gameSetting.transform.rotation, targetRot, Time.unscaledDeltaTime * speedP * 3600f);

                    windowTr_menu.position = gameSetting.transform.position + Vector3.down * 0.7f;
                    windowTr_menu.rotation = gameSetting.transform.rotation;
                }
                break;
            case ViewState.None:
                {
                    if (Input.GetKeyDown(KeyCode.M))
                    {
                        Click_InputKey("Menu");
                    }
                }
                break;
        }

        Vector3 trackerPos = camera_overlay.transform.position;
        if (!isOverlay)
        {
            trackerPos.y += 100f; 
        }
        renderer_grid.sharedMaterial.SetVector("_TrackerPos00", trackerPos);

        for (int i = 0; i < 2; i++)
        {
            trackerPos = handInfos[i].rayDir.position;
            if (!isOverlay)
            {
                trackerPos.y += 100f;
            }
            if (i == 0)
            {
                renderer_grid.sharedMaterial.SetVector("_TrackerPos01", trackerPos);
            }
            else
            {
                renderer_grid.sharedMaterial.SetVector("_TrackerPos02", trackerPos);
            }
        }

        renderer_grid.sharedMaterial.SetFloat("_TimeP", Time.unscaledTime);
        renderer_arrow.sharedMaterial.SetFloat("_TimeP", Time.unscaledTime);

        Vector3 tracker_pos = camera_overlay.transform.localPosition;
        tracker_pos.y = 0f;
        float tracker_dist = tracker_pos.sqrMagnitude;

        if (isSetFirst)
        {
            if (tracker_dist >= distP_pause)
            {
                SetViewCenter(true);
            }
            else
            {
                SetViewCenter(false);
            }
        }
        else
        {
            if (tracker_dist >= distP_pause && !isViewCenter)
            {
                SetViewCenter(true);
            }
            else if (tracker_dist <= distP_max && isViewCenter)
            {
                SetViewCenter(false);
            }
        }

        if (gridSize > 1.5f)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 temp_dist = handInfos[i].rayDir.position - transform.position;
                temp_dist.y = 0f;

                if (tracker_dist < (temp_dist - temp_dist.normalized * ((gridSize > 1.5f) ? 0.3f : 0.1f)).sqrMagnitude)
                {
                    tracker_dist = (temp_dist - temp_dist.normalized * ((gridSize > 1.5f) ? 0.3f : 0.1f)).sqrMagnitude;
                }
            }
        }

        float lineP = Mathf.Lerp(1.05f, 0.05f, Mathf.Clamp01(Mathf.InverseLerp(distP_min, distP_max, tracker_dist)));

        renderer_grid.sharedMaterial.SetFloat("_LineP", lineP);

        bool tempState = false;
        for (int i = 0; i < handInfos.Length; i++)
        {
            InputDevice device = handInfos[i].controller.inputDevice;
            bool primaryButtonState = false;
            tempState = (device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonState) // did get a value
                        && primaryButtonState)// the value we got
                        || (device.TryGetFeatureValue(CommonUsages.menuButton, out primaryButtonState) // did get a value
                        && primaryButtonState)// the value we got
                        || tempState; // cumulative result from other controllers
        }

        if (tempState != lastButtonState) // Button state changed since last frame
        {
            lastButtonState = tempState;
            if (lastButtonState)
            {
                Click_InputKey("Menu");
            }
        }

        if (viewState == ViewState.Menu || viewState == ViewState.Result || viewState == ViewState.Wait || viewState == ViewState.Setting)
        {
            for (int i = 0; i < 2; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(handInfos[i].rayDir.position, handInfos[i].rayDir.forward, out hit, 3f, layerMask))
                {
                    Vector3 dir = hit.point - handInfos[i].rayDir.position;

                    if (!handInfos[i].ball_transform.gameObject.activeSelf)
                    {
                        handInfos[i].ball_transform.gameObject.SetActive(true);
                    }
                    if (!handInfos[i].line_transform.gameObject.activeSelf)
                    {
                        handInfos[i].line_transform.gameObject.SetActive(true);
                    }

                    handInfos[i].ball_transform.position = hit.point;
                    handInfos[i].line_transform.position = handInfos[i].rayDir.position;

                    handInfos[i].line_transform.rotation = Quaternion.LookRotation(dir);
                    handInfos[i].line_transform.localScale = Vector3.forward * dir.magnitude;

                    float triggerValue = 0.5f;
                    if (handInfos[i].isControllerOn && handInfos[i].controller.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue))
                    {
                        bool onTrigger = (triggerValue >= 0.7f && !handInfos[i].isTriggerOn);
                        bool offTrigger = (triggerValue <= 0.3f && handInfos[i].isTriggerOn);

                        if (onTrigger || offTrigger)
                        {
                            handInfos[i].isTriggerOn = onTrigger;

                            if (!handInfos[i].isTriggerOn)
                            {
                                MeshButtonCtrl button = hit.collider.GetComponent<MeshButtonCtrl>();
                                if (button != null && button.IsInteractable())
                                {
                                    button.event_click.Invoke();
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (handInfos[i].ball_transform.gameObject.activeSelf)
                    {
                        handInfos[i].ball_transform.gameObject.SetActive(false);
                    }
                    if (handInfos[i].line_transform.gameObject.activeSelf)
                    {
                        handInfos[i].line_transform.gameObject.SetActive(false);
                    }
                }
            }
        }

        isSetFirst = false;

        if (Input.GetKey(KeyCode.Q))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                gameSetting.SetOnBGM(true);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                gameSetting.SetOnBGM(false);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                gameSetting.SetValueBGM(GameSettingCtrl.settingInfo.value_bgm - 0.1f);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                gameSetting.SetValueBGM(GameSettingCtrl.settingInfo.value_bgm + 0.1f);
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                gameSetting.SetOnEffect(true);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                gameSetting.SetOnEffect(false);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                gameSetting.SetValueEffect(GameSettingCtrl.settingInfo.value_eff - 0.1f);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                gameSetting.SetValueEffect(GameSettingCtrl.settingInfo.value_eff + 0.1f);
            }
        }
        if (Input.GetKey(KeyCode.E))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                gameSetting.SetOnHaptic(true);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                gameSetting.SetOnHaptic(false);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                gameSetting.SetValueHaptic(GameSettingCtrl.settingInfo.value_haptic - 0.1f);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                gameSetting.SetValueHaptic(GameSettingCtrl.settingInfo.value_haptic + 0.1f);
            }
        }
        if (Input.GetKey(KeyCode.R))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                gameSetting.SetRightHanded(true);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                gameSetting.SetRightHanded(false);
            }
        }
        if (Input.GetKey(KeyCode.T))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                gameSetting.SetOnVoiceChat(true);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                gameSetting.SetOnVoiceChat(false);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                gameSetting.SetValueVoiceChat(GameSettingCtrl.settingInfo.value_voice - 0.1f);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                gameSetting.SetValueVoiceChat(GameSettingCtrl.settingInfo.value_voice + 0.1f);
            }
        }
        if (Input.GetKey(KeyCode.Y))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                gameSetting.SetLanguageState((int)GameSettingCtrl.languageState - 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                gameSetting.SetLanguageState((int)GameSettingCtrl.languageState + 1);
            }
        }
    }

    /// <summary>
    /// TypeA1 - 상대가 있을시 사용하는 함수.
    /// </summary>
    /// <param name="arr_score">점수 배열 length 2. </param>
    /// <param name="winnerPlayer">이긴플레이어 (0, 1). </param>
    public void OpenResultBoard(string[] arr_score, int winnerPlayer, int medal_info = -1)
    {
        if (viewState == ViewState.Result || viewState == ViewState.Wait)
        {
            return;
        }

        if (SceneManager.GetActiveScene().name == "Lobby" || SceneManager.GetActiveScene().name == "Scene_Custom_Q")
        {
            return;
        }

        for (int i = 0; i < 2; i++)
        {
            if (gameDataManager.userInfos[i].nick != null && gameDataManager.userInfos[i].nick.Length > 0)
            {
                userInfos[i].text_nick_result.text = gameDataManager.userInfos[i].nick;
            }
            else
            {
                userInfos[i].text_nick_result.text = "Player0" + i;
            }
            userInfos[i].text_score.text = arr_score[i];
        }

        text_info_A.text = "";

        if (winnerPlayer == 0)
        {
            eff_win_b.transform.localPosition = new Vector3(-190f, 55f, 0f);
            eff_win_f.transform.localPosition = eff_win_b.transform.localPosition;
            eff_win_b.SetActive(true);
            eff_win_f.SetActive(true);
        }
        else if (winnerPlayer == 1)
        {
            eff_win_b.transform.localPosition = new Vector3(190f, 55f, 0f);
            eff_win_f.transform.localPosition = eff_win_b.transform.localPosition;
            eff_win_b.SetActive(true);
            eff_win_f.SetActive(true);
        }
        else
        {
            eff_win_b.SetActive(false);
            eff_win_f.SetActive(false);
        }

        if (winnerPlayer != 0 && winnerPlayer != 1)
        {
            switch (GameSettingCtrl.GetLanguageState())
            {
                case LanguageState.schinese:
                    resultTexts[2].sprite = sprite_resultText_cn[2];
                    break;
                default:
                    resultTexts[2].sprite = sprite_resultText_eng[2];
                    break;
            }

            resultTexts[0].gameObject.SetActive(false);
            resultTexts[1].gameObject.SetActive(false);
            resultTexts[2].gameObject.SetActive(true);

            if (!isResultUpload)
            {
                Play_Voice(2);
            }

        }
        else if (gameDataManager.userInfos[winnerPlayer].id == gameDataManager.userInfo_mine.id)
        {
            switch (GameSettingCtrl.GetLanguageState())
            {
                case LanguageState.schinese:
                    resultTexts[0].sprite = sprite_resultText_cn[0];
                    break;
                default:
                    resultTexts[0].sprite = sprite_resultText_eng[0];
                    break;
            }

            resultTexts[0].gameObject.SetActive(true);
            resultTexts[1].gameObject.SetActive(false);
            resultTexts[2].gameObject.SetActive(false);

            if (!isResultUpload)
            {
                if (gameDataManager.playType == GameDataManager.PlayType.Multi || (gameDataManager.playType == GameDataManager.PlayType.Single && GameDataManager.singleManager != null))
                {
                    //gameDataManager.SetMultiResultScore(new string[2] { gameDataManager.userInfos[0].id, gameDataManager.userInfos[1].id }, winnerPlayer);
                    GameDataManager.score_win_mine++;
                    gameDataManager.UnlockAchieve("Ach07", 1);
                }
                Play_Voice(0);
            }
        }
        else
        {
            switch (GameSettingCtrl.GetLanguageState())
            {
                case LanguageState.schinese:
                    resultTexts[1].sprite = sprite_resultText_cn[1];
                    break;
                default:
                    resultTexts[1].sprite = sprite_resultText_eng[1];
                    break;
            }

            resultTexts[0].gameObject.SetActive(false);
            resultTexts[1].gameObject.SetActive(true);
            resultTexts[2].gameObject.SetActive(false);

            if (!isResultUpload)
            {
                if (gameDataManager.playType == GameDataManager.PlayType.Multi || (gameDataManager.playType == GameDataManager.PlayType.Single && GameDataManager.singleManager != null))
                {
                    GameDataManager.score_lose_mine++;
                }
                Play_Voice(1);
            }
        }
        isResultUpload = true;

        StopDisconnectCheck();

        if (gameDataManager.playType != GameDataManager.PlayType.Multi)
        {
            for (int i = 0; i < 2; i++)
            {
                GameDataManager.UserInfo userInfo = gameDataManager.userInfos[i];
                userInfo.customModelData = gameDataManager.GetCustomModelData(i);
                gameDataManager.userInfos[i] = userInfo;
            }
        }

        profileCapture.PlayImages(new string[2] { gameDataManager.userInfos[0].id, gameDataManager.userInfos[1].id }, ProfileCaptureCtrl.ShotState.Profile, winnerPlayer);

        if (gameDataManager.playType == GameDataManager.PlayType.Single && GameDataManager.singleManager == null && GameDataManager.level < 5 && winnerPlayer == 0
            && !(gameDataManager.gameType == GameDataManager.GameType.Baseball && GameDataManager.mode == 1))
        {
            buttons_result[0].transform.localPosition = new Vector3(0.19f, -0.3f, -0.007f);
            buttons_result[2].transform.localPosition = new Vector3(-0.19f, -0.3f, -0.007f);
            buttons_result[1].gameObject.SetActive(true);
            buttons_result[0].gameObject.SetActive(true);
        }
        else
        {
            buttons_result[0].transform.localPosition = new Vector3(0.1f, -0.3f, -0.007f);
            buttons_result[2].transform.localPosition = new Vector3(-0.1f, -0.3f, -0.007f);
            buttons_result[1].gameObject.SetActive(false);
            buttons_result[0].gameObject.SetActive(true);
        }

        if (gameDataManager.playType == GameDataManager.PlayType.Single && GameDataManager.singleManager == null && winnerPlayer == 0)
        {
            if ((gameDataManager.gameType == GameDataManager.GameType.Baseball && GameDataManager.mode == 1)
                || (gameDataManager.gameType == GameDataManager.GameType.Basketball && GameDataManager.mode == 1)
                || (gameDataManager.gameType == GameDataManager.GameType.Bowling && GameDataManager.mode == 1))
            {
                image_medal.gameObject.SetActive(false);
            }
            else if (medal_info != -1)
            {
                switch (medal_info)
                {
                    case 5:
                        {
                            image_medal.sprite = sprites_medal[0];
                            image_medal.gameObject.SetActive(true);
                        }
                        break;
                    case 4:
                        {
                            image_medal.sprite = sprites_medal[1];
                            image_medal.gameObject.SetActive(true);
                        }
                        break;
                    case 3:
                        {
                            image_medal.sprite = sprites_medal[2];
                            image_medal.gameObject.SetActive(true);
                        }
                        break;
                    default:
                        {
                            image_medal.gameObject.SetActive(false);
                        }
                        break;
                }
            }
            else if (GameDataManager.level >= 3)
            {
                switch (GameDataManager.level)
                {
                    case 5:
                        {
                            image_medal.sprite = sprites_medal[0];
                        }
                        break;
                    case 4:
                        {
                            image_medal.sprite = sprites_medal[1];
                        }
                        break;
                    case 3:
                        {
                            image_medal.sprite = sprites_medal[2];
                        }
                        break;
                }

                image_medal.gameObject.SetActive(true);
            }
            else
            {
                image_medal.gameObject.SetActive(false);
            }
        }
        else
        {
            image_medal.gameObject.SetActive(false);
        }

        typeC.SetActive(false);
        typeB.SetActive(false);
        typeA.SetActive(true);

        SetViewState(ViewState.Result);
    }

    /// <summary>
    /// TypeA2 - 상대가 접속종료했을때 사용 함수.
    /// </summary>
    /// <param name="info">설명글. </param>
    /// <param name="winnerPlayer">이긴플레이어 (0, 1). </param>
    public void OpenResultBoard(string info, int winnerPlayer)
    {
        if (viewState == ViewState.Result || viewState == ViewState.Wait)
        {
            return;
        }

        if (SceneManager.GetActiveScene().name == "Lobby" || SceneManager.GetActiveScene().name == "Scene_Custom_Q")
        {
            return;
        }

        for (int i = 0; i < 2; i++)
        {
            userInfos[i].text_nick_result.text = gameDataManager.userInfos[i].nick;
            userInfos[i].text_score.text = "";
        }

        if (winnerPlayer == 0)
        {
            eff_win_b.transform.localPosition = new Vector3(-190f, 55f, 0f);
            eff_win_f.transform.localPosition = eff_win_b.transform.localPosition;
            eff_win_b.SetActive(true);
            eff_win_f.SetActive(true);
        }
        else if (winnerPlayer == 1)
        {
            eff_win_b.transform.localPosition = new Vector3(190f, 55f, 0f);
            eff_win_f.transform.localPosition = eff_win_b.transform.localPosition;
            eff_win_b.SetActive(true);
            eff_win_f.SetActive(true);
        }
        else
        {
            eff_win_b.SetActive(false);
            eff_win_f.SetActive(false);
        }

        if (winnerPlayer != 0 && winnerPlayer != 1)
        {
            switch (GameSettingCtrl.GetLanguageState())
            {
                case LanguageState.schinese:
                    resultTexts[2].sprite = sprite_resultText_cn[2];
                    break;
                default:
                    resultTexts[2].sprite = sprite_resultText_eng[2];
                    break;
            }

            resultTexts[0].gameObject.SetActive(false);
            resultTexts[1].gameObject.SetActive(false);
            resultTexts[2].gameObject.SetActive(true);

            if (!isResultUpload)
            {
                Play_Voice(2);
            }
        }
        else if (gameDataManager.userInfos[winnerPlayer].id == gameDataManager.userInfo_mine.id)
        {
            switch (GameSettingCtrl.GetLanguageState())
            {
                case LanguageState.schinese:
                    resultTexts[0].sprite = sprite_resultText_cn[0];
                    break;
                default:
                    resultTexts[0].sprite = sprite_resultText_eng[0];
                    break;
            }

            text_info_A.text = text_wait_info.text = GameSettingCtrl.GetLocalizationText("0034");
            resultTexts[0].gameObject.SetActive(true);
            resultTexts[1].gameObject.SetActive(false);
            resultTexts[2].gameObject.SetActive(false);

            if (!isResultUpload)
            {
                if (gameDataManager.playType == GameDataManager.PlayType.Multi && isDisCheck && PhotonNetwork.IsConnected)
                {
                    //gameDataManager.SetMultiResultScore(new string[2] { gameDataManager.userInfos[0].id, gameDataManager.userInfos[1].id }, winnerPlayer, true);
                    GameDataManager.score_win_mine++;
                    gameDataManager.UnlockAchieve("Ach07", 1);
                }

                Play_Voice(0);
            }
        }
        else
        {
            switch (GameSettingCtrl.GetLanguageState())
            {
                case LanguageState.schinese:
                    resultTexts[1].sprite = sprite_resultText_cn[1];
                    break;
                default:
                    resultTexts[1].sprite = sprite_resultText_eng[1];
                    break;
            }

            text_info_A.text = text_wait_info.text = GameSettingCtrl.GetLocalizationText("0090");
            resultTexts[0].gameObject.SetActive(false);
            resultTexts[1].gameObject.SetActive(true);
            resultTexts[2].gameObject.SetActive(false);

            if (!isResultUpload)
            {
                if (gameDataManager.playType == GameDataManager.PlayType.Multi || (gameDataManager.playType == GameDataManager.PlayType.Single && GameDataManager.singleManager != null))
                {
                    GameDataManager.score_lose_mine++;
                    GameDataManager.score_disconnect_mine++;
                }

                Play_Voice(1);
            }
        }
        isResultUpload = true;
        StopDisconnectCheck();
        
        profileCapture.PlayImages(new string[2] { gameDataManager.userInfos[0].id, gameDataManager.userInfos[1].id }, ProfileCaptureCtrl.ShotState.Profile, winnerPlayer);

        image_medal.gameObject.SetActive(false);

        buttons_result[2].transform.localPosition = new Vector3(0f, -0.3f, -0.007f);
        buttons_result[1].gameObject.SetActive(false);
        buttons_result[0].gameObject.SetActive(false);

        typeC.SetActive(false);
        typeB.SetActive(false);
        typeA.SetActive(true);

        SetViewState(ViewState.Result);
    }

    /// <summary>
    /// TypeB - 상대방없는 싱글게임에 사용하는 함수.
    /// </summary>
    /// <param name="mainScore">메인점수. </param>
    /// <param name="arr_subScore">서브점수. </param>
    public void OpenResultBoard(string mainScore, string[] arr_subScore, int medal_info = -1)
    {
        if (viewState == ViewState.Result || viewState == ViewState.Wait)
        {
            return;
        }

        if (SceneManager.GetActiveScene().name == "Lobby" || SceneManager.GetActiveScene().name == "Scene_Custom_Q")
        {
            return;
        }

        Play_Voice(5);

        text_userName_B.text = gameDataManager.userInfo_mine.nick;
        text_mainScore_B.text = mainScore;


        if (gameDataManager.gameType == GameDataManager.GameType.Basketball || gameDataManager.gameType == GameDataManager.GameType.Bowling)
        {
            mainScoreTitle_B.SetActive(true);
        }
        else
        {
            mainScoreTitle_B.SetActive(false);
        }

        int length = Math.Min(arr_subScore.Length, text_result_infos_B.Length);

        int i = 0;
        for (; i < length; i++)
        {
            text_result_infos_B[i].text = arr_subScore[i];
            text_result_infos_B[i].gameObject.SetActive(true);
            if (i % 2 == 0)
            {
                subScore_images_B[(int)Mathf.Round(i * 0.5f)].SetActive(true);
            }
        }

        for (; i < text_result_infos_B.Length; i++)
        {
            text_result_infos_B[i].gameObject.SetActive(false);
            if (i % 2 == 0)
            {
                subScore_images_B[(int)Mathf.Round(i * 0.5f)].SetActive(false);
            }
        }

        GameDataManager.UserInfo userInfo = gameDataManager.userInfos[0];
        userInfo.customModelData = gameDataManager.GetCustomModelData(0);
        gameDataManager.userInfos[0] = userInfo;

        profileCapture.PlayImages(new string[1] { gameDataManager.userInfo_mine.id }, ProfileCaptureCtrl.ShotState.Profile);

        if (GameDataManager.singleManager == null
            && (GameDataManager.level < 5 || (gameDataManager.gameType == GameDataManager.GameType.Golf && GameDataManager.level < 9))
            && gameDataManager.gameType != GameDataManager.GameType.Basketball && gameDataManager.gameType != GameDataManager.GameType.Bowling
            && !(gameDataManager.gameType == GameDataManager.GameType.Baseball && GameDataManager.mode == 1)
            && !(gameDataManager.gameType == GameDataManager.GameType.Boxing && GameDataManager.mode == 3))
        {
            buttons_result[0].transform.localPosition = new Vector3(0.19f, -0.3f, -0.007f);
            buttons_result[2].transform.localPosition = new Vector3(-0.19f, -0.3f, -0.007f);
            buttons_result[1].gameObject.SetActive(true);
            buttons_result[0].gameObject.SetActive(true);
        }
        else
        {
            buttons_result[0].transform.localPosition = new Vector3(0.1f, -0.3f, -0.007f);
            buttons_result[2].transform.localPosition = new Vector3(-0.1f, -0.3f, -0.007f);
            buttons_result[1].gameObject.SetActive(false);
            buttons_result[0].gameObject.SetActive(true);
        }

        if (GameDataManager.singleManager == null)
        {
            if (gameDataManager.gameType == GameDataManager.GameType.Baseball && GameDataManager.mode == 1)
            {
                image_medal.gameObject.SetActive(false);
            }
            else if (medal_info != -1)
            {
                switch (medal_info)
                {
                    case 5:
                        {
                            image_medal.sprite = sprites_medal[0];
                            image_medal.gameObject.SetActive(true);
                        }
                        break;
                    case 4:
                        {
                            image_medal.sprite = sprites_medal[1];
                            image_medal.gameObject.SetActive(true);
                        }
                        break;
                    case 3:
                        {
                            image_medal.sprite = sprites_medal[2];
                            image_medal.gameObject.SetActive(true);
                        }
                        break;
                    default:
                        {
                            image_medal.gameObject.SetActive(false);
                        }
                        break;
                }
            }
            else if (gameDataManager.gameType == GameDataManager.GameType.Golf && GameDataManager.level >= 7)
            {
                switch (GameDataManager.level)
                {
                    case 9:
                        {
                            image_medal.sprite = sprites_medal[0];
                        }
                        break;
                    case 8:
                        {
                            image_medal.sprite = sprites_medal[1];
                        }
                        break;
                    case 7:
                        {
                            image_medal.sprite = sprites_medal[2];
                        }
                        break;
                }

                image_medal.gameObject.SetActive(true);
            }
            else if (GameDataManager.level >= 3)
            {
                switch (GameDataManager.level)
                {
                    case 5:
                        {
                            image_medal.sprite = sprites_medal[0];
                        }
                        break;
                    case 4:
                        {
                            image_medal.sprite = sprites_medal[1];
                        }
                        break;
                    case 3:
                        {
                            image_medal.sprite = sprites_medal[2];
                        }
                        break;
                }

                image_medal.gameObject.SetActive(true);
            }
            else
            {
                image_medal.gameObject.SetActive(false);
            }
        }
        else
        {
            image_medal.gameObject.SetActive(false);
        }

        typeA.SetActive(false);
        typeC.SetActive(false);
        typeB.SetActive(true);

        SetViewState(ViewState.Result);
    }

    /// <summary>
    /// TypeC - 상대방없는 싱글게임에 사용하는 함수(성공여부 포함).
    /// </summary>
    /// <param name="mainScore">메인점수. </param>
    /// <param name="arr_subScore">서브점수. </param>
    /// <param name="isSuccess">성공 여부. </param>
    public void OpenResultBoard(string mainScore, string[] arr_subScore, bool isSuccess, int medal_info = -1)
    {
        if (viewState == ViewState.Result || viewState == ViewState.Wait)
        {
            return;
        }

        if (SceneManager.GetActiveScene().name == "Lobby" || SceneManager.GetActiveScene().name == "Scene_Custom_Q")
        {
            return;
        }

        if (isSuccess)
        {
            Play_Voice(3);
            text_info_C.text = "<color=#B3FFFA>" + GameSettingCtrl.GetLocalizationText("0035") + "</color>";
        }
        else
        {
            Play_Voice(4);
            text_info_C.text = "<color=#FF897A>" + GameSettingCtrl.GetLocalizationText("0036") + "</color>";
        }

        text_userName_C.text = gameDataManager.userInfo_mine.nick;
        text_mainScore_C.text = mainScore;

        if (gameDataManager.gameType == GameDataManager.GameType.Basketball || gameDataManager.gameType == GameDataManager.GameType.Bowling)
        {
            mainScoreTitle_C.SetActive(true);
        }
        else
        {
            mainScoreTitle_C.SetActive(false);
        }

        int length = Math.Min(arr_subScore.Length, text_result_infos_C.Length);

        int i = 0;
        for (; i < length; i++)
        {
            text_result_infos_C[i].text = arr_subScore[i];
            text_result_infos_C[i].gameObject.SetActive(true);
            if (i % 2 == 0)
            {
                subScore_images_C[(int)Mathf.Round(i * 0.5f)].SetActive(true);
            }
        }

        for (; i < text_result_infos_C.Length; i++)
        {
            text_result_infos_C[i].gameObject.SetActive(false);
            if (i % 2 == 0)
            {
                subScore_images_C[(int)Mathf.Round(i * 0.5f)].SetActive(false);
            }
        }

        GameDataManager.UserInfo userInfo = gameDataManager.userInfos[0];
        userInfo.customModelData = gameDataManager.GetCustomModelData(0);
        gameDataManager.userInfos[0] = userInfo;

        profileCapture.PlayImages(new string[1] { gameDataManager.userInfo_mine.id }, ProfileCaptureCtrl.ShotState.Profile, isSuccess ? 0 : 1);

        if (GameDataManager.singleManager == null && (GameDataManager.level < 5 || (gameDataManager.gameType == GameDataManager.GameType.Golf && GameDataManager.level < 9)) && isSuccess && gameDataManager.gameType != GameDataManager.GameType.Basketball && gameDataManager.gameType != GameDataManager.GameType.Bowling
            && !(gameDataManager.gameType == GameDataManager.GameType.Baseball && GameDataManager.mode == 1))
        {
            buttons_result[0].transform.localPosition = new Vector3(0.19f, -0.3f, -0.007f);
            buttons_result[2].transform.localPosition = new Vector3(-0.19f, -0.3f, -0.007f);
            buttons_result[1].gameObject.SetActive(true);
            buttons_result[0].gameObject.SetActive(true);
        }
        else
        {
            buttons_result[0].transform.localPosition = new Vector3(0.1f, -0.3f, -0.007f);
            buttons_result[2].transform.localPosition = new Vector3(-0.1f, -0.3f, -0.007f);
            buttons_result[1].gameObject.SetActive(false);
            buttons_result[0].gameObject.SetActive(true);
        }

        if (GameDataManager.singleManager == null && isSuccess)
        {
            if (gameDataManager.gameType == GameDataManager.GameType.Baseball && GameDataManager.mode == 1)
            {
                image_medal.gameObject.SetActive(false);
            }
            else if (medal_info != -1)
            {
                switch (medal_info)
                {
                    case 5:
                        {
                            image_medal.sprite = sprites_medal[0];
                            image_medal.gameObject.SetActive(true);
                        }
                        break;
                    case 4:
                        {
                            image_medal.sprite = sprites_medal[1];
                            image_medal.gameObject.SetActive(true);
                        }
                        break;
                    case 3:
                        {
                            image_medal.sprite = sprites_medal[2];
                            image_medal.gameObject.SetActive(true);
                        }
                        break;
                    default:
                        {
                            image_medal.gameObject.SetActive(false);
                        }
                        break;
                }
            }
            else if (gameDataManager.gameType == GameDataManager.GameType.Golf && GameDataManager.level >= 7)
            {
                switch (GameDataManager.level)
                {
                    case 9:
                        {
                            image_medal.sprite = sprites_medal[0];
                        }
                        break;
                    case 8:
                        {
                            image_medal.sprite = sprites_medal[1];
                        }
                        break;
                    case 7:
                        {
                            image_medal.sprite = sprites_medal[2];
                        }
                        break;
                }

                image_medal.gameObject.SetActive(true);
            }
            else if (GameDataManager.level >= 3)
            {
                switch (GameDataManager.level)
                {
                    case 5:
                        {
                            image_medal.sprite = sprites_medal[0];
                        }
                        break;
                    case 4:
                        {
                            image_medal.sprite = sprites_medal[1];
                        }
                        break;
                    case 3:
                        {
                            image_medal.sprite = sprites_medal[2];
                        }
                        break;
                }

                image_medal.gameObject.SetActive(true);
            }
            else
            {
                image_medal.gameObject.SetActive(false);
            }
        }
        else
        {
            image_medal.gameObject.SetActive(false);
        }

        typeA.SetActive(false);
        typeB.SetActive(false);
        typeC.SetActive(true);
        SetViewState(ViewState.Result);
    }
    public void CloseResultBoard()
    {
        if (viewState != ViewState.Result)
        {
            return;
        }

        SetViewState(ViewState.None);
    }
    public void OpenWaitUI()
    {
        if (waitUICoroutine != null)
        {
            StopCoroutine(waitUICoroutine);
        }
        PhotonNetwork.IsSyncScene = true;
        PhotonNetwork.isLoadLevel = true;
        waitUICoroutine = StartCoroutine(WaitUICoroutine());
    }

    Coroutine waitUICoroutine;

    IEnumerator WaitUICoroutine()
    {
        PhotonNetwork.LocalPlayer.CustomProperties["GameReady"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

        text_wait_info.text = GameSettingCtrl.GetLocalizationText("0038");
        text_wait_time.text = "";
        text_wait_time.gameObject.SetActive(true);
        button_wait.SetActive(false);
        SetViewState(ViewState.Wait);
        float timeP = 0f;
        int count = 0;
        yield return new WaitForSeconds(1f);

        while (true)
        {
            try
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
                {
                    text_wait_info.text = GameSettingCtrl.GetLocalizationText("0034");
                    break;
                }
            }
            catch (Exception)
            {
                text_wait_info.text = GameSettingCtrl.GetLocalizationText("0034");
                break;
            }
            count = 0;
            foreach (var player in PhotonNetwork.CurrentRoom.Players.OrderBy(i => i.Value.ActorNumber))
            {
                if (!((bool)player.Value.CustomProperties["GameReady"]))
                {
                    break;
                }
                count++;
                if (count == 2)
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        StartLoadLevel();
                    }

                    SetViewState(ViewState.None);
                    yield break;
                }
            }

            timeP += Time.unscaledDeltaTime;
            if (timeP >= 10)
            {
                break;
            }
            text_wait_time.text = ((int)timeP).ToString();
            yield return null;
        }

        text_wait_time.gameObject.SetActive(false);
        button_wait.SetActive(true);
        yield return null;

        while (true)
        {
            try
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
                {
                    text_wait_info.text = GameSettingCtrl.GetLocalizationText("0034");
                    yield break;
                }
            }
            catch (Exception)
            {
                text_wait_info.text = GameSettingCtrl.GetLocalizationText("0034");
                yield break;
            }
            count = 0;
            foreach (var player in PhotonNetwork.CurrentRoom.Players.OrderBy(i => i.Value.ActorNumber))
            {
                if (!((bool)player.Value.CustomProperties["GameReady"]))
                {
                    break;
                }
                count++;
                if (count == 2)
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        StartLoadLevel();
                    }

                    SetViewState(ViewState.None);
                    yield break;
                }
            }

            yield return null;
        }
    }
    public void AddLoadLobbyEvent(Action _action_lobby)
    {
        action_lobby += _action_lobby;
    }

    public void AddReplayEvent(Action _action_replay)
    {
        action_replay += _action_replay;
    }

    public void AddMenuEvent(Action<bool> _action_menu)
    {
        action_menu += _action_menu;
    }

    public void AddDisconnectEvent(Action _action_dis)
    {
        action_disconnect += _action_dis;
    }

    public void SetInteractable(bool _isInteractable)
    {
        isInteractable = _isInteractable;
        if (!isInteractable)
        {
            SetViewState(ViewState.None);
        }
    }

    IEnumerator DelayActive()
    {
        yield return null;
        playTime = 0;
        if (gameDataManager.playType == GameDataManager.PlayType.Multi && SceneManager.GetActiveScene().name != "Lobby" && SceneManager.GetActiveScene().name != "Scene_Custom_Q")
        {
            if (Application.internetReachability == NetworkReachability.NotReachable || !PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                if (action_lobby != null)
                {
                    action_lobby.Invoke();
                }

               /* if (leaderBoard.gameObject.activeSelf)
                {
                    //leaderBoard.SetVIewUI(false);
                }*/

                if (PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.IsSyncScene = false;
                    PhotonNetwork.Disconnect();
                }

                SetViewState(ViewState.None);
                gameDataManager.playType = GameDataManager.PlayType.None;

                isDisCheck = false;

                startViewState = StartViewState.Disconnect;
                SceneManager.LoadScene(0);

                yield break;
            }
            else
            {
                StartDisconnectCheck();
            }
        }

        while (Camera.main == null)
        {
            yield return null;
        }

        InitSceneCamera();

#if PICO_PLATFORM
        bgmCheckTime = 0f;
        list_audioSource_bgm = new List<AudioSource>();
        AudioMixerGroup audioMixerGroup = GameSettingCtrl.GetAudioMixerGroup("BGM");
        AudioSource[] bgms = FindObjectsOfType<AudioSource>();

        if (GameDataManager.instance.gameType == GameDataManager.GameType.Boxing && GameDataManager.mode == 1)
        {
            for (int i = 0; i < bgms.Length; i++)
            {
                if (bgms[i].gameObject.name == "Loader")
                {
                    list_audioSource_bgm.Add(bgms[i]);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < bgms.Length; i++)
            {
                if (bgms[i].outputAudioMixerGroup == audioMixerGroup)
                {
                    list_audioSource_bgm.Add(bgms[i]);
                }
            }
        }

#endif
        renderOriginCam.Init(camera_main.transform);

        XRInteractionManager interactionManager = GameObject.FindObjectOfType<XRInteractionManager>();
        for (int i = 0; i < 2; i++)
        {
            xRRayInteractors[i].interactionManager = interactionManager;
        }

        if (gameDataManager.gameType == GameDataManager.GameType.Baseball)
        {
            if (gameDataManager.playType == GameDataManager.PlayType.Multi || (gameDataManager.playType == GameDataManager.PlayType.Single && GameDataManager.mode != 1))
            {
                profileCapture.ShotImages_Baseball();
            }
        }

        SetCloneGrid();

        isInteractable = true;

        yield return new WaitForSeconds(3f);

        if (gameDataManager.playType == GameDataManager.PlayType.Multi)
        {
            PhotonNetwork.LocalPlayer.CustomProperties["GameReady"] = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.CustomProperties[PhotonNetwork.CurrentSceneProperty] = "";
                PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
            }
            PhotonNetwork.IsSyncScene = false;
            PhotonNetwork.isLoadLevel = false;
            SceneManagerHelper.ActiveSceneName = "";
        }

        if (SceneManager.GetActiveScene().name == "Lobby" || SceneManager.GetActiveScene().name == "Scene_Custom_Q")
        {
            yield break;
        }

        if (gameDataManager.playType == GameDataManager.PlayType.Single && GameDataManager.singleManager != null)
        {
            PlayerPrefs.SetInt("isDisCheck", 1);
        }

        if (gameDataManager.playType != GameDataManager.PlayType.Multi)
        {
            yield break;
        }

        yield return new WaitForSeconds(30f);

        isDisCheck = true;
    }

    IEnumerator AddPxrManager()
    {
        while (FindObjectOfType<XROrigin>() == null)
        {
            yield return null;
        }
#if !UNITY_EDITOR && PICO_PLATFORM
        var Xr_Rig = FindObjectOfType<XROrigin>().gameObject;
        var PxrManager = Xr_Rig.AddComponent<Unity.XR.PXR.PXR_Manager>();
        PxrManager.foveationLevel = Unity.XR.PXR.FoveationLevel.Low;
#endif
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!CheckLoadScene(scene.name))
        {
            return;
        }

        if (mode == LoadSceneMode.Single)
        {
            isOverlay = false;
            camera_overlay.enabled = false;
            renderer_grid.gameObject.SetActive(false);

            if (scene.name == "Lobby")
            {
                gridSize = 3f;
                SetGridSize(gridSize);

#if !TEST_ACCOUNT && !(QIYU_ACCOUNT && UNITY_EDITOR) && !(PICO_PLATFORM && UNITY_EDITOR)
                LobbyUIManager.GetInstance.medalView.Init();
                StartCoroutine(LobbyUIManager.GetInstance.achievement.SetAchieve());
#endif
            }
            else
            {
                buttons_menu[0].transform.localPosition = new Vector3(0.19f, 0f, 0f);
                buttons_menu[1].gameObject.SetActive(true);
                buttons_menu[2].transform.localPosition = new Vector3(-0.19f, 0f, 0f);
            }

            if (delayActiveCoroutine != null)
            {
                StopCoroutine(delayActiveCoroutine);
            }
            delayActiveCoroutine = StartCoroutine(DelayActive());
#if PICO_PLATFORM
            StartCoroutine(AddPxrManager());
#endif
        }
    }

    Coroutine delayActiveCoroutine;

    private void StopDisconnectCheck()
    {
        if (checkDisconnect != null)
        {
            StopCoroutine(checkDisconnect);
        }

        if (delayActiveCoroutine != null)
        {
            StopCoroutine(delayActiveCoroutine);
        }

        isDisCheck = false;
        PlayerPrefs.SetInt("isDisCheck", 0);
    }

    private void OnSceneUnloaded(Scene current)
    {
        profileCapture.StopImages();
        SetViewState(ViewState.None);

        isInteractable = false;
        isResultUpload = false;

        action_lobby = null;
        action_replay = null;
        action_menu = null;
        action_disconnect = null;

        GameSettingCtrl.ClearHandChangedEvent();
        GameSettingCtrl.ClearLocalizationChangedEvent();
    }

    private void SetOverlay(bool _isOverlay)
    {
        if (isOverlay == _isOverlay)
        {
            if (!isSetFirst)
            {
                return;
            }
        }

        isOverlay = _isOverlay;

        if (isOverlay)
        {
            camera_overlay.tag = "MainCamera";
            inputModule.uiCamera = camera_overlay;
            if (camera_main != null)
            {
                camera_main.tag = "Untagged";
                camera_main.enabled = false;
                camera_overlay.enabled = true;
                renderer_grid.gameObject.SetActive(true);
                grid_clone.SetActive(false);
                SetKeepObjectEnable(false);
            }
        }
        else
        {
            if (camera_main != null)
            {
                camera_overlay.tag = "Untagged";
                camera_main.tag = "MainCamera";
                inputModule.uiCamera = camera_main;
                camera_main.enabled = true;
                camera_overlay.enabled = false;
                renderer_grid.gameObject.SetActive(false);
                grid_clone.SetActive(true);
                SetKeepObjectEnable(true);
            }
        }
    }

    public void SetViewState(ViewState setViewState)
    {
        if (viewState == setViewState)
        {
            return;
        }

        if (setViewState == ViewState.None && viewState_keep == ViewState.Result && viewState == ViewState.Match)
        {
            setViewState = ViewState.Result;
        }

        viewState_keep = viewState;

        if (setViewState == ViewState.Menu)
        {
            if (action_menu != null)
            {
                action_menu.Invoke(true);
            }
        }
        else if (viewState == ViewState.Menu)
        {
            if (action_menu != null)
            {
                action_menu.Invoke(false);
            }
        }

        if (viewState == ViewState.Setting)
        {
            if (LobbyUIManager.IsInstance)
            {
                LobbyUIManager.GetInstance.gameSetting.View();
            }
        }

        viewState = setViewState;

        SetHandState(0);
        SetHandState(1);

        gameSetting.gameObject.SetActive(false);

        if (viewState != ViewState.None)
        {
            if (controllers_main != null)
            {
                for (int i = 0; i < controllers_main.Length; i++)
                {
                    controllers_main[i].enableInputActions = false;
                }
            }

            if (controllers_overlay != null)
            {
                for (int i = 0; i < controllers_overlay.Length; i++)
                {
                    controllers_overlay[i].enableInputActions = true;
                }
            }

            if (!isViewCenter)
            {
                SetOverlay(true);
            }

            Vector3 ui_pos = camera_overlay.transform.forward;
            ui_pos.y = 0f;
            ui_pos = ui_pos.normalized * 0.5f;
            ui_pos.y = -0.3f;

            targetPos = camera_overlay.transform.position;

            switch (viewState)
            {
                case ViewState.Menu:
                    {
                        windowTr_menu.position = camera_overlay.transform.position + camera_overlay.transform.forward * 0.5f + camera_overlay.transform.up * 0.5f;
                        targetRot = Quaternion.LookRotation(camera_overlay.transform.position - windowTr_menu.position);
                        windowTr_menu.rotation = targetRot;

                        if (SceneManager.GetActiveScene().name == "Lobby")
                        {
                            buttons_menu[0].transform.localPosition = new Vector3(0.135f, 0f, 0f);
                            buttons_menu[1].gameObject.SetActive(false);
                            buttons_menu[2].transform.localPosition = new Vector3(-0.135f, 0f, 0f);
                            buttons_menu[2].gameObject.SetActive(true);
                        }
                        else
                        {
                            SetLocaliztion();
                            gameSetting.View();
                            buttons_menu[0].transform.localPosition = new Vector3(0.19f, 0f, 0f);
                            buttons_menu[1].gameObject.SetActive(true);
                            buttons_menu[2].transform.localPosition = new Vector3(-0.19f, 0f, 0f);
                            buttons_menu[2].gameObject.SetActive(true);
                        }

                        windowTr_match.gameObject.SetActive(false);
                        anim_result.gameObject.SetActive(false);
                        windowTr_wait.gameObject.SetActive(false);
                        windowTr_menu.gameObject.SetActive(true);
                        SetHotKeyList(StateHotKeyList.Public_Menu);
                    }
                    break;
                case ViewState.Result:
                    {
                        anim_result.transform.position = camera_overlay.transform.position + camera_overlay.transform.forward * 0.5f + camera_overlay.transform.up * 0.5f;
                        targetRot = Quaternion.LookRotation(camera_overlay.transform.position - anim_result.transform.position);
                        anim_result.transform.rotation = targetRot;

                        windowTr_match.gameObject.SetActive(false);
                        windowTr_menu.gameObject.SetActive(false);
                        windowTr_wait.gameObject.SetActive(false);
                        anim_result.gameObject.SetActive(true);
                        SetHotKeyList(StateHotKeyList.Public_Result);
                    }
                    break;
                case ViewState.Wait:
                    {
                        windowTr_wait.position = camera_overlay.transform.position + camera_overlay.transform.forward * 0.5f + camera_overlay.transform.up * 0.5f;
                        targetRot = Quaternion.LookRotation(camera_overlay.transform.position - windowTr_wait.position);
                        windowTr_wait.rotation = targetRot;

                        anim_result.gameObject.SetActive(false);
                        windowTr_menu.gameObject.SetActive(false);
                        windowTr_match.gameObject.SetActive(false);
                        windowTr_wait.gameObject.SetActive(true);
                    }
                    break;
                case ViewState.Match:
                    {
                        windowTr_match.position = camera_overlay.transform.position + camera_overlay.transform.forward * 0.5f + camera_overlay.transform.up * 0.5f;
                        targetRot = Quaternion.LookRotation(camera_overlay.transform.position - windowTr_match.position);
                        windowTr_match.rotation = targetRot;

                        anim_result.gameObject.SetActive(false);
                        windowTr_menu.gameObject.SetActive(false);
                        windowTr_wait.gameObject.SetActive(false);
                        windowTr_match.gameObject.SetActive(true);
                    }
                    break;
                case ViewState.Setting:
                    {
                        gameSetting.transform.position = camera_overlay.transform.position + camera_overlay.transform.forward * 0.5f + camera_overlay.transform.up * 0.5f;
                        Vector3 dir = camera_overlay.transform.position - gameSetting.transform.position;
                        dir.y = 0f;
                        targetRot = Quaternion.LookRotation(dir);
                        gameSetting.transform.rotation = targetRot;

                        SetLocaliztion();
                        gameSetting.View();

                        buttons_menu[0].transform.localPosition = new Vector3(0f, 0f, 0f);
                        buttons_menu[1].gameObject.SetActive(false);
                        buttons_menu[2].gameObject.SetActive(false);

                        windowTr_match.gameObject.SetActive(false);
                        anim_result.gameObject.SetActive(false);
                        windowTr_wait.gameObject.SetActive(false);
                        windowTr_menu.gameObject.SetActive(true);
                    }
                    break;
            }
        }
        else
        {
            if (controllers_overlay != null)
            {
                for (int i = 0; i < controllers_overlay.Length; i++)
                {
                    controllers_overlay[i].enableInputActions = false;
                }
            }

            if (controllers_main != null)
            {
                for (int i = 0; i < controllers_main.Length; i++)
                {
                    controllers_main[i].enableInputActions = true;
                }
            }

            EndResultAnim();

            windowTr_menu.gameObject.SetActive(false);
            windowTr_wait.gameObject.SetActive(false);
            windowTr_match.gameObject.SetActive(false);

            if (!isViewCenter)
            {
                SetOverlay(false);
            }

            SetHotKeyList(StateHotKeyList.Public_Close);
        }
    }

    public void SetViewCenter(bool isActive)
    {
        if (isViewCenter == isActive)
        {
            if (!isSetFirst)
            {
                return;
            }
        }

        isViewCenter = isActive;
        if (isViewCenter)
        {
            if (isSetFirst || viewState == ViewState.None)
            {
                SetOverlay(true);
            }

            renderer_arrow.gameObject.SetActive(true);
        }
        else
        {
            renderer_arrow.gameObject.SetActive(false);
            if (isSetFirst || viewState == ViewState.None)
            {
                SetOverlay(false);
            }
        }
    }

    public void Click_InputKey(string key)
    {
        switch (key)
        {
            case "Lobby":
                {
                    StopDisconnectCheck();

                    if (action_lobby != null)
                    {
                        action_lobby.Invoke();
                    }

                    if (delayActiveCoroutine != null)
                    {
                        StopCoroutine(delayActiveCoroutine);
                    }

                   /* if (leaderBoard.gameObject.activeSelf)
                    {
                        //leaderBoard.SetVIewUI(false);
                    }*/

                    if (PlayerPrefs.GetInt("isDisCheck", 0) == 1 || isDisCheck)
                    {
                        GameDataManager.score_lose_mine++;
                    }

                    SetViewState(ViewState.None);
                    gameDataManager.playType = GameDataManager.PlayType.None;

                    MeshFadeCtrl.instance.LoadScene("Lobby", 1.5f);
                    if (CustomizeManager.GetInstance != null
                         && SceneManager.GetActiveScene().name == "Scene_Custom_Q")
                        CustomizeManager.GetInstance.Upload_UserData();
                }
                break;
            case "Exit":
                {
                  
                    // 캐릭터 데이터 저장 해야함 
                    if (SceneManager.GetActiveScene().name != "Lobby")
                        MeshFadeCtrl.instance.LoadScene("Lobby", 1.5f);
                    else
                        Application.Quit();
                }
                break;
        }

        if (!isInteractable)
        {
            return;
        }

        if (viewState == ViewState.Match)
        {
            return;
        }

        switch (key)
        {
            case "Play":
                {
                    if (viewState == ViewState.Setting || viewState == ViewState.Menu)
                    {
                        SetViewState(ViewState.None);
                    }
                }
                break;
            case "Setting":
                {
                    if (viewState == ViewState.Setting)
                    {
                        SetViewState(ViewState.None);
                    }
                    else if (viewState == ViewState.None)
                    {
                        SetViewState(ViewState.Setting);
                    }
                }
                break;
            case "Replay":
                {
                    /*if (leaderBoard.gameObject.activeSelf)
                    {
                        //leaderBoard.SetVIewUI(false);
                    }*/

                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        OpenWaitUI();
                    }
                    else
                    {
                        StartLoadLevel();
                    }
                }
                break;
            case "Menu":
                {
                    if (viewState == ViewState.Setting)
                    {
                        break;
                    }
                    if (viewState == ViewState.Menu)
                    {
                        SetViewState(ViewState.None);
                    }
                    else if (viewState == ViewState.None)
                    {
                        SetViewState(ViewState.Menu);
                    }
                }
                break;
            case "Next": // 멀티에는 나오면 안됨.
                {
                    /*if (leaderBoard.gameObject.activeSelf)
                    {
                        //leaderBoard.SetVIewUI(false);
                    }*/

                    SetViewState(ViewState.None);

                    GameDataManager.level += 1;
                    GameDataManager.UserInfo userInfo = gameDataManager.userInfos[1];
                    userInfo.nick = "Level" + GameDataManager.level;
                    gameDataManager.userInfos[1] = userInfo;

                    Invoke("StartLoadLevel", 1f);
                }
                break;
        }
    }

    public void SetGridSize(float sizeP)
    {
        gridSize = sizeP;
        renderer_grid.transform.localScale = new Vector3(gridSize, 2f, gridSize);

        if (grid_clone != null)
        {
            grid_clone.transform.localScale = renderer_grid.transform.localScale;
        }

        renderer_grid.sharedMaterial.SetVector("_Tiling", new Vector2(8f * gridSize, 5f));
        renderer_grid.sharedMaterial.SetVector("_TrackerPos00", Vector3.zero);
        renderer_grid.sharedMaterial.SetVector("_TrackerPos01", Vector3.zero);
        renderer_grid.sharedMaterial.SetVector("_TrackerPos02", Vector3.zero);
        if (sizeP > 1.5f)
        {
            distP_pause = gridSize * 0.5f;
            distP_min = distP_pause - 0.4f;
            distP_min *= distP_min;
            distP_max = distP_pause - 0.35f;
            distP_max *= distP_max;
            distP_pause *= distP_pause;
        }
        else
        {
            distP_pause = gridSize * 0.5f;
            distP_min = distP_pause - 0.1f;
            distP_min *= distP_min;
            distP_max = distP_pause - 0.05f;
            distP_max *= distP_max;
            distP_pause *= distP_pause;
        }
    }

    void SetCloneGrid()
    {
        grid_clone = Instantiate(renderer_grid.gameObject,camera_main.transform.parent);
        grid_clone.transform.localPosition = Vector3.zero;
        grid_clone.transform.localRotation = Quaternion.identity;
        grid_clone.transform.localScale = renderer_grid.transform.localScale;
        grid_clone.layer = 0;
        grid_clone.SetActive(true);
    }

    public void UpdateMatchRoomInfo()
    {
        if (PhotonNetwork.IsSyncScene && PhotonNetwork.isLoadLevel)
        {
            return;
        }
        Debug.Log("UpdateMatchRoomInfo()");

        int count = 0;
        GameDataManager.instance.userInfos.Clear();

        foreach (var player in PhotonNetwork.CurrentRoom.Players.OrderBy(i => i.Value.ActorNumber))
        {
            GameDataManager.UserInfo userInfo = new GameDataManager.UserInfo();

            userInfo.id = (string)player.Value.CustomProperties["AppnoriID"];
            userInfo.nick = player.Value.NickName;
            userInfo.customModelData = JsonUtility.FromJson<CustomModelData>((string)player.Value.CustomProperties["ModelData"]);

            GameDataManager.instance.userInfos.Add(userInfo);

#if TEST_ACCOUNT
            if (GameDataManager.instance.userInfos[count].id == GameDataManager.instance.userInfo_mine.id)
            {
                userInfos[count].text_nick_match.text = "Player01";
            }
            else
            {
                userInfos[count].text_nick_match.text = "Player02";
            }
#else
            userInfos[count].text_nick_match.text = userInfo.id;
#endif
            userInfos[count].text_info_match.text = ((int)player.Value.CustomProperties["Win"]).ToString() + "W " + ((int)player.Value.CustomProperties["Lose"]).ToString() + "L";
            count++;
        }

        if (count == 1)
        {
            userInfos[1].text_nick_match.text = "Waiting...";
            userInfos[1].text_info_match.text = "";

            profileCapture.PlayImages(new string[1] { GameDataManager.instance.userInfos[0].id }, ProfileCaptureCtrl.ShotState.Multi);

            GameDataManager.UserInfo userInfo = new GameDataManager.UserInfo();
            userInfo.id = "AI";
            userInfo.nick = userInfo.id;
            GameDataManager.instance.userInfos.Add(userInfo);
        }
        else
        {
            Play_Voice(6);
            profileCapture.PlayImages(new string[2] { GameDataManager.instance.userInfos[0].id, GameDataManager.instance.userInfos[1].id }, ProfileCaptureCtrl.ShotState.Multi);
        }
    }

    public void SetMatchTime(string setText)
    {
        text_matchTime.text = setText;
    }

    public void EndResultAnim()
    {
        if (viewState_keep != ViewState.Result)
        {
            return;
        }

        if (endResultCoroutine != null)
        {
            StopCoroutine(endResultCoroutine);
        }

        anim_result.SetTrigger("OnClose");
        endResultCoroutine = StartCoroutine(EndResultCoroutine());
    }

    Coroutine endResultCoroutine;

    IEnumerator EndResultCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        while (1f > anim_result.GetCurrentAnimatorStateInfo(0).normalizedTime)
        {
            yield return null;
        }
        anim_result.gameObject.SetActive(false);
    }

    void SetHandState(int index)
    {
        switch (viewState)
        {
            case ViewState.None:
                {
                    handInfos[index].anim.gameObject.SetActive(false);
                    handInfos[index].line_transform.gameObject.SetActive(false);
                    handInfos[index].ball_transform.gameObject.SetActive(false);
                }
                break;
            case ViewState.Menu:
            case ViewState.Wait:
            case ViewState.Result:
            case ViewState.Setting:
                {
                    handInfos[index].anim.gameObject.SetActive(true);
                    handInfos[index].anim.SetBool("IsPoint", true);
                    handInfos[index].line_transform.gameObject.SetActive(true);
                    handInfos[index].ball_transform.gameObject.SetActive(true);
                }
                break;
            case ViewState.Match:
                {
                    handInfos[index].anim.gameObject.SetActive(true);
                    handInfos[index].anim.SetBool("IsPoint", false);
                    handInfos[index].line_transform.gameObject.SetActive(false);
                    handInfos[index].ball_transform.gameObject.SetActive(false);
                }
                break;
        }
    }


    public void StartLoadLevel()
    {
#if APPNORI_LOCK
        if (!GameDataManager.instance.isUnlock)
        {
            return;
        }
#endif

        switch (gameDataManager.gameType)
        {
            case GameDataManager.GameType.Bowling:
                {
                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel("Scene_Game_Bowling");
                    }
                    else
                    {
                        SceneManager.LoadScene("Scene_Game_Bowling");
                    }
                }
                break;
            case GameDataManager.GameType.Archery:
                {
                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel("Scene_Game_Archery");
                    }
                    else
                    {
                        SceneManager.LoadScene("Scene_Game_Archery");
                    }
                }
                break;
            case GameDataManager.GameType.Basketball:
                {
                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel("Scene_Game_BasketBall");
                    }
                    else
                    {
                        SceneManager.LoadScene("Scene_Game_BasketBall");
                    }
                }
                break;
            case GameDataManager.GameType.Badminton:
                {
                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel("Scene_Game_Badminton");
                    }
                    else
                    {
                        SceneManager.LoadScene("Scene_Game_Badminton");
                    }
                }
                break;
            case GameDataManager.GameType.Billiards:
                {
                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel("Scene_Game_Billiards");
                    }
                    else
                    {
                        SceneManager.LoadScene("Scene_Game_Billiards");
                    }
                }
                break;
            case GameDataManager.GameType.Darts:
                {
                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel("Scene_Game_Darts");
                    }
                    else
                    {
                        SceneManager.LoadScene("Scene_Game_Darts");
                    }
                }
                break;
            case GameDataManager.GameType.TableTennis:
                {
                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel("Scene_Game_PingPong");
                    }
                    else
                    {
                        SceneManager.LoadScene("Scene_Game_PingPong");
                    }
                }
                break;
            case GameDataManager.GameType.Boxing:
                {
                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel("Scene_Game_Boxing_MT");
                    }
                    else
                    {
                        switch (GameDataManager.mode)
                        {
                            case 2:
                                {
                                    SceneManager.LoadScene("Scene_Game_Boxing");
                                }
                                break;
                            case 3:
                                {
                                    SceneManager.LoadScene("Scene_Game_Boxing_HB");
                                }
                                break;
                            default:
                                {
                                    SceneManager.LoadScene("Scene_Game_Boxing_AI");
                                }
                                break;
                        }
                    }
                }
                break;
            case GameDataManager.GameType.Golf:
                {
                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel("Scene_Game_Golf");
                    }
                    else
                    {
                        SceneManager.LoadScene("Scene_Game_Golf");
                    }
                }
                break;
            case GameDataManager.GameType.Baseball:
                {
                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel("Scene_Game_BaseBall");
                    }
                    else
                    {
                        SceneManager.LoadScene("Scene_Game_BaseBall");
                    }
                }
                break;
            case GameDataManager.GameType.Tennis:
                {
                    if (gameDataManager.playType == GameDataManager.PlayType.Multi)
                    {
                        PhotonNetwork.LoadLevel("Scene_Game_Tennis");
                    }
                    else
                    {
                        SceneManager.LoadScene("Scene_Game_Tennis");
                    }
                }
                break;
        }
    }

    public void Play(int index, bool isLoop = false)
    {
        audioSource_effect.loop = isLoop;
        audioSource_effect.clip = audios_effect[index];
        audioSource_effect.Play();
    }
    public void Stop()
    {
        audioSource_effect.Stop();
    }

    public void Play_Voice(int index, bool isLoop = false)
    {
        audioSource_voice.loop = isLoop;
        switch (GameSettingCtrl.GetLanguageState())
        {
            case LanguageState.schinese:
                {
                    audioSource_voice.clip = audios_voice_cn[index];
                }
                break;
            default:
                {
                    audioSource_voice.clip = audios_voice[index];
                }
                break;
        }
        audioSource_voice.Play();
    }

    public void Stop_Voice()
    {
        audioSource_voice.Stop();
    }

    public void SyncProfileLight(float _time)
    {
        eff_win_light.SetFloat("Time", _time);
    }

    public void SetHaptic(float amplitude = 0.5f, float duration = 0.5f)
    {
        if (controllers_main == null)
        {
            return;
        }

        for (int i = 0; i < controllers_main.Length; i++)
        {
            controllers_main[i].SendHapticImpulse(amplitude, duration);
        }
    }

    public ViewState GetCurrentState()
    {
        return viewState;
    }

    public void SetLocaliztion()
    {
        for (int i = 0; i < dataSlots_localization.Length; i++)
        {
            dataSlots_localization[i].ui.text = GameSettingCtrl.GetLocalizationText(dataSlots_localization[i].id);
        }
    }

    private List<Renderer> keep_object = new List<Renderer>();

    public bool IsOverlay()
    {
        return isOverlay;
    }

    private bool isPause = false;

    private void OnApplicationPause(bool pause)
    {
        if (gameDataManager == null)
        {
            return;
        }
        isPause = pause;

        Debug.Log("OnApplicationPause");
        if (gameDataManager.playType == GameDataManager.PlayType.Multi)
        {
            recorder.IsRecording = (Application.isFocused && !isPause);
        }
    }

    private float saveTimeScale = 1f;

#if OCULUS_PLATFORM
    private void CheckOculusFocus()
    {
        if (OVRManager.hasInputFocus != isFocus)
        {
            isFocus = OVRManager.hasInputFocus;

            if (gameDataManager.playType != GameDataManager.PlayType.Multi)
            {
                if (isFocus)
                {
                    Time.timeScale = saveTimeScale;
                }
                else
                {
                    saveTimeScale = Time.timeScale;
                    Time.timeScale = 0f;
                }
            }

            if (!isFocus && viewState == ViewState.Menu)
            {
                SetViewState(ViewState.None);
                SetOverlay(true);
            }
            else if (!isFocus)
            {
                SetOverlay(true);
            }
            else if (isFocus && viewState == ViewState.None)
            {
                SetOverlay(false);
            }
        }
    }
#endif

    private void OnApplicationFocus(bool focus)
    {
        if (gameDataManager == null)
        {
            return;
        }

        Debug.Log("OnApplicationFocus");
        if (gameDataManager.playType == GameDataManager.PlayType.Multi)
        {
            recorder.IsRecording = (focus && !isPause);
        }
    }

    void SetKeepObjectEnable(bool isEnable)
    {
        if (isEnable)
        {
            uint layer = (1 << 0);
            for (int i = 0; i < keep_object.Count; i++)
            {
                if (keep_object[i] != null)
                {
                    keep_object[i].renderingLayerMask = layer;
                }
            }
            keep_object.Clear();
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "AightBallPool")
            {
                try
                {
                    keep_object.AddRange(GameObject.Find("AightBallPoolGameManager/CuePivot/CueVertical/CueDisplacement").GetComponentsInChildren<Renderer>(true));
                    keep_object.AddRange(GameObject.Find("FreeLeftHand").GetComponentsInChildren<Renderer>(true));
                    keep_object.AddRange(GameObject.Find("FreeRightHand").GetComponentsInChildren<Renderer>(true));
                }
                catch
                {
                    return;
                }
            }
            else
            {
                for (int i = 0; i < controllers_main.Length; i++)
                {
                    keep_object.AddRange(controllers_main[i].GetComponentsInChildren<Renderer>(true));
                }
            }

            for (int i = 0; i < keep_object.Count; i++)
            {
                if (keep_object[i] != null)
                {
                    keep_object[i].renderingLayerMask = 0;
                }
            }

        }
    }

    void InitSceneCamera()
    {
        camera_main = Camera.main;
        controllers_main = camera_main.transform.parent.GetComponentsInChildren<XRController>();
    }

    bool CheckLoadScene(string sceneName)
    {
        if (sceneName == "Lobby" || sceneName == "Scene_Custom_Q"
            || sceneName == "Scene_Game_Bowling" || sceneName == "Scene_Game_Archery" || sceneName == "Scene_Game_BasketBall"
            || sceneName == "Scene_Game_Badminton" || sceneName == "Scene_Game_Billiards" || sceneName == "Scene_Game_Darts"
            || sceneName == "Scene_Game_PingPong" || sceneName == "Scene_Game_Boxing" || sceneName == "Scene_Game_Boxing_HB"
            || sceneName == "Scene_Game_Boxing_AI" || sceneName == "Scene_Game_Boxing_MT" || sceneName == "Scene_Game_Golf"
            || sceneName == "Scene_Game_BaseBall" || sceneName == "Scene_Game_Tennis"
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void ChangeSceneInit(Scene scene, LoadSceneMode mode)
    {
      /*  if(scene.name == "Lobby")
            StartCoroutine(CheckChangeSence());*/

        
       
    }

    private IEnumerator CheckChangeSence()
    {

       

        AsyncOperation async = SceneManager.LoadSceneAsync("Lobby");
        

        yield return new WaitForSeconds(1f);
       
       /* if (eventSystem != null)
        {

            Debug.LogError("destroy");
            EventSystem ob = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            if (ob != eventSystem)
            {
                Destroy(ob);
            }
        }*/
        // Do something.   

    }
}
