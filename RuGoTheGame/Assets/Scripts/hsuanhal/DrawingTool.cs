// https://www.codeproject.com/Articles/31859/Draw-a-Smooth-Curve-through-a-Set-of-2D-Points-wit

using System.Collections.Generic;
using UnityEngine;
using System;

public class DrawingTool : MonoBehaviour
{
    public float MinGap = 0.1f;
    public float LevelTolerance = 0.00001f;
    public float DominoDistance = 0.1f;

    private float MaxGap = 0.15f;
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
            if (visualizationMarks.Count == 0)
                Mark(singlePath);
            else
                Unmark();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Interpolated.");
            Interpolate();
        }


        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Smoothed.");
            Smooth();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            List<Vector3> newPoints = GetPoints(singlePath);
            for (int i = 0; i < newPoints.Count; i++)
            {
                print(newPoints[i]);
            }
            Mark(newPoints);
        }
    }

    private List<Vector3> GetPoints(List<Vector3> myPoints)
    {
        List<Vector3> points = new List<Vector3>();
        if (myPoints.Count > 0)
        {
            points.Add(myPoints[0]);
            print(myPoints[0]);
        }

        float leftover = 0f;
        for (int i = 1; i < myPoints.Count; i++)
        {
            float segmentLength = Vector3.Distance(myPoints[i], myPoints[i - 1]);
            float totalLength = segmentLength + leftover;
            int counter = 0;

            while (totalLength > DominoDistance)
            {
                counter += 1;
                Vector3 directionVector = myPoints[i] - myPoints[i - 1];
                float pointDistance = DominoDistance - leftover;
                leftover = 0;
                totalLength -= pointDistance;
                Vector3 point = myPoints[i - 1] + counter * (directionVector * pointDistance / segmentLength);
                points.Add(point);
                print(point);
            }
            leftover = totalLength;
        }
        if (myPoints.Count > 1)
        {
            points.Add(myPoints[myPoints.Count - 1]);
        }

        return points;
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

    private void Smooth()
    {
        // Smooth the line only when there is more than one point
        if (singlePath.Count > 1)
        {
            List<Vector3> smoothedLine = new List<Vector3>();
            List<Vector3> controlPoints = GetControlPoints();

            for (int i = 0; i < singlePath.Count - 1; i++)
            {
                smoothedLine.Add(singlePath[i]);
                smoothedLine.Add(controlPoints[2 * i]);
                smoothedLine.Add(controlPoints[2 * i + 1]);
            }
            smoothedLine.Add(singlePath[singlePath.Count - 1]);
            singlePath = smoothedLine;
        }
    }

    private List<Vector3> GetControlPoints()
    {
        List<Vector3> knots = singlePath;
        List<Vector3> controlPoints = new List<Vector3>();

        // Calculate control points only when there is more than one point
        if (knots != null && knots.Count > 1)
        {
            int n = knots.Count - 1;

            // Single segment (2 knot points only)
            if (n == 1)
            {
                // 3 * P1 = 2 * K0 + K1
                Vector3 firstControlPoint = (2 * knots[0] + knots[1]) / 3;
                controlPoints.Add(firstControlPoint);

                // P2 = 2 * P1 – K0
                Vector3 secondControlPoint = 2 * firstControlPoint - knots[0];
                controlPoints.Add(secondControlPoint);
            }
            // Multiple segments
            else
            {
                // Calculate the first control point for each segment
                List<Vector3> rhs = new List<Vector3>();
                rhs.Add(knots[0] + 2 * knots[1]);
                for (int i = 1; i < n - 1; i++)
                {
                    rhs.Add(4 * knots[i] + 2 * knots[i + 1]);
                }
                rhs.Add((8 * knots[n - 1] + knots[n]) / 2);
                List<Vector3> firstControlPoints = GetFirstControlPoints(rhs);

                // Calculate the second control point for each segment
                List<Vector3> secondControlPoints = new List<Vector3>();
                for (int i = 0; i < n - 1; i++)
                {
                    secondControlPoints.Add(2 * knots[i + 1] - firstControlPoints[i + 1]);
                }
                secondControlPoints.Add((knots[n] + firstControlPoints[n - 1]) / 2);

                // combine two lists
                for (int i = 0; i < n; i++)
                {
                    controlPoints.Add(firstControlPoints[i]);
                    controlPoints.Add(secondControlPoints[i]);
                }
            }
        }
        return controlPoints;
    }

    private List<Vector3> GetFirstControlPoints(List<Vector3> rhs)
    {
        int n = rhs.Count;
        List<Vector3> returnList = new List<Vector3>();
        float[] temp = new float[n];

        float b = 2.0f;
        returnList.Add(rhs[0] / b);

        for (int i = 1; i < n - 1; i++)
        {
            temp[i] = 1 / b;
            b = 4 - temp[i];
            returnList.Add((rhs[i] - returnList[i - 1]) / b);
        }

        b = 3.5f - 1 / b;
        returnList.Add((rhs[n - 1] - returnList[n - 2]) / b);


        for (int i = 1; i < n; i++)
        {
            returnList[n - i - 1] -= temp[n - i] * returnList[n - i];
        }

        return returnList;
    }

    /* add more points */
    private void Interpolate()
    {
        int index = 1;
        while (index < singlePath.Count)
        {
            Vector3 diff = singlePath[index] - singlePath[index - 1];
            int step_num = (int)(Math.Max(Math.Abs(diff.x), Math.Abs(diff.z)) / MaxGap);
            Vector3 step_vec = diff / step_num;

            if (step_num != 0)
            {
                for (int s = 1; s < step_num; s++)
                {
                    Vector3 newPoint = singlePath[index - 1] + step_vec * s;
                    singlePath.Insert(index - 1 + s, newPoint);
                }
                index += step_num;
            }
            else
            {
                index++;
            }
        }
    }

    /* Visualization Tools */
    private void Mark (List<Vector3> points)
    {
        for (int i = visualizationMarks.Count; i < points.Count; i++)
        {
            GameObject mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mark.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            mark.transform.position = points[i];
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

}
