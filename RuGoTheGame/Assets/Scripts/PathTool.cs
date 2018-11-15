using System;
using UnityEngine;
using System.Collections.Generic;


public class PathTool : MonoBehaviour
{
    [Tooltip("Maximum ray distance to check for when calculating minimum number of dominos required in draw mode.")]
    public float maxRayDistance = 2f;
    [Tooltip("Minimum ray distance to check for when calculating minimum number of dominos required in draw mode.")]
    public float minRayDistance = 0f;
    [Tooltip("Maximum threshold to limit the number of dominos being spawned.")]
    public int maxGadgetLimit = 8;
    [Tooltip("Minimum threshold to limit the number of dominos being spawned.")]
    public int minGadgetLimit = 1;
    [Tooltip("Maximum y distance limit for dominos to be placed.")]
    public float yLevelTolerance = 0.02f;
    [Tooltip("Minimum distance between each gadget to be placed.")]
    public float minGadgetDistance = 0.04f;

    private Action<Vector3[], float> pathCompleteCallBack;
    private List<Vector3> drawingPath;
    private float dominoThreshold;
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

        AddNewPoint();
    }


    /// <summary>
    /// Stores mouse click positions in drawingPath vector.
    /// </summary>
    private void AddNewPoint()
    {
        RaycastHit hit = mPointerRenderer.GetDestinationHit();
        Vector3 contactPointNormal = hit.normal;
        int normal_x = (int)(contactPointNormal.x * 100);
        int normal_y = (int)(contactPointNormal.y * 100);
        int normal_z = (int)(contactPointNormal.z * 100);
        
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
                    VisualizeNewPoint();
                }
            }
            else
            {
                // claculate the minimum number of dominos required to draw the path
                float interp = Mathf.InverseLerp(minRayDistance, maxRayDistance, hit.distance);
                dominoThreshold = Mathf.Lerp(minGadgetLimit, maxGadgetLimit, interp);

                drawingPath.Add(hit.point);
                VisualizeStartingPoint();
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
                float distanceBetweenTwoPoints = Vector3.Distance(drawingPath[i], drawingPath[i - 1]);
                Vector3 distanceVector = (drawingPath[i] - drawingPath[i - 1]) / distanceBetweenTwoPoints;

                if (Mathf.Approximately(leftover, 0.0f))
                {
                    int numOfDominosToBePlaced = (int)(distanceBetweenTwoPoints / minGadgetDistance);
                    for (int n = 0; n < numOfDominosToBePlaced; n++)
                    {
                        Vector3 newPoint = previousPoint + distanceVector * minGadgetDistance;
                        newPoints.Add(newPoint);
                        previousPoint = newPoint;
                    }
                    leftover = distanceBetweenTwoPoints - numOfDominosToBePlaced * minGadgetDistance;
                }
                else if (Vector3.Distance(previousPoint, drawingPath[i]) > minGadgetDistance)
                {
                    float angle_a = CalculateAngle(drawingPath[i - 1] - previousPoint, drawingPath[i] - drawingPath[i - 1]);
                    float side_b = Vector3.Distance(drawingPath[i - 1], previousPoint);
                    float side_c = Mathf.Approximately(angle_a, 0.0f) ? 0 : CalculateSide(minGadgetDistance, side_b, angle_a);
                    float remaining_segment = distanceBetweenTwoPoints - side_c;

                    Vector3 newPoint = drawingPath[i - 1] + side_c * distanceVector;
                    newPoints.Add(newPoint);
                    previousPoint = newPoint;

                    int numOfDominosToBePlaced = (int)(remaining_segment / minGadgetDistance);
                    for (int n = 0; n < numOfDominosToBePlaced; n++)
                    {
                        newPoint = previousPoint + distanceVector * minGadgetDistance;
                        newPoints.Add(newPoint);
                        previousPoint = newPoint;
                    }
                    leftover = remaining_segment - numOfDominosToBePlaced * minGadgetDistance;
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
        float temp = side_b * sin_of_angle_a / side_a;
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
    public void Activate(Action<Vector3[], float> createGadgetsAlongPath)
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
        pathCompleteCallBack(drawingPath.ToArray(), dominoThreshold);
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