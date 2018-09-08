using UnityEngine;

public class GadgetManipulator : MonoBehaviour
{
    public World World;

    // TODO might want to set it to private later
    public float translationDelta = 0.01f;

    private enum Mode { Modify, Create };
    private Gadget selectedGadget;
    private Mode currentMode;


    void Start ()
    {
        selectedGadget = null;
        currentMode = Mode.Modify;
    }

    void Update ()
    {
        // Receive inputs only when a gadget is selected
        if (GadgetSelected())
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
                if (ModifyModeEnabled())
                {
                    Debug.Log("Object is being set.");

                    selectedGadget.Deselect();
                    selectedGadget = null;
                }
                else
                {
                    Debug.Log("An object is created!");

                    World.CreateGadgetFromTemplate(selectedGadget);
                }
            }
        }
    }


    /************************** Public Functions **************************/

    // Function: ModifyModeEnabled
    // Input: none
    // Output:
    //  - A boolean value.
    // Description:
    //  - Check if the manipulator is currently in modify mode.
    public bool ModifyModeEnabled ()
    {
        return currentMode == Mode.Modify ? true : false;
    }

    // Function: CreateModeEnabled
    // Input: none
    // Output:
    //  - A boolean value.
    // Description:
    //  - Check if the manipulator is currently in create mode.
    public bool CreateModeEnabled ()
    {
        return currentMode == Mode.Create ? true : false;
    }

    // Function: EnableModifyMode
    // Input:
    // - gadget: reference to a gadget component.
    // Output: none
    // Description:
    // - Enter modify mode while a gadget is being selected. This function
    //   should be invoked by GameManager.
    public void EnableModifyMode (Gadget gadget)
    {
        Debug.Log("Enter manipulator modify mode.");

        selectedGadget = gadget;
        currentMode = Mode.Modify;
        selectedGadget.Highlight();
    }

    // Function: EnableCreateMode
    // Input:
    //  - gadget: reference to a gadget component.
    // Output: none
    // Description:
    //  - Enter create mode while a gadget is being selected. This function
    //    should be invoked by GameManager.
    public void EnableCreateMode (Gadget gadget)
    {
        Debug.Log("Enter manipulator create mode.");

        selectedGadget = gadget;
        currentMode = Mode.Create;
        selectedGadget.Transparent();
    }

    // Function: GadgetSelected
    // Input: none
    // Output:
    //  - A boolean value.
    // Description:
    //  - Check if a gadget is being selected.
    public bool GadgetSelected ()
    {
        return selectedGadget != null;
    }

    // Function: Activate
    // Input: none
    // Output: none
    // Description:
    //  - Activate manipulator gameObject in the scene.
    public void Activate ()
    {
        this.gameObject.SetActive(true);
    }

    // Function: Deactivate
    // Input: none
    // Output: none
    // Description:
    //  - Deactivate manipulator gameObject in the scene.
    public void Deactivate ()
    {
        this.Reset();
        this.gameObject.SetActive(false);
    }

    // Function: Reset
    // Input: none
    // Output: none
    // Description:
    //  - Reset manipulator to its initial state. Depending on which mode the
    //    game is currently in, the function will perform different actions.
    //
    //    1. Modify mode:
    //       - Simply deselect the current selected gadget.
    //       - Set selectedGadget to null.
    //    2. Create mode:
    //       - Destroy the gadget template.
    //       - Set mode back to modify mode.
    //       - Set selectedGadget to null.
    public void Reset ()
    {
        Debug.Log("GadgetManipulator is being reset.");

        if (GadgetSelected())
        {
            if (ModifyModeEnabled())
            {
                selectedGadget.Deselect();
            }
            else
            {
                Destroy(selectedGadget.gameObject);
                currentMode = Mode.Modify;
            }
            selectedGadget = null;
        }
    }
}
