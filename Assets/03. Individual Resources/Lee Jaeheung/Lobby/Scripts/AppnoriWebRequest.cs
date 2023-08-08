using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;



/*[System.Serializable]
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
public class GameScore
{
    public string gameTitleId;
    public int totLaddrPoint;
    public int totGameCoin;
    public int userGrade;
}
public class Response_GetUserInfo
{
    public int status;
    public string message;
    public UserInfos rs;
}*/


public partial class AppnoriWebRequest
{
#if DB_TEST
    public static readonly string api_url = "https://api-dev.appnori.com";
#elif SERVER_CHINA
    public static readonly string api_url = "https://api-dev.appnori.com";
#else
    public static readonly string api_url = "https://api-dev.appnori.com";
#endif
    public static string api_token = "";
    public static string api_refreshToken = "";
    public static string userID = "PI7229911703373333509";

    public static readonly string method_post = "POST";
    public static readonly string method_get = "GET";
    public static readonly string requestHeader_type = "Content-Type";
    public static readonly string requestHeader_value = "application/json";
    public static readonly string requestHeader_auth = "Authorization";
    public static readonly string requestHeader_gameVersion = "GameVersion";

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
        public string gameThema;
    }
    [System.Serializable]
    public class GameScore
    {
        public string gameTitleId;
        public int totLaddrPoint;
        public int userGrade;
    }


    [System.Serializable]
    public class Request_Login
    {
        public string userCd;
        public string userPs;
        public string gameTitle;

        public Request_Login(string _userCd, string _userPs)
        {
            userCd = _userCd;
            userPs = _userPs;
            gameTitle = "GODORI";
        }
    }

    [System.Serializable]
    public class Response_Login
    {
        public int status;
        public message error;
        public UserInfos rs;

        public struct message
        {
            public string code;
            public string data;
        }
    }

    [System.Serializable]
    public class Request_Register
    {
        public string userCd;
        public string nickNm;
        public string avatarUrl;
        public string gameTitle;

        public Request_Register(string _userCd, string _nickNm, string _avatarUrl)
        {
            userCd = _userCd;
            nickNm = _nickNm;
            avatarUrl = _avatarUrl;
            gameTitle = "GODORI";
        }
    }

    [System.Serializable]
    public class Response_Register
    {
        public int status;
        public message error;
        public UserInfos rs;

        public struct message
        {
            public string code;
            public string data;
        }
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
        public string gameThema;
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
            public string gameThema;
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
        public List<string> matchUserCds = new List<string>();
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
        public List<Infos> ds;
        public Infos rs;

        [System.Serializable]
        public class Infos
        {
            public string nickNm;
            public string avatarUrl;
            public int userGrade;
            public int totLaddrPoint;
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


    public static IEnumerator API_Login()
    {
        GameDataManager.instance.state_userInfo = GameDataManager.UserInfoState.Start_DB_UserInfo;
        string url = api_url + "/login";

        GameDataManager.UserInfo userInfo = GameDataManager.instance.userInfo_mine;
        Debug.Log("Login : ID - " + userInfo.id);
        Request_Login request = new Request_Login(userInfo.id, "vrGdw1234");
        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));

        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                GameDataManager.instance.state_userInfo = GameDataManager.UserInfoState.Failed_DB_UserInfo;
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_Login() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);

                Response_Login response = JsonConvert.DeserializeObject<Response_Login>(www.downloadHandler.text);

                if (response.status == 0
                    && response.error.code == "4110")
                {
                    GameDataManager.instance.UserRegister();
                    yield break;
                }
                

                api_token = www.GetResponseHeader("Authorization");
                api_refreshToken = www.GetResponseHeader("Token");

              
                GameDataManager.instance.userInfo_mine.nowdia.Value = response.rs.gameDia;
                GameDataManager.instance.userInfo_mine.nowGold.Value = response.rs.gameCoin;
                GameDataManager.instance.userInfo_mine.totalLaddr1 = response.rs.gameScore[0].totLaddrPoint;
                GameDataManager.instance.userInfo_mine.totalLaddr2 = response.rs.gameScore[1].totLaddrPoint;
                GameDataManager.instance.userInfo_mine.thema = response.rs.gameThema;
                GameDataManager.image_url_mine = response.rs.avatarUrl;
                GameDataManager.instance.ChangeMyResource();

                //GameDataManager.instance.userInfo_mine.gameScores = response.rs.gameScore;

                /*int[] clearLevelDatas_db = new int[response.rs.gameLvl.Count];
                for (int i = 0; i < clearLevelDatas_db.Length; i++)
                {
                    clearLevelDatas_db[i] = response.rs.gameLvl[((i + 1) * 100 + 2000).ToString()];
                }

               // string change_nick = (response.rs.nickNm != GameDataManager.origin_nick_mine) ? GameDataManager.origin_nick_mine : "";
                string change_url = (response.rs.avatarUrl != GameDataManager.image_url_mine) ? GameDataManager.image_url_mine : "";

                bool isUpdataData = false;

                for (int i = 0; i < clearLevelDatas_db.Length; i++)
                {
                    if (clearLevelDatas_db[i] > GameDataManager.clearLevelDatas[i])
                    {
                        GameDataManager.clearLevelDatas[i] = clearLevelDatas_db[i];
                    }
                    else if (clearLevelDatas_db[i] < GameDataManager.clearLevelDatas[i])
                    {
                        isUpdataData = true;
                    }
                }

                if (isUpdataData)
                {
                    //GameDataManager.instance.Upload_UserData();
                }*/
