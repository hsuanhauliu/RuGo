using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

public enum GadgetInventory
{
    Ramp, Ball, LightMarble, Goal, Box, Cannon, Spinner, Fan, Airplane, Domino, Pendulum, Floater, NUM // #TODO: If time permits, split the heirarchy to Insertable gadgets and Tools gadgets for LoadCube
}

public class GadgetLayers
{
    public static readonly string SHELF = "Shelf";
    public static readonly string INWORLD = "InWorld";
}

[Serializable]
public struct GadgetSaveData
{
    public string name;
    public float px;
    public float py;
    public float pz;

    public float ox;
    public float oy;
    public float oz;
    public float ow;

    public GadgetSaveData(GadgetInventory name, Vector3 position, Quaternion orientation) {
        this.name = name.ToString();
        px = position.x;
        py = position.y;
        pz = position.z;

        ox = orientation.x;
        oy = orientation.y;
        oz = orientation.z;
        ow = orientation.w;
    }

    public Vector3 GetPosition() {
        return new Vector3(px, py, pz);
    }

    public Quaternion GetQuaternion() {
        return new Quaternion(ox, oy, oz, ow);
    }
};

public abstract class Gadget : MonoBehaviour
{
    public bool MakeGadgetSolid = false;
    public bool HapticsEnabled = true;
    private VRTK.VRTK_InteractableObject mInteractableObject;
    protected GadgetSaveData mGadgetSaveData;

    protected bool isPhysicsMode;
    
    public enum GadgetState { InShelf, FirstPlacement, InWorld, Loaded };
    public GadgetState CurrentGadgetState;

    protected void Start()
    {
       
    }

    protected void Awake()
    {
        MakeGadgetInteractable();
        ChangeState(GadgetState.InShelf);
    }

    private void Update()
    {
        if (MakeGadgetSolid)
        {
            MakeGadgetSolid = false;
            MakeSolid();
        }
        if (this.transform.position.y < -10.0f)
        {
            World.Instance.RemoveGadget(this);
        }
    }

    private void LateUpdate()
    {
        // We do this to avoid the following problem:
        // --- On load, the gadget is instantiated but the positions are not set till end of frame. 
        // --- So Making the gadgets solid will apply physics to the components that will try to get to the new position.
        if(CurrentGadgetState == GadgetState.Loaded)
        {
            ChangeState(GadgetState.InWorld);
            MakeSolid();
        }
    }

    protected void UpdateGadgetSaveData() 
    {
        mGadgetSaveData.name = this.GetGadgetType().ToString();
        mGadgetSaveData.px = this.transform.position.x;
        mGadgetSaveData.py = this.transform.position.y;
        mGadgetSaveData.pz = this.transform.position.z;
        mGadgetSaveData.ox = this.transform.rotation.x;
        mGadgetSaveData.oy = this.transform.rotation.y;
        mGadgetSaveData.oz = this.transform.rotation.z;
        mGadgetSaveData.ow = this.transform.rotation.w;
    }

    public GadgetSaveData GetSaveData() 
    {
        return mGadgetSaveData;

        //return new GadgetSaveData(this.GetGadgetType(), this.transform.position, this.transform.rotation);
    }

    public void RestoreStateFromSaveData(GadgetSaveData data) {
        this.transform.position = data.GetPosition();
        this.transform.rotation = data.GetQuaternion();
        ChangeState(GadgetState.Loaded);
        UpdateGadgetSaveData();
    }

    public abstract GadgetInventory GetGadgetType();

    /// <summary>
    /// Enables and Disables rigid body physics and collision detection
    /// </summary>
    /// <param name="enable">If set to <c>true</c> enable rigid body physics and collision detection.</param>
    protected virtual void SetPhysicsMode(bool enable, bool keepCollision=false)
    {
        Rigidbody[] rigidBodies = this.GetComponentsInChildren<Rigidbody>();

        if (rigidBodies.Length > 0)
        {
            foreach (Rigidbody body in rigidBodies)
            {
                bool shouldBeKinematic = !enable;
                // If we have a non-convex mesh collider set is kinematic to true.
                MeshCollider attachedCollider = body.gameObject.GetComponent<Collider>() as MeshCollider;
                if (attachedCollider != null && !attachedCollider.convex)
                {
                    shouldBeKinematic = true;
                }
                
                body.isKinematic = shouldBeKinematic;
                body.detectCollisions = enable || keepCollision;
            }
        }
        else
        {
            EnableColliders(enable);
        }
        isPhysicsMode = enable;
    }

