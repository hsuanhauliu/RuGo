using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Gadget currentGadget;

    public GadgetSelectorMenu gadgetMenu;

    public float translationDelta = 0.01f;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (currentGadget)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentGadget.transform.Translate(Vector3.right * translationDelta);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentGadget.transform.Translate(Vector3.left * translationDelta);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentGadget.transform.Translate(Vector3.forward * translationDelta);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentGadget.transform.Translate(Vector3.back * translationDelta);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                currentGadget.Deselect();
                currentGadget = null;
            }

        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("TOGGLE MENU");
            gadgetMenu.ToggleMenu();
        }
    }
}
