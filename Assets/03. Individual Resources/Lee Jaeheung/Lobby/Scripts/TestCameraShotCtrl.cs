using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestCameraShotCtrl : MonoBehaviour
{
    public Transform targetTr;

    public Vector3 sumPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string path = Application.dataPath + "/" + DateTime.Now.ToString("MMddyyHHmmss") + "_SS.png";
            ScreenCapture.CaptureScreenshot(path, 4);
            Debug.LogError(path);
        }

        if (targetTr != null)
        {
            transform.rotation = Quaternion.LookRotation((targetTr.position + sumPos - transform.position).normalized);
        }

    }
}
