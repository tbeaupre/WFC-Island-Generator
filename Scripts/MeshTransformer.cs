using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTransformer : MonoBehaviour
{
    float EDGE_LENGTH = 1.73205f;
    MeshFilter mf;
    Vector3[] origVerts;
    Vector3[] newVerts;
    float angle;
    float halfEdgeLength;

    public void Init()
    {
        mf = GetComponent<MeshFilter>();
        origVerts = mf.mesh.vertices;
        newVerts = new Vector3[origVerts.Length];
    }

    public void StretchXTowardsPoint(Vector3 target)
    {
        float dist = Vector3.Distance(target, transform.position);
        halfEdgeLength = dist / 2; // for use in shear
        float scale = dist / EDGE_LENGTH;
        angle = Vector3.SignedAngle(Vector3.right, target - transform.position, Vector3.up);

        for (var i = 0; i < origVerts.Length; i++)
        {
            var pt = origVerts[i];
            pt.x *= scale;
            newVerts[i] = pt;
        }
        mf.mesh.vertices = newVerts;

        transform.eulerAngles = new Vector3(0, angle, 0);
    }

    public void ScaleShearTowardsPoint(Vector3 target)
    {
        Vector3 rotatedTarget = Quaternion.Euler(0, -angle, 0) * (target - transform.position) + transform.position;

        float xOffset = halfEdgeLength + transform.position.x - rotatedTarget.x;
        float shearMultiplier = xOffset / 1.5f;

        float zDist = transform.position.z - rotatedTarget.z;
        float scale = zDist / 1.5f;

        origVerts = mf.mesh.vertices;
        for (var i = 0; i < origVerts.Length; i++)
        {
            var pt = origVerts[i];
            pt.x += shearMultiplier * pt.z;
            pt.z *= scale;
            newVerts[i] = pt;
        }
        mf.mesh.vertices = newVerts;
    }
}
