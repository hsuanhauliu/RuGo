using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGadget : Gadget {

    void Start()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;
    }

    public override void Solidify()
    {
        base.Solidify();
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = false;
    }
}
