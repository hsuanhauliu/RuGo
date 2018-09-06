using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Gadget currentGadget;


	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (currentGadget)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                currentGadget.transform.Translate(Vector3.right * 0.02f);
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                currentGadget.transform.Translate(Vector3.left * 0.02f);
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                currentGadget.transform.Translate(Vector3.forward * 0.02f);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                currentGadget.transform.Translate(Vector3.back * 0.02f);
            }
        }
    }
}
