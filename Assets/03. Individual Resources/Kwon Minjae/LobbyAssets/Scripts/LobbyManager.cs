using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using NetworkManagement;
using Billiards;
using System.Linq;
using UnityEngine.SceneManagement;
using Appnori.Util;





namespace MJ
{
    public enum LobbyViewState
    {
        Start, GameState, Single, Multi, Friend, Point_M, Point_F, Setting, Info, Null, InputCode, GameType, PlayType
            ,Store,SettingBackGround
    }

    public enum InfoState
    {
        Buttons, Rules, Special, Genealogy, Money, Refill, Ranking
    }

    

    public partial class LobbyManager : MonoBehaviour
    {

        public static LobbyManager instance;

        // 게임 선택 변수
        [SerializeField]
        private bool isPocket = false;
        [SerializeField]
        private bool isCarom3 = false;
        [SerializeField]
        private bool isCarom4 = false;

        [SerializeField]
        private bool isSingle = false;

       
        public int Level { set; get; }

        [SerializeField]
        private LevelLoader Loader;

        [SerializeField] private Animator[] hands;
        public Text text_connectInfo;
        public Text[] texts_player_nick;
        public CustomModelSettingCtrl[] customModelData_player;
        //public CharacterCtrl[] charCtrls_player;
        public RawImage[] images_player;
        public Texture[] textures_player;

        // Start is called before the first frame update
        public Animator anim_lobby;

        public LobbyViewState currenLobbyState = LobbyViewState.Null;
        public InfoState infoState = InfoState.Buttons;

        public Text text_multi_title;

        public Sprite[] sprites_info_page;
        public Image image_info_page;
        public Text text_info_page;
        private int info_page_num = 1;

        //public CharacterCtrl[] charCtrls_single;

        public Button[] singleStateButtons = new Button[5];
        public Button[] multiStateButtons = new Button[4];
        public Button[] gameTypes = new Button[3];
        public Button[] playTypes = new Button[3];
        public List<Button> purchaseGold = new List<Button>();
        public List<Button> purchaseDia = new List<Button>();
        public List<Button> purchaseBackGround = new List<Button>();
        public List<Button> infoButtons = new List<Button>();
        public Button infoBackButton;

        public bool isOnFriendList = false;
        public GameObject friendListUI;

        public GameObject wheelGO;
        //public bool isLoadingPage = true;

        public GameObject bonusUI;

        public Text text_singleInfo;

        //Multi Friends
        private int codeSize_min = 4;
        private int codeSize_max = 4;
        private string inputString = "";

        public Text text_code;

        public GameObject[] infoObject;

        [SerializeField] private GameObject DisableUI;
        [SerializeField] private List<GameObject> Rules;
        

        // Start is called before the first frame update

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        void Start()
        {
            isOnFriendList = false;
            friendListUI.SetActive(isOnFriendList);
            SetPointHand();
            StartPasswordUI();
            initButton();
           
            /*if (PublicGameUIManager.GetInstance)
            {


            }*/
        }

       /* private void Update()
        {
        }*/

        Coroutine cor_startLobby;
        public void DelayStart()
        {
            anim_lobby.gameObject.SetActive(true);
            if (cor_startLobby != null)
            {
                StopCoroutine(cor_startLobby);
            }
            cor_startLobby = StartCoroutine(StartLobbyCoroutine());
        }

        IEnumerator StartLobbyCoroutine()
        {
            while (!PlayerInfoManager.instance.isDone)
            {
                yield return null;
            }
            currenLobbyState = LobbyViewState.Null;
            SetView(LobbyViewState.Start);
            //NetworkManager.GetInstance.DisconnectPhotonVoice();
            yield return new WaitForSeconds(1f);
            SoundManager.instance.PlayTitle();
            yield return new WaitForSeconds(1f);
            SoundManager.instance.DelayPlayBGM();
        }

        void DelaySetViewLobbyObject()
        {
            //LeaderBoardCtrl.instance.SetVIewUI(true);
            //SelectCharacterView.instance.SetVIewUI(true);
        }

