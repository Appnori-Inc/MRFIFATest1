using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Appnori.Util;
using UnityEngine.SceneManagement;
using MJ;

public partial class GameDataManager : MonoBehaviour
{
    [System.Serializable]
    public struct roomData
    {
        public string gameType;
        public int Lv;
        public string roomId;

    }

    [SerializeField] private AppnoriWebRequest.Response_Items _items;

    [SerializeField] private List<Text> LobbyShowMyData = new List<Text>();
    [SerializeField] private List<Appnori.XR.XRControllerState> XrRigs = new List<Appnori.XR.XRControllerState>();

    /*public Notifier<int> myDia;
    public Notifier<int> myGold;*/

    public roomData localRoom { private set; get; }
    public bool isGetUserInfo { private set; get; } = false;

    private void initSettingMyData()
    {

        LobbyShowMyData.Add(GameObject.Find("Canvas_My/MyInfo/Image_Board/Gold").GetComponentInChildren<Text>());
        LobbyShowMyData.Add(GameObject.Find("Canvas_My/MyInfo/Image_Board/Dia").GetComponentInChildren<Text>());
        LobbyShowMyData.Add(GameObject.Find("Canvas_My/MyInfo/Image_Board/Text_NickName").GetComponent<Text>());
        /*MyData.Add(GameObject.Find("Canvas_My/MyInfo/Image_Board/Gold").GetComponentInChildren<Text>());
        MyData.Add(GameObject.Find("Canvas_My/MyInfo/Image_Board/Dia").GetComponentInChildren<Text>());
        MyData.Add(GameObject.Find("Canvas_My/MyInfo/Image_Board/Text_NickName").GetComponent<Text>());
        myDia.Value = -1;
        myGold.Value = -1;
        MyData[2].text = "";*/
        /* myDia.OnDataChanged += SetDia;
         myGold.OnDataChanged += SetGold;*/

    }
    private void SetName(string i)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            LobbyShowMyData[2].text = userInfo_mine.nowdia.Value.ToString("0");
        }

    }
    private void SetDia(int i)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            LobbyShowMyData[1].text = userInfo_mine.nowdia.Value.ToString("0");
            LobbyShowMyData[2].text = userInfo_mine.nick;
        }

    }
    private void SetGold(int i)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            LobbyShowMyData[0].text = userInfo_mine.nowGold.Value.ToString("0");
            LobbyShowMyData[2].text = userInfo_mine.nick;
        }
    }
   

    public void ChangeMyResource()
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            /*LobbyShowMyData[2].text = userInfo_mine.nick;
            LobbyShowMyData[1].text = userInfo_mine.nowdia.ToString();
            LobbyShowMyData[0].text = userInfo_mine.nowGold.ToString();*/
        }
    }
    public void ChangeMyResource(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            if (LobbyShowMyData.Count <= 0)
            {
                LobbyShowMyData.Add(GameObject.Find("Canvas_My/MyInfo/Image_Board/Gold").GetComponentInChildren<Text>());
                LobbyShowMyData.Add(GameObject.Find("Canvas_My/MyInfo/Image_Board/Dia").GetComponentInChildren<Text>());
                LobbyShowMyData.Add(GameObject.Find("Canvas_My/MyInfo/Image_Board/Text_NickName").GetComponent<Text>());

            }
            LobbyShowMyData[2].text = userInfo_mine.nick;
            LobbyShowMyData[1].text = userInfo_mine.nowdia.Value.ToString();
            LobbyShowMyData[0].text = userInfo_mine.nowGold.Value.ToString();
        }
        else
        {
            LobbyShowMyData.Clear();
        }
    }
    public void SetItems(AppnoriWebRequest.Response_Items newitems)
    {
        _items = newitems;
        BackGroundManager.instance.SetThemaData();
    }
    public void SearchItemList(string id, out string newid, out int goldPrice, out int diaPrice, out string orderYn)
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
        newid = temp;
        if (_items.ds.Exists(x => x.itemId == temp))
        {
            newid = temp;
            goldPrice = _items.ds.Find(x => x.itemId == temp).gameCoin;
            diaPrice = _items.ds.Find(x => x.itemId == temp).gameDia;
            orderYn = _items.ds.Find(x => x.itemId == temp).orderYn;
        }


    }

    public void SearchThema(string id, out int dia, out bool orderYn)
    {
        dia = 0;
        orderYn = false;
        if (_items.ds.Exists(x => x.itemId == id))
        {
            dia = _items.ds.Find(x => x.itemId == id).gameDia;
            orderYn = _items.ds.Find(x => x.itemId == id).orderYn == "Y" ? true : false;
        }
    }

    public bool CheckICanBuyItem(string itemId, int gold, int dia)
    {
        Debug.LogError($"CheckICanBuyItem id{itemId} gold {gold}dia {dia}");
        if (gold > 0)
        {
            if (userInfo_mine.nowGold.Value > gold)
            {
                StartCoroutine(AppnoriWebRequest.API_BuyItem(itemId));
            }
            else
                return false;
        }
        else
        {
            if (userInfo_mine.nowdia.Value > dia)
            {
                StartCoroutine(AppnoriWebRequest.API_BuyItem(itemId));
            }
            else
                return false;
        }



        Debug.LogError($"CheckICanBuyItem true");

        return true;
    }



    public bool CheckICanBuyGold(int consumeDia, int gold)
    {
        //AppnoriWebRequest.API_BuyGold();
        if (userInfo_mine.nowdia.Value < consumeDia)
            return false;
        StartCoroutine(AppnoriWebRequest.API_BuyGold(userInfo_mine.id, $"Buygold{gold}", consumeDia, gold));

        return true;
    }
    public bool CheckICanBuyMap(int consumeDia, int index)
    {
        //AppnoriWebRequest.API_BuyGold();
        if (userInfo_mine.nowdia.Value < consumeDia)
            return false;
        StartCoroutine(AppnoriWebRequest.API_BuyItem(BackGroundManager.instance.GetMapList()[index].themaId));
        BackGroundManager.instance.SetBuyMap(index);
        GameDataManager.instance.ChangeMyResource();


        return true;
    }


    public void RefillMoney()
    {
        StartCoroutine(AppnoriWebRequest.API_CheckRefill());
    }


    public void AddUsers(string ids, string nicks, int coins, int laddrs, CustomModelData data)
    {
        UserInfo temp = new UserInfo();
        temp.id = ids;
        temp.nick = nicks;
        temp.nowGold = new Notifier<int>(); ;
        temp.nowGold.Value = coins;
        temp.totalLaddr2 = laddrs;
        temp.customModelData = data;
        userInfos.Add(temp);

    }


    public void startGame(string gameType, string roomid)
    {
        roomData temp = new roomData();
        temp.gameType = gameType;
        if (gameType == "S")
            temp.Lv = LobbyManager.instance.Level;
        temp.roomId = roomid;
        localRoom = temp;
        StartCoroutine(AppnoriWebRequest.API_GameStartNew(gameType, roomid));


    }

    public void EndGame(string winner, string Loser, int addGold)
    {
        StartCoroutine(AppnoriWebRequest.API_GameResult(winner, Loser, addGold));
    }

    public void SetCharData(string Id)
    {
        userInfo_mine.thema = Id;
        StartCoroutine(AppnoriWebRequest.API_SetCustomCharData(userInfo_mine.customModelDatas));
    }

    /* private void SetMyModel()
     {
         CustomizeManager.GetInstance.characterCtrl.SetItem(userInfo_mine.customModelData.);
     }*/


}
