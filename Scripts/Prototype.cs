using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrototypeSet
{
    public Prototype[] modules;
}

[System.Serializable]
public class Prototype
{
    public int id;
    public string name;
    public string meshName;
    public int rotation;
    public TraversalSet traversalSet;
    public SocketSet sockets;
    public ValidNeighbors validNeighbors;
}

[System.Serializable]
public class TraversalSet
{
    public bool back;
    public bool right;
    public bool left;
    public bool top;
    public bool bottom;

    public bool GetIsTraversableInDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Back:
                return back;
            case Direction.Right:
                return right;
            case Direction.Left:
                return left;
            case Direction.Top:
                return top;
            case Direction.Bottom:
                return bottom;
            default:
                return top;
        }
    }
}

[System.Serializable]
public class SocketSet
{
    public string back;
    public string right;
    public string left;
    public string top;
    public string bottom;

    public string GetSocketInDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Back:
                return back;
            case Direction.Right:
                return right;
            case Direction.Left:
                return left;
            case Direction.Top:
                return top;
            case Direction.Bottom:
                return bottom;
            default:
                return top;
        }
    } 
}

[System.Serializable]
public class ValidNeighbors
{
    public List<int> back;
    public List<int> right;
    public List<int> left;
    public List<int> top;
    public List<int> bottom;
}
