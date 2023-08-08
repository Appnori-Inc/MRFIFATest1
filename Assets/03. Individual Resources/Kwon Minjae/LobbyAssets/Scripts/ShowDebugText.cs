using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDebugText : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
       /* if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);*/
        Application.logMessageReceived += Application_logMessageReceived;
    }
    public TextMesh UnityErrorText;
    private const int maxIndex = 5;
    private int index = 0;
    private string[] strText = new string[maxIndex];
    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Error)
            return;

        strText[index] = condition;
        index = index + 1 == maxIndex ? index = 0 : index + 1;
        string str = "";
        int useIndex = index;
        for (int i = 0; i < maxIndex; i++)
        {
            str += strText[useIndex] + "\n";
            useIndex = useIndex + 1 == maxIndex ? useIndex = 0 : useIndex + 1;
        }
        UnityErrorText.text = str;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= Application_logMessageReceived;
    }
}
