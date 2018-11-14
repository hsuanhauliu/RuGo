using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public enum GameMode { BUILD, SELECTION, DRAW, DELETE, COMPLETE};

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public VRTK.VRTK_InteractNearTouch RightInteractNearTouch;
    [HideInInspector]
    public VRTK.VRTK_InteractGrab RightInteractGrab;
    [HideInInspector]
    public VRTK.VRTK_InteractTouch RightInteractTouch;
    [HideInInspector]
    public HandAnimator RightAnimator;
    [HideInInspector]
    public HandAnimator LeftAnimator;

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

    public BoxCollider LeftPlatformCollider;

    public GameMode CurrentGameMode;

    void Awake()
    {
        MakeSingleton();

        CurrentGameMode = GameMode.BUILD;

        /* Setup Controller Events */
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.TriggerPress, true, RightControllerEvents_TriggerDown);
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.TriggerPress, false, RightControllerEvents_TriggerUp);
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.GripPress, true, RightControllerEvents_GripDown);
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.GripPress, false, RightControllerEvents_GripUp);
        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.TouchpadPress, false, RightControllerEvents_TouchpadUp);

        LeftControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.TriggerPress, true, LeftControllerEvents_TriggerDown);
        LeftControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.TriggerPress, false, LeftControllerEvents_TriggerUp);

        /* Setup Controller VRTK scripts */
        VRTK.VRTK_InteractGrab rightInteractGrab = RightControllerEvents.GetComponent<VRTK.VRTK_InteractGrab>();
        rightInteractGrab.grabButton = VRTK.VRTK_ControllerEvents.ButtonAlias.TriggerPress;

        RightInteractNearTouch = RightControllerEvents.GetComponent<VRTK.VRTK_InteractNearTouch>();
        RightInteractTouch = RightControllerEvents.GetComponent<VRTK.VRTK_InteractTouch>();
        RightInteractGrab = RightControllerEvents.GetComponent<VRTK.VRTK_InteractGrab>();

        LeftPlatformCollider.isTrigger = true;
        
        RightAnimator = RightControllerEvents.gameObject.GetComponentInChildren<HandAnimator>();
        LeftAnimator = LeftControllerEvents.gameObject.GetComponentInChildren<HandAnimator>();

        RightAnimator.SetHandGhost(true);
        LeftAnimator.SetHandGhost(true);
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
            case GameMode.SELECTION:
                {
                    World.Instance.ShowShelf(false);
                }
                break;
            case GameMode.DRAW:
                {
                    PathTool.Deactivate();
                    RightAnimator.MakeHandIdle();
                    RightAnimator.SetHandGhost(true);
                }
                break;
            case GameMode.DELETE:
                break;
            case GameMode.COMPLETE:
                World.Instance.LoadCurrentSaveSlot();
                break;

        }

        CurrentGameMode = newGameMode;

        // Enter new Game mode
        switch (CurrentGameMode)
        {
            case GameMode.SELECTION:
                {
                    World.Instance.ShowShelf(true);
                }
                break;
            case GameMode.DRAW:
                {
                    PathTool.Activate(CreateGadgetAlongPath);
                    RightAnimator.MakeHandLaser();
                    RightAnimator.SetHandGhost(false);
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
    void LeftControllerEvents_TriggerDown(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        LeftPlatformCollider.isTrigger = false;

        LeftAnimator.SetHandGhost(false);
    }

    void LeftControllerEvents_TriggerUp(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        LeftPlatformCollider.isTrigger = true;

        LeftAnimator.SetHandGhost(true);
    }

    void RightControllerEvents_TriggerDown(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        
        if (RightInteractNearTouch.GetNearTouchedObjects().Count > 0 || // If we near interactable objects
            RightInteractGrab.GetGrabbedObject() != null || // .. or if we already have an object that we have grabbed
            RightInteractTouch.GetTouchedObject() != null) // .. or if we are touching an interactable object
        {
            // .. then don't Draw
            return;
        }

        ChangeGameMode(GameMode.DRAW);
    }

    void RightControllerEvents_TriggerUp(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        if (CurrentGameMode == GameMode.DRAW)
        {
            ChangeGameMode(GameMode.BUILD);
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
            ChangeGameMode(GameMode.BUILD);
        }
    }

    void RightControllerEvents_TouchpadUp(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        ToggleBuildMode();
    }

    /**************************************** GAME MODE ACTION **********************************/
    void ToggleBuildMode()
    {
        if (CurrentGameMode == GameMode.SELECTION)
        {
            ChangeGameMode(GameMode.BUILD);
        }
        else
        {
            ChangeGameMode(GameMode.SELECTION);
        }
    }


    /***************************************** HELPERS ******************************************/
    private void CreateGadgetAlongPath(Vector3[] path)
    {
        string gadgetName = "Domino";
        int min_Dominos = 5;

        GameObject gadgetPreb = Resources.Load(gadgetName) as GameObject;
        int numOfPoints = path.Length;

        if (numOfPoints > min_Dominos)
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
    }
}