#if UNITY_EDITOR
                /*Debug.LogFormat("[API_Login]\n/ status : {0} / message : {1} / userCd : {2} / nickNm : {3} / avatarUrl : {4} /"
                    , response.status, response.message, response.rs.userCd, response.rs.nickNm, response.rs.avatarUrl);

                for (int i = 0; i < response.rs.gameScore.Count; i++)
                {
                    Debug.LogFormat("[API_Login_GameScore]\n/ gameTitleNum : {0} / totLaddrPoint : {1} / userGrade : {2} /"
                    , response.rs.gameScore[i].gameTitleNum, response.rs.gameScore[i].totLaddrPoint, response.rs.gameScore[i].userGrade);
                }
                if (response.rs.gameLvl != null)
                {
                    foreach (var item in response.rs.gameLvl)
                    {
                        Debug.LogFormat("[API_Login_GameLvl]\n/ gameTitleNum : {0} / clearLvl : {1} /"
                   , item.Key, item.Value);
                    }
                }*/
#endif

                GameDataManager.instance.state_userInfo = GameDataManager.UserInfoState.Success_DB_UserInfo;
                GameDataManager.instance.Download_UserCustomMedel();
                //GameDataManager.instance.state_userInfo = GameDataManager.UserInfoState.Completed;
            }
        }
    }

    public static IEnumerator API_Register()
    {
        string url = api_url + "/v2/user";

        GameDataManager.UserInfo userInfo = GameDataManager.instance.userInfo_mine;
        if (string.IsNullOrWhiteSpace(GameDataManager.image_url_mine))
        {
            GameDataManager.image_url_mine = "null";
        }
        Request_Register request = new Request_Register(userInfo.id, GameDataManager.origin_nick_mine, GameDataManager.image_url_mine);
        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));

        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

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
                //GameDataManager.instance.userInfo_mine.gameScores = response.rs.gameScore;

                GameDataManager.clearLevelDatas = new int[10];
                for (int i = 0; i < GameDataManager.clearLevelDatas.Length; i++)
                {
                    if (i >= response.rs.gameLvl.Count)
                    {
                        GameDataManager.clearLevelDatas[i] = 0;
                    }
                    else
                    {
                        GameDataManager.clearLevelDatas[i] = response.rs.gameLvl[((i + 1) * 100 + 2000).ToString()];
                    }
                }


#if UNITY_EDITOR
              /*  Debug.LogFormat("[API_Register]\n/ status : {0} / message : {1} / userCd : {2} / nickNm : {3} / avatarUrl : {4} /"
                , response.status, response.message, response.rs.userCd, response.rs.nickNm, response.rs.avatarUrl);

                if (response.rs.gameScore != null)
                {
                    for (int i = 0; i < response.rs.gameScore.Count; i++)
                    {
                        Debug.LogFormat("[API_Register_GameScore]\n/ gameTitleNum : {0} / totLaddrPoint : {1} / userGrade : {2} /"
                        , response.rs.gameScore[i].gameTitleNum, response.rs.gameScore[i].totLaddrPoint, response.rs.gameScore[i].userGrade);
                    }
                }*/
                if (response.rs.gameLvl != null)
                {
                    foreach (var item in response.rs.gameLvl)
                    {
                        Debug.LogFormat("[API_Register_GameLvl]\n/ gameTitleNum : {0} / clearLvl : {1} /"
                        , item.Key, item.Value);
                    }
                }
