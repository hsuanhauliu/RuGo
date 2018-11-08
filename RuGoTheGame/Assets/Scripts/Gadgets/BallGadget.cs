using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGadget : Gadget
{
	protected AudioSource[] mAudioData;

	new void Start()
    {
    	base.Start();
        mAudioData = this.GetComponents<AudioSource>();
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Ball;
    }

    void OnCollisionEnter(Collision col)
    {
        print("\ncalling OnCollisionEnter with " + this.name + " and " + col.gameObject.name);
        if (col.gameObject.name == "Ball" || col.gameObject.name == "button")
        {
        	mAudioData[0].Play();
        }
    }

    void FixedUpdate() 
    {
    	if (!mAudioData[1].isPlaying)
        {
            mAudioData[1].Play();
        }
		else
        {
            mAudioData[1].pitch = this.GetComponent<Rigidbody>().velocity.magnitude / 0.35f;
        }
    }
}
