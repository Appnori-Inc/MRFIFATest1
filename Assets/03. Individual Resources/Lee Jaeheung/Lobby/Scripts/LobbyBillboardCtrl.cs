using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyBillboardCtrl : MonoBehaviour
{
    public Transform camTr;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 lookDir = transform.position - camTr.position;
        lookDir.y = 0f;

        transform.rotation = Quaternion.LookRotation(lookDir.normalized);
    }
}
