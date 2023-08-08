using Appnori.XR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Billiards
{

    public class LockMarker : MonoBehaviour
    {
        [SerializeField]
        private bool isLeft = false;

        [SerializeField]
        private GameObject MarkerObject;

        [SerializeField]
        private Transform VerticalRoot;

        [SerializeField]
        private Transform HorizontalRoot;

        [SerializeField]
        private Transform simulationRoot;
        public Transform SimulationRoot { get => simulationRoot; }

        [SerializeField]
        private Vector3 RotateAmount = new Vector3(0, 1, 0);

        XRControllerState currentController { get => isLeft ? BilliardsDataContainer.Instance.XRLeftControllerState : BilliardsDataContainer.Instance.XRRightControllerState; }

        private float RotationXOffset;
        private float RotationYOffset;

        private bool isInitialized = false;

        private void Awake()
        {
            MarkerObject.SetActive(false);

            if (isLeft)
                BilliardsDataContainer.Instance.LeftLockMarker.Value = this;
            else
                BilliardsDataContainer.Instance.RightLockMarker.Value = this;

            currentController[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger].OnDataChanged += LockMarker_OnDataChanged;
        }

        private void LockMarker_OnDataChanged(bool isPressed)
        {
            MarkerObject.SetActive(isPressed);

            if (isPressed)
            {
                transform.position = currentController.Position.Value;
            }
            else
            {
                isInitialized = false;
            }
        }

        public void InitializeAxis(Vector3 targetPosition)
        {
            if(isInitialized)
                return;

            isInitialized = true;

            RotationXOffset = 0;
            RotationYOffset = 0;

            VerticalRoot.localRotation = Quaternion.identity;
            HorizontalRoot.localRotation = Quaternion.identity;

            var rotation = Quaternion.LookRotation((targetPosition.ToXZ() - transform.position.ToXZ()).ToVector3FromXZ(), Vector3.up);
            transform.rotation = rotation;
        }

        public void UpdateSimulationOffset(float xAmount, float yAmount)
        {
            //Debug.Log($"xAmount : {xAmount} , yAmount : {yAmount}");
            //Debug.Log($"RotationXOffset : {RotationXOffset} , RotationYOffset : {RotationYOffset}");
            RotationXOffset += xAmount;
            RotationYOffset += yAmount;

            VerticalRoot.localRotation = Quaternion.Euler(RotationXOffset, 0, 0);
            HorizontalRoot.localRotation = Quaternion.Euler(0.0f, RotationYOffset, 0.0f);
        }

        private void Update()
        {
            if (MarkerObject.activeInHierarchy)
            {
                MarkerObject.transform.Rotate(RotateAmount * Time.deltaTime, Space.World);
            }
        }

        private void OnDestroy()
        {
            currentController[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger].OnDataChanged -= LockMarker_OnDataChanged;
        }
    }

}