using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshNameUtilities
{
    static char[] cliffChars = new char[] { 'S', 'L', 'M', 'm' };

    public static bool IsBaseLevelPrototype(Prototype p)
    {
        if (p.meshName.Length == 0)
            return p.name == "III";

        if (p.meshName == "FFF-EEE")
            return true;

        string baseStr = p.meshName.Split('-')[0];
        return !baseStr.Contains('E');
    }

    public static bool IsTopLevelPrototype(Prototype p)
    {
        if (p.meshName.Length == 0)
            return p.name == "EEE";

        string cliffStr = p.meshName.Split('-')[1];
        return cliffStr.IndexOfAny(cliffChars) == -1;
    }
}