    protected virtual void EnableColliders(bool enable)
    {
        Collider[] colliders = this.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = enable;
        }
    }

    /************************** Gadget + VRTK *****************************/
    private void MakeGadgetInteractable()
    {
        mInteractableObject = gameObject.AddComponent<VRTK.VRTK_InteractableObject>();
        mInteractableObject.isGrabbable = true;
        mInteractableObject.disableWhenIdle = false;
        mInteractableObject.InteractableObjectTouched += OnGadgetTouched;
        mInteractableObject.InteractableObjectGrabbed += OnGadgetGrabbed;
        mInteractableObject.InteractableObjectUngrabbed += OnGadgetUnGrabbed;

        if (HapticsEnabled) {
            EnableHaptics();
        }

        VRTK.GrabAttachMechanics.VRTK_ChildOfControllerGrabAttach childOfController = gameObject.AddComponent<VRTK.GrabAttachMechanics.VRTK_ChildOfControllerGrabAttach>();
        mInteractableObject.grabAttachMechanicScript = childOfController;
        childOfController.precisionGrab = true;

        mInteractableObject.enabled = true;
    }

    private void EnableHaptics() 
    {
        VRTK.VRTK_InteractHaptics interactHaptics = gameObject.AddComponent<VRTK.VRTK_InteractHaptics>();
        interactHaptics.strengthOnTouch = 0.285f;
        interactHaptics.durationOnTouch = 0.05f;
        interactHaptics.intervalOnTouch = 0.05f;
        interactHaptics.cancelOnUntouch = true;

        interactHaptics.strengthOnGrab = 1.0f;
        interactHaptics.durationOnGrab= 0.15f;
        interactHaptics.intervalOnGrab = 0.02f;
        interactHaptics.cancelOnUngrab = true;

    }

    private void OnGadgetTouched(object sender, VRTK.InteractableObjectEventArgs e)
    {
        GadgetTouched();
    }

    protected virtual void GadgetTouched()
    {
        if (GameManager.Instance.CurrentGameMode == GameMode.DELETE)
        {
            World.Instance.RemoveGadget(this);
        }
    }

    private void OnGadgetUnGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        UnGrabGradget();

        // TODO: We should be doing this in the GameManager, so that there is a global reference to gadget being grabbed
        GameManager.Instance.RightAnimator.MakeHandIdle();
    }

    protected virtual void UnGrabGradget()
    {
        switch (CurrentGadgetState)
        {
            case GadgetState.FirstPlacement:
                {
                    ChangeState(GadgetState.InWorld);
                    World.Instance.InsertGadget(this);
                }
                break;
            case GadgetState.InWorld:
                {
                    World.Instance.MarkWorldModified();
                }
                break;
        }
        MakeSolid();
        UpdateGadgetSaveData();
    }
    

    private void OnGadgetGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        GrabGradget();

        // TODO: We should be doing this in the GameManager, so that there is a global reference to gadget being grabbed
        GameManager.Instance.RightAnimator.MakeHandGrab();
    }

    protected virtual void GrabGradget()
    {
        if (CurrentGadgetState == GadgetState.InShelf)
        {
            ChangeState(GadgetState.FirstPlacement);

            mInteractableObject.OverridePreviousState(World.Instance.transform, false, true);
            GameManager.Instance.ChangeGameMode(GameMode.BUILD);
        }

        MakeTransparent();
    }

    /************************** Public Functions **************************/

    public virtual void Deselect()
    {
        MakeSolid();
    }

    public virtual void PerformSwitchAction() {
        Debug.Log("Perform Some Action");
    }

    public virtual void RemoveFromScene()
    {   
        Destroy(this.gameObject);
    }

    protected void SetLayer(Transform t, string layerName)
    {
        t.gameObject.layer = LayerMask.NameToLayer(layerName);

        foreach (Transform child in t)
        {
            SetLayer(child, layerName);
        }
    }

    public void SetLayer(string layerName)
    {
        SetLayer(this.transform, layerName);
    }

    public virtual void MakeSolid()
    {
        SetPhysicsMode(true);
    }

    public virtual void MakeTransparent(bool keepCollision=false)
    {
        SetPhysicsMode(false, keepCollision);
    }

    public virtual bool GetPhysicsMode()
    {
        return isPhysicsMode;
    }

    public virtual void ShowShelf(bool show)
    {

    }

    protected void ChangeState(GadgetState newState)
    {
        CurrentGadgetState = newState;
        if (CurrentGadgetState == GadgetState.InWorld || CurrentGadgetState == GadgetState.FirstPlacement)
        {
            SetLayer(GadgetLayers.INWORLD);
        }
        else 
        {
            SetLayer(GadgetLayers.SHELF);
            MakeTransparent(true);
        }
    }
}
