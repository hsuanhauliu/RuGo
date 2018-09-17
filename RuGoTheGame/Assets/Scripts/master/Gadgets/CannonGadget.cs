using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonGadget : Gadget
{
    private GameObject mCannonBallPrefab;

    private Transform mBarrel;

    private void Start()
    {
        mCannonBallPrefab = Resources.Load("CannonBall") as GameObject;
        mBarrel = this.transform.Find("Wooden_pillow");

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject cannonBall = Instantiate(mCannonBallPrefab, mBarrel);
            Rigidbody rigidBody = cannonBall.GetComponent<Rigidbody>();

            Vector3 barrelDirection = cannonBall.transform.up;
            rigidBody.AddForce(barrelDirection * 1.3f, ForceMode.Impulse);
        }

    }

}
