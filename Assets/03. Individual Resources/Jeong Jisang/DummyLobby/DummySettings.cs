using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DummySettings : MonoBehaviour
{

    public static float DragValue = 0.05f;
    public static float ForceValue = 1f;

    [SerializeField]
    private Slider Drag;
    [SerializeField]
    private Text currentDrag;

    [SerializeField]
    private Slider Force;
    [SerializeField]
    private Text currentForce;


    private void Update()
    {
        currentDrag.text = "drag : " + Drag.value;
        currentForce.text = "Force : " + Force.value;

        DragValue = Drag.value;
        ForceValue = Force.value;
    }

}
