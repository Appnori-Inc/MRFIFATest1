using Appnori.XR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Billiards
{

    public class PlayerHandTracker : MonoBehaviour
    {
        public enum Type
        {
            Left,
            Right
        }

        [SerializeField]
        private Type HandType;

        [SerializeField]
        private Animator animator;

        private XRControllerState currentControllerState => HandType == Type.Left ?
            BilliardsDataContainer.Instance.XRLeftControllerState :
            BilliardsDataContainer.Instance.XRRightControllerState;


        private void Awake()
        {
            currentControllerState.Position.OnDataChanged += Position_OnDataChanged;
            currentControllerState.Rotation.OnDataChanged += Rotation_OnDataChanged;
            currentControllerState.TriggerNotifier.OnDataChanged += PlayerHandTracker_OnDataChanged;
        }

        private void PlayerHandTracker_OnDataChanged(float triggerValue)
        {
            animator.SetFloat("Value", triggerValue);
        }

        private void Rotation_OnDataChanged(Quaternion obj)
        {
            transform.rotation = obj;
        }

        private void Position_OnDataChanged(Vector3 obj)
        {
            transform.position = obj;
        }
        public void SetHandRotaion(Quaternion obj)
        {
            transform.rotation = obj;
        }

        private void OnDestroy()
        {
            currentControllerState.Position.OnDataChanged -= Position_OnDataChanged;
            currentControllerState.Rotation.OnDataChanged -= Rotation_OnDataChanged;
            currentControllerState.TriggerNotifier.OnDataChanged -= PlayerHandTracker_OnDataChanged;
        }
    }

}