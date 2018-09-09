using System.Collections.Generic;
using UnityEngine;

public class DrawingTool : MonoBehaviour
{
    public float MinGap = 0.0001f;
    public float LevelTolerance = 0.01f;

    private List<List<Vector3>> Paths;
    private List<Vector3> singlePath;

    void Start()
    {
        Paths = new List<List<Vector3>>();
        singlePath = new List<Vector3>();
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
            EndPath();
        }
    }

    // store singlePath into Paths and reset singlePath
    private void EndPath()
    {
        Paths.Add(singlePath);
        singlePath = new List<Vector3>();
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

                if (hit.point.y - previousY < LevelTolerance && distance >= MinGap)
                {
                    print("****** Place item ******");
                    singlePath.Add(hit.point);
                    PlaceMark(hit.point);
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
                PlaceMark(hit.point);
            }
        }
    }

    // draw spheres for visualization
    private void PlaceMark(Vector3 markPosition)
    {
        GameObject mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mark.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        mark.GetComponent<Renderer>().material.color = Color.green;
        mark.transform.position = markPosition;
    }

}
