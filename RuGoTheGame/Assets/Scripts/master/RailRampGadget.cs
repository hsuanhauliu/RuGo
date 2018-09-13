using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailRampGadget : Gadget {
   
    private Renderer[] GadgetRenderers {
        get {
            return this.gameObject.GetComponentsInChildren<Renderer>();
        }
    }

    public override void Highlight()
    {
        Debug.Log("Highlighting Gadget");

        foreach (Renderer GadgetRenderer in GadgetRenderers) {
            GadgetRenderer.material.color = Color.yellow;
        }
    }

    public override void Solidify()
    {
        Debug.Log("Making Gadget Solid");
        foreach (Renderer GadgetRenderer in GadgetRenderers)
        {
            Color albedo = GadgetRenderer.material.color;
            albedo.a = 1.0f;
            GadgetRenderer.material.color = albedo;
        }
    }

    public override void Transparent()
    {
        Debug.Log("Making Gadget transparent");
        foreach (Renderer GadgetRenderer in GadgetRenderers)
        {
            Color albedo = GadgetRenderer.material.color;
            albedo.a = 0.5f;
            GadgetRenderer.material.color = albedo;
        }
       
    }
}
