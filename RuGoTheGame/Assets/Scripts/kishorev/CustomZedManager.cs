using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomZedManager : MonoBehaviour {

    private void Awake()
    {
        Debug.Log(this.name + " is Awake");
    }

    private void OnEnable()
    {
        Debug.Log(this.name + " is Enabled");
    }

    // Use this for initialization
    void Start () {
        Debug.Log(this.name + " is Started");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
