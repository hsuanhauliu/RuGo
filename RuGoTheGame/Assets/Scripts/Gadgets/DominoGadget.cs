using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominoGadget : Gadget
{
	protected AudioSource mAudioData;

	new void Start()
    {
    	base.Start();
        mAudioData = this.GetComponent<AudioSource>();
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Domino;
    }

    public void SetDominoInWorld()
    {
        ChangeState(GadgetState.InWorld);
        transform.SetParent(World.Instance.transform);
    }

    void OnCollisionEnter(Collision col)
    {
        // print("\ncalling OnCollisionEnter with " + this.name + " and " + col.gameObject.name);
        Gadget g = col.gameObject.GetComponent<Gadget>();

        if (g != null && g is DominoGadget)
        {
            mAudioData.Play();
        }
    }
}
