using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSmoother
{
    Mesh mesh;
    int[] indexMap; // Maps from original vertex index to combinedVertIndex
    List<int> combinedVertIndices = new List<int>(); // Each item is an index to the original vert array for a distinct pos
    Dictionary<int, HashSet<int>> connectionMap = new Dictionary<int, HashSet<int>>();
    float tolerance = 0.001f;

    public MeshSmoother(Mesh mesh, float tolerance = 0.001f)
    {
        this.mesh = mesh;
        this.tolerance = tolerance;

        CreateConnectionMap();
    }

    void CreateConnectionMap()
    {
        Vector3[] verts = mesh.vertices;
        indexMap = new int[verts.Length];

        // Create mapping and filter duplicates.
        for (int i = 0; i < verts.Length; i++)
        {
            var p = verts[i];
            bool duplicate = false;
            for (int j = 0; j < combinedVertIndices.Count; j++)
            {
                int originalIndex = combinedVertIndices[j];
                if ((verts[originalIndex] - p).sqrMagnitude <= tolerance)
                {
                    indexMap[i] = j;
                    duplicate = true;
                    break;
                }
            }
            if (!duplicate)
            {
                indexMap[i] = combinedVertIndices.Count;
                combinedVertIndices.Add(i);
            }
        }

        int[] tris = mesh.triangles;
        for (int i = 0; i < tris.Length; i += 3)
        {
            //if (tris[i] >= combinedVertIndices.Count || tris[i + 1] >= combinedVertIndices.Count || tris[i + 2] >= combinedVertIndices.Count)
            //{
            //    Debug.Log($"{i} / {tris.Length}");
            //    Debug.Log($"verts.Length: {verts.Length}");
            //    Debug.Log($"combinedVertIndices.Count: {combinedVertIndices.Count}");
            //    Debug.Log($"i: {tris[i]}, j: {tris[i + 1]}, k: {tris[i + 2]}");
            //}

            AddTriangleToConnectionMap(
                combinedVertIndices[indexMap[tris[i]]],
                combinedVertIndices[indexMap[tris[i + 1]]],
                combinedVertIndices[indexMap[tris[i + 2]]]);
        }
    }

    void AddTriangleToConnectionMap(int i, int j, int k)
    {
        if (connectionMap.ContainsKey(i))
            connectionMap[i].UnionWith(new HashSet<int> { j, k });
        else
            connectionMap.Add(i, new HashSet<int> { j, k });

        if (connectionMap.ContainsKey(j))
            connectionMap[j].UnionWith(new HashSet<int> { i, k });
        else
            connectionMap.Add(j, new HashSet<int> { i, k });

        if (connectionMap.ContainsKey(k))
            connectionMap[k].UnionWith(new HashSet<int> { j, i });
        else
            connectionMap.Add(k, new HashSet<int> { j, i });
    }

    public void Smooth(int iterations, float weight)
    {
        for (int iter = 0; iter < iterations; ++iter)
        {
            Smooth(weight);
        }
    }

    void Smooth(float weight)
    {
        Vector3[] verts = mesh.vertices;
        Vector3[] newVerts = new Vector3[verts.Length];
        for (int i = 0; i < verts.Length; ++i)
        {
            Vector3 sumNeighborPos = Vector3.zero;
            HashSet<int> neighbors = connectionMap[combinedVertIndices[indexMap[i]]];
            foreach (int neighborIndex in neighbors)
            {
                sumNeighborPos += verts[neighborIndex];
            }
            Vector3 aveNeighborPos = sumNeighborPos / neighbors.Count;
            newVerts[i] = verts[i] + (weight * (aveNeighborPos - verts[i]));
        }

        mesh.vertices = newVerts;
    }
}
