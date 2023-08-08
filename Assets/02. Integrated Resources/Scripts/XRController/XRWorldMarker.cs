using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Appnori.XR
{

    public class XRWorldMarker : MonoBehaviour
    {
        [SerializeField]
        private XRGripPosition position;

        [SerializeField]
        private XRGripRotation rotation;

        [SerializeField]
        private GameObject Root;

        private void Awake()
        {
            Root.SetActive(false);

            position.IsAllowed.OnDataChanged += IsAllowed_OnDataChanged;
            rotation.IsAllowed.OnDataChanged += IsAllowed_OnDataChanged;
        }

        private void IsAllowed_OnDataChanged(bool isOn)
        {
            Root.SetActive(isOn);
        }

        private void OnDestroy()
        {
            position.IsAllowed.OnDataChanged -= IsAllowed_OnDataChanged;
            rotation.IsAllowed.OnDataChanged -= IsAllowed_OnDataChanged;
        }
    }

}