using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshWelder : MonoBehaviour
{
    public void Weld()
    {
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        mf.mesh = new Mesh();

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];

        int vertCount = 0;

        for (int i = 1; i < meshFilters.Length; ++i)
        {
            meshFilters[i].mesh.SetTriangles(meshFilters[i].mesh.triangles, 0);
            vertCount += meshFilters[i].mesh.vertexCount;
            combine[i - 1].mesh = meshFilters[i].sharedMesh;
            combine[i - 1].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        mf.mesh.CombineMeshes(combine);

        WeldVertices(mf.mesh);

        Debug.Log($"Uncombined Vert Count: {vertCount};  Combined Vert Count: {mf.mesh.vertexCount}");
    }

    public static void WeldVertices(Mesh mesh, float tolerance = 0.001f)
    {
        Vector3[] verts = mesh.vertices;
        Vector3[] normals = mesh.normals;
        List<int> newVertIndices = new List<int>();
        int[] map = new int[verts.Length];

        // Create mapping and filter duplicates.
        for (int i = 0; i < verts.Length; i++)
        {
            var p = verts[i];
            var n = normals[i];
            bool duplicate = false;
            for (int i2 = 0; i2 < newVertIndices.Count; i2++)
            {
                int a = newVertIndices[i2];
                if ((verts[a] - p).sqrMagnitude <= tolerance &&
                    Vector3.Angle(normals[a], n) <= tolerance)
                {
                    map[i] = i2;
                    duplicate = true;
                    break;
                }
            }
            if (!duplicate)
            {
                map[i] = newVertIndices.Count;
                newVertIndices.Add(i);
            }
        }

        Debug.Log($"Duplicate count: {verts.Length - newVertIndices.Count}");

        // Create new vertices
        Vector3[] verts2 = new Vector3[newVertIndices.Count];
        Vector3[] normals2 = new Vector3[newVertIndices.Count];
        for (int i = 0; i < newVertIndices.Count; i++)
        {
            int index = newVertIndices[i];
            verts2[i] = verts[index];
            normals2[i] = normals[index];
        }

        // Map the triangle to the new vertices
        var tris = mesh.triangles;
        for (int i = 0; i < tris.Length; i++)
        {
            tris[i] = map[tris[i]];
        }
        mesh.triangles = tris;
        mesh.vertices = verts2;
        mesh.normals = normals2;
    }
}
