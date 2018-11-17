using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : Gadget
{
    private readonly float amplitude = 0.0005f;
    private readonly float frequency = 1.3f;

    new void Update()
    {
        base.Update();

        if (CurrentGadgetState == GadgetState.InShelf)
        {
            transform.localPosition += (Vector3.up * (Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude));
        }
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Floater;
    }

    public override void ShowShelf(bool show)
    {
        // avoid accumulating position error in shelf
        transform.localPosition = Vector3.zero;
    }

    protected override void SetPhysicsMode(bool enable, bool keepCollision = false)
    {
        Rigidbody[] rigidBodies = this.GetComponentsInChildren<Rigidbody>();

        if (rigidBodies.Length > 0)
        {
            foreach (Rigidbody body in rigidBodies)
            {
                body.isKinematic = true;
                body.detectCollisions = enable || keepCollision;
            }
        }
        isPhysicsMode = enable;
    }
}
