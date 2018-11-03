using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

public enum GadgetInventory
{
    PathTool, Ramp, Ball, Box, Cannon, Spinner, Fan, Airplane, Domino, Pendulum, NUM
};

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

    protected bool isPhysicsMode;

    private List<Renderer> mRenderers;
    
    private enum GadgetState { OnTable, InWorld };
    private GadgetState currentGadgetState;

    protected void Start()
    {
        currentGadgetState = GadgetState.OnTable;
    }

    protected void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Gadget");
        InitializeGadget();

        MakeGadgetInteractable();
    }

    private void Update()
    {
        if (MakeGadgetSolid)
        {
            MakeGadgetSolid = false;
            MakeSolid();
        }
    }

    protected void InitializeGadget()
    {
        mRenderers = GetRenderers();
        SetPhysicsMode(false);
    }

    public GadgetSaveData GetSaveData() {
        return new GadgetSaveData(this.GetGadgetType(), this.transform.position, this.transform.rotation);
    }

    public void RestoreStateFromSaveData(GadgetSaveData data) {
        this.transform.position = data.GetPosition();
        this.transform.rotation = data.GetQuaternion();
        this.MakeSolid();
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
                if(body.gameObject.GetComponent<Collider>() is MeshCollider)
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

    protected virtual List<Renderer> GetRenderers()
    {
        return new List<Renderer>
        {
            this.GetComponentInChildren<Renderer>()
        };
    }

    /************************** Gadget + VRTK *****************************/
    private void MakeGadgetInteractable()
    {
        VRTK.VRTK_InteractableObject interactableObject = gameObject.AddComponent<VRTK.VRTK_InteractableObject>();
        VRTK.GrabAttachMechanics.VRTK_ChildOfControllerGrabAttach childOfController = gameObject.AddComponent<VRTK.GrabAttachMechanics.VRTK_ChildOfControllerGrabAttach>();
        interactableObject.grabAttachMechanicScript = childOfController;
        interactableObject.isGrabbable = true;
        interactableObject.disableWhenIdle = false;
        childOfController.precisionGrab = true;

        interactableObject.InteractableObjectGrabbed += OnGadgetGrabbed;
        interactableObject.InteractableObjectUngrabbed += OnGadgetUnGrabbed;

        interactableObject.enabled = true;
    }

    protected void OnGadgetUnGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        switch(currentGadgetState) 
        {
            case GadgetState.OnTable:
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

    }

    protected void OnGadgetGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        if(currentGadgetState == GadgetState.OnTable)
        {
            World.Instance.ToggleShelf();
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
        print(t.gameObject.name + " " + layerName);

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

    private void ChangeState(GadgetState newState)
    {
        currentGadgetState = newState;
    }
}
