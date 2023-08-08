using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Appnori.Util;

public class OnHandRay : MonoBehaviour
{
    protected Ray ray;
    protected RaycastHit rayHit;

    public Notifier<GameObject> HandRay { get; private set; }
    public PlayAgainMenu _playAgainMenu;
    private void Awake()
    {
        init();
    }


    private void Update()
    {
        ray = new Ray(transform.position, Vector3.forward);
        Debug.DrawRay(ray.origin, ray.direction * 5f, Color.red);
        if (Physics.Raycast(ray.origin, ray.direction, out rayHit, 10f))
        {


        }
    }

    private void init()
    {

        ray = new Ray(transform.position, Vector3.forward);
        if (Physics.Raycast(ray.origin, ray.direction, out rayHit, 10f))
        {


        }
    }
}
