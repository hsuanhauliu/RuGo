using UnityEngine;

public class Gadget : MonoBehaviour
{
    public Material SolidMat;
    public Material TransparentMat;

    private Vector3 lastSavedPosition;
    private Renderer renderer
    {
        get
        {
            return this.GetComponent<Renderer>();
        }
    }

    void Start ()
    {
    }


    /************************** Public Functions **************************/

    // Function: Highlight
    // Input: none
    // Output: none
    // Description:
    //  - Change the color of the gadget when being clicked on in manipulate
    //    mode.
    public virtual void Highlight ()
    {
        Debug.Log("Highlighting Gadget");

        renderer.material.color = Color.yellow;
    }

    // Function: Deselect
    // Input: none
    // Output: none
    // Description:
    //  - Revert the color to its original and save current position.
    public virtual void Deselect ()
    {
        Debug.Log("Color should be restored");

        Solidify();
        lastSavedPosition = this.transform.position;
    }

    // Function: Reset
    // Input: none
    // Output: none
    // Description:
    //  - Move the gadget back to its original position.
    public virtual void Reset ()
    {
        Debug.Log("Gadget position is being reset.");

        this.transform.position = lastSavedPosition;
        //this.Deselect(); do we need this?
    }

    // Function: Solidify
    // Input: none
    // Output: none
    // Description:
    //  - Change the material to solid.
    public virtual void Solidify ()
    {
        Debug.Log("Making Gadget transparent");

        renderer.material = SolidMat;
    }

    // Function: Transparent
    // Input: none
    // Output: none
    // Description:
    //  - Change the material to transparent.
    public virtual void Transparent ()
    {
        Debug.Log("Making Gadget transparent");

        renderer.material = TransparentMat;
    }
}
