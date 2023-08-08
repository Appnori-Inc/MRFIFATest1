using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Appnori.Util;
using MJ;
using UnityEngine.SceneManagement;
using System;
using Billiards;


public class BackGroundManager : MonoBehaviour
{
    public Notifier<int> now_BackGround;
    [SerializeField]
    private Transform thema;
    [SerializeField]
    private List<Button> list_but = new List<Button>();

    [SerializeField]
    private string BackGround = " ";
    
    public List<MapData> mapList = new List<MapData>();
    public List<MapData> GetMapList() { return mapList; }

    [SerializeField] private List<Image> Uibuts = new List<Image>();
    [SerializeField] private List<Image> UiSqubuts = new List<Image>();

    public static BackGroundManager instance;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
       

    }

    void Start()
    {
        init();
        now_BackGround.Value = 0;
        now_BackGround.OnDataChanged += OnChanged;
    }

    private void init()
    {
        mapList.Add(Resources.Load<MapData>("GraphicMap_Hanok"));
        mapList.Add(Resources.Load<MapData>("GraphicMap_Factory"));
        mapList.Add(Resources.Load<MapData>("GraphicMap_WaterFall"));
        SceneManager.LoadScene(mapList[0].Name, LoadSceneMode.Additive);
        RenderSettings.ambientLight = mapList[0].skyColor;
        RenderSettings.skybox = mapList[0].skyMat;
        BackGround = mapList[0].Name;
        for(int i=1;i<=3 ;i++)
            list_but.Add(thema.Find($"ThemaT{i}").GetComponentInChildren<Button>());
        for(int i=0;i<3 ;i++)
        {
            int index = i;
            list_but[index].onClick.AddListener(() => SetMap(index));
        }
    }


    public void SetThemaData()
    {
        for(int i =0;i< mapList.Count ;i++)
        {
            GameDataManager.instance.SearchThema(mapList[i].themaId, out mapList[i].priceDia, out mapList[i].buyYn);
            if (mapList[i].themaId == "T5000")
                mapList[i].buyYn = true;
            LobbyManager.instance.SetThemaButton(i, mapList[i].buyYn);
        }

       
        switch (GameDataManager.instance.userInfo_mine.thema)
        {
            case "T5000":
                now_BackGround.Value = 0;
                break;
            case "T5001":
                now_BackGround.Value = 1;
                break;
            case "T5002":
                now_BackGround.Value = 2;
                break;
        }


    }
    private void OnChanged(int index)
    {
        if (!mapList[index].buyYn)
            return;

        SceneManager.UnloadSceneAsync(mapList.Find(x =>
            SceneManager.GetSceneByName(x.Name).isLoaded).Name);
        int curr = index;
        if (index > mapList.Count)
            curr = 0;

        BackGround = mapList[curr].Name;
        SceneManager.LoadScene(mapList[curr].Name, LoadSceneMode.Additive);
        RenderSettings.skybox = mapList[curr].skyMat;
        RenderSettings.ambientLight = mapList[curr].skyColor;
        if (RenderSettings.fog = mapList[curr].isFog)
            RenderSettings.fogColor = mapList[curr].fogColor;

        foreach (Image im in Uibuts)
        {
            if(mapList[curr].UIImage != null)
                im.sprite = mapList[curr].UIImage;
        }
        foreach (Image im in UiSqubuts)
        {
            if (mapList[curr].UISquImage != null)
                im.sprite = mapList[curr].UISquImage;
        }

        GameDataManager.instance.SetCharData(mapList[curr].themaId);
    }

    public  void SetBuyMap(int index)
    {
        mapList[index].buyYn = true;
    }
    public void SetMap(int index)
    {
        now_BackGround.Value = index;
    }
}
