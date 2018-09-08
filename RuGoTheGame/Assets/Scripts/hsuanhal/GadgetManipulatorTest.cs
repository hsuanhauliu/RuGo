using System.Collections.Generic;
using UnityEngine;

public class GadgetManipulatorTest : MonoBehaviour
{
    // Local Variables
    private GadgetTest selectedGadget;  // keep track of which gadget is currently being selected
    private int currentMode;        // 0 for default, 1 for selecting stamp mode,
                                    // 2 for modify mode

    public World world;            // a reference to the world object

    // Control for the moving speed
    public float translationDelta = 0.01f;  // TODO might want to set it to private later

	// Use this for initialization
	void Start ()
    {
        selectedGadget = null;
        currentMode = 0;
	}

    // Listen for input when a gadget is selected
    void Update ()
    {
        // Receive input only when a gadget is selected
        if (selectedGadget)
        {
            // Control movement of the selected gadget
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

            // Place gadget
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (currentMode == 1)
                {
                    // make a copy
                    world.AddGadget(selectedGadget.gameObject);
                    Debug.Log("A object is placed!");
                }
                else
                {
                    selectedGadget.Deselect();
                    selectedGadget = null;
                    currentMode = 0;
                }
            }
        }
    }


    /************************** Public Functions **************************/

    // Function: Set
    // Input:
    // - newGadget: reference to the selected gadget
    // - mode: which mode we are entering
    // Output: none
    // Description:
    // - A function that is used to set manipulator parameters when we switch
    //   to manipulate mode. Currently this should be invoked by GameManager only.
    public void Set (GadgetTest gadget, int mode)
    {
        if (IsValidSetMode(mode))
        {
            selectedGadget = gadget;
            currentMode = mode;

            if (currentMode == 1)
            {
                // TODO material can't change for some reason...
                selectedGadget.Transparent();
            }
            // hightlight the gadget if the game is in modify mode
            else
            {
                selectedGadget.Highlight();
            }
        }
        else
        {
            Debug.Log("Invalid input for mode.");
        }
    }

    // Function: Reset
    // Input: none
    // Output: none
    // Description:
    // - Reset manipulator to its initial state.
    public void Reset ()
    {
        Debug.Log("GadgetManipulator got reset.");

        if (selectedGadget)
        {
            // stamp mode
            if (currentMode == 1)
            {
                Destroy(selectedGadget.gameObject);     // destroy gadget stamp
            }
            // modify mode
            else if (currentMode == 2)
            {
                selectedGadget.Deselect();
            }

            selectedGadget = null;
            currentMode = 0;
        }
    }

    // Function: GetMode
    // Input: none
    // Output:
    // - selectMode: the current mode of manipulator
    // Description:
    // - Return the mode number for manipulator class.
    public int GetMode ()
    {
        return currentMode;
    }


    /************************** Private Functions **************************/

    // Function: IsValidMode
    // Input:
    // - mode: code of the mode to be checked
    // Output: none
    // Description:
    // - Check if this is a valid mode to enter.
    private bool IsValidSetMode (int mode)
    {
        return mode == 1 || mode == 2 ? true : false;
    }
}
