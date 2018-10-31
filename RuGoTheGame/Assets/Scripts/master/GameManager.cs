using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public GadgetManipulator Manipulator;
    public PathTool PathTool;
    public Text GameModeDisplay;

    public GadgetSelectorMenu GadgetSelectorMenu;
    public MainMenu MainMenu;
    public LoadMenu loadMenu;

    // #TEMP HACK: How to do this for VRTK or does this go away?
    public GameObject MainCamera;

    // Testing the VR Menu
    public int CurrentMenuOption = -1;
    public bool isVrRun;

    private enum GameMode { Build, Select, Draw };
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
            if (RuGoInteraction.Instance.IsMenuConfirmPressed &&
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
            if (Input.GetKeyDown(KeyCode.O))
            {
                Manipulator.Save();
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                Manipulator.CreateNewWorld();
            }
        }

        if (RuGoInteraction.Instance.IsMenuActionPressed)
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

    public void EnableDrawMode()
    {
        GadgetSelectorMenu.Deactivate();
        PathTool.Activate(CreateGadgetAlongPath);
        this.currentGameMode = GameMode.Draw;
        GameModeDisplay.text = "Mode: Draw Path";

        // Player Enable Look PC_ONLY
        SetPlayerLook(true);
    }

    public void EnableBuildMode()
    {
        GadgetSelectorMenu.Deactivate();
        Manipulator.Activate();
        this.currentGameMode = GameMode.Build;
        GameModeDisplay.text = "Mode: Build";

        // Player Enable Look PC_ONLY
        SetPlayerLook(true);
    }

    public void EnableSelectMode()
    {
        Manipulator.Deactivate();
        PathTool.Deactivate();
        GadgetSelectorMenu.Activate();
        this.currentGameMode = GameMode.Select;
        GameModeDisplay.text = "Mode: Select";

        // Player Enable Look PC_ONLY
        SetPlayerLook(false);
    }

    public void ResetGadgetsInWorld()
    {
        Manipulator.ResetGadgetsInWorld();
    }

    public void GameManagerClearWorld()
    {
        Manipulator.ClearWorld();
    }

    public void GameManagerSaves()
    {
        Manipulator.Save();
    }

    public void CreateGadget(string prefabResourceName)
    {
        this.EnableBuildMode();
       
        GameObject gadgetPrefab = Resources.Load(prefabResourceName) as GameObject;
        GameObject gadgetGameObject = Instantiate(gadgetPrefab, this.transform);
        Gadget gadget = gadgetGameObject.GetComponent<Gadget>();

        Manipulator.EnableCreateMode(gadget);
    }

    // TODO make it extendable to use for gadgets other than just the Domino
    public void CreateGadgetAlongPath(Vector3[] path)
    {
        string gadgetName = "Domino";
        GameObject gadgetPreb = Resources.Load(gadgetName) as GameObject;
        int numOfPoints = path.Length;

        if (numOfPoints > 1)
        {
            for (int p = 0; p < numOfPoints - 1; p++)
            {
                GameObject gadgetGameObject = Instantiate(gadgetPreb, this.transform);
                Gadget gadget = gadgetGameObject.GetComponent<Gadget>();

                Vector3 direction = path[p + 1] - path[p];
                gadget.transform.position = path[p];
                gadget.transform.rotation = Quaternion.LookRotation(direction);
                gadget.Deselect();
                Manipulator.InsertGadgetIntoWorld(gadget);
            }
            GameObject lastGadgetGameObject = Instantiate(gadgetPreb, this.transform);
            Gadget lastGadget = lastGadgetGameObject.GetComponent<Gadget>();

            Vector3 lastPathDirection = path[numOfPoints - 1] - path[path.Length - 2];
            lastGadget.transform.position = path[numOfPoints - 1];
            lastGadget.transform.rotation = Quaternion.LookRotation(lastPathDirection);
            lastGadget.Deselect();
            Manipulator.InsertGadgetIntoWorld(lastGadget);
        }
        else if (numOfPoints == 1)
        {
            GameObject gadgetGameObject = Instantiate(gadgetPreb, this.transform);
            Gadget gadget = gadgetGameObject.GetComponent<Gadget>();

            gadget.transform.position = path[0];
            gadget.Deselect();
            Manipulator.InsertGadgetIntoWorld(gadget);
        }
        EnableBuildMode();
    }

    //TODO change this to something better
    public void goFromTo()
    {
        GadgetSelectorMenu.Deactivate();
        MainMenu.Activate();
    }

    public void goToLoadMenu()
    {
        loadMenu.Activate();
    }


    /************************** Private Functions **************************/

    private void SelectExistingGadget()     {         Ray ray = RuGoInteraction.Instance.SelectorRay;         RaycastHit hit;          if (Physics.Raycast(ray, out hit))         {             Gadget gadget = hit.transform.GetComponent<Gadget>();
             if (gadget)             {                 Manipulator.EnableModifyMode(gadget);             }             else             {                 gadget = hit.transform.GetComponentInParent<Gadget>();
                 if (gadget)                 {
                    Manipulator.EnableModifyMode(gadget);
                }
            }         }     }

    private void SetPlayerLook(bool enabledFlag)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            FirstPersonMove playerMoveScript = player.GetComponent<FirstPersonMove>();
            playerMoveScript.EnableLook = enabledFlag;
        }        
    }
}
