using UnityEngine;

public class GadgetManipulator : MonoBehaviour
{
    public World World;

    // TODO might want to set it to private later
    public float translationDelta = 0.08f;

    private enum Mode { Modify, Create };
    private Gadget selectedGadget;
    private Mode currentMode;

    private int mRayCastMask;

    void Start ()
    {
        selectedGadget = null;
        currentMode = Mode.Modify;

        mRayCastMask = ~(1 << LayerMask.NameToLayer("SelectedGadget"));
    }

    void Update ()
    {
        // Receive inputs only when a gadget is selected
        if (GadgetSelected())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //https://docs.unity3d.com/Manual/Layers.html
            //We want to ignore the selected gadget otherwise the raycast will keep intersecting repeatedly with itself translating the object in undesirable ways
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mRayCastMask))
            {
                Debug.Log("Updating Position of SelectedGadget");
                selectedGadget.transform.position = hit.point;

                //TODO Come up with something more reasonable for ramp and other objects that are rotated
                selectedGadget.transform.Translate(Vector3.up * selectedGadget.transform.localScale.y / 2.0f);
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
    public bool ModifyModeEnabled ()
    {
        return currentMode == Mode.Modify;
    }

    public bool CreateModeEnabled ()
    {
        return currentMode == Mode.Create;
    }

    public void EnableModifyMode (Gadget gadget)
    {
        Debug.Log("Enter manipulator modify mode.");

        selectedGadget = gadget;
        currentMode = Mode.Modify;
        selectedGadget.Highlight();
    }

    public void EnableCreateMode (Gadget gadget)
    {
        Debug.Log("Enter manipulator create mode.");

        selectedGadget = gadget;
        currentMode = Mode.Create;
        selectedGadget.Transparent();
    }

    public bool GadgetSelected ()
    {
        return selectedGadget != null;
    }

    public void Activate ()
    {
        this.gameObject.SetActive(true);
    }

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

    public void ResetGadgetsInWorld() {
        World.Reset();
    }
}