#endif
                GameDataManager.instance.state_userInfo = GameDataManager.UserInfoState.Success_DB_UserInfo;
                GameDataManager.instance.Upload_UserCustomMedel();
            }
        }
    }

    /*public static IEnumerator API_SetUserInfo(string _nickNm = "", string _avatarUrl = "")
    {
        string url = api_url + "/api/v1/user/change";

        GameDataManager.UserInfo userInfo = GameDataManager.instance.userInfo_mine;

        Request_SetUserInfo request = new Request_SetUserInfo();
        request.nickNm = _nickNm;
        request.avatarUrl = _avatarUrl;
        request.gameLvl = new Dictionary<string, int>();

        for (int i = 0; i < 7; i++)
        {
            request.gameLvl.Add(((i + 1) * 100 + 2000).ToString(), GameDataManager.clearLevelDatas[i]);
        }

        ////////////////
        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonConvert.SerializeObject(request));

        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_SetUserInfo() - Form upload complete!");
                //Debug.Log(www.downloadHandler.text);
                Response_SetUserInfo response = JsonConvert.DeserializeObject<Response_SetUserInfo>(www.downloadHandler.text);

#if UNITY_EDITOR
                Debug.LogFormat("[API_SetUserInfo]\n/ status : {0} / message : {1} /"
                , response.status, response.message);
#endif
            }
        }
    }*/

    public static IEnumerator API_GetUserInfo()
    {
        string url = api_url + "/api/v1/user/score";

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_GetUserInfo() - Form upload complete!");
                //Debug.Log(www.downloadHandler.text);

                Response_GetUserInfo response = JsonConvert.DeserializeObject<Response_GetUserInfo>(www.downloadHandler.text);

                //GameDataManager.instance.userInfo_mine.gameScores = response.rs.gameScore;

#if UNITY_EDITOR
                foreach (var item in response.rs.gameLvl)
                {
                    Debug.Log(item.Key + " : " + item.Value);
                }

               /* Debug.LogFormat("[API_GetUserInfo]\n/ status : {0} / message : {1} / userCd : {2} / nickNm : {3} / avatarUrl : {4} /"
                    , response.status, response.message, response.rs.userCd, response.rs.nickNm, response.rs.avatarUrl);

                if (response.rs.gameScore != null)
                {
                    for (int i = 0; i < response.rs.gameScore.Count; i++)
                    {
                        Debug.LogFormat("[API_GetUserInfo_GameScore]\n/ gameTitleNum : {0} / totLaddrPoint : {1} / userGrade : {2} /"
                        , response.rs.gameScore[i].gameTitleNum, response.rs.gameScore[i].totLaddrPoint, response.rs.gameScore[i].userGrade);
                    }
                }*/

                foreach (var item in response.rs.gameLvl)
                {
                    Debug.LogFormat("[API_GetUserInfo_GameLvl]\n/ gameTitleNum : {0} / clearLvl : {1} /"
                    , item.Key, item.Value);
                }
#endif
            }
        }
    }


    public static IEnumerator API_SetCustomCharData(CustomModelData[] modelDatas)
    {
        Debug.Log("API_SetCustomCharData() - API_SetCustomCharData");
        string url = api_url + "/v1/user";

        GameDataManager.UserInfo userInfo = GameDataManager.instance.userInfo_mine;

        Request_SetCustomCharData customCharData = new Request_SetCustomCharData();
        customCharData.userCd = userInfo.id;
        customCharData.gender = userInfo.customModelData.Gender;

        customCharData.idFaceIM = modelDatas[0].ID_Face_I;
        customCharData.hexSkinCM = modelDatas[0].Hex_Skin_C;
        customCharData.hexEyeCM = modelDatas[0].Hex_Eye_C;
        customCharData.hexEyebrowCM = modelDatas[0].Hex_Eyebrow_C;

        customCharData.idHairIM = modelDatas[0].ID_Hair_I;
        customCharData.hexHairCM = modelDatas[0].Hex_Hair_C;

        customCharData.idWearIM = modelDatas[0].ID_Wear_I;
        customCharData.hexUpperCM = modelDatas[0].Hex_Upper_C;
        customCharData.hexLowerCM = modelDatas[0].Hex_Lower_C;
        customCharData.hexFootCM = modelDatas[0].Hex_Foot_C;
        customCharData.hexPatternCM = modelDatas[0].Hex_Pattern_C;

        customCharData.idAccIM = modelDatas[0].ID_Acc_I;
        customCharData.hexAccCM = modelDatas[0].Hex_Acc_C;

        customCharData.idFaceIF = modelDatas[1].ID_Face_I;
        customCharData.hexSkinCF = modelDatas[1].Hex_Skin_C;
        customCharData.hexEyeCF = modelDatas[1].Hex_Eye_C;
        customCharData.hexEyebrowCF = modelDatas[1].Hex_Eyebrow_C;

        customCharData.idHairIF = modelDatas[1].ID_Hair_I;
        customCharData.hexHairCF = modelDatas[1].Hex_Hair_C;

        customCharData.idWearIF = modelDatas[1].ID_Wear_I;
        customCharData.hexUpperCF = modelDatas[1].Hex_Upper_C;
        customCharData.hexLowerCF = modelDatas[1].Hex_Lower_C;
        customCharData.hexFootCF = modelDatas[1].Hex_Foot_C;
        customCharData.hexPatternCF = modelDatas[1].Hex_Pattern_C;

        customCharData.idAccIF = modelDatas[1].ID_Acc_I;
        customCharData.hexAccCF = modelDatas[1].Hex_Acc_C;
        customCharData.gameThema = GameDataManager.instance.userInfo_mine.thema;

        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(customCharData));

        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_SetCustomCharData() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);

                Response_SetCustomCharData response = JsonConvert.DeserializeObject<Response_SetCustomCharData>(www.downloadHandler.text);
