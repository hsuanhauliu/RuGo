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

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Airplane;
    }
}
