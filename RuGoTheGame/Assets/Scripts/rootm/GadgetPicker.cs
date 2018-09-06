using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetPicker : MonoBehaviour
{

    public Player player;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
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
                    player.currentGadget = gadget;
                }
            }
        }
    }
}
