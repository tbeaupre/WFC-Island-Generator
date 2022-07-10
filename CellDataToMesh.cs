using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CellDataToMesh : MonoBehaviour
{
    public GameObject CreateMeshFromCell(Cell cell)
    {
        GameObject parent = new GameObject();
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
            Vector3 pos = GetTriangleCenter(triangle, y);

            if (proto.name.StartsWith("III"))
                return CreateMeshObjectAtPoint("Interior", pos, parent);
            if (proto.name.StartsWith("EEE"))
                return CreateMeshObjectAtPoint("Exterior", pos, parent);
        }

        Vector2 p1_2d = triangle.vertices[(0 + proto.rotation) % 3];
        Vector2 p2_2d = triangle.vertices[(1 + proto.rotation) % 3];
        Vector2 p3_2d = triangle.vertices[(2 + proto.rotation) % 3];
        // Swap vertex order so triangles render face up
        Vector3 p3 = new Vector3(p1_2d.x, y, p1_2d.y);
        Vector3 p2 = new Vector3(p2_2d.x, y, p2_2d.y);
        Vector3 p1 = new Vector3(p3_2d.x, y, p3_2d.y);
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
        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + meshName), parent.transform);
        return go;
    }
}
