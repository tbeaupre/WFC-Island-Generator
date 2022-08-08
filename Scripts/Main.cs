using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static int height = 5;
    public static int radius = 10;
    public int triangleSize = 2;
    WaveFunctionCollapse wfc;
    CellDataToMesh cdtm;
    Dictionary<Tile, Cell> data;
    TileGrid tileGrid = new TileGrid(radius, height);

    private List<Prototype> prototypes;
    private List<GameObject> gameObjects;

    void Awake()
    {
        wfc = new WaveFunctionCollapse();
        cdtm = GetComponent<CellDataToMesh>();
    }

    // Start is called before the first frame update
    void Start()
    {
        prototypes = ModuleLoader.GetPrototypesFromFile();
        gameObjects = new List<GameObject>();

        StartCoroutine(wfc.CollapseCo(tileGrid, prototypes, height, 0.1f, data => {
            Draw(data);
        }));
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StopAllCoroutines();
            StartCoroutine(wfc.CollapseCo(tileGrid, prototypes, height, 0.1f, data => {
                Draw(data);
            }));
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

    void Draw(Dictionary<Tile, Cell> data)
    {
        DeleteGameObjects();
        foreach (Cell cell in data.Values)
        {
            gameObjects.Add(cdtm.CreateMeshFromCell(cell));
        }
    }
}
