using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanGadget : Gadget {

    protected override List<Renderer> GetRenderers()
    {
        List<Renderer> renderers = new List<Renderer>(this.gameObject.GetComponentsInChildren<Renderer>());
        return renderers;
    }

    private Transform blades;

    private Transform shaft;

    void Start()
    {
        blades = this.transform.Find("Blades");
        shaft = this.transform.Find("Shaft");
    }

    void Update()
    {
        blades.Rotate(new Vector3(0, 0, 20));
    }
}
