using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public int height = 2;
    PrototypeGenerator pg;
    HexGridCreator hgc;
    WaveFunctionCollapse wfc;
    CellDataToMesh cdtm;
    Dictionary<Triangle, Column> data;

    private int maxEntropy;
    private List<Triangle> triangles;
    private List<Prototype> prototypes;
    private List<GameObject> gameObjects;

    void Awake()
    {
        pg = GetComponent<PrototypeGenerator>();
        hgc = GetComponent<HexGridCreator>();
        wfc = new WaveFunctionCollapse();
        cdtm = GetComponent<CellDataToMesh>();
    }

    // Start is called before the first frame update
    void Start()
    {
        prototypes = pg.GeneratePrototypes();
        triangles = hgc.GetTriangulation();
        maxEntropy = prototypes.Count;
        data = wfc.InitializeData(triangles, prototypes, height);
        gameObjects = new List<GameObject>();
        //wfc.Collapse(triangles, prototypes, height);
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (!wfc.IsCollapsed(data))
            {
                Debug.Log("Drawing");
                wfc.Iterate(data, maxEntropy);
                Draw(data);
            }
            else
            { 
                Debug.Log("Restarting");
                data = wfc.InitializeData(triangles, prototypes, height);
            }
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
                if (cell.prototypes.Count == 1)
                {
                    Debug.Log("Drawing Mesh");
                    gameObjects.Add(cdtm.CreateMeshFromCell(cell));
                }
            }
        }
    }
}
