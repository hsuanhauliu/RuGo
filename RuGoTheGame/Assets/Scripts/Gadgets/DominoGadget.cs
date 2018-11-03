using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominoGadget : Gadget
{
    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Domino;
    }
}
