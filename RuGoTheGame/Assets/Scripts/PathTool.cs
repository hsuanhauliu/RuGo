using System;
using UnityEngine;
using System.Collections.Generic;


public class PathTool : MonoBehaviour
{
    private const float yLevelTolerance = 0.005f;
    private const float minGadgetDistance = 0.04f;

    private Action<Vector3[]> pathCompleteCallBack;
    private List<Vector3> drawingPath;
    private LineRenderer pathVisualizer;

    private VRTK.VRTK_Pointer mPointer;
    private VRTK.VRTK_StraightPointerRenderer mPointerRenderer;


    void Start()
    {
        EmptyPath();
        SetUpVisualizer();
        this.enabled = false;

        mPointer = GameManager.Instance.RightControllerEvents.gameObject.GetComponent<VRTK.VRTK_Pointer>();
        mPointerRenderer = GameManager.Instance.RightControllerEvents.gameObject.GetComponent<VRTK.VRTK_StraightPointerRenderer>();
    }

    void Update()
    {
        if (GameManager.Instance.CurrentGameMode != GameMode.DRAW)
            return;

        StorePointPosition();

        if (drawingPath.Count == 1)
        {
            VisualizeStartingPoint();
        }
        else if (drawingPath.Count > 1)
        {
            VisualizeNewPoint();
        }
    }

 

    /// <summary>
    /// Stores mouse click positions in drawingPath vector.
    /// </summary>
    private void StorePointPosition()
    {
        RaycastHit hit = mPointerRenderer.GetDestinationHit();
        Vector3 contactPointNormal = hit.normal;
        int normal_x = (int)contactPointNormal.x;
        int normal_y = (int)contactPointNormal.y;
        int normal_z = (int)contactPointNormal.z;

        if (hit.collider != null &&
            !(normal_x == 0 && normal_y == 0) &&
            !(normal_z == 0 && normal_y == 0) &&
            !(normal_x == 0 && normal_y < 0 && normal_z == 0))
        {
            if (drawingPath.Count != 0)
            {
                Vector3 previousPoint = drawingPath[drawingPath.Count - 1];
                float previousY = previousPoint.y;
                float gap = Vector3.Distance(hit.point, previousPoint);

                if (Math.Abs(hit.point.y - previousY) < yLevelTolerance &&
                    gap >= minGadgetDistance)
                {
                    drawingPath.Add(hit.point);
                }
            }
            else
            {
                drawingPath.Add(hit.point);
            }
        }        
    }


    /// <summary>
    /// Equalizes the distance between each point in the drawing path.
    /// </summary>
    private void EqualizePointDistances()
    {
        List<Vector3> newPoints = new List<Vector3>();

        if (drawingPath.Count > 0)
        {
            newPoints.Add(drawingPath[0]);
            float leftover = 0f;

            Vector3 previousPoint = drawingPath[0];
            for (int i = 1; i < drawingPath.Count; i++)
            {
                float segmentLength = Vector3.Distance(drawingPath[i], drawingPath[i - 1]);
                Vector3 segmentVector = (drawingPath[i] - drawingPath[i - 1]) / segmentLength;

                if (leftover == 0)
                {
                    int count = (int)(segmentLength / minGadgetDistance);
                    for (int n = 0; n < count; n++)
                    {
                        Vector3 newPoint = previousPoint + segmentVector * minGadgetDistance;
                        newPoints.Add(newPoint);
                        previousPoint = newPoint;
                    }
                    leftover = segmentLength - count * minGadgetDistance;
                }
                else if (Vector3.Distance(previousPoint, drawingPath[i]) > minGadgetDistance)
                {
                    float angle_a = CalculateAngle(drawingPath[i - 1] - previousPoint, drawingPath[i] - drawingPath[i - 1]);
                    float side_b = Vector3.Distance(drawingPath[i - 1], previousPoint);
                    float side_c = CalculateSide(minGadgetDistance, side_b, angle_a);
                    float remaining_segment = segmentLength - side_c;

                    Vector3 newPoint = drawingPath[i - 1] + side_c * segmentVector;
                    newPoints.Add(newPoint);
                    previousPoint = newPoint;

                    int count = (int)(remaining_segment / minGadgetDistance);
                    for (int n = 0; n < count; n++)
                    {
                        newPoint = previousPoint + segmentVector * minGadgetDistance;
                        newPoints.Add(newPoint);
                        previousPoint = newPoint;
                    }
                    leftover = remaining_segment - count * minGadgetDistance;
                }
            }
        }
        drawingPath = newPoints;
    }


    /// <summary>
    /// Calculates the third side of the triangle using SSA method.
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
    /// Calculates the angle between two vectors.
    /// </summary>
    /// <returns>The angle between two vectors.</returns>
    /// <param name="from">Vector 1.</param>
    /// <param name="to">Vector 2.</param>
    private float CalculateAngle(Vector3 from, Vector3 to)
    {
        return 180 - Vector3.Angle(from, to);
    }


    /// <summary>
    /// Sets up line renderer for path visualization.
    /// </summary>
    private void SetUpVisualizer()
    {
        pathVisualizer = gameObject.AddComponent<LineRenderer>();
        pathVisualizer.material = new Material(Shader.Find("Unlit/Texture"));
        pathVisualizer.startColor = Color.white;
        pathVisualizer.endColor = Color.white;
        pathVisualizer.startWidth = 0.02f;
        pathVisualizer.endWidth = 0.02f;
        pathVisualizer.enabled = false;
    }


    /// <summary>
    /// Marks the starting point of the Line Renderer.
    /// </summary>
    private void VisualizeStartingPoint()
    {
        pathVisualizer.positionCount = 2;
        pathVisualizer.SetPosition(0, drawingPath[0]);
        pathVisualizer.SetPosition(1, drawingPath[0]);
        pathVisualizer.enabled = true;
    }


    /// <summary>
    /// Visualizes a newly added point on the drawing path.
    /// </summary>
    private void VisualizeNewPoint()
    {
        int numOfPoints = drawingPath.Count;
        pathVisualizer.positionCount = numOfPoints;
        pathVisualizer.SetPosition(numOfPoints - 1, drawingPath[numOfPoints - 1]);
    }


    /// <summary>
    /// Activates the Path Tool.
    /// </summary>
    /// <param name="createGadgetsAlongPath">The function to call back upon finishing drawing.</param>
    public void Activate(Action<Vector3[]> createGadgetsAlongPath)
    {
        mPointer.Toggle(true);
        mPointerRenderer.validCollisionColor = Color.green;
        mPointerRenderer.invalidCollisionColor = Color.white;

        pathCompleteCallBack = createGadgetsAlongPath;
        this.enabled = true;
    }


    /// <summary>
    /// Deactivates the Path Tool.
    /// </summary>
    public void Deactivate()
    {
        mPointer.Toggle(false);

        EqualizePointDistances();
        pathCompleteCallBack(drawingPath.ToArray());

        this.enabled = false;
        ResetVisualizer();
        EmptyPath();
    }

    /// <summary>
    /// Resets the visualizer.
    /// </summary>
    private void ResetVisualizer()
    {
        pathVisualizer.enabled = false;
        pathVisualizer.positionCount = 0;
    }


    /// <summary>
    /// Empties the drawing path buffer.
    /// </summary>
    private void EmptyPath()
    {
        drawingPath = new List<Vector3>();
    }
}