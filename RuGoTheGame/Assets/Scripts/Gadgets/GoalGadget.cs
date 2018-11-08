using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalGadget : Gadget {
    public GameObject RocketPrefab;
    public GameObject Confetti;

    private AudioSource mAudioData;

    private ParticleSystem mFireWorks;
    public Transform LeftPole;
    public Transform RightPole;
    public float THICKNESS = 0.01f;
    private BoxCollider mCollider;

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.NUM; // #TODO: This should be changed to .Goal
    }

    new void Start () {
        base.Start();
      
        mCollider = this.GetComponent<BoxCollider>();
        mAudioData = GetComponent<AudioSource>();

        mCollider.center = ((LeftPole.localPosition + RightPole.localPosition) / 2.0f);
        mCollider.center += new Vector3(0, transform.GetChild(0).localPosition.y, 0);

        float height = LeftPole.GetChild(0).position.y - LeftPole.GetChild(2).position.y;
        float width = RightPole.GetChild(1).position.x - LeftPole.GetChild(1).position.x;
        mCollider.size = new Vector3(width, height, THICKNESS);
        IgnoreCollisionSelf(this.transform);
	}
	
    //TODO: Can we remove this redundancy?
    private void IgnoreCollisionSelf(Transform t)
    {
        if (t == transform)
        {
            return;
        }
        else
        {
            Collider otherCollider = t.GetComponent<Collider>();
            if (otherCollider != null)
            {
                Physics.IgnoreCollision(mCollider, otherCollider, true);
            }
        }

        foreach (Transform child in t)
        {
            IgnoreCollisionSelf(child);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<Gadget>() != null) {
            mFireWorks = Instantiate(RocketPrefab).GetComponent<ParticleSystem>();
            LineRenderer[] lasers = this.GetComponentsInChildren<LineRenderer>();
            foreach (LineRenderer laser in lasers)
            {
                laser.material.color = Color.green;
                laser.material.SetColor("_TintColor", Color.green);
                laser.startColor = Color.green;
                laser.endColor = Color.green;
            }

            mFireWorks.Play(true);
            mAudioData.Play();
            
            GameObject go = Instantiate(Resources.Load("Confetti")) as GameObject;
            //TODO ADD CONFETTI
        }
    }
}
