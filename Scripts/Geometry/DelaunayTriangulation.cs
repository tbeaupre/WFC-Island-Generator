using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://github.com/vanreusVU/Delaunay-Triangulation
public class DelaunayTriangulation
{
    public static List<Triangle> Generate(Vector2[] points)
    {
        List<Triangle> triangulation = new List<Triangle>();

        (Vector2 min, Vector2 max) = Utility.GetCoveringSquare(points);
        Triangle superTriangle = Utility.GetSuperTriangle(min, max);

        triangulation.Add(superTriangle);

        foreach(Vector2 p in points)
        {
            List<Triangle> badTriangles = new List<Triangle>();

            foreach(Triangle t in triangulation)
            {
                if (IsWithinCircumcircle(p, t))
                {
                    badTriangles.Add(t);
                }
            }

            List<Edge> polygon = new List<Edge>();

            foreach(Triangle t1 in badTriangles)
            {
                foreach(Edge e in t1.edges)
                {
                    bool isEdgeShared = false;
                    foreach(Triangle t2 in badTriangles)
                    {
                        if (t1 == t2)
                        {
                            continue;
                        }

                        if (t2.edges.Contains(e))
                        {
                            isEdgeShared = true;
                        }
                    }

                    if (!isEdgeShared)
                    {
                        polygon.Add(e);
                    }
                }
            }

            // Remove bad triangles
            foreach (Triangle badT in badTriangles)
            {
                triangulation.Remove(badT);
            }

            // Create triangles with the newly created edges
            foreach(Edge e in polygon)
            {
                triangulation.Add(new Triangle(new List<Vector2>() { e.p1, e.p2, p }));
            }
        }

        // Remove the triangles with connections to the super-triangle
        List<Vector2> superVertices = new List<Vector2>(superTriangle.vertices);
        for (int i = triangulation.Count - 1; i > -1; --i)
        {
            Triangle t = triangulation[i];

            bool hasCommonVertex = false;
            foreach (Vector2 v1 in t.vertices)
            {
                foreach(Vector2 v2 in superVertices)
                {
                    if (v1 == v2)
                    {
                        hasCommonVertex = true;
                        break;
                    }
                }
                // Could consider adding hasCommonVertex check and breaking if true
            }

            if (hasCommonVertex)
            {
                triangulation.Remove(t);
            }
        }

        // TODO: Flip triangles to CW from CCW

        // Set up Triangle neighbor lists
        foreach(Triangle t1 in triangulation)
        {
            foreach(Triangle t2 in triangulation)
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

        // Debug.Log(triangulation.Count);
        // foreach(Triangle t in triangulation)
        // {
        //     Debug.Log(t.ToString());
        // }

        return triangulation;
    }

    // https://stackoverflow.com/a/44875841
    private static bool IsWithinCircumcircle(Vector2 p, Triangle t)
    {
        Vector2 a_ = t.vertices[0] - p;
        Vector2 b_ = t.vertices[1] - p;
        Vector2 c_ = t.vertices[2] - p;

        return (
            (a_.x * a_.x + a_.y * a_.y) * (b_.x * c_.y - c_.x * b_.y) -
            (b_.x * b_.x + b_.y * b_.y) * (a_.x * c_.y - c_.x * a_.y) +
            (c_.x * c_.x + c_.y * c_.y) * (a_.x * b_.y - b_.x * a_.y)
        ) > 0;
    }
}


public static class Utility
{
    public static Triangle GetSuperTriangle(Vector2 min, Vector2 max)
    {
        float safety = 2;

        Vector2 p1 = new Vector2(max.x + safety, min.y - safety);
        Vector2 p2 = new Vector2(max.x + safety, max.y + Mathf.Abs(max.y - min.y) + safety);
        Vector2 p3 = new Vector2(min.x - Mathf.Abs(max.x - min.x) - safety, min.y - safety);

        return new Triangle(new List<Vector2>() { p1, p2, p3 });
    }

    public static (Vector2 min, Vector2 max) GetCoveringSquare(Vector2[] points)
    {
        Vector2 min = new Vector2(points[0].x, points[0].y);
        Vector2 max = new Vector2(points[0].x, points[0].y);

        foreach (Vector2 p in points)
        {
            if (p.x > max.x)
            {
                max = new Vector2(p.x, max.y);
            }
            else if (p.x < min.x)
            {
                min = new Vector2(p.x, min.y);
            }

            if (p.y > max.y)
            {
                max = new Vector2(max.x, p.y);
            }
            else if (p.y < min.y)
            {
                min = new Vector2(min.x, p.y);
            }
        }

        return (min, max);
    }
}