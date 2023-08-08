using SingletonBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LobbyHologramCtrl : Singleton<LobbyHologramCtrl>
{
    bool isPlay = false;
    int gameNum;

    private Animator anim;
    public GameObject arrow;
    private LobbyPropData lobbyPropData = null;

    public GameObject[] gos_activeProp;

    private Transform[] boneTrs;

    public Vector3[] setPoses;
    public Vector3[] setRots;
    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        anim.gameObject.SetActive(false);
        arrow.SetActive(false);
        boneTrs = new Transform[59];

        Transform[] transform_temp = anim.transform.GetComponentsInChildren<Transform>(true);

        lobbyPropData = new LobbyPropData();

        for (int i = 0; i < transform_temp.Length; i++)
        {
            switch (transform_temp[i].name)
            {
                case "ITEM_HEAD":
                    {
                        lobbyPropData.mesh_head = transform_temp[i].GetComponent<MeshFilter>();
                    }
                    break;
                case "ITEM_HAIR_F":
                    {
                        lobbyPropData.mesh_hair_f = transform_temp[i].GetComponent<MeshFilter>();
                    }
                    break;
                case "ITEM_HAIR_B":
                    {
                        lobbyPropData.mesh_hair_b = transform_temp[i].GetComponent<MeshFilter>();
                    }
                    break;
                case "BODY_ORG_HIDDEN":
                    {
                        lobbyPropData.mesh_body = transform_temp[i].GetComponent<SkinnedMeshRenderer>();
                    }
                    break;
                case "ITEM_UPPER":
                    {
                        lobbyPropData.mesh_upper = transform_temp[i].GetComponent<SkinnedMeshRenderer>();
                    }
                    break;
                case "ITEM_LOWER":
                    {
                        lobbyPropData.mesh_lower = transform_temp[i].GetComponent<SkinnedMeshRenderer>();
                    }
                    break;
                case "ITEM_LOWER_SUB":
                    {
                        lobbyPropData.mesh_lower_s = transform_temp[i].GetComponent<SkinnedMeshRenderer>();
                    }
                    break;
                case "ITEM_FOOT":
                    {
                        lobbyPropData.mesh_foot = transform_temp[i].GetComponent<SkinnedMeshRenderer>();
                    }
                    break;
                case "ITEM_HEAD_ACC":
                    {
                        lobbyPropData.mesh_acc = transform_temp[i].GetComponent<MeshFilter>();
                    }
                    break;
            }

            int index = WearInfoCreator.BoneNameToIndex(transform_temp[i].name);
            if (index == -1)
            {
                continue;
            }

            boneTrs[index] = transform_temp[i];
        }
    }

    public void PlayHologram()
    {
        if (isPlay)
        {
            return;
        }
        anim.gameObject.SetActive(true);
        arrow.SetActive(true);
        anim.SetInteger("StateNum", gameNum);
        anim.SetTrigger("OnState");
        anim.SetBool("IsPlay", true);
        isPlay = true;
    }

    public void StopHologram()
    {
        if (!isPlay)
        {
            return;
        }

        anim.gameObject.SetActive(false);
        arrow.SetActive(false);
        isPlay = false;
    }

    public void SetHologram(LobbyPropData _propData, GameDataManager.GameType _gameType)
    {
        //ITEM_HEAD
        lobbyPropData.mesh_head.sharedMesh = _propData.mesh_head.sharedMesh;
        //ITEM_HAIR_F
        if (_propData.mesh_hair_f != null)
        {
            lobbyPropData.mesh_hair_f.sharedMesh = _propData.mesh_hair_f.sharedMesh;
            lobbyPropData.mesh_hair_f.gameObject.SetActive(true);
        }
        else
        {
            lobbyPropData.mesh_hair_f.gameObject.SetActive(false);
        }
        //ITEM_HAIR_B
        if (_propData.mesh_hair_b != null)
        {
            lobbyPropData.mesh_hair_b.sharedMesh = _propData.mesh_hair_b.sharedMesh;
            lobbyPropData.mesh_hair_b.gameObject.SetActive(true);
        }
        else
        {
            lobbyPropData.mesh_hair_b.gameObject.SetActive(false);
        }
        //BODY_ORG_HIDDEN
        lobbyPropData.mesh_body.sharedMesh = _propData.mesh_body.sharedMesh;
        lobbyPropData.mesh_body.localBounds = new Bounds(_propData.mesh_body.localBounds.center, _propData.mesh_body.localBounds.extents * 2f);
        //ITEM_UPPER
        lobbyPropData.mesh_upper.sharedMesh = _propData.mesh_upper.sharedMesh;
        lobbyPropData.mesh_upper.localBounds = new Bounds(_propData.mesh_upper.localBounds.center, _propData.mesh_upper.localBounds.extents * 2f);
        //ITEM_LOWER
        if (_propData.mesh_lower != null)
        {
            lobbyPropData.mesh_lower.sharedMesh = _propData.mesh_lower.sharedMesh;
            lobbyPropData.mesh_lower.localBounds = new Bounds(_propData.mesh_lower.localBounds.center, _propData.mesh_lower.localBounds.extents * 2f);
            lobbyPropData.mesh_lower.gameObject.SetActive(true);
        }
        else
        {
            lobbyPropData.mesh_lower.gameObject.SetActive(false);
        }
        //ITEM_LOWER_SUB
        if (_propData.mesh_lower_s != null)
        {
            lobbyPropData.mesh_lower_s.sharedMesh = _propData.mesh_lower_s.sharedMesh;
            lobbyPropData.mesh_lower_s.localBounds = new Bounds(_propData.mesh_lower_s.localBounds.center, _propData.mesh_lower_s.localBounds.extents * 2f);
            lobbyPropData.mesh_lower_s.gameObject.SetActive(true);
        }
        else
        {
            lobbyPropData.mesh_lower_s.gameObject.SetActive(false);
        }
        //ITEM_FOOT
        if (_propData.mesh_foot != null)
        {
            lobbyPropData.mesh_foot.sharedMesh = _propData.mesh_foot.sharedMesh;
            lobbyPropData.mesh_foot.localBounds = new Bounds(_propData.mesh_foot.localBounds.center, _propData.mesh_foot.localBounds.extents * 2f);
            lobbyPropData.mesh_foot.gameObject.SetActive(true);
        }
        else
        {
            lobbyPropData.mesh_foot.gameObject.SetActive(false);
        }
        //ITEM_HEAD_ACC
        if (_propData.mesh_acc != null)
        {
            lobbyPropData.mesh_acc.sharedMesh = _propData.mesh_acc.sharedMesh;
            lobbyPropData.mesh_acc.gameObject.SetActive(true);
        }
        else
        {
            lobbyPropData.mesh_acc.gameObject.SetActive(false);
        }

        gameNum = (int)_gameType;

        anim.transform.localPosition = setPoses[gameNum - 1];
        anim.transform.localRotation = Quaternion.Euler(setRots[gameNum - 1]);

        switch (_gameType)
        {
            case GameDataManager.GameType.Bowling:
                {
                    for (int i = 0; i < gos_activeProp.Length; i++)
                    {
                        if (i == 0)
                        {
                            gos_activeProp[i].SetActive(true);
                            continue;
                        }
                        gos_activeProp[i].SetActive(false);
                    }
                }
                break;
            case GameDataManager.GameType.Archery:
                {
                    for (int i = 0; i < gos_activeProp.Length; i++)
                    {
                        if (i == 1)
                        {
                            gos_activeProp[i].SetActive(true);
                            continue;
                        }
                        gos_activeProp[i].SetActive(false);
                    }
                }
                break;
            case GameDataManager.GameType.Basketball:
                {
                    for (int i = 0; i < gos_activeProp.Length; i++)
                    {
                        if (i == 2)
                        {
                            gos_activeProp[i].SetActive(true);
                            continue;
                        }
                        gos_activeProp[i].SetActive(false);
                    }
                }
                break;
            case GameDataManager.GameType.Badminton:
                {
                    for (int i = 0; i < gos_activeProp.Length; i++)
                    {
                        if (i == 3 || i == 4)
                        {
                            gos_activeProp[i].SetActive(true);
                            continue;
                        }
                        gos_activeProp[i].SetActive(false);
                    }
                }
                break;
            case GameDataManager.GameType.Billiards:
                {
                    for (int i = 0; i < gos_activeProp.Length; i++)
                    {
                        if (i == 5)
                        {
                            gos_activeProp[i].SetActive(true);
                            continue;
                        }
                        gos_activeProp[i].SetActive(false);
                    }
                }
                break;
            case GameDataManager.GameType.Darts:
                {
                    for (int i = 0; i < gos_activeProp.Length; i++)
                    {
                        if (i == 6)
                        {
                            gos_activeProp[i].SetActive(true);
                            continue;
                        }
                        gos_activeProp[i].SetActive(false);
                    }
                }
                break;
            case GameDataManager.GameType.TableTennis:
                {
                    for (int i = 0; i < gos_activeProp.Length; i++)
                    {
                        if (i == 7 || i == 8)
                        {
                            gos_activeProp[i].SetActive(true);
                            continue;
                        }
                        gos_activeProp[i].SetActive(false);
                    }
                }
                break;
            case GameDataManager.GameType.Boxing:
                {
                    for (int i = 0; i < gos_activeProp.Length; i++)
                    {
                        if (i == 9)
                        {
                            gos_activeProp[i].SetActive(true);
                            continue;
                        }
                        gos_activeProp[i].SetActive(false);
                    }
                }
                break;
            case GameDataManager.GameType.Golf:
                {
                    for (int i = 0; i < gos_activeProp.Length; i++)
                    {
                        if (i == 10 || i == 11)
                        {
                            gos_activeProp[i].SetActive(true);
                            continue;
                        }
                        gos_activeProp[i].SetActive(false);
                    }
                }
                break;
            case GameDataManager.GameType.Baseball:
                {
                    for (int i = 0; i < gos_activeProp.Length; i++)
                    {
                        if (i == 12 || i == 13)
                        {
                            gos_activeProp[i].SetActive(true);
                            continue;
                        }
                        gos_activeProp[i].SetActive(false);
                    }
                }
                break;
            case GameDataManager.GameType.Tennis:
                {
                    for (int i = 0; i < gos_activeProp.Length; i++)
                    {
                        if (i == 14 || i == 15)
                        {
                            gos_activeProp[i].SetActive(true);
                            continue;
                        }
                        gos_activeProp[i].SetActive(false);
                    }
                }
                break;
            default:
                {
                    for (int i = 0; i < gos_activeProp.Length; i++)
                    {
                        gos_activeProp[i].SetActive(false);
                    }
                }
                break;
        }
    }
}
