using UnityEngine;
using System.Collections;
using Appnori.Util;
using System.Collections.Generic;

namespace Billiards
{

    public class NotifierTest : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> list;

        private Notifier<GameObject> target = new Notifier<GameObject>();


        // Use this for initialization
        void Start()
        {
            target.OnDataChanged += (gameObject) => Debug.Log("[OnDataChanged]object is " + gameObject.name);
            target.OnDataChangedOnce += (gameObject) => Debug.Log("[OnDataChangedOnce]object is " + gameObject.name);
        }


        int idx = 0;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                target.Value = list[(++idx) % list.Count];
            }
        }
    }

}