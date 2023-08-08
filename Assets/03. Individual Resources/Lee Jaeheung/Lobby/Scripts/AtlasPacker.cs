using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AtlasPacker))]
public class Edit_AtlasPacker : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AtlasPacker atlasPacker = (AtlasPacker)target;
        if (GUILayout.Button("Create"))
        {
            atlasPacker.Create();
        }
        if (GUILayout.Button("Remove"))
        {
            atlasPacker.RemoveObjects();
        }

        EditorGUILayout.HelpBox("버튼 누르기 전\n이미지 설정에 읽고쓰기 옵션 켜는것 확인\n무압축 확인", MessageType.Info);
    }
}
#endif

// "버튼 누르기 전\n이미지 설정에 읽고쓰기 옵션 켜는것 확인\n무압축 확인"
public class AtlasPacker : MonoBehaviour
{
    public string savePath = "Assets/03. Individual Resources/Lee Jaeheung/Atlas/Lobby";
    public bool isNormalMap = false;
    public bool isGetChilds = false;
#if UNITY_EDITOR
    public Texture2D[] atlasTextures;
    public Texture2D[] atlasNormals;
    public Mesh[] meshes;
#endif
    public int texture_size = 8192;
    public int texture_padding = 0;

    public void Create()
    {
#if UNITY_EDITOR

        if (isGetChilds)
        {
            atlasTextures = new Texture2D[transform.childCount];
            meshes = new Mesh[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform childTr = transform.GetChild(i);
                atlasTextures[i] = (Texture2D)childTr.GetComponent<MeshRenderer>().material.GetTexture("_BaseMap");
                meshes[i] = childTr.GetComponent<MeshFilter>().sharedMesh;
            }
            return;
        }

        Texture2D atlas_tex = new Texture2D(texture_size, texture_size);
        Rect[] rects_tex = atlas_tex.PackTextures(atlasTextures, texture_padding, texture_size, false);

        File.WriteAllBytes(savePath + ".png", atlas_tex.EncodeToPNG());

        if (isNormalMap)
        {
            Texture2D atlas_normal = new Texture2D(texture_size, texture_size);

            Rect[] rects_normal = atlas_normal.PackTextures(atlasNormals, texture_padding, texture_size, false);

            File.WriteAllBytes(savePath + "_N.png", atlas_normal.EncodeToPNG());

            UnityEditor.TextureImporter textureImporter = UnityEditor.AssetImporter.GetAtPath(savePath + "_N.png") as UnityEditor.TextureImporter;
            textureImporter.textureType = UnityEditor.TextureImporterType.NormalMap;
            textureImporter.SaveAndReimport();
        }

        if (meshes.Length > 0)
        {
            for (int i = 0; i < rects_tex.Length; i++)
            {
                CreateMesh(atlasTextures[i].name, meshes[i], rects_tex[i]);
            }

            UnityEditor.Formats.Fbx.Exporter.ModelExporter.ExportObject(savePath, gameObject);
        }
#endif
    }

    void CreateMesh(string setName, Mesh mesh_orizin, Rect rect)
    {
        Mesh mesh = mesh_orizin;
        GameObject go = new GameObject(setName, typeof(MeshFilter), typeof(MeshRenderer));
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        MeshFilter newMeshFilter = go.GetComponent<MeshFilter>();
        int[] triangles = mesh.GetTriangles(0);
        newMeshFilter.mesh = new Mesh();

        List<int> vertexIndices = new List<int>();
        List<Vector3> verts = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector4> tangents = new List<Vector4>();

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
                tangents.Add(mesh.tangents[curVertIndex]);
                if (mesh.uv != null && mesh.uv.Length > curVertIndex)
                {
                    uvs.Add(new Vector2(mesh.uv[curVertIndex].x * rect.width, mesh.uv[curVertIndex].y * rect.height) + rect.position);
                }
                if (mesh.uv2 != null && mesh.uv2.Length > curVertIndex)
                {
                    uvs2.Add(new Vector2(mesh.uv2[curVertIndex].x * rect.width, mesh.uv2[curVertIndex].y * rect.height) + rect.position);
                }
                if (mesh.uv3 != null && mesh.uv3.Length > curVertIndex)
                {
                    uvs3.Add(new Vector2(mesh.uv3[curVertIndex].x * rect.width, mesh.uv3[curVertIndex].y * rect.height) + rect.position);
                }
                if (mesh.uv4 != null && mesh.uv4.Length > curVertIndex)
                {
                    uvs4.Add(new Vector2(mesh.uv4[curVertIndex].x * rect.width, mesh.uv4[curVertIndex].y * rect.height) + rect.position);
                }

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
        newMeshFilter.sharedMesh.normals = normals.ToArray();
        newMeshFilter.sharedMesh.tangents = tangents.ToArray();

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
    }

    public void RemoveObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}

