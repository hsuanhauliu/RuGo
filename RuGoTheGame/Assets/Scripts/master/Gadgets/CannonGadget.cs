using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonGadget : Gadget
{
    private GameObject mCannonBallPrefab;
    private LineRenderer mTrajectory;

    private Transform mBarrel;

    private float mass = 0.5f;

    new void Start()
    {
        base.Start();
        mCannonBallPrefab = Resources.Load("CannonBall") as GameObject;
        mBarrel = this.transform.Find("SmallCannon").Find("Wooden_pillow");

        mTrajectory = mBarrel.gameObject.GetComponent<LineRenderer>();
        if (mTrajectory == null) 
        {
            mTrajectory = mBarrel.gameObject.AddComponent<LineRenderer>();
        }
        mTrajectory.startWidth = 0.01f;
        mTrajectory.endWidth = 0.01f;
        //mTrajectory.SetVertexCount(29);
    }

    public override void PerformSwitchAction()
    {
        FireCannon();
    }

    private void Update()
    {
        if (mTrajectory != null) 
        {
            PlotTrajectory();   
        }
        if (Input.GetKeyDown(KeyCode.F) && this.GetPhysicsMode())
        {
            FireCannon(); 
        }
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.SmallCannon;
    }

    private void PlotTrajectory()
    {
        Vector3 start = mBarrel.transform.position;

        if (mTrajectory.positionCount == 0 || mTrajectory.GetPosition(0) != start)
        {
            List<Vector3> trajectory_points = new List<Vector3>();
            
            GameObject temp_ball = Instantiate(mCannonBallPrefab, mBarrel);
            Vector3 initialVelocity = temp_ball.transform.up * 1.3f / mass;
            Destroy(temp_ball);

            Vector3 prev = start;
            int i;
            for (i = 0; i < 60; i++) {
                // mTrajectory.SetPosition(i, prev);
                trajectory_points.Add(prev);
                float t = 0.01f * i;

                Vector3 pos = start + initialVelocity * t + Physics.gravity * t * t * 0.5f;
                
                if (!Physics.Linecast(prev,pos))
                {
                    prev = pos;
                } 
            }

            mTrajectory.SetVertexCount(i);
            for (int j = 0; j < i; j++) 
            {
                mTrajectory.SetPosition(j, trajectory_points[j]);
            }
        }
    }

    private void FireCannon() 
    {
        GameObject cannonBall = Instantiate(mCannonBallPrefab, mBarrel);
        Rigidbody rigidBody = cannonBall.GetComponent<Rigidbody>();

        Vector3 barrelDirection = cannonBall.transform.up * 1.3f;
        //print(barrelDirection);
        rigidBody.AddForce(barrelDirection, ForceMode.Impulse);

        //IEnumerator coroutine = CleanCannon(cannonBall);
        //StartCoroutine(coroutine);
    }

    private IEnumerator CleanCannon(GameObject cannonBall) 
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(cannonBall);
    }
}
