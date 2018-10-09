using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    public bool IsVrRun = false;
    public float padding = 20f;
    public GameManager gameManager;



	// Use this for initialization
	void Start () {
        ReparentSelectorMenu();

        BuildToolBar();
        this.gameObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {

	}
    public void BuildToolBar()
    {
        

        float verticalOffset = 750f;
        float horizontalOffset = 0f;
        string buttonType  = "smallButton";
        AddButtonToToolBar("Reset", buttonType ,horizontalOffset, verticalOffset);
        //AddButtonToToolBar("Back", buttonType ,horizontalOffset - 330f, verticalOffset);
        AddButtonToToolBar("Save", buttonType ,horizontalOffset + 330f, verticalOffset);
        AddButtonToToolBar("Close", buttonType, horizontalOffset -330f, verticalOffset);
        buttonType = "BasicButton";
        AddButtonToToolBar("Insert Gadget", buttonType , horizontalOffset, verticalOffset - 500f);
        AddButtonToToolBar("Load", buttonType, horizontalOffset, verticalOffset - 800f);

    }

    public void AddButtonToToolBar(string buttonName, string buttonType ,float horizontalOffset, float verticalOffset)
    {

        GameObject SmallButton = Resources.Load("smallButton") as GameObject;
        GameObject gadgetButton = (GameObject)Instantiate(SmallButton, this.transform);
        UnityEngine.UI.Button uiButton = gadgetButton.GetComponent<UnityEngine.UI.Button>();

        
        RectTransform rectTransform = uiButton.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + horizontalOffset, verticalOffset + padding);
        if (IsVrRun)
        {
            BoxCollider collider = gadgetButton.AddComponent<BoxCollider>();
            collider.size = uiButton.GetComponent<RectTransform>().rect.size;
        }
        string buttonIdentifier = buttonName;
        uiButton.GetComponentInChildren<UnityEngine.UI.Text>().text = buttonIdentifier;
        if (string.Equals("Reset", buttonName))
        {
            uiButton.onClick.AddListener(() => gameManager.ResetGadgetsInWorld());

        }
        if (string.Equals("Save", buttonName))
        {
            uiButton.onClick.AddListener(() => gameManager.GameManagerSaves());

        }
        if (string.Equals("Insert Gadget", buttonName))
        {
            this.Deactivate();

            uiButton.onClick.AddListener(() =>this.GoToSelectorMenu());
        }
        if (string.Equals("Close", buttonName))
        {
            this.Deactivate();

            uiButton.onClick.AddListener(() => this.Deactivate());
        }
        if (string.Equals("Load", buttonName))
        {
            uiButton.onClick.AddListener(() => this.GoTOLoadMenu()); //gameManager.GameManagerLoad());

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
        this.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    public void GoToSelectorMenu()
    {
        this.Deactivate();
        gameManager.EnableSelectMode();
    }

    public void GoTOLoadMenu()
    {

        this.Deactivate();
        gameManager.goToLoadMenu();

    }
}
