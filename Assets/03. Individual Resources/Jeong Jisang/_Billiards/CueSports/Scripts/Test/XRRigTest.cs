using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Appnori.XR;
using Appnori.Util;
using UnityEngine.XR.Interaction.Toolkit;


namespace Billiards
{

    public class XRRigTest : MonoBehaviour
    {
        [SerializeField] private GameObject Marker1;
        [SerializeField] private GameObject Marker2;
        [SerializeField] private GameObject Marker3;
        [SerializeField] private GameObject Marker4;

        [SerializeField]
        private XRRig targetRig;

        [SerializeField]
        private ColliderEventRaiser HeadTracker;

        private Queue<Vector3> lastPositions =new Queue<Vector3>();
        private Vector3 lastPosition;
        private Vector3 currentPosition;

        private bool onTriggerEnter;

        private void Awake()
        {
            onTriggerEnter = false;

            HeadTracker.OnTriggerEnterEvent += HeadTracker_OnTriggerEnterEvent;
            HeadTracker.OnTriggerStayEvent += HeadTracker_OnTriggerStayEvent;
            HeadTracker.OnTriggerExitEvent += HeadTracker_OnTriggerExitEvent;
        }

        private void HeadTracker_OnTriggerEnterEvent(Collider obj)
        {
            onTriggerEnter = true;
        }

        private void HeadTracker_OnTriggerStayEvent(Collider obj)
        {
            var delta = lastPositions.Peek() - currentPosition;
            targetRig.MoveCameraToWorldLocation(targetRig.cameraGameObject.transform.position + delta * 1.1f);
        }

        private void HeadTracker_OnTriggerExitEvent(Collider obj)
        {
            onTriggerEnter = false;
        }

        private void Update()
        {
            Marker1.transform.position = targetRig.rigInCameraSpacePos;
            Marker2.transform.position = targetRig.cameraInRigSpacePos;
            Marker3.transform.position = targetRig.cameraGameObject.transform.position;
            Marker4.transform.position = targetRig.transform.position;

            if (!onTriggerEnter)
            {
                lastPosition = currentPosition;
                lastPositions.Enqueue(currentPosition);
                if (lastPositions.Count > 4)
                {
                    lastPositions.Dequeue();
                }
            }

            currentPosition = targetRig.cameraGameObject.transform.position;
        }
    }
}