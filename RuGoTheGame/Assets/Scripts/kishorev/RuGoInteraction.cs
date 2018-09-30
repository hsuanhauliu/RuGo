using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RuGoInteraction : MonoBehaviour {
    /*
     * TASKS
     * ** RAY       - Input space to world space ray
     * ** CONFIRM   - An action button triggered on all input
     * ** BACK      - A button mapped for Back action / Reset action
     * ** SCROLL    - 
    */

    public static RuGoInteraction Instance = null;
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

    private SteamVR_ControllerManager ControllerManager;
    private SteamVR_TrackedObject LeftTrackedObject;
    private SteamVR_TrackedObject RightTrackedObject;

    private SteamVR_Controller.Device LeftController
    {
        get
        {
            return SteamVR_Controller.Input((int)LeftTrackedObject.index);
        }
    }

    private SteamVR_Controller.Device RightController
    {
        get
        {
            return SteamVR_Controller.Input((int)RightTrackedObject.index);
        }
    }

    private void CacheControllers()
    {
        ControllerManager = GetComponent<SteamVR_ControllerManager>();

        if(ControllerManager != null)
        {
            LeftTrackedObject = ControllerManager.left.GetComponent<SteamVR_TrackedObject>();
            RightTrackedObject = ControllerManager.right.GetComponent<SteamVR_TrackedObject>();
        }
    }

    void Awake()
    {
        MakeSingleton();
    }

    void Start ()
    {
        // We do it here because we want all the VR initializations to be done first
        CacheControllers();
	}

    private void EnableDebugging()
    {
        if (!DebugCapsule.gameObject.activeSelf)
        {
            DebugCapsule.gameObject.SetActive(true);
        }

        if (!DebugCylinder.gameObject.activeSelf)
        {
            DebugCylinder.gameObject.SetActive(true);
        }
    }

    private void DisableDebugging()
    {
        if (DebugCapsule.gameObject.activeSelf)
        {
            DebugCapsule.gameObject.SetActive(false);
        }

        if (DebugCylinder.gameObject.activeSelf)
        {
            DebugCylinder.gameObject.SetActive(false);
        }
    }
	
	void Update ()
    {
        if(DebugInputs)
        {
            EnableDebugging();

            Ray selectorRay = SelectorRay;
            Debug.DrawRay(selectorRay.origin, selectorRay.direction, Color.red, 0.0f, false);
            DebugCapsule.position = selectorRay.origin;
            DebugCapsule.rotation = Quaternion.LookRotation(selectorRay.direction, Vector3.up);

            if (IsConfirmPressed)
            {
                RaycastHit hit;
                if (Physics.Raycast(selectorRay, out hit))
                {
                    DebugCylinder.position = hit.point;
                }
            }
        }
        else
        {
            DisableDebugging();
        }
	}


    // ACTIONS
    public bool IsUserRightHanded = true;
    public bool DebugInputs = false;
    public Transform DebugCapsule;
    public Transform DebugCylinder;

    private bool IsSelectorControllerActive()
    {
        return ControllerManager != null && ((IsUserRightHanded && ControllerManager.right.activeSelf) || (!IsUserRightHanded && ControllerManager.left.activeSelf));
    }

    private Transform GetSelectorController()
    {
        if (!IsSelectorControllerActive())
            return null;

        if(IsUserRightHanded)
        {
            return ControllerManager.right.transform;
        }
        else
        {
            return ControllerManager.left.transform;
        }
    }

    public Ray SelectorRay
    {
        get
        {
            Ray selectorRay = new Ray();

            if (IsSelectorControllerActive())
            {
                Transform selectorController = GetSelectorController();
                selectorRay.origin = selectorController.localPosition;
                selectorRay.direction = selectorController.forward;
            }
            else
            {
                if(Camera.main != null)
                {
                    selectorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                }
            }

            return selectorRay;
        }
    }

    public bool IsConfirmPressed
    {
        get
        {
            if (IsSelectorControllerActive())
            {
                if(IsUserRightHanded)
                {
                    return RightController.GetHairTriggerDown() && !(LeftController.GetHairTrigger());
                }
                else
                {
                    return LeftController.GetHairTriggerDown() && !(RightController.GetHairTrigger());
                }
            }
            else
            {
                return Input.GetMouseButtonDown(0);
            }
        }
    }

    public bool IsConfirmHeld
    {
        get
        {
            if (IsSelectorControllerActive())
            {
                if (IsUserRightHanded)
                {
                    return RightController.GetHairTrigger() && !(LeftController.GetHairTrigger());
                }
                else
                {
                    return LeftController.GetHairTrigger() && !(RightController.GetHairTrigger());
                }
            }
            else
            {
                return Input.GetMouseButton(0);
            }
        }
    }

    public bool IsConfirmReleased
    {
        get
        {
            if (IsSelectorControllerActive())
            {
                if (IsUserRightHanded)
                {
                    return RightController.GetHairTriggerUp() && !(LeftController.GetHairTrigger());
                }
                else
                {
                    return LeftController.GetHairTriggerUp() && !(RightController.GetHairTrigger());
                }
            }
            else
            {
                return Input.GetMouseButtonUp(0);
            }
        }
    }

    public bool IsDoubleTriggerDown
    {
        get
        {
            if(IsSelectorControllerActive())
            {
                return RightController.GetHairTrigger() && LeftController.GetHairTrigger();
            }
            else
            {
                // If PC always return False
                return false;
            }
        }
    }

     
    public Vector3 ControllerToControllerDirection
    {
        get
        {
            Vector3 direction = ControllerManager.right.transform.position - ControllerManager.left.transform.position;
            if(direction.sqrMagnitude > 0)
            {
                direction.Normalize();
            }

            return direction;
        }
    }

    public bool IsMenuActionPressed
    {
        get
        {
            if(IsSelectorControllerActive())
            {
                if(IsUserRightHanded)
                {
                    return RightController.GetPressDown(EVRButtonId.k_EButton_SteamVR_Touchpad);
                }
                else
                {
                    return LeftController.GetPressDown(EVRButtonId.k_EButton_SteamVR_Touchpad);
                }
            }
            else
            {
                // FOR PC We want to return false for now until controllers are unified
                //return Input.GetKeyDown(KeyCode.Return);
                return false;
            }
        }
    }

    public bool IsMenuConfirmPressed
    {
        get
        {
            if (IsSelectorControllerActive())
            {
                if (IsUserRightHanded)
                {
                    return RightController.GetPressDown(EVRButtonId.k_EButton_Grip);
                }
                else
                {
                    return LeftController.GetPressDown(EVRButtonId.k_EButton_Grip);
                }
            }
            else
            {
                // FOR PC We want to return false for now until controllers are unified
                //return Input.GetKeyDown(KeyCode.Return);
                return false;
            }
        }
    }
}
