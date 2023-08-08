using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeMidUICtrl : MonoBehaviour
{
    private bool isEndScene = false;
    private void Awake()
    {
        isEndScene = false;
    }


    public void Click_InputKey(string key)
    {
        if (isEndScene)
        {
            return;
        }

        LobbySoundManager.GetInstance.Play((int)Random.Range(0f, 2.9999f));

        switch (key)
        {
            case "Lobby":
                {
                    isEndScene = true;
                    CustomizeManager.GetInstance.Upload_UserData();
                    MeshFadeCtrl.instance.LoadScene("Lobby",1.5f);
                }
                break;
            case "Male":
                {
                    CustomizeManager.GetInstance.characterCtrl.SetGender(0);
                }
                break;
            case "Female":
                {
                    CustomizeManager.GetInstance.characterCtrl.SetGender(1);
                }
                break;
        }
    }
}
