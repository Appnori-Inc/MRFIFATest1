using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LeaderBoardCtrl : MonoBehaviour
{
    public class SlotInfo
    {
        public Text text_name;
        public Text text_score;
        public RawImage image_profile;
        public Image image_grade;
    }

    private SlotInfo[] slotInfos;

    private Text text_myRank;

    private Animator anim;

    private bool isInit = false;

    private static string imageCachePath;
    private static SHA1CryptoServiceProvider s_SHA1 = new SHA1CryptoServiceProvider();
    public Sprite[] sprites_grade;
    [System.Serializable]
    public class LeaderBoardInfo
    {
        public UserInfo[] users;

        [System.Serializable]
        public class UserInfo
        {
            public string Rank;
            public string Nick;
            public string Score;
            public string Image;
        }
    }
    private void Awake()
    {
        Init();
    }
    private void OnEnable()
    {
        Debug.Log("리더보드 리프레시");
        MyScoreUpdate();
    }
  /*  private void Start()
    {
        Debug.Log("리더보드 리프레시");
        MyScoreUpdate();

    }*/
    void Init()
    {
        if (isInit)
        {
            return;
        }

        anim = GetComponent<Animator>();

        slotInfos = new SlotInfo[11];

        Transform tempTr = transform.Find("Board/Image_Bar_Mine");
        text_myRank = tempTr.Find("Text_Rank").GetComponent<Text>();

        slotInfos[0] = new SlotInfo();
        slotInfos[0].text_name = tempTr.Find("Text_Name").GetComponent<Text>();
        slotInfos[0].text_score = tempTr.Find("Text_Score").GetComponent<Text>();
        slotInfos[0].image_profile = tempTr.Find("Image_Profile").GetComponent<RawImage>();
        slotInfos[0].image_grade = tempTr.Find("Image_Grade").GetComponent<Image>();

        tempTr = transform.Find("Board/List");

        for (int i = 0; i < 10; i++)
        {
            slotInfos[i + 1] = new SlotInfo();
            slotInfos[i + 1].text_name = tempTr.GetChild(i).Find("Text_Name").GetComponent<Text>();
            slotInfos[i + 1].text_score = tempTr.GetChild(i).Find("Text_Score").GetComponent<Text>();
            slotInfos[i + 1].image_profile = tempTr.GetChild(i).Find("Image_Profile").GetComponent<RawImage>();
            slotInfos[i + 1].image_grade = tempTr.GetChild(i).Find("Image_Grade").GetComponent<Image>();
        }

        //gameObject.SetActive(false);

        isInit = true;
    }

    void InitSlot()
    {
        for (int i = 0; i < slotInfos.Length; i++)
        {
            slotInfos[i].text_name.text = "-";
            slotInfos[i].text_score.text = "-";
            slotInfos[i].image_profile.texture = null;
            slotInfos[i].image_grade.enabled = false;
        }
        text_myRank.text = "-";
    }

    public void MyScoreUpdate()
    {
        Init();

        InitSlot();

        //gameObject.SetActive(true);
      

        if (coroutine_MyScoreUpdate != null)
        {
            StopCoroutine(coroutine_MyScoreUpdate);
        }

        coroutine_MyScoreUpdate = StartCoroutine(Coroutine_MyScoreUpdate());
    }

    Coroutine coroutine_MyScoreUpdate;

    IEnumerator Coroutine_MyScoreUpdate()
    {
        while (!GameDataManager.instance.isGetUserInfo)
        {
            yield return null;
        }

        WWWForm form = new WWWForm();
        StartCoroutine(AppnoriWebRequest.API_Ranking(SetTextUI));
    }



    void SetTextUI(AppnoriWebRequest.Response_Ranking leaderBoardInfo)
    {
        for (int i = 0; i < leaderBoardInfo.ds.Count; i++)
        {
            slotInfos[i + 1].text_score.text = leaderBoardInfo.ds[i].totLaddrPoint.ToString();
            slotInfos[i + 1].text_name.text = leaderBoardInfo.ds[i].nickNm;
            SetImage(leaderBoardInfo.ds[i].avatarUrl, slotInfos[i + 1].image_profile);
            SetGradeImage(leaderBoardInfo.ds[i].userGrade, slotInfos[i + 1].image_grade);
        }

        if (leaderBoardInfo.rs != null)
        {
            text_myRank.text = leaderBoardInfo.rs.userRank.ToString();
            slotInfos[0].text_score.text = leaderBoardInfo.rs.totLaddrPoint.ToString();
        }
        else
        {
            text_myRank.text = "-";
            slotInfos[0].text_score.text = "0";
        }

        slotInfos[0].text_name.text = GameDataManager.instance.userInfo_mine.nick;
        SetImage(GameDataManager.image_url_mine, slotInfos[0].image_profile);
        anim.SetTrigger("OnStart");
    }


    public void Close()
    {
        Init();
        //gameObject.SetActive(false);
    }

    public void SetImage(string url, RawImage set_ui_image)
    {
        set_ui_image.texture = null;
        if (url == null || url.Length < 10)
        {
            return;
        }
        StartCoroutine(SetImageCoroutine(url, set_ui_image));
    }

    public void SetGradeImage(int gradeNum, Image ui_image)
    {
        try
        {
            ui_image.sprite = sprites_grade[gradeNum];
            ui_image.enabled = true;
        }
        catch (Exception)
        {
            ui_image.sprite = sprites_grade[0];
        }
    }
    IEnumerator SetImageCoroutine(string url, RawImage set_ui_image)
    {
        string check_url = CheckCacheData(url);
        Texture2D texture = new Texture2D(2, 2);

        if (!check_url.Contains("http://") && !check_url.Contains("https://"))
        {
            //Debug.LogError(check_url);
            texture.LoadImage(File.ReadAllBytes(check_url));
            set_ui_image.texture = texture;
            yield break;
        }

        UnityWebRequest www = UnityWebRequest.Get(check_url);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            //Debug.Log(www.downloadHandler.text);

            byte[] results = www.downloadHandler.data;
            if (check_url.Contains(url))
            {
                File.WriteAllBytes(GetCachePath(url), results);
                //textttttt.text = "처음";
            }
            else
            {
                //textttttt.text = "로컬";
            }

            texture.LoadImage(results);
            set_ui_image.texture = texture;
        }
    }

    public static string CheckCacheData(string url)
    {
        string cachePath = GetCachePath(url);
        FileInfo fileInfo = new FileInfo(cachePath);
        if (fileInfo.Exists)
        {
            return cachePath;
        }
        else
        {
            return url;
        }
    }

    public static string GetCachePath(string url)
    {
        if (imageCachePath == null)
            imageCachePath = Application.temporaryCachePath + "/Cache/";

        if (!Directory.Exists(imageCachePath))
            Directory.CreateDirectory(imageCachePath);

        return imageCachePath + System.Convert.ToBase64String(s_SHA1.ComputeHash(UTF8Encoding.Default.GetBytes(url))).Replace('/', '_');
    }
}
