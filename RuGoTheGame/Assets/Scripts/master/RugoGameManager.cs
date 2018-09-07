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
        //TODO Delegate creation of gadgets to GadgetFactory
        //GadgetFactory factory = GadgetFactory.GetInstance();
        //Gadget gadget = factory.CreateGadget(gadgetName);
        GameObject d = Instantiate(box, this.transform);
        Gadget gadget = d.GetComponent<Gadget>();
        gadget.Highlight();
        player.currentGadget = gadget;
    }
}
