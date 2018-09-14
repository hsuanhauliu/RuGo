using System;
using UnityEngine;

public class PathTool : MonoBehaviour
{

    public void Activate(Action<Vector3[]> createGadgetsAlongPath)
    {
        Vector3[] pathPoints = new Vector3[2];
        pathPoints[0] = new Vector3(-0.5f, 0, 0.008f);
        pathPoints[1] = new Vector3(0.3f, 0, 0.008f);

        //Once the path is finalized, invoke the callback function with the path as a parameter
        createGadgetsAlongPath(pathPoints);
    }
}