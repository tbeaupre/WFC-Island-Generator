using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public Vector2 p1;
    public Vector2 p2;

    public Edge(Vector2 p1, Vector2 p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }

    public static bool operator ==(Edge e1, Edge e2)
    {
        return (e1.p1 == e2.p1 && e1.p2 == e2.p2) || (e1.p1 == e2.p2 && e1.p2 == e2.p1);
    }

    public static bool operator !=(Edge e1, Edge e2)
    {
        return (e1.p1 != e2.p1 || e1.p2 != e2.p2) && (e1.p1 != e2.p2 || e1.p2 != e2.p1);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Edge e = (Edge)obj;
        return (this.p1 == e.p1 && this.p2 == e.p2) || (this.p1 == e.p2 && this.p2 == e.p1);
    }

    public override string ToString()
    {
        return "Edge P1: " + p1.ToString() + ", P2: " + p2.ToString();
    }

    public override int GetHashCode()
    {
        return p1.GetHashCode() ^ p2.GetHashCode();
    }
}
