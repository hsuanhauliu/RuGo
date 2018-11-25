using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour {

	private Gadget mGadget;
    private Collider mSwitchCollider;
    private Animation mAnimation;

    private void Awake()
    {
        mAnimation = this.GetComponent<Animation>(); 
        mGadget = this.GetComponentInParent<Gadget>();
        mSwitchCollider = this.GetComponent<Collider>();

        if(mGadget != null)
        {
            IgnoreCollisionSelf(mGadget.transform);
        }
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

    void OnTriggerEnter(Collider collisionCollider)
    {
        if (mGadget != null)
        {
            Gadget otherGadget = collisionCollider.gameObject.GetComponentInParent<Gadget>();

            // Avoid trigger due to collision with Floating Platform
            if (otherGadget is Floater || otherGadget is BoxGadget) {
                return;
            }

            if (otherGadget != null && otherGadget.GetPhysicsMode())
            {
                PerformGadgetAction();
            }
        }
    }

    public void PerformGadgetAction()
    {
        if (mGadget != null && mAnimation != null && !mAnimation.isPlaying)
        {
            mAnimation.Play();
            mGadget.PerformSwitchAction();
        }
    }
}
