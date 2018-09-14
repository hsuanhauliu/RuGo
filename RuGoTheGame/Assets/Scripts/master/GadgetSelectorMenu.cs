using UnityEngine;

public class GadgetSelectorMenu : MonoBehaviour
{
    public GameManager gameManager;

    void Start()
    {
        GameObject gadgetPrefab = Resources.Load("BasicButton") as GameObject;
        //TODO Add Button to Panel transform instead of Entire Menu
        GameObject gadgetButton = (GameObject)Instantiate(gadgetPrefab, this.transform);

        UnityEngine.UI.Button uiButton = gadgetButton.GetComponent<UnityEngine.UI.Button>();
        string buttonIdentifier = GadgetInventory.Ball.ToString();
        uiButton.GetComponentInChildren<UnityEngine.UI.Text>().text = buttonIdentifier;
        uiButton.onClick.AddListener(() => SelectGadget(buttonIdentifier));
    }

    void Update()
    {

    }

    public void SelectGadget(string selectedGagdget) {
        gameManager.CreateGadget(selectedGagdget);
        gameManager.EnableBuildMode();
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
