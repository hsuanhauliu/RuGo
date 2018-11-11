using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public enum GameMode { NONE, BUILD, DRAW, DELETE, COMPLETE};

public class GameManager : MonoBehaviour
{
    private VRTK.VRTK_InteractNearTouch mRightInteractNearTouch;

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

    public bool IsGameOver {
        get {
            return CurrentGameMode == GameMode.COMPLETE;
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
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.TriggerPress, true, RightControllerEvents_TriggerDown);
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.TriggerPress, false, RightControllerEvents_TriggerUp);
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.GripPress, true, RightControllerEvents_GripDown);
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.GripPress, false, RightControllerEvents_GripUp);
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.TouchpadPress, false, RightControllerEvents_TouchpadUp);

        /* Setup Controller VRTK scripts */
        VRTK.VRTK_InteractGrab rightInteractGrab = RightControllerEvents.GetComponent<VRTK.VRTK_InteractGrab>();
        rightInteractGrab.grabButton = VRTK.VRTK_ControllerEvents.ButtonAlias.TriggerPress;

        mRightInteractNearTouch = RightControllerEvents.GetComponent<VRTK.VRTK_InteractNearTouch>();
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
            case GameMode.COMPLETE:
                World.Instance.LoadAuto();
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
            case GameMode.COMPLETE:
                IEnumerator coroutine = ResetGame();
                StartCoroutine(coroutine);
                break;
        }
    }

    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(5.0f);
        ChangeGameMode(GameMode.BUILD);
    }

    /************************** Input Events ********************************/
    void RightControllerEvents_TriggerDown(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        if (mRightInteractNearTouch.GetNearTouchedObjects().Count > 0)
        {
            Debug.Log("Looked to grab");
            return;
        }   

        ChangeGameMode(GameMode.DRAW);
    }

    void RightControllerEvents_TriggerUp(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        if (CurrentGameMode == GameMode.DRAW)
        {
            ChangeGameMode(GameMode.NONE);
        }
    }

    void RightControllerEvents_GripDown(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        // This is only possible if the user is not in draw mode already
        if (CurrentGameMode != GameMode.DRAW)
        {
            ChangeGameMode(GameMode.DELETE);
        }
    }

    void RightControllerEvents_GripUp(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        if (CurrentGameMode == GameMode.DELETE)
        {
            ChangeGameMode(GameMode.NONE);
        }
    }

    void RightControllerEvents_TouchpadUp(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        ToggleBuildMode();
    }

    /**************************************** GAME MODE ACTION **********************************/
    void ToggleBuildMode()
    {
        if (CurrentGameMode == GameMode.BUILD)
        {
            ChangeGameMode(GameMode.NONE);
        }
        else
        {
            ChangeGameMode(GameMode.BUILD);
        }
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
