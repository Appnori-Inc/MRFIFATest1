using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public  class AppnoriWebRequest_biliards : MonoBehaviour
{
#if DB_TEST
    public static readonly string api_url = "https://api.appnori.com";
#elif SERVER_CHINA
    public static readonly string api_url = "https://api.appnori.com";
#else
    public static readonly string api_url = "https://api.appnori.com";
#endif
    public static string api_token = "";
    public static string api_refreshToken = "";
    public static string userID = "T0579051954951";

    public static readonly string method_post = "POST";
    public static readonly string method_get = "GET";
    public static readonly string requestHeader_type = "Content-Type";
    public static readonly string requestHeader_value = "application/json";
    public static readonly string requestHeader_auth = "Authorization";

    //응답 성공 200, 실패 401.
    //ALL : 2000
    //JET_SKI : 2100
    //BEACH VOLLEYBALL : 2200
    //FISHING : 2300
    //FRISBEE : 2400
    //CYCLING : 2500
    //SWIMMING :2600 / 2610 (50m) / 2620 (100m) / 2630 (400m) / 2640 (1000m)

    [System.Serializable]
    public class UserInfos
    {
        public string userCd;
        public string avatarUrl;
        public string gameTitle;
        public List<GameScore> gameScore;
        public Dictionary<string, int> gameLvl;
        public int gameCoin;
        public int gameDia;
    }

    [System.Serializable]
    public class UserList
    {
        public string timetamp;
        public string status;
        public string error;
        public string path;

    }
    [System.Serializable]
    public class GameScore
    {
        public string gameTitleId;
        public int totLaddrPoint;
        public int totGameCoin;
        public int userGrade;
    }

    [System.Serializable]
    public class Request_Login
    {
        public string userCd;
        public string userPs;

        public Request_Login(string _userCd, string _userPs)
        {
            userCd = _userCd;
            userPs = _userPs;
        }
    }

    [System.Serializable]
    public class Response_Login
    {
        public int status;
        public UserInfos rs;


    }

    [System.Serializable]
    public class Request_Register
    {
        public string userCd;
        public string nickNm;
        public string avatarUrl;

        public Request_Register(string _userCd, string _nickNm, string _avatarUrl)
        {
            userCd = _userCd;
            nickNm = _nickNm;
            avatarUrl = _avatarUrl;
        }
    }

    [System.Serializable]
    public class Response_Register
    {
        public int status;
        public string message;
        public UserInfos rs;
    }

    [System.Serializable]
    public class Response_GetUserInfo
    {
        public int status;
        public string message;
        public UserInfos rs;
    }

    [System.Serializable]
    public class Request_SetUserInfo
    {
        public string avatarUrl;
        public string nickNm;
        public string gameTitle;
        public Dictionary<string, int> gameLvl;
    }

    [System.Serializable]
    public class Response_SetUserInfo
    {
        public int status;
        public string message;
    }


    [System.Serializable]
    public class Request_SetCustomCharData
    {
        public string userCd;
        public string gender;
        public string idFaceIM;
        public string hexSkinCM;
        public string hexEyeCM;
        public string hexEyebrowCM;
        public string idHairIM;
        public string hexHairCM;
        public string idWearIM;
        public string hexUpperCM;
        public string hexLowerCM;
        public string hexFootCM;
        public string hexPatternCM;
        public string idAccIM;
        public string hexAccCM;
        public string idFaceIF;
        public string hexSkinCF;
        public string hexEyeCF;
        public string hexEyebrowCF;
        public string idHairIF;
        public string hexHairCF;
        public string idWearIF;
        public string hexUpperCF;
        public string hexLowerCF;
        public string hexFootCF;
        public string hexPatternCF;
        public string idAccIF;
        public string hexAccCF;
    }

    [System.Serializable]
    public class Response_SetCustomCharData
    {
        public int status;
        public string message;
    }


    [System.Serializable]
    public class Response_GetCustomCharData
    {
        public int status;
        public string message;
        public Infos rs;

        [System.Serializable]
        public class Infos
        {
            public string userCd;
            public string gender;
            public string idFaceIM;
            public string hexSkinCM;
            public string hexEyeCM;
            public string hexEyebrowCM;
            public string idHairIM;
            public string hexHairCM;
            public string idWearIM;
            public string hexUpperCM;
            public string hexLowerCM;
            public string hexFootCM;
            public string hexPatternCM;
            public string idAccIM;
            public string hexAccCM;
            public string idFaceIF;
            public string hexSkinCF;
            public string hexEyeCF;
            public string hexEyebrowCF;
            public string idHairIF;
            public string hexHairCF;
            public string idWearIF;
            public string hexUpperCF;
            public string hexLowerCF;
            public string hexFootCF;
            public string hexPatternCF;
            public string idAccIF;
            public string hexAccCF;
        }
    }

    [System.Serializable]
    public class Request_GameStart
    {
        //JET_SKI : 2100.
        //BEACH VOLLEYBALL : 2200.
        //FISHING : 2300.
        //FRISBEE : 2400.
        //CYCLING: 2500.
        //SWIMMING : 2610 (50m) / 2620 (100m) / 2630 (400m) / 2640 (1000m).
        public string gameTitle;
        public int gameTitleNum;
        //멀티 : M.
        //싱글 : S.
        public string gameType;
        public List<string> matchUserCds;
        //VR 에서 생성한 게임 ID.
        //5명의 유저가 멀티 경기를 한 경우 참여한 사용자 모두 동일한 게임 ID 로 전달해야 함.
        public string gameId;
    }

    [System.Serializable]
    public class Response_GameStart
    {
        public int status;
        public string message;
    }

    [System.Serializable]
    public class Request_GameResult
    {
        //JET_SKI : 2100
        //BEACH VOLLEYBALL : 2200
        //FISHING : 2300
        //FRISBEE : 2400
        //CYCLING: 2500
        //SWIMMING : 2600
        public int gameTitleNum;
        public string gameId;
        public string gameTitle;
        //멀티 : M.
        //싱글 : S.
        public List<Infos> gameResult;
        //public int addLaddrPoint;
        public string discontYn;
        public int gameLvl;

        public string gameType;

        [System.Serializable]
        public class Infos
        {
            public string userCd;
            public int userRank;
            public int record;
            public int addGameCoin;
            public string exitYn;
        }

        public int addGameCoin;

    }

    [System.Serializable]
    public class Response_GameResult
    {
        public int status;
        public string message;
        public List<GameResult> ds;
        public Infos rs;

        [System.Serializable]
        public class GameResult
        {
            public string userCd;
            public int userRank;
            public int record;
        }

        [System.Serializable]
        public class Infos
        {
            public int userGrade;
            public int laddrPoint;
            public int addLaddrPoint;
            public string resultTitle;
        }
    }

    [System.Serializable]
    public class Request_FishingEnd
    {
        //FISHING : 2300.
        public int gameTitleNum;
        public string gameId;
        public string discontYn;
    }

    [System.Serializable]
    public class Response_FishingEnd
    {
        public int status;
        public string message;
    }

    [System.Serializable]
    public class Response_FishingCount
    {
        public int status;
        public string message;
        public List<Infos> ds;

        [System.Serializable]
        public class Infos
        {
            public string cmmCdNm;
            public int fishCount;
        }
    }

    [System.Serializable]
    public class Response_Ranking
    {
        public int status;
        public string message;
        public List<Infos> ds;
        public Infos rs;

        [System.Serializable]
        public class Infos
        {
            public string userCd;

            public string nickNm;
            public string avatarUrl;
            public int userGrade;
            public int totLaddrPoint;
            public int userRank;
        }
    }

    [System.Serializable]
    public class Response_Record
    {
        public int status;
        public string message;
        public List<Infos> ds;

        [System.Serializable]
        public class Infos
        {
            public string playEndDt;
            public string playEndTi;
            public int gameTitleNum;
            public int gameRank;
            public int laddrPoint;
            public string addLaddrPoint;
        }
    }

    [System.Serializable]
    public class Response_LeaderBoard
    {
        public int status;
        public string message;
        public List<Infos> ds;
        public Infos rs;

        [System.Serializable]
        public class Infos
        {
            public string userCd;

            public string nickNm;
            public string avatarUrl;
            //public int gameRank;
            //public int laddrPoint;
            //public string addLaddrPoint;
            public int record;
            //public string crtAt;
            //public string playEndDt;
            //public string playEndTi;
            //public int playMinute;
            public int userRank;
        }
    }

    [System.Serializable]
    public class Response_Notice
    {
        public int status;
        public string message;
        public Infos rs;

        [System.Serializable]
        public class Infos
        {
            public int id;
            public int index;
            public string createAt;
            public string updateAt;
            public int listSize;
            public string title;
            public string content;
        }
    }
    
    public static IEnumerator API_TESTRegister()
    {
        string url = api_url + "/v2/user";
        // 나중에 아이디만 다른곳에서 받아오면 됌
        //string ID = userID; 

        Request_Register request = new Request_Register(userID, "테스트아이디00000002", "123454321");
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
    public static IEnumerator API_GameStart(string gametype)
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
        request.gameId = "a7123ce8-bbd5-11ed-90b9-025fe15887063"; // Gaem 실행시 single // Multi 방 이름  
        if (gametype == "S")
        {
            request.gameType = "S";
        }
        else
        {
            request.gameType = "M";
        }
        Debug.LogFormat("API_GameStart() - Start\n/ gameTitleNum : {0} / gameId : {1} / gameType : {2} /"
                , request.gameTitleNum, request.gameId, request.gameType);

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
    public static IEnumerator API_GameResult(string gametype)
    {
        string url = api_url + "/v1/game/end";

        Request_GameResult request = new Request_GameResult();
        request.gameTitle = "GODORI";
        request.gameTitleNum = 1100;
        request.gameId = "a7123ce8-bbd5-11ed-90b9-025fe15887063"; // Gaem 실행시 single // Multi 방 이름  
        if (gametype == "S")
        {
            request.gameType = "S";
        }
        else
        {
            request.gameType = "M";
        }
        request.gameLvl = 5;
        request.gameResult = new List<Request_GameResult.Infos>();
        Request_GameResult.Infos temp = new Request_GameResult.Infos();
        temp.userCd = userID;
        temp.userRank = 1;
        temp.record = 133100;
        temp.addGameCoin = 100;
        temp.exitYn = "N";

        request.gameResult.Add(temp);
        temp.userCd = "C";
        temp.userRank = 2;
        temp.record = 164100;
        temp.addGameCoin = -100;
        temp.exitYn = "N";
        request.gameResult.Add(temp);

        request.addGameCoin = 100;
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
    public static IEnumerator API_Ranking(int gameTitleNum)
    {
        string url = api_url + "/v1/game/ranking?gameTitle=" + "GODORI" + "&gameTitleNum=" + gameTitleNum;

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
    public class Response_BuyItem
    {
        public string userId;
        public string itemId;


    }

    public string itemid = "F1005";
    public int dia = 10000;
    public int gold = 100000;

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
    public  Response_Items _items;
    public  IEnumerator API_UserItems()
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
                //Debug.Log(www.downloadHandler.text);

                Response_Items response = JsonConvert.DeserializeObject<Response_Items>(www.downloadHandler.text);
                if (response.status == 200)
                {
                    _items = response;
                   // CustomizeManager.
                }
                //response_Items.Add(response);
            }
        }
    }


    public IEnumerator API_BuyItem()
    {
        string url = api_url + "/v1/order/buyItem";

        Response_BuyItem request = new Response_BuyItem();
        request.userId = userID;
        request.itemId = itemid;

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

            }
        }
    }
    [System.Serializable]
    public class Response_BuyDia
    {
        public string userId;
        public string ordId;
        public string ordItem;
        public int diaQty;



    }

    public static IEnumerator API_BuyDia()
    {
        string url = api_url + "/v1/order/buyDia";

        Response_BuyDia request = new Response_BuyDia();
        request.userId = userID;
        request.ordId = "1111";
        request.ordItem = "1111";
        request.diaQty = 500000;

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

            }
        }
    }
    [System.Serializable]
    public class Response_BuyGold
    {
        public string userCd;
        public string ordId;
        public int diaQty;
        public int coinQty;


    }
    public static IEnumerator API_BuyGold()
    {
        string url = api_url + "/v1/order/buyCoin";

        Response_BuyGold request = new Response_BuyGold();
        request.userCd = userID;
        request.ordId = "1111";
        request.diaQty = 100;
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
                Debug.Log("API_BuyGold() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);

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
        request.userId = userID;
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
                Debug.Log(www.downloadHandler.text);
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
   
    public void SearchItemList(string id ,out int goldPrice, out int diaPrice, out string orderYn)
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
        goldPrice = _items.ds.Find(x => x.itemId == temp).gameCoin;
        diaPrice = _items.ds.Find(x => x.itemId == temp).gameDia;
        orderYn = _items.ds.Find(x => x.itemId == temp).orderYn;

       
    }

    private void Start()
    {
        StartCoroutine(API_TESTLogin());
        StartCoroutine(API_UserItems());
    }

    /* private void Start()
     {
         StartCoroutine(API_TESTLogin());
     }

     private void Update()
     {
         if (Input.GetKeyDown(KeyCode.Space))
         {
             StartCoroutine(API_TESTGetUserInfo());
         }
         *//*if (Input.GetKeyDown(KeyCode.K)) // 사용자 정보 변경 
         {
             StartCoroutine(API_SetUserInfo());
         }
         if (Input.GetKeyDown(KeyCode.I)) // 게임 시작 S
         {
             StartCoroutine(API_GameStart("S"));
         }
        *//* if (Input.GetKeyDown(KeyCode.O)) // 게임 시작 M
         {
             StartCoroutine(API_GameStart("M"));
         }*//*
         if (Input.GetKeyDown(KeyCode.Y)) // 게임 종료 S
         {
             StartCoroutine(API_GameResult("S"));
         }
         if (Input.GetKeyDown(KeyCode.T)) 
         {
             StartCoroutine(API_Ranking(1100));
         }*//*

         if (Input.GetKeyDown(KeyCode.A))
         {
             StartCoroutine(API_UserItems());
         }
         if (Input.GetKeyDown(KeyCode.S))
         {
             StartCoroutine(API_BuyItem());
         }
         if (Input.GetKeyDown(KeyCode.D))
         {
             StartCoroutine(API_BuyDia());
         }
         if (Input.GetKeyDown(KeyCode.F))
         {
             StartCoroutine(API_BuyGold());
         }
         if (Input.GetKeyDown(KeyCode.Z))
         {
             StartCoroutine(API_BonusCheck());
         }
         if (Input.GetKeyDown(KeyCode.X))
         {
             StartCoroutine(API_PlusCoin());
         }
         if (Input.GetKeyDown(KeyCode.C))
         {
             StartCoroutine(API_CheckRefill());
         }
         if (Input.GetKeyDown(KeyCode.V))
         {
             StartCoroutine(API_CheckUserList());
         }
     }*/


    public static UploadHandlerRaw GetUploadHandlerRaw(string jsonString)
    {
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);
        return new UploadHandlerRaw(jsonToSend);
    }

}
