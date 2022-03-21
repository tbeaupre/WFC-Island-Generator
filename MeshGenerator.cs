using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateDelaunayTriangulation();
        // CreateSuperTriangle();
        // CreateShape();
        UpdateMesh();
    }

    void CreateDelaunayTriangulation()
    {
        var points = new Vector2[] {
            new Vector2 (0, 0),
            new Vector2 (0, 1),
            new Vector2 (1, 0),
            new Vector2 (1, 1),
        };

        List<Triangle> triangulation = DelaunayTriangulation.Generate(points);

        List<Vector2> vertices2dList = new List<Vector2>();
        List<int> trianglesList = new List<int>();
        foreach(Triangle t in triangulation)
        {
            foreach(Vector2 p in t.vertices)
            {
                int i = vertices2dList.IndexOf(p);
                if (i == -1)
                {
                    trianglesList.Add(vertices2dList.Count);
                    vertices2dList.Add(p);
                }
                else
                {
                    trianglesList.Add(i);
                }
            }
        }

        List<Vector3> verticesList = new List<Vector3>();
        foreach(Vector2 p in vertices2dList)
        {
            verticesList.Add(p);
        }

        vertices = verticesList.ToArray();
        triangles = trianglesList.ToArray();
    }

    void CreateSuperTriangle()
    {
        var points = new Vector2[] {
            new Vector2 (0, 0),
            new Vector2 (0, 1),
            new Vector2 (1, 0),
            new Vector2 (1, 1),
        };

        (Vector2 min, Vector2 max) = Utility.GetCoveringSquare(points);
        Triangle t = Utility.GetSuperTriangle(min, max);

        vertices = new Vector3[] {
            t.vertices[0],
            t.vertices[1],
            t.vertices[2],
        };
        
        triangles = new int[] {
            0, 1, 2
        };
    }

    void CreateShape()
    {
        vertices = new Vector3[]
        {
            new Vector3 (0,0,0),
            new Vector3 (0,0,1),
            new Vector3 (1,0,0),
            new Vector3 (1,0,1),
        };

        triangles = new int[]
        {
            0, 1, 2,
            1, 3, 2,
        };
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
