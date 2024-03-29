using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Main : MonoBehaviour
{
    public int triangleSize = 2;
    WaveFunctionCollapse wfc;

    void Awake()
    {
        TileGrid.Init();
        WaveFunctionCollapse.Init();
        wfc = new WaveFunctionCollapse();
        CellManager.Init(transform);
        PrototypeManager.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateIsland();
    }

    void Update()
    {
        if (!SettingsManager.SettingsOpen)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GenerateIsland();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                GenerateIsland(WaveFunctionCollapse.seed);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void GenerateIsland(UnityEngine.Random.State? initSeed = null)
    {
        StopAllCoroutines();
        StartCoroutine(wfc.CollapseCo(initSeed, (data, drawAll) => {
            Draw(data, drawAll);
        }));
    }

    void Draw(Dictionary<Tile, Cell> data, bool drawAll)
    {
        CellManager.UpdateCells(data.Values.ToList(), drawAll);
    }
}
