using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.Unity.UtilityScripts;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using ExitGames.Client.Photon;
using SingletonPunBase;

public enum NetEventState
{
    Drop, SelectCard_Drop, SelectCard_Open, Shake, GookJin, GoStop, PresidentEnd, End, Return
}

public class NetEventData
{
    public NetEventState state;
    public int selectIndex;
}

namespace MJ
{

    public class NetworkManager : Singleton<NetworkManager>, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks, IInRoomCallbacks
    {
        private PhotonView pv;
        private Coroutine cor_connecting;
        private Coroutine cor_counting;

        private byte maxPlayers;

        public Text text_connectInfo;
        public Text[] texts_player_nick;
        public Text[] texts_player_record;
        public Text[] texts_player_money;
        public CustomModelSettingCtrl[] customModelData_player;
        //public CharacterCtrl[] charCtrls_player;
        public RawImage[] images_player;
        public Texture[] textures_player;

        public int moneyState = 0;

        public int[] cardList_firstTurn = null;
        public int[] cardList_play = null;

        public NetEventData[] netEventDatas = new NetEventData[50];
        int sendCount_event = 0;
        int readIndex_event = 0;

        public Transform hmdTr;
        public Transform controllerTrL;
        public Transform controllerTrR;

        public Vector3 netUserPosition_head;
        public Vector3 netUserPosition_handL;
        public Vector3 netUserPosition_handR;

        public Quaternion netUserRoation_head;
        public Quaternion netUserRoation_handL;
        public Quaternion netUserRoation_handR;

        public bool isOtherPlayerConnect;

        public float lastReceiveTime = 0f;

        public GameObject inviteMessageUI;
        public Text text_inviteMessageUI;

        private string invite_fromUserId;
        private string invite_realRoomName;

        List<GameObject> friendList = new List<GameObject>();

        public ConnectAndJoin connectAndJoin;

        public Transform content_friendList;

        public GameObject pref_buttonFriend;
        public GameObject pref_LoadingCircle;

        public bool isInit = false;

        private bool isFriendCall = false;

        public string roomCode { get; set; } = "";
        public enum PhotonMatchState { None, Single, Random, Friend }

        public PhotonMatchState photonMatchState = PhotonMatchState.None;

#if TEST_ACCOUNT && SERVER_GLOBAL
    const string PHOTON_REALTIME_APP_ID = "1461c7e4-ff3c-42ac-b727-7c1fa3b5a130";
    const string PHOTON_VOICE_APP_ID = "88e446b6-5bfd-4a1e-bec7-a31049308f60";
#elif TEST_ACCOUNT && SERVER_CHINA
    const string PHOTON_REALTIME_APP_ID = "9d293725-ce9f-4fbf-89ae-d297758592c7";//"e6360d31-7e59-4a4c-9b3c-d23a36148367";
    const string PHOTON_VOICE_APP_ID = "9d293725-ce9f-4fbf-89ae-d297758592c7";//"82adc176-1123-4cda-98e8-5895f79492ee";
#elif SERVER_CHINA
    const string PHOTON_REALTIME_APP_ID = "9d293725-ce9f-4fbf-89ae-d297758592c7";
    const string PHOTON_VOICE_APP_ID = "9d293725-ce9f-4fbf-89ae-d297758592c7";//"bc67eaa0-eded-418c-ba53-c528744c5481";
#else
        const string PHOTON_REALTIME_APP_ID = "1461c7e4-ff3c-42ac-b727-7c1fa3b5a130";
        const string PHOTON_VOICE_APP_ID = "88e446b6-5bfd-4a1e-bec7-a31049308f60";
#endif
        void Start()
        {
            isInit = true;
            pv = GetComponent<PhotonView>();
            PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
            maxPlayers = 2;
            isOtherPlayerConnect = false;
            ClearEventData();

            //StartConnect();
            //StartCoroutine(StartConnect());
        }


