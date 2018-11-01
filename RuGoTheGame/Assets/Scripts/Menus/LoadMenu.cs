using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 


public class LoadMenu : Menu
{
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

    public override void Activate()
    {
        BuildToolBar();
        base.Activate();
    }

    public void GoToMainMenu()
    {
        this.Deactivate();
        mainMenu.Activate();
    }

    /************************** Private Functions **************************/

    private void BuildToolBar()
    {
        float verticalOffset = 300f;
        float horizontalOffset = 0f;
        
        AddButtonToToolBar("Back", horizontalOffset - 330f, verticalOffset + 450f);

        string[] fileArray = Directory.GetDirectories(@"SavedGames");
        
        for (int i = 0; i < fileArray.Length; i++)
        {
            string fileName = fileArray[i];
#if UNITY_STANDALONE_WIN
            fileName = fileName.Replace("SavedGames\\", "");
#elif UNITY_STANDALONE_OSX
            fileName = fileName.Replace("SavedGames/", "");
#endif
            AddButtonToToolBar(fileName, horizontalOffset, verticalOffset - 500f -(i*300f));
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
        uiButton.GetComponentInChildren<UnityEngine.UI.Text>().text = buttonName;

        if (string.Equals("Back", buttonName))
        {
            uiButton.onClick.AddListener(() => this.GoToMainMenu());
        }
        else
        {
            uiButton.onClick.AddListener(() => world.LoadWorld(buttonName));
        }
    }
}
