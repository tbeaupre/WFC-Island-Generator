using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Tile tile;
    public List<Prototype> prototypes;
    private Cell[] neighbors;

    public Cell(Tile tile)
    {
        this.tile = tile;
        Reset();
    }

    public bool IsCollapsed => prototypes.Count == 1;
    public int Entropy => prototypes.Count;

    public void Reset()
    {
        if (tile.y == 0)
            this.prototypes = new List<Prototype>(WaveFunctionCollapse.baseLevelPrototypes);
        else if (tile.y == Main.height - 1)
            this.prototypes = new List<Prototype>(WaveFunctionCollapse.topLevelPrototypes);
        else
            this.prototypes = new List<Prototype>(WaveFunctionCollapse.noOceans);
    }

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
        prototypes = new List<Prototype> { selected };
    }

    public void CollapseTo(string prototypeName)
    {
        Prototype proto = prototypes.Find(p => p.name == prototypeName);
        prototypes = new List<Prototype>();
        prototypes.Add(proto);
        TraversalManager.SetTilePrototype(tile, proto);
    }

    public void Constrain(Prototype prototype)
    {
        prototypes.Remove(prototype);
    }
}
