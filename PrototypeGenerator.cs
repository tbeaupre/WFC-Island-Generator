using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeGenerator : MonoBehaviour
{
    public string path;

    void Start()
    {
        GeneratePrototypes();
    }

    void GeneratePrototypes()
    {
        // Get prototypes from JSON
        if (!File.Exists(path))
        {
            Debug.LogError("Prototypes data file not found at: " + path);
            return;
        }

        string fileContents = File.ReadAllText(path);
        PrototypeSet prototypeSet = JsonUtility.FromJson<PrototypeSet>(fileContents);

        List<Prototype> basePrototypes = new List<Prototype>(prototypeSet.prototypes);

        Debug.Log("Base Prototype Count: " + basePrototypes.Count);

        // Create final list of prototypes including rotations
        List<Prototype> prototypes = new List<Prototype>();
        foreach(Prototype p in basePrototypes)
        {
            p.rotation = 0;
            prototypes.Add(p);

            // Rotationally symmetrical prototypes don't need their rotations to be added to the list
            if (p.name.EndsWith("s"))
                continue;
            
            Prototype p1 = new Prototype();
            p1.name = p.name.Substring(0, p.name.Length - 1) + "1";
            p1.meshName = p.meshName;
            p1.rotation = 1;
            SocketSet sockets1 = new SocketSet();
            sockets1.back = p.sockets.left;
            sockets1.right = p.sockets.back;
            sockets1.left = p.sockets.right;
            sockets1.top = GetVerticalSocketString(p.sockets.top);
            sockets1.bottom = GetVerticalSocketString(p.sockets.bottom);
            p1.sockets = sockets1;
            prototypes.Add(p1);

            Prototype p2 = new Prototype();
            p2.name = p.name.Substring(0, p.name.Length - 1) + "2";
            p2.meshName = p.meshName;
            p2.rotation = 1;
            SocketSet sockets2 = new SocketSet();
            sockets2.back = p.sockets.right;
            sockets2.right = p.sockets.left;
            sockets2.left = p.sockets.back;
            sockets2.top = GetVerticalSocketString(p1.sockets.top);
            sockets2.bottom = GetVerticalSocketString(p1.sockets.bottom);
            p2.sockets = sockets2;
            prototypes.Add(p2);
        }

        // Add empty prototype
        Prototype empty = new Prototype();
        empty.name = "-1";
        empty.rotation = 0;
        SocketSet emptySockets = new SocketSet();
        emptySockets.back = "-1";
        emptySockets.right = "-1";
        emptySockets.left = "-1";
        emptySockets.top = "-1";
        emptySockets.bottom = "-1";
        empty.sockets = emptySockets;
        prototypes.Add(empty);

        Debug.Log("Final Prototype Count: " + prototypes.Count);

        foreach (Prototype proto1 in prototypes)
        {
            proto1.validNeighbors = new ValidNeighbors();
            foreach (Prototype proto2 in prototypes)
            {
                if (AreSocketsCompatible(proto1.sockets.back, proto2.sockets.back))
                    proto1.validNeighbors.back.Add(proto2.name);
                
                if (AreSocketsCompatible(proto1.sockets.left, proto2.sockets.left))
                    proto1.validNeighbors.left.Add(proto2.name);

                if (AreSocketsCompatible(proto1.sockets.right, proto2.sockets.right))
                    proto1.validNeighbors.right.Add(proto2.name);

                if (AreSocketsCompatible(proto1.sockets.top, proto2.sockets.bottom))
                    proto1.validNeighbors.top.Add(proto2.name);

                if (AreSocketsCompatible(proto1.sockets.bottom, proto2.sockets.top))
                    proto1.validNeighbors.bottom.Add(proto2.name);
            }
        }
    }

    bool AreSocketsCompatible(string socket1, string socket2)
    {
        bool isSymmetrical = socket1.EndsWith("s");
        bool isVertical = socket1.StartsWith("v");

        if (isSymmetrical || isVertical)
            return socket1 == socket2;
        
        return socket1 == socket2 + "f" || socket2 == socket1 + "f";
    }

    string GetVerticalSocketString(string socketName)
    {
        if (socketName == "-1")
            return socketName;
        if (socketName.EndsWith("s"))
            return socketName;
        return GetRotatedVerticalSocketString(socketName);
    }

    string GetRotatedVerticalSocketString(string socketName)
    {
        string[] subs = socketName.Split('_');
        int socketRotation = (int.Parse(subs[1]) + 1) % 3;
        return subs[0] + "_" + socketRotation;
    }
}
