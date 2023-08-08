using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Appnori.Util
{
    public class NoticeCtrl : MonoBehaviour
    {

        private AppnoriWebRequest.Response_Notice noticeInfo;

        private Text text_info;
        private ContentSizeFitter sizeFitter;
        private RectTransform contentTr; 

        private Text text_page;
        private CanvasGroup canvasGroup;
        private GameObject[] gos_button;

        private GameObject go_connect;

        // Start is called before the first frame update
        void Start()
        {
            Transform tempTr = transform.Find("Board/Scroll View/Viewport/Content/Text");
            text_info = tempTr.GetComponent<Text>();
            sizeFitter = tempTr.GetComponent<ContentSizeFitter>();
            contentTr = tempTr.parent.GetComponent<RectTransform>();

            tempTr = transform.Find("Board/Page/Text");
            text_page = tempTr.GetComponent<Text>();
            canvasGroup = tempTr.parent.GetComponent<CanvasGroup>();

            tempTr = transform.Find("Board");
            go_connect = tempTr.Find("Image_Disconnect").gameObject;

            gos_button = new GameObject[2];

            Button button = canvasGroup.transform.Find("Button_Back").GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { Click_InputKey("Back"); });
            gos_button[0] = button.gameObject;

            button = canvasGroup.transform.Find("Button_Next").GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { Click_InputKey("Next"); });
            gos_button[1] = button.gameObject;

            for (int i = 0; i < 2; i++)
            {
                gos_button[i].SetActive(false);
            }

            text_info.text = "";
            text_page.text = "";

            Invoke("DelayInit", 1f);
            //Click_InputKey("");
        }

        public void DelayInit()
        {
            Click_InputKey("");
            GameSettingCtrl.AddLocalizationChangedEvent(() => Click_InputKey(""));
        }

        public void SetText(AppnoriWebRequest.Response_Notice response)
        {
            canvasGroup.interactable = true;
            if (response.status != 200)
            {
                go_connect.SetActive(false);
                return;
            }

            noticeInfo = response;

            text_info.text = "<size=50><b>" + noticeInfo.rs.title + "</b></size>\n\n";
            text_info.text += noticeInfo.rs.content;

            text_page.text = (noticeInfo.rs.index + 1) + " / " + noticeInfo.rs.listSize;

            for (int i = 0; i < 2; i++)
            {
                gos_button[i].SetActive(noticeInfo.rs.listSize >= 2);
            }

            sizeFitter.SetLayoutVertical();
            contentTr.sizeDelta = new Vector2(0f, text_info.rectTransform.sizeDelta.y + 60);
            go_connect.SetActive(false);
        }

        public void Click_InputKey(string key)
        {
            canvasGroup.interactable = false;
            go_connect.SetActive(true);
            if (noticeInfo == null)
            {
                StartCoroutine(AppnoriWebRequest.API_Notice(0, SetText));
                return;
            }

            try
            {
                int select_index = 0;
                switch (key)
                {
                    case "Next":
                        {
                            select_index = noticeInfo.rs.index + 1;
                        }
                        break;
                    case "Back":
                        {
                            select_index = noticeInfo.rs.index - 1;
                        }
                        break;
                }

                if (select_index >= noticeInfo.rs.listSize)
                {
                    select_index = 0;
                }
                else if (select_index < 0)
                {
                    select_index = noticeInfo.rs.listSize - 1;
                }

                StartCoroutine(AppnoriWebRequest.API_Notice(select_index, SetText));
            }
            catch (System.Exception)
            {
                StartCoroutine(AppnoriWebRequest.API_Notice(0, SetText));
            }
        }

    }
}
