using Billiards;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Appnori.Util;
using Unity.XR.CoreUtils;

namespace Appnori.XR
{
    public class XRGripRotation : MonoBehaviour
    { 
        [SerializeField]
        private XROrigin xrRig;

        [SerializeField]
        private XRController mainController;

        [SerializeField]
        private XRController subController;

        [SerializeField]
        private InputHelpers.Button buttonType;

        public bool IsCanRotate { get; set; }
        public Notifier<bool> IsAllowed = new Notifier<bool>();

        private float defaultAngle;
        private Notifier<bool> isTotalPressed = new Notifier<bool>();


        private void Awake()
        {
            IsCanRotate = true;
            isTotalPressed.OnDataChanged += SetAllowRotate;
        }

        private void SetAllowRotate(bool isPressed)
        {
            IsAllowed.Value = isPressed && IsCanRotate;
            if (isPressed)
            {
                defaultAngle = GetAngle();
            }
        }

        void Update()
        {
            mainController.inputDevice.IsPressed(buttonType, out var mainPressed, mainController.axisToPressThreshold);
            subController.inputDevice.IsPressed(buttonType, out var subPressed, subController.axisToPressThreshold);

            isTotalPressed.Value = mainPressed && subPressed;

            if (IsAllowed.Value)
            {
                var currentAngle = GetAngle();
                xrRig.RotateAroundCameraUsingOriginUp(currentAngle - defaultAngle);
                defaultAngle = currentAngle;
            }
        }

        private float GetAngle()
        {
            return Vector2.SignedAngle(Vector2.up, subController.transform.localPosition.ToXZ() - mainController.transform.localPosition.ToXZ());
        }

        private void OnDestroy()
        {
            isTotalPressed.OnDataChanged -= SetAllowRotate;
        }

    }
}

