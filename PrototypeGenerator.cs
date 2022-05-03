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

    List<BasePrototype> GetBasePrototypesFromFile(string path)
    {
         if (!File.Exists(path))
        {
            Debug.LogError("Prototypes data file not found at: " + path);
            return new List<BasePrototype>();
        }

        string fileContents = File.ReadAllText(path);
        BasePrototypeSet prototypeSet = JsonUtility.FromJson<BasePrototypeSet>(fileContents);

        return new List<BasePrototype>(prototypeSet.basePrototypes);
    }

    List<BasePrototype> AddTopAndBottomToBasePrototypes(List<BasePrototype> basePrototypes)
    {
        List<BasePrototype> result = new List<BasePrototype>(basePrototypes);

        foreach (BasePrototype p in result)
        {
            p.corners.c0.top = CalcTopForCorner(p.corners.c0);
            p.corners.c0.bottom = CalcBottomForCorner(p.corners.c0);
            p.corners.c1.top = CalcTopForCorner(p.corners.c1);
            p.corners.c1.bottom = CalcBottomForCorner(p.corners.c1);
            p.corners.c2.top = CalcTopForCorner(p.corners.c2);
            p.corners.c2.bottom = CalcBottomForCorner(p.corners.c2);
        }

        return result;
    }

    string CalcTopForCorner(Corner corner)
    {
        if (corner.top != null)
            return corner.top;

        if ((corner.left == "Ht" && corner.right == "I") || (corner.left == "I" && corner.right == "Ht"))
            return "Ht";

        if (corner.left == "C" || corner.right == "C")
            return "C";
        if (corner.left == "Cb" || corner.right == "Cb")
            return "C";

        if (corner.left == "I" && corner.right == "I")
            return "I";

        return "E";
    }
    
    string CalcBottomForCorner(Corner corner)
    {
        if (corner.bottom != null)
            return corner.bottom;

        if ((corner.left == "E" && corner.right == "F") || (corner.left == "F" && corner.right == "E"))
            return "Ht";

        if (corner.left == "Ct" || corner.right == "Ct")
            return "C";
        if (corner.left == "C" || corner.right == "C")
            return "C";

        if (corner.left == "E" && corner.right == "E")
            return "E";

        return "I";
    }

    #region Sockets
    List<Prototype> ConvertBasePrototypesToPrototypes(List<BasePrototype> basePrototypes)
    {
        Dictionary<string, string> sockets = new Dictionary<string, string>();
        int socketCount = 0;
        List<Prototype> result = new List<Prototype>();

        foreach(BasePrototype bp in basePrototypes)
        {
            result.Add(ConvertBasePrototypeToPrototype(sockets, ref socketCount, bp));
        }

        return result;
    }

    Prototype ConvertBasePrototypeToPrototype(Dictionary<string, string> sockets, ref int socketCount, BasePrototype p)
    {
        Prototype result = new Prototype();
        result.name = p.name;
        result.meshName = p.meshName;
        result.rotation = 0;
        result.sockets = new SocketSet();

        result.sockets.back = GetSocketForSideFace(sockets, ref socketCount, p.corners.c0.left, p.corners.c1.right);
        result.sockets.right = GetSocketForSideFace(sockets, ref socketCount, p.corners.c1.left, p.corners.c2.right);
        result.sockets.left = GetSocketForSideFace(sockets, ref socketCount, p.corners.c2.left, p.corners.c0.right);
        result.sockets.top = GetSocketForVerticalFace(sockets, ref socketCount, p.corners.c0.top, p.corners.c1.top, p.corners.c2.top);
        result.sockets.bottom = GetSocketForVerticalFace(sockets, ref socketCount, p.corners.c0.bottom, p.corners.c1.bottom, p.corners.c2.bottom);

        return result;
    }

    string GetSocketForSideFace(Dictionary<string, string> sockets, ref int socketCount, string hf0, string hf1)
    {
        string face = hf0 + hf1;
        if (sockets.ContainsKey(face))
        {
            return sockets[face];
        }

        string newSocket;
        bool isSymmetrical = hf0 == hf1;
        if (isSymmetrical)
        {
            newSocket = socketCount + "s";
        }
        else
        {
            newSocket = socketCount.ToString();

            string flippedFace = hf1 + hf0;
            string flippedSocket = socketCount + "f";
            sockets.Add(flippedFace, flippedSocket);
        }

        sockets.Add(face, newSocket);
        ++socketCount;
        return newSocket;
    }

    string GetSocketForVerticalFace(Dictionary<string, string> sockets, ref int socketCount, string c0, string c1, string c2)
    {
        string face = c0 + c1 + c2;
        if (sockets.ContainsKey(face))
        {
            return sockets[face];
        }
        
        string newSocket;
        bool isSymmetrical = c0 == c1 && c1 == c2;
        if (isSymmetrical)
        {
            newSocket = "v" + socketCount + "s";
        }
        else
        {
            newSocket = "v" + socketCount + "_0";

            string r1Face = c1 + c2 + c0;
            string r1Socket = "v" + socketCount + "_1";
            sockets.Add(r1Face, r1Socket);

            string r2Face = c2 + c0 + c1;
            string r2Socket = "v" + socketCount + "_2";
            sockets.Add(r2Face, r2Socket);
        }

        sockets.Add(face, newSocket);
        ++socketCount;
        return newSocket;
    }
    #endregion

    void GeneratePrototypes()
    {
        List<BasePrototype> protoPrototypes = AddTopAndBottomToBasePrototypes(GetBasePrototypesFromFile(path));
        
        List<Prototype> basePrototypes = ConvertBasePrototypesToPrototypes(protoPrototypes);

        Debug.Log("Base Prototype Count: " + basePrototypes.Count);

        // Create final list of prototypes including rotations
        List<Prototype> prototypes = new List<Prototype>();
        foreach(Prototype p in basePrototypes)
        {
            prototypes.Add(p);

            // Rotationally symmetrical prototypes don't need their rotations to be added to the list
            if (p.sockets.top.EndsWith("s") && p.sockets.bottom.EndsWith("s"))
                continue;
            
            Prototype p1 = new Prototype();
            p1.name = p.name + "_1";
            p1.meshName = p.meshName;
            p1.rotation = 1;
            SocketSet sockets1 = new SocketSet();
            sockets1.back = p.sockets.left;
            sockets1.right = p.sockets.back;
            sockets1.left = p.sockets.right;
            sockets1.top = GetRotatedVerticalSocketString(p.sockets.top);
            sockets1.bottom = GetRotatedVerticalSocketString(p.sockets.bottom);
            p1.sockets = sockets1;
            prototypes.Add(p1);

            Prototype p2 = new Prototype();
            p2.name = p.name + "_2";
            p2.meshName = p.meshName;
            p2.rotation = 2;
            SocketSet sockets2 = new SocketSet();
            sockets2.back = p.sockets.right;
            sockets2.right = p.sockets.left;
            sockets2.left = p.sockets.back;
            sockets2.top = GetRotatedVerticalSocketString(p1.sockets.top);
            sockets2.bottom = GetRotatedVerticalSocketString(p1.sockets.bottom);
            p2.sockets = sockets2;
            prototypes.Add(p2);
        }

        Debug.Log("Final Prototype Count: " + prototypes.Count);

        foreach (Prototype proto1 in prototypes)
        {
            proto1.validNeighbors = new ValidNeighbors();
            foreach (Prototype proto2 in prototypes)
            {
                if (AreSocketsCompatible(proto1.sockets.back, proto2.sockets.back))
                    proto1.validNeighbors.back.Add(proto2);
                
                if (AreSocketsCompatible(proto1.sockets.left, proto2.sockets.left))
                    proto1.validNeighbors.left.Add(proto2);

                if (AreSocketsCompatible(proto1.sockets.right, proto2.sockets.right))
                    proto1.validNeighbors.right.Add(proto2);

                if (AreSocketsCompatible(proto1.sockets.top, proto2.sockets.bottom))
                    proto1.validNeighbors.top.Add(proto2);

                if (AreSocketsCompatible(proto1.sockets.bottom, proto2.sockets.top))
                    proto1.validNeighbors.bottom.Add(proto2);
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

    string GetRotatedVerticalSocketString(string socketName)
    {        
        if (socketName.EndsWith("s"))
            return socketName;
        string[] subs = socketName.Split('_');
        int socketRotation = (int.Parse(subs[1]) + 1) % 3;
        return subs[0] + "_" + socketRotation;
    }
}
