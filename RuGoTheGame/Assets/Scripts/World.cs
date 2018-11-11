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
    private Vector3[] shelfContainersPositions;

    public GameObject BubblePrefab;
    public static World Instance = null;

    /* Shelf control variables */
    public float BubbleStartPosition = -0.3f;
    public float BubbleEndPosition = 0.3f;
    public float GapBetweenGadgets = 0.3f;
    public float ShiftRateMin = 1.0f;
    public float ShiftRateMax = 1.2f;
    public float GadgetOffsetMax = 0.2f;


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
        SpawnGadgetShelf();
        ShowShelf(false);
        CreateDirectory(SAVED_GAME_DIR);
        InitializeNewWorld();   //TODO load the first world instead
    }

    private void Awake()
    {
        MakeSingleton();
        mGadgetShelf = transform.Find("GadgetShelf").gameObject;
    }

    void Update()
    {
        if (isWorldStateModified)
        {
            AutoSave();
            RespawnGadgets();
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
        //string fileName = SAVED_GAME_DIR + savedWorldName + "/" + savedWorldName + ".dat";
        string fileName = SAVED_GAME_DIR + savedWorldName + "/" + "autosave" + ".dat";

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

    public void ShowShelf(bool show)
    {
        if (!show)
        {
            StartCoroutine("DelayHideShelf");
        }
        else
        {
            mGadgetShelf.SetActive(show);
            AudioSource bubbleAudio = mGadgetShelf.GetComponent<AudioSource>();
            bubbleAudio.Play();
        }


        if (show)
        {
            Transform camera = GameManager.Instance.MainCamera.transform;
            Vector3 cameraXZPosition = new Vector3(camera.position.x, 0.0f, camera.position.z);
            Vector3 cameraForward = new Vector3(camera.forward.x, 0.0f, camera.forward.z);
            mGadgetShelf.transform.position = cameraXZPosition + cameraForward;


            mGadgetShelf.transform.LookAt(cameraXZPosition, Vector3.up);
            mGadgetShelf.transform.position = mGadgetShelf.transform.position + new Vector3(0.0f, camera.position.y, 0.0f);

            // the -1 in below for loop prevents from spawning first file load file in middle of menu 
            for (int i = 0; i < shelfContainersPositions.Length-1; i++)
            {
                StartCoroutine(ShiftGadgets(i));
            }
        }
    }

    private IEnumerator DelayHideShelf()
    {
        yield return new WaitForSeconds(0.3f);

        mGadgetShelf.SetActive(false);
    }

    private IEnumerator ShiftGadgets(int i)
    {
        float startTime = Time.time;
        float fraction = 0;
        float random_x = UnityEngine.Random.Range(0, GadgetOffsetMax);
        float random_y = UnityEngine.Random.Range(0, GadgetOffsetMax);
        Vector3 startingPosition = new Vector3(random_x, random_y, 0);
        float rate = UnityEngine.Random.Range(ShiftRateMin, ShiftRateMax);

        while (fraction * rate <= 1)
        {
            fraction = Time.time - startTime;
            mGadgetShelf.transform.GetChild(i).localPosition = Vector3.Lerp(startingPosition, shelfContainersPositions[i], fraction * rate);
            mGadgetShelf.transform.GetChild(i).localScale = Vector3.Lerp(Vector3.zero, Vector3.one, fraction * rate);
            yield return null;
        }
    }

    private void SpawnGadgetShelf()
    {
        float pos_x = BubbleStartPosition;
        float pos_y = BubbleStartPosition;
        shelfContainersPositions = new Vector3[(int)GadgetInventory.NUM];

        for (int i = 0; i < (int)GadgetInventory.NUM; i++)
        {
            // Create container and store their position
            GameObject container = new GameObject("Container " + i.ToString());
            container.transform.SetParent(mGadgetShelf.transform);
            Vector3 container_pos = new Vector3(pos_x, pos_y, 0);
            container.transform.localPosition = container_pos;
            shelfContainersPositions[i] = container_pos;

            // Create bubble
            GameObject bubbleObj = Instantiate(BubblePrefab, container.transform);

            // Create gadget
            string gadgetName = ((GadgetInventory)i).ToString();
            Debug.Log(gadgetName);
            SpawnSingleGadget(gadgetName, container.transform);

            // Update next position
            if (pos_x == BubbleEndPosition)
            {
                pos_y += GapBetweenGadgets;
                pos_x = BubbleStartPosition;
            }
            else
            {
                pos_x += GapBetweenGadgets;
            }
        }

        // spawn small bubbles for saved files
        pos_x = BubbleStartPosition - GapBetweenGadgets;
        pos_y = BubbleStartPosition - GapBetweenGadgets;

        for (int i = 0; i < 5; i++)
        {

            GameObject container = new GameObject("File Container " + i.ToString());
            container.transform.SetParent(mGadgetShelf.transform);
            Vector3 container_pos = new Vector3(pos_x, pos_y, 0);
            container.transform.localPosition = container_pos;

            // load file bubble
            GameObject bubbleObj = Instantiate(BubblePrefab, container.transform);
            bubbleObj.transform.localScale *= 0.5f;

            // Create gadget
            string gadgetName = "LoadCube" ;

            LoadCube spawnedLoadCube = SpawnSingleGadget(gadgetName, container.transform) as LoadCube;
            spawnedLoadCube.Slot = i.ToString();

            pos_x += GapBetweenGadgets;
        }

    }

    private void RespawnGadgets()
    {
        Debug.Log("Number of childern are "+mGadgetShelf.transform.childCount);
        int StartFromHereForFile = 0;
        for (int i = 0; i < (int)GadgetInventory.NUM; i++)
        {
            Transform placeHolder = mGadgetShelf.transform.GetChild(i);
            if (placeHolder.childCount < 2) // This is set to 2 because the placeholder has a bubble now
            {
                string gadgetName = ((GadgetInventory)i).ToString();
                SpawnSingleGadget(gadgetName, placeHolder);
                return;
            }
            StartFromHereForFile += 1;
        }
    }

    public void RespawnFiles()
    {
        int countOFchildern = mGadgetShelf.transform.childCount;
        for (int i = (int)GadgetInventory.NUM; i < countOFchildern; i++)
        {
            Transform placeHolder = mGadgetShelf.transform.GetChild(i);
            if(placeHolder.childCount < 2)
            {
                int slot = i - (int)GadgetInventory.NUM;
                string gadgetName = "LoadCube" ;
                LoadCube spawnedLoadCube = SpawnSingleGadget(gadgetName, placeHolder) as LoadCube;
                spawnedLoadCube.Slot = slot.ToString();
            }
        }
    }

    private Gadget SpawnSingleGadget(string gadgetName, Transform parentTransform)
    {
        GameObject gadgetResource = Resources.Load(gadgetName) as GameObject;
        GameObject gadgetObj = Instantiate(gadgetResource, parentTransform);
        gadgetObj.transform.localPosition = Vector3.zero;
        gadgetObj.name = gadgetName + " (OnShelf)";
        Gadget gadget = gadgetObj.GetComponent<Gadget>();
        Debug.Log(gadget);
        gadget.MakeTransparent(true);
        gadget.SetLayer(GadgetLayers.SHELF);

        return gadget;
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

    /***************************************** HELPERS ******************************************/

    private void CreateDirectory(string directoryName)
    {
        System.IO.Directory.CreateDirectory(directoryName);
    }
}