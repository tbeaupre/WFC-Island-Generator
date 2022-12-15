using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public enum Direction
{
    Back,
    Right,
    Left,
    Top,
    Bottom
}

// TODO: Consider edge cells as having lower entropy. Work from outside in.
// TODO: Make edge cells FFF? Should make the island look a bit nicer.
public class WaveFunctionCollapse
{
    public static Dictionary<Tile, Cell> data;
    List<Prototype> prototypes;

    public delegate void StartIsland();
    public static event StartIsland OnStartIsland;

    public delegate void FinishIsland();
    public static event FinishIsland OnFinishIsland;

    public static List<Prototype> baseLevelPrototypes;
    public static List<Prototype> topLevelPrototypes;
    public static List<Prototype> noOceans;

    public IEnumerator CollapseCo(List<Prototype> prototypes, float timeBetweenSteps, Action<Dictionary<Tile, Cell>, bool> callback)
    {
        this.prototypes = prototypes;
        baseLevelPrototypes = prototypes.Where(p => MeshNameUtilities.IsBaseLevelPrototype(p)).ToList();
        topLevelPrototypes = prototypes.Where(p => MeshNameUtilities.IsTopLevelPrototype(p)).ToList();
        noOceans = prototypes.Where(p => !OceanHelper.IsOceanPrototype(p)).ToList();

        InitializeDataStructure();

        OnStartIsland?.Invoke();

        Debug.Log("Starting");

        while (!IsCollapsed())
        {
            if (!Iterate())
            {
                Debug.Log("Iteration failed. Trying again...");
                ResetData();
                //callback(data, false);
                //yield break;
            }
            callback(data, false);
            yield return new WaitForSeconds(timeBetweenSteps);
        }

        OnFinishIsland?.Invoke();
    }

    void InitializeDataStructure()
    {
        CellManager.Clear();
        data = new Dictionary<Tile, Cell>();
        foreach (Tile t in TileGrid.tileMap.Values)
        {
            data.Add(t, new Cell(t));
        }
        TraversalManager.Init();
    }

    void ResetData()
    {
        foreach (Cell cell in data.Values)
        {
            cell.Reset();
        }
        TraversalManager.Init();
        CellManager.Clear();
    }

    public bool Iterate()
    {
        Cell cell = GetNextCell();
        // Debug.Log($"{cell.tile.a}, {cell.tile.b}, {cell.tile.c}, {cell.tile.y}");
        cell.Collapse();
        return Propagate(cell);
    }

    bool Propagate(Cell cell)
    {
        Stack<Cell> stack = new Stack<Cell>();
        stack.Push(cell);
        int iterations = 1;
        HashSet<Cell> cellsAttempted = new HashSet<Cell>();

        Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
        List<Task> tasks = new List<Task>();
        Cell[] neighbors = new Cell[directions.Length];
        ConcurrentBag<Output> outputs = new ConcurrentBag<Output>();


        while (stack.Count > 0)
        {
            Cell currentCell = stack.Pop();
            ++iterations;
            cellsAttempted.Add(currentCell);

            HashSet<string>[] possibleNeighborSets = GetPossibleNeighbors(currentCell);
            tasks.Clear();
            outputs.Clear();

            neighbors = currentCell.GetNeighbors();
            if (neighbors[0] is not null)
                tasks.Add(Task.Factory.StartNew(() => outputs.Add(PropagateToNeighbors(0, possibleNeighborSets[0], neighbors[0].prototypes))));
            if (neighbors[1] is not null)
                tasks.Add(Task.Factory.StartNew(() => outputs.Add(PropagateToNeighbors(1, possibleNeighborSets[1], neighbors[1].prototypes))));
            if (neighbors[2] is not null)
                tasks.Add(Task.Factory.StartNew(() => outputs.Add(PropagateToNeighbors(2, possibleNeighborSets[2], neighbors[2].prototypes))));
            if (neighbors[3] is not null)
                tasks.Add(Task.Factory.StartNew(() => outputs.Add(PropagateToNeighbors(3, possibleNeighborSets[3], neighbors[3].prototypes))));
            if (neighbors[4] is not null)
                tasks.Add(Task.Factory.StartNew(() => outputs.Add(PropagateToNeighbors(4, possibleNeighborSets[4], neighbors[4].prototypes))));

            Task.WaitAll(tasks.ToArray());

            foreach (Output output in outputs)
            {
                if (neighbors[output.i] is null)
                    continue;

                if (!output.wasSuccessful)
                    return false;

                if (!output.shouldPropagateToNeighbor)
                    continue;

                Cell neighbor = neighbors[output.i];
                if (!stack.Contains(neighbor))
                    stack.Push(neighbor);
            }
        }

        Debug.Log($"{iterations} iterations updating {cellsAttempted.Count} of {data.Values.Count} cells");

        return true;
    }

