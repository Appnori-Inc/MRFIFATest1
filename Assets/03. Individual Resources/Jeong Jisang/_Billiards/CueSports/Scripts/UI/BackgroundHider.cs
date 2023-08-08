using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Billiards
{

    public class BackgroundHider : MonoBehaviour
    {
        [SerializeField]
        private Material target;

        [SerializeField]
        private List<Renderer> renderers = new List<Renderer>();

        private WaitForSecondsRealtime waitUnscaledDelta;
        private Coroutine RunRoutine;
        private bool Active = false;

        public float CutoffValue
        {
            get => BilliardsDataContainer.Instance.TableBackgroundHiderCutoffValue.Value;
            private set => BilliardsDataContainer.Instance.TableBackgroundHiderCutoffValue.Value = value;
        }

        private void Awake()
        {
            BilliardsDataContainer.Instance.TableBackgroundHider.Value = this;

            using (var e = renderers.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    e.Current.sharedMaterial = target;
                }
            }

            target.SetFloat("_Cutoff", 1);
            CutoffValue = 1;
            waitUnscaledDelta = new WaitForSecondsRealtime(Time.fixedUnscaledDeltaTime);
        }

        public void HideBlack()
        {
            if (target.GetFloat("_Cutoff") == 1)
            {
                return;
            }

            if (RunRoutine != null)
            {
                StopCoroutine(RunRoutine);
                RunRoutine = null;
            }

            RunRoutine = StartCoroutine(Run(0.1f, 1));
        }

        public void ShowBlack()
        {
            if (target.GetFloat("_Cutoff") == 0)
                return;

            if (RunRoutine != null)
            {
                StopCoroutine(RunRoutine);
                RunRoutine = null;
            }

            RunRoutine = StartCoroutine(Run(0.1f, 0));
        }

        IEnumerator Run(float time, float to)
        {
            float t = 0;
            var current = target.GetFloat("_Cutoff");

            while (t < time)
            {
                CutoffValue = Mathf.Lerp(current, to, t / time);
                target.SetFloat("_Cutoff", CutoffValue);
                t += Time.fixedUnscaledDeltaTime;
                yield return waitUnscaledDelta;
            }

            CutoffValue = to;
            target.SetFloat("_Cutoff", CutoffValue);

            RunRoutine = null;

            Active = to == 0 ? true : false;
        }
    }
}