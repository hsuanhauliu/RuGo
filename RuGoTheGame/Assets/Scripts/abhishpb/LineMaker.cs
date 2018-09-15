
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

 
 public class LineMaker : MonoBehaviour
{

    public GameObject gameObject1;          // Reference to the first GameObject
    public GameObject gameObject2;          // Reference to the second GameObject

    private LineRenderer line;                           // Line Renderer

    // Use this for initialization
    void Start()
    {
        // Add a Line Renderer to the GameObject
        line = this.gameObject.AddComponent<LineRenderer>();
        // Set the width of the Line Renderer
        //line.SetWidth(0.05F, 0.05F);
        line.startWidth = 0.05f;
        // Set the number of vertex fo the Line Renderer
        //line.SetVertexCount(2);
        line.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the GameObjects are not null
        if (gameObject1 != null && gameObject2 != null)
        {
            // Update position of the two vertex of the Line Renderer
            Vector3 point1 = gameObject1.transform.position;
            point1.y = gameObject1.transform.lossyScale.y * 1.5f;
            Vector3 point2 = gameObject2.transform.position;
            point2.y = gameObject1.transform.lossyScale.y * 1.5f;
            line.SetPosition(0, point1);
            line.SetPosition(1, point2);
        }
    }
}