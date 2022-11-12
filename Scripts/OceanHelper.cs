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

    static bool IsOceanPrototype(Prototype p)
    {
        if (p.meshName.Length == 0)
            return false;

        string baseStr = p.meshName.Split('-')[0];
        if (baseStr.Contains('O'))
            return true;

        return false;
    }
}
