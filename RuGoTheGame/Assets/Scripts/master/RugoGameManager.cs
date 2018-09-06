using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RugoGameManager : MonoBehaviour {

    public Player player;

    public GameObject box;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateGadget(string name) {
        GameObject d = Instantiate(box, this.transform);
        player.currentGadget = d.GetComponent<Gadget>();
    }
}
