using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
        LeftControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.GripPress, false, LeftControllerEvents_GripUp);

        /* Setup Controller VRTK scripts */
        VRTK.VRTK_InteractGrab rightInteractGrab = RightControllerEvents.GetComponent<VRTK.VRTK_InteractGrab>();
        rightInteractGrab.grabButton = VRTK.VRTK_ControllerEvents.ButtonAlias.TriggerPress;

        RightInteractNearTouch = RightControllerEvents.GetComponent<VRTK.VRTK_InteractNearTouch>();
        RightInteractTouch = RightControllerEvents.GetComponent<VRTK.VRTK_InteractTouch>();
        RightInteractGrab = RightControllerEvents.GetComponent<VRTK.VRTK_InteractGrab>();

        RightAnimator = RightControllerEvents.gameObject.GetComponentInChildren<HandAnimator>();
        LeftAnimator = LeftControllerEvents.gameObject.GetComponentInChildren<HandAnimator>();

#if RUGO_AR
        VRTK.VRTK_SDKManager vrtkManager = VRTK.VRTK_SDKManager.instance;
        vrtkManager.LoadedSetupChanged += (sender, e) => {
            VRTK.VRTK_SDKSetup loadedSetup = vrtkManager.loadedSetup;
            if (loadedSetup){
                SteamVR_PlayArea playArea = loadedSetup.actualBoundaries.GetComponent<SteamVR_PlayArea>();
                playArea.drawInGame = false;

                MeshRenderer playAreaRenderer = loadedSetup.actualBoundaries.GetComponent<MeshRenderer>();
                playAreaRenderer.enabled = false;
            }
        };
#endif
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
                    RightAnimator.MakeHandIdleAndGhost();
                }
                break;
            case GameMode.DELETE:
                {
                    RightAnimator.MakeHandIdleAndGhost();
                }
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
                {
                    RightAnimator.MakeHandDelete();
                }
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
        LeftAnimator.SetHandGhost(false);
    }

    void LeftControllerEvents_TriggerUp(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        LeftAnimator.SetHandGhost(true);
    }

    void LeftControllerEvents_GripUp(object sender, VRTK.ControllerInteractionEventArgs e) 
    {
        // Only clear the current save slot in delete mode
        if (CurrentGameMode == GameMode.DELETE) 
        {
           World.Instance.ClearCurrentSaveSlot();
        }
    }

    private bool ValidateNearObjects()
    {
        List<GameObject> nearObjects = RightInteractNearTouch.GetNearTouchedObjects();
        foreach(GameObject nearObject in nearObjects)
        {
            if(nearObject == null)
            {
                RightInteractNearTouch.ForceStopNearTouching(null);
                return false;
            }
            else
            {
                // We couldn't possiblu be near anything else. So that means if gadget isn't active in heirarchy then our shelf is inactive.
                if (!nearObject.activeInHierarchy)
                {
                    RightInteractNearTouch.ForceStopNearTouching(null);
                    return false;
                }
            }
        }
        
        return true;
    }

    void RightControllerEvents_TriggerDown(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        
        if (RightInteractGrab.GetGrabbedObject() != null || // If we already have an object that we have grabbed
            RightInteractTouch.GetTouchedObject() != null) // .. or if we are touching an interactable object
        {
            return;
        }

        if(RightInteractNearTouch.GetNearTouchedObjects().Count > 0) // If we near interactable objects
        {
            if(ValidateNearObjects())
            {
                return;
            }
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
    private void CreateGadgetAlongPath(Vector3[] path, float minThreshold)
    {
        string gadgetName = "Domino";
        GameObject gadgetPreb = Resources.Load(gadgetName) as GameObject;
        float offset = gadgetPreb.transform.localScale.y * 0.5f;
        int numOfPoints = path.Length;
        int dominoPathColorIndex = DominoGadget.GetRandomColorIndex();

        if (numOfPoints > minThreshold)
        {
            for (int p = 0; p < numOfPoints - 1; p++)
            {
                GameObject gadgetGameObject = Instantiate(gadgetPreb, this.transform);
                DominoGadget gadget = gadgetGameObject.GetComponent<DominoGadget>();

                Vector3 direction = path[p + 1] - path[p];
                gadget.transform.position = new Vector3(path[p].x, path[p].y + offset, path[p].z);
                gadget.transform.rotation = Quaternion.LookRotation(direction);
                gadget.SetDominoInWorld(dominoPathColorIndex);
                World.Instance.InsertGadget(gadget);
            }

            GameObject lastGadgetGameObject = Instantiate(gadgetPreb, this.transform);
            DominoGadget lastGadget = lastGadgetGameObject.GetComponent<DominoGadget>();

            Vector3 lastPathDirection = path[numOfPoints - 1] - path[path.Length - 2];
            lastGadget.transform.position = new Vector3(path[numOfPoints - 1].x, path[numOfPoints - 1].y + offset, path[numOfPoints - 1].z);
            lastGadget.transform.rotation = Quaternion.LookRotation(lastPathDirection);
            lastGadget.SetDominoInWorld(dominoPathColorIndex);
            World.Instance.InsertGadget(lastGadget);
        }
    }
}
