using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGenerator : MonoBehaviour {
    // https://forum.unity.com/threads/how-to-create-ui-button-dynamically.393160/
    // Use this for initialization

    /*
     * was trying to add gadget class with name so can look up and instantiate directly 
     * private IDictionary<string, GameObject> dict = new Dictionary<string, GameObject>()
                                            {
        {"dominos",DominoGadget},
        {"marble", DominoGadget},
        {"spinner",DominoGadget}
                                            };
                                            */

    // AllGadgets has all availabel gadgets that can be added to the world 
    //  MakeMenu MAKES menu dynamically from AllGadgets

    public Canvas MainMenu;

    bool menuStatus = false;

    List<string> AllGadgets = new List<string>(new string[] { "Dominoe", "Sphere", "Spinner" , "chopSick", "Track" });


    void Start () {
        MakeMenu();
        MainMenu.GetComponent<Canvas>().enabled = false;


    }
    public RectTransform MenuPanel;

    public GameObject prefabButton;

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("M was pressed");
            if (menuStatus)
            {
                MainMenu.GetComponent<Canvas>().enabled = false;
                menuStatus = false;
                Debug.Log("Making menu disapper");

            }

            else
            {
                MainMenu.GetComponent<Canvas>().enabled = true;
                menuStatus = true;
                Debug.Log("Making menu reapper");

            }

        }


    }
    void ButtonClicked(int buttonNo)
    {
        Debug.Log("Button clicked = " + buttonNo);
    }


    void MakeMenu () {
        float initialPositionX = -4f;
        float initialPositionY = 4f;



        for (int i = 0; i < AllGadgets.Count; i++)
        {
            if (i%3 == 0)
            {
                initialPositionY = initialPositionY - 1f;
                initialPositionX = -4f;
            }

            string buttonName = AllGadgets[i];

            GameObject goButton = (GameObject)Instantiate(prefabButton);
            goButton.transform.SetParent(MenuPanel, false);
            goButton.transform.localScale = new Vector3(1.7f, 0.5f, 1);
            goButton.transform.position = new Vector3(initialPositionX + 2f, initialPositionY, 1f);
            initialPositionX = initialPositionX + 2f;

            var tempButton = goButton.GetComponent<UnityEngine.UI.Button>();
            int tempInt = i;
            tempButton.onClick.AddListener(() => ButtonClicked(tempInt));

            tempButton.GetComponentInChildren<UnityEngine.UI.Text>().text = buttonName;


            Debug.Log("Button added at" + goButton.transform);
        }


    }
}
