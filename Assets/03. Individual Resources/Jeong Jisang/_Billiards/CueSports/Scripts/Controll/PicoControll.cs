#if !UNITY_EDITOR && UNITY_ANDROID 
#define ANDROID_DEVICE
#endif

using System;
using UnityEngine;
using System.Collections;

namespace Billiards
{
    //using Pvr_UnitySDKAPI;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [Serializable]
    public class ControllerObject
    {
        public GameObject controller;
        public Transform dot;
        public Transform start;
        public Transform ray_alpha;
        public Transform CursorDot;

        public void SetActive(bool value)
        {
            controller.SetActive(value);
        }
    }


    [Obsolete("Use ViveControll instead", true)]
    public class PicoControll : MonoBehaviour
    {
        [SerializeField]
        private GameObject HeadSetController;

        [SerializeField]
        private ControllerObject controller0;
        [SerializeField]
        private ControllerObject controller1;


        private Ray ray;

        private int CachedMainHandNess = -1;
        private ControllerObject currentController
        {
            get
            {
                if (CachedMainHandNess == -1)
                    CachedMainHandNess = 0;

                if (CachedMainHandNess == 0)
                    return controller0;
                if (CachedMainHandNess == 1)
                    return controller1;

                return null;
            }
        }

        private bool TryGetController(int idx, out ControllerObject obj)
        {
            switch (idx)
            {
                case 0: obj = controller0; return true;
                case 1: obj = controller1; return true;
                default: obj = null; return false;
            }
        }



        private Transform lastHit;
        private Transform currentHit;

        public float rayDefaultLength = 4;
        private bool isHasController = false;
        private bool headcontrolmode = false;

        public static bool GetTotalTriggerUp =>
            Input.GetMouseButtonUp(0);
        //Controller.UPvr_GetKeyUp(0, Pvr_KeyCode.TRIGGER) ||
        //Controller.UPvr_GetKeyUp(1, Pvr_KeyCode.TRIGGER);

        public static bool GetTotalTriggerDown =>
            Input.GetMouseButtonDown(0);
        //Controller.UPvr_GetKeyDown(0, Pvr_KeyCode.TRIGGER) ||
        //Controller.UPvr_GetKeyDown(1, Pvr_KeyCode.TRIGGER);

        public static bool GetTotalTrigger =>
            Input.GetMouseButton(0);
        //Controller.UPvr_GetKey(0, Pvr_KeyCode.TRIGGER) ||
        //Controller.UPvr_GetKey(1, Pvr_KeyCode.TRIGGER);

        public static bool GetTotalTouched =>
            Input.GetMouseButton(1);
        //Controller.UPvr_IsTouching(0) ||
        //Controller.UPvr_IsTouching(1);

        public static bool GetTotalTouchButton =>
            Input.GetKeyDown(KeyCode.JoystickButton0) ||
            Input.GetMouseButton(1);
        //Controller.UPvr_GetKey(0, Pvr_KeyCode.TOUCHPAD) ||
        //Controller.UPvr_GetKey(1, Pvr_KeyCode.TOUCHPAD);

        public static Vector2 GetTotalTouchPad =>
            new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //Controller.UPvr_GetTouchPadPosition(CachedMainHandNess);

        public static Vector3 GetAngularVelocity =>
            new Vector3(Mathf.Clamp(Input.GetAxis("Mouse Y") * GameConfig.ControllerXAxisAngularVelocityRate, -GameConfig.MaxControllerXAxisAngularVelocity, GameConfig.MaxControllerXAxisAngularVelocity), 0, 0);
        //Controller.UPvr_GetAngularVelocity(CachedMainHandNess);

        public static bool GetTotalBackButton =>
            Input.GetKeyDown(KeyCode.Escape);
        //Controller.UPvr_GetKey(0, Pvr_KeyCode.APP) ||
        //Controller.UPvr_GetKey(1, Pvr_KeyCode.APP);


