using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AchieveSlotCtrl : MonoBehaviour
{
    private Text text_title;
    private Text text_info;
    private Text text_progress;
    private Image image_fill;
    private RawImage image_icon;
    private int index = -10;

    // Start is called before the first frame update
    void Start()
    {
        text_title = transform.Find("Text_Title").GetComponent<Text>();
        text_info = transform.Find("Text_Info").GetComponent<Text>();
        text_progress = transform.Find("Text_Progress").GetComponent<Text>();
        image_fill = transform.Find("Image_Fill").GetComponent<Image>();
        image_icon = transform.Find("Image_Icon").GetComponent<RawImage>();

        AchievementCtrl.instance.SetInitSlot(this);
    }

    public void SetSlot(int _index, bool isInit)
    {
        if (!isInit)
        {
            if (index == _index)
            {
                return;
            }
        }

        index = _index;
        if (index < 0 || index >= AchievementCtrl.instance.achieves.Length)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        GameDataManager.AchievementInfo achieve = AchievementCtrl.instance.achieves[index];

        if (achieve.is_archived)
        {
            text_title.text = "<color=#9DFF53>" + achieve.title + "</color>";
            text_info.text = "<color=#FFFFFF>" + achieve.unlocked_description_override + "</color>";
            image_fill.gameObject.SetActive(false);
            text_progress.gameObject.SetActive(false);
        }
        else
        {
            text_title.text = "<color=#969696>" + achieve.title + "</color>";
            text_info.text = "<color=#C8C8C8>" + achieve.description + "</color>";
            float fill = 0f;
            if (achieve.velue_target != 0)
            {
                fill = (float)achieve.velue_current / achieve.velue_target;
            }
            image_fill.fillAmount = Mathf.Clamp01(1f - fill);
            text_progress.text = (fill * 100f).ToString("F1") + "%";
            image_fill.gameObject.SetActive(true);
            text_progress.gameObject.SetActive(true);
        }

        GameDataManager.instance.SetImage(achieve.image_uri, image_icon);
        transform.localPosition = new Vector3(47.5f, _index * -23.5f - 11.5f, 0f);
    }
}
