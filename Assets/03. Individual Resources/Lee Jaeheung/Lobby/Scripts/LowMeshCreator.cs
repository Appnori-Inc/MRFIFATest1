using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Appnori.Util
{
    public class LowMeshCreator : MonoBehaviour
    {
        public float maxDelta_vert = 0.001f;
        public float maxDelta_uv = 0.001f;
        public bool isCheckSide = false;

        // Start is called before the first frame update
        void Start()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = GetWeldMesh(meshFilter.sharedMesh, isCheckSide, maxDelta_vert, maxDelta_uv);
            enabled = false;
        }

        public static Mesh GetWeldMesh(Mesh mesh, bool _isCheckSide = true, float _maxDelta_vert = 0.001f, float _maxDelta_uv = 0.001f)
        {
            int[] tris_origin = mesh.triangles;
            Vector3[] verts_origin = mesh.vertices;
            Vector3[] normals_origin = mesh.normals;
            Vector4[] tangents_origin = mesh.tangents;
            Vector2[] uvs_origin = mesh.uv;
            BoneWeight[] boneWeights_origin = mesh.boneWeights;

            /////////////////////

            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            bool[] isSideVerts = new bool[verts_origin.Length];

            if (_isCheckSide)
            {
                Dictionary<string, byte> dict_all_edge = new Dictionary<string, byte>();
                Dictionary<string, int[]> dict_side_edge = new Dictionary<string, int[]>();
                int[] edge = new int[2];

                for (int i = 0; i < tris_origin.Length; i += 3)
                {
                    edge = new int[] { tris_origin[i], tris_origin[i + 1] };
                    stringBuilder.Clear();

                    if (edge[0] < edge[1])
                    {
                        stringBuilder.Append(edge[0]);
                        stringBuilder.Append("_");
                        stringBuilder.Append(edge[1]);
                    }
                    else
                    {
                        stringBuilder.Append(edge[1]);
                        stringBuilder.Append("_");
                        stringBuilder.Append(edge[0]);
                    }

                    if (dict_all_edge.TryAdd(stringBuilder.ToString(), 0))
                    {
                        dict_side_edge.TryAdd(stringBuilder.ToString(), edge);
                    }
                    else
                    {
                        dict_side_edge.Remove(stringBuilder.ToString());
                    }

                    edge = new int[] { tris_origin[i + 1], tris_origin[i + 2] };
                    stringBuilder.Clear();

                    if (edge[0] < edge[1])
                    {
                        stringBuilder.Append(edge[0]);
                        stringBuilder.Append("_");
                        stringBuilder.Append(edge[1]);
                    }
                    else
                    {
                        stringBuilder.Append(edge[1]);
                        stringBuilder.Append("_");
                        stringBuilder.Append(edge[0]);
                    }

                    if (dict_all_edge.TryAdd(stringBuilder.ToString(), 0))
                    {
                        dict_side_edge.TryAdd(stringBuilder.ToString(), edge);
                    }
                    else
                    {
                        dict_side_edge.Remove(stringBuilder.ToString());
                    }

                    edge = new int[] { tris_origin[i + 2], tris_origin[i] };
                    stringBuilder.Clear();

                    if (edge[0] < edge[1])
                    {
                        stringBuilder.Append(edge[0]);
                        stringBuilder.Append("_");
                        stringBuilder.Append(edge[1]);
                    }
                    else
                    {
                        stringBuilder.Append(edge[1]);
                        stringBuilder.Append("_");
                        stringBuilder.Append(edge[0]);
                    }

                    if (dict_all_edge.TryAdd(stringBuilder.ToString(), 0))
                    {
                        dict_side_edge.TryAdd(stringBuilder.ToString(), edge);
                    }
                    else
                    {
                        dict_side_edge.Remove(stringBuilder.ToString());
                    }
                }

                foreach (var item in dict_side_edge)
                {
                    isSideVerts[item.Value[0]] = true;
                    isSideVerts[item.Value[1]] = true;
                }
            }

            List<int> newVerts = new List<int>();
            int[] map = new int[verts_origin.Length];

            for (int i = 0; i < verts_origin.Length; i++)
            {
                var p = verts_origin[i];
                var uv = uvs_origin[i];
                bool duplicate = false;
                if (!isSideVerts[i])
                {
                    for (int i2 = 0; i2 < newVerts.Count; i2++)
                    {
                        int a = newVerts[i2];
                        if (
                            (verts_origin[a] - p).sqrMagnitude <= _maxDelta_vert
                            && (uvs_origin[a] - uv).sqrMagnitude <= _maxDelta_uv
                            )
                        {
                            map[i] = i2;
                            duplicate = true;
                            break;
                        }
                    }
                }

                if (!duplicate)
                {
                    map[i] = newVerts.Count;
                    newVerts.Add(i);
                }
            }

            Vector3[] verts_set = new Vector3[newVerts.Count];
            Vector3[] normals_set = new Vector3[newVerts.Count];
            Vector4[] tangents_set = new Vector4[newVerts.Count];
            Vector2[] uvs_set = new Vector2[newVerts.Count];
            BoneWeight[] boneWeights_set = new BoneWeight[newVerts.Count];

            if (boneWeights_origin.Length > 0)
            {
                for (int i = 0; i < newVerts.Count; i++)
                {
                    int a = newVerts[i];
                    verts_set[i] = verts_origin[a];
                    normals_set[i] = normals_origin[a];
                    tangents_set[i] = tangents_origin[a];
                    uvs_set[i] = uvs_origin[a];
                    boneWeights_set[i] = boneWeights_origin[a];
                }
            }
            else if (tangents_origin.Length > 0)
            {
                for (int i = 0; i < newVerts.Count; i++)
                {
                    int a = newVerts[i];
                    verts_set[i] = verts_origin[a];
                    normals_set[i] = normals_origin[a];
                    tangents_set[i] = tangents_origin[a];
                    uvs_set[i] = uvs_origin[a];
                }
            }
            else
            {
                for (int i = 0; i < newVerts.Count; i++)
                {
                    int a = newVerts[i];
                    verts_set[i] = verts_origin[a];
                    normals_set[i] = normals_origin[a];
                    uvs_set[i] = uvs_origin[a];
                }
            }

            for (int i = 0; i < tris_origin.Length; i++)
            {
                tris_origin[i] = map[tris_origin[i]];
            }

            Dictionary<string, byte> dict_all_tris = new Dictionary<string, byte>();
            List<int> list_tris_set = new List<int>();
            for (int i = 0; i < tris_origin.Length; i += 3)
            {
                int[] findTris = new int[] { tris_origin[i], tris_origin[i + 1], tris_origin[i + 2] };
                if (findTris[0] != findTris[1] && findTris[0] != findTris[2] && findTris[1] != findTris[2])
                {
                    int[] nameTris = new int[] { tris_origin[i], tris_origin[i + 1], tris_origin[i + 2] };

                    System.Array.Sort(nameTris);
                    stringBuilder.Clear();
                    stringBuilder.Append(nameTris[0]);
                    stringBuilder.Append("_");
                    stringBuilder.Append(nameTris[1]);
                    stringBuilder.Append("_");
                    stringBuilder.Append(nameTris[2]);

                    if (dict_all_tris.TryAdd(stringBuilder.ToString(), 0))
                    {
                        list_tris_set.AddRange(findTris);
                    }
                }
            }

            Mesh weld_mesh = new Mesh();

            weld_mesh.vertices = verts_set;
            weld_mesh.normals = normals_set;
            weld_mesh.tangents = tangents_set;
            weld_mesh.uv = uvs_set;
            weld_mesh.boneWeights = boneWeights_set;
            weld_mesh.triangles = list_tris_set.ToArray();
            weld_mesh.Optimize();

            return weld_mesh;
        }
    }
}
