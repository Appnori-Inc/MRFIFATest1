using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Billiards
{
    public class CameraLook : MonoBehaviour
    {
        [SerializeField]
        private Camera MainCamera;

        [SerializeField]
        private float delayMovementRatio = 0;

        private void Awake()
        {
            MainCamera = BilliardsDataContainer.Instance.MainCamera.Value;

            BilliardsDataContainer.Instance.MainCamera.OnDataChanged += LeftEyeCamera_OnDataChanged;
        }

        private void OnDestroy()
        {
            BilliardsDataContainer.Instance.MainCamera.OnDataChanged -= LeftEyeCamera_OnDataChanged;
        }

        private void LeftEyeCamera_OnDataChanged(Camera obj)
        {
            MainCamera = obj;
        }

        void Update()
        {
            if ((object)MainCamera == null)
            {
                return;
            }

            var pos = (MainCamera.transform.position);
            pos.y = transform.position.y;
            var targetRotation = Quaternion.LookRotation(transform.position - pos, Vector3.up);

            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, 1 - delayMovementRatio);
        }
    }
}