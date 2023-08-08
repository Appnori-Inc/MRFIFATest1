using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Threading;

public class ThreadTest : MonoBehaviour
{
    List<Thread> threads = new List<Thread>();
    
    void Start()
    {
        var th1 = new Thread(()=> Play(1,2,3));
        th1.IsBackground = true;
        th1.Start();
    }

    void Play(int d1, int d2, int d3)
    {
        int sum = d1 + d2 + d3;

    }

    void Play2()
    {

    }
}
