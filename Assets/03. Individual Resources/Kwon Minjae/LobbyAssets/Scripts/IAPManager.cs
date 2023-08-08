#if PICO_PLATFORM
using System;
using System.Collections;
using Pico.Platform;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class IAPManager : MonoBehaviour
{
    private ProductList ProductList;
    private PurchaseList PurchaseList;

    public static IAPManager instance;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);

    }
    void Start()
    {
        try
        {
            CoreService.Initialize();
            Log($"Init Successfully");
        }
        catch (Exception e)
        {
            Log($"Initialize failed {e}");
            return;
        }

        getProductList();
        getPurchaseList();
    }


    void getProductList()
    {
        Log($"GetProductsBySKU");
        IAPService.GetProductsBySKU(null).OnComplete(msg =>
        {
            if (msg.IsError)
            {
                Log($"Get product list error:code={msg.Error.Code},message={msg.Error.Message}");
                return;
            }

            this.ProductList = msg.Data;
        });
    }

    void getPurchaseList()
    {
        Log("GetPurchaseList");
        IAPService.GetViewerPurchases().OnComplete(msg =>
        {
            if (msg.IsError)
            {
                Log($"Get Purchase List error:code={msg.Error.Code},message={msg.Error.Message}");
                return;
            }

            this.PurchaseList = msg.Data;
        });
    }


    public void Log(string newLine)
    {
        Debug.Log(newLine);
    }

    void LaunchCheckoutFlow(Product product)
    {
        Task<Purchase> task;
        task = IAPService.LaunchCheckoutFlow(product.SKU, product.Price, product.Currency);

        task.OnComplete(m =>
        {
            if (m.IsError)
            {
                Debug.Log($"LaunchCheckoutFlow error:Code={m.Error.Code},Message={m.Error.Message}");
                return;
            }
            if (m.Data == null || string.IsNullOrWhiteSpace(m.Data.ID))
            {
                Debug.Log($"LaunchCheckoutFlow failed:Data is empty");
                return;
            }
            Debug.Log($"LaunchCheckoutFlow successfully:{product.SKU}");
        });
    }

    void ConsumePurchase(int num)
    {
        Debug.Log("구매");
        var purchase = PurchaseList[0];
        AppnoriWebRequest.API_BuyDia(purchase.ID, num);
        IAPService.ConsumePurchase(purchase.SKU).OnComplete(m =>
        {
            if (m.IsError)
            {
                Log($"ConsumePurchase failed:code={m.Error.Code} message={m.Error.Message}");
                return;
            }

            Log($"Consume Purchase Successfully.");
        });
    }
    public void Click_IAPButton(int index)
    {
        StartCoroutine(IAPPurchase(index));
    }
    IEnumerator IAPPurchase(int index)
    {
        var product = ProductList[index];
        LaunchCheckoutFlow(product);
        getPurchaseList();
        yield return new WaitForSeconds(1f);
        while (PurchaseList.Count == 0)
        {
            Debug.LogError(PurchaseList.Count);
            getPurchaseList();
            yield return new WaitForSeconds(1f);
        }

        ConsumePurchase(index);

    }
}
#endif