using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableGadget : Gadget
{
    private void Update()
    {

    }

    protected new void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Gadget");
        //mRenderers = GetRenderers();

        //SetLayer(transform, "Gadget");
        SetPhysicsMode(true);

        /*
        mChildData = new List<Vector3>();
        foreach (Transform child in transform)
        {
            mChildData.Add(child.localPosition);
        }
        */

        //TODO Set the table such that it is not movable for now
        /*
        VRTK.VRTK_InteractableObject interactableObject = gameObject.AddComponent<VRTK.VRTK_InteractableObject>();
        VRTK.GrabAttachMechanics.VRTK_ChildOfControllerGrabAttach childOfController = gameObject.AddComponent<VRTK.GrabAttachMechanics.VRTK_ChildOfControllerGrabAttach>();
        interactableObject.grabAttachMechanicScript = childOfController;
        interactableObject.isGrabbable = true;
        */
    }

    protected override List<Renderer> GetRenderers()
    {
        List<Renderer> renderers = new List<Renderer>(this.gameObject.GetComponentsInChildren<Renderer>());
        return renderers;
    }

    public override GadgetInventory GetGadgetType()
    {
        //TODO table is a special gadget. Need to figure out whether it should be in GadgetInventory...
        return GadgetInventory.NUM;
    }
}
