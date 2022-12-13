using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CellManager
{
    private GameObject tilePrefab;
    private Transform rootTransform;
    private Dictionary<string, Mesh> nameMeshMap = new Dictionary<string, Mesh>();
    private Dictionary<Cell, CellDataToMesh> cellDataMap = new Dictionary<Cell, CellDataToMesh>();

    public void Init(Transform rootTransform)
    {
        List<Mesh> meshes = new List<Mesh>(Resources.LoadAll<Mesh>("Meshes"));
        foreach (Mesh mesh in meshes)
        {
            nameMeshMap.Add(mesh.name, mesh);
            mesh.SetTriangles(mesh.triangles, 0);
        }

        tilePrefab = Resources.Load<GameObject>("Prefabs/Tile");

        this.rootTransform = rootTransform;
    }

    public void Clear()
    {
        foreach (CellDataToMesh cdtm in cellDataMap.Values)
        {
            GameObject.Destroy(cdtm.gameObject);
        }
        cellDataMap.Clear();
    }

    public void UpdateCells(List<Cell> cells, bool drawAll)
    {
        List<Cell> toBeDeleted = cellDataMap.Keys.ToList();
        cells.ForEach(cell => toBeDeleted.Remove(cell));
        foreach (Cell key in toBeDeleted)
        {
            GameObject.Destroy(cellDataMap[key].gameObject);
            cellDataMap.Remove(key);
        }

        foreach (Cell cell in cells)
        {
            CreateOrUpdateMeshFromCell(cell, drawAll);
        }
    }

    void CreateOrUpdateMeshFromCell(Cell cell, bool drawAll)
    {
        if (cellDataMap.ContainsKey(cell))
            cellDataMap[cell].UpdateVisuals(cell, drawAll);
        else
            CreateMeshFromCell(cell);
    }

    void CreateMeshFromCell(Cell cell)
    {
        GameObject parent = new GameObject();
        parent.transform.parent = rootTransform;
        CellDataToMesh cdtm = parent.AddComponent<CellDataToMesh>();
        cellDataMap.Add(cell, cdtm);
        cdtm.Init(nameMeshMap, tilePrefab, cell);
    }
}
