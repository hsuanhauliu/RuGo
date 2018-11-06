using UnityEngine;
using UnityEngine.UI;


public enum GameMode { NONE, BUILD, DRAW, DELETE };

public class GameManager : MonoBehaviour
{
    private GameObject mMainCamera;
    public GameObject MainCamera
    {
        get
        {
            if(mMainCamera == null)
            {
                mMainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            return mMainCamera;
        }
    }

    public PathTool PathTool;
    public VRTK.VRTK_ControllerEvents RightControllerEvents;
    public VRTK.VRTK_ControllerEvents LeftControllerEvents;

    public GameMode CurrentGameMode;

    void Awake()
    {
        MakeSingleton();

        CurrentGameMode = GameMode.NONE;

        /* Setup Controller Events */
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.TriggerPress, true, RightControllerEvents_TriggerClicked);
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.TouchpadPress, true, RightControllerEvents_TouchpadDown);
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.TouchpadPress, false, RightControllerEvents_TouchpadUp);
    }

    public static GameManager Instance = null;
    private void MakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    /************************** State Management **************************/
    public void ChangeGameMode(GameMode newGameMode)
    {
        // Exit current Game mode
        switch(CurrentGameMode)
        {
            case GameMode.BUILD:
                {
                    World.Instance.ShowShelf(false);
                }
                break;
            case GameMode.DRAW:
                {
                    PathTool.Deactivate();
                }
                break;
            case GameMode.DELETE:
                break;
        }

        CurrentGameMode = newGameMode;

        // Enter new Game mode
        switch (CurrentGameMode)
        {
            case GameMode.BUILD:
                {
                    World.Instance.ShowShelf(true);
                }
                break;
            case GameMode.DRAW:
                {
                    PathTool.Activate(CreateGadgetAlongPath);
                }
                break;
            case GameMode.DELETE:
                break;
        }
    }

    /************************** Input Events ********************************/
    void RightControllerEvents_TriggerClicked(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        if(CurrentGameMode == GameMode.BUILD)
        {
            ChangeGameMode(GameMode.NONE);
        }
        else
        {
            ChangeGameMode(GameMode.BUILD);
        }
    }

    void RightControllerEvents_TouchpadDown(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        if (e.touchpadAxis.y > 0.0f)
        {
            ChangeGameMode(GameMode.DRAW);
        }
        else
        {
            ChangeGameMode(GameMode.DELETE);
        }
    }

    void RightControllerEvents_TouchpadUp(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        ChangeGameMode(GameMode.NONE);
    }


    /***************************************** HELPERS ******************************************/
    private void CreateGadgetAlongPath(Vector3[] path)
    {
        string gadgetName = "Domino";
        GameObject gadgetPreb = Resources.Load(gadgetName) as GameObject;
        int numOfPoints = path.Length;

        if (numOfPoints > 1)
        {
            for (int p = 0; p < numOfPoints - 1; p++)
            {
                GameObject gadgetGameObject = Instantiate(gadgetPreb, this.transform);
                DominoGadget gadget = gadgetGameObject.GetComponent<DominoGadget>();

                Vector3 direction = path[p + 1] - path[p];
                gadget.transform.position = path[p];
                gadget.transform.rotation = Quaternion.LookRotation(direction);
                gadget.Deselect();
                gadget.SetDominoInWorld();
                World.Instance.InsertGadget(gadget);
            }

            GameObject lastGadgetGameObject = Instantiate(gadgetPreb, this.transform);
            DominoGadget lastGadget = lastGadgetGameObject.GetComponent<DominoGadget>();

            Vector3 lastPathDirection = path[numOfPoints - 1] - path[path.Length - 2];
            lastGadget.transform.position = path[numOfPoints - 1];
            lastGadget.transform.rotation = Quaternion.LookRotation(lastPathDirection);
            lastGadget.Deselect();
            lastGadget.SetDominoInWorld();
            World.Instance.InsertGadget(lastGadget);
        }
        else if (numOfPoints == 1)
        {
            GameObject gadgetGameObject = Instantiate(gadgetPreb, this.transform);
            DominoGadget gadget = gadgetGameObject.GetComponent<DominoGadget>();

            gadget.transform.position = path[0];
            gadget.Deselect();
            gadget.SetDominoInWorld();
            World.Instance.InsertGadget(gadget);
        }
    }
}
