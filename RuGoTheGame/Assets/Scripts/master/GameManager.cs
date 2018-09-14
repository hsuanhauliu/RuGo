using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GadgetManipulator Manipulator;
    public GadgetSelectorMenu GadgetSelectorMenu;
    public Text GameModeDisplay;

    private enum GameMode { Build, Select, Sim };
    private GameMode currentGameMode;
    private bool BuildModeEnabled
    {
        get
        {
            return currentGameMode == GameMode.Build;
        }
    }

    void Start ()
    {
        EnableBuildMode();
    }

    void Update ()
    {
        if (BuildModeEnabled)
        {
            if (Input.GetMouseButtonDown(0) &&
                Manipulator.ModifyModeEnabled() &&
                !Manipulator.GadgetSelected()
               )
            {
                SelectExistingGadget();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                this.Reset();
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (GadgetSelectorMenu.isActiveAndEnabled)
            {
                EnableBuildMode();
            }
            else
            {
                EnableSelectMode();
            }
        }
    }


    /************************** Public Functions **************************/

    //TODO 1. might need to reset future new classes
    //     2. should be invoked by UI button instead of key press?
    //     3. need to make sure no gadget is being selected.
    // Function: Reset
    // Input: none
    // Output: none
    // Description:
    //  - Reset the world to its initial state.
    public void Reset ()
    {
        Manipulator.ResetGadgetsInWorld();
    }

    /// <summary>
    /// name of the prefab file.
    /// </summary>
    /// <param name="prefabName">The name of the resource to load from Prefab Directory</param>
    public void CreateGadget (string prefabName)
    {
        GameObject gadgetPrefab = Resources.Load(prefabName) as GameObject;
       
        GameObject gadgetGameObject = Instantiate(gadgetPrefab, this.transform);
        //TODO currently placing gadget at gameManager's position.
        // Might want to place it in front of the player for a set distance.
        Gadget gadget = gadgetGameObject.GetComponent<Gadget>();
        Manipulator.EnableCreateMode(gadget);
    }

    public void EnableBuildMode () {
        GadgetSelectorMenu.Deactivate();
        Manipulator.Activate();
        this.currentGameMode = GameMode.Build;
        GameModeDisplay.text = "Mode: Build";
    }

    public void EnableSelectMode () {
        Manipulator.Deactivate();
        GadgetSelectorMenu.Activate();
        this.currentGameMode = GameMode.Select;
        GameModeDisplay.text = "Mode: Select";
    }

        /************************** Private Functions **************************/

    private void SelectExistingGadget ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Gadget gadget = hit.transform.GetComponent<Gadget>();

            if (gadget)
            {
                Debug.Log("A gadget is being selected.");

                Manipulator.EnableModifyMode(gadget);
            }
        }
    }
}
