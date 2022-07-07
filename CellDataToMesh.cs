using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CellDataToMesh : MonoBehaviour
{
    public GameObject CreateMeshFromCell(Cell cell)
    {
        Prototype proto = cell.prototypes[0];
        string meshName = proto.meshName;
        float y = cell.y * 1.0f;

        if (meshName == "")
        {
            Vector2 center = (cell.triangle.vertices[0] + cell.triangle.vertices[1] + cell.triangle.vertices[2]) / 3;
            Vector3 pos = new Vector3(center.x, y, center.y);

            if (proto.name.StartsWith("III"))
                return CreateMeshObjectAtPoint("Interior", pos);
            if (proto.name.StartsWith("EEE"))
                return CreateMeshObjectAtPoint("Exterior", pos);
        }

        Vector2 p1_2d = cell.triangle.vertices[(0 + proto.rotation) % 3];
        Vector2 p2_2d = cell.triangle.vertices[(1 + proto.rotation) % 3];
        Vector2 p3_2d = cell.triangle.vertices[(2 + proto.rotation) % 3];
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
        MeshTransformer mt = go.AddComponent<MeshTransformer>();
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

    GameObject CreateMeshObjectWithTransform(string meshName, int rotation)
    {
        GameObject go = CreateMeshObject(meshName);
        RotateObject(go, rotation);
        return go;
    }

    void RotateObject(GameObject go, int rotation)
    {
        switch (rotation)
        {
            case 0:
                return;
            case 1:
                go.transform.Rotate(new Vector3(0, 120, 0));
                break;
            case 2:
                go.transform.Rotate(new Vector3(0, 240, 0));
                break;
        }
    }

    GameObject CreateMeshObject(string meshName)
    {
        Debug.Log(meshName);
        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + meshName));
        return go;
    }
}
