using UnityEngine;
using System.Collections;

namespace Billiards
{
    using System;
    public interface IBaseRaycastReactor
    {
        void OnRay(RaycastHit hitInfo);

        void OnRayEnd(RaycastHit hitInfo);

        bool RaycastReact(RaycastHit hitInfo);
    }

    [Obsolete("Legacy. Make Interactable instead")]
    public abstract class BaseRaycastReactor : MonoBehaviour, IBaseRaycastReactor
    {
        public abstract void OnRay(RaycastHit hitInfo);

        public abstract void OnRayEnd(RaycastHit hitInfo);

        public abstract bool RaycastReact(RaycastHit hitInfo);


        protected IEnumerator Easy(float runtime, Action<float> onUpdate = null, Action onComplete = null)
        {
            var waitRealTime = new WaitForSecondsRealtime(Time.fixedUnscaledDeltaTime);
            float t = 0;
            while (t < runtime)
            {
                onUpdate?.Invoke(t / runtime);
                t += Time.fixedUnscaledDeltaTime;
                yield return waitRealTime;
            }

            onUpdate?.Invoke(1);
            onComplete?.Invoke();
        }
    }
}
