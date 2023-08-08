using UnityEngine;
using System.Collections;

namespace Billiards
{
    using System;

    public class NotifiableVariable<T> where T : struct
    {
        private T value;
        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                Set(value);
            }
        }

        public event Action<T> OnNext;
        public event Action<T> OnComplete;

        public NotifiableVariable()
        {
            value = default;
        }

        public NotifiableVariable(T value)
        {
            this.value = value;
        }

        public void Set(T value, bool withNoti = true)
        {
            if (withNoti)
                OnNext?.Invoke(value);

            this.value = value;

            if (withNoti)
                OnComplete?.Invoke(this.value);
        }
    }


}