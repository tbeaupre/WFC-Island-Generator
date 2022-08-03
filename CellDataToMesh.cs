using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CellDataToMesh : MonoBehaviour
{
    private Dictionary<string, GameObject> namePrefabMap = new Dictionary<string, GameObject>();

    private void Awake()
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

    public GameObject CreateMeshFromCell(Cell cell)
    {
        GameObject parent = new GameObject();
        if (cell.prototypes.Count == 1)
            parent.name = "Collapsed: " + cell.prototypes[0].name;
        else if (cell.prototypes.Count == 0)
            parent.name = "FAILED";
        parent.transform.position = GetTriangleCenter(cell.triangle, cell.y * 1.0f);

        foreach (Prototype p in cell.prototypes)
        {
            CreateMeshFromPrototype(p, cell.triangle, cell.y, parent);
        }
        return parent;
    }

    Vector3 GetTriangleCenter(Triangle triangle, float y)
    {
            Vector2 center = (triangle.vertices[0] + triangle.vertices[1] + triangle.vertices[2]) / 3;
            return new Vector3(center.x, y, center.y);
    }

    GameObject CreateMeshFromPrototype(Prototype proto, Triangle triangle, float y, GameObject parent)
    {
        string meshName = proto.meshName;

        if (meshName == "")
        {
            return null;
            Vector3 pos = GetTriangleCenter(triangle, y);

            GameObject go;
            if (proto.name.StartsWith("III"))
                go = CreateMeshObjectAtPoint("Interior", new Vector3((triangle.vertices[1].x + pos.x) / 2, y, (triangle.vertices[1].y + pos.z) / 2), parent);
            else
                go = CreateMeshObjectAtPoint("Exterior", new Vector3((triangle.vertices[0].x + pos.x) / 2, y, (triangle.vertices[0].y + pos.z) / 2), parent);
            go.name = proto.name;
            return go;
        }

        Vector2 p1_2d = triangle.vertices[(0 + proto.rotation) % 3];
        Vector2 p2_2d = triangle.vertices[(1 + proto.rotation) % 3];
        Vector2 p3_2d = triangle.vertices[(2 + proto.rotation) % 3];
        Vector3 p1 = new Vector3(p1_2d.x, y, p1_2d.y);
        Vector3 p2 = new Vector3(p2_2d.x, y, p2_2d.y);
        Vector3 p3 = new Vector3(p3_2d.x, y, p3_2d.y);

        return CreateMeshObjectAtPoints(meshName, p1, p2, p3, parent);
    }

    public GameObject CreateMeshObjectAtPoints(string meshName, Vector3 p1, Vector3 p2, Vector3 p3, GameObject parent)
    {
        GameObject go = CreateMeshObjectAtPoints(meshName, p1, p2, parent);
        MeshTransformer mt = go.GetComponent<MeshTransformer>();
        mt.ScaleShearTowardsPoint(p3);

        return go;
    }

    public GameObject CreateMeshObjectAtPoints(string meshName, Vector3 p1, Vector3 p2, GameObject parent)
    {
        GameObject go = CreateMeshObjectAtPoint(meshName, p1, parent);
        MeshTransformer mt = go.AddComponent<MeshTransformer>();
        mt.Init();
        mt.StretchXTowardsPoint(p2);

        return go;
    }

    public GameObject CreateMeshObjectAtPoint(string meshName, Vector3 position, GameObject parent)
    {
        GameObject go = CreateMeshObject(meshName, parent);
        go.transform.position = position;
        return go;
    }

    GameObject CreateMeshObject(string meshName, GameObject parent)
    {
        GameObject go = Instantiate(namePrefabMap[meshName], parent.transform);
        return go;
    }
}
