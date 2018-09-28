using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominoGadget : Gadget {

    protected override List<Renderer> GetRenderers()
    {
        List<Renderer> renderers = new List<Renderer>(this.gameObject.GetComponentsInChildren<Renderer>());
        return renderers;
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Domino;
    }
}
