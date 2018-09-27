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

        if (Input.GetKeyDown(KeyCode.S))
        {
            Smooth();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            List<Vector3> newPoints = GetEqualDistancePoints(singlePath);
            Mark(newPoints);
        }
    }

    private List<Vector3> GetEqualDistancePoints(List<Vector3> myPoints)
    {
        List<Vector3> points = new List<Vector3>();
        if (myPoints.Count > 0)
        {
            // Add the first point
            points.Add(myPoints[0]);
            float leftover = 0f;

            Vector3 previousPoint = myPoints[0];
            for (int i = 1; i < myPoints.Count; i++)
            {
                float segmentLength = Vector3.Distance(myPoints[i], myPoints[i - 1]);
                Vector3 segmentVector = (myPoints[i] - myPoints[i - 1]) / segmentLength;

                if (leftover == 0)
                {
                    int count = (int)(segmentLength / DominoDistance);
                    for (int n = 0; n < count; n++)
                    {
                        Vector3 newPoint = previousPoint + segmentVector * DominoDistance;
                        points.Add(newPoint);
                        previousPoint = newPoint;
                    }
                    leftover = segmentLength - count * DominoDistance;
                }
                else if (Vector3.Distance(previousPoint, myPoints[i]) > DominoDistance)
                {
                    float angle_a = getAngle(myPoints[i - 1] - previousPoint, myPoints[i] - myPoints[i - 1]);
                    float side_b = Vector3.Distance(myPoints[i - 1], previousPoint);
                    float side_c = calculateSide(DominoDistance, side_b, angle_a);
                    float remaining_segment = segmentLength - side_c;

                    Vector3 newPoint = myPoints[i - 1] + side_c * segmentVector;
                    points.Add(newPoint);
                    previousPoint = newPoint;

                    int count = (int)(remaining_segment / DominoDistance);
                    for (int n = 0; n < count; n++)
                    {
                        newPoint = previousPoint + segmentVector * DominoDistance;
                        points.Add(newPoint);
                        previousPoint = newPoint;
                    }
                    leftover = remaining_segment - count * DominoDistance;
                }
            }
        }

        return points;
    }


    private float calculateSide(float side_a, float side_b, float angle_a)
    {
        float sin_of_angle_a = Mathf.Sin(angle_a * Mathf.PI / 180);
        float angle_b = Mathf.Asin(side_b * sin_of_angle_a / side_a) * 180 / Mathf.PI;
        float angle_c = angle_a + angle_b;

        return Mathf.Sin(angle_c * Mathf.PI / 180) * side_a / sin_of_angle_a;
    }

    // get angle between two vectors
    private float getAngle(Vector3 from, Vector3 to)
    {
        return 180 - Vector3.Angle(from, to);
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
            List<Vector3> myPoints = GetEqualDistancePoints(singlePath);
            List<Vector3> smoothedLine = new List<Vector3>();
            List<Vector3> controlPoints = GetControlPoints(myPoints);
            smoothedLine.Add(myPoints[0]);

            for (int i = 1; i < myPoints.Count; i++)
            {
                Vector3 newPoint = calculatePoint(0.5f, myPoints[i - 1],
                                                  controlPoints[2 * (i - 1)],
                                                  controlPoints[2 * i - 1],
                                                  myPoints[i]);
                smoothedLine.Add(newPoint);
                smoothedLine.Add(myPoints[i]);
            }
            singlePath = smoothedLine;
        }
    }

    private Vector3 calculatePoint(float t, Vector3 point_a, Vector3 point_b, Vector3 point_c, Vector3 point_d)
    {
        float reversed = 1 - t;
        Vector3 first_part = reversed * reversed * reversed * point_a;
        Vector3 second_part = 3 * reversed * reversed * t * point_b;
        Vector3 third_part = 3 * reversed * t * t * point_c;
        Vector3 fourth_part = t * t * t * point_d;

        return first_part + second_part + third_part + fourth_part;

    }

    private List<Vector3> GetControlPoints(List<Vector3> knots)
    {
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