        void Start()
        {
            ray = new Ray();
            //if (Pvr_UnitySDKManager.SDK.isHasController)
            //{
            //    //Pvr_ControllerManager.PvrServiceStartSuccessEvent += OnServiceStartSuccessed;
            //    //Pvr_ControllerManager.SetControllerStateChangedEvent += OnControllerStateChanged;
            //    //Pvr_ControllerManager.ControllerStatusChangeEvent += OnCheckControllerStateForGoblin;
            //    isHasController = true;

            //    //Controller.UPvr_SetMainHandNess(0);
            //    //SetupController();
            //}
        }

        void OnDestroy()
        {
            if (isHasController)
            {
                //Pvr_ControllerManager.PvrServiceStartSuccessEvent -= OnServiceStartSuccessed;
                //Pvr_ControllerManager.SetControllerStateChangedEvent -= OnControllerStateChanged;
                //Pvr_ControllerManager.ControllerStatusChangeEvent -= OnCheckControllerStateForGoblin;
            }
        }

        private bool TrySetRay()
        {
            if (HeadSetController.activeSelf)
            {
                //Pvr_UnitySDKManager.SDK.HeadPose.Orientation.eulerAngles (x,y)
                var eulerAngles = new Vector3(0, 0, 0);

                //headset
                HeadSetController.transform.parent.localRotation = Quaternion.Euler(eulerAngles);

                ray.direction = HeadSetController.transform.position - HeadSetController.transform.parent.parent.Find("Head").position;
                ray.origin = HeadSetController.transform.parent.parent.Find("Head").position;
                return true;
            }
            else if (currentController != null)
            {
                //controller
                ray.direction = currentController.CursorDot.position - currentController.start.position;
                ray.origin = currentController.start.position;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Raycast(out RaycastHit hit)
        {
            if (Physics.Raycast(ray, out hit, 10f, layerMask: 1 << Appnori.Layer.NameToLayer(Appnori.Layer.GameType.Billiards, "RayReactor")))
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

                //legacy code
                if (HeadSetController.activeSelf)
                {
                    if (HeadSetController.name == "SightPointer")
                    {
                        HeadSetController.transform.localScale = Vector3.zero;
                    }
                }

                //if (Pvr_ControllerManager.Instance.LengthAdaptiveRay)
                //{
                //    if (HeadSetController.activeSelf)
                //    {
                //        //headset
                //        HeadSetController.transform.position = HeadSetController.transform.parent.parent.Find("Head").position + ray.direction.normalized * (0.5f + rayDefaultLength);
                //        HeadSetController.transform.localScale = new Vector3(0.008f, 0.008f, 1);
                //    }
                //    else
                //    {
                //        //controller
                //        currentController.dot.localScale = new Vector3(0.178f, 0.178f, 1);
                //        currentController.dot.position = currentController.controller.transform.position + currentController.controller.transform.forward.normalized * (0.5f + rayDefaultLength);
                //    }
                //}
            }
        }

        void Update()
        {
            bool isReacted = false;
            if (TrySetRay())
            {
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

                if (GetTotalTriggerDown)
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
            }

//            //data change
//            BilliardsDataContainer.Instance.ControllerTrigger.isEnabled = isReacted == false;

//            BilliardsDataContainer.Instance.ControllerBackButton.CurrentData = GetTotalBackButton;
//            BilliardsDataContainer.Instance.ControllerTrackButton.CurrentData = GetTotalTouchButton;
//            BilliardsDataContainer.Instance.ControllerTrigger.CurrentData = GetTotalTrigger;

//            BilliardsDataContainer.Instance.ControllerRayOrigin.CurrentData = currentController.start.position;
//            BilliardsDataContainer.Instance.ControllerRayTarget.CurrentData = currentController.CursorDot.position;


//            var tempPosition = GetTotalTouchPad;
//            tempPosition = new Vector2(Mathf.Clamp(tempPosition.y, GameConfig.TouchRectMin.x, GameConfig.TouchRectMax.x), Mathf.Clamp(tempPosition.x, GameConfig.TouchRectMin.y, GameConfig.TouchRectMax.y));
//            tempPosition = new Vector2((tempPosition.x - GameConfig.TouchRectMin.x) - ((GameConfig.TouchRectMax.x - GameConfig.TouchRectMin.x) * 0.5f), (tempPosition.y - GameConfig.TouchRectMin.y) - ((GameConfig.TouchRectMax.y - GameConfig.TouchRectMin.y) * 0.5f));
//            tempPosition = new Vector2(tempPosition.x / ((GameConfig.TouchRectMax.x - GameConfig.TouchRectMin.x) * 0.5f), tempPosition.y / ((GameConfig.TouchRectMax.y - GameConfig.TouchRectMin.y) * 0.5f));

//            BilliardsDataContainer.Instance.ControllerTrackTouch.CurrentData = GetTotalTouched;
//            BilliardsDataContainer.Instance.ControllerTrackPosition.isEnabled = GetTotalTouched;
//            BilliardsDataContainer.Instance.ControllerTrackPosition.CurrentData = tempPosition;

//            var angularVelocity = GetAngularVelocity;
//            BilliardsDataContainer.Instance.ControllerAngularVelocity.CurrentData =
//                new Vector3(Mathf.Clamp(angularVelocity.x * GameConfig.ControllerXAxisAngularVelocityRate, -GameConfig.MaxControllerXAxisAngularVelocity, GameConfig.MaxControllerXAxisAngularVelocity), 0, 0);

//#if UNITY_EDITOR
//            BilliardsDataContainer.Instance.ControllerAngularVelocity.CurrentData =
//                new Vector3(Mathf.Clamp(Input.GetAxis("Mouse Y") * GameConfig.ControllerXAxisAngularVelocityRate, -GameConfig.MaxControllerXAxisAngularVelocity, GameConfig.MaxControllerXAxisAngularVelocity), 0, 0);

//            BilliardsDataContainer.Instance.ControllerTrackTouch.CurrentData = Input.GetKey(KeyCode.Space);

//            BilliardsDataContainer.Instance.ControllerTrackPosition.CurrentData =
//                new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
//#endif
        }


        //event
        //private void OnServiceStartSuccessed()
        //{
        //    if (Controller.UPvr_GetControllerState(0) == ControllerState.Connected ||
        //        Controller.UPvr_GetControllerState(1) == ControllerState.Connected)
        //    {
        //        HeadSetController.SetActive(false);
        //        SetupController();
        //    }
        //    else
        //    {
        //        HeadSetController.SetActive(true);
        //    }
        //}

        //private void OnControllerStateChanged(string data)
        //{

        //    if (Controller.UPvr_GetControllerState(0) == ControllerState.Connected ||
        //        Controller.UPvr_GetControllerState(1) == ControllerState.Connected)
        //    {
        //        HeadSetController.SetActive(false);
        //        SetupController();
        //    }
        //    else
        //    {
        //        HeadSetController.SetActive(true);
        //    }
        //}

//        private void SetupController()
//        {
//            var lastIndex = CachedMainHandNess;
//            var currentIndex = Controller.UPvr_GetMainHandNess();

//#if UNITY_EDITOR
//            currentIndex = 0;
//#endif

//            if (lastIndex != currentIndex)
//            {
//                if (TryGetController(lastIndex, out var last))
//                {
//                    last.controller.SetActive(false);
//                }

//                if (TryGetController(currentIndex, out var current))
//                {
//                    current.controller.SetActive(true);
//                    current.dot.gameObject.SetActive(true);
//                    current.ray_alpha.gameObject.SetActive(true);
//                    current.CursorDot.gameObject.SetActive(true);
//                }
//            }

//            CachedMainHandNess = currentIndex;
//        }

        //private void OnCheckControllerStateForGoblin(string state)
        //{
        //    HeadSetController.SetActive(!Convert.ToBoolean(Convert.ToInt16(state)));
        //}

        public void SwitchControlMode()
        {
#if UNITY_EDITOR
            if (headcontrolmode)
            {
                headcontrolmode = false;
                HeadSetController.SetActive(false);
                controller0.SetActive(true);
                controller1.SetActive(true);
            }
            else
            {
                headcontrolmode = true;
                HeadSetController.SetActive(true);
                controller0.SetActive(false);
                controller1.SetActive(false);
            }
#endif
        }

    }

}