using Appnori.XR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Billiards
{
    [Serializable]
    public class TutorialPair
    {
        public Transform TextAnchor;
        public List<Transform> ButtonAnchors;
        public List<LineRenderer> LineRenderers;
        public Text Desc;

        public void UpdateLineAlpha(float alpha)
        {
            using (var e = LineRenderers.GetEnumerator())
                while (e.MoveNext())
                {
                    var color = e.Current.startColor;
                    color.a = alpha;
                    e.Current.startColor = color;

                    color = e.Current.endColor;
                    color.a = alpha;
                    e.Current.endColor = color;
                }
        }

        public void UpdatePosition()
        {
            if (ButtonAnchors.Count != LineRenderers.Count)
            {
                Debug.LogError("Count not matched");
                return;
            }

            for (int i = 0; i < ButtonAnchors.Count; ++i)
            {
                LineRenderers[i].positionCount = 2;
                LineRenderers[i].SetPosition(0, TextAnchor.position);
                LineRenderers[i].SetPosition(1, ButtonAnchors[i].position);
            }
        }
    }


    public class UIControllerTutorial : MonoBehaviour
    {
        [SerializeField]
        private bool isLeft;

        [SerializeField]
        private bool isDummy;

        [Space]
        [SerializeField]
        private TutorialPair TriggerPair;

        [Space]
        [SerializeField]
        private TutorialPair TrackPadPair;

        [Space]
        [SerializeField]
        private TutorialPair GripPair;

        [Space]
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private float OpenDelay = 5f;

        [SerializeField]
        private float OpenTime = 1f;

        private float uptime = 0;

        private float lastUpdateTime = 0;
        private float LastAlpha;

        private XRControllerState currentController
        {
            get
            {
                if (isLeft)
                    return BilliardsDataContainer.Instance.XRLeftControllerState;
                else
                    return BilliardsDataContainer.Instance.XRRightControllerState;
            }
        }


        private void Awake()
        {
            if(!isDummy)
            {
                currentController.AngularVelocityNotifier.OnDataChanged += OnControllerMoved;
                currentController.VelocityNotifier.OnDataChanged += OnControllerMoved;
                BilliardsDataContainer.Instance.GameState.OnDataChanged += GameState_OnDataChanged;
                currentController[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Grip].OnDataChanged += UIControllerTutorial_OnDataChanged;
                currentController[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger].OnDataChanged += UIControllerTutorial_OnDataChanged;
                currentController[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Primary2DAxisClick].OnDataChanged += UIControllerTutorial_OnDataChanged;
            }
            if (isLeft)
            {
                TrackPadPair.Desc.text = "Point and click on the table to <color=#F1964D>move to the end</color>";
            }
            else
            {
                TrackPadPair.Desc.text = "Click left and right to <color=#F1964D>rotate by ball</color>";
            }
        }

        private void UIControllerTutorial_OnDataChanged(bool obj)
        {
            lastUpdateTime = Time.time;
        }

        private void GameState_OnDataChanged(BallPool.ShotController.GameStateType obj)
        {
            lastUpdateTime = Time.time;
        }

        private void OnControllerMoved(Vector3 obj)
        {
            if (obj.magnitude > 0.5f)
                lastUpdateTime = Time.time;
        }

        private void Update()
        {
            if (isDummy)
            {
                TriggerPair.UpdatePosition();
                TrackPadPair.UpdatePosition();
                GripPair.UpdatePosition();
                SetAlpha(1);
                return;
            }

            if (BilliardsDataContainer.Instance.GameState.Value == BallPool.ShotController.GameStateType.CameraFixAndWaitShot)
            {
                SetAlpha(0);
                return;
            }


            var time = Time.time - lastUpdateTime;
            var alpha = 0f;

            if (time > OpenDelay)
                alpha = Mathf.Clamp01((time - OpenDelay) / OpenTime);
            else
                alpha = LastAlpha - Mathf.Clamp01(time / OpenTime);

            SetAlpha(alpha);

            if (alpha > 0)
            {
                uptime += Time.deltaTime;

                TriggerPair.UpdatePosition();
                TrackPadPair.UpdatePosition();
                GripPair.UpdatePosition();
            }
            else
            {
                if (uptime > GameConfig.TutorialExposeTime)
                {
                    enabled = false;
                }
            }

            LastAlpha = alpha;
        }

        private void SetAlpha(float alpha)
        {
            canvasGroup.alpha = alpha;
            TriggerPair.UpdateLineAlpha(alpha);
            TrackPadPair.UpdateLineAlpha(alpha);
            GripPair.UpdateLineAlpha(alpha);
        }

        private void OnDestroy()
        {
            if (!isDummy)
            {
                currentController[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Primary2DAxisClick].OnDataChanged -= UIControllerTutorial_OnDataChanged;
                currentController[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger].OnDataChanged -= UIControllerTutorial_OnDataChanged;
                currentController[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Grip].OnDataChanged -= UIControllerTutorial_OnDataChanged;
                BilliardsDataContainer.Instance.GameState.OnDataChanged -= GameState_OnDataChanged;
                currentController.VelocityNotifier.OnDataChanged -= OnControllerMoved;
                currentController.AngularVelocityNotifier.OnDataChanged -= OnControllerMoved;
            }
        }


    }

}