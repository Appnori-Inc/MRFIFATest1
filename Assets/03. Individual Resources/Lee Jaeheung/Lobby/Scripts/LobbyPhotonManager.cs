using Photon.Pun;
using SingletonPunBase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.Linq;
using Photon.Voice.PUN;
#if OCULUS_PLATFORM
using Oculus.Platform.Models;
#endif
public class LobbyPhotonManager : Singleton<LobbyPhotonManager>
{
#if TEST_ACCOUNT && SERVER_GLOBAL
    const string PHOTON_REALTIME_APP_ID = "c639c9c5-24aa-431f-8e20-290f1bf81bee";
    const string PHOTON_VOICE_APP_ID = "b867ede8-6123-462a-9505-56f37f68ba80";
#elif TEST_ACCOUNT && SERVER_CHINA
    const string PHOTON_REALTIME_APP_ID = "9d293725-ce9f-4fbf-89ae-d297758592c7";//"e6360d31-7e59-4a4c-9b3c-d23a36148367";
    const string PHOTON_VOICE_APP_ID = "9d293725-ce9f-4fbf-89ae-d297758592c7";//"82adc176-1123-4cda-98e8-5895f79492ee";
#elif SERVER_CHINA
    const string PHOTON_REALTIME_APP_ID = "9d293725-ce9f-4fbf-89ae-d297758592c7";
    const string PHOTON_VOICE_APP_ID = "9d293725-ce9f-4fbf-89ae-d297758592c7";//"bc67eaa0-eded-418c-ba53-c528744c5481";
#else
    const string PHOTON_REALTIME_APP_ID = "a0444e18-b386-4d89-b8c8-776e1aefe0ff";
    const string PHOTON_VOICE_APP_ID = "7939b848-5a46-4592-a5fc-df2c5cb65750";
#endif
    public bool isReady = false;

    private List<RoomInfo> keep_roomInfos;

    private GameDataManager.GameType find_gameType = GameDataManager.GameType.None;

    public class ServerInfo
    {
        public string name;
        public string code;
        public string ping;
        public int grade;
    }

    public ServerInfo[] serverInfos;
    public int index_server = 0;

