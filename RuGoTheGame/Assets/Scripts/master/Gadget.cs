using UnityEngine;
using System.Collections.Generic;

public enum GadgetInventory
{
    Box, Ball, RailRamp
};

public abstract class Gadget : MonoBehaviour
{
    private Vector3 mLastSavedPosition;

    private List<Renderer> mRenderers;

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Gadget");
    }

    protected void InitGadget()
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

    void Awake()
    {
        InitGadget();
    }

    /************************** Public Functions **************************/

    // Function: Highlight
    // Input: none
    // Output: none
    // Description:
    //  - Change the color of the gadget when being clicked on in manipulate
    //    mode.
    public virtual void Highlight()
    {
        Debug.Log("Highlighting Gadget");
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

    // Function: Deselect
    // Input: none
    // Output: none
    // Description:
    //  - Revert the color to its original and save current position.
    public virtual void Deselect()
    {
        Debug.Log("Color should be restored");

        Solidify();
        mLastSavedPosition = this.transform.position;
    }

    // Function: Reset
    // Input: none
    // Output: none
    // Description:
    //  - Move the gadget back to its original position.
    public virtual void Reset()
    {
        Debug.Log("Gadget position is being reset.");

        this.transform.position = mLastSavedPosition;
    }

    // Function: Solidify
    // Input: none
    // Output: none
    // Description:
    //  - Change the material to solid.
    public virtual void Solidify()
    {
        Debug.Log("Making Gadget Solid");
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

    // Function: Transparent
    // Input: none
    // Output: none
    // Description:
    //  - Change the material to transparent.
    public virtual void Transparent()
    {
        SetPhysicsMode(false);
        Debug.Log("Making Gadget transparent");

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
