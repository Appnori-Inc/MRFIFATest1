using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandAnimation : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    public int MaxRandValue;
    public float randTime;


    private float currentTime = 0;
    
    void Start()
    {
        anim = GetComponent<Animator>();                
    }

    // Update is called once per frame
    void Update()
    {
        RandAnim();
    }

    private void RandAnim()
    {
        currentTime += Time.deltaTime;

        if(currentTime >= randTime)
        {
            currentTime = 0;
            int rand = Random.Range(0, MaxRandValue);
            anim.SetInteger("rand", rand);
        }
    }
}
