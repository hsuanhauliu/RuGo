using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxGadget : Gadget
{
    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Box;
    }
}
