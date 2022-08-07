using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public List<Vector2> vertices;
    public List<Edge> edges;
    public Triangle backNeighbor;
    public Triangle rightNeighbor;
    public Triangle leftNeighbor;

    public Triangle(List<Vector2> vertices)
    {
        this.vertices = vertices;
        this.edges = new List<Edge>()
        {
            new Edge(vertices[0], vertices[1]),
            new Edge(vertices[1], vertices[2]),
            new Edge(vertices[2], vertices[0]),
        };
    }

    public static bool operator ==(Triangle t1, Triangle t2)
    {
        return ListEquals(t1.edges, t2.edges) && ListEquals(t1.vertices, t2.vertices);
    }

    public static bool operator !=(Triangle t1, Triangle t2)
    {
        return !ListEquals(t1.edges, t2.edges) || !ListEquals(t1.vertices, t2.vertices);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Triangle t = (Triangle)obj;
        return ListEquals(this.edges, t.edges) && ListEquals(this.vertices, t.vertices);
    }

    public override int GetHashCode()
    {
        return vertices.GetHashCode() ^ edges.GetHashCode();
    }

    public static bool ListEquals<T>(IList<T> l1, IList<T> l2)
    {
        if (l1.Count != l2.Count)
        {
            return false;
        }
        for (int i = 0; i < l1.Count; ++i)
        {
            if (!l1[i].Equals(l2[i]))
            {
                return false;
            }
        }
        return true;
    }

    public override string ToString()
    {
        return "Triangle \nP1: " + vertices[0].ToString() + " P2: " + vertices[1].ToString() + " P3: " + vertices[2].ToString();
    }
}