    public int countTime;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.SerializationRate = 10;
        PhotonNetwork.SendRate = 20;

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 1f;
        StartDisconnect();
    }

    public void StartConnect(bool isRandom)
    {
        GameDataManager.instance.photonMatchState = (isRandom ? GameDataManager.PhotonMatchState.Random : GameDataManager.PhotonMatchState.Friend);
        StartConnect();
    }

    public void StartConnect(string regionCode = "")
    {
        isReady = false;

        if (GameDataManager.instance.photonMatchState == GameDataManager.PhotonMatchState.Random)
        {
#if DB_TEST
            countTime = 15;
#else
            countTime = 30;
#endif
        }
        else
        {
            countTime = 7;
        }

        PhotonNetwork.LocalPlayer.NickName = GameDataManager.instance.userInfo_mine.nick;
        PhotonNetwork.LocalPlayer.CustomProperties = new ExitGames.Client.Photon.Hashtable();
        PhotonNetwork.LocalPlayer.CustomProperties.Add("AppnoriID", GameDataManager.instance.userInfo_mine.id);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("Win", GameDataManager.score_win_mine);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("Lose", GameDataManager.score_lose_mine);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("Dis", GameDataManager.score_disconnect_mine);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("GameReady", true);

        PhotonNetwork.LocalPlayer.CustomProperties.Add("ModelData", JsonUtility.ToJson(GameDataManager.instance.GetMyCustomModelData()));

        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = PHOTON_REALTIME_APP_ID;
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice = PHOTON_VOICE_APP_ID;


#if DB_TEST
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "test_aio";
#else
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = Application.version + "_aio";
#endif

#if SERVER_CHINA
        UserInfoManager.GetPortonServer portonServer = GameDataManager.instance.getPortonServer;
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = portonServer.UseNameServer;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = portonServer.FixedRegion;
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = portonServer.Server;
        PhotonNetwork.PhotonServerSettings.AppSettings.Port = portonServer.Port;

        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = "60.205.105.94";
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



    public void StartDisconnect()
    {
        isReady = false;
        PhotonNetwork.IsSyncScene = false;
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            PhotonNetwork.Disconnect();
        }
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
        LobbyUIManager.GetInstance.text_time.text = "";
    }

    public void ClearServerInfos()
    {
        serverInfos = null;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
#if SERVER_CHINA
        serverInfos = new ServerInfo[1];
        serverInfos[0] = new ServerInfo();
        serverInfos[0].code = "cn";
        serverInfos[0].name = GetServerName(serverInfos[0].code);

        serverInfos[0].ping = PhotonNetwork.GetPing() + "ms";
        if (PhotonNetwork.GetPing() >= 200)
        {
            serverInfos[0].grade = 2;
        }
        else if (PhotonNetwork.GetPing() >= 100)
        {
            serverInfos[0].grade = 1;
        }
        else
        {
            serverInfos[0].grade = 0;
        }
        LobbyUIManager.GetInstance.InitServerUI();
#else
        if (serverInfos == null && string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion))
        {
            serverInfos = new ServerInfo[PhotonNetwork.NetworkingClient.RegionHandler.EnabledRegions.Count];
            for (int i = 0; i < serverInfos.Length; i++)
            {
                serverInfos[i] = new ServerInfo();
                serverInfos[i].code = PhotonNetwork.NetworkingClient.RegionHandler.EnabledRegions[i].Code;
                serverInfos[i].name = GetServerName(serverInfos[i].code);
                serverInfos[i].ping = PhotonNetwork.NetworkingClient.RegionHandler.EnabledRegions[i].Ping + "ms";
                if (PhotonNetwork.NetworkingClient.RegionHandler.EnabledRegions[i].Ping >= 200)
                {
                    serverInfos[i].grade = 2;
                }
                else if (PhotonNetwork.NetworkingClient.RegionHandler.EnabledRegions[i].Ping >= 100)
                {
                    serverInfos[i].grade = 1;
                }
                else
                {
                    serverInfos[i].grade = 0;
                }
            }
            LobbyUIManager.GetInstance.InitServerUI();
        }
