using System.Collections.Generic;
using UnityEngine;
using System;

public class DrawingTool : MonoBehaviour
{
    public float MinGap = 0.1f;
    public float LevelTolerance = 0.00001f;

    private List<Vector3> singlePath;
    private List<GameObject> visualizationMarks;

    void Start()
    {
        singlePath = new List<Vector3>();
        visualizationMarks = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("Pressed");
            StorePosition();
        }
        else if (Input.GetMouseButton(0))
        {
            print("Hold");
            StorePosition();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            print("Released");
            StorePosition();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Mark();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {   
            Unmark();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Interpolate();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Report();
        }
    }

    // store the mouse position in world coordinates
    private void StorePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // if nothing is in the path
            if (singlePath.Count != 0)
            {
                print("****** Prepare to place item ******");
                // Grab the previous mark position
                Vector3 previousMark = singlePath[singlePath.Count - 1];

                // check if the y coordinate is off from the previous point
                float previousY = previousMark.y;
                float distance = Vector3.Distance(hit.point, previousMark);
                print(distance);

                if (Math.Abs(hit.point.y - previousY) < LevelTolerance &&
                    distance >= MinGap)
                {
                    print("****** Place item ******");
                    singlePath.Add(hit.point);
                }
                else
                {
                    print("****** FAILED TO PLACE ******");
                }
            }
            else
            {
                print("****** Place first item ******");
                singlePath.Add(hit.point);
            }
        }
    }

    /* add more points */
    private void Interpolate()
    {
        for (int i = 1; i < singlePath.Count; i++)
        {
            Vector3 difference = singlePath[i] - singlePath[i - 1];
            Vector3 newPoint = singlePath[i - 1];
            bool changed = false;

            if (Math.Abs(difference.x) > MinGap)
            {
                if (difference.x > 0)
                {
                    newPoint += new Vector3(MinGap, 0, 0);
                }
                else
                {
                    newPoint -= new Vector3(MinGap, 0, 0);
                }
                changed = true;
            }
            if (Math.Abs(difference.z) > MinGap)
            {
                if (difference.z > 0)
                {
                    newPoint += new Vector3(0, 0, MinGap);
                }
                else
                {
                    newPoint -= new Vector3(0, 0, MinGap);
                }
                changed = true;
            }
            if (changed)
            {
                singlePath.Insert(i, newPoint);
            }
        }
    }

    /* Visualization Tools */
    private void Mark ()
    {
        for (int i = visualizationMarks.Count; i < singlePath.Count; i++)
        {
            GameObject mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mark.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            mark.GetComponent<Renderer>().material.color = Color.green;
            mark.transform.position = singlePath[i];
            visualizationMarks.Add(mark);
        }
    }

    private void Unmark ()
    {
        for (int i = visualizationMarks.Count - 1; i > -1; i--)
        {
            Destroy(visualizationMarks[i]);
        }
        visualizationMarks.Clear();
    }

    private void Report ()
    {
        print(visualizationMarks.Count);
    }

}
