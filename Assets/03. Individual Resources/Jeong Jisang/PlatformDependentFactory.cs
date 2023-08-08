using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlatformDependentFactory : MonoBehaviour
{

    [Serializable]
    public class Pair
    {
        public bool enabled;
        public GameObject Origin;
    }

    [SerializeField]
    private List<Pair> Pairs;

    protected void Awake()
    {
        foreach (var pair in Pairs)
        {
            if (pair.enabled)
            {
                var instance = Instantiate(pair.Origin);
                instance.name = pair.Origin.name;
            }
        }
    }

}
