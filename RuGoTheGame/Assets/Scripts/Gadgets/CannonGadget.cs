using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonGadget : Gadget
{
    private GameObject mCannonBallPrefab;
    private LineRenderer mTrajectory;
    private Transform mHeading;
    private Transform mBarrelTip;
    private Transform mBarrel;

    private float mass = 0.5f;

    private AudioSource mAudioData;

    new void Awake()
    {
        base.Awake();
        mCannonBallPrefab = Resources.Load("CannonBall") as GameObject;
        mBarrel = this.transform.Find("Wooden_pillow");
        mHeading = mBarrel.Find("Heading");
        mBarrelTip = mBarrel.Find("BarrelTip");

        mTrajectory = mBarrel.gameObject.GetComponent<LineRenderer>();
        if (mTrajectory == null) 
        {
            mTrajectory = mBarrel.gameObject.AddComponent<LineRenderer>();
        }

        mTrajectory.material = new Material(Shader.Find("Unlit/Texture"));
        mTrajectory.startColor = Color.white;
        mTrajectory.endColor = Color.white;
        mTrajectory.startWidth = 0.01f;
        mTrajectory.endWidth = 0.01f;
       
        mAudioData = GetComponent<AudioSource>();
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
        return GadgetInventory.Cannon;
    }

    private void PlotTrajectory()
    {
        Vector3 start = mBarrelTip.position;

        if (mTrajectory.positionCount == 0 || mTrajectory.GetPosition(0) != start)
        {
            List<Vector3> trajectory_points = new List<Vector3>();
            
            Vector3 initialVelocity = mBarrelTip.forward * 1.3f / mass;
            
            Vector3 prev = start;
            int i;
            for (i = 0; i < 60; i++) {
                trajectory_points.Add(prev);
                float t = 0.01f * i;

                Vector3 pos = start + initialVelocity * t + Physics.gravity * t * t * 0.5f;
                
                if (!Physics.Linecast(prev,pos))
                {
                    prev = pos;
                } 
            }

            mTrajectory.positionCount = i;
            for (int j = 0; j < i; j++) 
            {
                mTrajectory.SetPosition(j, trajectory_points[j]);
            }
        }
    }

    public override void MakeSolid()
    {
        base.MakeSolid();
        mTrajectory.enabled = false;
    }

    public override void MakeTransparent(bool keepCollision = false)
    {
        base.MakeTransparent(keepCollision);
        if(mTrajectory != null)
        {
            mTrajectory.enabled = true;
        }
    }

    private void FireCannon() 
    {
        mAudioData.Play(0);
        GameObject cannonBall = Instantiate(mCannonBallPrefab, mBarrel);
        cannonBall.transform.localPosition = mHeading.localPosition;
        Rigidbody rigidBody = cannonBall.GetComponent<Rigidbody>();

        Vector3 barrelDirection = mHeading.forward * 1.3f;
        rigidBody.AddForce(barrelDirection, ForceMode.Impulse);

        IEnumerator coroutine = CleanCannon(cannonBall);
        StartCoroutine(coroutine);
    }

    private IEnumerator CleanCannon(GameObject cannonBall) 
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(cannonBall);
    }
}
