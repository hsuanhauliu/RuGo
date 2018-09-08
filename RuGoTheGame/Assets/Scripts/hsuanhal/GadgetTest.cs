using UnityEngine;

public class GadgetTest : MonoBehaviour
{
    // The full and transparent materials of the gadget
    public Material solidMat;
    public Material transparentMat;

    // Local variables
    private Vector3 lastSavedPosition;

    void Start ()
    {
        Renderer r = this.GetComponent<Renderer>();
        r.material = solidMat;
    }

    // higtlight the gadget when being clicked in manipulate mode
    public virtual void Highlight ()
    {
        Renderer r = this.GetComponent<Renderer>();
        r.material.color = Color.yellow;
        Debug.Log("Highlighting Gadget");
    }

    // cancel selection
    public virtual void Deselect ()
    {
        Renderer r = this.GetComponent<Renderer>();
        r.material = solidMat;
        Debug.Log("Color should be restored");

        lastSavedPosition = this.transform.position;
    }

    public virtual void Reset ()
    {
        this.transform.position = lastSavedPosition;
        this.Deselect();
    }

    public virtual void Transparent ()
    {
        Renderer r = this.GetComponent<Renderer>();
        r.material = transparentMat;
        Debug.Log("Making Gadget transparent");
    }
}
