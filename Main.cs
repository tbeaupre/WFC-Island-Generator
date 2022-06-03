using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public int height = 10;
    PrototypeGenerator pg;
    HexGridCreator hgc;
    WaveFunctionCollapse wfc;
    CellDataToMesh cdtm;

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
        List<Prototype> prototypes = pg.GeneratePrototypes();
        List<Triangle> triangles = hgc.GetTriangulation();
        Dictionary<Triangle, Column> data = wfc.Collapse(triangles, prototypes, height);
    }

    void Draw(Dictionary<Triangle, Column> data)
    {
        foreach(KeyValuePair<Triangle, Column> kvp in data)
        {
            foreach(Cell cell in kvp.Value.cells)
            {
                cdtm.CreateMeshFromCell(cell);
            }
        }
    }
}
