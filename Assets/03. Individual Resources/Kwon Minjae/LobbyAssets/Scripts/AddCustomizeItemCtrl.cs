using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public partial class CustomizeItemCtrl : CustomizeProp
{
    [SerializeField] private MeshRenderer _lock;
    [SerializeField] private Canvas lockCanvas;
    [SerializeField] private GameObject _lockGO;
    [SerializeField] private Text _price;

    [SerializeField] private string itemid ="";
    [SerializeField] private int priceGold = 0;
    [SerializeField] private int priceDia = 0;
    [SerializeField] private string orderYn = "N";


    [SerializeField] private Button but;
    private void SetItemData(string id)
    {
        _lockGO = transform.Find("Lock").gameObject;
        but = _lockGO.transform.GetComponentInChildren<Button>();
        but.onClick.AddListener(() => OnClickBuyItem()); 
        GameDataManager.instance.SearchItemList(id,out itemid, out priceGold, out priceDia, out orderYn);
        if (priceGold > 0)
            _price.text = priceGold.ToString() + "gold";
        else
            _price.text = priceDia.ToString() + "dia";
        if(priceDia ==0 && priceGold ==0)
            orderYn = "Y";
        _lock.gameObject.SetActive(orderYn != "Y");
        lockCanvas = _lock.transform.GetComponentInChildren<Canvas>();
        lockCanvas.worldCamera = GameObject.Find("XRRig/Camera Offset/Main Camera").GetComponent<Camera>();

    }

    public void OnClickBuyItem()
    {
        if(priceDia >0 )
            CustomizeManager.GetInstance.SetPurchase(true, true, priceDia ,this);
        else
            CustomizeManager.GetInstance.SetPurchase(true, false, priceGold, this);
       
    }

    public void SendPurchaseData()
    {
        if (GameDataManager.instance.CheckICanBuyItem(itemid, priceGold, priceDia))
        {
            orderYn = "Y";
            _lock.gameObject.SetActive(orderYn != "Y");
        }
    }
   
}
