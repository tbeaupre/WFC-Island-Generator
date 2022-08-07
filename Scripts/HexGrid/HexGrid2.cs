using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid2
{
    public List<Triangle> GetTriangulation(int sideLength, float triSize)
    {
        float triHeight = Mathf.Sin(60 * Mathf.Deg2Rad) * triSize;
        List<Triangle> triangles = new List<Triangle>();
        int size = sideLength * 2 + 1;

        Vector2[,] vertices = new Vector2[size, size];
        for (int j = 0; j < size; ++j)
        {
            for (int i = 0; i < size; ++i)
            {
                float x = (i + 0.5f * j) * triSize;
                float y = j * triHeight;
                vertices[i, j] = new Vector2(x, y);
            }
        }

        // Create first half of hexagon
        for (int j = 0; j < sideLength; ++j)
        {
            triangles.AddRange(CreateRow(vertices, sideLength - j, j, sideLength + j));
            triangles.AddRange(CreateUpsideDownRow(vertices, sideLength - j, j, sideLength + j + 1));
            Debug.Log(triangles.Count);
        }

        for (int j = sideLength; j < sideLength * 2; ++j)
        {
            triangles.AddRange(CreateUpsideDownRow(vertices, 1, j, 3 * sideLength - j - 1));
            triangles.AddRange(CreateRow(vertices, 0, j, 3 * sideLength - j));
        }

        // Set up Triangle neighbor lists
        foreach (Triangle t1 in triangles)
        {
            foreach (Triangle t2 in triangles)
            {
                if (t1 == t2)
                    continue;

                if (t2.edges.Contains(t1.edges[0]))
                    t1.backNeighbor = t2;
                
                if (t2.edges.Contains(t1.edges[1]))
                    t1.rightNeighbor = t2;

                if (t2.edges.Contains(t1.edges[2]))
                    t1.leftNeighbor = t2;
            }
        }

        return triangles;
    }
    List<Triangle> CreateRow(Vector2[,] vertices, int x, int y, int width)
    {
        List<Triangle> result = new List<Triangle>();

        for (int i = x; i < x + width; ++i)
        {
            // Debug.Log("(" + i + ", " + y + "); (" + (i + 1) + ", " + y + "); (" + i + ", " + (y + 1) + "); ");
            Vector2 p1 = vertices[i, y];
            Vector2 p2 = vertices[i + 1, y];
            Vector2 p3 = vertices[i, y + 1];
            result.Add(new Triangle(new List<Vector2>() { p3, p2, p1 }));
        }

        return result;
    }

    List<Triangle> CreateUpsideDownRow(Vector2[,] vertices, int x, int y, int width)
    {
        List<Triangle> result = new List<Triangle>();
        // Debug.Log(x + ", " + y + ", " + width);
        for (int i = x; i < x + width; ++i)
        {
            // Debug.Log("(" + i + ", " + (y+1) + "); (" + (i - 1) + ", " + (y+1) + "); (" + i + ", " + y + "); ");
            Vector2 p1 = vertices[i, y + 1];
            Vector2 p2 = vertices[i - 1, y + 1];
            Vector2 p3 = vertices[i, y];
            result.Add(new Triangle(new List<Vector2>() { p3, p2, p1 }));
        }

        return result;
    }
}
