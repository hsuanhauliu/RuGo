using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 


public class LoadMenu : Menu
{
    public GameManager gameManager;
    public MainMenu mainMenu;
    public World world;

    public float padding = 20f;

	// Use this for initialization
	void Start ()
    {
        ReparentMenu();
        BuildToolBar();
        this.Deactivate();
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

    /************************** Public Functions **************************/

    public void Activate()
    {
        BuildToolBar();
        this.gameObject.SetActive(true);
    }

    public void GoToMainMenu()
    {
        this.Deactivate();
        mainMenu.Activate();
        //MainMenu.Activate();
        //gameManager.EnableSelectMode();
    }

    /************************** Private Functions **************************/

    private void BuildToolBar()
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

    private void AddButtonToToolBar(string buttonName, float horizontalOffset, float verticalOffset)
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
            collider.center = new Vector3(0, -collider.size.y / 2, 0);
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
}
