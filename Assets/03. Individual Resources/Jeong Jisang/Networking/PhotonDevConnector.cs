using Billiards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Appnori.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using SingletonPunBase;

public class PhotonDevConnector : MonoBehaviour
{
    const string PHOTON_REALTIME_APP_ID = "1461c7e4-ff3c-42ac-b727-7c1fa3b5a130";
    const string PHOTON_VOICE_APP_ID = "88e446b6-5bfd-4a1e-bec7-a31049308f60";
    public static PhotonDevConnector instance;

    private PhotonView pv;
    private BilliardsMatchInfo infos;

    [SerializeField]
    private LevelLoader loader;

   
    [SerializeField] private GameObject waitingUI;
    [SerializeField] private Text MultiText;

    [SerializeField] private CustomModelSettingCtrl opponent;

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
        if (loader == null)
            return;
        pv = GetComponent<PhotonView>();
       
        var info = loader.BuildDevelopInfo();
        StartCoroutine(StartInit());
        
        if (info.playingType != PlayType.Multi)
            return;
        PhotonManager.Instance.ConnectedPlayerCount.OnDataChanged += ConnectedPlayerCount_OnDataChanged;
        PhotonManager.Instance.StartConnect();
       
    }

    IEnumerator StartInit()
    {
        while (GameDataManager.instance.state_userInfo != GameDataManager.UserInfoState.Completed)
            yield return null;
        PhotonNetwork.LocalPlayer.NickName = GameDataManager.instance.userInfo_mine.nick;
        PhotonNetwork.LocalPlayer.CustomProperties = new ExitGames.Client.Photon.Hashtable();
        PhotonNetwork.LocalPlayer.CustomProperties.Add("AppnoriID", GameDataManager.instance.userInfo_mine.id);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("Ladder", GameDataManager.instance.userInfo_mine.totalLaddr2);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("Coin", GameDataManager.instance.userInfo_mine.nowGold.Value);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("ModelData", JsonUtility.ToJson(GameDataManager.instance.userInfo_mine.customModelData));
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = PHOTON_REALTIME_APP_ID;
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice = PHOTON_VOICE_APP_ID;
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "Billiards" + Application.version;
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
        //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = ""; // bestRegion -> 가까운 서버로 접속함
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = "";
        PhotonNetwork.PhotonServerSettings.AppSettings.Port = 0;
        

    }

    
        

    public void OnConnectAndStart(BilliardsMatchInfo info)
    {
        PhotonManager.Instance.ConnectedPlayerCount.OnDataChanged += PlayerCountChanged;
        PhotonManager.Instance.StartConnect();
        
        void PlayerCountChanged(int obj)
        {
            if (obj != 2)
                return;

            PhotonManager.Instance.ConnectedPlayerCount.OnDataChanged -= PlayerCountChanged;
            info.userName = PhotonNetwork.LocalPlayer.NickName;
            info.userId = PhotonNetwork.LocalPlayer.UserId;

            
            foreach (var player in PhotonNetwork.CurrentRoom.Players.OrderBy(i => i.Value.ActorNumber))
            {
                if (player.Value.IsLocal)
                {
                   
                    continue;
                }

                

                info.otherName = player.Value.NickName;
                info.otherId = player.Value.UserId;

               
               
                //Debug.Log($"other:{player.Value.UserId}2");
            }
            infos = info;
            pv.RPC("SetOpponent", RpcTarget.Others,null);

            //LevelLoader.InitializeAndLoad(info);
        }
    }

    [PunRPC]
    public void SetOpponent()
    {
        Debug.Log($"SetOpponent");
        StartCoroutine(StartMultiGame());
    }

    private IEnumerator StartMultiGame()
    {

        float sec = 5;
        MultiText.text = $"매칭 되었습니다!!\n{sec}초 후\n게임을 시작 합니다.";
       
        waitingUI.SetActive(false);
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value.IsLocal)
            {
                continue;
            }
          

            opponent.Init(JsonUtility.FromJson<CustomModelData>(player.Value.CustomProperties["ModelData"].ToString()));
            opponent.SetRenderQueue(1000);
            GameDataManager.instance.AddUsers(player.Value.CustomProperties["AppnoriID"].ToString(), player.Value.NickName,
                int.Parse(player.Value.CustomProperties["Coin"].ToString()), int.Parse(player.Value.CustomProperties["Ladder"].ToString()),
                JsonUtility.FromJson<CustomModelData>(player.Value.CustomProperties["ModelData"].ToString()));
            if (PhotonNetwork.IsMasterClient)
            {
                GameDataManager.instance.startGame("M", PhotonNetwork.CurrentRoom.Name);
            }
        }
        while (sec >= 0)
        {
            sec -= Time.deltaTime;
            
            if (sec > 0) MultiText.text = $"매칭 되었습니다!!\n{sec.ToString("0.0")}초 후\n게임을 시작 합니다.";
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        LevelLoader.InitializeAndLoad(infos);
    }

    private void ConnectedPlayerCount_OnDataChanged(int obj)
    {
        if (obj != 2)
            return;

        PhotonManager.Instance.ConnectedPlayerCount.OnDataChanged -= ConnectedPlayerCount_OnDataChanged;

        var info = loader.BuildDevelopInfo();
        info.userName = PhotonNetwork.LocalPlayer.NickName;
        info.userId = PhotonNetwork.LocalPlayer.UserId;
       
        foreach (var player in PhotonNetwork.CurrentRoom.Players.OrderBy(i => i.Value.ActorNumber))
        {
            if (player.Value.IsLocal)
                continue;
            if (waitingUI.activeSelf)
                waitingUI.SetActive(false);

            info.otherName = player.Value.NickName;
            info.otherId = player.Value.UserId;

            pv.RPC("SetOpponent", RpcTarget.Others, GameDataManager.instance.userInfo_mine.customModelData);
            Debug.LogError($"other:{player.Value.UserId}1");
        }
        
        LevelLoader.InitializeAndLoad(info);
    }

    public void SetRoom(string playType)
    {
        PhotonManager.Instance.photonMatchState = playType switch
        {
            "Single" => PhotonManager.PhotonMatchState.Single,
            "Random" => PhotonManager.PhotonMatchState.Random,
            "Friend" => PhotonManager.PhotonMatchState.Friend,
            _ => throw new System.Exception("Not matched")
        };
    }

    public void SetRoomCode(string code)
    {
        PhotonManager.Instance.roomCode = code;
        PhotonManager.Instance.photonMatchState = PhotonManager.PhotonMatchState.Friend;
    }
}
