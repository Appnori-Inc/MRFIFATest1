using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Billiards;
using Appnori.Util;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;

namespace Appnori.XR
{

    public class InputDeviceInfo : Singleton<InputDeviceInfo>
    {
        public bool isInitialized { get; private set; } = false;
        public bool Allow2DAxisWithoutClick { get; private set; }
        public Notifier<bool> Primary2DAxisInput { get; private set; } = new Notifier<bool>();


        public void Initialize()
        {
            isInitialized = true;
            var list = new List<string>();

            list.Add(SystemInfo.deviceName);
            list.Add(SystemInfo.deviceModel);

            Debug.Log($"Device info : SystemInfo.deviceName({SystemInfo.deviceName}),SystemInfo.deviceModel({SystemInfo.deviceModel})");
            var controller = GameObject.FindObjectOfType<XRController>();
            try
            {
                list.Add(controller.inputDevice.name);
                list.Add(controller.inputDevice.subsystem.SubsystemDescriptor.id);
                list.Add(controller.inputDevice.manufacturer);

                Debug.Log(
                    $"  controller.inputDevice.name({controller.inputDevice.name})" +
                    $"  controller.inputDevice.serialNumber({controller.inputDevice.serialNumber})" +
                    $"  controller.inputDevice.subsystem({controller.inputDevice.subsystem})" +
                    $"  controller.inputDevice.subsystem.SubsystemDescriptor.id({controller.inputDevice.subsystem.SubsystemDescriptor.id})" +
                    $"  controller.inputDevice.manufacturer({controller.inputDevice.manufacturer})");
            }
            catch (System.Exception e) { }

            var target = list.Where((s) => s != null && s != string.Empty);
            foreach (var str in target)
            {
                Allow2DAxisWithoutClick = Allow2DAxisWithoutClick || str.ToLower().Contains("oculus");
            }

        }


    }
}