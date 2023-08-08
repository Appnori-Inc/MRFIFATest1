using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonBase;
using UnityEngine.UI;

public partial class CustomizeManager : Singleton<CustomizeManager>
{
    [SerializeField] private GameObject Purchase_Enable;
    [SerializeField] private GameObject Purchase_Disable;
    [SerializeField] private Image image_Price;
    [SerializeField] private Text text_Price;
    [SerializeField] private Sprite sprite_Gold;
    [SerializeField] private Sprite sprite_Dia;

    private CustomizeItemCtrl nowCtrl = null;
    private List<Button> List_purchaseButton = new List<Button>();
    private void InitPurchase()
    {
        //List_purchaseButton
        List_purchaseButton.Add(Purchase_Enable.transform.Find("Button_Cancle").GetComponentInChildren<Button>());
        List_purchaseButton.Add(Purchase_Enable.transform.Find("Button_Purchase").GetComponentInChildren<Button>());
        List_purchaseButton.Add(Purchase_Disable.GetComponentInChildren<Button>());
        List_purchaseButton[0].onClick.AddListener(() => OnCloseWindow());
        List_purchaseButton[1].onClick.AddListener(() => OnConfirePurchase());
        List_purchaseButton[2].onClick.AddListener(() => OnCloseWindow());

    }

    public void SetPurchase(bool isEnable, bool isDia, int price, CustomizeItemCtrl now)
    {
        nowCtrl = now;
        if (isEnable)
        {
            Purchase_Enable.SetActive(true);
            if (isDia)
            {
                text_Price.color = Color.red;
                image_Price.sprite = sprite_Dia;
            }
            else
            {
                text_Price.color = Color.yellow;
                image_Price.sprite = sprite_Gold;
            }
            text_Price.text = price.ToString();
        }
        else
        {
            Purchase_Disable.SetActive(true);
        }
    }

    public void OnConfirePurchase()
    {
        nowCtrl.SendPurchaseData();
        OnCloseWindow();
    }
    public void OnCloseWindow()
    {
        if (Purchase_Disable.activeSelf)
            Purchase_Disable.SetActive(false);
        if (Purchase_Enable.activeSelf)
            Purchase_Enable.SetActive(false);
    }
}
