using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OptiTest : MonoBehaviour
{

    public GameObject OB_CUBE_Each;
    public GameObject OB_CUBE_Con;
    float count = -5f;
    int time = -700;

    public XRController LeftControlle;
    public XRController RightController;

    Appnori.Util.Notifier<bool> OnTriggerL = new Appnori.Util.Notifier<bool>();
    Appnori.Util.Notifier<bool> OnTriggerR = new Appnori.Util.Notifier<bool>();
    // Start is called before the first frame update
    void Start()
    {
        OnTriggerL.OnDataChanged += OnTriggerL_OnDataChanged;
        OnTriggerR.OnDataChanged += OnTriggerR_OnDataChanged;
    }


    // Update is called once per frame
    void Update()
    {
        //time += 1;

        //if(time % 2 == 0)
        //{
        //    Instantiate<GameObject>(OB_CUBE, new Vector3(time / 100f, count, 10), Quaternion.identity);
        //}
        //if(time % 3000 == 0)
        //{
        //    time = -700;
        //    count += 0.5f;
        //}

        if (RightController != null || RightController.inputDevice != null)
        {
            if (RightController.inputDevice.IsPressed(InputHelpers.Button.Trigger, out var isPressedR, 0.5f))
            {
                OnTriggerR.Value = isPressedR;
            }
        }

        if (LeftControlle != null || LeftControlle.inputDevice != null)
        {
            if (LeftControlle.inputDevice.IsPressed(InputHelpers.Button.Trigger, out var isPressedL, 0.5f))
            {
                OnTriggerL.Value = isPressedL;
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Instantiate<GameObject>(OB_CUBE_Each, new Vector3(0, 0, 10), Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Instantiate<GameObject>(OB_CUBE_Con, new Vector3(0, 0, 10), Quaternion.identity);
        }
    }

    private void OnTriggerL_OnDataChanged(bool obj)
    {
        Instantiate<GameObject>(OB_CUBE_Each, new Vector3(0, 0, 10), Quaternion.identity);
    }

    private void OnTriggerR_OnDataChanged(bool obj)
    {
        Instantiate<GameObject>(OB_CUBE_Con, new Vector3(0, 0, 10), Quaternion.identity);
    }


}