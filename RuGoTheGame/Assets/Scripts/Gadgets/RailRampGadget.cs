using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailRampGadget : Gadget
{
    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Ramp;
    }
}