        public void StartConnect(string regionCode = "")
        {

            //PhotonNetwork.LocalPlayer.NickName = GameDataManager.instance.userInfo_mine.nick;
            //PhotonNetwork.LocalPlayer.CustomProperties = new ExitGames.Client.Photon.Hashtable();
            //PhotonNetwork.LocalPlayer.CustomProperties.Add("AppnoriID", GameDataManager.instance.userInfo_mine.id);
            ////PhotonNetwork.LocalPlayer.CustomProperties.Add("Ladder", GameDataManager.instance.userInfo_mine.ladder_point);
            ////PhotonNetwork.LocalPlayer.CustomProperties.Add("Grade", GameDataManager.instance.userInfo_mine.grade);
            //PhotonNetwork.LocalPlayer.CustomProperties.Add("GameScore", Newtonsoft.Json.JsonConvert.SerializeObject(GameDataManager.instance.userInfo_mine.gameScores));
            //PhotonNetwork.LocalPlayer.CustomProperties.Add("SeatIndex", -1);

            //PhotonNetwork.LocalPlayer.CustomProperties.Add("ModelData", JsonUtility.ToJson(/*GameDataManager.instance.GetMyCustomModelData()*/    CustomModelSettingCtrl.GetRandomModelData()));

            //PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = PHOTON_REALTIME_APP_ID;
            //PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice = PHOTON_VOICE_APP_ID;

#if DB_TEST
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "test_GoStop";
#else
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = Application.version + "_GoStop";
#endif
#if SERVER_CHINA
            PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
            PhotonNetwork.PhotonServerSettings.AppSettings.Server = "47.94.234.45";
            PhotonNetwork.PhotonServerSettings.AppSettings.Port = 5055;
            //PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
            //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "cn";
            //PhotonNetwork.PhotonServerSettings.AppSettings.Server = "ns.photonengine.cn";
            //PhotonNetwork.PhotonServerSettings.AppSettings.Port = 0;
#else
            PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = regionCode;
            PhotonNetwork.PhotonServerSettings.AppSettings.Server = "";
            PhotonNetwork.PhotonServerSettings.AppSettings.Port = 0;
#endif
            PhotonNetwork.ConnectUsingSettings();
        }


        public IEnumerator ConnectPhoton()
        {

            yield return null;
        }

        void DelayExit()
        {

        }

        void OnLobby()
        {

        }


        public void OutRoom()
        {
            if (cor_connecting != null)
                StopCoroutine(cor_connecting);

            if (cor_counting != null)
                StopCoroutine(cor_counting);

            StopSendUserTransform();

            isOtherPlayerConnect = false;

            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();

                //텍스트 초기화
                for (int i = 0; i < 2; i++)
                {
                    texts_player_nick[i].text = "비접속";
                    texts_player_record[i].text = "";
                    texts_player_money[i].text = "";
                }
                images_player[1].texture = textures_player[2];

                pref_LoadingCircle.SetActive(false);
                text_connectInfo.text = "";

                Debug.Log("OutRoom - 초기화");
            }
            else
            {
                Debug.Log("OutRoom - 초기화 안함");
            }

        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if (photonMatchState == PhotonMatchState.Random)
            {
                for (int i = 0; i < roomList.Count; i++)
                {
                    if (roomList[i].CustomProperties == null || !roomList[i].CustomProperties.ContainsKey("Code") || !roomList[i].IsOpen)
                    {
                        continue;
                    }
                    if ((string)roomList[i].CustomProperties["Code"] == "" && roomList[i].PlayerCount != roomList[i].MaxPlayers)
                    {
                        PhotonNetwork.JoinRoom(roomList[i].Name);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < roomList.Count; i++)
                {
                    if (roomList[i].CustomProperties == null || !roomList[i].CustomProperties.ContainsKey("Code"))
                    {
                        continue;
                    }
                    if ((string)roomList[i].CustomProperties["Code"] == roomCode && roomList[i].PlayerCount != roomList[i].MaxPlayers)
                    {
                        PhotonNetwork.JoinRoom(roomList[i].Name);
                        return;
                    }
                }


            }
            CreateRoom();
        }

