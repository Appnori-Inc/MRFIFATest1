using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticTest : MonoBehaviour
{
    [SerializeField]
    private XRController mainController;
    [SerializeField]
    private XRController subController;

    void Update()
    {
        var mainDevice = mainController.inputDevice;
        var subDevice = subController.inputDevice;

        DeviceControl(mainDevice, mainController.axisToPressThreshold);
        DeviceControl(subDevice, subController.axisToPressThreshold);
    }


    private void DeviceControl(in UnityEngine.XR.InputDevice device, in float threshold)
    {
        if (device.IsPressed(InputHelpers.Button.Trigger, out var isTriggerPressed, threshold))
        {
            if(isTriggerPressed)
                TrySendHaptic(device);
        }
    }

    private bool TrySendHaptic(in UnityEngine.XR.InputDevice device)
    {
        if (!device.TryGetHapticCapabilities(out var capabilities))
            return false;

        if (!capabilities.supportsImpulse)
            return false;

        device.SendHapticImpulse(0, 1, 1);

        return true;
    }
}
