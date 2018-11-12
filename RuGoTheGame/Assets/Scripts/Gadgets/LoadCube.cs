using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCube : Gadget
{
    public string Slot;

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.NUM; // #TODO: If time permits, figure out gadget types for this guy.
    }

    protected override void UnGrabGradget()
    {
        World.Instance.LoadSaveSlot(Slot);
        RemoveFromScene();
        World.Instance.RespawnFiles();
    }
}
