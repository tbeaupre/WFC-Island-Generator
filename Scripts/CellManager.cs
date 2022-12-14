using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class CellManager
{
    private static GameObject tilePrefab;
    private static Transform rootTransform;
    private static Dictionary<string, Mesh> nameMeshMap = new Dictionary<string, Mesh>();
    private static Dictionary<Tile, CellDataToMesh> tileCellMap = new Dictionary<Tile, CellDataToMesh>();

    public static void Init(Transform _rootTransform)
    {
        List<Mesh> meshes = new List<Mesh>(Resources.LoadAll<Mesh>("Meshes"));
        foreach (Mesh mesh in meshes)
        {
            nameMeshMap.Add(mesh.name, mesh);
            mesh.SetTriangles(mesh.triangles, 0);
        }

        tilePrefab = Resources.Load<GameObject>("Prefabs/Tile");

        rootTransform = _rootTransform;
    }

    public static void Clear()
    {
        foreach (CellDataToMesh cdtm in tileCellMap.Values)
        {
            cdtm.Clear();
        }
    }

    public static void UpdateCells(List<Cell> cells, bool drawAll)
    {
        foreach (Cell cell in cells)
        {
            CreateOrUpdateMeshFromCell(cell, drawAll);
        }
    }

    static void CreateOrUpdateMeshFromCell(Cell cell, bool drawAll)
    {
        if (tileCellMap.ContainsKey(cell.tile))
            tileCellMap[cell.tile].UpdateVisuals(cell, drawAll);
        else
            CreateMeshFromCell(cell);
    }

    static void CreateMeshFromCell(Cell cell)
    {
        GameObject parent = new GameObject();
        parent.transform.parent = rootTransform;
        CellDataToMesh cdtm = parent.AddComponent<CellDataToMesh>();
        tileCellMap.Add(cell.tile, cdtm);
        cdtm.Init(nameMeshMap, tilePrefab, cell);
    }
}
