using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : Gadget
{
    public float amplitude = 0.001f;
    public float frequency = 1f;

    new void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (CurrentGadgetState == GadgetState.InShelf) {
            transform.position += (transform.up * (Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude));
        }
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Floater;
    }

}
