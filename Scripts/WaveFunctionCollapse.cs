using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Dictionary<Tile, Cell> data = new Dictionary<Tile, Cell>();
    TileGrid tileGrid;
    List<Prototype> prototypes;
    int height;

    public Dictionary<Tile, Cell> Collapse(TileGrid tileGrid, List<Prototype> prototypes, int height)
    {
        this.tileGrid = tileGrid;
        this.prototypes = prototypes;
        this.height = height;

        InitializeData();

        while (!IsCollapsed())
        {
            if (!Iterate(prototypes.Count))
            {
                Debug.Log("Iteration failed. Trying again...");
                InitializeData();
            }
        }

        return data;
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
                    cell.CollapseTo("OOO");
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
            cell.CollapseTo("EEE");
            if (!Propagate(cell))
                return false;
        }
        return true;
    }

    public bool Iterate(int maxEntropy)
    {
        Cell cell = GetMinEntropyCell(maxEntropy);
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
                            return false;
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

    Cell GetMinEntropyCell(int maxEntropy)
    {
        int lowestEntropyValue = maxEntropy;
        List<Cell> candidates = new List<Cell>();

        foreach (Cell cell in data.Values)
        {
            if (cell.Entropy == 1) // Ignore already collapsed cells
                continue;
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

        candidates = GetLowestCells(candidates);
        candidates = GetFurthestCells(candidates);

        // Randomly choose one from candidates and return it
        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    List<Cell> GetLowestCells(List<Cell> cells)
    {
        List<Cell> candidates = new List<Cell> { cells[0] };
        int lowestY = cells[0].tile.y;

        foreach (Cell cell in cells)
        {
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
            int dist = cell.tile.DistanceTo(0, 0, 0);
            if (dist < furthestDist)
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

public class Cell
{
    const int TRAVERSAL_MULTIPLIER = 5;

    public Tile tile;
    public List<Prototype> prototypes;

    public Cell(Tile tile, List<Prototype> prototypes)
    {
        this.tile = tile;
        this.prototypes = new List<Prototype>(prototypes);
    }

    public bool IsCollapsed => prototypes.Count == 1;
    public int Entropy => prototypes.Count;

    public void Collapse()
    {
        Prototype proto = prototypes[UnityEngine.Random.Range(0, prototypes.Count)];
        prototypes = new List<Prototype>();
        prototypes.Add(proto);

        //int sumOfWeights = 0;
        //foreach (Prototype p in prototypes)
        //{
        //    sumOfWeights += (1 + (p.traversalScore * TRAVERSAL_MULTIPLIER));
        //}
        //int target = UnityEngine.Random.Range(0, sumOfWeights);
        //int currentValue = 0;
        //foreach (Prototype p in prototypes)
        //{
        //    currentValue += 1 + (p.traversalScore * TRAVERSAL_MULTIPLIER);
        //    if (currentValue > target)
        //    {
        //        prototypes = new List<Prototype>();
        //        prototypes.Add(p);
        //        return;
        //    }
        //}
    }

    public void CollapseTo(string prototypeName)
    {
        Prototype proto = prototypes.Find(p => p.name == prototypeName);
        prototypes = new List<Prototype>();
        prototypes.Add(proto);
    }

    public void Constrain(Prototype prototype)
    {
        prototypes.Remove(prototype);
    }
}