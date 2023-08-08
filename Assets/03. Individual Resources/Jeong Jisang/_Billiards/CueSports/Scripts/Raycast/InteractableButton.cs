using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Billiards
{
    using BallPool;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using UnityEngine.XR.Interaction.Toolkit;
    using UnityEngine.EventSystems;

    public class InteractableButton : XRBaseInteractable, IPointerClickHandler
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent { }

        [SerializeField]
        protected Image TargetGraphic;
        protected Color defaultColor;

        [SerializeField]
        protected Color NormalColor = Color.white;

        [SerializeField]
        protected Color HighlightedColor;

        [SerializeField]
        protected Color PressedColor;

        [SerializeField]
        protected Color DisabledColor = Color.gray;

        [SerializeField]
        protected bool isMultipleColor = true;

        [Range(1f, 5f)]
        [SerializeField]
        protected float Multiplier = 1;

        [SerializeField]
        protected float FadeDuration = 0.1f;


        [SerializeField]
        protected ButtonClickedEvent onClick;

        private bool _interactable = true;
        public bool interactable
        {
            get => _interactable;
            set
            {

                if (_interactable == value)
                    return;

                _interactable = value;

                if (value)
                {
                    StopAllCoroutines();
                    targetColor = NormalColor;
                    TargetGraphic.color = NormalColor * defaultColor;
                }
                else
                {
                    targetColor = DisabledColor;
                    TargetGraphic.color = DisabledColor * defaultColor;
                }
            }
        }

#if UNITY_EDITOR
        public ButtonClickedEvent OnClick { get => onClick; }
#endif

        protected Color targetColor;

        protected override void Awake()
        {
            base.Awake();
            defaultColor = TargetGraphic.color;
        }

        protected virtual void OnDisable()
        {
            targetColor = NormalColor;
            TargetGraphic.color = NormalColor * defaultColor;
        }

        protected override void OnHoverEntered(XRBaseInteractor interactor)
        {
            base.OnHoverEntered(interactor);

            if (!interactable)
                return;

            if (targetColor != HighlightedColor)
            {
                targetColor = HighlightedColor;
                StartCoroutine(Fade());
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickButton();
        }

        protected override void OnSelectEntered(XRBaseInteractor interactor)
        {
            base.OnSelectEntered(interactor);
            OnClickButton();
        }

        private void OnClickButton()
        {
            if (!interactable)
                return;

            if (targetColor != PressedColor)
            {
                targetColor = PressedColor;
                StartCoroutine(Fade(() =>
                {
                    targetColor = NormalColor;
                    StartCoroutine(Fade());
                }));

                onClick.Invoke();
            }
        }

        protected override void OnHoverExited(XRBaseInteractor interactor)
        {
            base.OnHoverExited(interactor);

            if (!interactable)
                return;

            if (gameObject == null)
                return;

            if (targetColor != NormalColor)
            {
                targetColor = NormalColor;
                StartCoroutine(Fade());
            }
        }

        protected IEnumerator Fade(Action OnComplete = null)
        {
            float t = 0;
            var firstColor = TargetGraphic.color;
            var waitRealTime = new WaitForSecondsRealtime(Time.fixedUnscaledDeltaTime);

            while (t < FadeDuration)
            {
                TargetGraphic.color = Color.Lerp((firstColor * Multiplier) * (defaultColor / Multiplier), (targetColor * Multiplier) * (defaultColor / Multiplier), t / FadeDuration);

                t += Time.fixedUnscaledDeltaTime;
                yield return waitRealTime;
            }

            TargetGraphic.color = Color.Lerp((firstColor * Multiplier) * (defaultColor / Multiplier), (targetColor * Multiplier) * (defaultColor / Multiplier), 1);
            OnComplete?.Invoke();
        }

    }
}
