using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gadget : MonoBehaviour {

    public virtual void Highlight() {
        Renderer r = this.GetComponent<Renderer>();
        r.material.color = Color.yellow;
    }
}
