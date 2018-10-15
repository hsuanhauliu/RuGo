using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARSpaceManager : MonoBehaviour {
    public Camera LeftCameraEyes;
    public Camera RightCameraEyes;
    public Transform debugSphere;

    private bool mIsZedReady;

    /// <summary>
    /// Buffer for holding a new plane's vertex data from the SDK.
    /// </summary>
    private Vector3[] planeMeshVertices;
    /// <summary>
    /// Buffer for holding a new plane's triangle data from the SDK.
    /// </summary>
    private int[] planeMeshTriangles;
    /// <summary>
    /// GameObject all planes are parented to. Created at runtime, called '[ZED Planes]' in Hierarchy.
    /// </summary>
    private GameObject holder;

    // Use this for initialization
    void Start () {

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        //initialize Vertices/Triangles with enough length
        planeMeshVertices = new Vector3[65000];
        planeMeshTriangles = new int[65000];

        //Create a holder for all the planes
        holder = new GameObject();
        holder.name = "[ZED Planes]";
        holder.transform.parent = this.transform;
        holder.transform.position = Vector3.zero;
        holder.transform.rotation = Quaternion.identity;
        StaticBatchingUtility.Combine(holder);
    }
	
	// Update is called once per frame
	void Update () {
		if(RuGoInteraction.Instance.IsConfirmPressed)
        {
            FetchPlane();
        }
	}

    private void OnEnable()
    {
        ZEDManager.OnZEDReady += ZEDReady;
    }

    private void OnDisable()
    {
        ZEDManager.OnZEDReady -= ZEDReady;
    }

    void ZEDReady()
    {
        if (LeftCameraEyes && RightCameraEyes)
        {
            mIsZedReady = true;
            ZEDPlaneDetectionManager.isDisplay = true; // This is a zed thing
        }
    }

    // Zed function to get local mesh vertices
    private void TransformCameraToLocalMesh(Transform camera, Vector3[] srcVertices, int[] srcTriangles, Vector3[] dstVertices, int[] dstTriangles, int numVertices, int numTriangles, Vector3 centerpos)
    {
        if (numVertices == 0 || numTriangles == 0)
            return; //Plane is empty. 

        System.Array.Copy(srcVertices, dstVertices, numVertices);
        System.Buffer.BlockCopy(srcTriangles, 0, dstTriangles, 0, numTriangles * sizeof(int));

        for (int i = 0; i < numVertices; i++)
        {
            dstVertices[i] -= centerpos;
            dstVertices[i] = camera.transform.rotation * dstVertices[i];
        }
    }

    private bool HitTestAlongRay(Ray ray, out Vector3 collisionPoint, float maxDistance = 5.0f, float distBetweenTests = 0.01f)
    {
        //Check for occlusion in a series of dots, spaced apart evenly.
        for (float i = 0; i <= maxDistance; i += distBetweenTests)
        {
            Vector3 pointtocheck = ray.GetPoint(i);
            
            bool hit = ZEDSupportFunctions.HitTestAtPoint(LeftCameraEyes, pointtocheck);

            if (hit)
            {
                //Return the last valid place before the collision.
                collisionPoint = pointtocheck;
                Vector2 screenPos = LeftCameraEyes.WorldToScreenPoint(pointtocheck);

                ZEDPlaneGameObject.PlaneData plane = new ZEDPlaneGameObject.PlaneData();
                if (sl.ZEDCamera.GetInstance().findPlaneAtHit(ref plane, screenPos) == sl.ERROR_CODE.SUCCESS) //We found a plane. 
                {
                    int numVertices, numTriangles;
                    sl.ZEDCamera.GetInstance().convertHitPlaneToMesh(planeMeshVertices, planeMeshTriangles, out numVertices, out numTriangles);

                    if (numVertices > 0 && numTriangles > 0)
                    {
                        GameObject newhitGO = new GameObject(); //Make a new GameObject to hold the new plane. 
                                                                //newhitGO.transform.SetParent(transform);
                        newhitGO.transform.SetParent(holder.transform);

                        Vector3[] worldPlaneVertices = new Vector3[numVertices];
                        int[] worldPlaneTriangles = new int[numTriangles];
                        TransformCameraToLocalMesh(LeftCameraEyes.transform, planeMeshVertices, planeMeshTriangles, worldPlaneVertices, worldPlaneTriangles, numVertices, numTriangles, plane.PlaneCenter);

                        //Move the GameObject to the center of the plane. Note that the plane data's center is relative to the camera. 
                        newhitGO.transform.position = LeftCameraEyes.transform.position; //Add the camera's world position 
                        newhitGO.transform.position += LeftCameraEyes.transform.rotation * plane.PlaneCenter; //Add the center of the plane

                        ZEDPlaneGameObject hitPlane = newhitGO.AddComponent<ZEDPlaneGameObject>();

                        hitPlane.Create(plane, worldPlaneVertices, worldPlaneTriangles, 1);

                        hitPlane.SetPhysics(false);
                        hitPlane.SetVisible(true);
                    }

                    collisionPoint = LeftCameraEyes.transform.rotation * plane.PlaneTransformPosition + LeftCameraEyes.transform.position;
                    Debug.Log("Collision: " + collisionPoint.ToString() + " distance: " + i);
                    return true;
                }
            }
        }

        //There was no collision at any of the points checked. 
        collisionPoint = ray.GetPoint(maxDistance);
        Debug.Log("Final: " + collisionPoint.ToString());
        return false;
    }

    public void FetchPlane()
    {
        if (!mIsZedReady)
            return;

        foreach (Transform child in holder.transform)
        {
            Destroy(child.gameObject);
        }

        Vector3 collisionPoint;
        bool isCollision = HitTestAlongRay(RuGoInteraction.Instance.SelectorRay, out collisionPoint); // Perform test using a max distance of 3m and 1cm increments.


        debugSphere.position = collisionPoint;
        debugSphere.GetComponent<Renderer>().material.color = isCollision ? Color.green : Color.red;
    }
}
