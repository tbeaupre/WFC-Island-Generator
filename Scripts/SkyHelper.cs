using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class SkyHelper
{
    public static List<Prototype> ReduceForEdges(Tile tile, List<Prototype> prototypes)
    {
        if (tile.y != SettingsManager.Height - 1)
            return prototypes;

        List<Prototype> result = new List<Prototype>(prototypes);
        result = result.Where(p => p.validNeighbors.top.Contains(PrototypeManager.externalIndex)).ToList();
        return result;
    }
}
