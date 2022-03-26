using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshGenerator))]
public class PointManager : MonoBehaviour
{
    MeshGenerator mg;
    List<Vector3> points;

    void Awake()
    {
        mg = GetComponent<MeshGenerator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        points = new List<Vector3>() {
            new Vector3 (0, 0, 0),
            new Vector3 (0, 0, 1),
            new Vector3 (1, 0, 0),
        };
        
        mg.Recalculate(points);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Clicked();
        }
    }

    void Clicked()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            points.Add(hit.point);
            Debug.Log(hit.point);
            mg.Recalculate(points);
        }
    }


}
