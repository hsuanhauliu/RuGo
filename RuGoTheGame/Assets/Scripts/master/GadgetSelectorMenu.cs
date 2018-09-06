using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetSelectorMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToggleMenu() {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
