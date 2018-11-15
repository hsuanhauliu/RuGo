using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerGadget : Gadget
{
    public GameObject Spindle;

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Spinner;
    }

    protected override void SetPhysicsMode(bool enable, bool keepCollision = false)
    {
        Rigidbody[] rigidBodies = this.GetComponentsInChildren<Rigidbody>();

        if (rigidBodies.Length > 0)
        {
            foreach (Rigidbody body in rigidBodies)
            {
                bool shouldBeKinematic = !enable;
                if(body.gameObject == Spindle)
                {
                    shouldBeKinematic = true;
                }

                body.isKinematic = shouldBeKinematic;
                body.detectCollisions = enable || keepCollision;
            }
        }
        isPhysicsMode = enable;
    }
}
