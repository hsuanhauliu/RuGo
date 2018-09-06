using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public GameObject currentlySelectedObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (currentlySelectedObject)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                currentlySelectedObject.transform.Translate(Vector3.right * 0.02f);
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                currentlySelectedObject.transform.Translate(Vector3.left * 0.02f);
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                currentlySelectedObject.transform.Translate(Vector3.forward * 0.02f);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                currentlySelectedObject.transform.Translate(Vector3.back * 0.02f);
            }
        }
    }
}
