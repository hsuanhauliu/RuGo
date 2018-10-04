using UnityEngine;

public class GadgetSelectorMenu : MonoBehaviour
{
    public GameManager gameManager;

    public float padding = 20f;
    public bool IsVrRun = false;

    void Start()
    {
        BuildButtonPanel();
    }

    void Update()
    {

    }

    public void BuildButtonPanel() {
        GameObject gadgetPrefab = Resources.Load("BasicButton") as GameObject;

        //TODO Refactor this
        for (int i = 0; i < System.Enum.GetValues(typeof(GadgetInventory)).Length; i++) {
            GadgetInventory gadgetItem = (GadgetInventory)i;
            BuildButton(gadgetPrefab, gadgetItem, ((1+i) * -150));
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
        this.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
}
