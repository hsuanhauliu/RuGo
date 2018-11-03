using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour {
	
    protected AudioSource mAudioData;

    void Start()
    {
        mAudioData = this.transform.parent.GetComponent<AudioSource>();
    }

	void OnCollisionEnter(Collision col)
    {
        // print("\ncalling OnCollisionEnter with " + this.name + " and " + col.gameObject.name);
        if (this.name == "BallGeo") {
            if(col.gameObject.name == "BallGeo" 
                || col.gameObject.name == "BoxBody" 
                || col.gameObject.name == "button"
                )
            {
                mAudioData.Play(0);
            }
        }
        if (this.name == "DominoBody") {
            if(col.gameObject.name == "DominoBody")
            {
                mAudioData.Play(0);
            }
        }
    }
}