#if PUN || PHOTON_UNITY_NETWORKING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkManagement;
using Photon.Pun;
using Photon.Realtime;

public class PunNetwork : NetworkEngine, AightBallPoolMessenger, IConnectionCallbacks, IInRoomCallbacks, ILobbyCallbacks, IMatchmakingCallbacks, IPunObservable
{
    private PhotonView photonView;
    private string gameVersion = "0.01";
    private Player opponentPlayer;
    private AightBallPoolNetworkMessenger messenger;

    private List<RoomInfo> cachedRoomList;

    public override void Initialize()
    {
        if (!messenger)
        {
            messenger = gameObject.AddComponent<AightBallPoolNetworkMessenger>();
        }
        if (cachedRoomList == null)
        {
            cachedRoomList = new List<RoomInfo>();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Connect ");
        sendRate = 10;
        PhotonNetwork.SendRate = sendRate;
        PhotonNetwork.SerializationRate = sendRate;
        //PhotonNetwork.autoJoinLobby = true; // not use
        //PhotonNetwork.EnableLobbyStatistics = true; // moved to photon server setting's LobbyStatistics
        photonView = gameObject.AddComponent<PhotonView>();
        photonView.ObservedComponents = new List<Component>(0);
        photonView.ObservedComponents.Add(this);
        //photonView.synchronization = ViewSynchronization.ReliableDeltaCompressed;
        photonView.ViewID = 5;
        //PhotonNetwork.AutomaticallySyncScene = true;
        //PhotonNetwork.KeepAliveInBackground = BackgroundTimeout;
        PhotonNetwork.AddCallbackTarget(this);
        //Connect();
    }

    public void OnDestroy()
    {
        if(photonView != null)
        {
            photonView.ObservedComponents.Clear();
        }

        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void Disable()
    {
        base.Disable();
    }
    protected override void Update()
    {
        base.Update();
    }

    public override void SendRemoteMessage(string message, params object[] args)
    {
        photonView.RPC(message, opponentPlayer, args);
    }

    public override void OnGoToPLayWithPlayer(PlayerProfile player)
    {
        if (PhotonNetwork.LocalPlayer != PhotonNetwork.MasterClient)
        {
            NetworkManager.opponentPlayer = player;
            PhotonNetwork.JoinRoom(NetworkManager.PlayerToString(NetworkManager.opponentPlayer));
        }
        else
        {
            //The client who first created the room
            photonView.RPC("OnOpponenReadToPlay", opponentPlayer, NetworkManager.PlayerToString(NetworkManager.mainPlayer), AightBallPoolNetworkGameAdapter.is3DGraphics);
        }
    }

    public override void CreateRoom()
    {
        if (PhotonNetwork.NetworkClientState == ClientState.Joined)
        {
            PhotonNetwork.LeaveRoom();
        }
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.NetworkClientState == ClientState.JoinedLobby)
        {
            PhotonNetwork.CreateRoom(NetworkManager.PlayerToString(NetworkManager.mainPlayer)/* + PhotonNetwork.GetRoomList().Length*/, new RoomOptions() { MaxPlayers = 2 }, null);
        }
    }
    public override void Reset()
    {
        LeftRoom();
    }
    public override void LeftRoom()
    {
        if (PhotonNetwork.NetworkClientState == ClientState.Joined)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
    public override void Connect()
    {
        if (photonView && reachable)
        {
            Debug.Log("photonView " + photonView + "  reachable " + reachable + "   PhotonNetwork.NetworkClientState " + PhotonNetwork.NetworkClientState);
            if (PhotonNetwork.IsConnected == false)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            else if (PhotonNetwork.NetworkClientState == ClientState.ConnectingToMasterServer)
            {
                PhotonNetwork.Disconnect();
            }
            //Debug.Log("photonView " + photonView + "  reachable " + reachable +   "   PhotonNetwork.NetworkClientState " + PhotonNetwork.connectionState);
            //if(PhotonNetwork.connectionState == ClientState.Disconnected)
            //{
            //    PhotonNetwork.ConnectUsingSettings(gameVersion);
            //}
            //else if(PhotonNetwork.connectionState != ConnectionState.Connecting)
            //{
            //    PhotonNetwork.Disconnect();
            //}
        }
    }

    public override void Disconnect()
    {
        if (PhotonNetwork.NetworkClientState != ClientState.Disconnected)
        {
            PhotonNetwork.Disconnect();
        }
    }


    void IConnectionCallbacks.OnConnected()
    {
        CallNetworkState(NetworkState.Connected);
        Debug.Log("state " + state);

    }
    void IConnectionCallbacks.OnConnectedToMaster()
    {
        //Debug.Log("OnConnectedToMaster ");
        //Debug.Log("Operating LobbyJoin... ");
        //PhotonNetwork.JoinLobby();
    }
    void IConnectionCallbacks.OnRegionListReceived(RegionHandler regionHandler)
    {
    }
    void IConnectionCallbacks.OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }
    void IConnectionCallbacks.OnCustomAuthenticationFailed(string debugMessage)
    {
    }
    void IConnectionCallbacks.OnDisconnected(DisconnectCause parameters)
    {
        //void OnFailedToConnectToPhoton(object parameters)
        switch (parameters)
        {
            //FailedToConnect
            case DisconnectCause.ExceptionOnConnect:
            case DisconnectCause.Exception:
            case DisconnectCause.AuthenticationTicketExpired:
            case DisconnectCause.CustomAuthenticationFailed:
            case DisconnectCause.InvalidRegion:
            case DisconnectCause.InvalidAuthentication:

            case DisconnectCause.MaxCcuReached:
                CallNetworkState(NetworkState.FiledToConnect);
                break;

            //disconnect
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.DisconnectByServerReasonUnknown:
            case DisconnectCause.OperationNotAllowedInCurrentState:
            case DisconnectCause.DisconnectByClientLogic:
                CallNetworkState(NetworkState.LostConnection);
                break;
        }

        CallNetworkState(NetworkState.FiledToConnect);
        Debug.LogWarning($"OnDisconnected is invoked. Cause : {parameters}");
        Debug.LogWarning(state);
    }


    void ILobbyCallbacks.OnJoinedLobby()
    {
        Debug.Log($"Lobby Joined ");
    }

    void ILobbyCallbacks.OnRoomListUpdate(List<RoomInfo> roomInfos)
    {
        cachedRoomList = roomInfos;
        StartUpdatePlayers();
    }
    void ILobbyCallbacks.OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    { 
    }
    void ILobbyCallbacks.OnLeftLobby()
    {
        Debug.Log($"Lobby Left");
    }





    //matchmakingCallback
    void IMatchmakingCallbacks.OnJoinedRoom()
    {
        if (PhotonNetwork.LocalPlayer != PhotonNetwork.MasterClient)
        {
            opponentPlayer = PhotonNetwork.MasterClient;
            photonView.RPC("OnOpponenReadToPlay", opponentPlayer, NetworkManager.PlayerToString(NetworkManager.mainPlayer), AightBallPoolNetworkGameAdapter.is3DGraphics);
        }
        CallNetworkState(NetworkState.JoinedToRoom);
    }

    void IMatchmakingCallbacks.OnCreateRoomFailed(short code, string msg)
    {

    }
    void IMatchmakingCallbacks.OnCreatedRoom()
    {
        StartUpdatePlayers();
        CallNetworkState(NetworkState.CreatedRoom);
    }

    void IMatchmakingCallbacks.OnLeftRoom()
    {
        opponentPlayer = null;
        PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
        CallNetworkState(NetworkState.LeftRoom);
    }

    void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
    {
        CallNetworkState(NetworkState.JoinRoomFailed);
    }

    void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
    {
        CallNetworkState(NetworkState.JoinRoomFailed);
    }




    //inRoomCallback
    void IInRoomCallbacks.OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("OnPhotonPlayerConnected " + newPlayer.UserId);
        opponentPlayer = newPlayer;
    }
    void IInRoomCallbacks.OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        Debug.LogWarning("OnMasterClientSwitched");
    }
    void IInRoomCallbacks.OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.LogWarning("OnPhotonPlayerDisconnected");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.RemoveRPCs(otherPlayer);
        PhotonNetwork.DestroyPlayerObjects(otherPlayer);
    }

    void IInRoomCallbacks.OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
    }
    void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

    }


    public override void LoadPlayers(ref PlayerProfile[] players)
    {
        RoomInfo[] rooms = cachedRoomList.ToArray();
        List<PlayerProfile> playersList = new List<PlayerProfile>(0);

        using (var e = cachedRoomList.GetEnumerator())
        {
            while (e.MoveNext())
            {
                var room = e.Current;
                if (room.PlayerCount < room.MaxPlayers)
                {
                    playersList.Add(NetworkManager.PlayerFromString(room.Name, ChackIsFriend));
                }
            }
        }
        players = playersList.ToArray();
    }

    public override bool ChackIsFriend(string id)
    {
        string[] friendsId = NetworkManager.social.GetFriendsId();
        if (friendsId != null)
        {
            foreach (string friendId in friendsId)
            {
                if (id == friendId)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override void StartUpdatePlayers()
    {
        StartCoroutine(UpdatePlayers());
    }

    private IEnumerator UpdatePlayers()
    {
        NetworkManager.social.UpdateFriendsList();
        while (!NetworkManager.social.friendsListIsUpdated)
        {
            yield return null;
        }
        NetworkManager.UpdatePlayers();
        StartCoroutine(NetworkManager.LoadRandomPlayer());
        StartCoroutine(NetworkManager.LoadFriendsAndRandomPlayers(50));
    }
    //PUN
    #region PUN
    [PunRPC]
    public override void OnOpponenReadToPlay(string playerData, bool is3DGraphicMode)
    {
        NetworkManager.opponentPlayer = NetworkManager.PlayerFromString(playerData, ChackIsFriend);
        AightBallPoolNetworkGameAdapter.isSameGraphicsMode = AightBallPoolNetworkGameAdapter.is3DGraphics == is3DGraphicMode;
        Debug.Log("OnOpponenReadToPlay " + playerData);
        CallNetworkState(NetworkState.OpponentReadToPlay);
        if (PhotonNetwork.LocalPlayer != PhotonNetwork.MasterClient)
        {
            int turnId = Random.Range(0, 2);
            int turnIdForSend = turnId == 1 ? 0 : 1;
            OnOpponenStartToPlay(turnId);
            photonView.RPC("OnOpponenStartToPlay", opponentPlayer, turnIdForSend);
        }
    }
    [PunRPC]
    public override void OnOpponenStartToPlay(int turnId)
    {
        Debug.Log(" OnOpponenStartToPlay " + turnId);
        adapter.SetTurn(turnId);
    }

    [PunRPC]
    public override void OnSendTime(float time01)
    {
        messenger.SetTime(time01);
    }
    [PunRPC]
    public override void StartSimulate(string impulse)
    {
        messenger.StartSimulate(impulse);
    }
    [PunRPC]
    public override void EndSimulate(string ballsState)
    {
        messenger.EndSimulate(ballsState);
    }
    [PunRPC]
    public override void OnOpponenWaitingForYourTurn()
    {
        base.OnOpponenWaitingForYourTurn();
    }
    [PunRPC]
    public override void OnOpponenInGameScene()
    {
        StartCoroutine(messenger.OnOpponenInGameScene());
    }
    [PunRPC]
    public override void OnOpponentForceGoHome()
    {
        messenger.OnOpponentForceGoHome();
    }
    #endregion PUN

    #region AightBallPool Interface
    [PunRPC]
    public void OnSendGameState(int state)
    {
        messenger.OnSendGameState(state);
    }
    [PunRPC]
    public void OnSendMainHanded(int isRightHanded)
    {
        messenger.OnSendMainHanded(isRightHanded == 1);
    }

    [PunRPC]
    public void OnSendCueControl(Vector3 cuePivotPosition, float cuePivotLocalRotationY, float cueVerticalLocalRotationX, Vector2 cueDisplacementLocalPositionXY, float cueSliderLocalPositionZ, float force)
    {
        messenger.OnSendCueControl(cuePivotPosition, cuePivotLocalRotationY, cueVerticalLocalRotationX, cueDisplacementLocalPositionXY, cueSliderLocalPositionZ, force);
    }
    [PunRPC]
    public void OnForceSendCueControl(Vector3 cuePivotPosition, float cuePivotLocalRotationY, float cueVerticalLocalRotationX, Vector2 cueDisplacementLocalPositionXY, float cueSliderLocalPositionZ, float force)
    {
        messenger.OnForceSendCueControl(cuePivotPosition, cuePivotLocalRotationY, cueVerticalLocalRotationX, cueDisplacementLocalPositionXY, cueSliderLocalPositionZ, force);
    }
    [PunRPC]
    public void OnMoveBall(Vector3 ballPosition)
    {
        messenger.OnMoveBall(ballPosition);
    }
    [PunRPC]
    public void SelectBallPosition(Vector3 ballPosition)
    {
        messenger.SelectBallPosition(ballPosition);
    }
    [PunRPC]
    public void SetBallPosition(Vector3 ballPosition)
    {
        messenger.SetBallPosition(ballPosition);
    }

    [PunRPC]
    public void SetMechanicalStatesFromNetwork(int ballId, string mechanicalStateData)
    {
        messenger.SetMechanicalStatesFromNetwork(ballId, mechanicalStateData);
    }
    [PunRPC]
    public void WaitAndStopMoveFromNetwork(float time)
    {
        messenger.WaitAndStopMoveFromNetwork(time);
    }
    [PunRPC]
    public void SendOpponentCueURL(string url)
    {
        messenger.SetOpponentCueURL(url);
    }
    [PunRPC]
    public void SendOpponentTableURLs(string boardURL, string clothURL, string clothColor)
    {
        messenger.SetOpponentTableURLs(boardURL, clothURL, clothColor);
    }

    #endregion AightBallPool Interface

    #region Appnori PICO
    [PunRPC]
    public void SendPlayerTransform(string uuid, Vector3 worldPosition, Quaternion rotation, Vector3 worldPosition2, Quaternion rotation2, Vector3 worldPosition3, Quaternion rotation3)
    {
        messenger.SetPlayerTransform(uuid, worldPosition, rotation, worldPosition2, rotation2, worldPosition3, rotation3);
    }

    [PunRPC]
    public void SendPlayerScore(string uuid, int score, int targetScore)
    {
        messenger.SetPlayerScore(uuid, score, targetScore);
    }

    [PunRPC]
    public void SendRequestRematch(bool value)
    {
        Debug.Log("Received RemoteMessage : SendRequestRematch " + value);
        messenger.SetRequestRematch(value);
    }
    [PunRPC]
    public void SendFoulInfo(int FoulType, int ballId, int pocketId)
    {
        Debug.Log("Received RemoteMessage : SendFoulInfo " + FoulType);
        messenger.SetFoul(FoulType, ballId, pocketId);
    }

    public bool isMasterClient()
    {
        return PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
    #endregion Appnori PICO
}
#endif
