#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
public class Test_SelectCtrl : MonoBehaviour
{
    [MenuItem("Appnori/Select MeshObject")]
    public static void SelectObjects()
    {
        Selection.objects = GameObject.FindObjectsOfType<MeshRenderer>().Select(rb => rb.gameObject).ToArray();
    }
}
#endif
