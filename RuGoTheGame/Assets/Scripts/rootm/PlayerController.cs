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
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (currentlySelectedObject) {
                currentlySelectedObject.transform.position += transform.forward * 0.2f;
            }
        }
    }
}
