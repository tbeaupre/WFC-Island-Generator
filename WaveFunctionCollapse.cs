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

public class WaveFunctionCollapse
{
    public Dictionary<Triangle, Column> Collapse(List<Triangle> triangles, List<Prototype> prototypes, int height)
    {
        Dictionary<Triangle, Column> data = InitializeData(triangles, prototypes, height);

        while (!IsCollapsed(data))
        {
            Iterate(data, prototypes.Count);
        }

        return data;
    }

    Dictionary<Triangle, Column> InitializeData(List<Triangle> triangles, List<Prototype> prototypes, int height)
    {
        Dictionary<Triangle, Column> data = new Dictionary<Triangle, Column>();
        foreach (Triangle t in triangles)
        {
            data.Add(t, new Column(t, prototypes, height));
        }
        return data;
    }

    void Iterate(Dictionary<Triangle, Column> data, int maxEntropy)
    {
        Cell cell = GetMinEntropyCell(data, maxEntropy);
        cell.Collapse();
        Propagate(data, cell);
    }

    void Propagate(Dictionary<Triangle, Column> data, Cell cell)
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

                List<Prototype> possibleNeighbors = GetPossibleNeighbors(currentCell, dir);

                if (otherPossiblePrototypes.Count == 0)
                    continue;

                foreach (Prototype otherPrototype in otherPossiblePrototypes)
                {
                    if (!possibleNeighbors.Contains(otherPrototype))
                    {
                        neighbor.Constrain(otherPrototype);
                        if (!stack.Contains(neighbor))
                            stack.Push(neighbor);
                    }
                }
            }
        }
    }

    List<Prototype> GetPossibleNeighbors(Cell cell, Direction dir)
    {
        if (cell.prototypes.Count > 0)
        {
            switch (dir)
            {
                // Assumes cell has already been collapsed
                case Direction.Back:
                    return cell.prototypes[0].validNeighbors.back;
                case Direction.Right:
                    return cell.prototypes[0].validNeighbors.right;
                case Direction.Left:
                    return cell.prototypes[0].validNeighbors.left;
                case Direction.Top:
                    return cell.prototypes[0].validNeighbors.top;
                case Direction.Bottom:
                    return cell.prototypes[0].validNeighbors.bottom;
            }
        }
        return new List<Prototype>();
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

    bool IsCollapsed(Dictionary<Triangle, Column> data)
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
            int entropy = lowestEntropyCellInColumn.GetEntropy();
            if (entropy < lowestEntropyValue)
            {
                lowestEntropyValue = entropy;
                candidates = new List<Cell>();
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
            if (cell.GetEntropy() < lowestEntropyCell.GetEntropy())
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

    public void Constrain(Prototype prototype)
    {
        prototypes.Remove(prototype);
        Debug.Log(prototypes.Count);
    }
}