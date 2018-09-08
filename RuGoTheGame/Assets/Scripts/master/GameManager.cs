using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // References to different classes
    public GadgetManipulator manipulator;           // manipulator for gadgets
    public GadgetSelectorMenu gadgetSelectorMenu;   // selector menu

    // TODO: needs to change it later. manually referencing prefabs for now
    public GameObject box;

    // Local variables
    private string[] gameModes;
    private int currentGameMode;

    // Initializer; executed when the game starts
    void Start()
    {
        // TODO: add more game modes in the future
        gameModes = new string[] {
            "manipulate",
            "selectorMenu"
        };

        // Set up game mode and object active states
        currentGameMode = 0;                            // set game mode
        manipulator.gameObject.SetActive(true);         // make sure manipulator is active
        gadgetSelectorMenu.gameObject.SetActive(false); // make sure it is turned off

    }

    // Update is called once per frame
    void Update()
    {
        // If we're in manipulate mode...
        if (currentGameMode == 0)
        {
            // Listen for left click
            if (Input.GetMouseButtonDown(0) && manipulator.GetMode() == 0)
            {
                SelectExistingGadget();
            }

            // Listen for R for reset button
            if (Input.GetKeyDown(KeyCode.R))
            {
                this.Reset();
            }
        }

        // Listen for M for menu toggle
        if (Input.GetKeyDown(KeyCode.M))
        {
            // If menu is active, turn it off and change mode to manipulate
            if (gadgetSelectorMenu.isActiveAndEnabled)
            {
                SwitchMode(0);
            }
            // If menu is inactive, turn it on and change mode to selectorMenu
            else
            {
                SwitchMode(1);
            }
        }
    }


    /************************** Public Functions **************************/

    //TODO 1. might need to reset future new classes
    //     2. should be invoked by UI button instead of key press?
    // Function: Reset
    // Input: none
    // Output: none
    // Description:
    // - Reset the world to its initial state.
    public void Reset()
    {
        //gadgetsInWorld.ForEach((Gadget g) => g.Reset());
    }

    // Function: CreateGadget
    // Input: name of the prefab file
    // Output: none
    // Description:
    // - Create a gameObject using prefab. Invoked by UI buttons.
    public void CreateGadget(string name)
    {
        //TODO Delegate creation of gadgets to GadgetFactory
        //GadgetFactory factory = GadgetFactory.GetInstance();
        //Gadget gadget = factory.CreateGadget(gadgetName);

        //TODO currently placing gadget at gameManager's position.
        // Might want to place it in front of the player for a set distance.
        GameObject gadgetObj = Instantiate(box, this.transform);
        Gadget gadget = gadgetObj.GetComponent<Gadget>();
        manipulator.Set(gadget, 1);
    }

    // Function: SwitchMode
    // Input:
    // - newMode: code of the new mode we want to switch to
    // Output: none
    // Description:
    // - Switch to a different mode. Rvery time there is a mode change, make
    //   sure to reset/clean up the previous mode, set it to inactive, and change
    //   gameMode code.
    public void SwitchMode(int newMode)
    {
        Debug.Log("*** Game mode switched from " + gameModes[currentGameMode] +
                  " to " + gameModes[newMode] + " ***");

        // Turn off modules associated with the current mode
        switch (currentGameMode)
        {
            case 0:
                manipulator.Reset();
                manipulator.gameObject.SetActive(false);
                break;
            case 1:
                gadgetSelectorMenu.gameObject.SetActive(false);
                break;
            default:
                break;
        }

        // Turn on module associated with the new mode
        currentGameMode = newMode;
        switch (newMode)
        {
            case 0:
                manipulator.gameObject.SetActive(true);
                break;
            case 1:
                gadgetSelectorMenu.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }


    /************************** Private Functions **************************/

    // Function: SelectExistingGadget
    // Input: none
    // Output: none
    // Description:
    // - Find and select the gadget being clicked on.
    private void SelectExistingGadget()
    {
        // Convert Mouse Screen Coordinates to Ray in 3D Space
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // if we do click on something...
        if (Physics.Raycast(ray, out hit))
        {
            // Create a reference to the gadget that we clicked on
            Gadget gadget = hit.transform.GetComponent<Gadget>();

            // Check whether the object that we clicked on was a gadget
            if (gadget)
            {
                Debug.Log("A gadget is being selected.");
                manipulator.Set(gadget, 2);
            }
        }
    }
}
