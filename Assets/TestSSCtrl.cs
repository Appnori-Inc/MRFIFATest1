using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TestSSCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string path = Application.dataPath + "/" + DateTime.Now.ToString("MMddyyHHmmss") + "_SS.png";
            ScreenCapture.CaptureScreenshot(path, 8);
            Debug.LogError(path);
        }
    }
}
