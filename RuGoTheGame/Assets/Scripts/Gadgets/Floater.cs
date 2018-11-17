using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : Gadget
{
    private readonly float amplitude = 0.001f;
    private readonly float frequency = 1.1f;

    new void Update()
    {
        base.Update();

        if (CurrentGadgetState == GadgetState.InShelf)
        {
            transform.localPosition += (Vector3.up * (Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude));
        }
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Floater;
    }

    public override void ShowShelf(bool show)
    {
        // avoid accumulating position error in shelf
        transform.localPosition = Vector3.zero;
    }
}