#if UNITY_EDITOR
                Debug.LogFormat("[API_SetCustomCharData]\n/ status : {0} / message : {1} /"
                , response.status, response.message);
#endif
            }
        }
    }

    public static IEnumerator API_SetCustomCharData()
    {
        string url = api_url + "/v1/user";

        GameDataManager.UserInfo userInfo = GameDataManager.instance.userInfo_mine;
        Request_SetCustomCharData customCharData = new Request_SetCustomCharData();
        customCharData.userCd = GameDataManager.instance.userInfo_mine.id;
        customCharData.gender = "m";

        CustomModelData customModelData = CustomModelSettingCtrl.GetRandomModelData(0);

        customCharData.idFaceIM = customModelData.ID_Face_I;
        customCharData.hexSkinCM = customModelData.Hex_Skin_C;
        customCharData.hexEyeCM = customModelData.Hex_Eye_C;
        customCharData.hexEyebrowCM = customModelData.Hex_Eyebrow_C;

        customCharData.idHairIM = customModelData.ID_Hair_I;
        customCharData.hexHairCM = customModelData.Hex_Hair_C;

        customCharData.idWearIM = customModelData.ID_Wear_I;
        customCharData.hexUpperCM = customModelData.Hex_Upper_C;
        customCharData.hexLowerCM = customModelData.Hex_Lower_C;
        customCharData.hexFootCM = customModelData.Hex_Foot_C;
        customCharData.hexPatternCM = customModelData.Hex_Pattern_C;

        customCharData.idAccIM = customModelData.ID_Acc_I;
        customCharData.hexAccCM = customModelData.Hex_Acc_C;
        GameDataManager.instance.userInfo_mine.customModelDatas[0] = customModelData;

       customModelData = CustomModelSettingCtrl.GetRandomModelData(1);

        customCharData.idFaceIF = customModelData.ID_Face_I;
        customCharData.hexSkinCF = customModelData.Hex_Skin_C;
        customCharData.hexEyeCF = customModelData.Hex_Eye_C;
        customCharData.hexEyebrowCF = customModelData.Hex_Eyebrow_C;

        customCharData.idHairIF = customModelData.ID_Hair_I;
        customCharData.hexHairCF = customModelData.Hex_Hair_C;

        customCharData.idWearIF = customModelData.ID_Wear_I;
        customCharData.hexUpperCF = customModelData.Hex_Upper_C;
        customCharData.hexLowerCF = customModelData.Hex_Lower_C;
        customCharData.hexFootCF = customModelData.Hex_Foot_C;
        customCharData.hexPatternCF = customModelData.Hex_Pattern_C;

        customCharData.idAccIF = customModelData.ID_Acc_I;
        customCharData.hexAccCF = customModelData.Hex_Acc_C;
        customCharData.gameThema = "T5000";

        GameDataManager.instance.userInfo_mine.customModelDatas[1] = customModelData;

        //customCharData.idFaceIM = PlayerPrefs.GetString("id_item_face_m", "1000");
        //customCharData.hexSkinCM = PlayerPrefs.GetString("hex_color_skin_m", "FFAA80");
        //customCharData.hexEyeCM = PlayerPrefs.GetString("hex_color_eye_m", "000000");
        //customCharData.hexEyebrowCM = PlayerPrefs.GetString("hex_color_eyebrow_m", "000000");

        //customCharData.idHairIM = PlayerPrefs.GetString("id_item_hair_m", "2000");
        //customCharData.hexHairCM = PlayerPrefs.GetString("hex_color_hair_m", "000000");

        //customCharData.idWearIM = PlayerPrefs.GetString("id_item_wear_m", "3000");
        //customCharData.hexUpperCM = PlayerPrefs.GetString("hex_color_upper_m", "808080");
        //customCharData.hexLowerCM = PlayerPrefs.GetString("hex_color_lower_m", "808080");
        //customCharData.hexFootCM = PlayerPrefs.GetString("hex_color_foot_m", "808080");
        //customCharData.hexPatternCM = PlayerPrefs.GetString("hex_color_pattern_m", "000000");

        //customCharData.idAccIM = PlayerPrefs.GetString("id_item_acc_m", "4000");
        //customCharData.hexAccCM = PlayerPrefs.GetString("hex_color_acc_m", "808080");

        //customCharData.idFaceIF = PlayerPrefs.GetString("id_item_face_f", "1000");
        //customCharData.hexSkinCF = PlayerPrefs.GetString("hex_color_skin_f", "FFAA80");
        //customCharData.hexEyeCF = PlayerPrefs.GetString("hex_color_eye_f", "000000");
        //customCharData.hexEyebrowCF = PlayerPrefs.GetString("hex_color_eyebrow_f", "000000");

        //customCharData.idHairIF = PlayerPrefs.GetString("id_item_hair_f", "2000");
        //customCharData.hexHairCF = PlayerPrefs.GetString("hex_color_hair_f", "000000");

        //customCharData.idWearIF = PlayerPrefs.GetString("id_item_wear_f", "3000");
        //customCharData.hexUpperCF = PlayerPrefs.GetString("hex_color_upper_f", "808080");
        //customCharData.hexLowerCF = PlayerPrefs.GetString("hex_color_lower_f", "808080");
        //customCharData.hexFootCF = PlayerPrefs.GetString("hex_color_foot_f", "808080");
        //customCharData.hexPatternCF = PlayerPrefs.GetString("hex_color_pattern_f", "000000");

        //customCharData.idAccIF = PlayerPrefs.GetString("id_item_acc_f", "4000");
        //customCharData.hexAccCF = PlayerPrefs.GetString("hex_color_acc_f", "808080");

        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(customCharData));

        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_SetCustomCharData() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);

                Response_SetCustomCharData response = JsonConvert.DeserializeObject<Response_SetCustomCharData>(www.downloadHandler.text);
                GameDataManager.instance.state_userInfo = GameDataManager.UserInfoState.Completed;

