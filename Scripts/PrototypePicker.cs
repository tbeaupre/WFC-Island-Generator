using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PrototypePicker
{
    public static Prototype PickPrototype(Cell cell)
    {
        List<Prototype> possibleProtos = OceanHelper.ReduceForEdges(cell.tile, cell.prototypes);
        if (possibleProtos.Count == cell.prototypes.Count)
        {
            Prototype internalProto = cell.prototypes.Find(p => p.name == "III");
            if (internalProto != null)
            {
                TraversalManager.SetTilePrototype(cell.tile, internalProto);
                return internalProto;
            }
        }
        possibleProtos = OceanHelper.ReduceOceans(cell.tile, possibleProtos);
        possibleProtos = MountainHelper.RemoveLocalMinima(cell.tile, possibleProtos);

        if (cell.tile.y == 0) // Don't leave holes in the map.
        {
            possibleProtos = possibleProtos.Where(p => !IsPrototypeOpenOnBottom(p)).ToList();
            if (possibleProtos.Count == 0)
                Debug.Log(cell.prototypes);
        }

        return TraversalManager.PickTraversalWeightedPrototype(cell.tile, possibleProtos);
    }

    static private bool IsPrototypeOpenOnBottom(Prototype p)
    {
        if (p.meshName.Length == 0)
            return p.name == "EEE";

        string baseStr = p.meshName.Split('-')[0];
        if (baseStr.Contains('E'))
            return true;

        return false;
    }
}
