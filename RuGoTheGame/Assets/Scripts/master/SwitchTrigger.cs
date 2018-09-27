using System.Collections;
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
            Collider collider = collision.collider;
            Gadget otherGadget = collider.gameObject.GetComponentInParent<Gadget>();
            if (otherGadget != null && otherGadget.GetPhysicsMode())
            {
                mGadget.PerformSwitchAction();
            }
        }
    }
}
