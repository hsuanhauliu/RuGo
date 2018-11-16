﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour {

	private Gadget mGadget;
    private Collider mSwitchCollider;

    private void Awake()
    {
        mGadget = this.GetComponentInParent<Gadget>();
        mSwitchCollider = this.GetComponent<Collider>();

        IgnoreCollisionSelf(mGadget.transform);
    }

    private void IgnoreCollisionSelf(Transform t)
    {
        if(t == transform)
        {
            return;
        }
        else
        {
            Collider otherCollider = t.GetComponent<Collider>();
            if(otherCollider != null)
            {
                Physics.IgnoreCollision(mSwitchCollider, otherCollider, true);
            }
        }

        foreach(Transform child in t)
        {
            IgnoreCollisionSelf(child);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(mGadget != null)
        {
            Collider collisionCollider = collision.collider;
            Gadget otherGadget = collisionCollider.gameObject.GetComponentInParent<Gadget>();


            // Avoid trigger due to collision with Floating Platform
            if (otherGadget is Floater) {
                return;
            }


            VRTK.VRTK_PlayerObject isPlayer = collisionCollider.gameObject.GetComponentInParent<VRTK.VRTK_PlayerObject>();
            if ((otherGadget != null && otherGadget.GetPhysicsMode()) || isPlayer != null)
            {
                mGadget.PerformSwitchAction();
            }
        }
    }
}
