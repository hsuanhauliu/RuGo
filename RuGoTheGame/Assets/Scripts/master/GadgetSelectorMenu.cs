using UnityEngine;

public class GadgetSelectorMenu : MonoBehaviour
{
    public GameManager gameManager;
    public MainMenu mainMenu;
    public LoadMenu loadMenu;
    public float padding = 20f;
    public bool IsVrRun = false;

    void Start()
    {
        BuildButtonPanel();
        BuildToolBar();
        Debug.Log("called now ");
    }

    void Update()
    {

    }

    public void ReparentSelectorMenu()
    {
        // Parent the gadget selector menu underneath the main camera
        GameObject menuParent = GameObject.FindGameObjectWithTag("MainCamera");
        transform.SetParent(menuParent.transform);
        if(IsVrRun)
        {
            transform.localPosition = new Vector3(0, 0, 1);
        }
        else
        {
            transform.localPosition = new Vector3(0.2f,0.02f,0.7f);
        }
        //transform.localPosition = new Vector3(0, 0, 1);
        transform.localRotation = Quaternion.identity;
    }

    public void BuildButtonPanel() {

        GameObject gadgetPrefab = Resources.Load("BasicButton") as GameObject;

        //TODO Refactor this
        for (int i = 0; i < (int)GadgetInventory.NUM; i++) {
            GadgetInventory gadgetItem = (GadgetInventory)i;
            BuildButton(gadgetPrefab, gadgetItem, ((1+i) * -150));
        }
    }

    public void BuildToolBar() {
        
        float verticalOffset = 750f;
        float horizontalOffset = 0f ;
        AddButtonToToolBar("Reset",horizontalOffset, verticalOffset);
        //AddButtonToToolBar("Load", horizontalOffset -330f , verticalOffset);
        AddButtonToToolBar("Save", horizontalOffset + 330f, verticalOffset);
        AddButtonToToolBar("Clear", horizontalOffset + 330f, verticalOffset +250f);
        AddButtonToToolBar("Back", horizontalOffset - 330f, verticalOffset );




    }

    public void AddButtonToToolBar(string buttonName, float horizontalOffset, float verticalOffset){

        GameObject SmallButton = Resources.Load("smallButton") as GameObject;
        GameObject gadgetButton = (GameObject)Instantiate(SmallButton, this.transform);
        UnityEngine.UI.Button uiButton = gadgetButton.GetComponent<UnityEngine.UI.Button>();

        
        RectTransform rectTransform = uiButton.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + horizontalOffset , verticalOffset + padding);
        if (IsVrRun)
        {
            BoxCollider collider = gadgetButton.AddComponent<BoxCollider>();
            collider.size = uiButton.GetComponent<RectTransform>().rect.size;
            collider.center = new Vector3(0, -collider.size.y / 2, 0);

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
        if (string.Equals("Load", buttonName))
        {
            uiButton.onClick.AddListener(() => gameManager.GameManagerLoad());

        }
        if (string.Equals("Clear", buttonName))
        {
            uiButton.onClick.AddListener(() => gameManager.GameManagerClearWorld());

        }
        if (string.Equals("Back", buttonName))
        {
            this.Deactivate();

            uiButton.onClick.AddListener(() => this.GoToMainMenu()); //this.Deactivate());
        }

    
    }


    public void BuildButton(GameObject buttonPrefab, GadgetInventory gadgetItem, float verticalOffset) {
        //TODO Add Button to Panel transform instead of Entire Menu
        GameObject gadgetButton = (GameObject)Instantiate(buttonPrefab, this.transform);

        UnityEngine.UI.Button uiButton = gadgetButton.GetComponent<UnityEngine.UI.Button>();
        if(IsVrRun)
        {
            BoxCollider collider = gadgetButton.AddComponent<BoxCollider>();
            collider.size = uiButton.GetComponent<RectTransform>().rect.size;
            collider.center = new Vector3(0, -collider.size.y / 2, 0);

        }

        RectTransform rectTransform = uiButton.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, verticalOffset + padding);
      
        string buttonIdentifier = gadgetItem.ToString();
        uiButton.GetComponentInChildren<UnityEngine.UI.Text>().text = buttonIdentifier;

        //TODO Find a better way to handle the Path Tool
        if (gadgetItem == GadgetInventory.PathTool) {
            uiButton.onClick.AddListener(SelectDrawTool);
        }
        else {
            uiButton.onClick.AddListener(() => SelectGadget(buttonIdentifier));
        }
    }

    public void SelectDrawTool() {
        gameManager.EnableDrawMode();
    }

    public void SelectGadget(string selectedGagdget) {
        gameManager.CreateGadget(selectedGagdget);
    }

    public void Activate()
    {
        mainMenu.Deactivate();
        loadMenu.Deactivate();
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
