using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BallPool;
using System;
using System.Linq;
using Billiards;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
/// <summary>
/// Result. shot Player Info
/// </summary>
public class PlayAgainMenu : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] RectTransform root;

    [SerializeField] private Text winnerName;
    [SerializeField] private RawImage winnerImage;
    [SerializeField] private GameObject ButtonRoot;
    [SerializeField] private InteractableButton playAgainButton;
    [SerializeField] private Text playAgainText;
    [SerializeField] private GameUIController controller;

    [SerializeField] private CameraLook cameraLook;

    [SerializeField] private HandLineControl[] lineControl;

    //private RenderTexture MainPlayerTexture
    //{
    //    get
    //    {
    //        if (BallPoolGameLogic.playMode == BallPool.PlayMode.PlayerAI)
    //        {
    //            return GameDataManager.instance.GetRenderTexture(0);
    //        }

    //        return GameDataManager.instance.GetRenderTexture(GameDataManager.instance.userInfos.Select((info) => info.nick).ToList().IndexOf(BallPoolPlayer.mainPlayer.name));
    //    }
    //}

    //private RenderTexture OtherPlayerTexture
    //{
    //    get
    //    {
    //        if (BallPoolGameLogic.playMode == BallPool.PlayMode.PlayerAI)
    //        {
    //            return GameDataManager.instance.GetRenderTexture(1);
    //        }

    //        return GameDataManager.instance.GetRenderTexture(GameDataManager.instance.userInfos.Select((info) => info.nick).ToList().IndexOf(BallPoolPlayer.players[1].name));
    //    }
    //}

    public event Action<bool> onActive;

    private bool isWin;

    public bool wasOpened { get; private set; }

    public void OnClickRematch()
    {
        DisablePlayAgainButton();
        foreach(BallPoolPlayer player in BallPoolPlayer.players)
            player.CaromScore = 0;
        controller.Rematch(this.isWin);
    }

    public void DisablePlayAgainButton()
    {
        playAgainButton.interactable = false;
        playAgainText.text = "Waiting";
    }

    public void HidePlayAgainButton()
    {
        playAgainButton.gameObject.SetActive(false);
    }

    public void Hide()
    {
        menu.SetActive(false);

        HandLineManager.Instance.MainHand.RequestShow(false, this);
        HandLineManager.Instance.SubHand.RequestShow(false, this);
        onActive?.Invoke(false);

        wasOpened = false;
    }

    public void Show(BallPoolPlayer player)
    {
        onActive?.Invoke(true);

        winnerName.text = player.name;

        //if (player == BallPoolPlayer.mainPlayer)
        //{
        //    winnerImage.texture = MainPlayerTexture;
        //}
        //else
        //{
        //    winnerImage.texture = OtherPlayerTexture;
        //}

        //XRController.posVec = Vector3.zero;
        /* if (GameDataManager.instance.IsPico())
             XRController.rotQ = Quaternion.Euler(-45f, 0, 0);
         else
             XRController.rotQ = Quaternion.Euler(45f, 0, 0);*/
       
        menu.SetActive(true);
        foreach (HandLineControl hand in lineControl)
            hand.SetLine();

        HandLineManager.Instance.MainHand.RequestShow(true, this);
        HandLineManager.Instance.SubHand.RequestShow(true, this);


        

      

       wasOpened = true;

        StartCoroutine(MenuOpenAnimation(1f));

        RebuildLayout();
    }

    public void SetNextStage(bool isWin)
    {
        if (BallPoolGameLogic.playMode != BallPool.PlayMode.PlayerAI)
            return;

        this.isWin = isWin;

        if (isWin)
        {
            playAgainText.text = "Next Level";
        }
        else
        {
            playAgainText.text = "Rematch";
        }
    }

    //OpenAnimation()
    private IEnumerator MenuOpenAnimation(float runTime)
    {
        cameraLook.enabled = false;
        ButtonRoot.SetActive(false);

        RebuildLayout();

        float t = 0;
        while (t < runTime)
        {
            transform.Rotate(Vector3.up, (runTime - t) * 10f);

            t += Time.deltaTime;
            yield return null;
        }

        cameraLook.enabled = true;
        ButtonRoot.SetActive(true);

        RebuildLayout();
        yield break;
    }

    private void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }

    public void ShowMainPlayer()
    {
        HidePlayAgainButton();
        Show(BallPoolPlayer.mainPlayer);
        SoundManager.PlaySound(SoundManager.AudioClipType.Win);
    }
}
