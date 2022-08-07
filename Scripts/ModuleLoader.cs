using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModuleLoader
{
    public static List<Prototype> GetPrototypesFromFile()
    {
        TextAsset jsonObj = Resources.Load<TextAsset>("Modules");
        PrototypeSet prototypeSet = JsonUtility.FromJson<PrototypeSet>(jsonObj.text);

        return new List<Prototype>(prototypeSet.modules);
    }
}
