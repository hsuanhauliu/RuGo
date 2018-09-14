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

    void Update()
    {

    }

    /************************** Public Functions **************************/
    public void CreateGadgetFromTemplate(Gadget gadgetTemplate)
    {
        Debug.Log("A new gameObject has been created in the World.");

        GameObject gadgetObj = Instantiate(gadgetTemplate.gameObject, this.transform);
        Gadget gadget = gadgetObj.GetComponent<Gadget>();
        gadget.Solidify();
        gadgetsInWorld.Add(gadget);
    }

    internal void Reset()
    {
        gadgetsInWorld.ForEach((Gadget g) => g.Reset());
    }
}
