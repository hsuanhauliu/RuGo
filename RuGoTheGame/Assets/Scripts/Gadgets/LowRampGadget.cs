using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowRampGadget : Gadget
{
    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.LowRamp;
    }
}
