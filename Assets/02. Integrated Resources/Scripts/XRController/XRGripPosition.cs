using Billiards;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Appnori.Util;
using BallPool.Mechanics;
using Unity.XR.CoreUtils;

namespace Appnori.XR
{
    public class XRGripPosition : MonoBehaviour
    {
        [SerializeField]
        private XROrigin xrRig;

        [SerializeField]
        private XRController mainController;

        [SerializeField]
        private XRController subController;

        [SerializeField]
        private InputHelpers.Button buttonType;

        public bool IsAllowedMove { get; set; }
        public Notifier<bool> IsAllowed = new Notifier<bool>();

        private Vector2 defaultPosition;
        private Notifier<bool> isTotalPressed = new Notifier<bool>();


        private void Awake()
        {
            IsAllowedMove = true;
            isTotalPressed.OnDataChanged += SetAllowPosition;
        }

        private void SetAllowPosition(bool isPressed)
        {
            IsAllowed.Value = isPressed && IsAllowedMove;
            if (isPressed)
            {
                defaultPosition = GetPosition();
            }
        }

        private void FixedUpdate()
        {
            mainController.inputDevice.IsPressed(buttonType, out var mainPressed, mainController.axisToPressThreshold);
            subController.inputDevice.IsPressed(buttonType, out var subPressed, subController.axisToPressThreshold);

            isTotalPressed.Value = mainPressed && subPressed;

            if (IsAllowed.Value)
            {
                var currentPosition = GetPosition();
                var delta = (defaultPosition - currentPosition);
                xrRig.MoveCameraToWorldLocation(xrRig.Camera.transform.position + (xrRig.transform.rotation * delta.ToVector3FromXZ()));
                defaultPosition = currentPosition;
            }
        }

        private Vector2 GetPosition()
        {
            return Vector2.Lerp(subController.transform.localPosition.ToXZ(), mainController.transform.localPosition.ToXZ(), 0.5f);
        }

        private void OnDestroy()
        {
            isTotalPressed.OnDataChanged -= SetAllowPosition;
        }

    }
}