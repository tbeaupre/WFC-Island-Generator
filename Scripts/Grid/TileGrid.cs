using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileGrid
{
    public List<Tile> tiles = new List<Tile>();
    public Dictionary<(int, int, int, int), Tile> tileMap = new Dictionary<(int, int, int, int), Tile>();

    public TileGrid(int radius, int height)
    {
        int min = -radius;
        int max = radius;
        float squareRadius = radius * radius + 0.1f;
        for (int a = min; a <= max; ++a)
        {
            for (int b = min; b <= max; ++b)
            {
                for (int c = min; c <= max; ++c)
                {
                    Tile testTile = new Tile(a, b, c, 0);
                    if (testTile.IsValid() && testTile.SquareDistToCenter() <= squareRadius)
                    {
                        tileMap.Add((a, b, c, 0), testTile);
                        for (int y = 1; y < height; ++y)
                        {
                            Tile tile = new Tile(a, b, c, y);
                            tileMap.Add((a, b, c, y), tile);
                        }
                    }
                }
            }
        }
    }

    public Tile GetTile(int a, int b, int c, int y) => tileMap.ContainsKey((a, b, c, y)) ? tileMap[(a, b, c, y)] : null;

    public Tile GetNeighbor(Tile t, Direction dir)
    {
        switch (dir)
        {
            case Direction.Back:
                if (t.PointsUp)
                    return GetTile(t.a, t.b - 1, t.c, t.y);
                else
                    return GetTile(t.a, t.b + 1, t.c, t.y);
            case Direction.Right:
                if (t.PointsUp)
                    return GetTile(t.a - 1, t.b, t.c, t.y);
                else
                    return GetTile(t.a + 1, t.b, t.c, t.y);
            case Direction.Left:
                if (t.PointsUp)
                    return GetTile(t.a, t.b, t.c - 1, t.y);
                else
                    return GetTile(t.a, t.b, t.c + 1, t.y);
            case Direction.Top:
                return GetTile(t.a, t.b, t.c, t.y + 1);
            default: //Direction.Bottom
                return GetTile(t.a, t.b, t.c, t.y - 1);
        }
    }
}
