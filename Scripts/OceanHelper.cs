using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OceanHelper
{
    static TileGrid tileGrid;
    static Dictionary<Tile, Cell> data;

    public static void Init(TileGrid newTg, Dictionary<Tile, Cell> newData)
    {
        tileGrid = newTg;
        data = newData;
    }

    // Treat edges of the map like they're OOO-EEE
    public static List<Prototype> ReduceForEdges(Tile tile, List<Prototype> prototypes)
    {
        if (tile.y != 0)
            return prototypes;

        List<Prototype> result = new List<Prototype>(prototypes);
        Direction[] sides = new Direction[] { Direction.Back, Direction.Right, Direction.Left };
        foreach (Direction dir in sides)
        {
            if (tileGrid.GetNeighbor(tile, dir) is null)
            {
                result = result.Where(p => IsFullOceanPrototypeInDirection(p, dir)).ToList();
            }
        }
        return result;
    }

    public static List<Prototype> ReduceOceans(Tile tile, List<Prototype> prototypes)
    {
        List<Prototype> result;
        List<Prototype> noEmptyOcean = new List<Prototype>(prototypes);
        noEmptyOcean.RemoveAll(p => p.name == "OOO-EEE");
        if (noEmptyOcean.Count == 0)
            return prototypes;

        Direction[] sides = new Direction[] { Direction.Back, Direction.Right, Direction.Left };
        foreach (Direction dir in sides)
        {
            Tile neighborTile = tileGrid.GetNeighbor(tile, dir);
            if (neighborTile is null)
                continue;
            if (IsOceanCell(data[neighborTile], dir))
            {
                return noEmptyOcean;
            }
        }

        result = prototypes.Where(p => !IsOceanPrototype(p)).ToList();
        return result.Count == 0 ? noEmptyOcean : result;
    }

    static bool IsOceanCell(Cell cell, Direction dir)
    {
        if (!cell.IsCollapsed)
            return false;

        return IsOceanPrototypeInDirection(cell.prototypes[0], dir);
    }

    static bool IsOceanPrototypeInDirection(Prototype p, Direction dir)
    {
        if (p.meshName.Length == 0)
            return false;

        string baseStr = p.meshName.Split('-')[0];

        switch (dir)
        {
            case Direction.Back:
                return baseStr[(-p.rotation + 3) % 3] == 'O' || baseStr[(1 - p.rotation + 3) % 3] == 'O';
            case Direction.Right:
                return baseStr[(1 -p.rotation + 3) % 3] == 'O' || baseStr[(2 - p.rotation + 3) % 3] == 'O';
            default: // Direction.Left:
                return baseStr[(2 - p.rotation + 3) % 3] == 'O' || baseStr[(-p.rotation + 3) % 3] == 'O';
        }
    }

    static bool IsFullOceanPrototypeInDirection(Prototype p, Direction dir)
    {
        if (p.meshName.Length == 0)
            return false;

        string baseStr = p.meshName.Split('-')[0];
        string cliffStr = p.meshName.Split('-')[1];

        switch (dir)
        {
            case Direction.Back:
                return baseStr[(-p.rotation + 3) % 3] == 'O' && baseStr[(1 - p.rotation + 3) % 3] == 'O' &&
                    cliffStr[(-p.rotation + 3) % 3] == 'E' && cliffStr[(1 - p.rotation + 3) % 3] == 'E';
            case Direction.Right:
                return baseStr[(1 - p.rotation + 3) % 3] == 'O' && baseStr[(2 - p.rotation + 3) % 3] == 'O' &&
                    cliffStr[(1 - p.rotation + 3) % 3] == 'E' && cliffStr[(2 - p.rotation + 3) % 3] == 'E';
            default: // Direction.Left:
                return baseStr[(2 - p.rotation + 3) % 3] == 'O' && baseStr[(-p.rotation + 3) % 3] == 'O' &&
                    cliffStr[(2 - p.rotation + 3) % 3] == 'E' && cliffStr[(-p.rotation + 3) % 3] == 'E';
        }
    }

    public static bool IsOceanPrototype(Prototype p)
    {
        if (p.meshName.Length == 0)
            return false;

        string baseStr = p.meshName.Split('-')[0];
        if (baseStr.Contains('O'))
            return true;

        return false;
    }
}
