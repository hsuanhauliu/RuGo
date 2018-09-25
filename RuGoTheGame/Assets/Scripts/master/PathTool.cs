using System;
using UnityEngine;
using System.Collections.Generic;

public class PathTool : MonoBehaviour
{
    private Action<Vector3[]> mPathCompleteCallBack;
    private bool isActive = false;
    private List<Vector3> mPathPoints;

    void Start()
    {
        mPathPoints = new List<Vector3>();
    }

    void Update()
    {
        if (isActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 positionInWorld = hit.point;
                    mPathPoints.Add(positionInWorld);
                    Debug.Log("ADDING POINT");
                    if (mPathPoints.Count == 2)
                    {
                        mPathCompleteCallBack(mPathPoints.ToArray());
                        Deactivate();
                    }
                }
            }
        }
    }

    public void Deactivate() {
        isActive = false;
        mPathPoints = new List<Vector3>();
    }

    public void Activate(Action<Vector3[]> createGadgetsAlongPath)
    {
        isActive = true;
        mPathCompleteCallBack = createGadgetsAlongPath;
    }
}