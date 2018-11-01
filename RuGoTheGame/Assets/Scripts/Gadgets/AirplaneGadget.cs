using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneGadget : Gadget
{
    protected override List<Renderer> GetRenderers()
    {
        List<Renderer> renderers = new List<Renderer>(this.gameObject.GetComponentsInChildren<Renderer>());
        return renderers;
    }

    private void OnCollisionEnter(Collision collision)
    {
        this.transform.Translate(Vector3.left * 10f);
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Airplane;
    }
}
