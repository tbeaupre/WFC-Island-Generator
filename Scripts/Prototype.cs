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
public class PrototypeSet
{
    public Prototype[] modules;
}

[System.Serializable]
public class Prototype
{
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
    public List<string> back;
    public List<string> right;
    public List<string> left;
    public List<string> top;
    public List<string> bottom;

    public ValidNeighbors()
    {
        back = new List<string>();
        right = new List<string>();
        left = new List<string>();
        top = new List<string>();
        bottom = new List<string>();
    }

    public string ToString()
    {
        string result = "Back Neighbors: ";
        foreach (string neighbor in back)
        {
            result += neighbor + ", ";
        }
        result += "\nRight Neighbors: ";
        foreach (string neighbor in right)
        {
            result += neighbor + ", ";
        }
        result += "\nLeft Neighbors: ";
        foreach (string neighbor in left)
        {
            result += neighbor + ", ";
        }
        result += "\nTop Neighbors: ";
        foreach (string neighbor in top)
        {
            result += neighbor + ", ";
        }
        result += "\nBottom Neighbors: ";
        foreach (string neighbor in bottom)
        {
            result += neighbor + ", ";
        }
        return result;
    }
}