        public void SetView(LobbyViewState lobbyViewState)
        {
            isOnFriendList = false;
            friendListUI.SetActive(isOnFriendList);

            switch (lobbyViewState)
            {
                case LobbyViewState.Start:
                    {
                        if (currenLobbyState == LobbyViewState.GameState)
                        {
                            Invoke("DelaySetViewLobbyObject", 0.5f);
                            anim_lobby.SetInteger("StateNum", 21);
                        }
                        else if (currenLobbyState == LobbyViewState.Setting)
                        {
                            SoundManager.instance.SaveSetVolume();
                            Invoke("DelaySetViewLobbyObject", 0.5f);
                            anim_lobby.SetInteger("StateNum", 61);
                        }
                        else if (currenLobbyState == LobbyViewState.Info)
                        {
                            Invoke("DelaySetViewLobbyObject", 0.5f);
                            anim_lobby.SetInteger("StateNum", 91);
                        }
                        else if(currenLobbyState == LobbyViewState.Multi) // 멀티에서 뒤로 가기 할시 포톤 연결 해제 해야함 
                        {
                            if (PhotonNetwork.IsConnected)
                                PhotonNetwork.Disconnect();
                            Invoke("DelaySetViewLobbyObject", 0.5f);
                            anim_lobby.SetInteger("StateNum", 52);
                        }
                        else
                        {
                            //if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["GameReady"])
                            //{
                            //    NetworkManager.instance.GameStart();
                            //}
                            anim_lobby.SetInteger("StateNum", 1);
                        }
                        gameTypes[0].interactable = true;
                        gameTypes[1].interactable = true;
                        gameTypes[2].interactable = true;
                        anim_lobby.SetTrigger("OnStart");
                        SoundManager.instance.PlayUISound(1);
                    }
                    break;

                case LobbyViewState.GameType: // 게임 종류 선택 
                    {
                        if (currenLobbyState == LobbyViewState.Start)
                        {
                            StartChanceCheck();
                            return;
                           
                        }
                        else if (currenLobbyState == LobbyViewState.Single)
                        {
                            anim_lobby.SetInteger("StateNum", 32);
                        }
                        else if (currenLobbyState == LobbyViewState.Point_M || currenLobbyState == LobbyViewState.Point_F)
                        {
                            anim_lobby.SetInteger("StateNum", 42);
                        }
                        else if (currenLobbyState == LobbyViewState.InputCode)
                        {
                            anim_lobby.SetInteger("StateNum", 47);
                        }
                        else if (currenLobbyState == LobbyViewState.PlayType)
                        {
                            anim_lobby.SetInteger("StateNum", 15);
                        }
                        else
                        {
                            anim_lobby.SetInteger("StateNum", 52);
                            if (PhotonNetwork.IsConnected)
                                PhotonNetwork.Disconnect();
                        }

                      

                        anim_lobby.SetTrigger("OnStart");
                        SoundManager.instance.PlayUISound(1);
                    }
                    break;
                case LobbyViewState.PlayType: // 플레이 종류 선택 
                    {

                        if (currenLobbyState == LobbyViewState.Single)
                        {
                            anim_lobby.SetInteger("StateNum", 32);
                        }
                        else if (currenLobbyState == LobbyViewState.Multi)
                        {
                            if (PhotonNetwork.IsConnected)
                                PhotonNetwork.Disconnect();
                            anim_lobby.SetInteger("StateNum", 47);
                        }
                        else if(currenLobbyState == LobbyViewState.InputCode)
                        {
                            anim_lobby.SetInteger("StateNum", 52);
                        }
                        else
                        {
                            anim_lobby.SetInteger("StateNum", 100);
                        }
                        playTypes[0].interactable = true;
                        playTypes[1].interactable = true;
                        playTypes[2].interactable = true;
                       

                        anim_lobby.SetTrigger("OnStart");
                        SoundManager.instance.PlayUISound(1);
                    }
                    break;
                case LobbyViewState.GameState:
                    {
                        //LeaderBoardCtrl.instance.SetVIewUI(false);
                        //SelectCharacterView.instance.SetVIewUI(false);
                        if (currenLobbyState == LobbyViewState.Start)
                        {
                            StartChanceCheck();
                            return;
                            //anim_lobby.SetInteger("StateNum", 12);
                        }
                        else if (currenLobbyState == LobbyViewState.Single)
                        {
                            anim_lobby.SetInteger("StateNum", 32);
                        }
                        else if (currenLobbyState == LobbyViewState.Point_M || currenLobbyState == LobbyViewState.Point_F)
                        {
                            anim_lobby.SetInteger("StateNum", 42);
                        }
                        else if (currenLobbyState == LobbyViewState.InputCode)
                        {
                            anim_lobby.SetInteger("StateNum", 47);
                        }
                        else
                        {
                            anim_lobby.SetInteger("StateNum", 52);
                            if (PhotonNetwork.IsConnected)
                                PhotonNetwork.Disconnect();
                        }
                        anim_lobby.SetTrigger("OnStart");
                        SoundManager.instance.PlayUISound(1);
                        //NetworkManager.GetInstance.OutRoom();
                    }
                    break;
                case LobbyViewState.Single:
                    {
                        anim_lobby.SetInteger("StateNum", 23);
                        anim_lobby.SetTrigger("OnStart");

                        /* charCtrls_single[0].SetCharacter(23);
                         charCtrls_single[1].SetCharacter(9);
                         charCtrls_single[2].SetCharacter(20);
                         charCtrls_single[3].SetCharacter(22);
                         charCtrls_single[4].SetCharacter(24);*/
                        //SetSingleInfoText(0);

                        /* long temp_money = (long)PhotonNetwork.LocalPlayer.CustomProperties["Money"];

                         singleStateButtons[4].interactable = (temp_money >= (1000000 * 100));
                         singleStateButtons[3].interactable = (temp_money >= (100000 * 100));
                         singleStateButtons[2].interactable = (temp_money >= (10000 * 100));
                         singleStateButtons[1].interactable = (temp_money >= (1000 * 100));
                         singleStateButtons[0].interactable = (temp_money > 0);*/
                        singleStateButtons[4].interactable = true;
                        singleStateButtons[3].interactable = true;
                        singleStateButtons[2].interactable = true;
                        singleStateButtons[1].interactable = true;
                        singleStateButtons[0].interactable = true;

                        isSingle = true;
                        SoundManager.instance.PlayUISound(1);
                        PhotonDevConnector.instance.SetRoom("Single");
                    }
                    break;
                case LobbyViewState.Multi:
                    {
                        isSingle = false;
                        anim_lobby.SetInteger("StateNum", 45);
                        anim_lobby.SetTrigger("OnStart");
                        text_multi_title.text = "랜덤 대전";
                        PhotonNetwork.LocalPlayer.NickName = GameDataManager.instance.userInfo_mine.nick;
                        texts_player_nick[0].text = PhotonNetwork.LocalPlayer.NickName;
                        texts_player_nick[1].text = "NullPlayer";
                        images_player[1].texture = textures_player[2];

                        //charCtrls_multi[0].SetCharacter(5);
                        //charCtrls_multi[1].SetCharacter(6);
                        SoundManager.instance.PlayUISound(1);
                        //Loader.SetPlayerUIData(0, PhotonNetwork.LocalPlayer.NickName);
                        PhotonDevConnector.instance.SetRoom("Random");
                        OnStartGame();
                        //PhotonNetwork.JoinRandomRoom();
                      /*  NetworkManager.GetInstance.photonMatchState = NetworkManager.PhotonMatchState.Random;
                        NetworkManager.GetInstance.OnOpenMultiRoom();*/
                    }
                    break;
                case LobbyViewState.Friend:
                    {
                        //LeaderBoardCtrl.instance.SetVIewUI(false);
                        //SelectCharacterView.instance.SetVIewUI(false);
                        anim_lobby.SetInteger("StateNum", 48);
                        anim_lobby.SetTrigger("OnStart");
                        text_multi_title.text = "친구 대전";

                        SoundManager.instance.PlayUISound(1);
                        PhotonDevConnector.instance.SetRoom("Friend");
                        PhotonNetwork.LocalPlayer.NickName = GameDataManager.instance.userInfo_mine.nick;
                        texts_player_nick[0].text = PhotonNetwork.LocalPlayer.NickName;
                        texts_player_nick[1].text = "NullPlayer";
                        /*  NetworkManager.GetInstance.photonMatchState = NetworkManager.PhotonMatchState.Friend;
                          NetworkManager.GetInstance.OnOpenMultiRoom();*/

                    }
                    break;
                case LobbyViewState.Point_M:
                case LobbyViewState.Point_F:
                    {
                        anim_lobby.SetInteger("StateNum", 24);
                        anim_lobby.SetTrigger("OnStart");

                        long temp_money = (long)PhotonNetwork.LocalPlayer.CustomProperties["Money"];

                        multiStateButtons[3].interactable = (temp_money >= (1000000 * 100));
                        multiStateButtons[2].interactable = (temp_money >= (100000 * 100));
                        multiStateButtons[1].interactable = (temp_money >= (10000 * 100));
                        multiStateButtons[0].interactable = (temp_money > 0);

                        SoundManager.instance.PlayUISound(1);
                    }
                    break;
                case LobbyViewState.Setting:
                    {
                        anim_lobby.SetInteger("StateNum", 16);
                        anim_lobby.SetTrigger("OnStart");

                        SoundManager.instance.PlayUISound(1);
                    }
                    break;
                case LobbyViewState.Info:
                    {
                        anim_lobby.SetInteger("StateNum", 19);
                        anim_lobby.SetTrigger("OnStart");
                        //info_page_num = 1;
                        //image_info_page.sprite = sprites_info_page[0];
                        //text_info_page.text = info_page_num + " / " + sprites_info_page.Length;
                        SoundManager.instance.PlayUISound(1);
                    }
                    break;
                case LobbyViewState.Null:
                    {
                        anim_lobby.gameObject.SetActive(false);
                        SoundManager.instance.PlayUISound(1);
                    }
                    break;
                case LobbyViewState.InputCode:
                    {
                        anim_lobby.SetInteger("StateNum", 46);
                        anim_lobby.SetTrigger("OnStart");
                        StartPasswordUI();
                        inputString = "";
                        SoundManager.instance.PlayUISound(1);
                    }
                    break;
                case LobbyViewState.Store:
                    {
                        anim_lobby.SetInteger("StateNum", 17);
                        anim_lobby.SetTrigger("OnStart");
                       
                      
                        SoundManager.instance.PlayUISound(1);
                    }
                    break;
               
            }
            currenLobbyState = lobbyViewState;
        }