#if UNITY_EDITOR
               /* Debug.LogFormat("[API_SetCustomCharData]\n/ status : {0} / message : {1} /"
                , response.status, response.message);*/
               
#endif
                
            }
        }
    }

    public static IEnumerator API_GetCustomCharData(Action<Response_GetCustomCharData> action)
    {
        string url = api_url + "/v1/user";

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);

                action.Invoke(null);
            }
            else
            {
                Debug.Log("API_GetCustomCharData() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);
                Response_GetCustomCharData response = JsonConvert.DeserializeObject<Response_GetCustomCharData>(www.downloadHandler.text);

                action.Invoke(response);

#if UNITY_EDITOR
                Debug.LogFormat($"[API_GetCustomCharData]\n/ status : {response.status} / rs : {response.rs} /");
#endif
            }
        }
    }

    public static IEnumerator API_GetCustomCharData()
    {
        GameDataManager.instance.state_userInfo = GameDataManager.UserInfoState.Start_DB_ModelData;
        string url = api_url + "/v1/user";

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);

                GameDataManager.instance.SetMyCustomModelData(null);
                //GameDataManager.instance.state_userInfo = GameDataManager.UserInfoState.Completed;
            }
            else
            {
                Debug.Log("API_GetCustomCharData() - Form upload complete!");
                Debug.Log(www.downloadHandler.text);
                Response_GetCustomCharData response = JsonConvert.DeserializeObject<Response_GetCustomCharData>(www.downloadHandler.text);
#if UNITY_EDITOR
                Debug.LogFormat("[API_GetCustomCharData]\n/ status : {0}  /"
                , response.status);
                Debug.Log($"{response.rs.gender}/{response.rs.hexAccCF}/" +
                    $"{response.rs.hexAccCM}/{response.rs.hexEyebrowCF}/{response.rs.hexEyebrowCM}" +
                    $"/{response.rs.hexEyeCF}/{response.rs.hexEyeCM}/{response.rs.hexHairCF}/" +
                    $"{response.rs.gender}/{response.rs.gender}/{response.rs.gender}/{response.rs.gender}/");
#endif

                if (response.rs == null)
                {
                    GameDataManager.instance.SetMyCustomModelData(null);
                    //GameDataManager.instance.state_userInfo = GameDataManager.UserInfoState.Completed;
                }
                else
                {
                    CustomModelData modelData = new CustomModelData(); //GameDataManager.userInfo_mine.customModelData;
                    CustomModelData modelData1 = new CustomModelData(); //GameDataManager.userInfo_mine.customModelData;

                    if (response.rs.gender == "m")
                    {
                        modelData1.Gender = "f";

                        modelData1.ID_Face_I = response.rs.idFaceIF;
                        modelData1.Hex_Skin_C = response.rs.hexSkinCF;
                        modelData1.Hex_Eye_C = response.rs.hexEyeCF;
                        modelData1.Hex_Eyebrow_C = response.rs.hexEyebrowCF;

                        modelData1.ID_Hair_I = response.rs.idHairIF;
                        modelData1.Hex_Hair_C = response.rs.hexHairCF;

                        modelData1.ID_Wear_I = response.rs.idWearIF;
                        modelData1.Hex_Upper_C = response.rs.hexUpperCF;
                        modelData1.Hex_Lower_C = response.rs.hexLowerCF;
                        modelData1.Hex_Foot_C = response.rs.hexFootCF;
                        modelData1.Hex_Pattern_C = response.rs.hexPatternCF;

                        modelData1.ID_Acc_I = response.rs.idAccIF;
                        modelData1.Hex_Acc_C = response.rs.hexAccCF;
                        GameDataManager.instance.userInfo_mine.customModelDatas[1] = modelData1;
                        modelData.Gender = "m";

                        modelData.ID_Face_I = response.rs.idFaceIM;
                        modelData.Hex_Skin_C = response.rs.hexSkinCM;
                        modelData.Hex_Eye_C = response.rs.hexEyeCM;
                        modelData.Hex_Eyebrow_C = response.rs.hexEyebrowCM;

                        modelData.ID_Hair_I = response.rs.idHairIM;
                        modelData.Hex_Hair_C = response.rs.hexHairCM;

                        modelData.ID_Wear_I = response.rs.idWearIM;
                        modelData.Hex_Upper_C = response.rs.hexUpperCM;
                        modelData.Hex_Lower_C = response.rs.hexLowerCM;
                        modelData.Hex_Foot_C = response.rs.hexFootCM;
                        modelData.Hex_Pattern_C = response.rs.hexPatternCM;

                        modelData.ID_Acc_I = response.rs.idAccIM;
                        modelData.Hex_Acc_C = response.rs.hexAccCM;
                        GameDataManager.instance.userInfo_mine.customModelDatas[0] = modelData;
                       

                    }
                    else
                    {
                        modelData1.Gender = "m";

                        modelData1.ID_Face_I = response.rs.idFaceIM;
                        modelData1.Hex_Skin_C = response.rs.hexSkinCM;
                        modelData1.Hex_Eye_C = response.rs.hexEyeCM;
                        modelData1.Hex_Eyebrow_C = response.rs.hexEyebrowCM;

                        modelData1.ID_Hair_I = response.rs.idHairIM;
                        modelData1.Hex_Hair_C = response.rs.hexHairCM;

                        modelData1.ID_Wear_I = response.rs.idWearIM;
                        modelData1.Hex_Upper_C = response.rs.hexUpperCM;
                        modelData1.Hex_Lower_C = response.rs.hexLowerCM;
                        modelData1.Hex_Foot_C = response.rs.hexFootCM;
                        modelData1.Hex_Pattern_C = response.rs.hexPatternCM;

                        modelData1.ID_Acc_I = response.rs.idAccIM;
                        modelData1.Hex_Acc_C = response.rs.hexAccCM;
                        GameDataManager.instance.userInfo_mine.customModelDatas[0] = modelData1;
                        modelData.Gender = "f";

                        modelData.ID_Face_I = response.rs.idFaceIF;
                        modelData.Hex_Skin_C = response.rs.hexSkinCF;
                        modelData.Hex_Eye_C = response.rs.hexEyeCF;
                        modelData.Hex_Eyebrow_C = response.rs.hexEyebrowCF;

                        modelData.ID_Hair_I = response.rs.idHairIF;
                        modelData.Hex_Hair_C = response.rs.hexHairCF;

                        modelData.ID_Wear_I = response.rs.idWearIF;
                        modelData.Hex_Upper_C = response.rs.hexUpperCF;
                        modelData.Hex_Lower_C = response.rs.hexLowerCF;
                        modelData.Hex_Foot_C = response.rs.hexFootCF;
                        modelData.Hex_Pattern_C = response.rs.hexPatternCF;

                        modelData.ID_Acc_I = response.rs.idAccIF;
                        modelData.Hex_Acc_C = response.rs.hexAccCF;
                        GameDataManager.instance.userInfo_mine.customModelDatas[1] = modelData;
                       
                    }

                    GameDataManager.instance.SetMyCustomModelData(modelData);
                   
                }
            }
            GameDataManager.instance.Check_DisconnectLog();
        }
    }

    public static IEnumerator API_GameStart()
    {
        string activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (activeScene == "Scene_Lobby" || activeScene == "Scene_Customize")
        {
            yield break;
        }

        string url = api_url + "/api/v1/game/start";

        Request_GameStart request = new Request_GameStart();

        request.gameTitleNum = GetGameTitleNum();
        request.gameId = GameDataManager.SetGameGUID();

        //if (GameDataManager.instance.playType == GameDataManager.PlayType.Multi || GameDataManager.instance.playType == GameDataManager.PlayType.Multi_Solo)
        //{
        //    request.gameType = "M";
        //    request.matchUserCds = new List<string>();
        //    for (int i = 0; i < GameDataManager.instance.userInfos.Count; i++)
        //    {
        //        request.matchUserCds.Add((GameDataManager.instance.userInfos[i].id.Contains("CPU_") ? "C" : GameDataManager.instance.userInfos[i].id));
        //    }
        //}
        //else
        //{
        //    request.gameType = "S";
        //}
#if UNITY_EDITOR
        Debug.LogFormat("API_GameStart() - Start\n/ gameTitleNum : {0} / gameId : {1} / gameType : {2} /"
                , request.gameTitleNum, request.gameId, request.gameType);

        if (request.matchUserCds != null)
        {
            for (int i = 0; i < request.matchUserCds.Count; i++)
            {
                Debug.Log(request.matchUserCds[i]);
            }
        }
#endif
        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));

        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_GameStart() - Form upload complete!");
                //Debug.Log(www.downloadHandler.text);
                Response_GameStart response = JsonConvert.DeserializeObject<Response_GameStart>(www.downloadHandler.text);

