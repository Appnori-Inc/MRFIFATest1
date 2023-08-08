using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class WaitingSingleManager : MonoBehaviourPunCallbacks
{
    private float keep_timeScale = 1f;

    public string keep_serverCode = "";

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        DontDestroyOnLoad(this.gameObject);

        GameDataManager.singleManager = this;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scene_Lobby")
        {
            Destroy(this.gameObject);
            return;
        }

        if (GameDataManager.instance.playType == GameDataManager.PlayType.Multi || scene.buildIndex == 0)
        {
            Destroy(this.gameObject);
            return;
        }

        Debug.Log(scene.buildIndex);

        Invoke("StartConnect", 2f);
    }

    private void OnSceneUnloaded(Scene current)
    {
        if (GameDataManager.instance.playType == GameDataManager.PlayType.Multi)
        {
            return;
        }

        Debug.Log("OnSceneUnloaded: " + current);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void StartConnect()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = keep_serverCode;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PublishUserId = true;
        roomOptions.MaxPlayers = 2;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add("GameType", GameDataManager.instance.gameType);
        roomOptions.CustomRoomProperties.Add("Code", "");


        roomOptions.CustomRoomPropertiesForLobby = new string[] { "GameType", "Code" };

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

        PhotonNetwork.CreateRoom("Lobby" + Time.time, roomOptions);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateRoomInfo();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomInfo();
    }

    void UpdateRoomInfo()
    {
        PublicGameUIManager.GetInstance.UpdateMatchRoomInfo();

        if (PhotonNetwork.CurrentRoom.Players.Count == 1)
        {
            StartMatchCoroutine(false);
        }
        else
        {
            StartMatchCoroutine(true);
        }
    }

    public void StartMatchCoroutine(bool isMatch)
    {
        if (isMatch)
        {
            keep_timeScale = Time.timeScale;
        }

        if (matchCoroutine != null)
        {
            StopCoroutine(matchCoroutine);
        }
        matchCoroutine = StartCoroutine(MatchCoroutine(isMatch));
    }

    Coroutine matchCoroutine;

    IEnumerator MatchCoroutine(bool isMatch)
    {
        Time.timeScale = 0f;
        PublicGameUIManager.GetInstance.SetViewState(PublicGameUIManager.ViewState.Match);

        if (!isMatch)
        {
            PublicGameUIManager.GetInstance.SetMatchTime("Other player has left.");
            PhotonNetwork.IsSyncScene = false;
            PhotonNetwork.isLoadLevel = false;
            yield return new WaitForSecondsRealtime(1.5f);
            Time.timeScale = keep_timeScale;
            PublicGameUIManager.GetInstance.SetViewState(PublicGameUIManager.ViewState.None);

        }
        else
        {
            PublicGameUIManager.GetInstance.SetMatchTime("Match!");
            yield return new WaitForSecondsRealtime(1f);
            PhotonNetwork.IsSyncScene = true;
            PhotonNetwork.isLoadLevel = true;
            int countTime = 3;
            while (countTime > 0)
            {
                PublicGameUIManager.GetInstance.SetMatchTime(countTime.ToString());
                yield return new WaitForSecondsRealtime(1f);
                countTime--;
            }

            Time.timeScale = 1f;
            GameDataManager.instance.playType = GameDataManager.PlayType.Multi;

            PublicGameUIManager.GetInstance.StartLoadLevel();
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PublicGameUIManager.GetInstance.SetViewState(PublicGameUIManager.ViewState.None);
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