    class Output
    {
        public int i;
        public bool wasSuccessful;
        public bool shouldPropagateToNeighbor;

        public Output(int i)
        {
            this.i = i;
            this.wasSuccessful = false;
            this.shouldPropagateToNeighbor = false;
        }
    }

    static Output PropagateToNeighbors(int i, HashSet<string> possibleNeighbors, List<Prototype> neighborPrototypes)
    {
        Output output = new Output(i);

        if (neighborPrototypes.Count == 0)
            return output; // by default wasSuccessful is false

        foreach (Prototype otherPrototype in neighborPrototypes.ToArray())
        {
            if (!possibleNeighbors.Contains(otherPrototype.name))
            {
                neighborPrototypes.Remove(otherPrototype);
                if (neighborPrototypes.Count == 0)
                    return output; // by default wasSuccessful is false
                output.shouldPropagateToNeighbor = true;
            }
        }

        output.wasSuccessful = true;
        return output;
    }

    HashSet<string>[] GetPossibleNeighbors(Cell cell)
    {
        var result = new HashSet<string>[5];
        for (int i = 0; i < 5; ++i)
        {
            result[i] = new HashSet<string>();
        }

        foreach (Prototype p in cell.prototypes)
        {
            result[0].UnionWith(p.validNeighbors.back);
            result[1].UnionWith(p.validNeighbors.right);
            result[2].UnionWith(p.validNeighbors.left);
            result[3].UnionWith(p.validNeighbors.top);
            result[4].UnionWith(p.validNeighbors.bottom);
        }
        return result;
    }

    public bool IsCollapsed()
    {
        foreach(Cell cell in data.Values)
        {
            if (!cell.IsCollapsed)
                return false;
        }
        return true;
    }

    Cell GetNextCell()
    {
        List<Cell> candidates = new List<Cell>(data.Values);
        candidates = candidates.Where(c => c.prototypes.Count != 1).ToList();

        candidates = GetLowestCells(candidates);
        candidates = GetFurthestCells(candidates);
        candidates = GetMinEntropyCells(candidates);

        // Randomly choose one from candidates and return it
        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    List<Cell> GetMinEntropyCells(List<Cell> cells)
    {
        int lowestEntropyValue = prototypes.Count;
        List<Cell> candidates = new List<Cell>();

        foreach (Cell cell in cells)
        {
            if (cell.Entropy < lowestEntropyValue)
            {
                lowestEntropyValue = cell.Entropy;
                candidates.Clear();
                candidates.Add(cell);
                continue;
            }
            if (cell.Entropy == lowestEntropyValue)
                candidates.Add(cell);
        }

        return candidates;
    }

    List<Cell> GetLowestCells(List<Cell> cells)
    {
        List<Cell> candidates = new List<Cell> { cells[0] };
        int lowestY = cells[0].tile.y;

        foreach (Cell cell in cells)
        {
            if (cell == cells[0])
                continue;
            if (cell.tile.y < lowestY)
            {
                lowestY = cell.tile.y;
                candidates.Clear();
                candidates.Add(cell);
                continue;
            }
            if (cell.tile.y == lowestY)
                candidates.Add(cell);
        }

        return candidates;
    }

    List<Cell> GetFurthestCells(List<Cell> cells)
    {
        List<Cell> candidates = new List<Cell> { cells[0] };
        float furthestDist = cells[0].tile.TileCenterToOrigin();

        foreach (Cell cell in cells)
        {
            if (cell == cells[0])
                continue;
            float dist = cell.tile.TileCenterToOrigin();
            if (dist > furthestDist)
            {
                furthestDist = dist;
                candidates.Clear();
                candidates.Add(cell);
                continue;
            }
            if (dist == furthestDist)
                candidates.Add(cell);
        }

        return candidates;
    }
}