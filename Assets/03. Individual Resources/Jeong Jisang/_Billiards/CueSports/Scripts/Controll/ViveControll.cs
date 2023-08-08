using System;
using UnityEngine;
using System.Collections;

namespace Billiards
{
    using Unity.XR.CoreUtils;
    using UnityEngine.XR.Interaction.Toolkit;

    public class ViveControll : MonoBehaviour
    {
        [SerializeField]
        private XROrigin xrRig;

        [SerializeField]
        private XRRayInteractor interactor;

        [SerializeField]
        private XRController RightController;

        [SerializeField]
        private XRController LeftController;

        private Transform lastHit;
        private Transform currentHit;

        private void Awake()
        {
            Appnori.XR.XRControllerState.Button target =
                Appnori.XR.XRControllerState.Button.MenuButton |
                Appnori.XR.XRControllerState.Button.Trigger |
                Appnori.XR.XRControllerState.Button.Grip |
                Appnori.XR.XRControllerState.Button.Primary2DAxisTouch |
                Appnori.XR.XRControllerState.Button.Primary2DAxisClick |
                Appnori.XR.XRControllerState.Button.PrimaryAxis2DUp |
                Appnori.XR.XRControllerState.Button.PrimaryAxis2DDown |
                Appnori.XR.XRControllerState.Button.PrimaryAxis2DRight |
                Appnori.XR.XRControllerState.Button.PrimaryAxis2DLeft;

            BilliardsDataContainer.Instance.XRRigid.Value = xrRig;
            BilliardsDataContainer.Instance.XRLeftControllerState.Initialize(xrRig, LeftController, target);
            BilliardsDataContainer.Instance.XRRightControllerState.Initialize(xrRig, RightController, target);
        }

        private void Raycast(out RaycastHit hit)
        {
            if(interactor.TryGetCurrent3DRaycastHit(out hit))
            {
                currentHit = hit.transform;

                if (currentHit != lastHit && lastHit != null)
                {
                    var react = CacheManager.Get<BaseRaycastReactor>(lastHit);
                    if (react != null)
                    {
                        react.OnRayEnd(hit);
                    }
                }

                lastHit = hit.transform;
            }
            else
            {
                currentHit = null;

                if (currentHit != lastHit && lastHit != null)
                {
                    var react = CacheManager.Get<BaseRaycastReactor>(lastHit);
                    if (react != null)
                    {
                        react.OnRayEnd(hit);
                    }
                }

            }
        }

        void Update()
        {
            bool isReacted = false;
            Raycast(out var hit);

            var react = CacheManager.Get<BaseRaycastReactor>(hit.transform);
            if (react != null)
            {
                switch (react)
                {
                    //if need overload, add here
                    case BaseRaycastReactor baseReactor: baseReactor.OnRay(hit); break;
                    default: react.OnRay(hit); break;
                }
            }

            if (BilliardsDataContainer.Instance.XRRightControllerState[InputHelpers.Button.Trigger].Value)
            {
                if (react != null)
                {
                    switch (react)
                    {
                        //if need overload, add here
                        case BaseRaycastReactor baseReactor: isReacted = baseReactor.RaycastReact(hit); break;
                        default: isReacted = react.RaycastReact(hit); break;
                    }
                }
            }

            BilliardsDataContainer.Instance.XRLeftControllerState.ForceUpdate(BilliardsDataContainer.Instance.XRLeftControllerState.Controller.inputDevice);
            BilliardsDataContainer.Instance.XRRightControllerState.ForceUpdate(BilliardsDataContainer.Instance.XRRightControllerState.Controller.inputDevice);

        }


    }

}