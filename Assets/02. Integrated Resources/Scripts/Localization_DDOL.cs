using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using SingletonBase;
using UnityEngine.SceneManagement;

public class Localization_DDOL : Singleton<Localization_DDOL>
{
    public enum GameLanguageState
    {
        systemLanguage, korean, chinese, taiwan, english
    }
    public GameLanguageState gameLanguage;

    protected override void Awake()
    {
        base.Awake();
        if (gameLanguage == GameLanguageState.systemLanguage)
            gameLanguage = (GameLanguageState)GetSystemLanguage();


        DontDestroyOnLoad(this.gameObject);
    }

    [System.Runtime.InteropServices.DllImport("KERNEL32.DLL")]
    private static extern int GetSystemDefaultLCID();
    CultureInfo GetSystemCulture()
    {
        return new CultureInfo(GetSystemDefaultLCID());
    }

    int GetSystemLanguage()
    {
        switch (GetSystemCulture().Name)
        {
            case "ko":
                return 1;
            case "ko-KR":
                return 1;
            case "zh-CN":
                return 2;
            case "zh-TW":
                return 3;
            default:
                return 4;
        }
    }
}
