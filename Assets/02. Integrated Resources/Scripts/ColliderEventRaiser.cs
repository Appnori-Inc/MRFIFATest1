using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Appnori.Util
{

    public class ColliderEventRaiser : MonoBehaviour
    {
        public event Action<Collider> OnTriggerEnterEvent;
        public event Action<Collider> OnTriggerStayEvent;
        public event Action<Collider> OnTriggerExitEvent;

        public event Action<Collision> OnCollisionEnterEvent;
        public event Action<Collision> OnCollisionStayEvent;
        public event Action<Collision> OnCollisionExitEvent;

        private void OnTriggerEnter(Collider other) => OnTriggerEnterEvent?.Invoke(other);
        private void OnTriggerStay(Collider other) => OnTriggerStayEvent?.Invoke(other);
        private void OnTriggerExit(Collider other) => OnTriggerExitEvent?.Invoke(other);

        private void OnCollisionEnter(Collision collision) => OnCollisionEnterEvent?.Invoke(collision);
        private void OnCollisionStay(Collision collision) => OnCollisionStayEvent?.Invoke(collision);
        private void OnCollisionExit(Collision collision) => OnCollisionExitEvent?.Invoke(collision);

    }

}