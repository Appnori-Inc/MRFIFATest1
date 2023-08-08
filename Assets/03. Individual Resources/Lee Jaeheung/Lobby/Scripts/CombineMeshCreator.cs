#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMeshCreator : MonoBehaviour
{

    public void Create()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.active = false;
            i++;
        }
        MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.sharedMesh.CombineMeshes(combine);
        Unwrapping.GenerateSecondaryUVSet(meshFilter.sharedMesh);
        gameObject.SetActive(true);

        AssetDatabase.CreateAsset(meshFilter.sharedMesh, "Assets/CombinedMeshes/" + meshFilters[0].name +  meshFilter.sharedMesh.GetInstanceID().ToString() + ".asset");
    }
}
#endif