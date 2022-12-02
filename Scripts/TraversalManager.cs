using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

        List<Prototype> possibleProtos = OceanHelper.ReduceForEdges(cell.tile, cell.prototypes);
        if (possibleProtos.Count == cell.prototypes.Count)
        {
            Prototype internalProto = cell.prototypes.Find(p => p.name == "III");
            if (internalProto != null)
            {
                traversalMap.Add(cell.tile, new HashSet<Tile>());
                return internalProto;
            }
        }
        possibleProtos = OceanHelper.ReduceOceans(cell.tile, possibleProtos);
        possibleProtos = MountainHelper.RemoveLocalMinima(cell.tile, possibleProtos);

        if (cell.tile.y == 0) // Don't leave holes in the map.
        {
            possibleProtos = possibleProtos.Where(p => !IsPrototypeOpenOnBottom(p)).ToList();
            if (possibleProtos.Count == 0)
                Debug.Log(cell.prototypes);
        }

        foreach (Prototype p in possibleProtos)
        {
            protoTraversalMap.Add(p, GetAllReachableTiles(cell.tile, p));
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

    private bool IsPrototypeOpenOnBottom(Prototype p)
    {
        if (p.meshName.Length == 0)
            return p.name == "EEE";

        string baseStr = p.meshName.Split('-')[0];
        if (baseStr.Contains('E'))
            return true;

        return false;
    }
}
