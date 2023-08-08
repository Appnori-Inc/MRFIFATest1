using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonVoicePrefCtrl : MonoBehaviour
{
    private static PhotonVoicePrefCtrl instance;

    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        Destroy(this.gameObject);
    //    }
    //}
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
