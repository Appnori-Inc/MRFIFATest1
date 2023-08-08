using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Billiards
{

    [ExecuteInEditMode]
    public class MarkerGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject Origin;

        [SerializeField]
        private Transform root;

        [SerializeField]
        private float distance;

        [SerializeField]
        private float interval;

        private void OnEnable()
        {
            for (float x = -distance * 0.5f; x < distance * 0.5f; x += interval)
            {
                for (float y = -distance * 0.5f; y < distance * 0.5f; y += interval)
                {
                    var instance = Instantiate<GameObject>(Origin);
                    instance.transform.parent = root;
                    instance.transform.localPosition = new Vector3(x, 0, y);
                    instance.transform.localRotation = Quaternion.identity;
                }
            }
        }
        private void OnDisable()
        {
            var list = new List<GameObject>();
            for (int i = 0; i < root.childCount; ++i)
            {
                var obj = root.GetChild(i).gameObject;
                if (obj.GetInstanceID() == Origin.GetInstanceID())
                    continue;

                list.Add(obj);

            }

            using (var e = list.ToList().GetEnumerator())
            {
                while(e.MoveNext())
                {
                    DestroyImmediate(e.Current);
                }
            }

        }
    }

}