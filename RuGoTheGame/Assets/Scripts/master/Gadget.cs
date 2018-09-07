using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget : MonoBehaviour {

    public Color originalColor;

    private Vector3 lastSavedPosition;

    void Start()
    {
        Renderer r = this.GetComponent<Renderer>();
        originalColor = r.material.color;
    }

    public virtual void Highlight() {
        Renderer r = this.GetComponent<Renderer>();
        r.material.color = Color.yellow;
        Debug.Log("Highlighting Gadget");
    }

    public virtual void Deselect() {
        Renderer r = this.GetComponent<Renderer>();
        r.material.color = Color.black;
        Debug.Log("Color should be restored");

        lastSavedPosition = this.transform.position;
    }

    public virtual void Reset() {
        this.transform.position = lastSavedPosition;
        this.Deselect();
    }
}
