using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

public enum GadgetInventory
{
    PathTool, RailRamp, Ball, Box, SmallCannon, Spinner, Fan, Airplane, Domino, Pendulum, NUM
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
    protected bool isPhysicsMode;

    private List<Renderer> mRenderers;

    private List<Vector3> mChildData;

    protected void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Gadget");
    }

    void Awake()
    {
        InitializeGadget();

        mChildData = new List<Vector3>();
        foreach (Transform child in transform)
        {
            mChildData.Add(child.localPosition);
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

    public virtual void RemoveFromScene()
    {
        Destroy(this.gameObject);
    }

    public virtual void MakeSolid()
    {
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
        Vector3 firstPosition = Vector3.zero;
        int childCount = transform.childCount;
        for (int childIndex = 0; childIndex < childCount; childIndex++)
        {
            Transform child = transform.GetChild(childIndex);
            if (childIndex == 0)
            {
                firstPosition = child.position;
            }

            child.gameObject.layer = LayerMask.NameToLayer("SelectedGadget");
            child.localPosition = mChildData[childIndex];   
        }
        if(childCount > 0)
        {
            transform.position = firstPosition + mChildData[0];
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
