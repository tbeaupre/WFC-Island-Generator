using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class SkyHelper
{
    public static List<Prototype> ReduceForEdges(Tile tile, List<Prototype> prototypes)
    {
        if (tile.y != Main.height - 1)
            return prototypes;

        List<Prototype> result = new List<Prototype>(prototypes);
        result = result.Where(p => p.validNeighbors.top.Contains("EEE")).ToList();
        return result;
    }
}