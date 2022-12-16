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

    void Awake()
    {
        TileGrid.Init(radius, height);
        wfc = new WaveFunctionCollapse();
        CellManager.Init(transform);
        PrototypeManager.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(wfc.CollapseCo(timeBetweenSteps, (data, drawAll) => {
            Draw(data, drawAll);
        }));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            StartCoroutine(wfc.CollapseCo(timeBetweenSteps, (data, drawAll) => {
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
