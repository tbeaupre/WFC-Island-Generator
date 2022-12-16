using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PrototypePicker
{
    public static Prototype PickPrototype(Cell cell)
    {
        List<Prototype> possibleProtos = cell.prototypes.Select(p => PrototypeManager.prototypes[p]).ToList();
        possibleProtos = SkyHelper.ReduceForEdges(cell.tile, possibleProtos);
        possibleProtos = OceanHelper.ReduceForEdges(cell.tile, possibleProtos);
        if (possibleProtos.Count == cell.prototypes.Count)
        {
            if (cell.prototypes.Contains(PrototypeManager.internalIndex))
            {
                Prototype internalPrototype = PrototypeManager.prototypes[PrototypeManager.internalIndex];
                TraversalManager.SetTilePrototype(cell.tile, internalPrototype);
                return internalPrototype;
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
