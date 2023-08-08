using Billiards;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace Appnori.XR
{

    public class XRReticleProvider : MonoBehaviour
    {
        [Serializable]
        public class ReticlePositionEvent : UnityEvent<XRNode, Vector3> { }

        [SerializeField]
        private XRNode ControllerNode;

        public ReticlePositionEvent ReticleMovementevent;

        private void Awake()
        {
            switch (ControllerNode)
            {
                case XRNode.LeftHand:
                    BilliardsDataContainer.Instance.XRLeftReticleProvider.Value = this;
                    break;
                case XRNode.RightHand:
                    BilliardsDataContainer.Instance.XRRightReticleProvider.Value = this;
                    break;
                default:
                    break;
            }
        }

        void Update()
        {
            if (ReticleMovementevent.GetPersistentEventCount() != 0)
            {
                ReticleMovementevent.Invoke(ControllerNode, transform.position);
            }
        }
    }

}