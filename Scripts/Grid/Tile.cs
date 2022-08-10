using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    const float SQRT_3 = 1.73205080757f;
    private static float triWidth = 1;
    private static float yFactor = Mathf.Sin(60 * Mathf.Deg2Rad);
    private static float triHeight = yFactor * triWidth;
    public int a;
    public int b;
    public int c;
    public int y;

    public Tile(int a, int b, int c, int y)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.y = y;
    }

    public int DistanceTo(int a, int b, int c)
    {
        return Mathf.Abs(this.a - a) + Mathf.Abs(this.b - b) + Mathf.Abs(this.c - c);
    }

    public Triangle ToTriangle()
    {
        Vector2 triCenter = new Vector2(
            ((0.5f * a) + (-0.5f * c)) * triWidth,
            ((-SQRT_3 / 6 * a) + (SQRT_3 / 3 * b) - (SQRT_3 / 6 * c)) * triWidth);
        float yOffsetToBase = SQRT_3 / 6 * triWidth;
        if (PointsUp)
        {
            Vector2 p1 = triCenter + new Vector2(0.5f * triWidth, -yOffsetToBase);
            Vector2 p2 = triCenter + new Vector2(-0.5f * triWidth, -yOffsetToBase);
            Vector2 p3 = triCenter + new Vector2(0, triHeight - yOffsetToBase);
            return new Triangle(new List<Vector2> { p1, p2, p3 });
        }
        else
        {
            Vector2 p1 = triCenter + new Vector2(-0.5f * triWidth, yOffsetToBase);
            Vector2 p2 = triCenter + new Vector2(0.5f * triWidth, yOffsetToBase);
            Vector2 p3 = triCenter + new Vector2(0, -triHeight + yOffsetToBase);
            return new Triangle(new List<Vector2> { p1, p2, p3 });
        }
    }

    public bool IsValid()
    {
        int sum = a + b + c;
        return (sum == 1 || sum == 2);
    }

    public bool PointsUp => a + b + c == 2;

    public bool PositionalMatch(int a, int b, int c, int y) => this.a == a && this.b == b && this.c == c && this.y == y;
}