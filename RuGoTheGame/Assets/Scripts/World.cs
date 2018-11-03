using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class World : MonoBehaviour
{
    private List<Gadget> gadgetsInWorld;
    private bool isWorldStateModified = false;
    private string WorldName;
    private readonly string AUTO_SAVE_FILE = "autosave.dat";
    private readonly string SAVED_GAME_DIR = "SavedGames/";
    private GameObject tableObj;

    public VRTK.VRTK_ControllerEvents RightControllerEvents;

    public static World Instance = null;
    private void MakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        gadgetsInWorld = new List<Gadget>();
        InsertInitialGadgets();
        CreateDirectory(SAVED_GAME_DIR);
        InitializeNewWorld();   //TODO load the first world instead

        RightControllerEvents.SubscribeToButtonAliasEvent(VRTK.VRTK_ControllerEvents.ButtonAlias.TriggerPress, true, RightControllerEvents_TriggerClicked);
    }

    void RightControllerEvents_TriggerClicked(object sender, VRTK.ControllerInteractionEventArgs e)
    {
        ToggleTable();
    }


    private void Awake()
    {
        MakeSingleton();
    }

    void Update()
    {
        if (isWorldStateModified)
        {
            AutoSave();
            SpawnGadgetsOnTable();
            isWorldStateModified = false;
        }
    }

    public void CreateNewWorld()
    {
        Clear();
        InitializeNewWorld();
        Save();
    }

    public void InitializeNewWorld()
    {
        string[] timeStamp = System.DateTime.UtcNow.ToString().Replace(":", " ").Replace("/", " ").Split(' ');
        WorldName = string.Join(string.Empty, timeStamp);
    }

    public void Save()
    {
        CreateDirectory(SAVED_GAME_DIR + WorldName);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(SAVED_GAME_DIR + WorldName + "/" + WorldName + ".dat");

        List<GadgetSaveData> saveData = gadgetsInWorld.ConvertAll<GadgetSaveData>((Gadget input) => input.GetSaveData());
        bf.Serialize(file, saveData);
        file.Close();

        AutoSave();
    }

    private void AutoSave()
    {
        string fileName = SAVED_GAME_DIR + WorldName + "/" + WorldName + ".dat";

        if (File.Exists(fileName))
        {
            CreateDirectory(SAVED_GAME_DIR + WorldName);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(SAVED_GAME_DIR + WorldName + "/" + AUTO_SAVE_FILE);

            List<GadgetSaveData> saveData = gadgetsInWorld.ConvertAll<GadgetSaveData>((Gadget input) => input.GetSaveData());
            bf.Serialize(file, saveData);
            file.Close();
        }
        else
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(SAVED_GAME_DIR + "/" + AUTO_SAVE_FILE);

            List<GadgetSaveData> saveData = gadgetsInWorld.ConvertAll<GadgetSaveData>((Gadget input) => input.GetSaveData());
            bf.Serialize(file, saveData);
            file.Close();
        }
    }

    public void LoadWorld(string savedWorldName)
    {
        WorldName = savedWorldName;
        string fileName = SAVED_GAME_DIR + savedWorldName + "/" + savedWorldName + ".dat";
        Load(fileName);
        AutoSave();
    }

    public void LoadAuto()
    {
        string fileName = SAVED_GAME_DIR + WorldName + "/" + WorldName + ".dat";

        if (File.Exists(fileName))
        {
            string worldAutoSaveFile = SAVED_GAME_DIR + WorldName + "/" + AUTO_SAVE_FILE;
            Load(worldAutoSaveFile);
        }
        else if (gadgetsInWorld.Count != 0)
        {
            string tempAutoSaveFile = SAVED_GAME_DIR + "/" + AUTO_SAVE_FILE;
            Load(tempAutoSaveFile);
        }
    }

    public void Load(string fileName)
    {
        if (File.Exists(fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(fileName, FileMode.Open);

            List<GadgetSaveData> savedGadgets = (List<GadgetSaveData>)bf.Deserialize(file);
            Clear();
            gadgetsInWorld = savedGadgets.ConvertAll<Gadget>(ConvertSavedDataToGadget);

            file.Close();
        }
        else
        {
            Debug.Log("Loading Data failed. File " + fileName + "doesn't exist");
        }
    }

    private Gadget ConvertSavedDataToGadget(GadgetSaveData savedGadgetData)
    {
        string prefabName = savedGadgetData.name;
        GameObject gadgetPrefab = Resources.Load(prefabName) as GameObject;
        GameObject savedGameObject = Instantiate(gadgetPrefab, this.transform);

        Gadget gadget = savedGameObject.GetComponent<Gadget>();
        gadget.RestoreStateFromSaveData(savedGadgetData);
        gadget.transform.position += this.transform.position;

        return gadget;
    }

    public void Clear()
    {
        foreach (Gadget gadget in gadgetsInWorld)
        {
            gadget.RemoveFromScene();
        }
        gadgetsInWorld = new List<Gadget>();
    }

    private void InsertInitialGadgets()
    {
        string table = "MetalTable";

        GameObject gadgetResource = Resources.Load(table) as GameObject;
        tableObj = Instantiate(gadgetResource, this.transform);
        tableObj.GetComponent<Gadget>().SetLayer("TableGadget");
        tableObj.transform.position = Vector3.zero;
        SpawnGadgetsOnTable();

        ToggleTable();
    }

    public void ToggleTable()
    {
        bool enable = !tableObj.activeSelf;

        StartCoroutine("SetTableObjActive", enable);
        
        if(enable)
        {
            Transform camera = GameObject.FindWithTag("MainCamera").transform;
            Vector3 cameraXZPosition = new Vector3(camera.position.x, 0.0f, camera.position.z);
            Vector3 cameraForward = new Vector3(camera.forward.x, 0.0f, camera.forward.z);
            tableObj.transform.position = cameraXZPosition + cameraForward;


            tableObj.transform.LookAt(cameraXZPosition, Vector3.up);

            tableObj.transform.position = tableObj.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        }
    }

    private IEnumerator SetTableObjActive(bool enable)
    {
        yield return new WaitForSeconds(0.5f);
        tableObj.SetActive(enable);

        yield return null;
    }


    private void SpawnGadgetsOnTable()
    {
        for (int i = 1; i < (int)GadgetInventory.NUM; i++)
        {
            Transform placeHolder = tableObj.transform.GetChild(0).GetChild(i - 1);
            if (placeHolder.childCount == 0)
            {
                GadgetInventory nextGadget = (GadgetInventory)i;
                string gadgetName = nextGadget.ToString();

                GameObject gadgetResource = Resources.Load(gadgetName) as GameObject;
                GameObject gadgetObj = Instantiate(gadgetResource, placeHolder.transform);
                gadgetObj.transform.localPosition = Vector3.zero;
                Gadget gadget = gadgetObj.GetComponent<Gadget>();
                gadget.SetLayer("TableGadget");
                gadget.MakeTransparent(true);
            }
        }
    }

    public void InsertGadget(Gadget gadget)
    {
        gadgetsInWorld.Add(gadget);
        gadget.gameObject.transform.SetParent(transform);
        MarkWorldModified();
    }

    public void CreateGadgetFromTemplate(Gadget gadgetTemplate)
    {
        GameObject gadgetObj = Instantiate(gadgetTemplate.gameObject, this.transform);
        gadgetObj.transform.position -= this.transform.position;
        Gadget gadget = gadgetObj.GetComponent<Gadget>();
        gadget.MakeSolid();
        InsertGadget(gadget);
    }

    public void RemoveGadget(Gadget gadget)
    {
        gadgetsInWorld.Remove(gadget);
        gadget.RemoveFromScene();
        MarkWorldModified();
    }

    public void MarkWorldModified()
    {
        isWorldStateModified = true;
    }

    private void CreateDirectory(string directoryName)
    {
        System.IO.Directory.CreateDirectory(directoryName);
    }
}