#if UNITY_EDITOR
                Debug.LogFormat("[API_GameStart]\n/ status : {0} / message : {1} /"
                , response.status, response.message);
#endif
            }
        }
    }

    /*
      // 낚시 점수  
        AppnoriWebRequest.Request_GameResult gameResult = new AppnoriWebRequest.Request_GameResult();
        gameResult.gameTitleNum = AppnoriWebRequest.GetGameTitleNum();
        gameResult.gameId = GameDataManager.game_guid;
        gameResult.gameResult = new List<Request_GameResult.Infos>();
        Request_GameResult.Infos result = new Request_GameResult.Infos();
        result.userCd = GameDataManager.userInfo_mine.id;
        result.userRank = 1;
        result.record = 2;
        gameResult.gameResult.Add(result);
        gameResult.addLaddrPoint = 0;
        gameResult.discontYn = "N";
        gameResult.gameLvl = 0;
        StartCoroutine(AppnoriWebRequest.API_GameResult(gameResult));
    */

    public static IEnumerator API_GameResult(Request_GameResult request, Action<Response_GameResult> action = null)
    {
        string url = api_url + "/v1/game/end";
#if UNITY_EDITOR
        Debug.LogFormat("API_GameResult() - Start\n/ gameTitleNum : {0} / gameId : {1} / gameType : {2} /"
        , request.gameTitleNum, request.gameId, request.gameType);

        if (request.gameResult != null)
        {
            for (int i = 0; i < request.gameResult.Count; i++)
            {
                Debug.Log(request.gameResult[i].userRank);
                Debug.Log(request.gameResult[i].userCd);
                Debug.Log(request.gameResult[i].record);
            }
        }
#endif
        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));

        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_GameResult() - Form upload complete!");
                //Debug.Log(www.downloadHandler.text);
                Response_GameResult response = JsonConvert.DeserializeObject<Response_GameResult>(www.downloadHandler.text);

                GameDataManager.instance.state_userInfo = GameDataManager.UserInfoState.Completed;

                //if (GameDataManager.instance.playType == GameDataManager.PlayType.Multi || GameDataManager.instance.playType == GameDataManager.PlayType.Multi_Solo)
                //{
                //    PublicGameUIManager.GetInstance.SetResultAddPoint(response.rs.addLaddrPoint);
                //}

                //if (GameDataManager.instance.playType == GameDataManager.PlayType.Single && GameDataManager.instance.gameType == GameDataManager.GameType.Swimming)
                //{
                //    PublicGameUIManager.leaderBoard.MyScoreUpdate();
                //}

                if (action != null)
                {
                    action.Invoke(response);
                }

