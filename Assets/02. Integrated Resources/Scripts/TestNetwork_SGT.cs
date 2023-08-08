using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using SingletonPunBase;
using ExitGames.Client.Photon;

public class TestNetwork_SGT : Singleton<TestNetwork_SGT>, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks, IInRoomCallbacks
{
#if PHOTON_UNITY_NETWORKING
    const string PHOTON_REALTIME_APP_ID = "c639c9c5-24aa-431f-8e20-290f1bf81bee";
#endif

    private PhotonView pv = null;

    private byte maxPlayers;
    private bool isConnectComplete;
    private bool isGuiEnd;

    public string loadingGameName;


    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();

        maxPlayers = 10;
        isGuiEnd = false;
        isConnectComplete = false;

        //In Lobby Start
        if (PhotonNetwork.IsConnected == true) PhotonNetwork.Disconnect();
        Connect();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(loadingGameName);
        }
    }
    public void DisConnect()
    {
        if (PhotonNetwork.IsConnected == true) PhotonNetwork.Disconnect();;
    }

    public void Connect()
    {
        /// 1 호출
        /// <summary>
        /// PhotonServerSettings 설정. 
        /// </summary>

        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = PHOTON_REALTIME_APP_ID;
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        /// 2 호출
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short sh, string st)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CleanupCacheOnLeave = false;
        roomOptions.BroadcastPropsChangeToAll = true;
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("룸에 들어옴");
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                pv.RPC("LoadScene", RpcTarget.All, null);
            }
        }
    }

    [PunRPC]
    public void LoadScene()
    {
        StopAllCoroutines();
        StartCoroutine(CorLoadScene());
    }

    IEnumerator CorLoadScene()
    {
        yield return YieldInstructionCache.WaitForSeconds(Time.unscaledDeltaTime * 100);
        SceneManager.LoadScene(loadingGameName);
    }
}
