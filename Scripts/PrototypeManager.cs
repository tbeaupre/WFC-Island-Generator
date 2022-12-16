using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PrototypeManager
{
    public static List<Prototype> prototypes;
    public static int externalIndex;
    public static int internalIndex;

    public static void Init()
    {
        prototypes = ModuleLoader.GetPrototypesFromFile();
        externalIndex = prototypes.Find(p => p.name == "EEE").id;
        internalIndex = externalIndex + 1;
    }
}
