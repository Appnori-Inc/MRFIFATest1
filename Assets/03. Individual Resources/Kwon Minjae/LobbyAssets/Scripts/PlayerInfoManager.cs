using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MJ;

public class MyInfo
{
    public string nickName;
    public int index_character;
    public int index_purchased_character;
    public long havGold;

    public MyInfo(string _nickName, int _characterIndex, int _purchasedCharacterIndex, long _havGold)
    {
        nickName = _nickName;
        index_character = _characterIndex;
        index_purchased_character = _purchasedCharacterIndex;
        havGold = _havGold;
    }
}

public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager instance;

    private string getDB_URL_playerInfo;
    private string update_URL_playerInfo;
    private string getDB_URL_timeStamp;
    private string update_URL_timeStamp;

    public string gameName;
    public string nickName = "Player01";

    private int index_character;
    private int index_purchased_character = 5;
    private long havGold = 1000000;
    private long totalWinnings;
    private int timeStamp_access;
    private int timeStamp_giveReward;

    private int lastAccessDay = 0;
    private int currentAccessDay = 0;
    private int rescueCount = 0;

    [HideInInspector]
    public bool isDone = false;

    public bool isUpdate = false;

    public int startBetAmount = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitURL();
        GetDB_Data();
    }

    private void InitURL()
    {
        gameName = "GoStop";
        getDB_URL_playerInfo = "appnori.dothome.co.kr/KTSuperVRGame_PlayerInfo/GetPlayerInfo.php";
        update_URL_playerInfo = "appnori.dothome.co.kr/KTSuperVRGame_PlayerInfo/UpdatePlayerInfo.php";
        getDB_URL_timeStamp = "appnori.dothome.co.kr/KTSuperVRGame_PlayerInfo/GetTimeStamp.php";
        update_URL_timeStamp = "appnori.dothome.co.kr/KTSuperVRGame_PlayerInfo/UpdateTImeStamp.php";
    }

    public void GetDB_Data()
    {
        isDone = false;

        //try
        //{
        //    Pico.Platform.CoreService.AsyncInitialize().OnComplete(m =>
        //    {
        //        if (m.IsError)
        //        {
        //            Debug.LogError($"Async initialize failed: code={m.GetError().Code} message={m.GetError().Message}");
        //            return;
        //        }

        //        if (m.Data != Pico.Platform.PlatformInitializeResult.Success && m.Data != Pico.Platform.PlatformInitializeResult.AlreadyInitialized)
        //        {
        //            Debug.LogError($"Async initialize failed: result={m.Data}");
        //            return;
        //        }

        //        Debug.Log("AsyncInitialize Successfully");
        //        Pico.Platform.UserService.GetLoggedInUser().OnComplete(msg =>
        //        {
        //            if (!msg.IsError)
        //            {
        //                Debug.Log("Received get user success");
        //                //state_userInfo = UserInfoState.Success_Platform_UserInfo;

        //                Pico.Platform.Models.User user = msg.Data;
        //                nickName = user.DisplayName;
        //                //Debug.Log(user.ID + "         " + user.DisplayName);
        //                //userInfo_mine.id = "PI" + user.ID;
        //                //userInfo_mine.nick = GetSubtractNick(user.DisplayName);
        //                //origin_nick_mine = user.DisplayName;
        //                //image_url_mine = user.ImageUrl;

        //                //LobbyDebugCtrl.AddText(userInfo_mine.id);
        //                //LobbyDebugCtrl.AddText(userInfo_mine.nick);

        //                //StartCoroutine(AppnoriWebRequest.API_Login());
        //            }
        //            else
        //            {
        //                Debug.LogError("Received get user error");
        //                Pico.Platform.Models.Error error = msg.GetError();
        //                Debug.LogError("Error: " + error.Message);
        //            }
        //        });

        //    });
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError($"Async Initialize Failed:{e}");
        //    return;
        //}


        StartCoroutine(GetDB_DataCoroutine());
    }

    private IEnumerator GetDB_DataCoroutine()
    {
        //while (!NetworkManager.instance.isInit)
        //{
        //    yield return null;
        //}
        yield return new WaitForSecondsRealtime(0.1f);

        UnBoxing_DB_PlayerInfo(nickName + ",0,10000,0");
        //UnBoxing_DB_PlayerInfo("Player01,0,10000,0");

        isDone = true;
        LobbyManager.instance.DelayStart();
        yield break;
        while (isUpdate)
        {
            yield return null;
        }

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("name", PhotonNetwork.LocalPlayer.NickName));
        formData.Add(new MultipartFormDataSection("CharacterIndex", Convert.ToString(index_character)));
        formData.Add(new MultipartFormDataSection("Gold", Convert.ToString(havGold)));
        formData.Add(new MultipartFormDataSection("purchasedCharacterIndex", Convert.ToString(index_purchased_character)));

        UnityWebRequest www = UnityWebRequest.Post(getDB_URL_playerInfo, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);

            while (true)
            {
                yield return new WaitForSecondsRealtime(3f);
                www = UnityWebRequest.Post(getDB_URL_playerInfo, formData);
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);

                   // NetworkManager.GetInstance.FailGetDBData();
                    yield break;
                    //continue;
                }
                else
                {
                    UnBoxing_DB_PlayerInfo(www.downloadHandler.text);
                    break;
                }
            }
        }
        else
        {
            UnBoxing_DB_PlayerInfo(www.downloadHandler.text);
        }

        formData.Clear();
        formData.Add(new MultipartFormDataSection("name", PhotonNetwork.LocalPlayer.NickName));
        formData.Add(new MultipartFormDataSection("gameName", gameName));

        www = UnityWebRequest.Post(getDB_URL_timeStamp, formData);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);

            while (true)
            {
                yield return new WaitForSecondsRealtime(3f);
                www = UnityWebRequest.Post(getDB_URL_timeStamp, formData);
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                    //NetworkManager.GetInstance.FailGetDBData();
                    yield break;
                    //continue;
                }
                else
                {
                    UnBoxing_DB_GameData(www.downloadHandler.text);
                    break;
                }
            }
        }
        else
        {
            UnBoxing_DB_GameData(www.downloadHandler.text);
        }

        StartCoroutine(Download_Record(PhotonNetwork.LocalPlayer.NickName));
        isDone = true;
        LobbyManager.instance.DelayStart();
    }

    private void UnBoxing_DB_PlayerInfo(string data)
    {
        string[] arr_data = data.Split(',');

        PhotonNetwork.LocalPlayer.NickName = arr_data[0];
        //nickName = arr_data[0];
        index_character = Convert.ToInt32(arr_data[1]);
        havGold = Convert.ToInt64(arr_data[2]);
        index_purchased_character = Convert.ToInt32(arr_data[3]);
        Debug.Log("HavGold : " + havGold);
        Debug.Log("CharacterIndex : " + index_character);
        Debug.Log("purchasedCharacterIndex : " + index_purchased_character);
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("CharIndex"))
        {
            PhotonNetwork.LocalPlayer.CustomProperties.Add("CharIndex", index_character);
            PhotonNetwork.LocalPlayer.CustomProperties.Add("CharIndex_Pur", index_purchased_character);
            PhotonNetwork.LocalPlayer.CustomProperties.Add("Money", havGold);
            PhotonNetwork.LocalPlayer.CustomProperties.Add("Win", "0");
            PhotonNetwork.LocalPlayer.CustomProperties.Add("Lose", "0");
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        }
       // SelectCharacterView.instance.SetMyInfo();
    }

    private void UnBoxing_DB_GameData(string data)
    {
        string[] arr_data = data.Split(',');

        //nickName = arr_data[0];
        totalWinnings = Convert.ToInt64(arr_data[1]);
        timeStamp_giveReward = Convert.ToInt32(arr_data[2]);
        timeStamp_access = Convert.ToInt32(arr_data[3]);

        lastAccessDay = Convert.ToInt32(arr_data[4]);
        currentAccessDay = Convert.ToInt32(arr_data[5]);
        rescueCount = Convert.ToInt32(arr_data[6]);

        Debug.Log("TotalWinnings : " + totalWinnings);
        Debug.Log("GiveRewardTimeStamp : " + timeStamp_giveReward);
        Debug.Log("AccessTimeStamp : " + timeStamp_access);
        Debug.Log("LastAccessDay : " + lastAccessDay);
        Debug.Log("CurrentAccessDay : " + currentAccessDay);
        Debug.Log("RescueCount : " + rescueCount);
        //PhotonNetwork.LocalPlayer.CustomProperties.Add("CharIndex", index_character);
        //PhotonNetwork.LocalPlayer.CustomProperties.Add("HavGold", havGold);
        //PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        //LeaderBoardCtrl.instance.MyScoreUpdate(totalWinnings,true);

        UpdateRescueCount();
    }


    public bool IsGetReward()
    {
        int giveRewardTime = timeStamp_access - timeStamp_giveReward;
        giveRewardTime = Mathf.Abs(giveRewardTime);

        Debug.Log("giveRewardTime : " + giveRewardTime);

        //지난접속시간보다 3시간 뒤 접속하면 보상을 받을 수 있다
        if (giveRewardTime > 10800)
        {
            //timeStamp_giveReward = timeStamp_access;
            return true;
        }

        else
            return false;
    }

    public void AddMyGold(long gold)
    {
        if (gold > 0)
        {
            UpdateMyInfotoDB(gold);
        }
        isUpdate = true;
        havGold += gold;

        if (havGold < 0)
        {
            havGold = 0;
        }
        StartCoroutine(UpdatePlayerInfoCoroutine(havGold, index_character, index_purchased_character));
    }

    public void UpdateMyGold(long gold)
    {
        UpdateMyInfotoDB(gold);
        isUpdate = true;
        havGold = gold;
        StartCoroutine(UpdatePlayerInfoCoroutine(havGold, index_character, index_purchased_character));
    }

    public void UpdateMyInfotoDB(long _havGold, int _index_character, int _index_purchased_character)
    {
        isUpdate = true;
        havGold = _havGold;
        index_character = _index_character;
        index_purchased_character = _index_purchased_character;

        StartCoroutine(UpdatePlayerInfoCoroutine(havGold, index_character, index_purchased_character));
    }

    private IEnumerator UpdatePlayerInfoCoroutine(long _havGold, int _characterIndex, int _purchasedCharacterIndex)
    {
        yield break;
        PhotonNetwork.LocalPlayer.CustomProperties["Money"] = _havGold;
        PhotonNetwork.LocalPlayer.CustomProperties["CharIndex"] = _characterIndex;
        PhotonNetwork.LocalPlayer.CustomProperties["CharIndex_Pur"] = _purchasedCharacterIndex;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("name", PhotonNetwork.LocalPlayer.NickName));
        formData.Add(new MultipartFormDataSection("CharacterIndex", Convert.ToString(_characterIndex)));
        formData.Add(new MultipartFormDataSection("Gold", Convert.ToString(_havGold)));
        formData.Add(new MultipartFormDataSection("purchasedCharacterIndex", Convert.ToString(_purchasedCharacterIndex)));

        UnityWebRequest www = UnityWebRequest.Post(update_URL_playerInfo, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }

        else
        {
            Debug.Log("Form upload complete!");
        }
        isUpdate = false;
    }

    public IEnumerator PenaltyPlayerInfoCoroutine(string nickName, long setMoney)
    {
        yield break;
        string enemyNick = nickName;
        int enemyCharNum = 0;
        int enemyCharNum_Pur = 0;
        long enemyMoney = 0;

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("name", enemyNick));
        formData.Add(new MultipartFormDataSection("CharacterIndex", Convert.ToString(enemyCharNum)));
        formData.Add(new MultipartFormDataSection("Gold", Convert.ToString(enemyMoney)));
        formData.Add(new MultipartFormDataSection("purchasedCharacterIndex", Convert.ToString(enemyCharNum_Pur)));

        UnityWebRequest www = UnityWebRequest.Post(getDB_URL_playerInfo, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);

            while (true)
            {
                yield return new WaitForSecondsRealtime(3f);
                www = UnityWebRequest.Post(getDB_URL_playerInfo, formData);
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                   // NetworkManager.GetInstance.FailGetDBData();
                    yield break;
                    //continue;
                }
                else
                {
                    string[] arr_data = www.downloadHandler.text.Split(',');

                    enemyCharNum = Convert.ToInt32(arr_data[1]);
                    enemyMoney = Convert.ToInt64(arr_data[2]);
                    enemyCharNum_Pur = Convert.ToInt32(arr_data[3]);
                    Debug.Log("enemyMoney : " + havGold);
                    Debug.Log("enemyCharNum : " + index_character);
                    Debug.Log("enemyCharNum_Pur : " + index_purchased_character);
                    break;
                }
            }
        }
        else
        {
            string[] arr_data = www.downloadHandler.text.Split(',');

            enemyCharNum = Convert.ToInt32(arr_data[1]);
            enemyMoney = Convert.ToInt64(arr_data[2]);
            enemyCharNum_Pur = Convert.ToInt32(arr_data[3]);
            Debug.Log("enemyMoney : " + havGold);
            Debug.Log("enemyCharNum : " + index_character);
            Debug.Log("enemyCharNum_Pur : " + index_purchased_character);
        }

        formData.Clear();

        formData.Add(new MultipartFormDataSection("name", enemyNick));
        formData.Add(new MultipartFormDataSection("CharacterIndex", Convert.ToString(enemyCharNum)));
        formData.Add(new MultipartFormDataSection("Gold", Convert.ToString(enemyMoney + setMoney)));
        formData.Add(new MultipartFormDataSection("purchasedCharacterIndex", Convert.ToString(enemyCharNum_Pur)));

        www = UnityWebRequest.Post(update_URL_playerInfo, formData);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }

        else
        {
            Debug.Log("Form upload complete!");
        }
    }

    public void UpdateAccessTimeStamp(bool getReward)
    {
        if (getReward)
        {
            timeStamp_giveReward = timeStamp_access;
            StartCoroutine(IEnum_UpdateMyInfotoDB(totalWinnings, timeStamp_access));
        }

        //else
            //StartCoroutine(IEnum_UpdateMyInfotoDB(totalWinnings, timeStamp_giveReward));
    }

    // 리더 보드 용 딴돈.
    public void UpdateMyInfotoDB(long _totalWinnings)
    {
        totalWinnings += _totalWinnings;

        StartCoroutine(IEnum_UpdateMyInfotoDB(totalWinnings, timeStamp_giveReward));
    }

    public bool IsGetRescue()
    {
        if (rescueCount > 10)
            return false;
        else
            return true;
    }

    public void AddRescueCount()
    {
        rescueCount++;
        StartCoroutine(IEnum_UpdateMyInfotoDB(rescueCount));
    }

    private void UpdateRescueCount()
    {
        if (currentAccessDay == lastAccessDay)
            return;

        //지난접속이랑 현재접속일이 다르면 파산도움횟수를 0으로 초기화 

        Debug.Log("파산도움횟수 초기화!!");
        rescueCount = 0;
        StartCoroutine(IEnum_UpdateMyInfotoDB(rescueCount));
    }

    private IEnumerator IEnum_UpdateMyInfotoDB(long _totalWinnings, int _giveRewardTimeStamp)
    {
        yield break;
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("name", PhotonNetwork.LocalPlayer.NickName));
        formData.Add(new MultipartFormDataSection("gameName", gameName));
        formData.Add(new MultipartFormDataSection("totalWinnings", Convert.ToString(_totalWinnings)));
        formData.Add(new MultipartFormDataSection("giveRewardTimestamp", Convert.ToString(_giveRewardTimeStamp)));

        formData.Add(new MultipartFormDataSection("rescueCount", Convert.ToString(rescueCount)));

        UnityWebRequest www = UnityWebRequest.Post(update_URL_timeStamp, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            //NetworkManager.GetInstance.FailGetDBData();
            yield break;
        }

        else
        {
            Debug.Log(www.downloadHandler.text);
            Debug.Log("Form upload complete!");
        }
    }

    private IEnumerator IEnum_UpdateMyInfotoDB(int _rescueCount)
    {
        yield break;
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("name", PhotonNetwork.LocalPlayer.NickName));
        formData.Add(new MultipartFormDataSection("gameName", gameName));
        formData.Add(new MultipartFormDataSection("totalWinnings", Convert.ToString(totalWinnings)));
        formData.Add(new MultipartFormDataSection("giveRewardTimestamp", Convert.ToString(timeStamp_giveReward)));
        formData.Add(new MultipartFormDataSection("rescueCount", Convert.ToString(_rescueCount)));

        UnityWebRequest www = UnityWebRequest.Post(update_URL_timeStamp, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            //NetworkManager.GetInstance.FailGetDBData();
            yield break;
        }

        else
        {
            Debug.Log(www.downloadHandler.text);
            Debug.Log("Form upload complete!");
        }
    }

    IEnumerator Download_Record(string nickName)
    {
        PhotonNetwork.LocalPlayer.CustomProperties["Win"] = 0;
        PhotonNetwork.LocalPlayer.CustomProperties["Lose"] = 0;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        yield break;
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("Game", gameName)); // 게임 타이틀.
        formData.Add(new MultipartFormDataSection("Name", nickName)); // 유저 닉네임.

        UnityWebRequest www = UnityWebRequest.Post("appnori.dothome.co.kr/LeaderBoard/RecordRead.php", formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form download complete!");

            string[] strRecord = www.downloadHandler.text.Split('▒');

            Debug.Log("닉네임 - " + strRecord[0] + ", 승리 - " + strRecord[1] + " 회, 패배 - " + strRecord[2] + " 회, 강제종료 - " + strRecord[3] + " 회");

            PhotonNetwork.LocalPlayer.CustomProperties["Win"] = strRecord[1];
            PhotonNetwork.LocalPlayer.CustomProperties["Lose"] = strRecord[2];
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        }
    }

    public IEnumerator Upload_Record(string nickName, string info)
    {
        yield break;
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("Game", gameName)); // 게임 타이틀.
        formData.Add(new MultipartFormDataSection("Name", nickName)); // 유저 닉네임.
        formData.Add(new MultipartFormDataSection("Info", info)); // 갱신할 정보.(ex."Win,Lose,Dis")

        UnityWebRequest www = UnityWebRequest.Post("appnori.dothome.co.kr/LeaderBoard/RecordUpdate.php", formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");

            string[] strRecord = www.downloadHandler.text.Split('▒');

            Debug.Log("닉네임 - " + strRecord[0] + ", 승리 - " + strRecord[1] + " 회, 패배 - " + strRecord[2] + " 회, 강제종료 - " + strRecord[3] + " 회");

            PhotonNetwork.LocalPlayer.CustomProperties["Win"] = strRecord[1];
            PhotonNetwork.LocalPlayer.CustomProperties["Lose"] = strRecord[2];
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        }
    }

    public void SetStartBetAmount(int amount)
    {
        startBetAmount = amount;
    }

    public MyInfo GetMyInfo()
    {
        MyInfo myInfo = new MyInfo(PhotonNetwork.LocalPlayer.NickName,
            index_character,
            index_purchased_character,
            havGold);

        return myInfo;
    }
}
