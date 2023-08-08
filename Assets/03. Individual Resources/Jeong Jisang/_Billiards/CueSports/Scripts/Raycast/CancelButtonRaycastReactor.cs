using UnityEngine;
using System.Collections;

namespace Billiards
{
    using System;
    using UnityEngine.UI;
    using UnityEngine.Events;

    [Obsolete]
    public class CancelButtonRaycastReactor : BaseRaycastReactor
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent { }

        [SerializeField]
        protected ButtonClickedEvent onClick;

        [SerializeField]
        private Transform Root;

        [SerializeField]
        private Transform FirstPivotRootA;
        [SerializeField]
        private Transform SecondPivotRootA;

        [SerializeField]
        private Transform FirstPivotRootB;
        [SerializeField]
        private Transform SecondPivotRootB;

        [SerializeField]
        private AnimationCurve FirstCurve;
        [SerializeField]
        private AnimationCurve SecondCurve;

        [SerializeField]
        private float runtime;


        private Coroutine RayRoutine = null;
        public override void OnRay(RaycastHit hitInfo)
        {
            if (RayRoutine != null || ReactRoutine != null || RayEndRoutine != null)
            {
                //StopCoroutine(RayRoutine);
                return;
            }

            var defaultScale = Root.transform.localScale;
            RayRoutine = StartCoroutine(Easy(0.2f, (t) =>
            {
                Root.transform.localScale = Vector3.Lerp(defaultScale, Vector3.one * 1.2f, t);
            }, onComplete: () => RayRoutine = null));
        }

        private Coroutine RayEndRoutine = null;
        public override void OnRayEnd(RaycastHit hitInfo)
        {
            if (RayEndRoutine != null)
            {
                //StopCoroutine(RayEndRoutine);
                return;
            }

            if(RayRoutine != null)
            {
                StopCoroutine(RayRoutine);
                RayRoutine = null;
            }

            var defaultScale = Root.transform.localScale;
            RayEndRoutine = StartCoroutine(Easy(0.2f, (t) =>
            {
                Root.transform.localScale = Vector3.Lerp(defaultScale, Vector3.one, t);
            }, onComplete: () => RayEndRoutine = null));
        }

        private Coroutine ReactRoutine = null;
        public override bool RaycastReact(RaycastHit hitInfo)
        {
            if (ReactRoutine != null)
            {
                return false;
            }

            if (RayRoutine != null)
            {
                StopCoroutine(RayRoutine);
                RayRoutine = null;
            }

            var defaultScale = Root.transform.localScale;
            SecondPivotRootA.localScale = Vector3.one.ToXZ().ToVector3FromXZ();
            SecondPivotRootB.localScale = Vector3.one.ToXZ().ToVector3FromXZ();

            ReactRoutine = StartCoroutine(Easy(runtime, onUpdate: (t) =>
            {
                if (t < 0.5f)
                {
                    var cta = Mathf.Clamp(t - 0.1f, 0, 0.4f) * 2.5f;
                    var ctb = Mathf.Clamp(t + 0.1f, 0, 0.6f) * 1.67f;
                    FirstPivotRootA.localScale = FirstPivotRootA.localScale.ToXZ().ToVector3FromXZ(Mathf.Lerp(1, 0, FirstCurve.Evaluate(cta)));
                    FirstPivotRootB.localScale = FirstPivotRootB.localScale.ToXZ().ToVector3FromXZ(Mathf.Lerp(1, 0, FirstCurve.Evaluate(ctb)));
                }
                else
                {
                    var ct = (t - 0.5f) * 2;
                    var cta = Mathf.Clamp(t - 0.6f, 0f, 0.4f) * 2.5f;
                    var ctb = Mathf.Clamp(t - 0.5f, 0f, 0.5f) * 2.0f;
                    SecondPivotRootA.localScale = SecondPivotRootA.localScale.ToXZ().ToVector3FromXZ(Mathf.Lerp(0, 1, SecondCurve.Evaluate(cta)));
                    SecondPivotRootB.localScale = SecondPivotRootB.localScale.ToXZ().ToVector3FromXZ(Mathf.Lerp(0, 1, SecondCurve.Evaluate(ctb)));
                }
            }, onComplete: () =>
            {
                ReactRoutine = null;
                onClick.Invoke();
            }));

            return true;
        }

    }
}