#endif
        PhotonNetwork.JoinLobby();
    }

    public string GetServerName(string code)
    {
        switch (code.ToLower())
        {
            case "us":
                return "USA";
            case "eu":
                return "Europe";
            case "ru":
                return "Russia";
            case "sa":
                return "<size=40>S.America</size>";
            case "kr":
                return "Korea";
            case "asia":
                return "Asia";
            case "cn":
                return "China";
            default:
                return code;
        }
    }

    public void SetFindRoom(GameDataManager.GameType gameType)
    {
        isReady = false;
        find_gameType = gameType;
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (GameDataManager.instance.photonMatchState == GameDataManager.PhotonMatchState.Random)
        {
            for (int i = 0; i < LobbyUIManager.GetInstance.meshButtons_gameType.Length; i++)
            {
                LobbyUIManager.GetInstance.meshButtons_gameType[i].SetMaterial(LobbyUIManager.GetInstance.materials_lobbyUI[0]);
                LobbyUIManager.GetInstance.meshButtons_gameType[i].SetInteractable(false);
            }

            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].CustomProperties == null || !roomList[i].CustomProperties.ContainsKey("GameType") || !roomList[i].CustomProperties.ContainsKey("Code"))
                {
                    continue;
                }

                if ((string)roomList[i].CustomProperties["Code"] != "" || roomList[i].PlayerCount == roomList[i].MaxPlayers || roomList[i].IsOpen == false)
                {
                    continue;
                }

                if ((GameDataManager.GameType)roomList[i].CustomProperties["GameType"] == GameDataManager.instance.gameType)
                {
                    continue;
                }

                switch ((GameDataManager.GameType)roomList[i].CustomProperties["GameType"])
                {
                    case GameDataManager.GameType.Bowling:
                        {
                            LobbyUIManager.GetInstance.meshButtons_gameType[0].SetMaterial(LobbyUIManager.GetInstance.materials_lobbyUI[1]);
                            LobbyUIManager.GetInstance.meshButtons_gameType[0].SetInteractable(true);
                        }
                        break;
                    case GameDataManager.GameType.Archery:
                        {
                            LobbyUIManager.GetInstance.meshButtons_gameType[1].SetMaterial(LobbyUIManager.GetInstance.materials_lobbyUI[1]);
                            LobbyUIManager.GetInstance.meshButtons_gameType[1].SetInteractable(true);
                        }
                        break;
                    case GameDataManager.GameType.Basketball:
                        {
                            LobbyUIManager.GetInstance.meshButtons_gameType[2].SetMaterial(LobbyUIManager.GetInstance.materials_lobbyUI[1]);
                            LobbyUIManager.GetInstance.meshButtons_gameType[2].SetInteractable(true);
                        }
                        break;
                    case GameDataManager.GameType.Badminton:
                        {
                            LobbyUIManager.GetInstance.meshButtons_gameType[3].SetMaterial(LobbyUIManager.GetInstance.materials_lobbyUI[1]);
                            LobbyUIManager.GetInstance.meshButtons_gameType[3].SetInteractable(true);
                        }
                        break;
                    case GameDataManager.GameType.Billiards:
                        {
                            LobbyUIManager.GetInstance.meshButtons_gameType[4].SetMaterial(LobbyUIManager.GetInstance.materials_lobbyUI[1]);
                            LobbyUIManager.GetInstance.meshButtons_gameType[4].SetInteractable(true);
                        }
                        break;
                    case GameDataManager.GameType.Darts:
                        {
                            LobbyUIManager.GetInstance.meshButtons_gameType[5].SetMaterial(LobbyUIManager.GetInstance.materials_lobbyUI[1]);
                            LobbyUIManager.GetInstance.meshButtons_gameType[5].SetInteractable(true);
                        }
                        break;
                    case GameDataManager.GameType.TableTennis:
                        {
                            LobbyUIManager.GetInstance.meshButtons_gameType[6].SetMaterial(LobbyUIManager.GetInstance.materials_lobbyUI[1]);
                            LobbyUIManager.GetInstance.meshButtons_gameType[6].SetInteractable(true);
                        }
                        break;
                    case GameDataManager.GameType.Boxing:
                        {
                            LobbyUIManager.GetInstance.meshButtons_gameType[7].SetMaterial(LobbyUIManager.GetInstance.materials_lobbyUI[1]);
                            LobbyUIManager.GetInstance.meshButtons_gameType[7].SetInteractable(true);
                        }
                        break;
                    case GameDataManager.GameType.Golf:
                        {
                            LobbyUIManager.GetInstance.meshButtons_gameType[8].SetMaterial(LobbyUIManager.GetInstance.materials_lobbyUI[1]);
                            LobbyUIManager.GetInstance.meshButtons_gameType[8].SetInteractable(true);
                        }
                        break;
                    case GameDataManager.GameType.Baseball:
                        {
                            LobbyUIManager.GetInstance.meshButtons_gameType[9].SetMaterial(LobbyUIManager.GetInstance.materials_lobbyUI[1]);
                            LobbyUIManager.GetInstance.meshButtons_gameType[9].SetInteractable(true);
                        }
                        break;
                    case GameDataManager.GameType.Tennis:
                        {
                            LobbyUIManager.GetInstance.meshButtons_gameType[10].SetMaterial(LobbyUIManager.GetInstance.materials_lobbyUI[1]);
                            LobbyUIManager.GetInstance.meshButtons_gameType[10].SetInteractable(true);
                        }
                        break;
                }
            }
            GameDataManager.GameType gameType = find_gameType;
            find_gameType = GameDataManager.GameType.None;

            keep_roomInfos = roomList;

            if (gameType == GameDataManager.GameType.None)
            {
                for (int i = 0; i < roomList.Count; i++)
                {
                    if (roomList[i].CustomProperties == null || !roomList[i].CustomProperties.ContainsKey("GameType") || !roomList[i].CustomProperties.ContainsKey("Code") || !roomList[i].IsOpen)
                    {
                        continue;
                    }

                    if ((GameDataManager.GameType)roomList[i].CustomProperties["GameType"] == GameDataManager.instance.gameType
                        && (string)roomList[i].CustomProperties["Code"] == ""
                        && roomList[i].PlayerCount != roomList[i].MaxPlayers)
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
                    if (roomList[i].CustomProperties == null || !roomList[i].CustomProperties.ContainsKey("GameType") || !roomList[i].CustomProperties.ContainsKey("Code"))
                    {
                        continue;
                    }

                    if ((GameDataManager.GameType)roomList[i].CustomProperties["GameType"] == gameType
                        && (string)roomList[i].CustomProperties["Code"] == ""
                        && roomList[i].PlayerCount != roomList[i].MaxPlayers)
                    {
                        GameDataManager.instance.gameType = gameType;
                        PhotonNetwork.JoinRoom(roomList[i].Name);
                        return;
                    }
                }
            }


        }
        else
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].CustomProperties == null || !roomList[i].CustomProperties.ContainsKey("GameType") || !roomList[i].CustomProperties.ContainsKey("Code"))
                {
                    continue;
                }

                if ((string)roomList[i].CustomProperties["Code"] == GameDataManager.roomCode && roomList[i].PlayerCount != roomList[i].MaxPlayers)
                {
                    PhotonNetwork.JoinRoom(roomList[i].Name);
                    return;
                }
            }
        }
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        isReady = true;

        UpdateRoomInfo();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.JoinLobby();
    }

    void CreateRoom()
    {
        //PhotonNetwork.AutomaticallySyncScene = true;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;
        roomOptions.MaxPlayers = 2;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add("GameType", GameDataManager.instance.gameType);

        switch (GameDataManager.instance.gameType)
        {
            case GameDataManager.GameType.Bowling:
            case GameDataManager.GameType.Archery:
            case GameDataManager.GameType.Basketball:
            case GameDataManager.GameType.Badminton:
                {
                    roomOptions.CleanupCacheOnLeave = false;
                }
                break;
        }
        if (GameDataManager.instance.photonMatchState == GameDataManager.PhotonMatchState.Friend)
        {
            roomOptions.CustomRoomProperties.Add("Code", GameDataManager.roomCode);
        }
        else
        {
            roomOptions.CustomRoomProperties.Add("Code", "");
        }
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "GameType", "Code"};

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateRoomInfo();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomInfo();
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdateRoomInfo();
    }

    void UpdateRoomInfo()
    {
        if (PhotonNetwork.IsSyncScene && PhotonNetwork.isLoadLevel)
        {
            return;
        }
        Debug.Log("UpdateRoomInfo()");
        GameDataManager.instance.gameType = (GameDataManager.GameType)PhotonNetwork.CurrentRoom.CustomProperties["GameType"];

        LobbyUIManager.GetInstance.SetViewGameType();

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
                LobbyUIManager.GetInstance.text_userNick[count].text = "Player01";
            }
            else
            {
                LobbyUIManager.GetInstance.text_userNick[count].text = "Player02";
            }
