using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombinerAndCleaner : MonoBehaviour
{
    private void OnEnable()
    {
        WaveFunctionCollapse.OnFinishIsland += CombineAndCleanMesh;
    }

    public void CombineAndCleanMesh()
    {
        GetComponent<MeshWelder>().Weld();
        MeshSmoother smoother = new MeshSmoother(GetComponent<MeshFilter>().mesh);
        smoother.Smooth(50, 0.05f);
    }
}
