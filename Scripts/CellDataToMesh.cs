﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class CellDataToMesh : MonoBehaviour
{
    [SerializeField]
    List<string> prototypeNames = new List<string>();
    Cell cell;
    [SerializeField]
    bool debug = false;
    bool isCollapsed = false;
    float yScaling = 0.5f;
    private Dictionary<string, Mesh> namePrefabMap = new Dictionary<string, Mesh>();
    private GameObject tilePrefab;

    public void Clear()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
        isCollapsed = false;
        prototypeNames = new List<string>();
    }

    public void Init(Dictionary<string, Mesh> namePrefabMap, GameObject tilePrefab, Cell cell)
    {
        this.cell = cell;
        this.namePrefabMap = namePrefabMap;
        this.tilePrefab = tilePrefab;

        transform.position = cell.tile.GetCenter();

        UpdateName();
    }

    public void UpdateVisuals(Cell cell, bool drawAll)
    {
        this.cell = cell;
        UpdateName();
        if (drawAll && !isCollapsed)
        {
            foreach (int p in cell.prototypes)
            {
                CreateMeshFromPrototype(PrototypeManager.prototypes[p], cell.tile.ToTriangle(), cell.tile.y * yScaling);
            }
            if (cell.prototypes.Count == 1)
                isCollapsed = true;
        }
        else
        {
            if (!isCollapsed && cell.prototypes.Count == 1)
            {
                CreateMeshFromPrototype(PrototypeManager.prototypes[cell.prototypes[0]], cell.tile.ToTriangle(), cell.tile.y * yScaling);
                isCollapsed = true;
            }
        }
        if (!isCollapsed && cell.prototypes.Count == 0)
        {
            Triangle tri = cell.tile.ToTriangle();
            CreateMeshObjectAtPoint("Interior", GetTriangleCenter(tri, cell.tile.y * yScaling));
        }
    }

    private void UpdateName()
    {
        string tileCoords = $"{cell.tile.a}, {cell.tile.b}, {cell.tile.c}, {cell.tile.y}";
        if (cell.prototypes.Count == 1)
            name = $"{tileCoords} Collapsed: {PrototypeManager.prototypes[cell.prototypes[0]].name}";
        else if (cell.prototypes.Count == 0)
            name = $"{tileCoords} FAILED";
        else
            name = tileCoords;
        prototypeNames = cell.prototypes.Select(p => PrototypeManager.prototypes[p].name).ToList();
    }

    Vector3 GetTriangleCenter(Triangle triangle, float y)
    {
            Vector2 center = (triangle.vertices[0] + triangle.vertices[1] + triangle.vertices[2]) / 3;
            return new Vector3(center.x, y, center.y);
    }

    GameObject CreateMeshFromPrototype(Prototype proto, Triangle triangle, float y)
    {
        string meshName = proto.meshName;

        if (meshName == "")
        {
            if (!debug)
                return null;
            Vector3 pos = GetTriangleCenter(triangle, y);

            GameObject go;
            if (proto.name.StartsWith("III"))
                go = CreateMeshObjectAtPoint("Interior", new Vector3((triangle.vertices[1].x + pos.x) / 2, y, (triangle.vertices[1].y + pos.z) / 2));
            else
                go = CreateMeshObjectAtPoint("Exterior", new Vector3((triangle.vertices[0].x + pos.x) / 2, y, (triangle.vertices[0].y + pos.z) / 2));
            go.name = proto.name;
            return go;
        }

        Vector2 p1_2d = triangle.vertices[(0 + proto.rotation) % 3];
        Vector2 p2_2d = triangle.vertices[(1 + proto.rotation) % 3];
        Vector2 p3_2d = triangle.vertices[(2 + proto.rotation) % 3];
        Vector3 p1 = new Vector3(p1_2d.x, y, p1_2d.y);
        Vector3 p2 = new Vector3(p2_2d.x, y, p2_2d.y);
        Vector3 p3 = new Vector3(p3_2d.x, y, p3_2d.y);

        return CreateMeshObjectAtPoints(meshName, p1, p2, p3);
    }

    public GameObject CreateMeshObjectAtPoints(string meshName, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        GameObject go = CreateMeshObjectAtPoints(meshName, p1, p2);
        MeshTransformer mt = go.GetComponent<MeshTransformer>();
        mt.ScaleShearTowardsPoint(p3);

        return go;
    }

    public GameObject CreateMeshObjectAtPoints(string meshName, Vector3 p1, Vector3 p2)
    {
        GameObject go = CreateMeshObjectAtPoint(meshName, p1);
        MeshTransformer mt = go.GetComponent<MeshTransformer>();
        mt.Init();
        mt.StretchXTowardsPoint(p2);

        return go;
    }

    public GameObject CreateMeshObjectAtPoint(string meshName, Vector3 position)
    {
        GameObject go = CreateMeshObject(meshName);
        go.transform.position = position;
        return go;
    }

    GameObject CreateMeshObject(string meshName)
    {
        GameObject go;
        if (meshName == "Interior")
        {
            go = Instantiate(Resources.Load<GameObject>("Prefabs/Interior"), transform);
        }
        else
        {
            go = Instantiate(tilePrefab, transform);
            go.GetComponent<MeshFilter>().mesh = namePrefabMap[meshName];
        }
        go.transform.localScale = new Vector3(1, yScaling, 1);
        return go;
    }
}