        void CreateRoom()
        {
            //PhotonNetwork.AutomaticallySyncScene = true;
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.PublishUserId = true;
            roomOptions.MaxPlayers = 2;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            roomOptions.CleanupCacheOnLeave = false;


            if (photonMatchState == PhotonMatchState.Friend)
            {
                roomOptions.CustomRoomProperties.Add("Code", roomCode);
            }
            else
            {
                roomOptions.CustomRoomProperties.Add("Code", "");
            }
            roomOptions.CustomRoomProperties.Add("GameReady", false);
            roomOptions.CustomRoomProperties.Add("StartTime", PhotonNetwork.ServerTimestamp);
            roomOptions.CustomRoomProperties.Add("RandomLevel", (int)UnityEngine.Random.Range(1f, 3.9999f));

            roomOptions.CustomRoomPropertiesForLobby = new string[] { "Code", "StartTime", "GameReady", "RandomLevel" };

            PhotonNetwork.CreateRoom(null, roomOptions);
        }

        public void OnOpenMultiRoom()
        {
            StartConnect();
        }


        public void OnFriendGameRoom()
        {
            StartConnect();
            //RoomOptions options = new RoomOptions();
            //options.IsVisible = false;
            //options.MaxPlayers = 2;
            //options.PublishUserId = true;
            //options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            //PhotonNetwork.CreateRoom("", options);
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.PublishUserId = true;
            roomOptions.CleanupCacheOnLeave = false;
            roomOptions.BroadcastPropsChangeToAll = true;
            roomOptions.MaxPlayers = maxPlayers;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            PhotonNetwork.CreateRoom("", roomOptions, TypedLobby.Default);
        }

        public override void OnJoinRandomFailed(short sh, string st)
        {
            Debug.Log("OnJoinRandomFailed");

            if (isFriendCall)
            {
                isFriendCall = false;
                return;
            }

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.PublishUserId = true;
            roomOptions.CleanupCacheOnLeave = false;
            roomOptions.BroadcastPropsChangeToAll = true;
            roomOptions.MaxPlayers = maxPlayers;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            PhotonNetwork.CreateRoom("", roomOptions, TypedLobby.Default);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRoomFailed");

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.PublishUserId = true;
            roomOptions.CleanupCacheOnLeave = false;
            roomOptions.BroadcastPropsChangeToAll = true;
            roomOptions.MaxPlayers = maxPlayers;
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            roomOptions.CustomRoomProperties.Add("Code", roomCode);
            PhotonNetwork.CreateRoom("", roomOptions, TypedLobby.Default);

        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom");

            if (isFriendCall)
            {
                isFriendCall = false;
               /* if (GameManager.instance.inGameRoom)
                {
                    GameManager.instance.FriendCall_StopGame();
                    LobbyManager.instance.anim_lobby.gameObject.SetActive(true);
                }*/
                LobbyManager.instance.SetView(LobbyViewState.Friend);
            }

            if (PhotonNetwork.CurrentRoom != null)
            {
                //connectAndJoin.KTConnect();
                Debug.Log("Room Join success. name: " + PhotonNetwork.CurrentRoom.Name);

                //roomName.text = string.Format("RoomName : {0}", PhotonNetwork.CurrentRoom.Name);

                foreach (int key in PhotonNetwork.CurrentRoom.Players.Keys)
                {
                    //text.text = player.NickName;
                }
            }

            CheckPlayerCount();
        }

        IEnumerator ConnectingCoroutine()
        {
            int wait_count = 0;
            text_connectInfo.text = "게임 찾는 중";
            pref_LoadingCircle.SetActive(true);
            while (!isOtherPlayerConnect)
            {
                yield return new WaitForSecondsRealtime(1f);

                wait_count++;
                if (wait_count == 5)
                {
                    text_connectInfo.text = "매칭!!!";
                    pref_LoadingCircle.SetActive(false);
                    wait_count = 4;

                    texts_player_nick[1].text = "광수";
                    texts_player_record[1].text = (90.ToString()) + "승 " + (20.ToString()) + "패";
                    texts_player_money[1].text = string.Format("{0:#,###}원", (500));
                    customModelData_player[1].Init(CustomModelSettingCtrl.GetRandomModelData(), CustomModelViewState.Normal, null, 0.1f);
                    images_player[1].texture = textures_player[1];
                    break;
                }
            }
            yield return new WaitForSecondsRealtime(1f);
            while (wait_count != 1)
            {
                wait_count--;
                text_connectInfo.text = wait_count.ToString();
                yield return new WaitForSecondsRealtime(1f);
            }
            text_connectInfo.text = "게임 시작!";
            yield return new WaitForSecondsRealtime(1f);

            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();

            LobbyManager.instance.Click_Button(UnityEngine.Random.Range(11, 16));
            SoundManager.instance.PlayUISound(1);
            OutRoom();

        }

