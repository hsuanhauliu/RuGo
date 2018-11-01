using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeHelper : MonoBehaviour {
    public float Length;
    public float ConeHalfAngle;

	// Use this for initialization
	void Start () {
        BuildFanAffect();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private readonly int mSections = 20;

    private void BuildFanAffect()
    {
        float radius = Length * Mathf.Tan(ConeHalfAngle * Mathf.Deg2Rad);

        var meshFilter = transform.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        var mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }
        mesh.Clear();

        if (mSections < 3)
        {
            Debug.LogError("Number of viewcone sections must be 3 or more");
            return;
        }

        float sectionStep = (2 * Mathf.PI) / mSections;
        float sectorAngle = 2 * Mathf.PI; // start in 360 and going decrement

        var coneVertices = new Vector3[mSections + 1 + 1];

        coneVertices[0] = new Vector3(Length, 0, 0); // center of cone

        for (var i = 1; i < (mSections + 1); i++)
        {
            coneVertices[i] = new Vector3(Length, Mathf.Sin(sectorAngle) * radius, Mathf.Cos(sectorAngle) * radius);
            sectorAngle += sectionStep;
        }

        coneVertices[coneVertices.Length - 1] = new Vector3(0, 0, 0); // center of circle

        var idx = 1;
        var indices = (mSections) * 3; // Only for circle triangles
        indices *= 2; // X2 for every triangle in wall of cone

        // Build cone Traingles
        var coneTriangles = new int[indices]; // one triagle for each section (has 3 vertex per triangle)

        // Cone Circle as triangles
        for (var i = 0; i < indices * .5; i += 3)
        {
            coneTriangles[i] = 0; //center of circle
            coneTriangles[i + 1] = idx; //next vertex

            // Keep building indices till last vertex and then loop it back to 1
            if (i >= indices * .5 - 3)
            {
                coneTriangles[i + 2] = 1;
            }
            else
            {
                coneTriangles[i + 2] = idx + 1;
            }
            idx++;
        }

        idx = 1;
        // Cone Wall as triangles 
        for (var i = (int)(indices * .5); i < indices; i += 3)
        {

            coneTriangles[i] = idx; //next vertex
            coneTriangles[i + 1] = coneVertices.Length - 1; // Peak vertex


            if (i >= indices - 3)
            {
                coneTriangles[i + 2] = 1;
            }
            else
            {
                coneTriangles[i + 2] = idx + 1;
            }

            idx++;
        }

        mesh.vertices = coneVertices;
        mesh.triangles = coneTriangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
