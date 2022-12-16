using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cell
{
    public Tile tile;
    public List<int> prototypes;
    private Cell[] neighbors;

    public Cell(Cell cell)
    {
        this.tile = cell.tile;
        this.prototypes = new List<int>(cell.prototypes);
    }

    public Cell(Tile tile)
    {
        this.tile = tile;
        if (tile.y == 0)
            this.prototypes = WaveFunctionCollapse.baseLevelPrototypes.Select(p => p.id).ToList();
        else if (tile.y == Main.height - 1)
            this.prototypes = WaveFunctionCollapse.topLevelPrototypes.Select(p => p.id).ToList();
        else
            this.prototypes = WaveFunctionCollapse.noOceans.Select(p => p.id).ToList();
    }

    public bool IsCollapsed => prototypes.Count == 1;
    public int Entropy => prototypes.Count;

    public Cell[] GetNeighbors()
    {
        if (neighbors == null)
            InitNeighbors();
        return neighbors;
    }

    private void InitNeighbors()
    {
        Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
        neighbors = new Cell[directions.Length];
        foreach (Direction dir in directions)
        {
            Tile neighborTile = TileGrid.GetNeighbor(tile, dir);
            if (neighborTile is not null)
                neighbors[(int)dir] = WaveFunctionCollapse.data[neighborTile];
        }
    }

    public void Collapse()
    {
        Prototype selected = PrototypePicker.PickPrototype(this);
        prototypes = new List<int> { selected.id };
    }
}
