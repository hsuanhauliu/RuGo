using UnityEngine;

public class GadgetSelectorMenu : MonoBehaviour
{
	void Start () {
		
	}
	
	void Update () {
		
	}


    /************************** Public Functions **************************/

    // Function: Activate
    // Input: none
    // Output: none
    // Description:
    //  - Activate GadgetSelectorMenu gameObject in the scene.
    public void Activate ()
    {
        this.gameObject.SetActive(true);
    }

    // Function: Deactivate
    // Input: none
    // Output: none
    // Description:
    //  - Deactivate GadgetSelectorMenu gameObject in the scene.
    public void Deactivate ()
    {
        this.gameObject.SetActive(false);
    }
}
