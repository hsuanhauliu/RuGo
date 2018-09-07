using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetManipulator : MonoBehaviour {

    public Gadget selectedGadget;

    public float translationDelta = 0.01f;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (selectedGadget)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedGadget.transform.Translate(Vector3.right * translationDelta);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedGadget.transform.Translate(Vector3.left * translationDelta);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedGadget.transform.Translate(Vector3.forward * translationDelta);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedGadget.transform.Translate(Vector3.back * translationDelta);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                selectedGadget.Deselect();
                selectedGadget = null;
            }
        }
    }
}