#else
            LobbyUIManager.GetInstance.text_userNick[count].text = userInfo.id;
           
#endif
            LobbyUIManager.GetInstance.text_userInfo[count].text = ((int)player.Value.CustomProperties["Win"]).ToString() + "W " + ((int)player.Value.CustomProperties["Lose"]).ToString() + "L";

            count++;
        }


        if (count == 1)
        {
            LobbyUIManager.GetInstance.text_userNick[1].text = "Waiting...";
            LobbyUIManager.GetInstance.text_userInfo[1].text = "";

            PublicGameUIManager.profileCapture.PlayImages(new string[1] { GameDataManager.instance.userInfos[0].id }, ProfileCaptureCtrl.ShotState.Multi);

            if (GameDataManager.instance.photonMatchState == GameDataManager.PhotonMatchState.Random)
            {
                GameDataManager.UserInfo userInfo = new GameDataManager.UserInfo();
                userInfo.id = "AI";
                userInfo.nick = userInfo.id;
                GameDataManager.instance.userInfos.Add(userInfo);
            }
            else
            {
                LobbyUIManager.GetInstance.text_time.text = "";
            }
            StartTimeCoroutine(false);
        }
        else
        {
            PublicGameUIManager.profileCapture.PlayImages(new string[2] { GameDataManager.instance.userInfos[0].id, GameDataManager.instance.userInfos[1].id }, ProfileCaptureCtrl.ShotState.Multi);
            StartTimeCoroutine(true);
        }
    }

    public void StartTimeCoroutine(bool isMatch)
    {
        if (timeCoroutine != null)
        {
            StopCoroutine(timeCoroutine);
        }
        timeCoroutine = StartCoroutine(TimeCoroutine(isMatch));
    }

    Coroutine timeCoroutine;

    IEnumerator TimeCoroutine(bool isMatch)
    {
        if (!isMatch)
        {
            LobbyUIManager.GetInstance.SetMatchBackButton(true);
            LobbyUIManager.GetInstance.SetOtherGames(GameDataManager.instance.photonMatchState == GameDataManager.PhotonMatchState.Random);
            LobbyUIManager.GetInstance.SetServerUI(true);

            if (GameDataManager.instance.photonMatchState == GameDataManager.PhotonMatchState.Friend || GameDataManager.instance.gameType == GameDataManager.GameType.Golf)
            {
                LobbyUIManager.GetInstance.text_time.text = "0";
                yield break;
            }

            int countTime_current = countTime;

            while (countTime_current > 0)
            {
                LobbyUIManager.GetInstance.text_time.text = countTime_current.ToString();
                yield return new WaitForSecondsRealtime(1f);
                countTime_current--;
            }
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
            Instantiate(Resources.Load<GameObject>("Networks/WaitingSingleManager")).GetComponent<WaitingSingleManager>().keep_serverCode = GetServerInfo().code;
            LobbyUIManager.GetInstance.text_userNick[1].text = "AI";
            LobbyUIManager.GetInstance.SetViewMatchText();
            LobbyUIManager.GetInstance.SetOtherGames(false);
            LobbyUIManager.GetInstance.SetServerUI(false);

            GameDataManager.instance.SetRandomLevel();
            GameDataManager.instance.customModelDatas_single = new CustomModelData[5];
            for (int j = 0; j < GameDataManager.instance.customModelDatas_single.Length; j++)
            {
                GameDataManager.instance.customModelDatas_single[j] = CustomModelSettingCtrl.GetRandomModelData();
            }
            GameDataManager.instance.playType = GameDataManager.PlayType.Single;
            PublicGameUIManager.profileCapture.PlayImages(new string[2] { GameDataManager.instance.userInfos[0].id, GameDataManager.instance.userInfos[1].id }, ProfileCaptureCtrl.ShotState.Multi);
            LobbyUIManager.GetInstance.SetMatchBackButton(false);
            yield return new WaitForSecondsRealtime(1f);
            PhotonNetwork.IsSyncScene = false;
            PhotonNetwork.isLoadLevel = false;
            countTime_current = 3;
            while (countTime_current > 0)
            {
                LobbyUIManager.GetInstance.text_time.text = countTime_current.ToString();
                yield return new WaitForSecondsRealtime(1f);
                countTime_current--;
            }

            LobbyUIManager.GetInstance.StartGame();
        }
        else
        {
            LobbyUIManager.GetInstance.SetMatchBackButton(false);
            LobbyUIManager.GetInstance.SetOtherGames(false);
            LobbyUIManager.GetInstance.SetServerUI(false);

            LobbyUIManager.GetInstance.SetViewMatchText();
            yield return new WaitForSecondsRealtime(1f);
            PhotonNetwork.IsSyncScene = true;
            PhotonNetwork.isLoadLevel = true;
            int countTime = 3;
            while (countTime > 0)
            {
                LobbyUIManager.GetInstance.text_time.text = countTime.ToString();
                yield return new WaitForSecondsRealtime(1f);
                countTime--;
            }
            GameDataManager.instance.playType = GameDataManager.PlayType.Multi;
            LobbyUIManager.GetInstance.StartGame();
        }
    }

    public ServerInfo GetServerInfo()
    {
        return serverInfos[index_server];
    }
}
