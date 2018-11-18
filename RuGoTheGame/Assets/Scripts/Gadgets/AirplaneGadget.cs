using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneGadget : Gadget
{
    private Transform mBlades;
    private Rigidbody mRigidbody;

    private float mCurrentBladeSpeed;
    private float mTargetBladesSpeed;

    protected new void Awake()
    {
        base.Awake();

        mBlades = transform.Find("Blades");

        mRigidbody = GetComponent<Rigidbody>();
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Airplane;
    }

    new void Update()
    {
        float factor = Mathf.Min(1.0f, mRigidbody.velocity.sqrMagnitude);
        mTargetBladesSpeed = 60.0f * factor;
        
        mCurrentBladeSpeed = Mathf.Lerp(mCurrentBladeSpeed, mTargetBladesSpeed, Time.deltaTime);
        mBlades.Rotate(Vector3.right, mCurrentBladeSpeed);
    }
}
