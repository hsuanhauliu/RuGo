using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ButtonClick : MonoBehaviour {

    public Text displayBanner;

   

    public void button_onClick()
    {
        IDictionary<string, string> dict = new Dictionary<string, string>();

        dict.Add("Roll1", "Toy Cars");
        dict.Add("Roll2", "Marbles");
        dict.Add("Roll3", "Dominoes");

        dict.Add("Ramp1", "Toy Train Tracks");
        dict.Add("Ramp2", "Marble Runs");
        dict.Add("Ramp3", "Books");

        dict.Add("Hm1", "Chopsticks");
        dict.Add("Hm2", "Bowl");
        dict.Add("Hm3", "Balloons");

        string Button_name = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log("Button clicked was"+Button_name+" dictonary value was "+dict[Button_name]);

        displayBanner.text = "You clicked "+ dict[Button_name];
    }
}