        void DelaySetFriendList()
        {
            isOnFriendList = true;
            CheckFriendUIState();
        }

        public void CheckFriendUIState()
        {
            if (!isOnFriendList)
            {
                return;
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                friendListUI.SetActive(false);
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                friendListUI.SetActive(true);
            }
        }

        public void Click_Button(int stateNum)
        {
            if (bonusUI.activeSelf)
            {
                return;
            }

            switch (stateNum)
            {
                case 1:
                    {

                        if (infoState == InfoState.Buttons)
                            SetView(LobbyViewState.Start);
                        else
                            Click_Info(0);
                    }
                    break;
                case 2:
                    {
                        //NetworkManager.GetInstance.OutRoom();
                        SetView(LobbyViewState.GameType);
                    }
                    break;
                case 3:
                    {
                        SetView(LobbyViewState.Single);
                    }
                    break;
                case 4:
                    {
                        //NetworkManager.GetInstance.OutRoom();
                        SetView(LobbyViewState.Multi);
                    }
                    break;
                case 5:
                    {
                        //NetworkManager.GetInstance.OutRoom();
                        SetView(LobbyViewState.InputCode);
                    }
                    break;
                case 42:
                    {
                        //NetworkManager.GetInstance.OutRoom();
                        isPocket = true;
                        isCarom3 = false;
                        isCarom4 = false;
                        SetView(LobbyViewState.PlayType);
                    }
                    break;
                case 43:
                    {
                        //NetworkManager.GetInstance.OutRoom();
                        isPocket = false;
                        isCarom3 = true;
                        isCarom4 = false;
                        SetView(LobbyViewState.PlayType);
                    }
                    break;
                case 44:
                    {
                        //NetworkManager.GetInstance.OutRoom();
                        isPocket = false;
                        isCarom3 = false;
                        isCarom4 = true;
                        SetView(LobbyViewState.PlayType);
                    }
                    break;
                case 45:
                    {
                       
                        SetView(LobbyViewState.PlayType);
                    }
                    break;


                case 40:
                    {
                        SetView(LobbyViewState.Point_M);
                    }
                    break;
                case 50:
                case 51:
                case 52:
                case 53:
                    {
                        switch (stateNum)
                        {
                            case 50:
                                {
                                    //NetworkManager.GetInstance.moneyState = 0;
                                }
                                break;
                            case 51:
                                {
                                    //NetworkManager.GetInstance.moneyState = 1;
                                }
                                break;
                            case 52:
                                {
                                    //NetworkManager.GetInstance.moneyState = 2;
                                }
                                break;
                            case 53:
                                {
                                    //NetworkManager.GetInstance.moneyState = 3;
                                }
                                break;

                        }
                        if (currenLobbyState == LobbyViewState.Point_M)
                        {
                            SetView(LobbyViewState.Multi);
                        }
                        else
                        {
                            SetView(LobbyViewState.InputCode);
                        }
                    }
                    break;
                case 11:
                    {
                       // GameManager.instance.StartGame(true, 1);
                    }
                    break;
                case 12:
                    {
                        //GameManager.instance.StartGame(true, 2);
                    }
                    break;
                case 13:
                    {
                       // GameManager.instance.StartGame(true, 3);
                    }
                    break;
                case 14:
                    {
                       // GameManager.instance.StartGame(true, 4);
                    }
                    break;
                case 15:
                    {
                        //GameManager.instance.StartGame(true, 5);
                    }
                    break;
                case 6:
                    {
                        SetView(LobbyViewState.Setting);
                       
                    }
                    break;
              
                case 96:// 상점
                    {
                        SetView(LobbyViewState.Store);
                    }
                    break;
                case 97:// 꾸미기
                    {
                        MeshFadeCtrl.instance.LoadScene("Scene_Custom_Q", 1.1f);
                        //SceneManager.LoadScene("Scene_Custom_Q", LoadSceneMode.Single);
                    }
                    break;

                case 99:
                    {
                        SetView(LobbyViewState.Info);
                    }
                    break;
                case 98:
                    {
                        Application.Quit();
                    }
                    break;

                    
            }
        }

