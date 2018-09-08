using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private List<Gadget> gadgetsInWorld;

    void Start () {
        gadgetsInWorld = new List<Gadget>();
    }
	
	void Update () {
		
	}

    /************************** Public Functions **************************/

    // Function: CreateGadgetFromTemplate
    // Input:
    //  - gadgetTemplate: a reference to the gadget template.
    // Output: none
    // Description:
    //  - In manipulator create mode, we need to add the gadget to the world
    //    whenever the user wants to create a new one. The gadget template is
    //    passed in and we duplicate the template and change its color.
    public void CreateGadgetFromTemplate (Gadget gadgetTemplate)
    {
        Debug.Log("A new gameObject has been created in the World.");

        GameObject gadgetObj = Instantiate(gadgetTemplate.gameObject, this.transform);
        Gadget gadget = gadgetObj.GetComponent<Gadget>();
        gadget.Solidify();
        gadgetsInWorld.Add(gadget);
    }
}
