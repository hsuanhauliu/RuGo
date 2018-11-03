using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumGadget : Gadget
{
    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Pendulum;
    }
}
