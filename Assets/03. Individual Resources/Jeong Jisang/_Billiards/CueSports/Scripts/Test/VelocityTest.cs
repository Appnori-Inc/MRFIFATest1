using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Billiards
{

    public class VelocityTest : MonoBehaviour
    {
        [SerializeField]
        private Transform pivot;

        [SerializeField]
        private Transform slider;

        private float cueSliderDisplacementZ;

        private bool allow = false;

        private void Start()
        {
            BilliardsDataContainer.Instance.XRRightControllerState[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger].OnDataChanged += VelocityTest_OnDataChanged;
            BilliardsDataContainer.Instance.XRRightControllerState[UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button.Trigger].OnDataChangedOnce += VelocityTest_OnDataChangedOnce;
        }

        private void VelocityTest_OnDataChanged(bool obj)
        {
            Debug.Log("always : VelocityTest_OnDataChanged");
            allow = obj;
            if (!obj)
            {
                cueSliderDisplacementZ = 0;
            }
        }

        private void VelocityTest_OnDataChangedOnce(bool obj)
        {
            Debug.Log("CallOnce : VelocityTest_OnDataChangedOnce");
        }

        private void Update()
        {
            //rot
            var playerPos = BilliardsDataContainer.Instance.MainCamera.Value.transform.position.ToXZ();

            var dir = (pivot.position.ToXZ() - playerPos).normalized;

            pivot.rotation = Quaternion.LookRotation(dir.ToVector3FromXZ(), Vector3.up);

            Debug.Log("Allow : " + allow);
            if (!allow)
                return;

            //pos
            var controllerVelocity = BilliardsDataContainer.Instance.XRRightControllerState.VelocityNotifier.Value;
            var projectedVelocity = Vector3.Project(controllerVelocity, slider.transform.forward);
            //var projectedVelocity_up = Vector3.Project(controllerVelocity, pivot.transform.up);
            //var projectedVelocity_right = Vector3.Project(controllerVelocity, pivot.transform.right);
            var localVelocity = slider.InverseTransformVector(projectedVelocity);

            cueSliderDisplacementZ += localVelocity.z * Time.deltaTime;
            Debug.Log("cueSliderDisplacementZ : " + cueSliderDisplacementZ);

            cueSliderDisplacementZ = Mathf.Clamp(cueSliderDisplacementZ, -1, 0);
            //slider.localPosition = new Vector3(0,0, cueSliderDisplacementZ);
            slider.position += projectedVelocity * Time.deltaTime;
            //slider.localEulerAngles += new Vector3(projectedVelocity_up.y, projectedVelocity_right.x, 0);

        }
    }

}