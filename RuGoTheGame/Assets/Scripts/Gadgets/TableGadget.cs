using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableGadget : Gadget
{
    new void Awake()
    {

    }

    public override GadgetInventory GetGadgetType()
    {
        //TODO table is a special gadget. Need to figure out whether it should be in GadgetInventory...
        return GadgetInventory.NUM;
    }
}
