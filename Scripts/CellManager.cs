using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CellManager
{
    private Dictionary<string, GameObject> namePrefabMap = new Dictionary<string, GameObject>();
    private Dictionary<Cell, CellDataToMesh> cellDataMap = new Dictionary<Cell, CellDataToMesh>();

    public void Init()
    {
        GameObject parent = Resources.Load<GameObject>("Meshes/modules");
        foreach (Transform child in parent.transform)
        {
            namePrefabMap.Add(child.name, child.gameObject);
        }
        namePrefabMap.Add("Interior", Resources.Load<GameObject>("Prefabs/Interior"));
        namePrefabMap.Add("Exterior", Resources.Load<GameObject>("Prefabs/Exterior"));
        Debug.Log($"namePrefabMap count: {namePrefabMap.Count}");
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
        CellDataToMesh cdtm = parent.AddComponent<CellDataToMesh>();
        cellDataMap.Add(cell, cdtm);
        cdtm.Init(namePrefabMap, cell);
    }
}
