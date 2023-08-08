using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionCtrl : MonoBehaviour
{
    void Start()
    {
        GameSettingCtrl.AddLocalizationChangedEvent(SetText);

    }

    public void SetText(LanguageState languageState)
    {
        transform.GetComponent<TextMesh>().text = GameSettingCtrl.GetLocalizationText("0158") + "\n" + Application.version;
    }
}
