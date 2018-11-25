using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalGadget : Gadget {
    public bool IsGoalComplete = false;
    private Collider mCollider;

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Goal;
    }

    new void Start () {
        base.Start();
      
        mCollider = this.GetComponent<Collider>();
       
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
        Gadget otherGadget = other.gameObject.GetComponentInParent<Gadget>();

        if (otherGadget == null || otherGadget is Floater || otherGadget is BoxGadget)
        {
            return;
        }

        if (!GameManager.Instance.IsGameOver)
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
