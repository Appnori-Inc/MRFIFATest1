using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallPool;
using NetworkManagement;
using System;

namespace Billiards
{

    public class PhotonRPCTest : MonoSingleton<PhotonRPCTest>
    {
        //[SerializeField]
        //private bool Sender;

        //public Int64 SendCount; //발신자 발송횟수
        //public Int64 ReceiveCount; //발신자 수신횟수
        //public Int64 ReactCount; //수신자 수신횟수


        //public void ReceivePhotonRPCTest()
        //{
        //    if (!Sender)
        //    {
        //        ReactCount += 1;
        //        //NetworkManager.network.SendRemoteMessage("SendPhotonRPCTest");
        //    }
        //    else
        //    {
        //        ReceiveCount += 1;
        //    }

        //}

        //// Update is called once per frame
        //void Update()
        //{
        //    if (Sender)
        //    {
        //        //NetworkManager.network.SendRemoteMessage("SendPhotonRPCTest");
        //        SendCount += 1;
        //    }
        //}
    }

}