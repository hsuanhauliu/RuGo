using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    // Local variable; keep track of the gadgets in the world
    private List<Gadget> gadgetsInWorld;

    // Use this for initialization
    void Start () {
        gadgetsInWorld = new List<Gadget>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddGadget (GameObject newGadget)
    {
        GameObject gadgetObj = Instantiate(newGadget, this.transform);
        Gadget gadget = gadgetObj.GetComponent<Gadget>();
        gadgetsInWorld.Add(gadget);
    }
}
