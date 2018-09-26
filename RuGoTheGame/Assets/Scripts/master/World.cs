using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private List<Gadget> gadgetsInWorld;

    void Start()
    {
        gadgetsInWorld = new List<Gadget>();
    }


    public void Reset()
    {
        gadgetsInWorld.ForEach((Gadget g) => g.Reset());
    }

    public void InsertGadget(Gadget g) {
        gadgetsInWorld.Add(g);
    }

    public void CreateGadgetFromTemplate(Gadget gadgetTemplate)
    {
        Debug.Log("A new gameObject has been created and inserted in the World.");

        GameObject gadgetObj = Instantiate(gadgetTemplate.gameObject, this.transform);
        Gadget gadget = gadgetObj.GetComponent<Gadget>();
        gadget.MakeSolid();
        gadgetsInWorld.Add(gadget);
    }

    public void RemoveGadget(Gadget gadget)
    {
        gadgetsInWorld.Remove(gadget);
        gadget.RemoveFromScene();
    }
}
