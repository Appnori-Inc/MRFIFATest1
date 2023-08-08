using Appnori.XR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Billiards
{

    public class CacheTest : MonoBehaviour
    {
        [SerializeField]
        private GameObject xrRig;

        // Start is called before the first frame update
        void Start()
        {
            var rig = CacheManager.Get<XRRig>(xrRig);
            var pos = CacheManager.Get<XRGripPosition>(xrRig);
            var rot = CacheManager.Get<XRGripRotation>(xrRig);
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                CacheManager.Remove<XRRig>(xrRig);
            }
        }
    }

}