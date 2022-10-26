using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversalManager
{
    TileGrid tileGrid;
    Dictionary<Tile, HashSet<Tile>> traversalMap;

    public TraversalManager(TileGrid tileGrid)
    {
        this.tileGrid = tileGrid;
        traversalMap = new Dictionary<Tile, HashSet<Tile>>();
    }

    public Prototype PickTraversalWeightedPrototype(Cell cell)
    {
        Dictionary<Prototype, HashSet<Tile>> protoTraversalMap = new Dictionary<Prototype, HashSet<Tile>>();

        foreach (Prototype p in cell.prototypes)
        {
            protoTraversalMap.Add(p, GetAllReachableTiles(cell.tile, p));
        }

        int sumOfWeights = 0;
        foreach (Prototype p in cell.prototypes)
        {
            sumOfWeights += 1 + protoTraversalMap[p].Count;
        }
        int target = UnityEngine.Random.Range(0, sumOfWeights);
        int currentValue = 0;
        foreach (Prototype p in cell.prototypes)
        {
            currentValue += 1 + protoTraversalMap[p].Count;
            if (currentValue > target)
            {
                AddPickToTraversalMap(cell.tile, protoTraversalMap[p]);
                return p;
            }
        }

        return null;
    }

    public void SetTilePrototype(Tile t, Prototype p)
    {
        AddPickToTraversalMap(t, GetAllReachableTiles(t, p));
    }

    List<Prototype> GetMostTraversableOptions(Cell cell)
    {
        int highestTraversalValue = 0;
        List<Prototype> candidates = new List<Prototype>();

        foreach (Prototype p in cell.prototypes)
        {
            HashSet<Tile> reachableTiles = GetAllReachableTiles(cell.tile, p);
            if (reachableTiles.Count > highestTraversalValue)
            {
                highestTraversalValue = reachableTiles.Count;
                candidates.Clear();
                candidates.Add(p);
                continue;
            }
            if (reachableTiles.Count == highestTraversalValue)
                candidates.Add(p);
        }

        return candidates;
    }

    void AddPickToTraversalMap(Tile t, HashSet<Tile> reachableTiles)
    {
        foreach (Tile reachableTile in reachableTiles)
        {
            traversalMap[reachableTile].Add(t);
        }
        traversalMap.Add(t, reachableTiles);
    }

    HashSet<Tile> GetAllReachableTiles(Tile t, Prototype p)
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

    HashSet<Tile> GetReachableTilesInDirection(Tile t, Direction direction)
    {
        Tile neighbor = tileGrid.GetNeighbor(t, direction);
        if (!(neighbor is null) && traversalMap.ContainsKey(neighbor))
            return traversalMap[neighbor];
        return new HashSet<Tile>();
    }
}
