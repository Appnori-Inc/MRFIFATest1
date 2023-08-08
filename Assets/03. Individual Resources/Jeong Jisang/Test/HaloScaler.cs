using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR

[ExecuteInEditMode]
public class HaloScaler : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curve;

    private void Update()
    {
        if (transform.childCount <= 0)
        {
            enabled = false;
            return;
        }

        for (int i = 0; i < transform.childCount; ++i)
        {
            var target = transform.GetChild(i);
            target.localScale = Vector3.one + Vector3.one * curve.Evaluate((float)i / transform.childCount);
        }

        enabled = false;
    }
}

#else

public class HaloScaler : MonoBehaviour
{
}
#endif
