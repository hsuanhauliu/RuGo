using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonGadget : Gadget
{
    private GameObject mCannonBallPrefab;

    private Transform mBarrel;

    new void Start()
    {
        base.Start();
        mCannonBallPrefab = Resources.Load("CannonBall") as GameObject;
        mBarrel = this.transform.Find("SmallCannon").Find("Wooden_pillow");
    }

    public override void PerformSwitchAction()
    {
        FireCannon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && this.GetPhysicsMode())
        {
            FireCannon(); 
        }

    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.SmallCannon;
    }

    private void FireCannon() 
    {
        GameObject cannonBall = Instantiate(mCannonBallPrefab, mBarrel);
        Rigidbody rigidBody = cannonBall.GetComponent<Rigidbody>();

        Vector3 barrelDirection = cannonBall.transform.up;
        rigidBody.AddForce(barrelDirection * 1.3f, ForceMode.Impulse);

        IEnumerator coroutine = CleanCannon(cannonBall);
        StartCoroutine(coroutine);
    }

    private IEnumerator CleanCannon(GameObject cannonBall) 
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(cannonBall);
    }
}
