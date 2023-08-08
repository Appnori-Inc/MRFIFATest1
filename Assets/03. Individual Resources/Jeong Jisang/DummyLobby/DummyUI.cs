using Billiards;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DummyUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform root;

    [SerializeField]
    private ToggleGroup gameTypeGroup;

    [SerializeField]
    private Toggle Pocket;
    [SerializeField]
    private Toggle Carom3;
    [SerializeField]
    private Toggle Carom4;

    [SerializeField]
    private Toggle Single;

    [SerializeField]
    private Slider Level;


    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);

    }


    public void OnClickStart()
    {
        var gameTypeToggle = gameTypeGroup.ActiveToggles().FirstOrDefault();

        var info = new BilliardsMatchInfo()
        {
            userId = "userID",
            userName = "_userName",
            otherId = "otherId",
            otherName = "_otherName",

            //playingType = playingType,
            //gameType = gameType,
            //level = Level,
        };

        info.gameType = gameTypeToggle switch
        {
            Toggle when gameTypeToggle == Pocket => GameType.PocketBilliards,
            Toggle when gameTypeToggle == Carom3 => GameType.CaromThree,
            Toggle when gameTypeToggle == Carom4 => GameType.CaromFour,
            _ => throw new System.Exception("Not matched")
        };
        info.playingType = Single.isOn ? PlayType.Single : PlayType.Multi;
        info.level = Mathf.RoundToInt(Level.value);

        if(info.playingType == PlayType.Single)
            LevelLoader.InitializeAndLoad(info);
        else
        {

        }
           // PhotonDevConnector.ConnectAndStart(info);
    }

}
