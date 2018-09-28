using System;
using UnityEngine;
using System.Collections.Generic;

public class PathTool : MonoBehaviour
{
    public float MinGap = 0.04f;
    public float LevelTolerance = 0.00001f;
    public float DominoDistance = 0.04f;

    private Action<Vector3[]> mPathCompleteCallBack;
    private bool isActive = false;
    private List<Vector3> singlePath;

    void Start()
    {
        singlePath = new List<Vector3>();
    }

    void Update()
    {
        if (isActive)
        {

            if (RuGoInteraction.Instance.IsConfirmPressed)
            {
                print("Pressed");
                StorePosition();
            }
            else if (singlePath.Count != 0 && RuGoInteraction.Instance.IsConfirmHeld)
            {
                print("Hold");
                StorePosition();
            }
            else if (singlePath.Count != 0 && RuGoInteraction.Instance.IsConfirmReleased)
            {
                print("Released");
                StorePosition();
                singlePath = GetEqualDistancePoints(singlePath);
                mPathCompleteCallBack(singlePath.ToArray());
                Deactivate();
            }
        }
    }

    // store the mouse position in world coordinates
    private void StorePosition()
    {
        Ray ray = RuGoInteraction.Instance.SelectorRay;
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

    public void Deactivate() {
        isActive = false;
        singlePath = new List<Vector3>();
    }

    public void Activate(Action<Vector3[]> createGadgetsAlongPath)
    {
        isActive = true;
        mPathCompleteCallBack = createGadgetsAlongPath;
    }
}