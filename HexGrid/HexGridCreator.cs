using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshGenerator))]
public class HexGridCreator : MonoBehaviour
{
    MeshGenerator mg;
    public int size = 4;
    public float triSize = 1.0f;

    void Awake()
    {
        mg = GetComponent<MeshGenerator>();
    }

    void Start()
    {
        mg.Recalculate(GenerateHexPoints(size, triSize));
    }

    List<Vector3> GenerateHexPoints(int size, float triSize)
    {
        List<Vector3> result = new List<Vector3>();
        bool isShrinking = false;
        int prevWidth = size - 1;
        float leftX = 0;
        float triHeight = Mathf.Sin(60 * Mathf.Deg2Rad) * triSize; // Will need to do a bit of math to make the triangles equilateral.
        float xOffset;

        for (int y = 0; y < size * 2 - 1; ++y)
        {
            if (isShrinking)
                xOffset = triSize * 0.5f;
            else
                xOffset = triSize * -0.5f;

            int xWidth = prevWidth + (isShrinking ? -1 : 1);

            for (int x = 0; x < xWidth; ++x)
            {
                if (x == 0)
                    leftX += xOffset;
                result.Add(new Vector3(x * triSize + leftX, 0, y * triHeight));
            }

            prevWidth = xWidth;
            if (y == size - 1)
                isShrinking = true;
        }

        return result;
    }
}
