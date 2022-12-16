using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PrototypeManager
{
    public static List<Prototype> prototypes;

    public static void Init()
    {
        prototypes = ModuleLoader.GetPrototypesFromFile();
        for (int i = 0; i < prototypes.Count; ++i)
            prototypes[i].id = i;
    }
}
