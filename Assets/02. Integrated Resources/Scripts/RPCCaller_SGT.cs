using UnityEngine;
using System.Collections;
using System;

using Photon.Pun;
using SingletonPunBase;

public class RPCCaller_SGT : Singleton<RPCCaller_SGT>
{

    //// Use this for initialization
    //void Start()
    //{
    //    RPC_SGT.GetInstance.PreRegisterTarget(func);
    //}

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        var data = new RPC_SGT.SerializableClass()
    //        {
    //            valueInt = 10,
    //            valueString = "call in " + PhotonNetwork.Time.ToString()
    //        };


    //        if (RPC_SGT.GetInstance.TrySend(func, data, RpcTarget.Others))
    //        {
    //            Debug.Log("Send : " + data.valueInt + "," + data.valueString);
    //        }
    //    }
    //}

    //public void func(RPC_SGT.SerializableClass packet)
    //{
    //    Debug.Log("Receive : " + packet.valueInt + "," + packet.valueString);
    //}
}