        public void StartSingle(int lv) // 버튼으로 이벤트 등록함
        {
            Level = lv;
            //Debug.LogError(Level);
            OnStartGame();
        }
        public void Click_Info(int key)
        {
            foreach (GameObject i in infoObject)
            {
                i.SetActive(false);
            }

            infoObject[key].SetActive(true);
            infoState = (InfoState)key;
        }

        public void Click_InputKey(string key)
        {
            switch (key)
            {
                case "Exit":
                    {
                        //gameSetting.SaveData();
                        Application.Quit();
                    }
                    break;
                case "Setting":
                    {
                    }
                    break;
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

                case "Back":
                    {
                        if (inputString.Length <= 0)
                        {
                            //사운드
                            return;
                        }
                        inputString = inputString.Remove(inputString.Length - 1);
                        StartPasswordUI();
                    }
                    break;
                case "Enter":
                    {
                        if (inputString.Length < codeSize_min)
                        {
                            //사운드
                            //LobbySoundManager.GetInstance.Play(8);
                            return;
                        }
                        //NetworkManager.GetInstance.roomCode = inputString;
                        SetView(LobbyViewState.Friend);
                        PhotonDevConnector.instance.SetRoomCode(inputString);
                        inputString = "";
                        StopPasswordUI();
                        OnStartGame();
                        //SetLobbyUI(LobbyUIState.Friend);
                    }
                    break;
            }
        }

