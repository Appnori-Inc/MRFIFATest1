using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Localization_CTRL : MonoBehaviour
{
    public string ID;
    private Text text;

    //public string str_Korean;
    //public string str_Chinese;
    //public string str_English;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Start()
    {
        if (ID == "")
            return;
        //if (Localization_DDOL.GetInstance == null)
        //    return;

        //switch (Localization_DDOL.GetInstance.gameLanguage)
        //{
        //    case Localization_DDOL.GameLanguageState.korean:
        //        text.text = str_Korean;
        //        break;

        //    case Localization_DDOL.GameLanguageState.chinese:
        //        text.text = str_Chinese;
        //        break;

        //    case Localization_DDOL.GameLanguageState.english:
        //    default:
        //        text.text = str_English;
        //        break;
        //}
        StaticLocalizationCtrl.GetInstance.AddData(ID, text);
    }
}