#if UNITY_EDITOR
                Debug.LogFormat("[API_GameResult]\n/ status : {0} / message : {1} /"
                , response.status, response.message);

                if (response.ds != null)
                {
                    foreach (var item in response.ds)
                    {
                        Debug.LogFormat("[API_GameResult_List]\n/ userRank : {0} / userCd : {1} / record : {2} /"
                        , item.userRank, item.userCd, item.record);
                    }
                }

                if (response.rs != null)
                {
                    Debug.LogFormat("[API_GameResult_Mine]\n/ laddrPoint : {0} / addLaddrPoint : {1} /"
                    , response.rs.laddrPoint, response.rs.addLaddrPoint);
                }
#endif
            }
        }
    }

    public static IEnumerator API_FishingEnd(string _gameId, int _gameTitleNum = 2300, string _discontYn = "N")
    {
        string url = api_url + "/api/v1/game/fishEnd";

        Request_FishingEnd request = new Request_FishingEnd();
        request.gameId = _gameId;
        request.gameTitleNum = _gameTitleNum;
        request.discontYn = _discontYn;

        UploadHandler uploadHandler = GetUploadHandlerRaw(JsonUtility.ToJson(request));

        using (UnityWebRequest www = new UnityWebRequest(url, method_post, new DownloadHandlerBuffer(), uploadHandler))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_FishingEnd() - Form upload complete!");
                //Debug.Log(www.downloadHandler.text);
                Response_FishingEnd response = JsonConvert.DeserializeObject<Response_FishingEnd>(www.downloadHandler.text);

#if UNITY_EDITOR
                Debug.LogFormat("[API_FishingEnd]\n/ status : {0} / message : {1} /"
                , response.status, response.message);
#endif
            }
        }
    }

    public static IEnumerator API_FishingCount(Action<Response_FishingCount> action)
    {
        string url = api_url + "/api/v1/game/fishingCount";

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                action.Invoke(null);
            }
            else
            {
                Debug.Log("API_FishingCount() - Form upload complete!");
                //Debug.Log(www.downloadHandler.text);
                Response_FishingCount response = JsonConvert.DeserializeObject<Response_FishingCount>(www.downloadHandler.text);
                action.Invoke(response);
#if UNITY_EDITOR
                Debug.LogFormat("[API_FishingCount]\n/ status : {0} / message : {1} /"
                , response.status, response.message);

                if (response.ds != null)
                {
                    foreach (var item in response.ds)
                    {
                        Debug.LogFormat("[API_FishingCount_List]\n/ cmmCdNm : {0} / fishCount : {1} /"
                        , item.cmmCdNm, item.fishCount);
                    }
                }
#endif
            }
        }
    }

    public static IEnumerator API_Ranking(int gameTitleNum, string playType, Action<Response_Ranking> action)
    {
        string url = api_url + "/api/v1/game/ranking?gameTitleNum=" + gameTitleNum + "&gameType=" + playType;

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_Ranking() - Form upload complete!");
                //Debug.Log(www.downloadHandler.text);
                Response_Ranking response = JsonConvert.DeserializeObject<Response_Ranking>(www.downloadHandler.text);
                action.Invoke(response);
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

   /* public static IEnumerator API_Record()
    {
        string url = api_url + "/api/v1/game/record";

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_Record() - Form upload complete!");
                //Debug.Log(www.downloadHandler.text);
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
    }*/

    public static IEnumerator API_LeaderBoard(int gameTitleNum, Action<Response_LeaderBoard> action)
    {
        string url = api_url + "/api/v1/game/leaderBoard?gameTitleNum=" + gameTitleNum;

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_auth, api_token);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_LeaderBoard() - Form upload complete!");
                //Debug.Log(www.downloadHandler.text);
                Response_LeaderBoard response = JsonConvert.DeserializeObject<Response_LeaderBoard>(www.downloadHandler.text);

                action.Invoke(response);
#if UNITY_EDITOR
              
#endif
            }
        }
    }

    public static IEnumerator API_Notice(int index, Action<Response_Notice> action)
    {
        string url = api_url + "/api/v2/notice?gameTitleNum=5000" + "&lang=" + GetConvertLanguage() + "&index=" + index;
        //string url = api_url + "/api/v2/notice?" +"lang=" + lang + "&index=" + index;

        using (UnityWebRequest www = new UnityWebRequest(url, method_get, new DownloadHandlerBuffer(), null))
        {
            www.SetRequestHeader(requestHeader_type, requestHeader_value);
            www.SetRequestHeader(requestHeader_gameVersion, Application.version);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("API_Notice() - Form upload complete!");
                //Debug.Log(www.downloadHandler.text);
                Response_Notice response = JsonConvert.DeserializeObject<Response_Notice>(www.downloadHandler.text);

                action.Invoke(response);
#if UNITY_EDITOR
                Debug.LogFormat("[API_Notice]\n/ status : {0} / message : {1} /"
                , response.status, response.message);
                if (response.rs != null)
                {
                    Debug.LogFormat("[API_Notice_Info]\n/ id : {0} / index : {1} / createAt : {2} / updateAt : {3} / listSize : {4} / title : {5} / content : {6} /"
                    , response.rs.id, response.rs.index, response.rs.createAt, response.rs.updateAt, response.rs.listSize, response.rs.title, response.rs.content);
                }
#endif
            }
        }
    }

    public static UploadHandlerRaw GetUploadHandlerRaw(string jsonString)
    {
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonString);
        return new UploadHandlerRaw(jsonToSend);
    }

    public static int GetGameTitleNum()
    {
        switch (GameDataManager.instance.gameType)
        {
            case GameDataManager.GameType.Bowling:
                {
                    return 1100;
                }
            case GameDataManager.GameType.Archery:
                {
                    return 1200;
                };
            case GameDataManager.GameType.Basketball:
                {
                    return 1300;
                };
            case GameDataManager.GameType.Badminton:
                {
                    return 1400;
                };
            case GameDataManager.GameType.Billiards:
                {
                    return 1500;
                };
            case GameDataManager.GameType.Darts:
                {
                    return 1600;
                };
            case GameDataManager.GameType.TableTennis:
                {
                    return 1700;
                };
            case GameDataManager.GameType.Boxing:
                {
                    return 1800;
                };
            case GameDataManager.GameType.Golf:
                {
                    return 1900;
                };
            case GameDataManager.GameType.Baseball:
                {
                    return 2000;
                };
            case GameDataManager.GameType.Tennis:
                {
                    return 2100;
                };
            default:
                {
                    return 1000;
                };
        }
    }

    public static string GetConvertLanguage()
    {
        if (GameSettingCtrl.settingInfo == null)
        {
            return "EN";
        }

        switch (GameSettingCtrl.settingInfo.language)
        {
            case "english":
                {
                    return "EN";
                }
            case "koreana":
                {
                    return "KR";
                }
            case "schinese":
                {
                    return "CNS";
                }
            case "tchinese":
                {
                    return "CNT";
                }
            case "japanese":
                {
                    return "JP";
                }
            default:
                {
#if SERVER_CHINA
                    return "CNS";
#else
                    return "EN";
#endif
                }
        }
    }
}
