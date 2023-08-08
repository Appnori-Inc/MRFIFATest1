#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SubMeshesExtractor : MonoBehaviour
{
    public void Extract()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        int i = 0;
        while (i < meshFilters.Length)
        {
            for (int j = 0; j < meshFilters[i].sharedMesh.subMeshCount; j++)
            {
                CreateMesh(meshFilters[i], j);
            }
            meshFilters[i].GetComponent<MeshRenderer>().enabled = false;
            i++;
        }
    }

    void CreateMesh(MeshFilter meshFilter_orizin, int index)
    {
        Mesh mesh = meshFilter_orizin.sharedMesh;
        GameObject go = new GameObject(mesh.name + "_SubMesh_" + index, typeof(MeshFilter), typeof(MeshRenderer));
        go.transform.parent = meshFilter_orizin.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        MeshFilter newMeshFilter = go.GetComponent<MeshFilter>();
        int[] triangles = mesh.GetTriangles(index);
        newMeshFilter.mesh = new Mesh();

        List<int> vertexIndices = new List<int>();
        List<Vector3> verts = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector3> normals = new List<Vector3>();

        List<Vector2> uvs, uvs2, uvs3, uvs4;
        uvs = new List<Vector2>(); uvs2 = new List<Vector2>(); uvs3 = new List<Vector2>(); uvs4 = new List<Vector2>();
        List<int> tris = new List<int>();

        newMeshFilter.sharedMesh.Clear();
        int curVertIndex = 0;
        int newVertIndex;
        int curSubVertIndex = 0;
        for (int i = 0; i < triangles.Length; i++)
        {
            curVertIndex = triangles[i];

            if (!vertexIndices.Contains(curVertIndex))
            {
                newVertIndex = curSubVertIndex;
                vertexIndices.Add(curVertIndex);

                verts.Add(mesh.vertices[curVertIndex]);

                if (mesh.colors != null && mesh.colors.Length > curVertIndex)
                    colors.Add(mesh.colors[curVertIndex]);

                normals.Add(mesh.normals[curVertIndex]);

                if (mesh.uv != null && mesh.uv.Length > curVertIndex)
                    uvs.Add(mesh.uv[curVertIndex]);
                if (mesh.uv2 != null && mesh.uv2.Length > curVertIndex)
                    uvs2.Add(mesh.uv2[curVertIndex]);
                if (mesh.uv3 != null && mesh.uv3.Length > curVertIndex)
                    uvs3.Add(mesh.uv3[curVertIndex]);
                if (mesh.uv4 != null && mesh.uv4.Length > curVertIndex)
                    uvs4.Add(mesh.uv4[curVertIndex]);

                curSubVertIndex++;
            }
            else
            {
                newVertIndex = vertexIndices.IndexOf(curVertIndex);
            }

            tris.Add(newVertIndex);
        }

        newMeshFilter.sharedMesh.vertices = verts.ToArray();
        newMeshFilter.sharedMesh.triangles = tris.ToArray();
        if (uvs.Count > 0)
            newMeshFilter.sharedMesh.uv = uvs.ToArray();
        if (uvs2.Count > 0)
            newMeshFilter.sharedMesh.uv2 = uvs2.ToArray();
        if (uvs3.Count > 0)
            newMeshFilter.sharedMesh.uv3 = uvs3.ToArray();
        if (uvs4.Count > 0)
            newMeshFilter.sharedMesh.uv4 = uvs4.ToArray();
        if (colors.Count > 0)
            newMeshFilter.sharedMesh.colors = colors.ToArray();

        newMeshFilter.sharedMesh.Optimize();
        newMeshFilter.sharedMesh.RecalculateBounds();
        newMeshFilter.sharedMesh.RecalculateNormals();
        AssetDatabase.CreateAsset(newMeshFilter.sharedMesh, "Assets/03. Individual Resources/Lee Jaeheung/ExtractMeshes/" + meshFilter_orizin.name + meshFilter_orizin.sharedMesh.GetInstanceID().ToString() + "_" + index + ".asset");
        //return newMesh;
        //AssetDatabase.CreateAsset(newMesh, "Assets/"+mesh.name+"_submesh["+index+"].asset");
        go.GetComponent<MeshRenderer>().sharedMaterial = meshFilter_orizin.GetComponent<MeshRenderer>().sharedMaterials[index];
    }
}
#endif