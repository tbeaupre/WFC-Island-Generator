using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombinerAndCleaner : MonoBehaviour
{
    private void OnEnable()
    {
        WaveFunctionCollapse.OnStartIsland += Init;
        WaveFunctionCollapse.OnFinishIsland += CombineAndCleanMesh;
    }

    public void Init()
    {
        gameObject.GetComponent<MeshFilter>().mesh = new Mesh();
    }

    public void CombineAndCleanMesh()
    {
        GetComponent<MeshWelder>().Weld();
        MeshSmoother smoother = new MeshSmoother(GetComponent<MeshFilter>().mesh);
        smoother.Smooth(10, 0.05f);
    }
}
