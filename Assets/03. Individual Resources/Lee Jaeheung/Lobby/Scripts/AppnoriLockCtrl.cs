#if APPNORI_LOCK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI;
using System.Windows.Forms;
public class AppnoriLockCtrl : MonoBehaviour
{

    public class AccessCode_DB
    {
        public int result;

        public string title;
        public string info;
        public string endDate;
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (code == null || code.Length < 2)
        //{
        //    System.Windows.Forms.MessageBox.Show("Null A Code.", "Start Error");
        //    Application.Quit();
        //    return;
        //}

        if (!File.Exists(UnityEngine.Application.dataPath + "/UnlockCode.txt"))
        {
            System.Windows.Forms.MessageBox.Show("Null A Code.", "Start Error");
            UnityEngine.Application.Quit();
            return;
        }
        string code = File.ReadAllText(UnityEngine.Application.dataPath + "/UnlockCode.txt");

        StartCoroutine(InitCoroutine(code));
    }

    IEnumerator InitCoroutine(string code)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("Code", code));

        UnityWebRequest www = UnityWebRequest.Post(UserInfoManager.addr + "Access_AppnoriLock.php", formData);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            System.Windows.Forms.MessageBox.Show("Access failed.", "Start Error");
            UnityEngine.Application.Quit();
        }
        else
        {
            //Debug.Log("Form upload complete!");
            Debug.Log(www.downloadHandler.text);
            AccessCode_DB data = JsonUtility.FromJson<AccessCode_DB>(www.downloadHandler.text);
            if (data.result == 1)
            {
                GameDataManager.instance.isUnlock = true;

                Text text = transform.GetComponentInChildren<Text>();
                text.text = data.title + "\n";
                text.text += "Trial period : " + data.endDate + "\n";
                text.text += data.info;

                Debug.Log("Form upload complete!");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("The demo version has expired.", "Info");
                UnityEngine.Application.Quit();
            }
        }
        www.Dispose();
    }
}
#endif