using UnityEngine;

public class MenuManager : MonoBehaviour
{
    GadgetSelectorMenu SelectorMenu;
    LoadMenu LoadMenu;
    MainMenu MainMenu;

    public bool IsVrRun = false;

    // Use this for initialization
    void Start ()
    {
        if (IsVrRun)
        {
            SelectorMenu.SetVRFlag(true);
            LoadMenu.SetVRFlag(true);
            MainMenu.SetVRFlag(true);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
