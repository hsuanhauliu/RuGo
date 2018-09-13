using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGenerator : MonoBehaviour {
    // https://forum.unity.com/threads/how-to-create-ui-button-dynamically.393160/
    // Use this for initialization

    /*
     * will have to add classes of gadgets as 2nd data type 
     * 
     * was trying to add gadget class with name so can look up and instantiate directly 
     * 
     * private IDictionary<string, GameObject> dict = new Dictionary<string, GameObject>()
                                            {
        {"dominos",DominoGadget},
        {"marble", DominoGadget},
        {"spinner",DominoGadget}
                                            };
                                            */

    // AllGadgets has all availabel gadgets that can be added to the world 
    //  MakeMenu MAKES menu dynamically from AllGadgets


    // Important note rectTransform can be MainMenu.GetComponent<Canvas>().enabled = false;
    // but when working with game objects they have to be SetActive 
    public Canvas MainMenu;

    //public RectTransform MenuPanel;

    public GameObject MenuPanel;

    public GameObject RollPanel;

    public GameObject MovePanel;

    public GameObject RampPanel;

    public GameObject RecyclablesPanel;



    public GameObject prefabButton;


    bool menuStatus = false;

    List<string> AllCategorys = new List<string>(new string[] { "Roll","Move","Ramp","Recyclable"});

    //List<string> AllGadgets = new List<string>(new string[] { "Dominoe", "Sphere", "Spinner" , "chopSick", "Track" });

    List<string> Roll = new List<string>(new string[] { "Marbles", "Skateboard", "Dominoe", "Baseball" });

    List<string> Move = new List<string>(new string[] { "Dominoe", "Mousetrap", "Toaster", "Fan"});

    List<string> Ramp = new List<string>(new string[] { "Books", "Trays", "Gutters" });

    List<string> Recyclables = new List<string>(new string[] { "Cardboard", "Cans" });




    void Start () {
        MakeMenu(AllCategorys, MenuPanel,false);
        MenuPanel.SetActive(false);
        MakeMenu(Roll, RollPanel, true);
        RollPanel.SetActive(false);
        MakeMenu(Move, MovePanel, true);
        MovePanel.SetActive(false);
        MakeMenu(Ramp, RampPanel, true);
        RampPanel.SetActive(false);
        MakeMenu(Recyclables, RecyclablesPanel, true);
        RecyclablesPanel.SetActive(false);




    }
  
    void Update () {

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("M was pressed");
            if (MenuPanel.activeInHierarchy)
            {
                MenuPanel.SetActive(false);
                Debug.Log("Making menu disapper");
            }

            else
            {

                MenuPanel.SetActive(true);
                Debug.Log("Making menu reapper");

            }

        }


    }
    void ButtonClicked(string buttonName)
    {
        Debug.Log("Button clicked = " + buttonName);
        MainMenu.GetComponent<Canvas>().enabled = false;
        menuStatus = false;
        if(buttonName.Equals("Roll"))
        {
            MenuPanel.SetActive(false);
            RollPanel.SetActive(true);
        }
        if (buttonName.Equals("Move"))
        {
            MenuPanel.SetActive(false);
            MovePanel.SetActive(true);
        }
        if (buttonName.Equals("Ramp"))
        {
            MenuPanel.SetActive(false);
            RampPanel.SetActive(true);
        }
        if (buttonName.Equals("Recyclable"))
        {
            MenuPanel.SetActive(false);
            RecyclablesPanel.SetActive(true);
        }
        MainMenu.GetComponent<Canvas>().enabled = true;
        menuStatus = true;

    }


    void MakeMenu (List<string> items, GameObject workon, bool back) {
        float initialPositionX = -4f;
        float initialPositionY = 4f;

        if (back)
        {
            Debug.Log("Back Button was created ");
            float xBack = -6f;
            float yBack = -4f;
            string backButton = "Back";
            // just for back button 

            GameObject goButton = (GameObject)Instantiate(prefabButton);
            goButton.transform.SetParent(workon.transform, false);

            //goButton.transform.localScale = new Vector3(1.7f, 0.5f, 1);
            goButton.transform.position = new Vector3(initialPositionX + 2f, initialPositionY, 1f);
            initialPositionX = initialPositionX + 2f;

            var tempButton = goButton.GetComponent<UnityEngine.UI.Button>();

            tempButton.onClick.AddListener(GoBack);

            tempButton.GetComponentInChildren<UnityEngine.UI.Text>().text = backButton;

            // till here 
        }





        for (int i = 0; i < items.Count; i++)
        {
            if (i%3 == 0)
            {
                initialPositionY = initialPositionY - 1f;
                initialPositionX = -4f;
            }

            string buttonName = items[i];

            GameObject goButton = (GameObject)Instantiate(prefabButton);
            goButton.transform.SetParent(workon.transform, false);
            //goButton.transform.localScale = new Vector3(1.7f, 0.5f, 1);
            goButton.transform.position = new Vector3(initialPositionX + 2f, initialPositionY, 1f);
            initialPositionX = initialPositionX + 2f;

            var tempButton = goButton.GetComponent<UnityEngine.UI.Button>();
            int tempInt = i;
            
            tempButton.onClick.AddListener(() => ButtonClicked(buttonName));

            tempButton.GetComponentInChildren<UnityEngine.UI.Text>().text = buttonName;


            Debug.Log("Button added at" + goButton.transform);


        }
  



    }
    void GoBack()
    {
        RollPanel.SetActive(false);
        MovePanel.SetActive(false);
        RampPanel.SetActive(false);
        RecyclablesPanel.SetActive(false);
        MenuPanel.SetActive(true);



    }
}
