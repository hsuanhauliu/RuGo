﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour {

	private Gadget mGadget;

    private void Start()
    {
        mGadget = this.GetComponentInParent<Gadget>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(mGadget != null)
        {
            Collider collisionCollider = collision.collider;
            Gadget otherGadget = collisionCollider.gameObject.GetComponentInParent<Gadget>();
            if (otherGadget != null && otherGadget.GetPhysicsMode())
            {
                mGadget.PerformSwitchAction();
            }
        }
    }
}