        public void SetCodeText(string key)
        {
            if (inputString.Length >= codeSize_max)
            {
                //사운드
                return;
            }
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


        Coroutine coroutine_textAnim;
        public void SetInfoPage(bool isNext)
        {
            int last_page = sprites_info_page.Length;

            info_page_num += (isNext ? 1 : -1);
            if (info_page_num > last_page)
            {
                info_page_num = 1;
            }
            else if (1 > info_page_num)
            {
                info_page_num = last_page;
            }

            image_info_page.sprite = sprites_info_page[info_page_num - 1];
            text_info_page.text = info_page_num + " / " + last_page;
        }

        public void PlaySound(int index)
        {
            SoundManager.instance.PlayUISound(index);
        }

        public void StartChanceCheck()
        {
            if (wheelGO.activeInHierarchy) return;
            if (!PlayerInfoManager.instance.isDone) return;

            bool isGetReward = PlayerInfoManager.instance.IsGetReward();

            wheelGO.SetActive(isGetReward);
            PlayerInfoManager.instance.UpdateAccessTimeStamp(isGetReward);

            StartCoroutine(ChanceCheckCoroutine());
        }

        private IEnumerator ChanceCheckCoroutine()
        {
            while (wheelGO.activeInHierarchy)
            {
                yield return null;
            }


            //게임시작
            //....
            anim_lobby.SetInteger("StateNum", 12);
            anim_lobby.SetTrigger("OnStart");
            SoundManager.instance.PlayUISound(1);
            //NetworkManager.GetInstance.OutRoom();
            currenLobbyState = LobbyViewState.GameType;
            Debug.Log("GameStart!!");
            yield return null;
            //wheelGO.GetComponent<WheelController>().blocked = false;
        }


        private void initButton()
        {
            string temp = "";
            temp = "Window_Store/Type_Gold/goldT";
            for (int i = 1; i <= 5; i++)
                purchaseGold.Add(transform.Find($"{temp}{i}/Button").GetComponentInChildren<Button>());
            temp = "Window_Store/Type_Dia/DiaT";
            for (int i = 1; i <= 5; i++)
                purchaseDia.Add(transform.Find($"{temp}{i}/Button").GetComponentInChildren<Button>());
            temp = "Window_Store/Type_Thema/ThemaT";
            for (int i = 1; i <= 3; i++)
                purchaseBackGround.Add(transform.Find($"{temp}{i}/Button").GetComponentInChildren<Button>());
            temp = "Window_Info/Buttons/Button_Rules_AightBall";
            infoButtons.Add(transform.Find($"{temp}").GetComponentInChildren<Button>());
            temp = "Window_Info/Buttons/Button_Rules_3Carom";
            infoButtons.Add(transform.Find($"{temp}").GetComponentInChildren<Button>());
            temp = "Window_Info/Buttons/Button_Rules_4Carom";
            infoButtons.Add(transform.Find($"{temp}").GetComponentInChildren<Button>());
            temp = "Window_Info/Buttons/Button_Money";
            infoButtons.Add(transform.Find($"{temp}").GetComponentInChildren<Button>());
            temp = "Window_Info/Buttons/Button_Custom";
            infoButtons.Add(transform.Find($"{temp}").GetComponentInChildren<Button>());
            temp = "Window_Info/Buttons/Button_Ranking";
            infoButtons.Add(transform.Find($"{temp}").GetComponentInChildren<Button>());
            temp = "Window_Info/Button_Back";
            infoBackButton = transform.Find($"{temp}").GetComponentInChildren<Button>();

            // 다이아로 냥 구매 이벤트 추가
            for (int i = 0; i < 5; i++)
            {
                int index = i;
                //AddListener 가 호출 시점에 이루어짐 그래서  람다식 내부의 처리는 i 최대값으로 들어감
                //즉, for 문에서 선언된 int i 는 임시 클래스의 멤버 변수로 선언 되고 해당 변수를
                //for 문 내에서 참조하여 사용하기 때문에 최종적으로 Log를 찍는 순간에는 for문의 증감문이
                //전부 적용되어 int i 는 3이 되었고 해당 변수를 동일하게 참조하고 있기 때문에
                //3의 값으로만 출력되는 것이다.
                purchaseGold[index].onClick.AddListener(() => Cllck_BuyGold(index));
                
            }
#if PICO_PLATFORM
            // 현금으로 다이아 구매 버튼 이벤트 추가 
            for (int i = 0; i < 5; i++)
            {
                int index = i;
                purchaseDia[index].onClick.AddListener(() => IAPManager.instance.Click_IAPButton(index));
            }
#endif
            // 배경 구매 이벤트 추가
            for (int i = 1; i <= 2; i++)
            {
                int index = i;
                purchaseBackGround[index].onClick.AddListener(() => OnClickBuyMap(index));
            }

            // Add infosButton Event

            for (int i = 0; i < 6; i++)
            {
                int index = i;
                infoButtons[index].onClick.AddListener(() => OnRuleCanvas(index));
            }

            infoBackButton.onClick.AddListener(() => Click_Button(1));
        }
        public void SetThemaButton(int index, bool buyYn)
        {
            purchaseBackGround[index].gameObject.SetActive(!buyYn);
        }
        public void OnRuleCanvas(int ButtonNum)
        {
            Rules[6].SetActive(false);
            Rules[ButtonNum].SetActive(true);
            infoBackButton.onClick.RemoveAllListeners();
            infoBackButton.onClick.AddListener(() => BackRule());


        }

        public void BackRule()
        {
            Rules[6].SetActive(true);

            for (int i = 0; i < 6; i++)
            {
                if (Rules[i].activeSelf)
                    Rules[i].SetActive(false);
            }
            infoBackButton.onClick.RemoveAllListeners();
            infoBackButton.onClick.AddListener(() => Click_Button(1));
        }
        public void Cllck_BuyGold(int index)
        {
            int gold = 0;
            int consumeDia = 0;
            switch(index)
            {
                case 0:
                    gold = 6100000;
                    consumeDia = 1;
                    break;
                case 1:
                    gold = 30000000;
                    consumeDia = 5;
                    break;
                case 2:
                    gold = 100000000;
                    consumeDia = 15;
                    break;
                case 3:
                    gold = 500000000;
                    consumeDia = 70;
                    break;
                case 4:
                    gold = 1000000000;
                    consumeDia = 120;
                    break;
                default:
                    gold = -1;
                    consumeDia = -1;
                    break;
            }

            if(!GameDataManager.instance.CheckICanBuyGold(consumeDia,gold))
            {
                // 구매 못하면
                //Debug.LogError($"{index}구매 안됌");
                if (!DisableUI.activeSelf)
                    StartCoroutine(ShowDisableUI());
            }
            else if (gold == -1)
            {
                Debug.LogError("없는 버튼임");
            }
              

        }

        private IEnumerator ShowDisableUI()
        {
            DisableUI.SetActive(true);
            yield return new WaitForSeconds(2f);
            DisableUI.SetActive(false);
        }

        public void OnClickBuyMap(int index)
        {
            //Debug.LogError($"{index}구매 안됌");
            if (GameDataManager.instance.CheckICanBuyMap(240, index)
                && purchaseBackGround[index].gameObject.activeSelf)
            {
                GameDataManager.instance.ChangeMyResource();
                purchaseBackGround[index].gameObject.SetActive(false);
            }
            else
            {
                if (!DisableUI.activeSelf)
                    StartCoroutine(ShowDisableUI());
            }
        }

        private void OnStartGame()
        {
            //var gameTypeToggle = gameTypeGroup.ActiveToggles().FirstOrDefault();

            var info = new BilliardsMatchInfo()
            {
                userId = "userID",
                userName = "_userName",
                otherId = "otherId",
                otherName = "_otherName",

                //playingType = playingType,
                //gameType = gameType,
                //level = Level,
            };

            /* info.gameType = gameTypeToggle switch
             {
                 Toggle when gameTypeToggle == Pocket => GameType.PocketBilliards,
                 Toggle when gameTypeToggle == Carom3 => GameType.CaromThree,
                 Toggle when gameTypeToggle == Carom4 => GameType.CaromFour,
                 _ => throw new System.Exception("Not matched")
             };*/
           
            info.gameType = true switch
            {
                bool when true == isPocket => GameType.PocketBilliards,
                bool when true == isCarom3 => GameType.CaromThree,
                bool when true == isCarom4 => GameType.CaromFour,
                _ => throw new System.Exception("Not matched")
            };




            info.playingType = isSingle ? PlayType.Single : PlayType.Multi;
           // info.level = Mathf.RoundToInt(Level);
            info.level = Level;




            if (info.playingType == PlayType.Single)
            {
                GameDataManager.instance.startGame("S", System.Guid.NewGuid().ToString());
                LevelLoader.InitializeAndLoad(info);
            }
            else
            {
                PhotonDevConnector.instance.OnConnectAndStart(info);
                //Debug.LogError(PhotonNetwork.ServerAddress);
            }
        }

        public void SetNickPlayer(string nick,int i)
        {
            texts_player_nick[i].text = nick;
            images_player[i].texture = textures_player[i];
            //Debug.LogError(nick);

        }

        private void SetPointHand()
        {
            foreach(Animator handani in hands)
            {
                handani.SetTrigger("OnPoint");
            }
        }
       

      
        
    }

}
