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

    void Start()
    {
        EnableBuildMode();
    }

    void Update()
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
                this.ResetGadgetsInWorld();
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
    public void ResetGadgetsInWorld()
    {
        Manipulator.ResetGadgetsInWorld();
    }

    /// <summary>
    /// name of the prefab file.
    /// </summary>
    /// <param name="prefabResourceName">The name of the resource to load from Prefab Directory</param>
    public void CreateGadget(string prefabResourceName)
    {
        //TODO currently placing gadget at gameManager's position.
        // Might want to place it in front of the player for a set distance.
        GameObject gadgetPrefab = Resources.Load(prefabResourceName) as GameObject;
        GameObject gadgetGameObject = Instantiate(gadgetPrefab, this.transform);

        Gadget gadget = gadgetGameObject.GetComponent<Gadget>();
        Manipulator.EnableCreateMode(gadget);
    }

    public void EnableBuildMode()
    {
        GadgetSelectorMenu.Deactivate();
        Manipulator.Activate();
        this.currentGameMode = GameMode.Build;
        GameModeDisplay.text = "Mode: Build";
    }

    public void EnableSelectMode()
    {
        Manipulator.Deactivate();
        GadgetSelectorMenu.Activate();
        this.currentGameMode = GameMode.Select;
        GameModeDisplay.text = "Mode: Select";
    }

    /************************** Private Functions **************************/

    private void SelectExistingGadget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Gadget gadget = hit.transform.GetComponent<Gadget>();

            if (gadget)
            {
                Manipulator.EnableModifyMode(gadget);
            }
        }
    }
}
