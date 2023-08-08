using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyShadowCtrl : MonoBehaviour
{
    public Transform target;
    private float posY = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        posY = transform.localPosition.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = target.localPosition;
        pos.y = posY;
        transform.localPosition = pos;
    }
}
