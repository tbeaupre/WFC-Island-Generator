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
    public Dictionary<Triangle, Column> Collapse(List<Triangle> triangles, List<Prototype> prototypes, int height)
    {
        Dictionary<Triangle, Column> data = InitializeData(triangles, prototypes, height);

        while (!IsCollapsed(data))
        {
            if (!Iterate(data, prototypes.Count))
            {
                Debug.Log("Iteration failed. Trying again...");
                data = InitializeData(triangles, prototypes, height);
            }
        }

        return data;
    }

    public Dictionary<Triangle, Column> InitializeData(List<Triangle> triangles, List<Prototype> prototypes, int height)
    {
        Dictionary<Triangle, Column> data = InitializeDataStructure(triangles, prototypes, height);

        while (!SetUpOceansAndSkies(data, height))
        {
            Debug.Log("Failed to initialize oceans and skies. Trying again...");
            data = InitializeDataStructure(triangles, prototypes, height);
        }

        return data;
    }

    Dictionary<Triangle, Column> InitializeDataStructure(List<Triangle> triangles, List<Prototype> prototypes, int height)
    {
        Dictionary<Triangle, Column> data = new Dictionary<Triangle, Column>();
        foreach (Triangle t in triangles)
        {
            data.Add(t, new Column(t, prototypes, height));
        }
        return data;
    }

    bool SetUpOceansAndSkies(Dictionary<Triangle, Column> data, int height)
    {
        if (!CreateOceans(data))
            return false;
        if (!CreateSkies(data, height))
            return false;
        return true;
    }

    bool CreateOceans(Dictionary<Triangle, Column> data)
    {
        foreach (Column column in data.Values)
        {
            Cell bottomCell = column.GetCellAtY(0);
            Direction[] sides = new Direction[] { Direction.Back, Direction.Right, Direction.Left };
            foreach (Direction dir in sides)
            {
                if (GetNeighborCell(data, bottomCell, dir) is null)
                {
                    bottomCell.CollapseTo("OOO");
                    if (!Propagate(data, bottomCell))
                        return false;
                    break;
                }
            }
        }
        return true;
    }

    bool CreateSkies(Dictionary<Triangle, Column> data, int height)
    {
        foreach (Column column in data.Values)
        {
            Cell topCell = column.GetCellAtY(height - 1);
            topCell.CollapseTo("EEE");
            if (!Propagate(data, topCell))
                return false;
        }
        return true;
    }

    public bool Iterate(Dictionary<Triangle, Column> data, int maxEntropy)
    {
        Cell cell = GetMinEntropyCell(data, maxEntropy);
        cell.Collapse();
        return Propagate(data, cell);
    }

    bool Propagate(Dictionary<Triangle, Column> data, Cell cell)
    {
        Stack<Cell> stack = new Stack<Cell>();
        stack.Push(cell);

        while (stack.Count > 0)
        {
            Cell currentCell = stack.Pop();

            Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
            foreach (Direction dir in directions)
            {
                Cell neighbor = GetNeighborCell(data, currentCell, dir);
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

    Cell GetNeighborCell(Dictionary<Triangle, Column> data, Cell cell, Direction dir)
    {
        Triangle neighbor;
        switch (dir)
        {
            case Direction.Back:
                neighbor = cell.triangle.backNeighbor;
                if (!(neighbor is null))
                    return data[neighbor].GetCellAtY(cell.y);
                break;
            case Direction.Right:
                neighbor = cell.triangle.rightNeighbor;
                if (!(neighbor is null))
                    return data[neighbor].GetCellAtY(cell.y);
                break;
            case Direction.Left:
                neighbor = cell.triangle.leftNeighbor;
                if (!(neighbor is null))
                    return data[neighbor].GetCellAtY(cell.y);
                break;
            case Direction.Top:
                if (cell.y != cell.column.height - 1)
                    return cell.column.GetCellAtY(cell.y + 1);
                break;
            case Direction.Bottom:
                if (cell.y != 0)
                    return cell.column.GetCellAtY(cell.y - 1);
                break;
        }

        return null;
    }

    public bool IsCollapsed(Dictionary<Triangle, Column> data)
    {
        foreach(Column column in data.Values)
        {
            if (!column.IsCollapsed())
                return false;
        }
        return true;
    }

    Cell GetMinEntropyCell(Dictionary<Triangle, Column> data, int maxEntropy)
    {
        int lowestEntropyValue = maxEntropy;
        List<Cell> candidates = new List<Cell>();

        foreach (Column column in data.Values)
        {
            Cell lowestEntropyCellInColumn = column.CalcMinEntropyCell();
            //Debug.Log("LowestEntropyInCell: " + lowestEntropyCellInColumn.GetEntropy());
            int entropy = lowestEntropyCellInColumn.GetEntropy();
            if (entropy == 1) // Ignore already collapsed cells
                continue;
            if (entropy < lowestEntropyValue)
            {
                lowestEntropyValue = entropy;
                candidates.Clear();
                candidates.Add(lowestEntropyCellInColumn);
                continue;
            }
            if (entropy == lowestEntropyValue)
                candidates.Add(lowestEntropyCellInColumn);
        }

        // Randomly choose one from candidates and return it
        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }
}

public class Column
{
    Triangle triangle;
    public List<Cell> cells;
    public int height;

    public Column(Triangle triangle, List<Prototype> prototypes, int height)
    {
        this.triangle = triangle;
        this.height = height;

        this.cells = new List<Cell>();
        for (int i = 0; i < height; ++i)
        {
            this.cells.Add(new Cell(triangle, this, prototypes, i));
        }
    }

    public Cell GetCellAtY(int y)
    {
        return cells[y];
    }

    public bool IsCollapsed()
    {
        foreach (Cell cell in cells)
        {
            if (cell.GetEntropy() > 1)
                return false;
        }
        return true;
    }

    public Cell CalcMinEntropyCell()
    {
        Cell lowestEntropyCell = cells[0];
        foreach (Cell cell in cells)
        {
            // Can't include already collapsed cells.
            if (lowestEntropyCell.GetEntropy() == 1)
            {
                lowestEntropyCell = cell;
                continue;
            }
            if (cell.GetEntropy() > 1 && cell.GetEntropy() < lowestEntropyCell.GetEntropy())
                lowestEntropyCell = cell;
        }
        return lowestEntropyCell;
    }
}

public class Cell
{
    public Triangle triangle;
    public Column column;
    public List<Prototype> prototypes;
    public int y;

    public Cell(Triangle triangle, Column column, List<Prototype> prototypes, int y)
    {
        this.triangle = triangle;
        this.column = column;
        this.prototypes = new List<Prototype>(prototypes);
        this.y = y;
    }

    public int GetEntropy()
    {
        return prototypes.Count;
    }

    public void Collapse()
    {
        Prototype proto = prototypes[UnityEngine.Random.Range(0, prototypes.Count)];
        prototypes = new List<Prototype>();
        prototypes.Add(proto);
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