using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 


public class LoadMenu : MonoBehaviour {
    public GameManager gameManager;
    public MainMenu mainMenu;
    public World world;
    public float padding = 20f;

    public bool IsVrRun = false;

	// Use this for initialization
	void Start () {
        ReparentSelectorMenu();
        BuildToolBar();
        this.Deactivate();
		
	}
	
	// Update is called once per frame
	void Update () {
	}
    public void BuildToolBar()
    {

        float verticalOffset = 300f;
        float horizontalOffset = 0f;
        
        AddButtonToToolBar("Back", horizontalOffset - 330f, verticalOffset + 450f);

        string[] fileArray = Directory.GetFiles(@"SavedGames");
        string buttonType ="BasicButton";

        for (int i = 0; i < fileArray.Length; i++)
        {
            Debug.Log(fileArray[i]);
                    AddButtonToToolBar(fileArray[i] , horizontalOffset, verticalOffset - 500f -(i*300f));
        }




    }


    public void AddButtonToToolBar(string buttonName, float horizontalOffset, float verticalOffset)
    {
        GameObject SmallButton = Resources.Load("smallButton") as GameObject;

        if (string.Equals("Back", buttonName))
        {
             SmallButton = Resources.Load("smallButton") as GameObject;

        }
        else
        {
             SmallButton = Resources.Load("BasicButton") as GameObject;

        }

        GameObject gadgetButton = (GameObject)Instantiate(SmallButton, this.transform);
        UnityEngine.UI.Button uiButton = gadgetButton.GetComponent<UnityEngine.UI.Button>();

        
        RectTransform rectTransform = uiButton.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + horizontalOffset, verticalOffset + padding);
        if (IsVrRun)
        {
            BoxCollider collider = gadgetButton.AddComponent<BoxCollider>();
            collider.size = uiButton.GetComponent<RectTransform>().rect.size;
        }
        string buttonIdentifier = buttonName.Replace("SavedGames/", "");
        uiButton.GetComponentInChildren<UnityEngine.UI.Text>().text = buttonIdentifier;

        if (string.Equals("Back", buttonName))
        {
            //this.Deactivate();

            uiButton.onClick.AddListener(() => this.GoToMainMenu()); //this.Deactivate());
        }
        else
        {
            uiButton.onClick.AddListener(() => world.Load(buttonName)); //this.Deactivate());

        }


    }

    public void ReparentSelectorMenu()
    {
        // Parent the gadget selector menu underneath the main camera
        GameObject menuParent = GameObject.FindGameObjectWithTag("MainCamera");
        transform.SetParent(menuParent.transform);
        transform.localPosition = new Vector3(0, 0, 1);
        transform.localRotation = Quaternion.identity;
    }

    public void Activate()
    {
        BuildToolBar();

        this.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    public void GoToMainMenu()
    {
        this.Deactivate();
        mainMenu.Activate();
        //MainMenu.Activate();
        //gameManager.EnableSelectMode();
    }
}
