using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;


public partial class AppnoriWebRequest
{
    [System.Serializable]
    public class Response_Items
    {
        public int status;
        public string comment;
        public List<Infos> ds;

        [System.Serializable]
        public class Infos
        {
            public string itemType;
            public string itemNm;
            public string itemId;
            public int gameCoin;
            public int gameDia;
            public string freeYn;
            public string useYn;
            public string orderYn;
            public string updatedAt;
        }
    }
   
    public static IEnumerator API_TESTRegister()
    {
        string url = api_url + "/v2/user";
        // 나중에 아이디만 다른곳에서 받아오면 됌
        //string ID = userID; 

        Request_Register request = new Request_Register(GameDataManager.instance.userInfo_mine.id, GameDataManager.instance.userInfo_mine.nick,
            "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/06/06896f8cb324e3a683a229cd8a066d56075c6548.jpg");
        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));

        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_Register() - Form upload complete!");
                //Debug.Log(www.downloadHandler.text);
                api_token = www.GetResponseHeader("Authorization");
                api_refreshToken = www.GetResponseHeader("Token");
                Response_Register response = JsonConvert.DeserializeObject<Response_Register>(www.downloadHandler.text);
                GameDataManager.instance.userInfo_mine.nowdia.Value = response.rs.gameDia;
                GameDataManager.instance.userInfo_mine.nowGold.Value = response.rs.gameCoin;
                Debug.LogError("Dia - " + response.rs.gameDia);
                Debug.LogError("Coin - " + response.rs.gameCoin);
                GameDataManager.instance.state_userInfo = GameDataManager.UserInfoState.Success_DB_UserInfo;
                GameDataManager.instance.Upload_UserCustomMedel();
            }
        }
        
    }

    public static IEnumerator API_TESTLogin()
    {
        string url = api_url + "/login";
        //string ID = "TEST00000005";
        Request_Login request = new Request_Login(userID, "vrGdw1234");
        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));
        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_Login() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);

                Response_Login response = JsonConvert.DeserializeObject<Response_Login>(www.downloadHandler.text);

                if (response.status == 4110)
                {
                    Debug.Log("새로등록");
                    yield break;
                }

                api_token = www.GetResponseHeader("Authorization");
                api_refreshToken = www.GetResponseHeader("Token");
            }
        }
    }
    public static IEnumerator API_TESTGetUserInfo()
    {
        string url = api_url + "/v1/user/score?gameTitle=GODORI";

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_GetUserInfo() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);

                Response_GetUserInfo response = JsonConvert.DeserializeObject<Response_GetUserInfo>(www.downloadHandler.text);

                Debug.Log("변환된 변수: " + response.rs.gameScore[0].gameTitleId);
                //GameDataManager.instance.userInfo_mine.gameScores = response.rs.gameScore;
            }
        }
    }
    public static IEnumerator API_SetUserInfo(string _nickNm = "", string _avatarUrl = "")
    {
        string url = api_url + "/v1/user/change";


        Request_SetUserInfo request = new Request_SetUserInfo();
        request.nickNm = "ssss";
        request.avatarUrl = "https://www.naver.com/1.jpg";
        request.gameTitle = "GODORI";
        request.gameLvl = new Dictionary<string, int>();



        for (int i = 0; i < 1; i++)
        {
            request.gameLvl.Add(((i + 1) * 100 + 1000).ToString(), 3);
        }

        ////////////////
        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonConvert.SerializeObject(request));

        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            // www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {



                Debug.Log("API_SetUserInfo() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);

                // Response_GetUserInfo response = JsonConvert.DeserializeObject<Response_GetUserInfo>(www.downloadHandler.text);



            }
        }
    }
    public static IEnumerator API_GameStartNew(string gametype,string roomid)
    {
        //string activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        /* if (activeScene == "Scene_Lobby" || activeScene == "Scene_Customize")
         {
             yield break;
         }*/

        string url = api_url + "/v1/game/start";

        Request_GameStart request = new Request_GameStart();
        request.gameTitle = "GODORI";
        request.gameTitleNum = 1100;
        request.gameId = roomid; // Gaem 실행시 single // Multi 방 이름  
        if (gametype == "S")
        {
            request.gameType = "S";
        }
        else
        {
            request.gameType = "M";
            request.matchUserCds.Add(GameDataManager.instance.userInfo_mine.id);
            request.matchUserCds.Add(GameDataManager.instance.userInfos[0].id);
        }
        Debug.Log($"API_GameStart() - Start\n/ gameTitleNum : { request.gameTitleNum} / gameId : {request.gameId} / gameType : {request.gameType} /");

        if (request.matchUserCds != null)
        {
            for (int i = 0; i < request.matchUserCds.Count; i++)
            {
                Debug.Log(request.matchUserCds[i]);
            }
        }
        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));

        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            //www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_GameStart() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);
                Response_GameStart response = JsonConvert.DeserializeObject<Response_GameStart>(www.downloadHandler.text);


                Debug.LogFormat("[API_GameStart]\n/ status : {0} / message : {1} /"
                , response.status, response.message);

            }
        }
    }
    public static IEnumerator API_GameResult(string Winner, string Loser,int addGold)
    {
        string url = api_url + "/v1/game/end";

        Request_GameResult request = new Request_GameResult();
        request.gameTitle = "GODORI";
        request.gameTitleNum = 1100;
        request.gameId = GameDataManager.instance.localRoom.roomId; // Gaem 실행시 single // Multi 방 이름  
        request.gameType = GameDataManager.instance.localRoom.gameType;
        if(GameDataManager.instance.localRoom.gameType == "S") request.gameLvl = GameDataManager.instance.localRoom.Lv;
        request.gameResult = new List<Request_GameResult.Infos>();
        Request_GameResult.Infos temp = new Request_GameResult.Infos();
        temp.userCd = Winner;
        temp.userRank = 1; // 이긴경우
        temp.record = 100;
        temp.addGameCoin = addGold;
        temp.exitYn = "N";
        request.gameResult.Add(temp);
        temp.userCd = Loser;
        temp.userRank = 2; // 진 경우
        temp.record = 50;
        temp.addGameCoin = -addGold;
        temp.exitYn = "N";
        request.gameResult.Add(temp);

        request.addGameCoin = addGold;
        request.discontYn = "N";




        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));

        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);


            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_GameResult() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);
                Response_GameResult response = JsonConvert.DeserializeObject<Response_GameResult>(www.downloadHandler.text);
                Debug.LogFormat("[API_GameStart]\n/ status : {0} / message : {1} /"
               , response.status, response.message);


            }
        }
    }
    public static IEnumerator API_Ranking(Action<Response_Ranking> action)
    {
        string url = api_url + "/v1/game/ranking?gameTitle=" + "GODORI" + "&gameTitleNum=" + "1100" +"&gameType=M";

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);


            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_Ranking() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);
                Response_Ranking response = JsonConvert.DeserializeObject<Response_Ranking>(www.downloadHandler.text);
                //action.Invoke(response);
