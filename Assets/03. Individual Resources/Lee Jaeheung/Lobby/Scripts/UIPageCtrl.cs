using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPageCtrl : MonoBehaviour
{
    private bool isInit = false;
    private Text text_page;
    public ItemUIPart item_part;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        if (isInit)
        {
            return;
        }
        isInit = true;
        text_page = transform.Find("Canvas/Text").GetComponent<Text>();

        CustomizeManager.GetInstance.InitPageUI(this);
        //CustomizeManager.GetInstance.InitButtonSlot(transform.Find("Button_Next").GetComponent<MeshButtonCtrl>());
        //CustomizeManager.GetInstance.InitButtonSlot(transform.Find("Button_Back").GetComponent<MeshButtonCtrl>());
    }

    public void SetPage(int current_page, int max_page)
    {
        Debug.LogError(item_part + " - " + max_page);
        Init();

        text_page.text = (current_page + 1) + " / " + (max_page + 1);

        gameObject.SetActive(max_page != 0);
    }

    public void ClickNextPage()
    {
        CustomizeManager.GetInstance.SetNextPage((int)item_part);
    }

    public void ClickBackPage()
    {
        CustomizeManager.GetInstance.SetBackPage((int)item_part);
    }
}
