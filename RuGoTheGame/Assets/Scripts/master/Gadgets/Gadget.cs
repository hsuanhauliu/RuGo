using UnityEngine;
using System.Collections.Generic;

public enum GadgetInventory
{
    Box, Ball, RailRamp, PathTool
};

public abstract class Gadget : MonoBehaviour
{
    private Vector3 mLastSavedPosition;

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
        Rigidbody rigidBody = this.GetComponent<Rigidbody>();
        if (rigidBody)
        {
            rigidBody.isKinematic = !enable;
            rigidBody.detectCollisions = enable;
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
            this.GetComponent<Renderer>()
        };
    }

    /************************** Public Functions **************************/
    public virtual void Highlight()
    {
        //TODO Can we simplify this??
        this.gameObject.layer = LayerMask.NameToLayer("SelectedGadget");
        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("SelectedGadget");
        }

        foreach (Renderer GadgetRenderer in mRenderers)
        {
            GadgetRenderer.material.color = Color.yellow;
        }
    }

    public virtual void Deselect()
    {
        MakeSolid();
    }

    public virtual void Reset()
    {
        this.transform.position = mLastSavedPosition;
    }

    public virtual void MakeSolid()
    {
        mLastSavedPosition = this.transform.position;

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
