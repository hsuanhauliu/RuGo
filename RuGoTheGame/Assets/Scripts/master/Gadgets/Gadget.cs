using UnityEngine;
using System.Collections.Generic;

public enum GadgetInventory
{
    Box, Ball, RailRamp, PathTool, SmallCannon, Spinner, Fan
};

public abstract class Gadget : MonoBehaviour
{
    private Vector3 mLastSavedPosition;
    private Quaternion mLastSavedOrientation;

    private List<Renderer> mRenderers;

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Gadget");
    }

    void Awake()
    {
        InitializeGadget();
    }

    protected void InitializeGadget()
    {
        mRenderers = GetRenderers();
        SetPhysicsMode(false);
    }

    /// <summary>
    /// Enables and Disables rigid body physics and collision detection
    /// </summary>
    /// <param name="enable">If set to <c>true</c> enable rigid body physics and collision detection.</param>
    protected virtual void SetPhysicsMode(bool enable)
    {
        Rigidbody[] rigidBodies = this.GetComponentsInChildren<Rigidbody>();
        if (rigidBodies.Length > 0)
        {
            foreach (Rigidbody body in rigidBodies)
            {
                body.isKinematic = !enable;
                body.detectCollisions = enable;
            }
        }
        else
        {
            MakeAllCollidersTrigger(!enable);
        }
    }

    protected virtual void MakeAllCollidersTrigger(bool isTrigger)
    {
        Collider[] colliders = this.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.isTrigger = isTrigger;
        }
    }

    protected virtual List<Renderer> GetRenderers()
    {
        return new List<Renderer>
        {
            this.GetComponentInChildren<Renderer>()
        };
    }

    /************************** Public Functions **************************/

    public virtual void Deselect()
    {
        MakeSolid();
    }

    public virtual void Reset()
    {
        this.transform.position = mLastSavedPosition;
        this.transform.rotation = mLastSavedOrientation;
    }

    public virtual void Remove()
    {
        Destroy(this.gameObject);
    }

    public virtual void MakeSolid()
    {
        mLastSavedPosition = this.transform.position;
        mLastSavedOrientation = this.transform.rotation;

        foreach (Renderer GadgetRenderer in mRenderers)
        {
            Color albedo = GadgetRenderer.material.color;
            albedo.a = 1.0f;
            GadgetRenderer.material.color = albedo;
        }

        foreach (Transform t in transform)
        {
            t.gameObject.layer = LayerMask.NameToLayer("Gadget");
        }

        SetPhysicsMode(true);
    }

    public virtual void MakeTransparent()
    {
        SetPhysicsMode(false);

        //TODO Remove the duplication between highlight and transparent....
        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("SelectedGadget");
        }

        foreach (Renderer GadgetRenderer in mRenderers)
        {
            Color albedo = GadgetRenderer.material.color;
            albedo.a = 0.5f;
            GadgetRenderer.material.color = albedo;
        }
    }
}
