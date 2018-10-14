using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARSpaceManager : MonoBehaviour {
    public Camera LeftCameraEyes;
    public Transform debugSphere;

    private bool mIsZedReady; 

	// Use this for initialization
	void Start () {
		
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
        if (LeftCameraEyes)
        {
            mIsZedReady = true;
        }
    }

    private bool HitTestAlongRay(Ray ray, out Vector3 collisionPoint, float maxDistance = 5.0f, float distBetweenTests = 0.01f)
    {
        //Check for occlusion in a series of dots, spaced apart evenly.
        for (float i = 0; i <= maxDistance; i += distBetweenTests)
        {
            Vector3 pointtocheck = ray.GetPoint(i);
            bool hit = ZEDSupportFunctions.HitTestAtPoint(LeftCameraEyes, pointtocheck);

            Transform point = Instantiate(debugSphere, this.transform);
            point.position = pointtocheck;
            point.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            if (hit)
            {
                //Return the last valid place before the collision.
                collisionPoint = pointtocheck;
                Debug.Log("Collision: " + collisionPoint.ToString() + " distance: " + i);
                return true;
            }
        }

        //There was no collision at any of the points checked. 
        collisionPoint = ray.GetPoint(maxDistance);
        Debug.Log("Final: " + collisionPoint.ToString());
        return false;
    }

    public void FetchPlane()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Vector3 collisionPoint;
        bool isCollision = HitTestAlongRay(RuGoInteraction.Instance.SelectorRay, out collisionPoint); // Perform test using a max distance of 3m and 1cm increments.

        debugSphere.position = collisionPoint;
        debugSphere.GetComponent<Renderer>().material.color = isCollision ? Color.green : Color.red;
    }
}
