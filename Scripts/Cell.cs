using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    const int TRAVERSAL_MULTIPLIER = 10;

    public Tile tile;
    public List<Prototype> prototypes;

    public Cell(Tile tile)
    {
        this.tile = tile;
        if (tile.y == 0)
            this.prototypes = new List<Prototype>(WaveFunctionCollapse.baseLevelPrototypes);
        else if (tile.y == Main.height - 1)
            this.prototypes = new List<Prototype>(WaveFunctionCollapse.topLevelPrototypes);
        else
            this.prototypes = new List<Prototype>(WaveFunctionCollapse.noOceans);
    }

    public bool IsCollapsed => prototypes.Count == 1;
    public int Entropy => prototypes.Count;

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