        IEnumerator ReadyCoroutine(int m_moneyState)
        {
            int wait_count = 4;
            text_connectInfo.text = "매칭!!";
            pref_LoadingCircle.SetActive(false);
            yield return new WaitForSecondsRealtime(1f);
            while (wait_count != 1)
            {
                wait_count--;
                text_connectInfo.text = wait_count.ToString();
                yield return new WaitForSecondsRealtime(1f);
            }
            text_connectInfo.text = "게임 시작!";
            yield return new WaitForSecondsRealtime(1f);

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsVisible = false;
            }
            ClearDeckList();

            if (LobbyManager.instance.currenLobbyState == LobbyViewState.Friend)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    //GameManager.instance.StartGame(false, moneyState);
                }
                else
                {
                    //GameManager.instance.StartGame(false, m_moneyState);
                }
            }
            else
            {
                //GameManager.instance.StartGame(false, Math.Min(moneyState, m_moneyState));
            }

            lastReceiveTime = Time.unscaledTime;
        }

        void CheckPlayerCount()
        {
            LobbyManager.instance.CheckFriendUIState();
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                int playerCount = 0;
                foreach (var player in PhotonNetwork.CurrentRoom.Players)
                {
                    texts_player_nick[playerCount].text = player.Value.NickName;
                    texts_player_record[playerCount].text = ((string)player.Value.CustomProperties["Win"]) + "승 " + ((string)player.Value.CustomProperties["Lose"]) + "패";
                    texts_player_money[playerCount].text = string.Format("{0:#,###}원", ((long)player.Value.CustomProperties["Money"]));
                    customModelData_player[playerCount].Init(CustomModelSettingCtrl.GetRandomModelData(), CustomModelViewState.Normal, null, 0.1f);
                    //charCtrls_player[playerCount].SetCharacter((int)player.Value.CustomProperties["CharIndex"]);
                    images_player[playerCount].texture = textures_player[playerCount];
                    playerCount++;
                    if (playerCount >= 2)
                    {
                        break;
                    }
                }

                isOtherPlayerConnect = true;
                text_connectInfo.text = "매칭!!";
                pv.RPC("MultiGameStart", RpcTarget.Others, moneyState);
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                foreach (var player in PhotonNetwork.CurrentRoom.Players)
                {
                    texts_player_nick[0].text = player.Value.NickName;
                    texts_player_record[0].text = ((string)player.Value.CustomProperties["Win"]) + "승 " + ((string)player.Value.CustomProperties["Lose"]) + "패";
                    texts_player_money[0].text = string.Format("{0:#,###}원", ((long)player.Value.CustomProperties["Money"]));
                    customModelData_player[0].Init(CustomModelSettingCtrl.GetRandomModelData(), CustomModelViewState.Normal, null, 0.1f);
                    //charCtrls_player[0].SetCharacter((int)player.Value.CustomProperties["CharIndex"]);
                    //images_player[0].texture = textures_player[0];
                }

                texts_player_nick[1].text = "비접속";
                texts_player_record[1].text = "";
                texts_player_money[1].text = "";
                //charCtrls_player[1].SetCharacter();
                images_player[1].texture = textures_player[2];

                isOtherPlayerConnect = false;
                // 수정 되어야함
               /* if (!GameManager.instance.isGameStart)
                {
                    if (cor_connecting != null)
                    {
                        StopCoroutine(cor_connecting);
                    }
                    cor_connecting = StartCoroutine(ConnectingCoroutine());
                }*/
            }
            else
            {
                isOtherPlayerConnect = false;
            }
        }

        public override void OnPlayerEnteredRoom(Player player)
        {
            Debug.Log("OnPlayerEnteredRoom");
            //text.text = newPlayer.NickName;
            CheckPlayerCount();
        }

        public override void OnPlayerLeftRoom(Player player)
        {
            Debug.Log("OnPlayerLeftRoom");
            CheckPlayerCount();
        }

        public override void OnLeftRoom()
        {
            if (PhotonNetwork.CurrentRoom != null)
                Debug.Log("Room Left success. name: " + PhotonNetwork.CurrentRoom.Name);
        }

        // 마스터가 보낸덱리스트 대로 나머지 유저들 세팅.
        [PunRPC]
        public void SynchronizationDeck(int[] deckList, bool isFirstTurn)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                pv.RPC("SynchronizationDeck", RpcTarget.Others, deckList, isFirstTurn);
            }
            else
            {
                //GameManager.instance.StandByShuffleCard(deckList);
                if (isFirstTurn)
                {
                    cardList_firstTurn = deckList;
                }
                else
                {
                    cardList_play = deckList;
                }
            }
        }

        public void ClearDeckList(bool isFirstTurn = false)
        {
            if (isFirstTurn)
            {
                cardList_firstTurn = null;
            }
            else
            {
                cardList_firstTurn = null;
                cardList_play = null;
            }
        }

        // 마스터가 보낸덱리스트 대로 나머지 유저들 세팅.
        [PunRPC]
        public void SynchronizationMission(int missionStateNum, int mulPoint)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                pv.RPC("SynchronizationMission", RpcTarget.Others, missionStateNum, mulPoint);
            }
            else
            {
                //GameManager.instance.SetMission(missionStateNum, mulPoint);
            }
        }

        [PunRPC]
        public void MultiGameStart(int m_moneyState)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                if (cor_connecting != null)
                {
                    StopCoroutine(cor_connecting);
                }
                if (cor_counting != null)
                {
                    StopCoroutine(cor_counting);
                }
                cor_connecting = StartCoroutine(ReadyCoroutine(m_moneyState));
            }
            else
            {
                if (cor_connecting != null)
                {
                    StopCoroutine(cor_connecting);
                }
                if (cor_counting != null)
                {
                    StopCoroutine(cor_counting);
                }

                cor_connecting = StartCoroutine(ConnectingCoroutine());
            }
        }

        public NetEventData GetEventData()
        {
            if (netEventDatas[readIndex_event] != null)
            {
                return netEventDatas[readIndex_event];
            }
            else
            {
                return null;
            }
        }

        public void SetDataReadIndex()
        {
            readIndex_event++;
        }

        public void ClearEventData()
        {
            for (int i = 0; i < netEventDatas.Length; i++)
            {
                netEventDatas[i] = null;
            }
            sendCount_event = 0;
            readIndex_event = 0;
        }

        public void SendEventData(NetEventData netEventData)
        {
          /*  if (!GameManager.instance.isNetMatch || GameManager.instance.currentPlayerNum == 1)
            {
                return;
            }*/
            Debug.LogError("Send : Count - " + sendCount_event + " / State - " + netEventData.state + " / Index - " + netEventData.selectIndex);
            pv.RPC("ReceiveEventData", RpcTarget.Others, sendCount_event, netEventData.state, netEventData.selectIndex);
            sendCount_event++;
        }

        [PunRPC]
        public void ReceiveEventData(int sendCount, NetEventState state, int index)
        {
            NetEventData netEventData = new NetEventData();
            netEventData.state = state;
            netEventData.selectIndex = index;
            netEventDatas[sendCount] = netEventData;
            Debug.LogWarning("Receive : Count - " + sendCount + " / State - " + netEventData.state + " / Index - " + netEventData.selectIndex);
            //Debug.LogError("ReceiveEventData - " + netEventData.state + " / " + netEventData.selectIndex);
        }

        Coroutine cor_userTranform;

        public void SendUserTransform()
        {
            if (cor_userTranform != null)
            {
                StopCoroutine(cor_userTranform);
            }
            cor_userTranform = StartCoroutine(SendUserTransformCoroutine());
        }

        public void StopSendUserTransform()
        {
            if (cor_userTranform != null)
            {
                StopCoroutine(cor_userTranform);
            }
        }

        IEnumerator SendUserTransformCoroutine()
        {
            while (true)
            {
                pv.RPC("ReceiveUserTransform", RpcTarget.Others, hmdTr.position, controllerTrL.position, controllerTrR.position, hmdTr.rotation, controllerTrL.rotation, controllerTrR.rotation);
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }

        [PunRPC]
        public void ReceiveUserTransform(Vector3 pos_head, Vector3 pos_handL, Vector3 pos_handR, Quaternion rot_head, Quaternion rot_handL, Quaternion rot_handR)
        {
            var transEuler = Quaternion.Euler(-rot_head.eulerAngles.x, rot_head.eulerAngles.y, -rot_head.eulerAngles.z);
            netUserRoation_head = transEuler;

            transEuler = Quaternion.Euler(-rot_handL.eulerAngles.x, rot_handL.eulerAngles.y, -rot_handL.eulerAngles.z);
            netUserRoation_handL = transEuler;

            transEuler = Quaternion.Euler(-rot_handR.eulerAngles.x, rot_handR.eulerAngles.y, -rot_handR.eulerAngles.z);
            netUserRoation_handR = transEuler;

            netUserPosition_head = pos_head;
            netUserPosition_handL = pos_handL;
            netUserPosition_handR = pos_handR;
            lastReceiveTime = Time.unscaledTime;
        }

        public void GameReady()
        {
            PhotonNetwork.LocalPlayer.CustomProperties["GameReady"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        }
        public void GameStart()
        {
            PhotonNetwork.LocalPlayer.CustomProperties["GameReady"] = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        }


        //public void OnInvitationReceived(string fromUserId, string roomName)
        //{
        //    //월드 대전을 실행하고 있을때는 초대를 받지 않는다.
        //    if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.IsVisible)
        //    {
        //        return;
        //    }

        //    //2명이서 매칭이 됬을때는 초대를 받지 않는다.
        //    if (PhotonNetwork.InRoom && 1 < PhotonNetwork.CurrentRoom.PlayerCount)
        //    {
        //        return;
        //    }

        //    if (inviteMessageUI.activeSelf || GameManager.instance.isNetMatch)
        //    {
        //        return;
        //    }

        //    string userNick = "";
        //    invite_realRoomName = "";
        //    invite_fromUserId = fromUserId;
        //    if (roomName[0] == 'A')
        //    {
        //        int _length = Int32.Parse(roomName[1].ToString());
        //        int index = 2;
        //        for (; index < _length + 2; index++)
        //        {
        //            userNick += roomName[index].ToString();
        //        }
        //        for (; index < roomName.Length; index++)
        //        {
        //            invite_realRoomName += roomName[index].ToString();
        //        }

        //        Debug.Log("UserNick : " + userNick);
        //        Debug.Log("realRoomName : " + invite_realRoomName);
        //    }
        //    else if (roomName[0] == 'B')
        //    {
        //        string temp = "";
        //        temp += roomName[1].ToString();
        //        temp += roomName[2].ToString();
        //        int _length = Int32.Parse(temp);
        //        int index = 3;
        //        for (; index < _length + 3; index++)
        //        {
        //            userNick += roomName[index].ToString();
        //        }
        //        for (; index < roomName.Length; index++)
        //        {
        //            invite_realRoomName += roomName[index].ToString();
        //        }

        //        Debug.Log("UserNick : " + userNick);
        //        Debug.Log("realRoomName : " + invite_realRoomName);
        //    }


        //    // 초대 메시지 UI.
        //    inviteMessageUI.SetActive(true);
        //    text_inviteMessageUI.text = string.Format("[{0}]님이\n초대 메시지를 보냈습니다.", userNick);
        //}

        //public void Click_SelectInvitation(bool isOk)
        //{
        //    if (isOk)
        //    {
        //        isFriendCall = true;

        //        PhotonNetwork.JoinRoom(invite_realRoomName);
        //    }
        //    else
        //    {

        //        // 원상복구.
        //    }
        //    inviteMessageUI.SetActive(false);
        //}


        public void DisconnectPhotonVoice()
        {
            return;

        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("cause : " + cause);
            StartCoroutine(Reconnect());
        }


        IEnumerator Reconnect()
        {


            yield return null;

        }

        public void OnServerConnected()
        {

        }

        public void OnServerDisconnected()
        {

        }


        //_value - 무조건 1.

        public void Func_Achieve(string _code, int _value)
        {
            return;

        }

        public void FailGetDBData()
        {

        }
    }
}
