using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

public enum GadgetInventory
{
    PathTool, RailRamp, Ball, Box, SmallCannon, Spinner, Fan, Airplane, Domino, NUM
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
    private Vector3 mLastSavedPosition;
    private Quaternion mLastSavedOrientation;
    private Vector3 mBodyLastSavedPosition;
    private Quaternion mBodyLastSavedOrientation;
    private bool isPhysicsMode;

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

    /************************** Public Functions **************************/

    public virtual void Deselect()
    {
        MakeSolid();
    }

    public virtual void PerformSwitchAction() {
        Debug.Log("Perform Some Action");
    }

    public virtual void Reset()
    {
        this.RestoreState();
    }

    public virtual void RemoveFromScene()
    {
        Destroy(this.gameObject);
    }

    public virtual void StoreState() {
        mLastSavedPosition = this.transform.position;
        mLastSavedOrientation = this.transform.rotation;

        Rigidbody body = this.GetComponentInChildren<Rigidbody>();

        if (body) {
            mBodyLastSavedPosition = body.transform.position;
            mBodyLastSavedOrientation = body.transform.rotation;
        }
    }

    public virtual void RestoreState() {
        this.transform.position = mLastSavedPosition;
        this.transform.rotation = mLastSavedOrientation;

        Rigidbody body = this.GetComponentInChildren<Rigidbody>();
        if (body) {
            body.transform.position = mBodyLastSavedPosition;
            body.transform.rotation = mBodyLastSavedOrientation;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }


    public virtual void MakeSolid()
    {
        this.StoreState();
    
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

    public virtual bool GetPhysicsMode()
    {
        return isPhysicsMode;
    }
}
