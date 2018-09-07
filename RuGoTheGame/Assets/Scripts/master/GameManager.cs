using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour {

    public GadgetManipulator manipulator;

    public GameObject box;

    public GadgetSelectorMenu gadgetSelectorMenu;

    private List<Gadget> gadgetsInWorld;

	void Start () {
        gadgetsInWorld = new List<Gadget>();
    }
	
    private bool PickingEnabled() {
        return !gadgetSelectorMenu.isActiveAndEnabled && !manipulator.selectedGadget;
    }

	void Update () {
        if (PickingEnabled()) {
            if (Input.GetMouseButtonDown(0))
            {
                //Convert Mouse Screen Coordinates to Ray in 3D Space
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Gadget gadget = hit.transform.GetComponent<Gadget>();
                    if (gadget)
                    {
                        gadget.Highlight();
                        manipulator.selectedGadget = gadget;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            gadgetSelectorMenu.ToggleMenu();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            this.Reset();
        }
    }

    public void Reset()
    {
        gadgetsInWorld.ForEach((Gadget g) => g.Reset());
    }

    public void CreateGadget(string name) {
        //TODO Delegate creation of gadgets to GadgetFactory
        //GadgetFactory factory = GadgetFactory.GetInstance();
        //Gadget gadget = factory.CreateGadget(gadgetName);
        GameObject d = Instantiate(box, this.transform);
        Gadget gadget = d.GetComponent<Gadget>();
        gadgetsInWorld.Add(gadget);

        gadget.Highlight();
        manipulator.selectedGadget = gadget;
    }
}
