using UnityEngine;

public class Gadget : MonoBehaviour
{
    // The full and transparent materials of the gadget
    public Material SolidMat;
    public Material TransparentMat;

    // Local variables
    private Vector3 lastSavedPosition;
    private Renderer renderer
    {
        get
        {
            return this.GetComponent<Renderer>();
        }
    }

    void Start()
    {
    }

    // higtlight the gadget when being clicked in manipulate mode
    public virtual void Highlight()
    {
        renderer.material.color = Color.yellow;
        Debug.Log("Highlighting Gadget");
    }

    // cancel selection
    public virtual void Deselect()
    {
        Solidify();
        Debug.Log("Color should be restored");

        lastSavedPosition = this.transform.position;
    }

    public virtual void Reset()
    {
        this.transform.position = lastSavedPosition;
        this.Deselect();
    }

    public virtual void Transparent()
    {
        renderer.material = TransparentMat;
        Debug.Log("Making Gadget transparent");
    }

    public virtual void Solidify()
    {
        renderer.material = SolidMat;
        Debug.Log("Making Gadget transparent");
    }
}
