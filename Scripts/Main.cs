using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Main : MonoBehaviour
{
    public static int height = 6;
    public static int radius = 4;
    public int triangleSize = 2;
    WaveFunctionCollapse wfc;
    float timeBetweenSteps = 0;

    private List<Prototype> prototypes;

    void Awake()
    {
        TileGrid.Init(radius, height);
        wfc = new WaveFunctionCollapse();
        CellManager.Init(transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        prototypes = ModuleLoader.GetPrototypesFromFile();

        StartCoroutine(wfc.CollapseCo(prototypes, timeBetweenSteps, (data, drawAll) => {
            Draw(data, drawAll);
        }));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            StartCoroutine(wfc.CollapseCo(prototypes, timeBetweenSteps, (data, drawAll) => {
                Draw(data, drawAll);
            }));
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void Draw(Dictionary<Tile, Cell> data, bool drawAll)
    {
        CellManager.UpdateCells(data.Values.ToList(), drawAll);
    }
}
