﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class World : MonoBehaviour
{
    private List<Gadget> gadgetsInWorld;
    private bool isWorldStateModified = false;

    private string CurrentSaveSlot;

    private readonly string AUTO_SAVE_FILE = "autosave.dat";
    private readonly string SAVED_GAME_DIR = "SavedGames/";
    private GameObject mGadgetShelf;
    private Vector3[] shelfContainersPositions;

    public GameObject BubblePrefab;
    public static World Instance = null;

    /* Shelf control variables */
    public float shelfRadius = 1.0f;
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
        mGadgetShelf.transform.position = new Vector3(100, 100, 100);
        ShowShelf(false);
        LoadLastModifiedSaveSlot();
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

    public void LoadLastModifiedSaveSlot()
    {
        //TODO: Determine Last Modified Save Slot #
        LoadSaveSlot("0");
    }

    private void AutoSave()
    {
        string currentSaveSlotFile = SAVED_GAME_DIR + CurrentSaveSlot + "/" + AUTO_SAVE_FILE ;

        if (File.Exists(currentSaveSlotFile))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(SAVED_GAME_DIR + CurrentSaveSlot + "/" + AUTO_SAVE_FILE);

            List<GadgetSaveData> saveData = gadgetsInWorld.ConvertAll<GadgetSaveData>((Gadget input) => input.GetSaveData());
            bf.Serialize(file, saveData);
            file.Close();
        }
        else
        {
            Debug.Log("Loading AutoSave Failed. File " + currentSaveSlotFile + "doesn't exist");
        }

    }

    public void LoadCurrentSaveSlot()
    {
        LoadSaveSlot(CurrentSaveSlot);
    }

    public void LoadSaveSlot(string saveSlot)
    {
        CurrentSaveSlot = saveSlot;

        string serializedAutoSaveFile = SAVED_GAME_DIR + CurrentSaveSlot + "/" + AUTO_SAVE_FILE;

        LoadSerializedGadgets(serializedAutoSaveFile);
        AutoSave();
    }

    private void LoadSerializedGadgets(string serializedFileName)
    {
        if (File.Exists(serializedFileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(serializedFileName, FileMode.Open);
            Clear();
            if (fileStream.Length != 0) 
            {
                List<GadgetSaveData> savedGadgets = (List<GadgetSaveData>)bf.Deserialize(fileStream);
                gadgetsInWorld = savedGadgets.ConvertAll<Gadget>(ConvertSavedDataToGadget);

            }
            fileStream.Close();
        }
        else
        {
            Debug.Log("Loading Data failed. File " + serializedFileName + "doesn't exist");
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
            StartCoroutine(DelayHideShelf());
        }
        else
        {
            mGadgetShelf.SetActive(show);
            AudioSource bubbleAudio = mGadgetShelf.GetComponent<AudioSource>();
            bubbleAudio.Play();
        }


        if (show)
        {
            Transform myCamera = GameManager.Instance.MainCamera.transform;
            Vector3 pureCameraRotation = myCamera.rotation.eulerAngles;

            mGadgetShelf.transform.position = myCamera.position;
            mGadgetShelf.transform.rotation = Quaternion.Euler(0.0f, myCamera.rotation.eulerAngles.y - 90.0f, 0.0f);

            // the -1 in below for loop prevents from spawning first file load file in middle of menu 
            for (int i = 0; i < shelfContainersPositions.Length - 1; i++)
            {
                StartCoroutine(ShiftGadgets(i));
            }
        }
    }

    private IEnumerator DelayHideShelf()
    {
        yield return new WaitForSeconds(0.3f);

        Gadget[] shelfGadgets = mGadgetShelf.GetComponentsInChildren<Gadget>();
        foreach (Gadget gadget in shelfGadgets)
        {
            GameManager.Instance.RightInteractNearTouch.ForceStopNearTouching(gadget.gameObject);
        }
        
        mGadgetShelf.SetActive(false);
    }

    private IEnumerator ShiftGadgets(int i)
    {
        float startTime = Time.time;
        float fraction = 0;
        float random_x = UnityEngine.Random.Range(0, GadgetOffsetMax);
        float random_y = UnityEngine.Random.Range(0, GadgetOffsetMax);
        Vector3 startingPosition = new Vector3(random_x + shelfRadius, random_y, 0);
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
        float startDegree_xz = -60.0f;
        float gap = 30f;
        int counter = 0;
        float y_pos = 0.0f;

        shelfContainersPositions = new Vector3[(int)GadgetInventory.NUM];

        for (int i = 0; i < (int)GadgetInventory.NUM; i++)
        {
            // Create container and store their position
            GameObject container = new GameObject("Container " + i.ToString());
            container.transform.SetParent(mGadgetShelf.transform);

            float container_pos_x = shelfRadius * Mathf.Cos(startDegree_xz * (float)Math.PI / 180);
            float container_pos_z = shelfRadius * Mathf.Sin(startDegree_xz * (float)Math.PI / 180);

            Vector3 container_pos = new Vector3(container_pos_x, y_pos, container_pos_z);
            container.transform.localPosition = container_pos;
            shelfContainersPositions[i] = container_pos;

            // Create bubble
            GameObject bubbleObj = Instantiate(BubblePrefab, container.transform);

            // Create gadget
            string gadgetName = ((GadgetInventory)i).ToString();

            SpawnSingleGadget(gadgetName, container.transform);

            startDegree_xz += gap;
            counter += 1;

            if (counter == 5)
            {
                counter = 0;
                startDegree_xz = -60.0f;
                if (y_pos >= 0)
                {
                    y_pos += 0.3f;
                    y_pos = -y_pos;
                }
                else
                {
                    y_pos = -y_pos;
                }
            }
        }

        // spawn small bubbles for saved files
        startDegree_xz = -20.0f;
        if (y_pos >= 0)
        {
            y_pos += 0.3f;
            y_pos = -y_pos;
        }
        else
        {
            y_pos = -y_pos;
        }

        for (int i = 0; i < 5; i++)
        {
            GameObject container = new GameObject("File Container " + i.ToString());
            container.transform.SetParent(mGadgetShelf.transform);

            float container_pos_x = shelfRadius * Mathf.Cos(startDegree_xz * (float)Math.PI / 180);
            float container_pos_z = Mathf.Sin(startDegree_xz * (float)Math.PI / 180);
            Vector3 container_pos = new Vector3(container_pos_x, y_pos, container_pos_z);
            container.transform.localPosition = container_pos;

            // load file bubble
            GameObject bubbleObj = Instantiate(BubblePrefab, container.transform);
            bubbleObj.transform.localScale *= 0.5f;

            // Create gadget
            string gadgetName = "LoadCube" ;
            LoadCube spawnedLoadCube = SpawnSingleGadget(gadgetName, container.transform) as LoadCube;
            spawnedLoadCube.Slot = i.ToString();

            startDegree_xz += 10.0f;
        }
    }

    private void RespawnGadgets()
    {
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

        gadget.MakeTransparent(true);
       
        return gadget;
    }

    public void InsertGadget(Gadget gadget)
    {
        gadget.gameObject.name = gadget.GetType().ToString() + gadgetsInWorld.Count.ToString();
        gadgetsInWorld.Add(gadget);
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

   
}