using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid2
{
    public List<Triangle> GetTriangulation(int size, float triSize)
    {
        float triHeight = Mathf.Sin(60 * Mathf.Deg2Rad) * triSize;
        List<Triangle> triangles = new List<Triangle>();

        Vector2[,] vertices = new Vector2[size, size];
        for (int j = 0; j < size; ++j)
        {
            for (int i = 0; i < size; ++i)
            {
                // float xOffset = (j % 2 == 1) ? 0.5f * triSize : 0;
                float x = (i + 0.5f * j) * triSize;// + xOffset;
                float y = j * triHeight;
                vertices[i, j] = new Vector2(x, y);
            }
        }

        for (int j = 0; j < (size - 1) * 2 ; ++j)
        {
            int y = j / 2;
            if (j % 2 == 0)
            {
                for (int i = 0; i < size - 1; ++i)
                {
                    Vector2 p1 = vertices[i, y];
                    Vector2 p2 = vertices[i + 1, y];
                    Vector2 p3 = vertices[i, y + 1];
                    triangles.Add(new Triangle(new List<Vector2>() { p3, p2, p1 }));
                }
            }
            else
            {
                ++y;
                for (int i = 0; i < size  -1; ++i)
                {
                    Vector2 p1 = vertices[i + 1, y];
                    Vector2 p2 = vertices[i, y];
                    Vector2 p3 = vertices[i + 1, y - 1];
                    triangles.Add(new Triangle(new List<Vector2>() { p3, p2, p1 }));
                }
            }
        }

        // Set up Triangle neighbor lists
        foreach(Triangle t1 in triangles)
        {
            foreach(Triangle t2 in triangles)
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
}
