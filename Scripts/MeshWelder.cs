using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshWelder : MonoBehaviour
{
    private void OnEnable()
    {
        WaveFunctionCollapse.OnFinishIsland += Weld;
    }

    public void Weld()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int vertCount = 0;

        for (int i = 0; i < meshFilters.Length; ++i)
        {
            vertCount += meshFilters[i].mesh.vertexCount;
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combine);

        Debug.Log($"Uncombined Vert Count: {vertCount};  Combined Vert Count: {mf.mesh.vertexCount}");
    }
}
