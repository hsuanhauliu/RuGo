using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private SteamVR_TrackedObject LeftController;
    private SteamVR_TrackedObject RightController;

    private void CacheControllers()
    {
        ControllerManager = GetComponent<SteamVR_ControllerManager>();
        
        LeftController    = ControllerManager.left.GetComponent<SteamVR_TrackedObject>();
        RightController   = ControllerManager.right.GetComponent<SteamVR_TrackedObject>();
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
	
	void Update ()
    {
		
	}
}
