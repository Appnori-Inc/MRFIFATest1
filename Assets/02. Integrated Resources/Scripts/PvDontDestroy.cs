using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PvDontDestroy : MonoBehaviour
{
    private PhotonView pv;
    // Start is called before the first frame update
    void Start()
    {
        if(GameDataManager.instance != null)
        {
            if(GameDataManager.instance.playType == GameDataManager.PlayType.Multi)
            {
                pv = GetComponent<PhotonView>();
                pv.isRuntimeInstantiated = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
