using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHandCtrl : MonoBehaviour
{
    private bool isTrigger = false;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        if (!isTrigger)
        {
            anim.SetBool("IsPoint", false);
        }
        isTrigger = false;
    }

    private void OnTriggerStay(Collider other)
    {
        RaycastHit hit;
        //Debug.LogError("sdfsdf");
        anim.SetBool("IsPoint", true);
        isTrigger = true;
    }
}
