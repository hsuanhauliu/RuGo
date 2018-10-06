using System;
using UnityEngine;
using System.Collections.Generic;

public class PathTool : MonoBehaviour
{
    // control variables for gap between each point and y-level tolerance
    private const float levelTolerance = 0.00001f;
    private const float dominoDistance = 0.04f;

    private bool isActive = false;
    private Action<Vector3[]> pathCompleteCallBack;
    private List<Vector3> singlePath;
    private LineRenderer lineRenderer;


    void Start()
    {
        singlePath = new List<Vector3>();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Texture"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.enabled = false;
    }


    void Update()
    {
        if (isActive)
        {
            if (singlePath.Count == 1 && lineRenderer.enabled)
            {
                Ray ray = RuGoInteraction.Instance.SelectorRay;
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    lineRenderer.SetPosition(1, hit.point);
                }
            }

            if (singlePath.Count == 0 && RuGoInteraction.Instance.IsConfirmPressed)
            {
                Debug.Log("Mouse down detected.");
                storePosition();
                markStartingPoint();
            }
            else if (singlePath.Count != 0 && RuGoInteraction.Instance.IsConfirmHeld)
            {
                Debug.Log("Mouse hold detected.");
                storePosition();

                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, singlePath[singlePath.Count - 1]);
            }
            else if (singlePath.Count > 1 && RuGoInteraction.Instance.IsConfirmReleased)
            {
                Debug.Log("Mouse up detected.");
                storePosition();
                singlePath = getEqualDistancePoints(singlePath);
                pathCompleteCallBack(singlePath.ToArray());
                Deactivate();
            }
        }
    }


    // Store the mouse position in world coordinates
    private void storePosition()
    {
        Ray ray = RuGoInteraction.Instance.SelectorRay;
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // if nothing is in the path
            if (singlePath.Count != 0)
            {
                Debug.Log("****** Prepare to place item ******");
                // Grab the previous mark position
                Vector3 previousMark = singlePath[singlePath.Count - 1];

                // check if the y coordinate is off from the previous point
                float previousY = previousMark.y;
                float distance = Vector3.Distance(hit.point, previousMark);

                if (Math.Abs(hit.point.y - previousY) < levelTolerance &&
                    distance >= dominoDistance)
                {
                    Debug.Log("****** Place item ******");
                    singlePath.Add(hit.point);
                }
                else
                {
                    Debug.Log("****** FAILED TO PLACE ******");
                }
            }
            else
            {
                Debug.Log("****** Place first item ******");
                singlePath.Add(hit.point);
            }
        }
    }


    private List<Vector3> getEqualDistancePoints(List<Vector3> myPoints)
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
                    int count = (int)(segmentLength / dominoDistance);
                    for (int n = 0; n < count; n++)
                    {
                        Vector3 newPoint = previousPoint + segmentVector * dominoDistance;
                        points.Add(newPoint);
                        previousPoint = newPoint;
                    }
                    leftover = segmentLength - count * dominoDistance;
                }
                else if (Vector3.Distance(previousPoint, myPoints[i]) > dominoDistance)
                {
                    float angle_a = getAngle(myPoints[i - 1] - previousPoint, myPoints[i] - myPoints[i - 1]);
                    float side_b = Vector3.Distance(myPoints[i - 1], previousPoint);
                    float side_c = calculateSide(dominoDistance, side_b, angle_a);
                    float remaining_segment = segmentLength - side_c;

                    Vector3 newPoint = myPoints[i - 1] + side_c * segmentVector;
                    points.Add(newPoint);
                    previousPoint = newPoint;

                    int count = (int)(remaining_segment / dominoDistance);
                    for (int n = 0; n < count; n++)
                    {
                        newPoint = previousPoint + segmentVector * dominoDistance;
                        points.Add(newPoint);
                        previousPoint = newPoint;
                    }
                    leftover = remaining_segment - count * dominoDistance;
                }
            }
        }

        return points;
    }


    // Calculate the third side of the triangle using SSA
    private float calculateSide(float side_a, float side_b, float angle_a)
    {
        float sin_of_angle_a = Mathf.Sin(angle_a * Mathf.PI / 180);
        float angle_b = Mathf.Asin(side_b * sin_of_angle_a / side_a) * 180 / Mathf.PI;
        float angle_c = angle_a + angle_b;

        return Mathf.Sin(angle_c * Mathf.PI / 180) * side_a / sin_of_angle_a;
    }

    // Get angle between two vectors
    private float getAngle(Vector3 from, Vector3 to)
    {
        return 180 - Vector3.Angle(from, to);
    }


    private void markStartingPoint()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, singlePath[0]);
        lineRenderer.SetPosition(1, singlePath[0]);
        lineRenderer.enabled = true;
    }


    public void Activate(Action<Vector3[]> createGadgetsAlongPath)
    {
        isActive = true;
        pathCompleteCallBack = createGadgetsAlongPath;
    }


    public void Deactivate()
    {
        if (isActive)
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 0;
            isActive = false;
            singlePath = new List<Vector3>();
        }
    }
}