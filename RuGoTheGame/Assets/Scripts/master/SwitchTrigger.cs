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
        mGadget.PerformSwitchAction();
    }
}
