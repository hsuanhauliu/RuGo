using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalGadget : Gadget {
    public bool IsGoalComplete = false;
    private BoxCollider mCollider;

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Goal;
    }

    new void Start () {
        base.Start();
      
        mCollider = this.GetComponent<BoxCollider>();
       
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

    protected override void GadgetTouched()
    {
        // Do Nothing To Avoid Deleting Gadgets by Touch
    }

    public void SetGoalInWorld()
    {
        ChangeState(GadgetState.InWorld);
        MakeSolid();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<Gadget>() != null && !GameManager.Instance.IsGameOver)
        {
            this.IsGoalComplete = true;

            World.Instance.NotifyGoalComplete();

           

            LineRenderer[] lasers = this.GetComponentsInChildren<LineRenderer>();
            foreach (LineRenderer laser in lasers)
            {
                laser.material.color = Color.green;
                laser.material.SetColor("_TintColor", Color.green);
                laser.startColor = Color.green;
                laser.endColor = Color.green;
            }
        }
    }
}
