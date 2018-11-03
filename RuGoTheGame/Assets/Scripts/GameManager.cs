using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public PathTool PathTool;

    // #TEMP HACK: How to do this for VRTK or does this go away?
    public GameObject MainCamera;

    private enum GameMode { Build, Select, Draw };
    private GameMode currentGameMode;

    private bool BuildModeEnabled
    {
        get
        {
            return currentGameMode == GameMode.Build;
        }
    }

    void Start()
    {
        EnableBuildMode();
    }

    /************************** Public Functions **************************/

    public void EnableDrawMode()
    {
        PathTool.Activate(CreateGadgetAlongPath);
        this.currentGameMode = GameMode.Draw;
    }

    public void EnableBuildMode()
    {
        this.currentGameMode = GameMode.Build;
    }

    public void EnableSelectMode()
    {
        PathTool.Deactivate();
        this.currentGameMode = GameMode.Select;
    }

    // TODO make it extendable to use for gadgets other than just the Domino
    public void CreateGadgetAlongPath(Vector3[] path)
    {
        string gadgetName = "Domino";
        GameObject gadgetPreb = Resources.Load(gadgetName) as GameObject;
        int numOfPoints = path.Length;

        if (numOfPoints > 1)
        {
            for (int p = 0; p < numOfPoints - 1; p++)
            {
                GameObject gadgetGameObject = Instantiate(gadgetPreb, this.transform);
                Gadget gadget = gadgetGameObject.GetComponent<Gadget>();

                Vector3 direction = path[p + 1] - path[p];
                gadget.transform.position = path[p];
                gadget.transform.rotation = Quaternion.LookRotation(direction);
                gadget.Deselect();
                World.Instance.InsertGadget(gadget);
            }
            GameObject lastGadgetGameObject = Instantiate(gadgetPreb, this.transform);
            Gadget lastGadget = lastGadgetGameObject.GetComponent<Gadget>();

            Vector3 lastPathDirection = path[numOfPoints - 1] - path[path.Length - 2];
            lastGadget.transform.position = path[numOfPoints - 1];
            lastGadget.transform.rotation = Quaternion.LookRotation(lastPathDirection);
            lastGadget.Deselect();
            World.Instance.InsertGadget(lastGadget);
        }
        else if (numOfPoints == 1)
        {
            GameObject gadgetGameObject = Instantiate(gadgetPreb, this.transform);
            Gadget gadget = gadgetGameObject.GetComponent<Gadget>();

            gadget.transform.position = path[0];
            gadget.Deselect();
            World.Instance.InsertGadget(gadget);
        }
        EnableBuildMode();
    }
}
