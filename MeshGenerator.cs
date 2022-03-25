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
    }

    public void Recalculate(List<Vector3> points)
    {
        List<Vector2> points2d = new List<Vector2>();
        foreach(Vector3 p in points)
        {
            points2d.Add(new Vector2(p.x, p.z));
        }

        CreateDelaunayTriangulation(points2d.ToArray());
        UpdateMesh();
    }

    void CreateDelaunayTriangulation(Vector2[] points)
    {
        List<Triangle> triangulation = DelaunayTriangulation.Generate(points);

        List<Vector2> vertices2dList = new List<Vector2>();
        List<int> trianglesList = new List<int>();
        foreach(Triangle t in triangulation)
        {
            // Swap vertex order so triangles render face up
            for (int i = t.vertices.Count - 1; i > -1; --i)
            {
                Vector2 p = t.vertices[i];
                int index = vertices2dList.IndexOf(p);
                if (index == -1)
                {
                    trianglesList.Add(vertices2dList.Count);
                    vertices2dList.Add(p);
                }
                else
                {
                    trianglesList.Add(index);
                }
            }
        }

        List<Vector3> verticesList = new List<Vector3>();
        foreach(Vector2 p in vertices2dList)
        {
            verticesList.Add(new Vector3(p.x, 0, p.y));
        }

        vertices = verticesList.ToArray();
        triangles = trianglesList.ToArray();
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
