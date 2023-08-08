using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy_CTRL : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayDestory_C(10));
    }

    IEnumerator DelayDestory_C(float time)
    {
        yield return YieldInstructionCache.WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
