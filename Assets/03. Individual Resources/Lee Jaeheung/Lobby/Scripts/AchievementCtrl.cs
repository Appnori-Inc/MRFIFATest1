using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AchievementCtrl : MonoBehaviour
{
    public static AchievementCtrl instance;

    public GameDataManager.AchievementInfo[] achieves;
    public RectTransform rect_content;
    public AchieveSlotCtrl[] achieveSlots;

    private bool isInit = false;

    private GameObject[] active_gos;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        isInit = false;

        active_gos = new GameObject[3];

        active_gos[0] = transform.Find("Board/Image_Disconnect").gameObject;
        active_gos[1] = transform.Find("Board/Image_Bar_Mine").gameObject;
        active_gos[2] = transform.Find("Board/Scroll View").gameObject;

        active_gos[0].SetActive(true);
        active_gos[1].SetActive(false);
        active_gos[2].SetActive(false);
    }

    public void SetInitSlot(AchieveSlotCtrl achieveSlot)
    {
        if (achieveSlots == null || achieveSlots.Length == 0)
        {
            achieveSlots = new AchieveSlotCtrl[7];
        }
        Transform parentTr = achieveSlot.transform.parent;
        for (int i = 0; i < parentTr.childCount; i++)
        {
            if (parentTr.GetChild(i) == achieveSlot.transform)
            {
                achieveSlots[i] = achieveSlot;
                return;
            }
        }
    }

    public IEnumerator SetAchieve()
    {
        while (achieveSlots == null || achieveSlots.Length == 0)
        {
            yield return null;
        }

        for (int i = 0; i < 7; i++)
        {
            while (achieveSlots[i] == null)
            {
                yield return null;
            }
        }

        while (GameDataManager.instance.dict_achieve == null || GameDataManager.instance.dict_achieve.Count == 0)
        {
            yield return null;
        }

        active_gos[0].SetActive(false);
        active_gos[1].SetActive(true);
        active_gos[2].SetActive(true);

        achieves = GameDataManager.instance.dict_achieve.Values.ToArray();

        int unlockCount = 0;
        for (int i = 0; i < achieves.Length; i++)
        {
            if (achieves[i].is_archived)
            {
                unlockCount++;
            }
        }

        rect_content.sizeDelta = new Vector2(0f, 23.5f * achieves.Length);
        rect_content.anchoredPosition = new Vector2(0f, 0f);
        for (int i = 0; i < achieveSlots.Length; i++)
        {
            achieveSlots[i].SetSlot(i, true);
        }

        transform.Find("Board/Image_Bar_Mine/Text_Progress01").GetComponent<Text>().text = unlockCount + " / " + achieves.Length;
        transform.Find("Board/Image_Bar_Mine/Text_Progress02").GetComponent<Text>().text = Mathf.RoundToInt(100f / achieves.Length * unlockCount).ToString() + "%";
        isInit = true;
    }

    private void Update()
    {
        if (!isInit)
        {
            return;
        }

        int index = (int)(rect_content.anchoredPosition.y / 23.5f);
        for (int i = 0; i < achieveSlots.Length; i++)
        {
            achieveSlots[Mathf.Abs(index % 7)].SetSlot(index, false);
            index++;
        }
    }
}
