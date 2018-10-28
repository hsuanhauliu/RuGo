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

        /* Line renderer settings */
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
                StorePosition();
                MarkStartingPoint();
            }
            else if (singlePath.Count != 0 && RuGoInteraction.Instance.IsConfirmHeld)
            {
                Debug.Log("Mouse hold detected.");
                StorePosition();

                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, singlePath[singlePath.Count - 1]);
            }
            else if (singlePath.Count > 1 && RuGoInteraction.Instance.IsConfirmReleased)
            {
                Debug.Log("Mouse up detected.");
                StorePosition();
                singlePath = GetEqualDistancePoints(singlePath);
                pathCompleteCallBack(singlePath.ToArray());
                Deactivate();
            }
        }
    }

    /************************** Public Functions **************************/

    /// <summary>
    /// Activate the Path Tool.
    /// </summary>
    /// <param name="createGadgetsAlongPath">The function to call back upon finishing drawing.</param>
    public void Activate(Action<Vector3[]> createGadgetsAlongPath)
    {
        isActive = true;
        pathCompleteCallBack = createGadgetsAlongPath;
    }

    /// <summary>
    /// Deactivate the Path Tool.
    /// </summary>
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

    /************************** Private Functions **************************/

    /// <summary>
    /// Store mouse click positions in singlePath vector.
    /// </summary>
    private void StorePosition()
    {
        Ray ray = RuGoInteraction.Instance.SelectorRay;
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // if nothing is in the path
            if (singlePath.Count != 0)
            {
                Debug.Log("****** Prepare to gadget item ******");
                // Grab the previous mark position
                Vector3 previousMark = singlePath[singlePath.Count - 1];

                // check if the y coordinate is off from the previous point
                float previousY = previousMark.y;
                float distance = Vector3.Distance(hit.point, previousMark);

                if (Math.Abs(hit.point.y - previousY) < levelTolerance &&
                    distance >= dominoDistance)
                {
                    Debug.Log("****** Place gadget ******");
                    singlePath.Add(hit.point);
                }
                else
                {
                    Debug.Log("****** FAILED TO PLACE ******");
                }
            }
            else
            {
                Debug.Log("****** Place first gadget ******");
                singlePath.Add(hit.point);
            }
        }
    }

    /// <summary>
    /// Take a list of points and return a list of equally spaced points.
    /// </summary>
    /// <returns>The equal distance points.</returns>
    /// <param name="inputPoints">A list of points of a path.</param>
    private List<Vector3> GetEqualDistancePoints(List<Vector3> inputPoints)
    {
        List<Vector3> newPoints = new List<Vector3>();

        if (inputPoints.Count > 0)
        {
            // Add the first point
            newPoints.Add(inputPoints[0]);
            float leftover = 0f;

            Vector3 previousPoint = inputPoints[0];
            for (int i = 1; i < inputPoints.Count; i++)
            {
                float segmentLength = Vector3.Distance(inputPoints[i], inputPoints[i - 1]);
                Vector3 segmentVector = (inputPoints[i] - inputPoints[i - 1]) / segmentLength;

                if (leftover == 0)
                {
                    int count = (int)(segmentLength / dominoDistance);
                    for (int n = 0; n < count; n++)
                    {
                        Vector3 newPoint = previousPoint + segmentVector * dominoDistance;
                        newPoints.Add(newPoint);
                        previousPoint = newPoint;
                    }
                    leftover = segmentLength - count * dominoDistance;
                }
                else if (Vector3.Distance(previousPoint, inputPoints[i]) > dominoDistance)
                {
                    float angle_a = getAngle(inputPoints[i - 1] - previousPoint, inputPoints[i] - inputPoints[i - 1]);
                    float side_b = Vector3.Distance(inputPoints[i - 1], previousPoint);
                    float side_c = CalculateSide(dominoDistance, side_b, angle_a);
                    float remaining_segment = segmentLength - side_c;

                    Vector3 newPoint = inputPoints[i - 1] + side_c * segmentVector;
                    newPoints.Add(newPoint);
                    previousPoint = newPoint;

                    int count = (int)(remaining_segment / dominoDistance);
                    for (int n = 0; n < count; n++)
                    {
                        newPoint = previousPoint + segmentVector * dominoDistance;
                        newPoints.Add(newPoint);
                        previousPoint = newPoint;
                    }
                    leftover = remaining_segment - count * dominoDistance;
                }
            }
        }
        return newPoints;
    }

    /// <summary>
    /// Calculate the third side of the triangle using SSA method.
    /// </summary>
    /// <returns>The side.</returns>
    /// <param name="side_a">Side A of the triangle.</param>
    /// <param name="side_b">Side B of the triangle.</param>
    /// <param name="angle_a">Angle a of the triangle.</param>
    private float CalculateSide(float side_a, float side_b, float angle_a)
    {
        float sin_of_angle_a = Mathf.Sin(angle_a * Mathf.PI / 180);
        float angle_b = Mathf.Asin(side_b * sin_of_angle_a / side_a) * 180 / Mathf.PI;
        float angle_c = angle_a + angle_b;

        return Mathf.Sin(angle_c * Mathf.PI / 180) * side_a / sin_of_angle_a;
    }

    /// <summary>
    /// Get the angle between two vectors.
    /// </summary>
    /// <returns>The angle between two vectors.</returns>
    /// <param name="from">Vector 1.</param>
    /// <param name="to">>Vector 2.</param>
    private float getAngle(Vector3 from, Vector3 to)
    {
        return 180 - Vector3.Angle(from, to);
    }

    /// <summary>
    /// Marks the starting point of the Line Renderer.
    /// </summary>
    private void MarkStartingPoint()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, singlePath[0]);
        lineRenderer.SetPosition(1, singlePath[0]);
        lineRenderer.enabled = true;
    }
}