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

    private GameObject mGadgetShelf;

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
            SpawnGadgets();
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
        SpawnInvisibleShelf();
        SpawnBubbles();
        SpawnGadgets();

        ShowShelf(false);
    }

    public void ShowShelf(bool show)
    {
        if(!show)
        {
            StartCoroutine("DelayHideShelf");
        }
        else
        {
            mGadgetShelf.SetActive(show);
        }
        

        if (show)
        {
            Transform camera = GameManager.Instance.MainCamera.transform;
            Vector3 cameraXZPosition = new Vector3(camera.position.x, 0.0f, camera.position.z);
            Vector3 cameraForward = new Vector3(camera.forward.x, 0.0f, camera.forward.z);
            mGadgetShelf.transform.position = cameraXZPosition + cameraForward;


            mGadgetShelf.transform.LookAt(cameraXZPosition, Vector3.up);

            mGadgetShelf.transform.position = mGadgetShelf.transform.position + new Vector3(0.0f, camera.position.y, 0.0f);
        }
    }

    private IEnumerator DelayHideShelf()
    {
        yield return new WaitForSeconds(0.3f);

        mGadgetShelf.SetActive(false);
    }

    private void SpawnInvisibleShelf()
    {
        string shelf = "GadgetShelf";

        GameObject gadgetResource = Resources.Load(shelf) as GameObject;
        mGadgetShelf = Instantiate(gadgetResource, this.transform);
        mGadgetShelf.GetComponent<Gadget>().SetLayer(GadgetLayers.SHELF);
        mGadgetShelf.transform.position = Vector3.zero;
    }

    private void SpawnBubbles()
    {
        string bubbleEffectName = "Bubble";

        for (int i = 0; i < (int)GadgetInventory.NUM; i++)
        {
            Transform placeHolder = mGadgetShelf.transform.GetChild(i);

            GameObject gadgetResource = Resources.Load(bubbleEffectName) as GameObject;
            GameObject gadgetObj = Instantiate(gadgetResource, placeHolder.transform);
            gadgetObj.transform.localPosition = Vector3.zero;
        }
    }

    private void SpawnGadgets()
    {
        for (int i = 0; i < (int)GadgetInventory.NUM; i++)
        {
            Transform placeHolder = mGadgetShelf.transform.GetChild(i);

            if (placeHolder.childCount < 2) // This is set to 2 because the placeholder has a bubble now
            {
                GadgetInventory nextGadget = (GadgetInventory)i;
                string gadgetName = nextGadget.ToString();

                GameObject gadgetResource = Resources.Load(gadgetName) as GameObject;
                GameObject gadgetObj = Instantiate(gadgetResource, placeHolder.transform);
                gadgetObj.transform.localPosition = Vector3.zero;
                gadgetObj.name = gadgetName + " (OnShelf)";
                Gadget gadget = gadgetObj.GetComponent<Gadget>();
                gadget.MakeTransparent(true);
                gadget.SetLayer(GadgetLayers.SHELF);
            }
        }
    }

    public void InsertGadget(Gadget gadget)
    {
        gadget.gameObject.name = gadget.GetType().ToString() + gadgetsInWorld.Count.ToString();
        gadgetsInWorld.Add(gadget);
        gadget.SetLayer(GadgetLayers.INWORLD);
        MarkWorldModified();
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