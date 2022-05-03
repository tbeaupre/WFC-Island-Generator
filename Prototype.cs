using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasePrototypeSet
{
    public BasePrototype[] basePrototypes;
}

#region Manually Created Prototype Metadata
[System.Serializable]
public class BasePrototype
{
    public string name;
    public string meshName;
    public bool isSymmetrical;
    public CornerSet corners;
}

[System.Serializable]
public class CornerSet
{
    public Corner c0;
    public Corner c1;
    public Corner c2;
}

[System.Serializable]
public class Corner
{
    public string left;
    public string right;
    public string top;
    public string bottom;
}
#endregion

[System.Serializable]
public class Prototype
{
    public string name;
    public string meshName;
    public int rotation;
    public SocketSet sockets;
    public ValidNeighbors validNeighbors;
}

[System.Serializable]
public class SocketSet
{
    public string back;
    public string right;
    public string left;
    public string top;
    public string bottom;
}

[System.Serializable]
public class ValidNeighbors
{
    public List<Prototype> back;
    public List<Prototype> right;
    public List<Prototype> left;
    public List<Prototype> top;
    public List<Prototype> bottom;

    public ValidNeighbors()
    {
        back = new List<Prototype>();
        right = new List<Prototype>();
        left = new List<Prototype>();
        top = new List<Prototype>();
        bottom = new List<Prototype>();
    }
}

/*
1 = vertex present; 0 = no vertex present

top left, top right, bottom left, bottom right
  "sideFaceGeometry": {
    "0s": [1, 1, 1, 1],
    "1":  [1, 0, 1, 1],
    "1f": [0, 1, 1, 1],
    "2s": [0, 0, 1, 1],
    "3":  [1, 0, 1, 0],
    "3f": [0, 1, 0, 1],
    "4":  [0, 0, 0, 1],
    "4f": [0, 0, 1, 0]
  },

top right, bottom center, top left
  "topFaceGeometry": {
    "v0s": [1, 1, 1],
    "v1_0": [1, 0, 1],
    "v1_1": [1, 1, 0],
    "v1_2": [0, 1, 1],
    "v2_0": [0, 0, 1],
    "v2_1": [1, 0, 0],
    "v2_2": [0, 1, 0]
  }
*/
