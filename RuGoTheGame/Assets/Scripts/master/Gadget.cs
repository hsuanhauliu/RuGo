using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget : MonoBehaviour {

    public Color originalColor;

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
    }
}
