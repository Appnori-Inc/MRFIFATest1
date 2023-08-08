using UnityEngine;
using System.Collections;
using System;
using Appnori.Util;
using UnityEngine.XR.Interaction.Toolkit;

namespace Appnori.XR
{

    public class XRContainer : Billiards.Singleton<XRContainer>
    {
        public Notifier<XRRig> CurrentRig = new Notifier<XRRig>();


    }
}