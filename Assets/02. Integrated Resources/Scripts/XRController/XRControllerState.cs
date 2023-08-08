using UnityEngine;
using System.Collections;
using Appnori.Util;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using System.Collections.Generic;
using UnityEngine.XR;
using Unity.Collections.LowLevel.Unsafe;
using Billiards;
using Unity.XR.CoreUtils;

namespace Appnori.XR
{
    using XRController = UnityEngine.XR.Interaction.Toolkit.XRController;

    public class XRControllerState /*: Notifier<XRControllerState>*/
    {
        [Flags]
        public enum Button
        {
            None = 0,
            MenuButton = (1 << 0),
            Trigger = (1 << 1),
            Grip = (1 << 2),
            TriggerPressed = (1 << 3),
            GripPressed = (1 << 4),
            PrimaryButton = (1 << 5),
            PrimaryTouch = (1 << 6),
            SecondaryButton = (1 << 7),
            SecondaryTouch = (1 << 8),
            Primary2DAxisTouch = (1 << 9),
            Primary2DAxisClick = (1 << 10),
            Secondary2DAxisTouch = (1 << 11),
            Secondary2DAxisClick = (1 << 12),
            PrimaryAxis2DUp = (1 << 13),
            PrimaryAxis2DDown = (1 << 14),
            PrimaryAxis2DLeft = (1 << 15),
            PrimaryAxis2DRight = (1 << 16),
            SecondaryAxis2DUp = (1 << 17),
            SecondaryAxis2DDown = (1 << 18),
            SecondaryAxis2DLeft = (1 << 19),
            SecondaryAxis2DRight = (1 << 20),
        }
        class InputButtonComparer : IEqualityComparer<InputHelpers.Button>
        {
            public bool Equals(InputHelpers.Button x, InputHelpers.Button y)
            {
                return x == y;
            }
            public int GetHashCode(InputHelpers.Button obj)
            {
                return (int)obj;
            }
        }

        public XRControllerState()
        {
            TriggerNotifier = new Notifier<float>();
            VelocityNotifier = new Notifier<Vector3>();
            AngularVelocityNotifier = new Notifier<Vector3>();
            Primary2DAxisNotifier = new Notifier<Vector2>();
            Position = new Notifier<Vector3>();
            Rotation = new Notifier<Quaternion>();
            ControllerRayOrigin = new Notifier<Vector3>();
            ControllerRayTarget = new Notifier<Vector3>();
        }

        public XRController Controller { get; private set; }

        private Dictionary<InputHelpers.Button, Notifier<bool>> ButtonStateDict = new Dictionary<InputHelpers.Button, Notifier<bool>>(new InputButtonComparer());

        public bool Exists(InputHelpers.Button type)
        {
            return ButtonStateDict.ContainsKey(type);
        }

        public Notifier<bool> this[InputHelpers.Button type]
        {
            get
            {
                if (!ButtonStateDict.TryGetValue(type, out var notifier))
                {
                    notifier = new Notifier<bool>();
                    ButtonStateDict.Add(type, notifier);
                }

                return notifier;
            }
        }

        public Notifier<float> TriggerNotifier { get; private set; }

        public Notifier<Vector3> VelocityNotifier { get; private set; }
        public Notifier<Vector3> AngularVelocityNotifier { get; private set; }

        public Notifier<Vector2> Primary2DAxisNotifier { get; private set; }
        public Notifier<Vector3> Position { get; private set; }
        public Notifier<Quaternion> Rotation { get; private set; }

        public Notifier<Vector3> ControllerRayOrigin { get; private set; }
        public Notifier<Vector3> ControllerRayTarget { get; private set; }

        private List<InputHelpers.Button> UpdateTargets = new List<InputHelpers.Button>();

        private XRRayInteractor interactor;

        private XROrigin XRRigid;

        private int LastState = 0;

        public bool IsInitialized { get; private set; }

        public void Initialize(XROrigin xrRig,XRController controller, Button target)
        {
            LastState = 0;

            Controller = controller;

            for (int i = (int)InputHelpers.Button.MenuButton; i <= (int)InputHelpers.Button.PrimaryAxis2DRight; ++i)
            {
                int flag = 1 << (i - 1);
                if (target.HasFlag((Button)flag))
                {
                    UpdateTargets.Add((InputHelpers.Button)i);
                }
            }

            IsInitialized = true;

            interactor = controller.GetComponent<XRRayInteractor>();
            XRRigid = xrRig;
        }

        public void ForceUpdate(in InputDevice inputDevice)
        {
            if (!IsInitialized)
                return;

            localFunc_1(inputDevice);
            localFunc_2(inputDevice);
            localFunc_3(inputDevice);


            void localFunc_1(in InputDevice id)
            {
                using (var e = UpdateTargets.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        id.IsPressed(e.Current, out var isPressed, Controller.axisToPressThreshold);
                        this[e.Current].Value = isPressed;
                    }
                }
            }

            void localFunc_2(in InputDevice id)
            {

                if (id.TryGetFeatureValue(CommonUsages.trigger, out var triggerValue))
                {
                    TriggerNotifier.Value = triggerValue;
                }

                if (id.TryGetFeatureValue(CommonUsages.primary2DAxis, out var touchpadPosition))
                {
                    Primary2DAxisNotifier.Value = Vector2.ClampMagnitude(touchpadPosition, 0.8f);
                }

                if (id.TryGetFeatureValue(CommonUsages.deviceVelocity, out var velocity))
                {
                    VelocityNotifier.Value = XRRigid.transform.TransformVector(velocity);
                }

                if (id.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out var angularAcceleration))
                {
                    AngularVelocityNotifier.Value = Controller.transform.InverseTransformVector(angularAcceleration);
                }
            }

            void localFunc_3(in InputDevice id)
            {
                Position.Value = Controller.transform.position;
                Rotation.Value = Controller.transform.rotation;

                ControllerRayOrigin.Value = interactor.attachTransform.position;
                ControllerRayTarget.Value = interactor.attachTransform.position + interactor.attachTransform.forward;

            }
        }
    }
}