using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

public enum GadgetInventory
{
    Ramp, Ball, Goal, Box, Cannon, Spinner, Fan, Airplane, Domino, Pendulum, Floater, LoadCube, NUM // #TODO: Insert Goal here
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
    private VRTK.VRTK_InteractableObject mInteractableObject;

    protected bool isPhysicsMode;
    
    public enum GadgetState { InShelf, FirstPlacement, InWorld };
    public GadgetState CurrentGadgetState;

    protected void Start()
    {
       
    }

    protected void Awake()
    {
        MakeGadgetInteractable();
        CurrentGadgetState = GadgetState.InShelf;
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
        VRTK.GrabAttachMechanics.VRTK_ChildOfControllerGrabAttach childOfController = gameObject.AddComponent<VRTK.GrabAttachMechanics.VRTK_ChildOfControllerGrabAttach>();
        mInteractableObject.grabAttachMechanicScript = childOfController;
        mInteractableObject.isGrabbable = true;
        mInteractableObject.disableWhenIdle = false;
        childOfController.precisionGrab = true;

        mInteractableObject.InteractableObjectGrabbed += OnGadgetGrabbed;
        mInteractableObject.InteractableObjectUngrabbed += OnGadgetUnGrabbed;

        mInteractableObject.enabled = true;
    }

    protected void OnGadgetUnGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        switch(CurrentGadgetState) 
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

    }

    protected void OnGadgetGrabbed(object sender, VRTK.InteractableObjectEventArgs e)
    {
        if(this.name.Contains("LoadCube"))
        {
            RemoveFromScene();
            World.Instance.LoadWorld(this.name.Replace("LoadCube", "").Split(' ')[0].Trim());
            World.Instance.RespawnFiles();

            return;
        }
        if (CurrentGadgetState == GadgetState.InShelf)
        {
            ChangeState(GadgetState.InWorld);
            MakeTransparent();

            ChangeState(GadgetState.FirstPlacement);

            mInteractableObject.OverridePreviousState(World.Instance.transform, false, true);
            GameManager.Instance.ChangeGameMode(GameMode.NONE);
        }
        else
        {
            MakeTransparent();
        }
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

    protected void ChangeState(GadgetState newState)
    {
        CurrentGadgetState = newState;
    }
}
