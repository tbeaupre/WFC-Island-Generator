using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Main : MonoBehaviour
{
    public static int height = 5;
    public static int radius = 13;
    public int triangleSize = 2;
    WaveFunctionCollapse wfc;
    TileGrid tileGrid = new TileGrid(radius, height);
    CellManager cm = new CellManager();

    private List<Prototype> prototypes;

    void Awake()
    {
        wfc = new WaveFunctionCollapse();
        cm.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        prototypes = ModuleLoader.GetPrototypesFromFile();

        StartCoroutine(wfc.CollapseCo(tileGrid, prototypes, height, 0.1f, (data, drawAll) => {
            Draw(data, drawAll);
        }));
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StopAllCoroutines();
            cm.Clear();
            StartCoroutine(wfc.CollapseCo(tileGrid, prototypes, height, 0.1f, (data, drawAll) => {
                Draw(data, drawAll);
            }));
        }
    }

    void Draw(Dictionary<Tile, Cell> data, bool drawAll)
    {
        cm.UpdateCells(data.Values.ToList(), drawAll);
    }
}
