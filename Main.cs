using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public int height = 2;
    public int sideLength = 5;
    public int triangleSize = 2;
    WaveFunctionCollapse wfc;
    CellDataToMesh cdtm;
    HexGrid2 hg2;
    Dictionary<Triangle, Column> data;

    private int maxEntropy;
    private List<Triangle> triangles;
    private List<Prototype> prototypes;
    private List<GameObject> gameObjects;

    void Awake()
    {
        hg2 = new HexGrid2();
        wfc = new WaveFunctionCollapse();
        cdtm = GetComponent<CellDataToMesh>();
    }

    // Start is called before the first frame update
    void Start()
    {
        prototypes = ModuleLoader.GetPrototypesFromFile();
        triangles = hg2.GetTriangulation(sideLength, triangleSize);
        maxEntropy = prototypes.Count;
        gameObjects = new List<GameObject>();

        data = wfc.Collapse(triangles, prototypes, height);
        Draw(data);
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            data = wfc.Collapse(triangles, prototypes, height);
            Draw(data);
        }
    }

    void DeleteGameObjects()
    {
        foreach(GameObject go in gameObjects)
        {
            Destroy(go);
        }
        gameObjects.Clear();
    }

    void Draw(Dictionary<Triangle, Column> data)
    {
        DeleteGameObjects();
        foreach(KeyValuePair<Triangle, Column> kvp in data)
        {
            foreach(Cell cell in kvp.Value.cells)
            {
                gameObjects.Add(cdtm.CreateMeshFromCell(cell));
            }
        }
    }
}
