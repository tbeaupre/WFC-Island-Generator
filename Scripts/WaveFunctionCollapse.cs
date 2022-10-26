using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    TraversalManager tm;

    public IEnumerator CollapseCo(TileGrid tileGrid, List<Prototype> prototypes, int height, float timeBetweenSteps, Action<Dictionary<Tile, Cell>, bool> callback)
    {
        this.tileGrid = tileGrid;
        this.prototypes = prototypes;
        this.height = height;

        InitializeData();

        while (!IsCollapsed())
        {
            if (!Iterate())
            {
                Debug.Log("Iteration failed. Trying again...");
                callback(data, true);
                yield break;
                InitializeData();
            }
            callback(data, false);
            yield return new WaitForSeconds(timeBetweenSteps);
        }

    }

    public void InitializeData()
    {
        InitializeDataStructure();

        while (!SetUpOceansAndSkies())
        {
            Debug.Log("Failed to initialize oceans and skies. Trying again...");
            InitializeDataStructure();
        }
    }

    void InitializeDataStructure()
    {
        data = new Dictionary<Tile, Cell>();
        foreach (Tile t in tileGrid.tiles)
        {
            data.Add(t, new Cell(t, prototypes));
        }
        tm = new TraversalManager(tileGrid);
    }

    bool SetUpOceansAndSkies()
    {
        if (!CreateOceans())
            return false;
        if (!CreateSkies())
            return false;
        return true;
    }

    bool CreateOceans()
    {
        foreach (Cell cell in data.Values)
        {
            if (cell.tile.y > 0)
                continue;
            Direction[] sides = new Direction[] { Direction.Back, Direction.Right, Direction.Left };
            foreach (Direction dir in sides)
            {
                if (GetNeighborCell(cell, dir) is null)
                {
                    cell.CollapseTo(tm, "OOO-EEE");
                    if (!Propagate(cell))
                        return false;
                    break;
                }
            }
        }
        return true;
    }

    bool CreateSkies()
    {
        foreach (Cell cell in data.Values)
        {
            if (cell.tile.y < height - 1)
                continue;
            cell.CollapseTo(tm, "EEE");
            if (!Propagate(cell))
                return false;
        }
        return true;
    }

    public bool Iterate()
    {
        Cell cell = GetNextCell();
        // Debug.Log($"{cell.tile.a}, {cell.tile.b}, {cell.tile.c}, {cell.tile.y}");
        cell.Collapse(tm);
        return Propagate(cell);
    }

    bool Propagate(Cell cell)
    {
        Stack<Cell> stack = new Stack<Cell>();
        stack.Push(cell);

        while (stack.Count > 0)
        {
            Cell currentCell = stack.Pop();

            Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
            foreach (Direction dir in directions)
            {
                Cell neighbor = GetNeighborCell(currentCell, dir);
                if (neighbor is null)
                    continue;

                List<Prototype> otherPossiblePrototypes = new List<Prototype>(neighbor.prototypes);

                HashSet<string> possibleNeighbors = GetPossibleNeighbors(currentCell, dir);

                if (otherPossiblePrototypes.Count == 0)
                    continue;

                foreach (Prototype otherPrototype in otherPossiblePrototypes)
                {
                    if (!possibleNeighbors.Contains(otherPrototype.name))
                    {
                        neighbor.Constrain(otherPrototype);
                        if (neighbor.prototypes.Count == 0)
                        {
                            Debug.Log($"{neighbor.tile.a}, {neighbor.tile.b}, {neighbor.tile.c}, {neighbor.tile.y}");
                            return false;
                        }
                        if (!stack.Contains(neighbor))
                            stack.Push(neighbor);
                    }
                }
            }
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
        int furthestDist = cells[0].tile.DistanceTo(0, 0, 0);

        foreach (Cell cell in cells)
        {
            if (cell == cells[0])
                continue;
            int dist = cell.tile.DistanceTo(0, 0, 0);
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