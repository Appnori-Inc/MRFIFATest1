using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalViewCtrl : MonoBehaviour
{
    public class MedalInfo
    {
        public Transform transform;
        public Vector3 startPos;
    }

    public Mesh[] meshes_medal;

    private MedalInfo[] medalInfos;

    public AnimationCurve animationCurve_pos;
    private float timeP = 0f;
    private float speed = 0.5f;

    private bool isInit = false;
    // Start is called before the first frame update
    void Awake()
    {
        medalInfos = new MedalInfo[transform.childCount];
        for (int i = 0; i < medalInfos.Length; i++)
        {
            medalInfos[i] = new MedalInfo();
            medalInfos[i].transform = transform.GetChild(i);
            medalInfos[i].startPos = medalInfos[i].transform.position;
            medalInfos[i].transform.gameObject.SetActive(false);
        }
        isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInit)
        {
            return;
        }

        timeP += Time.deltaTime * speed;
        if (timeP > 1f)
        {
            timeP -= 1f;
        }

        Vector3 pos = new Vector3(0f, animationCurve_pos.Evaluate(timeP),0f);
        Quaternion rot = Quaternion.Euler(0f, timeP * 360f, 90f);

        for (int i = 0; i < medalInfos.Length; i++)
        {
            medalInfos[i].transform.SetPositionAndRotation(medalInfos[i].startPos + pos, rot);
        }
    }

    public void Init()
    {
        medalInfos = new MedalInfo[transform.childCount];
        for (int i = 0; i < medalInfos.Length; i++)
        {
            medalInfos[i] = new MedalInfo();
            medalInfos[i].transform = transform.GetChild(i);
            medalInfos[i].startPos = medalInfos[i].transform.position;
            medalInfos[i].transform.GetComponent<MeshFilter>().sharedMesh = meshes_medal[0];
        }

        int count_bronze = 0;
        int count_silver = 0;
        int count_gold = 0;

        for (int i = 0; i < GameDataManager.clearLevelDatas.Length && i < medalInfos.Length; i++)
        {
            if (i == 8)
            {
                switch (GameDataManager.clearLevelDatas[i])
                {
                    case 9:
                        {
                            medalInfos[i].transform.GetComponent<MeshFilter>().sharedMesh = meshes_medal[0];
                            medalInfos[i].transform.gameObject.SetActive(true);
                            count_gold++;
                            count_silver++;
                            count_bronze++;
                        }
                        break;
                    case 8:
                        {
                            medalInfos[i].transform.GetComponent<MeshFilter>().sharedMesh = meshes_medal[1];
                            medalInfos[i].transform.gameObject.SetActive(true);
                            count_silver++;
                            count_bronze++;
                        }
                        break;
                    case 7:
                        {
                            medalInfos[i].transform.GetComponent<MeshFilter>().sharedMesh = meshes_medal[2];
                            medalInfos[i].transform.gameObject.SetActive(true);
                            count_bronze++;
                        }
                        break;
                    default:
                        {
                            medalInfos[i].transform.gameObject.SetActive(false);
                        }
                        break;
                }
            }
            else
            {
                switch (GameDataManager.clearLevelDatas[i])
                {
                    case 5:
                        {
                            medalInfos[i].transform.GetComponent<MeshFilter>().sharedMesh = meshes_medal[0];
                            medalInfos[i].transform.gameObject.SetActive(true);
                            count_gold++;
                            count_silver++;
                            count_bronze++;
                        }
                        break;
                    case 4:
                        {
                            medalInfos[i].transform.GetComponent<MeshFilter>().sharedMesh = meshes_medal[1];
                            medalInfos[i].transform.gameObject.SetActive(true);
                            count_silver++;
                            count_bronze++;
                        }
                        break;
                    case 3:
                        {
                            medalInfos[i].transform.GetComponent<MeshFilter>().sharedMesh = meshes_medal[2];
                            medalInfos[i].transform.gameObject.SetActive(true);
                            count_bronze++;
                        }
                        break;
                    default:
                        {
                            medalInfos[i].transform.gameObject.SetActive(false);
                        }
                        break;
                }
            }
        }


        if (count_bronze > 0)
        {
            GameDataManager.instance.UnlockAchieve("Ach01", count_bronze, GameDataManager.UnlockAchieveState.Change);
        }
        if (count_silver > 0)
        {
            GameDataManager.instance.UnlockAchieve("Ach02", count_silver, GameDataManager.UnlockAchieveState.Change);
        }
        if (count_gold > 0)
        {
            GameDataManager.instance.UnlockAchieve("Ach03", count_gold, GameDataManager.UnlockAchieveState.Change);
        }
        isInit = true;
    }
}
