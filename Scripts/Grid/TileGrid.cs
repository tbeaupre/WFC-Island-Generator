using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileGrid
{
    public List<Tile> tiles = new List<Tile>();

    public TileGrid(int radius, int height)
    {
        int min = -radius;
        int max = radius;
        for (int a = min; a <= max; ++a)
        {
            for (int b = min; b <= max; ++b)
            {
                for (int c = min; c <= max; ++c)
                {
                    Tile testTile = new Tile(a, b, c, 0);
                    if (testTile.IsValid() && testTile.AltDistanceTo(0, 0, 0) <= radius)
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            Tile tile = new Tile(a, b, c, y);
                            tiles.Add(tile);
                        }
                    }
                }
            }
        }
    }

    public Tile GetTile(int a, int b, int c, int y) => tiles.FirstOrDefault(t => t.PositionalMatch(a, b, c, y));

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
