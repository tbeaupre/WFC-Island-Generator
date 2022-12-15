using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;

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
    Dictionary<Tile, Cell> data;
    TileGrid tileGrid;
    List<Prototype> prototypes;
    int height;

    public delegate void StartIsland();
    public static event StartIsland OnStartIsland;

    public delegate void FinishIsland();
    public static event FinishIsland OnFinishIsland;

    public IEnumerator CollapseCo(TileGrid tileGrid, List<Prototype> prototypes, int height, float timeBetweenSteps, Action<Dictionary<Tile, Cell>, bool> callback)
    {
        this.tileGrid = tileGrid;
        this.prototypes = prototypes;
        this.height = height;

        InitializeDataStructure();

        OnStartIsland?.Invoke();

        while (!IsCollapsed())
        {
            if (!Iterate())
            {
                Debug.Log("Iteration failed. Trying again...");
                InitializeDataStructure();
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
        foreach (Tile t in tileGrid.tileMap.Values)
        {
            data.Add(t, new Cell(t, prototypes));
        }
        TraversalManager.Init(tileGrid);
        OceanHelper.Init(tileGrid, data);
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

        while (stack.Count > 0)
        {
            Cell currentCell = stack.Pop();

            var inputs = new NativeArray<ArrayTestJob.Input>(3, Allocator.TempJob);
            var outputs = new NativeArray<ArrayTestJob.Output>(3, Allocator.TempJob);

            var input = new ArrayTestJob.Input();
            input.ints = 1;
            inputs[0] = input;
            inputs[1] = input;
            inputs[2] = input;

            ArrayTestJob jobData = new ArrayTestJob();
            jobData.inputs = inputs;
            jobData.outputs = outputs;

            JobHandle handle = jobData.Schedule(outputs.Length, 1);

            // Wait for the job to complete
            handle.Complete();

            for (int i = 0; i < outputs.Length; ++i)
            {
                Debug.Log(outputs[i].ints);
            }

            // Free the memory allocated by the arrays
            inputs.Dispose();
            outputs.Dispose();

            //var input = new NativeArray<PropagateJob.PropagateInput>(5, Allocator.TempJob);
            //var output = new NativeArray<PropagateJob.PropagateOutput>(5, Allocator.TempJob);

            //Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
            //Cell[] neighbors = new Cell[directions.Length];
            //for (int i = 0; i < directions.Length; ++i)
            //{
            //    var propagateInput = new PropagateJob.PropagateInput();
            //    propagateInput.dir = directions[i];
            //    propagateInput.cell = currentCell;

            //    Cell neighbor = GetNeighborCell(currentCell, directions[i]);
            //    neighbors[i] = neighbor;
            //    if (neighbor is null)
            //        propagateInput.neighborPrototypes = new List<Prototype>();
            //    else
            //        propagateInput.neighborPrototypes = neighbor.prototypes;

            //    input[i] = propagateInput;
            //}

            //PropagateJob jobData = new PropagateJob();
            //jobData.input = input;
            //jobData.output = output;

            //// Schedule the job with one Execute per index in the results array and only 1 item per processing batch
            //JobHandle handle = jobData.Schedule(output.Length, 1);

            //// Wait for the job to complete
            //handle.Complete();

            //for (int i = 0; i < output.Length; ++i)
            //{
            //    if (neighbors[i] is null)
            //        continue;

            //    if (!output[i].wasSuccessful)
            //        return false;

            //    if (!output[i].shouldPropagateToNeighbor)
            //        continue;

            //    neighbors[i].prototypes = output[i].neighborPrototypes;
            //    if (!stack.Contains(neighbors[i]))
            //        stack.Push(neighbors[i]);
            //}

            //// Free the memory allocated by the arrays
            //input.Dispose();
            //output.Dispose();
        }
        return true;
    }

    HashSet<string> GetPossibleNeighbors(Cell cell, Direction dir)
    {
        HashSet<string> possibleNeighbors = new HashSet<string>();
        foreach (Prototype p in cell.prototypes)
        {
            switch (dir)
            {
                case Direction.Back:
                    possibleNeighbors.UnionWith(p.validNeighbors.back);
                    break;
                case Direction.Right:
                    possibleNeighbors.UnionWith(p.validNeighbors.right);
                    break;
                case Direction.Left:
                    possibleNeighbors.UnionWith(p.validNeighbors.left);
                    break;
                case Direction.Top:
                    possibleNeighbors.UnionWith(p.validNeighbors.top);
                    break;
                case Direction.Bottom:
                    possibleNeighbors.UnionWith(p.validNeighbors.bottom);
                    break;
            }
        }
        return possibleNeighbors;
    }

    Cell GetNeighborCell(Cell cell, Direction dir)
    {
        Tile neighborTile = tileGrid.GetNeighbor(cell.tile, dir);
        if (neighborTile is not null)
            return data[neighborTile];
        return null;
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