using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class TraversalManager
{
    static Dictionary<Tile, HashSet<Tile>> traversalMap = new Dictionary<Tile, HashSet<Tile>>();

    public static void Init()
    {
        traversalMap = new Dictionary<Tile, HashSet<Tile>>();
    }

    public static Prototype PickTraversalWeightedPrototype(Tile tile, List<Prototype> possibleProtos)
    {
        Dictionary<Prototype, HashSet<Tile>> protoTraversalMap = new Dictionary<Prototype, HashSet<Tile>>();

        foreach (Prototype p in possibleProtos)
        {
            protoTraversalMap.Add(p, GetAllReachableTiles(tile, p));
        }

        int sumOfWeights = 0;
        foreach (Prototype p in possibleProtos)
        {
            sumOfWeights += 1 + protoTraversalMap[p].Count;
        }
        int target = UnityEngine.Random.Range(0, sumOfWeights);
        int currentValue = 0;
        foreach (Prototype p in possibleProtos)
        {
            currentValue += 1 + protoTraversalMap[p].Count;
            if (currentValue > target)
            {
                AddPickToTraversalMap(tile, protoTraversalMap[p]);
                return p;
            }
        }

        return null;
    }

    public static void SetTilePrototype(Tile t, Prototype p)
    {
        AddPickToTraversalMap(t, GetAllReachableTiles(t, p));
    }

    static void AddPickToTraversalMap(Tile t, HashSet<Tile> reachableTiles)
    {
        foreach (Tile reachableTile in reachableTiles)
        {
            traversalMap[reachableTile].Add(t);
        }
        traversalMap.Add(t, reachableTiles);
    }

    static HashSet<Tile> GetAllReachableTiles(Tile t, Prototype p)
    {
        HashSet<Tile> reachableTiles = new HashSet<Tile>();
        if (p.traversalSet.back)
            reachableTiles.UnionWith(GetReachableTilesInDirection(t, Direction.Back));
        if (p.traversalSet.right)
            reachableTiles.UnionWith(GetReachableTilesInDirection(t, Direction.Right));
        if (p.traversalSet.left)
            reachableTiles.UnionWith(GetReachableTilesInDirection(t, Direction.Left));
        if (p.traversalSet.top)
            reachableTiles.UnionWith(GetReachableTilesInDirection(t, Direction.Top));
        if (p.traversalSet.bottom)
            reachableTiles.UnionWith(GetReachableTilesInDirection(t, Direction.Bottom));
        return reachableTiles;
    }

    static HashSet<Tile> GetReachableTilesInDirection(Tile t, Direction direction)
    {
        Tile neighbor = TileGrid.GetNeighbor(t, direction);
        if (!(neighbor is null) && traversalMap.ContainsKey(neighbor))
            return traversalMap[neighbor];
        return new HashSet<Tile>();
    }
}