#if UNITY_EDITOR
                Debug.LogFormat("[API_Ranking]\n/ status : {0} / message : {1} /"
                    , response.status, response.message);

                if (response.ds != null)
                {
                    foreach (var item in response.ds)
                    {
                        Debug.LogFormat("[API_Ranking_List]\n/ userRank : {0} / nickNm : {1} / avatarUrl : {2} / userGrade : {3} / totLaddrPoint : {4} /"
                        , item.userRank, item.nickNm, item.avatarUrl, item.userGrade, item.totLaddrPoint);
                    }
                }
                if (response.rs != null)
                {
                    Debug.LogFormat("[API_Ranking_Mine]\n/ userRank : {0} / userCd : {1} / nickNm : {2} / avatarUrl : {3} / userGrade : {4} / totLaddrPoint : {5} /"
                    , response.rs.userRank, response.rs.userCd, response.rs.nickNm, response.rs.avatarUrl, response.rs.userGrade, response.rs.totLaddrPoint);
                }
                action.Invoke(response);
#endif
            }
        }
    }
    public static IEnumerator API_Record()
    {
        string url = api_url + "/v1/game/record";

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);


            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("API_Record() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);
                Response_Record response = JsonConvert.DeserializeObject<Response_Record>(www.downloadHandler.text);

#if UNITY_EDITOR
                Debug.LogFormat("[API_Record]\n/ status : {0} / message : {1} /"
                    , response.status, response.message);
                if (response.ds != null)
                {
                    foreach (var item in response.ds)
                    {
                        Debug.LogFormat("[API_Record_List]\n/ playEndDt : {0} / playEndTi : {1} / gameTitleNum : {2} / gameRank : {3} / laddrPoint : {4} / addLaddrPoint : {5} /"
                        , item.playEndDt, item.playEndTi, item.gameTitleNum, item.gameRank, item.laddrPoint, item.addLaddrPoint);
                    }
                }
#endif
            }
        }
    }
    public class UserList
    {
        public string timetamp;
        public string status;
        public string error;
        public string path;

    }
    public static IEnumerator API_UserList()
    {
        string url = api_url + "/api/v1/users?gameTitleNum=2000&filter&sttDt&endDt";

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);


            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("API_UserList() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);



                UserList response = JsonConvert.DeserializeObject<UserList>(www.downloadHandler.text);


                Debug.LogFormat($"[API_UserList]\n/ timetamp : {response.timetamp} " +
                    $"/ status : {response.status} /error : {response.error} /path : {response.path} /");


            }
        }
    }
    [System.Serializable]
    public class Request_BuyItem
    {
        public string userId;
        public string itemId;


    }
    
    public static IEnumerator API_UserItems()
    {
        string url = api_url + "/v1/user/items";

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);


            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("API_UserItems() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);

                Response_Items response = JsonConvert.DeserializeObject<Response_Items>(www.downloadHandler.text);
                if (response.status == 200)
                {
                    // _items = response;
                    Debug.Log("API_UserItems() - Form upload complete!");
                    GameDataManager.instance.SetItems(response);
                    // CustomizeManager.
                }
               // Debug.Log("API_UserItems() - Form upload complete!");
                //response_Items.Add(response);
            }
        }
    }
    [System.Serializable]
    public class Response_BuyItems
    {
        public int status;
        public string comment;
        public Infos rs;

        [System.Serializable]
        public class Infos
        {
            public string itemType;
            public int gameCoin;
            public int gameDia;
           
        }
    }

    public static IEnumerator API_BuyItem(string item)
    {
        string url = api_url + "/v1/order/buyItem";

        Request_BuyItem request = new Request_BuyItem();
        request.userId = GameDataManager.instance.userInfo_mine.id;
        request.itemId = item;

        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));
        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("API_BuyItem() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);
                Response_BuyItems response = JsonConvert.DeserializeObject<Response_BuyItems>(www.downloadHandler.text);
                if (response.status == 200)
                {

                    GameDataManager.instance.userInfo_mine.nowdia.Value = response.rs.gameDia;
                    GameDataManager.instance.userInfo_mine.nowGold.Value = response.rs.gameCoin;
                    GameDataManager.instance.ChangeMyResource();
                }
                else
                {
                    Debug.Log("Error - API_BuyItem()");
                }
                

            }
        }
    }
    [System.Serializable]
    public class Request_BuyDia
    {
        public string userId;
        public string ordId;
        public string ordItem;
        public int diaQty;



    }
    [System.Serializable]
    public class Respones_BuyDia
    {
        public int status;
        public string comment;
        public info rs;
        public class info
        {
            public string sucessYn;
            public int gameDia;
            public int gameCoin;
        }
    }



    public static IEnumerator API_BuyDia(string ordid, int diaQ)
    {
        string url = api_url + "/v1/order/buyDia";
        int changeDiaQ = 0; ;
        switch(diaQ)
        {
            case 0:
                changeDiaQ = 60;
                break;
            case 1:
                changeDiaQ = 99;
                break;
            case 2:
                changeDiaQ = 190;
                break;
            case 3:
                changeDiaQ = 550;
                break;
            case 4:
                changeDiaQ = 1000;
                break;
        }

        Request_BuyDia request = new Request_BuyDia();
        request.userId = GameDataManager.instance.userInfo_mine.id;
        request.ordId = ordid;
        request.ordItem = changeDiaQ.ToString()+"diaQty";
        request.diaQty = changeDiaQ;

        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));
        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("API_BuyDia() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);

                Respones_BuyDia response = JsonConvert.DeserializeObject<Respones_BuyDia>(www.downloadHandler.text);

                if(response.status == 200)
                {
                    GameDataManager.instance.userInfo_mine.nowdia.Value = response.rs.gameDia;
                    GameDataManager.instance.ChangeMyResource();
                }
                else
                {
                    Debug.LogError($"Error - API_BuyDia()");

                }    
            }
        }
        
    }
    [System.Serializable]
    public class Request_BuyGold
    {
        public string userCd;
        public string ordId;
        public int diaQty;
        public int coinQty;


    }

    [System.Serializable]
    public class Response_BuyGold
    {
        public int status;
        public string comment;
        public info rs;

        public class info
        {
            public string sucessYn;
            public int gameDia;
            public int gameCoin;
        }

    }
    public static IEnumerator API_BuyGold(string id,string ordid,int useDia, int takeGold)
    {
        string url = api_url + "/v1/order/buyCoin";

        Request_BuyGold request = new Request_BuyGold();
        request.userCd = id;
        request.ordId = ordid;
        request.diaQty = useDia;
        request.coinQty = takeGold;
        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));
        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.LogError("API_BuyGold() - Form upload complete!");
                Debug.LogError(www.downloadHandler.text);

                Response_BuyGold response = JsonConvert.DeserializeObject<Response_BuyGold>(www.downloadHandler.text);
                if(response.status == 200)
                {
                    GameDataManager.instance.userInfo_mine.nowdia.Value = response.rs.gameDia;
                    GameDataManager.instance.userInfo_mine.nowGold.Value = response.rs.gameCoin;
                }
                else
                {
                    Debug.LogError($"Error - API_BuyGold");
                }
            }
        }
    }
    // SucessYn -> 리필 가능 한지??
    public static IEnumerator API_BonusCheck()
    {
        string url = api_url + "/v1/bonus/check";

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);


            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("API_BonusCheck() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    [System.Serializable]
    public class Response_PlusCoin
    {
        public string userId;
        public int coinQty;
    }
    public static IEnumerator API_PlusCoin()
    {
        string url = api_url + "/v1/bonus/coin";

        Response_PlusCoin request = new Response_PlusCoin();
        request.userId = GameDataManager.instance.userInfo_mine.id;
        request.coinQty = 1000000;

        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));
        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("API_PlusCoin() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);

            }
        }
    }

    [System.Serializable]
    public class Response_CheckRefill
    {
        public int status;
        public infos rs;

        public class infos
        {
            public string sucessYn;
            public int refillCount;
            public int gameDia;
            public int gameCoin;
        }

    }
    public static IEnumerator API_CheckRefill()
    {
        string url = api_url + "/v1/bonus/refill";
        Response_CheckRefill request = new Response_CheckRefill();
        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));
        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);


            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("API_CheckRefill() - Form upload complete!");
                //Debug.Log(www.downloadHandler.text);
                Response_CheckRefill response = JsonConvert.DeserializeObject<Response_CheckRefill>(www.downloadHandler.text);

                GameDataManager.instance.userInfo_mine.nowdia.Value = response.rs.gameDia;
                GameDataManager.instance.userInfo_mine.nowGold.Value = response.rs.gameCoin;

            }
        }
    }
    public static IEnumerator API_CheckUserList()
    {
        string url = api_url + "/v1/user/admin?gameTitleNum=2000&filter&sttDt&endDt";
        Response_CheckRefill request = new Response_CheckRefill();
        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));
        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);


            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("API_CheckUserList() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
   

    /*  public static void SearchItemList(string id, out int goldPrice, out int diaPrice, out string orderYn)
      {
          string temp = "M";
          goldPrice = 0;
          diaPrice = 0;
          orderYn = "N";

          if (CustomizeManager.GetInstance.selectGenderNum == 0)
              temp += id;
          else
          {
              temp = "F";
              temp += id;
          }
          if (_items.ds.Exists(x => x.itemId == temp))
          {
              goldPrice = _items.ds.Find(x => x.itemId == temp).gameCoin;
              diaPrice = _items.ds.Find(x => x.itemId == temp).gameDia;
              orderYn = _items.ds.Find(x => x.itemId == temp).orderYn;
          }


      }*/
    /* public static UploadHandlerRaw GetUploadHandlerRaw(string jsonString)
     {
         byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);
         return new UploadHandlerRaw(jsonToSend);
     }*/
}
