using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneGadget : Gadget
{
    private Transform mBlades;

    protected new void Awake()
    {
        base.Awake();

        mBlades = transform.Find("Blades");
    }

    protected override List<Renderer> GetRenderers()
    {
        List<Renderer> renderers = new List<Renderer>(this.gameObject.GetComponentsInChildren<Renderer>());
        return renderers;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // We need to redo this somehow.
        this.transform.Translate(Vector3.left * 10f);
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Airplane;
    }
}
